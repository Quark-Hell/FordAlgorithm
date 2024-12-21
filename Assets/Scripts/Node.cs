using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Node : MonoBehaviour
{
    public List<Edge> Edges { get; private set; } = new List<Edge>();
    public int Pos;
    public TextMeshPro Text;

    private GameObject textContainer;

    public void Configurate()
    {
        transform.tag = "Vertex";
        transform.name = "Node";

        textContainer = new GameObject();
        textContainer.transform.SetParent(transform, false);

        Text = textContainer.AddComponent<TextMeshPro>();
        RectTransform rectTransform = textContainer.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 1);

        Text.fontSize = 4;
        Text.color = new Color(255, 255, 255);
        Text.alignment = TextAlignmentOptions.Center;
        Text.verticalAlignment = VerticalAlignmentOptions.Middle;
    }

    public void SetImage(Sprite sprite)
    {
        Image img = transform.gameObject.AddComponent<Image>();
        img.sprite = sprite;
    }
    public void SetSize(Vector2 size)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }
    public void SetPosition(Vector2 position)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(position.x, position.y, -10);
    }

    public void AddEdge(Node start, Node end, Sprite arrow, Material mat)
    {
        for (int i = 0; i < Edges.Count; i++)
        {
            Edge edge = Edges[i].GetComponent<Edge>();
            if (edge.StartPoint == start && edge.EndPoint == end)
            {
                return;
            }
            if (edge.StartPoint == end && edge.EndPoint == start)
            {
                return;
            }
        }

        GameObject obj = new GameObject();
        Edge e = obj.AddComponent<Edge>();
        e.Create(start, end, arrow, mat);
        obj.transform.SetParent(transform);

        Edges.Add(e);
    }

    public void RemoveConnected(Node node)
    {
        for (int i = 0; i < Edges.Count; i++)
        {
            Edge edge = Edges[i].GetComponent<Edge>();
            if(edge.StartPoint == node || edge.EndPoint == node)
            {
                Destroy(edge.gameObject);
                Edges.RemoveAt(i);
                i--;
            }
        }
    }

    void Start()
    {

    }
}
