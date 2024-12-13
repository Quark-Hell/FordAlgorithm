using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private Canvas canvas;

    private float doubleClickThreshold = 0.2f;
    private float lastClickTime = -1f;

    public Vector2 mouseWorldPos { get; private set; }
    public Vector2 mouseScreenPos { get; private set; }

    public Vector2 startPos { get; private set; }
    public Vector2 currentPos { get; private set; }

    public void Init(Canvas cnv)
    {
        canvas = cnv;
    }

    public void StartRecord()
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPos = cursorWorldPosition;
        currentPos = cursorWorldPosition;
    }
    public void UpdateRecord()
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos = cursorWorldPosition;
    }

    void Start()
    {
        Vector2 worldPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition, canvas.worldCamera,
            out worldPos);

        mouseWorldPos = worldPos;
        mouseScreenPos = Camera.main.WorldToScreenPoint(mouseWorldPos);
    }

    void Update()
    {
        Vector2 worldPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition, canvas.worldCamera,
            out worldPos);

        mouseWorldPos = worldPos;
        mouseScreenPos = Camera.main.WorldToScreenPoint(mouseWorldPos);
    }

    public bool DoubleClick()
    {
        float currentTime = Time.time;
        if (lastClickTime > 0 && currentTime - lastClickTime <= doubleClickThreshold)
        {
            return true;
        }
        lastClickTime = currentTime;
        return false;
    }
}
