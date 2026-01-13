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

        private Vector3 _lastObservedScale;
        private bool _hasObservedScale;

        private DynamicsStructure[] _cachedDynamicsStructures;
        private bool _hasCachedDynamicsStructures;
        private Transform _cachedAvatarRootTransform;

        public void InitializeWithLoadedAvatar(ManagedAvatar avatar)
        {
            _avatar = avatar;

            _cachedDynamicsStructures = null;
            _hasCachedDynamicsStructures = false;
            _cachedAvatarRootTransform = null;

            _hasObservedScale = false;
            _lastObservedScale = default;

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

            CacheDynamicsStructuresIfNeeded();
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

        private void Update() 
        {
            // HACK: To prevent hair Dynamics from breaking on device, we need to prewarm them each time a new scale is observed. 

            if (Application.isEditor)
            {
                return;
            }

            if (_avatar == null || _avatar.Root == null || _retargeter == null)
            {
                return;
            }

            CacheDynamicsStructuresIfNeeded();

            Vector3 currentScale = transform.localScale;
            bool hasScaleChanged = !_hasObservedScale || currentScale != _lastObservedScale;

            if (hasScaleChanged)
            {
                _lastObservedScale = currentScale;
                _hasObservedScale = true;

                // DynamicsStructure prewarms itself in LateUpdate. If scale changes after it has been initialized,
                // request a prewarm so particles/links/colliders re-stabilize under the new scale.
                if (_cachedDynamicsStructures != null)
                {
                    for (int i = 0; i < _cachedDynamicsStructures.Length; i++)
                    {
                        DynamicsStructure dynamicsStructure = _cachedDynamicsStructures[i];
                        if (dynamicsStructure != null)
                        {
                            dynamicsStructure.RequestPrewarmOnNextFrame();
                        }
                    }
                }
            }
        }

        private void CacheDynamicsStructuresIfNeeded()
        {
            if (_avatar == null || _avatar.Root == null)
            {
                return;
            }

            Transform avatarRootTransform = _avatar.Root.transform;

            if (_hasCachedDynamicsStructures && _cachedAvatarRootTransform == avatarRootTransform)
            {
                return;
            }

            _cachedAvatarRootTransform = avatarRootTransform;
            _cachedDynamicsStructures = _avatar.Root.GetComponentsInChildren<DynamicsStructure>(includeInactive: true);
            _hasCachedDynamicsStructures = true;
        }

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
