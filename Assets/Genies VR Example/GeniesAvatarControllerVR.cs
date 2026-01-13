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
        [SerializeField] private MetaSourceDataProvider _metaSourceDataProvider;
        [SerializeField] private Shader _skinShaderWithInvisibleHeadSupport;
        [SerializeField] private Camera _vrCamera;
        private ManagedAvatar _avatar;

        private HeadHider _headHider;

        private GeniesCharacterRetargeterForMeta _retargeter;

        private Vector3 _lastAppliedScale;
        private bool _hasAppliedScale;

        public void InitializeWithLoadedAvatar(ManagedAvatar avatar)
        {
            _avatar = avatar;

            _retargeter = this.gameObject.AddComponent<GeniesCharacterRetargeterForMeta>();

            if (Application.isEditor) 
            {
                // Don't apply root scale in Editor, otherwise we'll get a "zero" scale for some reason.
                _retargeter.SkeletonRetargeter.ApplyRootScale = false;
            }
            
            _metaSourceDataProvider.enabled = true;

            // This stuff isn't working on device, so we're just going to ignore it.
            SkinnedMeshRenderer avatarRenderer = _avatar.ModelRoot.GetComponentInChildren<SkinnedMeshRenderer>();
            ApplyUpdatedSkinShader(avatarRenderer);

            _headHider = new HeadHider(avatarRenderer);
            // Visible by default for all non-VR cameras.
            ShowHead(show: true);

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        // Public API to control head visibility at runtime.
        // show: true = show head, false = hide head.
        public void ShowHead(bool show)
        {
            _headHider?.ShowHead(show);
        }

        private void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        // private void Update() 
        // {
        //     if (_avatar == null || _avatar.Root == null || _retargeter == null) return;

        //     // Match the avatar's root transform to the retargeter's transform. (Ideally this would be done automatically
        //     // via parenting, but I'm getting a weird bug when I try to spawn the avatar as a child of the retargeter.)
        //     _avatar.Root.transform.position = _retargeter.transform.position;
        //     _avatar.Root.transform.rotation = _retargeter.transform.rotation;

        //     // Scaling is a different issue. For some reason, the _retargeter's scale in the Editor is (0, 0, 0), so only do it on device.
        //     if (Application.isEditor) return;

        //     // In order to not break hair/head dynamics stability, only apply scale changes that are significant enough.

        //     const float scaleThreshold = 0.01f;

        //     Vector3 targetScale = _retargeter.transform.localScale;

        //     if (!_hasAppliedScale ||
        //         Mathf.Abs(targetScale.x - _lastAppliedScale.x) > scaleThreshold ||
        //         Mathf.Abs(targetScale.y - _lastAppliedScale.y) > scaleThreshold ||
        //         Mathf.Abs(targetScale.z - _lastAppliedScale.z) > scaleThreshold)
        //     {
        //         _avatar.Root.transform.localScale = targetScale;
        //         _lastAppliedScale = targetScale;
        //         _hasAppliedScale = true;

        //         // DynamicsStructure prewarms itself in LateUpdate. If scale changes after it has been initialized,
        //         // request a prewarm so particles/links/colliders re-stabilize under the new scale.
        //         var dynamicsStructures = _avatar.Root.GetComponentsInChildren<DynamicsStructure>(includeInactive: true);
        //         for (int i = 0; i < dynamicsStructures.Length; i++)
        //         {
        //             var dynamicsStructure = dynamicsStructures[i];
        //             if (dynamicsStructure != null)
        //             {
        //                 dynamicsStructure.RequestPrewarmOnNextFrame();
        //             }
        //         }
        //     }
        // }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (!IsVrCamera(camera)) return;
            _headHider?.ShowHead(false);
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (!IsVrCamera(camera)) return;

            _headHider?.ShowHead(true);
        }

        private bool IsVrCamera(Camera camera)
        {
            if (_vrCamera == null) return false;
            return camera == _vrCamera;
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
