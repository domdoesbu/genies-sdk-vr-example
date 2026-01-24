using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Genies.Sdk;

namespace Genies.VRExample
{
    public sealed class HeadHider
    {
        private static readonly string[] ScaleHideJointNames = { "FaceBind" };
        private const string InvisibleShaderName = "Hidden/Genies/NoDraw_NoWrite_URP";
        private const string HeadRendererName = "unified-face";
        private const string BodyRendererName = "Default";

        private readonly SkinnedMeshRenderer _headRenderer;
        private readonly SkinnedMeshRenderer _bodyRenderer;
        private readonly Transform _skeletonRoot;

        private bool _materialsInitialized;
        private bool _scaleHideJointsInitialized;
        private bool _isHidden;

        private Material _invisibleMaterial;
        private Material[] _headOriginalMaterials;
        private Material[] _bodyOriginalMaterials;
        private int[] _bodyHairMaterialIndices;

        private Transform[] _scaleHideJoints;
        private Vector3[] _scaleHideJointOriginalScales;

        public HeadHider(ManagedAvatar avatar)
        {
            if (avatar != null && avatar.ModelRoot != null)
            {
                var allRenderers = avatar.ModelRoot.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (var renderer in allRenderers)
                {
                    if (renderer.name == HeadRendererName)
                    {
                        _headRenderer = renderer;
                    }
                    else if (renderer.name == BodyRendererName)
                    {
                        _bodyRenderer = renderer;
                    }

                    if (renderer != null)
                    {
                        // In SRP/XR, skinned meshes may not reflect transform changes made during rendering
                        // unless matrices are recalculated per render. (If we don't do this, the joints hidden by
                        // scaling to zero may not update properly).
                        renderer.forceMatrixRecalculationPerRender = true;
                    }
                }

                if (_headRenderer == null)
                {
                    Debug.LogWarning($"[HeadHider] Could not find head renderer '{HeadRendererName}'.");
                }

                if (_bodyRenderer == null)
                {
                    Debug.LogWarning($"[HeadHider] Could not find body renderer '{BodyRendererName}'.");
                }
            }
            else 
            {
                Debug.LogError("[HeadHider] Avatar or ModelRoot is null. Cannot initialize HeadHider.");
            }

            if (avatar != null)
            {
                _skeletonRoot = avatar.SkeletonRoot;
            }
        }

        public void ShowHead(bool show)
        {
            EnsureScaleHideJointsInitialized();
            ApplyScaleHideJointState(show);

            EnsureMaterialsInitialized();

            if (show)
            {
                if (!_isHidden)
                {
                    return;
                }

                if (_headRenderer != null && _headOriginalMaterials != null)
                {
                    _headRenderer.sharedMaterials = _headOriginalMaterials;
                }

                if (_bodyRenderer != null && _bodyOriginalMaterials != null)
                {
                    _bodyRenderer.sharedMaterials = _bodyOriginalMaterials;
                }

                _isHidden = false;
                return;
            }

            if (_isHidden)
            {
                return;
            }

            if (_headRenderer != null && _headOriginalMaterials != null && _invisibleMaterial != null)
            {
                var hideMaterials = new Material[_headOriginalMaterials.Length];
                for (int i = 0; i < hideMaterials.Length; i++)
                {
                    hideMaterials[i] = _invisibleMaterial;
                }
                _headRenderer.sharedMaterials = hideMaterials;
            }

            if (_bodyRenderer != null && _bodyOriginalMaterials != null && _invisibleMaterial != null && _bodyHairMaterialIndices != null)
            {
                var hideMaterials = (Material[])_bodyOriginalMaterials.Clone();
                for (int i = 0; i < _bodyHairMaterialIndices.Length; i++)
                {
                    int index = _bodyHairMaterialIndices[i];
                    if (index >= 0 && index < hideMaterials.Length)
                    {
                        hideMaterials[index] = _invisibleMaterial;
                    }
                }
                _bodyRenderer.sharedMaterials = hideMaterials;
            }

            _isHidden = true;
        }

        private void EnsureMaterialsInitialized()
        {
            if (_materialsInitialized)
            {
                return;
            }

            if (_invisibleMaterial == null)
            {
                _invisibleMaterial = CreateInvisibleMaterial();
            }

            if (_headRenderer != null)
            {
                _headOriginalMaterials = _headRenderer.sharedMaterials;
            }

            if (_bodyRenderer != null)
            {
                _bodyOriginalMaterials = _bodyRenderer.sharedMaterials;

                var hairIndices = new List<int>();
                for (int i = 0; i < _bodyOriginalMaterials.Length; i++)
                {
                    var mat = _bodyOriginalMaterials[i];
                    var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                    if (materialName.Contains("hair"))
                    {
                        hairIndices.Add(i);
                    }
                }

                _bodyHairMaterialIndices = hairIndices.ToArray();
            }

            _materialsInitialized = true;
        }

        private void EnsureScaleHideJointsInitialized()
        {
            if (_scaleHideJointsInitialized)
            {
                return;
            }

            if (_skeletonRoot == null)
            {
                _scaleHideJointsInitialized = true;
                _scaleHideJoints = Array.Empty<Transform>();
                _scaleHideJointOriginalScales = Array.Empty<Vector3>();
                return;
            }

            var joints = new List<Transform>();
            var originalScales = new List<Vector3>();

            Transform[] all = _skeletonRoot.GetComponentsInChildren<Transform>(includeInactive: true);

            for (int i = 0; i < all.Length; i++)
            {
                Transform t = all[i];
                if (t == null)
                {
                    continue;
                }

                for (int j = 0; j < ScaleHideJointNames.Length; j++)
                {
                    string jointName = ScaleHideJointNames[j];
                    if (string.IsNullOrEmpty(jointName))
                    {
                        continue;
                    }

                    if (t.name == jointName)
                    {
                        joints.Add(t);
                        originalScales.Add(t.localScale);
                        break;
                    }
                }
            }

            _scaleHideJoints = joints.ToArray();
            _scaleHideJointOriginalScales = originalScales.ToArray();
            _scaleHideJointsInitialized = true;
        }

        private void ApplyScaleHideJointState(bool show)
        {
            if (_scaleHideJoints == null || _scaleHideJointOriginalScales == null)
            {
                return;
            }

            int count = _scaleHideJoints.Length;
            if (_scaleHideJointOriginalScales.Length < count)
            {
                count = _scaleHideJointOriginalScales.Length;
            }

            for (int i = 0; i < count; i++)
            {
                Transform t = _scaleHideJoints[i];
                if (t == null)
                {
                    continue;
                }

                t.localScale = show ? _scaleHideJointOriginalScales[i] : Vector3.zero;
            }
        }



        private static Material CreateInvisibleMaterial()
        {
            var shader = Shader.Find(InvisibleShaderName);

            if (shader == null)
            {
                Debug.LogError($"[HeadHider] Could not find shader: '{InvisibleShaderName}'.");
                return null;
            }

            if (!shader.isSupported)
            {
                Debug.LogError($"[HeadHider] Shader is not supported on this device ('{shader.name}'). Expect magenta.");
            }

            return new Material(shader)
            {
                name = "GeniesVR_NoDrawNoWrite"
            };
        }
    }
}
