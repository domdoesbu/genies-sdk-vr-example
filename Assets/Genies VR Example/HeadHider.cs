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
        private static readonly int HeadId = Shader.PropertyToID("_Head");
        private static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

        private const string AlphaTestKeyword = "_ALPHATEST_ON";
        private const string InvisibleShaderName = "Hidden/Genies/NoDraw_NoWrite_URP";

        private readonly Renderer _renderer;
        private readonly SkinnedMeshRenderer _skinnedMeshRenderer;
        private readonly Shader _skinShaderWithInvisibleHeadSupport;
        private readonly Transform _skeletonRoot;

        private bool _materialsInitialized;
        private bool _scaleHideJointsInitialized;
        private bool _isHidden;

        private Material _invisibleMaterial;
        private Material[] _showMaterials;
        private Material[] _hideMaterials;
        private Material[] _skinMaterials;

        private Transform[] _scaleHideJoints;
        private Vector3[] _scaleHideJointOriginalScales;

        public HeadHider(ManagedAvatar avatar, Shader skinShaderWithInvisibleHeadSupport)
        {
            if (avatar != null && avatar.ModelRoot != null)
            {
                _skinnedMeshRenderer = avatar.ModelRoot.GetComponentInChildren<SkinnedMeshRenderer>();
                _renderer = _skinnedMeshRenderer;

                if (_skinnedMeshRenderer != null)
                {
                    // In SRP/XR, skinned meshes may not reflect transform changes made during rendering
                    // unless matrices are recalculated per render. (If we don't do this, the joints hidden by
                    // scaling to zero may not update properly).
                    _skinnedMeshRenderer.forceMatrixRecalculationPerRender = true;
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

            _skinShaderWithInvisibleHeadSupport = skinShaderWithInvisibleHeadSupport;
        }

        public void ShowHead(bool show)
        {
            EnsureScaleHideJointsInitialized();
            ApplyScaleHideJointState(show);

            if (_renderer == null)
            {
                return;
            }

            EnsureMaterialsInitialized();

            if (show)
            {
                if (!_isHidden) return;

                ApplySkinState(show: true);

                if (_showMaterials != null)
                {
                    _renderer.sharedMaterials = _showMaterials;
                }

                _isHidden = false;
                return;
            }

            if (_isHidden) return;

            ApplySkinState(show: false);

            if (_hideMaterials != null)
            {
                _renderer.sharedMaterials = _hideMaterials;
            }

            _isHidden = true;
        }

        private void EnsureMaterialsInitialized()
        {
            if (_materialsInitialized)
            {
                return;
            }

            if (_renderer == null)
            {
                return;
            }

            // Force instanced materials once (avoids allocations later).
            var instanced = _renderer.materials;

            ApplyUpdatedSkinShader(instanced);

            // Same rule here: mutate renderer.materials and assign back to renderer.materials.
            _renderer.materials = instanced;
            _showMaterials = instanced;

            if (_invisibleMaterial == null)
            {
                _invisibleMaterial = CreateInvisibleMaterial();
            }

            _hideMaterials = (Material[])instanced.Clone();

            var skinList = new List<Material>(capacity: instanced.Length);

            for (int i = 0; i < instanced.Length; i++)
            {
                var mat = instanced[i];
                var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                if (mat != null && materialName.Contains("skin"))
                {
                    skinList.Add(mat);
                }

                if (_invisibleMaterial != null &&
                    (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race")))
                {
                    _hideMaterials[i] = _invisibleMaterial;
                }
            }

            _skinMaterials = skinList.ToArray();

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

        private void ApplyUpdatedSkinShader(Material[] materials)
        {
            // Use a newer version of the skin shader, which includes visual and perf improvements,
            // and, crucially, support for an invisible head.

            if (materials == null)
            {
                return;
            }

            if (_skinShaderWithInvisibleHeadSupport == null)
            {
                return;
            }

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                if (materialName.Contains("skin") && mat != null && mat.shader != _skinShaderWithInvisibleHeadSupport)
                {
                    mat.shader = _skinShaderWithInvisibleHeadSupport;
                }
            }
        }

        private void ApplySkinState(bool show)
        {
            if (_skinMaterials == null) return;

            for (int i = 0; i < _skinMaterials.Length; i++)
            {
                var mat = _skinMaterials[i];
                if (mat == null) continue;

                mat.SetFloat(HeadId, show ? 1f : 0f);

                if (mat.HasProperty(AlphaClipId))
                {
                    mat.SetFloat(AlphaClipId, show ? 0f : 1f);
                }

                SetMaterialKeyword(mat, AlphaTestKeyword, enabled: !show);
            }
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
