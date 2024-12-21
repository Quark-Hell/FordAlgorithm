using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Data
{
    public GameObject selectedObject;
    public Graph graph = new();
    public GameObject[] selectedNode = new GameObject[2];
}

public class Controller : MonoBehaviour,
    IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Canvas parentCanvas;
    [SerializeField]
    private Sprite vertexSprite;
    [SerializeField]
    private Sprite arrowSprite;
    [SerializeField]
    private Material edgeMaterial;

    private Vector2 mousePos;
    private Data data = new();

    [SerializeField]
    private Notification notification;

    void Start()
    {
        data.graph.ford.noti = notification;
        data.selectedNode[0] = null;
        data.selectedNode[1] = null;

        data.graph.sprite = vertexSprite;
        data.graph.rectParent = transform;
        data.graph.arrow = arrowSprite;
        data.graph.edgeMat = edgeMaterial;
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out mousePos);

        InputFunc();
    }


    public void RemoveAll()
    {
        data.selectedObject = null;
        data.selectedNode[0] = null;
        data.selectedNode[1] = null;
        data.graph.RemoveAll();
    }
    public void Save()
    {
        string path = "Saves";
        string screenName = GetUniqueScreenshotName(path, "screenshot_", "png");
        string saveName = GetUniqueScreenshotName(path, "save_", "json");

        string fullPathScreenshot = Path.Combine(path, screenName);
        string fullPathSave = Path.Combine(path, saveName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        TakeScreenshot(fullPathScreenshot);
        data.graph.Serialize(fullPathSave);
    }

    private void TakeScreenshot(string path)
    {
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("A screenshot was taken!");
        notification.Show("Данные сохранены");
    }

    string GetUniqueScreenshotName(string path, string name, string extension)
    {
        int counter = 0;
        string fileName;
        string fullPath;

        do
        {
            fileName = $"{name}{counter}.{extension}";
            fullPath = Path.Combine(path, fileName);
            counter++;
        }
        while (File.Exists(fullPath));

        return fileName;
    }

    public void Load(string path)
    {
        data.selectedObject = null;
        data.selectedNode[0] = null;
        data.selectedNode[1] = null;
        data.graph.Deserialize(path);
    }

    private GameObject startObject = null;
    public void StartAlg()
    {
        DeHighlight();

        Node node = data.selectedObject.GetComponent<Node>();
        startObject = data.selectedObject;

        Highlight();
        data.graph.StartAlgh(node);
    }

    private void DeHighlight()
    {
        if (startObject == null) { return; }
        RectTransform rectTransform = startObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1.0f, 1.0f);
        SetColor(startObject, Color.white);
        startObject = null;
    }

    private void Highlight()
    {
        if (startObject == null) { return; }
        RectTransform rectTransform = startObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1.2f,1.2f);
        SetColor(startObject, Color.red);
    }

    private void InputFunc()
    {
        if (data.selectedNode == null) { return; }

        if (Input.GetKeyUp(KeyCode.Delete))
        {
            SetColor(data.selectedNode[0], Color.white);
            SetColor(data.selectedNode[1], Color.white);

            data.graph.RemoveNode(data.selectedObject);
            data.selectedObject = null;
            data.selectedNode[0] = null;
            data.selectedNode[1] = null;
        }
    }

    private bool InSection(Vector2 widthBorder, Vector2 heightBorder)
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorScreenPosition = Camera.main.WorldToScreenPoint(cursorWorldPosition);

        if (!(cursorScreenPosition.x > widthBorder.x)) { return false; }
        if (!(cursorScreenPosition.x < widthBorder.y)) { return false; }

        if (!(cursorScreenPosition.y > heightBorder.x)) { return false; }
        if (!(cursorScreenPosition.y < heightBorder.y)) { return false; }

        return true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            if (data.selectedObject == null) { return; }

            Vector2 widthBorder;
            Vector2 heightBorder;

            widthBorder.x = (Screen.width / 40);
            widthBorder.y = Screen.width - (Screen.width / 40);

            heightBorder.x = (Screen.height / 24);
            heightBorder.y = Screen.height - (Screen.height / 6f);

            if (!InSection(widthBorder, heightBorder)) { return; }

            Vector2 pos = mousePos;
            pos.y += 50;
            data.graph.SetNodePosition(data.selectedObject, pos);
        }
    }

    private void SetColor(GameObject selectedObject, Color color)
    {
        if (selectedObject == null) { return; }
        Image img = selectedObject.GetComponent<Image>();
        img.color = color;
    }
    private void Deselect()
    {
        if (data.selectedNode == null) { return; }

        if (data.selectedNode[0] != null)
        {
            SetColor(data.selectedNode[0], Color.white);
            data.selectedNode[0] = null;
        }

        if (data.selectedNode[1] != null)
        {
            SetColor(data.selectedNode[1], Color.white);
            data.selectedNode[1] = null;
        }

        DeHighlight();
    }
    private void CreateEdge()
    {
        if (data.selectedNode[0] == null || data.selectedNode[1] == null) { return; }
        if (data.selectedNode[0] == data.selectedNode[1]) { return; }

        Node node;

        node = data.selectedNode[0].GetComponent<Node>();
        node.AddEdge(
            data.selectedNode[0].GetComponent<Node>(),
            data.selectedNode[1].GetComponent<Node>(),
            arrowSprite,
            edgeMaterial);
    }
    private void CreateVertex()
    {
        Vector2 pos = mousePos;
        pos.y += 100;
        data.graph.AddNode(pos);
    }


    private void SelectObject(PointerEventData eventData, ref GameObject obj, string name, string stopName = "")
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == obj || result.gameObject.name == stopName)
            {
                return;
            }
        }

        foreach (var result in results)
        {
            if (result.gameObject.name == name)
            {
                obj = result.gameObject;
                break;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SelectObject(eventData, ref data.selectedObject, "Node");
            if (data.selectedObject == null) { return; }

            if (data.selectedNode[0] == null)
            {
                data.selectedNode[0] = data.selectedObject;
                SetColor(data.selectedObject, Color.cyan);
                Highlight();
            }
            else if (data.selectedNode[1] == null && data.selectedObject != data.selectedNode[0])
            {
                data.selectedNode[1] = data.selectedObject;

                SetColor(data.selectedNode[0], Color.yellow);
                SetColor(data.selectedNode[1], Color.cyan);
                Highlight();
            }
            else if (data.selectedObject != data.selectedNode[1])
            {
                SetColor(data.selectedNode[0], Color.white);

                data.selectedNode[0] = data.selectedNode[1];
                data.selectedNode[1] = data.selectedObject;

                SetColor(data.selectedNode[0], Color.yellow);
                SetColor(data.selectedNode[1], Color.cyan);
                Highlight();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 widthBorder;
            Vector2 heightBorder;

            widthBorder.x = (Screen.width / 40);
            widthBorder.y = Screen.width - (Screen.width / 40);

            heightBorder.x = (Screen.height / 24);
            heightBorder.y = Screen.height - (Screen.height / 4f);

            if (!InSection(widthBorder, heightBorder)) { return; }

            GameObject selected = null;
            SelectObject(eventData, ref selected, "GameField", "Node");
            if (selected == null) { return; }
            CreateVertex();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CreateEdge();
        }
        else if (Input.GetMouseButtonUp(2))
        {
            Deselect();
        }
    }
}
