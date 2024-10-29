using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LineSegment
{
    public Vector3 start, end;
    public Color color;

    public Vector3 direction
    {
        get => end - start;
        set => end = start + value; 
    }
}

[ExecuteInEditMode]
public class LineDrawer : MonoBehaviour
{
    [SerializeField] private List<LineSegment> LinesToDraw = new List<LineSegment>();

    private void Update()
    {
        foreach (LineSegment line in LinesToDraw)
        {
            Debug.DrawLine(line.start, line.end, line.color, 0);
        }
    }
}