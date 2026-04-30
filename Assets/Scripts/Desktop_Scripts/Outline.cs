using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    public Color OutlineColor = Color.yellow;
    public float OutlineWidth = 5f;

    private Renderer[] renderers;
    private Material outlineMaterial;
    private bool isEnabled;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        Shader shader = Shader.Find("Hidden/Outline");
        outlineMaterial = new Material(shader);
        outlineMaterial.SetColor("_OutlineColor", OutlineColor);
        outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
    }

    public void SetOutline(bool enabled)
    {
        if (enabled == isEnabled) return;

        foreach (var rend in renderers)
        {
            var mats = rend.materials;

            if (enabled)
            {
                System.Array.Resize(ref mats, mats.Length + 1);
                mats[mats.Length - 1] = outlineMaterial;
            }
            else
            {
                if (mats.Length > 1)
                {
                    System.Array.Resize(ref mats, mats.Length - 1);
                }
            }

            rend.materials = mats;
        }

        isEnabled = enabled;
    }
}