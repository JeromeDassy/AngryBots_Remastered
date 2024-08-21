using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ReBirth : MonoBehaviour
{
    private void Start()
    {
        AudioListener al = null;
        al = Camera.main.gameObject.GetComponent<AudioListener>();

        //if (al)
        //    al.volume = 1.0f;//TODO to fix

        ShaderDatabase sm = GetComponent<ShaderDatabase>();
        sm.WhiteIn();

        GetComponent<Camera>().backgroundColor = Color.white;
    }
}
