using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph
{
    private readonly List<Node> nodes = new();
    public Ford ford { get; private set; } = new();

    public void AddNode(Vector2 pos, Sprite sprite, Transform rectParent)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(rectParent);

        Node n = obj.AddComponent<Node>();
        n.SetImage(sprite);
        n.SetSize(new Vector2(1,1));
        n.SetPosition(pos);

        n.Pos = nodes.Count;

        nodes.Add(n);
    }

    public void SetNodePosition(GameObject node, Vector2 pos)
    {
        Node n = node.GetComponent<Node>();

        if (!nodes.Contains(n)) { return; }
        n.SetPosition(pos);
    }

    public void RemoveNode(GameObject node) 
    {
        Node n = node.GetComponent<Node>();

        if (!nodes.Contains(n)) { return; }

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].RemoveConnected(node.GetComponent<Node>());
        }

        UnityEngine.Object.Destroy(node);
        nodes.Remove(n);

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Pos = i;
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < nodes.Count;)
        {
            RemoveNode(nodes[i].gameObject);
        }
    }
    public void StartAlgh(Node start)
    {
        int startVertex = 0;

        for(int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == start)
            {
                startVertex = i;
                break;
            }
        }

        List<Edge> edgesBuffer = new();
        List<Node> nodesBuffer = new();

        foreach (var node in nodes)
        {
            foreach (var edge in node.Edges)
            {
                edgesBuffer.Add(edge);
            }
        }

        ford.StartAlghorithm(nodes.Count, edgesBuffer, startVertex, nodes);
    }
}
