using UnityEngine;

public class Joystick : MonoBehaviour
{
    [SerializeField]
    private bool touchPad;  // Is this a TouchPad?

    [SerializeField]
    private Rect touchZone;

    [SerializeField]
    private float deadZone = 0;  // Control when position is output

    [SerializeField]
    private bool normalize = false;  // Normalize output after the dead-zone?

    [SerializeField]
    public Vector2 position;  // [-1, 1] in x,y

    private int tapCount;  // Current tap count

    private int lastFingerId = -1;  // Finger last used for this joystick
    private float tapTimeWindow;  // How much time there is left for a tap to occur
    private Vector2 fingerDownPos;
    private float fingerDownTime;
    private float firstDeltaTime = 0.5f;

    private GUITexture gui;  // Joystick graphic
    private Rect defaultRect;  // Default position / extents of the joystick graphic
    private Boundary guiBoundary = new Boundary();  // Boundary for joystick graphic
    private Vector2 guiTouchOffset;  // Offset to apply to touch input
    private Vector2 guiCenter;  // Center of joystick

    private static Joystick[] joysticks;  // A static collection of all joysticks
    private static bool enumeratedJoysticks = false;
    private static float tapTimeDelta = 0.3f;  // Time allowed between taps

    private class Boundary
    {
        public Vector2 min = Vector2.zero;
        public Vector2 max = Vector2.zero;
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        gui = GetComponent<GUITexture>();

        defaultRect = gui.pixelInset;

        defaultRect.x += transform.position.x * Screen.width;
        defaultRect.y += transform.position.y * Screen.height;

        transform.position = new Vector3(0.0f, 0.0f, transform.position.z);

        if (touchPad)
        {
            if (gui.texture)
                touchZone = defaultRect;
        }
        else
        {
            guiTouchOffset.x = defaultRect.width * 0.5f;
            guiTouchOffset.y = defaultRect.height * 0.5f;

            guiCenter.x = defaultRect.x + guiTouchOffset.x;
            guiCenter.y = defaultRect.y + guiTouchOffset.y;

            guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
            guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
            guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
            guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        enumeratedJoysticks = false;
    }

    public void ResetJoystick()
    {
        gui.pixelInset = defaultRect;
        lastFingerId = -1;
        position = Vector2.zero;
        fingerDownPos = Vector2.zero;

        if (touchPad)
            gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.025f);
    }

    public bool IsFingerDown()
    {
        return (lastFingerId != -1);
    }

    public void LatchedFinger(int fingerId)
    {
        if (lastFingerId == fingerId)
            ResetJoystick();
    }

    private void Update()
    {
        if (!enumeratedJoysticks)
        {
            joysticks = FindObjectsOfType<Joystick>();
            enumeratedJoysticks = true;
        }

        int count = Input.touchCount;

        if (tapTimeWindow > 0)
            tapTimeWindow -= Time.deltaTime;
        else
            tapCount = 0;

        if (count == 0)
        {
            ResetJoystick();
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 guiTouchPos = touch.position - guiTouchOffset;

                bool shouldLatchFinger = false;
                if (touchPad)
                {
                    if (touchZone.Contains(touch.position))
                        shouldLatchFinger = true;
                }
                else if (gui.HitTest(touch.position))
                {
                    shouldLatchFinger = true;
                }

                if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
                {
                    if (touchPad)
                    {
                        gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.15f);

                        lastFingerId = touch.fingerId;
                        fingerDownPos = touch.position;
                        fingerDownTime = Time.time;
                    }

                    lastFingerId = touch.fingerId;

                    if (tapTimeWindow > 0)
                    {
                        tapCount++;
                    }
                    else
                    {
                        tapCount = 1;
                        tapTimeWindow = tapTimeDelta;
                    }

                    for (int j = 0; j < joysticks.Length; j++)
                    {
                        Joystick joystick = joysticks[j];
                        if (joystick != null && joystick != this)
                            joystick.LatchedFinger(touch.fingerId);
                    }
                }

                if (lastFingerId == touch.fingerId)
                {
                    if (touch.tapCount > tapCount)
                        tapCount = touch.tapCount;

                    if (touchPad)
                    {
                        position.x = Mathf.Clamp((touch.position.x - fingerDownPos.x) / (touchZone.width / 2), -1, 1);
                        position.y = Mathf.Clamp((touch.position.y - fingerDownPos.y) / (touchZone.height / 2), -1, 1);
                    }
                    else
                    {
                        position.x = (touch.position.x - guiCenter.x) / guiTouchOffset.x;
                        position.y = (touch.position.y - guiCenter.y) / guiTouchOffset.y;
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        ResetJoystick();
                }
            }
        }

        float length = position.magnitude;

        if (length < deadZone)
        {
            position = Vector2.zero;
        }
        else
        {
            if (length > 1)
                position = position / length;
            else if (normalize)
                position = position / length * Mathf.InverseLerp(length, deadZone, 1);
        }

        //if (!touchPad)//TODO
        //{
        //    gui.pixelInset.x = (position.x - 1) * guiTouchOffset.x + guiCenter.x;
        //    gui.pixelInset.y = (position.y - 1) * guiTouchOffset.y + guiCenter.y;
        //}
    }
}
