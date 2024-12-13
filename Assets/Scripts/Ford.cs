using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ford
{
    public GameObject infoText;

    public void StartAlghorithm(int verticesCount, List<Edge> edges, int startVertex, List<Node> nodes)
    {
        TextMeshProUGUI text = infoText.GetComponent<TextMeshProUGUI>();
        text.text = "";
        int[] distances = new int[verticesCount];
   
        for (int i = 0; i < verticesCount; i++)
        {
            distances[i] = int.MaxValue;
        }
        distances[startVertex] = 0;
   
        for (int i = 1; i < verticesCount; i++)
        {
            bool updated = false;
            foreach (var edge in edges)
            {
                if (distances[edge.StartPoint.Pos] != int.MaxValue && distances[edge.StartPoint.Pos] + edge.Weight < distances[edge.EndPoint.Pos])
                {
                    distances[edge.EndPoint.Pos] = distances[edge.StartPoint.Pos] + edge.Weight;
                    updated = true;
                }
            }

            if (!updated) break;
        }
   
        foreach (var edge in edges)
        {
            if (distances[edge.StartPoint.Pos] != int.MaxValue && distances[edge.StartPoint.Pos] + edge.Weight < distances[edge.EndPoint.Pos])
            {
                text.text = "Граф содержит отрицательный цикл.";
                break;
            }
        }

        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == int.MaxValue)
            {
                nodes[i].Text.text = "";
            }
            else
            {
                nodes[i].Text.text = distances[i].ToString();
            }
        }
    }
}
