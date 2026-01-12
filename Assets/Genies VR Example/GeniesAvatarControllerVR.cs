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

        private HeadHider _headHider;

        private Vector3 _lastAppliedScale;
        private bool _hasAppliedScale;

        private bool _isHeadShown;

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

            _headHider = new HeadHider(avatarRenderer);
            ShowHead(show: false);
            _isHeadShown = false;
        }

        // Public API to control head visibility at runtime.
        // show: true = show head, false = hide head.
        public void ShowHead(bool show)
        {
            _headHider?.ShowHead(show);
        }

        private void Update() 
        {
            if (_avatar == null || _avatar.Root == null || _retargeter == null) return;

            // Match the avatar's root transform to the retargeter's transform. (Ideally this would be done automatically
            // via parenting, but I'm getting a weird bug when I try to spawn the avatar as a child of the retargeter.)
            _avatar.Root.transform.position = _retargeter.transform.position;
            _avatar.Root.transform.rotation = _retargeter.transform.rotation;

            // TEST: Toggle head visibility.
            if (ShouldToggleHeadThisFrame())
            {
                _isHeadShown = !_isHeadShown;
                ShowHead(_isHeadShown);
            }

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

        private static bool ShouldToggleHeadThisFrame()
        {
    #if UNITY_EDITOR
            return Input.GetMouseButtonDown(1);
    #else
            // Meta XR / Quest: use OVRInput (A on right controller / X on left controller).
            return OVRInput.GetDown(OVRInput.Button.One);
    #endif
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
