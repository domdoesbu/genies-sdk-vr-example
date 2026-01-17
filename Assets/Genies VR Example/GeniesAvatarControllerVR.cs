using System;
using System.Collections;
using System.Collections.Generic;
using Genies.Components.Dynamics;
using Genies.Sdk;
using Meta.XR.Movement.Retargeting;
using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.Rendering;

namespace Genies.VRExample
{
    [DefaultExecutionOrder(-1000)]
    public class GeniesAvatarControllerVR : MonoBehaviour
    {
        public bool IsAvatarLoaded => _avatar != null;
        [SerializeField] private MetaSourceDataProvider _metaSourceDataProvider;
        [SerializeField] private Shader _skinShaderWithInvisibleHeadSupport;
        [SerializeField] private Camera _vrCamera;
        [SerializeField] private OVRCameraRig _ovrCameraRig;
        [SerializeField] private FirstPersonLocomotor _firstPersonLocomotor;
        [SerializeField] private RuntimeAnimatorController _locomotionAnimatorController;

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

            _retargeter.SetUpForLocomotion(_ovrCameraRig.transform, _avatar, _firstPersonLocomotor, _locomotionAnimatorController);
            
            _metaSourceDataProvider.enabled = true;

            _headHider = new HeadHider(_avatar, _skinShaderWithInvisibleHeadSupport);
            
            // Visible by default for all non-VR cameras.
            _headHider.ShowHead(show: true);

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

            CacheDynamicsStructuresIfNeeded();
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
            if (!IsVrCamera(camera)) 
            {
                return;
            }

            _headHider.ShowHead(false);
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (!IsVrCamera(camera)) 
            {
                return;
            }

            _headHider.ShowHead(true);
        }

        private bool IsVrCamera(Camera camera)
        {
            if (_vrCamera == null) 
            {
                return false;
            }
            return camera == _vrCamera;
        }
    }   
}
