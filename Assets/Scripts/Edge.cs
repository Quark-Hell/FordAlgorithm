using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Edge : MonoBehaviour, IScrollHandler
{
    public Node StartPoint { get; private set; }
    public Node EndPoint { get; private set; }
    public int Weight;

    private LineRenderer lineRenderer = null;
    private GameObject textContainer = null;
    private TextMeshPro textMeshPro = null;

    private GameObject Arrow = null;

    public void Create(Node startPoint, Node endPoint, Sprite arrow, Material mat)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;

        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, -10);

        lineRenderer = this.AddComponent<LineRenderer>();

        lineRenderer.material = mat;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.positionCount = 2;
        lineRenderer.numCapVertices = 16;

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;

        CreateArrow(arrow);
        CreateText();
    }

    private void CreateArrow(Sprite arrow)
    {
        Arrow = new GameObject();
        Arrow.name = "Arrow";
        Arrow.transform.SetParent(transform);
        Image arrImg = Arrow.AddComponent<Image>();
        arrImg.sprite = arrow;
        arrImg.raycastTarget = false;

        RectTransform rect = Arrow.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0.8f, 0.8f);
    }

    private void CreateText()
    {
        GameObject text = new GameObject();
        text.name = "text";
        text.transform.SetParent(transform);
        text.transform.Rotate(new Vector3(0, 0, 30f));

        textMeshPro = text.AddComponent<TextMeshPro>();
        textMeshPro.material.color = Color.white;
        textMeshPro.fontSize = 4;

        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;

        textContainer = new GameObject();
        textContainer.name = "textContainer";
        textContainer.transform.SetParent(transform);
        text.transform.SetParent(textContainer.transform);

        RectTransform rect = textContainer.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0.8f, 0.8f);

        Image img = textContainer.AddComponent<Image>();
        img.color = new Vector4(0, 0, 0, 0);
    }

    void Start()
    {
        transform.name = "Edge";

        SetTransform();
    }
    void Update()
    {
        SetTransform();
    }

    private void SetTransform()
    {
        SetLineTransform();
        SetArrowTransform();
        SetTextTransorm();

        textMeshPro.text = Weight.ToString();
    }

    private void SetTextTransorm() 
    {
        Vector3 originPoint = StartPoint.transform.position;
        Vector3 circleCenter = EndPoint.transform.position;

        Vector3 targetPosition;
        Quaternion targetRotation;

        float radius = 1.5f;

        Vector3 dir = originPoint - circleCenter;

        targetPosition = GetNearestPointOnCircle(circleCenter, radius, originPoint);
        targetRotation = Quaternion.Euler(0, 0, 0);

        textContainer.transform.rotation = targetRotation;
        textMeshPro.transform.rotation = targetRotation;

        textContainer.transform.position = new Vector3(targetPosition.x, targetPosition.y, 85);
    }


    private void SetLineTransform()
    {
        Vector3 originPoint = StartPoint.transform.position;
        Vector3 circleCenter = EndPoint.transform.position;
        float radius = 0.75f;

        Vector3 start = GetNearestPointOnCircle(originPoint, radius, circleCenter);
        Vector3 end = GetNearestPointOnCircle(circleCenter, radius, originPoint);

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void SetArrowTransform()
    {
        Vector3 originPoint = StartPoint.transform.position;
        Vector3 circleCenter = EndPoint.transform.position;
        float radius = 0.95f;

        Vector3 nearestPoint = GetNearestPointOnCircle(circleCenter, radius, originPoint);

        Arrow.transform.position = nearestPoint;

        Vector3 direction = nearestPoint - originPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        Arrow.transform.rotation = targetRotation;
    }


    private Vector3 GetNearestPointOnCircle(Vector3 center, float radius, Vector3 point)
    {
        Vector3 direction = point - center;
        direction.Normalize();
        Vector3 nearestPoint = center + direction * radius;

        return nearestPoint;
    }
    public void OnScroll(PointerEventData eventData)
    {
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var obj in results)
        {
            if (obj.gameObject.name == "textContainer")
            {
                Edge e = obj.gameObject.GetComponentInParent<Edge>();
                e.Weight += (int)eventData.scrollDelta.y;
            }
        }
    }
}
