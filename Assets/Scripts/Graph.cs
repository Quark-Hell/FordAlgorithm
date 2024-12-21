using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class NodeData
{
    public List<EdgeData> Edges = new List<EdgeData>();
    public string Text;
    public int Pos;
    public Vector2 Position;

    public NodeData(string text, int pos, Vector2 position)
    {
        Text = text;
        Pos = pos;
        Position = position;
    }
}

public class NodeDataList
{
    public List<NodeData> nodes = new List<NodeData>();
}

[System.Serializable]
public class EdgeData
{
    public int StartPoint;
    public int EndPoint;
    public int Weight;

    public EdgeData(int st, int end, int weight)
    {
        StartPoint = st;
        EndPoint = end;
        Weight = weight;
    }
}

public class Graph
{
    public Sprite sprite;
    public Transform rectParent;

    public Sprite arrow;
    public Material edgeMat;

    private readonly List<Node> nodes = new();
    public Ford ford { get; private set; } = new();

    public void Serialize(string path)
    {
        NodeDataList nodeDatas = new NodeDataList();


        for (int i = 0; i < nodes.Count; i++)
        {
            Vector2 pos = nodes[i].GetComponent<RectTransform>().anchoredPosition;
            nodeDatas.nodes.Add(new NodeData(nodes[i].Text.text, nodes[i].Pos, pos));
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].Edges.Count; j++)
            {
                Node firstPoint = nodes[i].Edges[j].StartPoint;
                Node endPoint = nodes[i].Edges[j].EndPoint;

                int fP = 0;
                int endP = 0;

                for (int k = 0; k < nodes.Count; k++)
                {
                    if(firstPoint == nodes[k])
                    {
                        fP = k;
                    }
                    if (endPoint == nodes[k])
                    {
                        endP = k;
                    }
                }

                int weight = nodes[i].Edges[j].Weight;
                nodeDatas.nodes[i].Edges.Add(new EdgeData(fP, endP, weight));
            }
        }

        string json = JsonUtility.ToJson(nodeDatas, true);

        File.WriteAllText(path, json);
        Debug.Log($"Data saved in: {path}");
    }

    public bool Deserialize(string path) 
    {
        if (!File.Exists(path)) { return false; }

        string json = File.ReadAllText(path);
        NodeDataList dataList = JsonUtility.FromJson<NodeDataList>(json);

        RemoveAll();

        foreach (var node in dataList.nodes)
        {
            AddNode(node.Position);
            nodes[nodes.Count - 1].Pos = node.Pos;
            nodes[nodes.Count - 1].Text.text = node.Text;


        }

        for (int i = 0; i < nodes.Count; i++)
        {
            foreach (var edge in dataList.nodes[i].Edges)
            {
                Node start = nodes[edge.StartPoint];
                Node end = nodes[edge.EndPoint];

                nodes[i].AddEdge(start, end, arrow, edgeMat);
                Edge ed = nodes[i].Edges[nodes[i].Edges.Count - 1];
                ed.Weight = edge.Weight;
            }
        }

        return true;
    }

    public void AddNode(Vector2 pos)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(rectParent);

        Node n = obj.AddComponent<Node>();
        n.SetImage(sprite);
        n.SetSize(new Vector2(1,1));
        n.SetPosition(pos);

        n.Pos = nodes.Count;
        n.Configurate();

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
