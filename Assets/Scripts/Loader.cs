using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public GameObject GameField;
    public Sprite saveSprite;
    public GameObject Panel;
    public GameObject Parent;
    public string Path;

    public Controller controller;

    private List<GameObject> saves = new List<GameObject>();

    public void Show()
    {
        GameField.SetActive(false);
        Panel.SetActive(true);
        LoadFiles();
    }

    private void LoadFiles()
    {
        if (!Directory.Exists(Path)) { return; }
        string[] files = Directory.GetFiles(Path, "*.json");

        foreach (var save in saves)
        {
            Destroy(save);
        }

        float shift = -850;
        const float factor = 200;
        float realFactor = 0;

        saves = new List<GameObject>();
        foreach (var item in files)
        {
            string name = item.Remove(0, 6);

            RectTransform rectTransform;

            GameObject save = new GameObject();
            save.transform.SetParent(Parent.transform);
            RectTransform parentRect = save.AddComponent<RectTransform>();
            parentRect.sizeDelta = new Vector2(1, 1);
            parentRect.anchoredPosition3D = new Vector3(shift + realFactor, 70, 0);

            GameObject saveButton = new GameObject();
            saveButton.transform.SetParent(save.transform);
            rectTransform = saveButton.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1,1);
            rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);

            Button butt = saveButton.AddComponent<Button>();
            butt.transform.name = "Saves/" + name;
            butt.onClick.AddListener(() => LoadLevel(butt.transform.name));
            Image img = saveButton.AddComponent<Image>();
            img.sprite = saveSprite;

            butt.targetGraphic = img;


            GameObject saveText = new GameObject();
            saveText.transform.SetParent(save.transform);
            TextMeshProUGUI textMeshPro = saveText.AddComponent<TextMeshProUGUI>();
            textMeshPro.text = name.ToString();
            textMeshPro.fontSize = 0.3f;
            rectTransform = saveText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 1);
            rectTransform.anchoredPosition3D = new Vector3(0.1f, -1, -10);

            realFactor += factor;
        }

    }

    public void LoadLevel(string path)
    {
        controller.Load(path);
        Debug.Log(path);

        Hide();
    }

    public void Hide()
    {
        GameField.SetActive(true);
        Panel.SetActive(false);
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
