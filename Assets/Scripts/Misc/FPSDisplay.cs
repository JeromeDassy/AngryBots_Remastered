using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime;
    private float currentFPS;

    private void Update()
    {
        // Calculate the frame time and FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        currentFPS = 1.0f / deltaTime;
    }

    private void OnGUI()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        int fontSize = 18;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleRight;
        style.fontSize = fontSize;
        style.normal.textColor = Color.green; // Set the text color to black
        GUI.Label(new Rect(screenWidth - 150, screenHeight - fontSize, 150, fontSize), "FPS: " + Mathf.Round(currentFPS), style);
    }
}
