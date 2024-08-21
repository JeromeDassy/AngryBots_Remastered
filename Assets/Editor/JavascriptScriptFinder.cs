using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class JavascriptScriptFinder
{
    [MenuItem("Tools/Find JavaScript Scripts")]
    private static void FindJavascriptScripts()
    {
        var monoScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
        var javascriptScripts = new List<MonoScript>();

        foreach (var monoScript in monoScripts)
        {
            if (IsJavascriptScript(monoScript))
            {
                javascriptScripts.Add(monoScript);
            }
        }

        if (javascriptScripts.Count > 0)
        {
            Debug.Log("JavaScript scripts found in the scene:");
            foreach (var javascriptScript in javascriptScripts)
            {
                Debug.Log(javascriptScript.name);
            }
        }
        else
        {
            Debug.Log("No JavaScript scripts found in the scene.");
        }
    }

    private static bool IsJavascriptScript(MonoScript monoScript)
    {
        var scriptType = monoScript.GetClass();
        return scriptType != null && IsJavascriptClassName(scriptType.Name);
    }

    private static bool IsJavascriptClassName(string className)
    {
        // Check if the class name ends with "JS" or "Js" or "js"
        return className.EndsWith("JS", StringComparison.OrdinalIgnoreCase)
            || className.EndsWith("Js", StringComparison.OrdinalIgnoreCase)
            || className.EndsWith("js", StringComparison.OrdinalIgnoreCase);
    }
}
