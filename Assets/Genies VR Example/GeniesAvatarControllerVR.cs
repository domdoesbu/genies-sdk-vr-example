using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Genies.Components.Dynamics;
using Genies.Sdk;
using Meta.XR.Movement.FaceTracking.Samples;
using Meta.XR.Movement.Retargeting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Genies.VRExample
{
    [DefaultExecutionOrder(-1000)]
    public class GeniesAvatarControllerVR : MonoBehaviour
    {
        [SerializeField] private TextAsset _config;
        [SerializeField] private GeniesCharacterRetargeterForMeta _retargeter;
        [SerializeField] private MetaSourceDataProvider _metaSourceDataProvider;
        [SerializeField] private Shader _skinShaderWithInvisibleHeadSupport;

        private ManagedAvatar _avatar;

        private Material _invisible;

        private Vector3 _lastAppliedScale;
        private bool _hasAppliedScale;

        private static readonly int HeadId = Shader.PropertyToID("_Head");
        private static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

        private const string AlphaTestKeyword = "_ALPHATEST_ON";

        public void InitializeWithLoadedAvatar(ManagedAvatar avatar)
        {
            _avatar = avatar;

            _retargeter.gameObject.SetActive(true);
            _retargeter.ConfigAsset = _config;
            _retargeter.enabled = true;
            _metaSourceDataProvider.enabled = true;

            // This stuff isn't working on device, so we're just going to ignore it.
            SkinnedMeshRenderer avatarRenderer = _avatar.ModelRoot.GetComponentInChildren<SkinnedMeshRenderer>();
            ApplyUpdatedSkinShader(avatarRenderer);
            HideHead(avatarRenderer);
        }

        private void Update() 
        {
            if (_avatar == null || _avatar.Root == null || _retargeter == null) return;

            // Match the avatar's root transform to the retargeter's transform. (Ideally this would be done automatically
            // via parenting, but I'm getting a weird bug when I try to spawn the avatar as a child of the retargeter.)
            _avatar.Root.transform.position = _retargeter.transform.position;
            _avatar.Root.transform.rotation = _retargeter.transform.rotation;

            // Scaling is a different issue. For some reason, the _retargeter's scale in the Editor is (0, 0, 0), so only do it on device.

            if (Application.isEditor) return;

            // In order to not break hair/head dynamics stability, only apply scale changes that are significant enough.

            const float scaleThreshold = 0.01f;

            Vector3 targetScale = _retargeter.transform.localScale;

            if (!_hasAppliedScale ||
                Mathf.Abs(targetScale.x - _lastAppliedScale.x) > scaleThreshold ||
                Mathf.Abs(targetScale.y - _lastAppliedScale.y) > scaleThreshold ||
                Mathf.Abs(targetScale.z - _lastAppliedScale.z) > scaleThreshold)
            {
                _avatar.Root.transform.localScale = targetScale;
                _lastAppliedScale = targetScale;
                _hasAppliedScale = true;

                // DynamicsStructure prewarms itself in LateUpdate. If scale changes after it has been initialized,
                // request a prewarm so particles/links/colliders re-stabilize under the new scale.
                var dynamicsStructures = _avatar.Root.GetComponentsInChildren<DynamicsStructure>(includeInactive: true);
                for (int i = 0; i < dynamicsStructures.Length; i++)
                {
                    var dynamicsStructure = dynamicsStructures[i];
                    if (dynamicsStructure != null)
                    {
                        dynamicsStructure.RequestPrewarmOnNextFrame();
                    }
                }
            }
        }

        private void HideHead(Renderer avatarRenderer)
        {
            // Show or hide the avatar's head to prevent rendering issues in VR.
            
            if (avatarRenderer == null)
            {
                Debug.LogError("Could not find SkinnedMeshRenderer on avatar to show/hide head.");
                return;
            }

            if (_invisible == null)
            {
                _invisible = CreateInvisibleMaterial();
            }

            // IMPORTANT: use renderer.materials (instanced) consistently when you intend to mutate per-renderer values.
            var materials = avatarRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                // Swap out certain submeshes to an invisible material.
                if (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race") || materialName.Contains("hat"))
                {
                    materials[i] = _invisible;
                    continue;
                }

                // Switch off the head on the skin shader (requires the updated skin shader).
                if (materialName.Contains("skin"))
                {
                    mat.SetFloat(HeadId, 0f);
                    if (mat.HasProperty(AlphaClipId))
                    {
                        mat.SetFloat(AlphaClipId, 1f);
                    }

                    // If the shader gates alpha clipping behind a keyword, make sure it's enabled.
                    SetMaterialKeyword(mat, AlphaTestKeyword, enabled: true);

                    Debug.Log("Head: " + mat.GetFloat(HeadId));
                    Debug.Log("AlphaClip property exists: " + mat.HasProperty(AlphaClipId));
                    if (mat.HasProperty(AlphaClipId))
                    {
                        Debug.Log("AlphaClip: " + mat.GetFloat(AlphaClipId));
                    }
                    Debug.Log("AlphaTest keyword enabled: " + mat.IsKeywordEnabled(AlphaTestKeyword));
                    Debug.Log("Keywords: " + string.Join(", ", mat.shaderKeywords));
                }
            }

            // Assign back the same array you modified.
            avatarRenderer.materials = materials;
        }

        private static void SetMaterialKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null || material.shader == null || string.IsNullOrWhiteSpace(keyword)) return;

            // ShaderGraph/URP commonly uses local keywords (shader_feature_local). Using LocalKeyword avoids
            // silently toggling the wrong keyword set on some platforms.
            try
            {
                var localKeyword = new LocalKeyword(material.shader, keyword);
                material.SetKeyword(localKeyword, enabled);
            }
            catch
            {
                if (enabled) material.EnableKeyword(keyword);
                else material.DisableKeyword(keyword);
            }
        }

        private Material CreateInvisibleMaterial()
        {
            // Prefer a serialized reference to avoid build-time shader stripping.
            var shader = Shader.Find("Hidden/Genies/NoDraw_NoWrite_URP");

            if (shader == null)
            {
                Debug.LogError("[GeniesAvatarControllerVR] Could not find shader: 'Hidden/Genies/NoDraw_NoWrite_URP'.");
                return null;
            }

            if (!shader.isSupported)
            {
                Debug.LogError($"[GeniesAvatarControllerVR] Shader is not supported on this device ('{shader.name}'). Expect magenta.");
            }

            var mat = new Material(shader)
            {
                name = "GeniesVR_NoDrawNoWrite"
            };

            return mat;
        }

        private void ApplyUpdatedSkinShader(Renderer avatarRenderer)
        {
            // Use a newer version of the skin shader, which includes visual and perf improvements,
            // and, crucially, support for an invisible head.
    
            if (avatarRenderer == null)
            {
                Debug.LogError("Could not find SkinnedMeshRenderer on avatar to update skin shader.");
                return;
            }

            // Same rule here: mutate renderer.materials and assign back to renderer.materials.
            var materials = avatarRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                if (materialName.Contains("skin") && mat.shader != _skinShaderWithInvisibleHeadSupport)
                {
                    mat.shader = _skinShaderWithInvisibleHeadSupport;
                }
            }

            avatarRenderer.materials = materials;
        }
    }   
}
