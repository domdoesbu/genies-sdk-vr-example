using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Genies.Sdk;
using Meta.XR.Movement.FaceTracking.Samples;
using Meta.XR.Movement.Retargeting;
using UnityEngine;

namespace Genies.VRExample
{
    public class GeniesAvatarControllerVR : MonoBehaviour
    {
        [SerializeField] private TextAsset _config;
        [SerializeField] private GeniesCharacterRetargeterForMeta _retargeter;
        [SerializeField] private MetaSourceDataProvider _metaSourceDataProvider;
        [SerializeField] private Shader _skinShaderWithInvisibleHeadSupport;

        private ManagedAvatar _avatar;

        private Material _invisible;

        private static readonly int HeadId = Shader.PropertyToID("_Head");
        private static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

        public void InitializeWithLoadedAvatar(ManagedAvatar avatar)
        {
            _avatar = avatar;

            _retargeter.gameObject.SetActive(true);
            _retargeter.ConfigAsset = _config;
            _retargeter.enabled = true;
            _metaSourceDataProvider.enabled = true;

            SkinnedMeshRenderer avatarRenderer = _avatar.ModelRoot.GetComponentInChildren<SkinnedMeshRenderer>();
            
            ApplyUpdatedSkinShader(avatarRenderer);

            HideHead(avatarRenderer);
        }

        private void LateUpdate() 
        {
            if (_avatar == null || _avatar.Root == null || _retargeter == null) return;
            // Match the avatar's root transform to the retargeter's transform. (Ideally this would be done automatically
            // via parenting, but I'm getting a weird bug when I try to spawn the avatar as a child of the retargeter.)
            _avatar.Root.transform.position = _retargeter.transform.position;
            _avatar.Root.transform.rotation = _retargeter.transform.rotation;

            if (!Application.isEditor)
            {
                _avatar.Root.transform.localScale = _retargeter.transform.localScale;
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
                if (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race"))
                {
                    materials[i] = _invisible;
                    continue;
                }

                // Switch off the head on the skin shader (requires the updated skin shader).
                if (materialName.Contains("skin"))
                {
                    mat.SetFloat(HeadId, 0f);
                    mat.SetFloat(AlphaClipId, 1f);

                    // If the shader gates alpha clipping behind a keyword, make sure it's enabled.
                    mat.EnableKeyword("_ALPHATEST_ON");
                }
            }

            // Assign back the same array you modified.
            avatarRenderer.materials = materials;
        }

        private Material CreateInvisibleMaterial()
        {
            Debug.Log("Finding invisible shader...");
            var shader = Shader.Find("Hidden/Genies/NoDraw_NoWrite_URP");
            Debug.Log(shader != null ? shader.name : "Shader not found!");
            var mat = new Material(shader);
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
