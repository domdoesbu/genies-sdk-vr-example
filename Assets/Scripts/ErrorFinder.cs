using UnityEditor;
using UnityEngine;

public class ErrorFinder
{
    [MenuItem("Tools/Find Broken Components")]
    static void Find()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log($"Missing component on: {go.name}", go);
                }
            }
        }

        Debug.Log("Search complete.");
    }
}
