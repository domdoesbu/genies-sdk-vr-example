using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Genies.VRExample
{
    public sealed class HeadHider
    {
        private static readonly int HeadId = Shader.PropertyToID("_Head");
        private static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

        private const string AlphaTestKeyword = "_ALPHATEST_ON";
        private const string InvisibleShaderName = "Hidden/Genies/NoDraw_NoWrite_URP";

        private readonly Renderer _renderer;

        private Material[] _originalMaterials;
        private Material _invisibleMaterial;

        public HeadHider(Renderer renderer)
        {
            _renderer = renderer;
        }

        public void ShowHead(bool show)
        {
            if (_renderer == null) return;

            if (_originalMaterials == null)
            {
                // Capture the starting point so we can restore later.
                // Using renderer.materials ensures we are working with instanced materials.
                var mats = _renderer.materials;
                _originalMaterials = (Material[])mats.Clone();
            }

            if (show)
            {
                // Restore original materials first (undo invisible swaps).
                _renderer.materials = (Material[])_originalMaterials.Clone();

                // Then ensure the skin materials are put back into the visible state.
                var materials = _renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (mat == null) continue;

                    var materialName = mat.name.ToLowerInvariant();
                    if (!materialName.Contains("skin")) continue;

                    mat.SetFloat(HeadId, 1f);

                    if (mat.HasProperty(AlphaClipId))
                    {
                        mat.SetFloat(AlphaClipId, 0f);
                    }

                    SetMaterialKeyword(mat, AlphaTestKeyword, enabled: false);
                }

                _renderer.materials = materials;
                return;
            }

            // Hide
            if (_invisibleMaterial == null)
            {
                _invisibleMaterial = CreateInvisibleMaterial();
            }

            var hideMaterials = _renderer.materials;

            for (int i = 0; i < hideMaterials.Length; i++)
            {
                var mat = hideMaterials[i];
                var materialName = mat != null ? mat.name.ToLowerInvariant() : string.Empty;

                // Swap out certain submeshes to an invisible material.
                if (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race") || materialName.Contains("hat"))
                {
                    hideMaterials[i] = _invisibleMaterial;
                    continue;
                }

                // Switch off the head on the skin shader.
                if (mat != null && materialName.Contains("skin"))
                {
                    mat.SetFloat(HeadId, 0f);

                    if (mat.HasProperty(AlphaClipId))
                    {
                        mat.SetFloat(AlphaClipId, 1f);
                    }

                    SetMaterialKeyword(mat, AlphaTestKeyword, enabled: true);
                }
            }

            _renderer.materials = hideMaterials;
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
