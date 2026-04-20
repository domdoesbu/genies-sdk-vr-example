using UnityEngine;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

namespace Genies.VRExample
{
    [ExecuteAlways]
    public class Mirror : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public Camera _mirrorCamera;
        [SerializeField] private Renderer _displayRenderer;

        [Header("Render Texture")]
        [SerializeField, Min(16)] public int _targetTextureHeight = 720;
        [SerializeField, Min(1)] private int _maxTextureSize = 4096;
        [SerializeField] private FilterMode _filterMode = FilterMode.Bilinear;
        [Header("Sharing")]
        [SerializeField] private bool _useSharedTexture = false;
        [SerializeField] private Mirror _sourceMirror;
        [Header("Behavior")]
        [SerializeField] private bool _matchDisplayAspect = true;

        private RenderTexture _renderTexture;
        private Material _materialInstance;

        private int _currentWidth;
        private int _currentHeight;

        private void Reset()
        {
            AutoAssignReferences();
            EnsureMaterial();
            EnsureRenderTexture();
        }

        private void OnEnable()
        {
            AutoAssignReferences();
            EnsureMaterial();
            EnsureRenderTexture();
            ConfigureMirrorCamera();
            ApplyLinks();
        }

        private void OnDisable()
        {
            ReleaseResources();
        }

        private void OnDestroy()
        {
            ReleaseResources();
        }

        private void OnValidate()
        {
            _targetTextureHeight = Mathf.Max(16, _targetTextureHeight);
            _maxTextureSize = Mathf.Max(1, _maxTextureSize);

            if (!isActiveAndEnabled)
                return;

            AutoAssignReferences();
            EnsureMaterial();
            EnsureRenderTexture();
            ConfigureMirrorCamera();
            ApplyLinks();
        }

        private void ConfigureMirrorCamera()
        {
            if (_mirrorCamera == null)
                return;

            // A mirror RenderTexture is a 2D display; forcing non-stereo avoids XR-specific rendering paths
            // (and their shader keyword/variant requirements) that commonly differ on device.
            _mirrorCamera.stereoTargetEye = StereoTargetEyeMask.None;

#if UNITY_RENDER_PIPELINE_UNIVERSAL
            var additionalData = _mirrorCamera.GetUniversalAdditionalCameraData();
            if (additionalData != null)
            {
                additionalData.allowXRRendering = false;
            }
#endif
        }

        private void Update()
        {
            if (_mirrorCamera == null || _displayRenderer == null)
                return;

            if (_matchDisplayAspect)
            {
                int desiredWidth;
                int desiredHeight;
                ComputeDesiredTextureSize(out desiredWidth, out desiredHeight);

                if (desiredWidth != _currentWidth || desiredHeight != _currentHeight)
                {
                    EnsureRenderTexture();
                    ApplyLinks();
                }
            }
        }

        private void AutoAssignReferences()
        {
            if (_mirrorCamera == null)
                _mirrorCamera = GetComponentInChildren<Camera>(includeInactive: true);

            if (_displayRenderer == null)
            {
                // Prefer a quad child if present.
                var renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null)
                        continue;

                    if (renderers[i].gameObject == gameObject)
                        continue;

                    _displayRenderer = renderers[i];
                    break;
                }
            }
        }

        private void EnsureMaterial()
        {
            if (_displayRenderer == null)
                return;

            if (_materialInstance != null)
                return;

            // Use a simple unlit shader so the render texture appears as-is.
            Shader shader = Shader.Find("Unlit/Texture");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Unlit");

            if (shader == null)
                shader = Shader.Find("Standard");

            _materialInstance = new Material(shader)
            {
                name = "Mirror (Runtime)"
            };

            _displayRenderer.sharedMaterial = _materialInstance;
        }

        private void EnsureRenderTexture()
        {
            if (_useSharedTexture && _sourceMirror != null)
            {
                _renderTexture = _sourceMirror._renderTexture;
                _currentWidth = _renderTexture != null ? _renderTexture.width : 0;
                _currentHeight = _renderTexture != null ? _renderTexture.height : 0;
                return;
            }
            if (_mirrorCamera == null)
                return;

            int desiredWidth;
            int desiredHeight;
            ComputeDesiredTextureSize(out desiredWidth, out desiredHeight);

            if (_renderTexture != null && _renderTexture.width == desiredWidth && _renderTexture.height == desiredHeight)
            {
                _currentWidth = desiredWidth;
                _currentHeight = desiredHeight;
                return;
            }

            ReleaseRenderTextureOnly();

            _renderTexture = new RenderTexture(desiredWidth, desiredHeight, 24, RenderTextureFormat.Default)
            {
                name = "MirrorRT (Runtime)",
                filterMode = _filterMode,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
                autoGenerateMips = false
            };
            _renderTexture.Create();

            _currentWidth = desiredWidth;
            _currentHeight = desiredHeight;
        }

        private void ApplyLinks()
        {
            if (!_useSharedTexture && _mirrorCamera != null)
            {
                _mirrorCamera.targetTexture = _renderTexture;

                if (_renderTexture != null)
                    _mirrorCamera.aspect = (float)_renderTexture.width / _renderTexture.height;
            }

            if (_mirrorCamera != null)
            {
                _mirrorCamera.targetTexture = _renderTexture;

                if (_renderTexture != null)
                    _mirrorCamera.aspect = (float)_renderTexture.width / _renderTexture.height;
            }

            if (_displayRenderer != null)
            {
                EnsureMaterial();

                if (_materialInstance != null)
                    _materialInstance.mainTexture = _renderTexture;
            }
        }

        private void ComputeDesiredTextureSize(out int width, out int height)
        {
            height = Mathf.Clamp(_targetTextureHeight, 16, _maxTextureSize);
            float aspect = 1f;

            if (_matchDisplayAspect)
                aspect = Mathf.Max(0.0001f, GetDisplayAspect());

            width = Mathf.RoundToInt(height * aspect);
            width = Mathf.Clamp(width, 16, _maxTextureSize);

            // If clamping width changed the actual aspect too much, adjust height down to match.
            if (_matchDisplayAspect && width == _maxTextureSize)
            {
                height = Mathf.Clamp(Mathf.RoundToInt(width / aspect), 16, _maxTextureSize);
            }
        }

        private float GetDisplayAspect()
        {
            if (_displayRenderer == null)
                return 1f;

            // Best-effort: use mesh bounds and local scale to infer width/height.
            Vector2 meshSize = Vector2.one;
            var meshFilter = _displayRenderer.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                var size = meshFilter.sharedMesh.bounds.size;
                meshSize = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            }

            Vector3 scale = _displayRenderer.transform.lossyScale;
            float w = Mathf.Abs(meshSize.x * scale.x);
            float h = Mathf.Abs(meshSize.y * scale.y);

            if (h <= 0.0001f)
                return 1f;

            return w / h;
        }

        private void ReleaseResources()
        {
            ReleaseRenderTextureOnly();

            if (_materialInstance != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(_materialInstance);
                else
#endif
                    Destroy(_materialInstance);

                _materialInstance = null;
            }
        }

        private void ReleaseRenderTextureOnly()
        {
            if (_mirrorCamera != null && _mirrorCamera.targetTexture == _renderTexture)
                _mirrorCamera.targetTexture = null;

            if (_renderTexture != null)
            {
                _renderTexture.Release();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(_renderTexture);
                else
#endif
                    Destroy(_renderTexture);

                _renderTexture = null;
            }

            _currentWidth = 0;
            _currentHeight = 0;
        }
    }
}
