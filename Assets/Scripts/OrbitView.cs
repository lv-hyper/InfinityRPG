using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class OrbitView : MonoBehaviour
{
    [SerializeField]
    UILineRenderer uiLineRenderer;

    [SerializeField]
    int segmentCount = 36;

    [SerializeField]
    float radius, lineWidth=1.2f;

    public void SetWidth(float _lineWidth)
    {
        lineWidth = _lineWidth;
        uiLineRenderer.LineThickness = lineWidth;
    }

    public void SetRadius(float _radius)
    {
        radius = _radius;
        DrawOrbit();
    }

    private void Awake()
    {
        DrawOrbit();
    }

    public void DrawOrbit()
    {
        List<Vector2> orbitPoint = new List<Vector2>();

        if (uiLineRenderer == null)
        {
            Debug.LogError("UILineRenderer is null");
            return;
        }


        for(int _segment=0;_segment<=segmentCount;++_segment)
        {
            Vector2 point = new Vector2(
                Mathf.Cos(2 * Mathf.PI / segmentCount * _segment) * radius,
                Mathf.Sin(2 * Mathf.PI / segmentCount * _segment) * radius
            );

            orbitPoint.Add(point);
        }

        uiLineRenderer.Points = orbitPoint.ToArray();
        uiLineRenderer.LineThickness = lineWidth;
    }

    public void SetColor(Color orbitColor)
    {
        uiLineRenderer.color = orbitColor;
    }

    private void OnValidate()
    {
        DrawOrbit();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OrbitView))]
public class OrbitViewEditor : Editor
{
    private void OnValidate()
    {
        Debug.Log("test");
        OrbitView orbitView = target as OrbitView;

        orbitView.DrawOrbit();
    }
}
#endif
