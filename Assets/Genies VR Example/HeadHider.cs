using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Genies.VRExample
{
    public sealed class HeadHider
    {
        private static readonly int HeadId = Shader.PropertyToID("_Head");
        private static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

        private const string AlphaTestKeyword = "_ALPHATEST_ON";
        private const string InvisibleShaderName = "Hidden/Genies/NoDraw_NoWrite_URP";

        private readonly Renderer _renderer;

        private bool _initialized;
        private bool _isHidden;

        private Material _invisibleMaterial;
        private Material[] _showMaterials;
        private Material[] _hideMaterials;
        private Material[] _skinMaterials;

        public HeadHider(Renderer renderer)
        {
            _renderer = renderer;
        }

        public void ShowHead(bool show)
        {
            if (_renderer == null) return;

            EnsureInitialized();

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

        private void EnsureInitialized()
        {
            if (_initialized) return;

            // Force instanced materials once (avoids allocations later).
            var instanced = _renderer.materials;
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
                    (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race") || materialName.Contains("hat")))
                {
                    _hideMaterials[i] = _invisibleMaterial;
                }
            }

            _skinMaterials = skinList.ToArray();
            _initialized = true;
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
