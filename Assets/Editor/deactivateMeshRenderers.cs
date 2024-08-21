using UnityEditor;
using UnityEngine;

public class deactivateMeshRenderers
{
    [MenuItem("GameObject/Deactivate Renderers")]
    static void DeactivateRenderers()
    {
        var selectedGameObjects = Selection.gameObjects;

        foreach (var gameObject in selectedGameObjects)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    [MenuItem("GameObject/Activate Renderers")]
    static void ActivateRenderers()
    {
        var selectedGameObjects = Selection.gameObjects;

        foreach (var gameObject in selectedGameObjects)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
    }
}
