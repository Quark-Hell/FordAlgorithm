using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshPro;
    [SerializeField]
    private GameObject textContainer;

    public void Close()
    {
        textContainer.SetActive(false);
    }

    public void Show(string text)
    {
        textMeshPro.text = text;
        textContainer.SetActive(true);
    }
}
