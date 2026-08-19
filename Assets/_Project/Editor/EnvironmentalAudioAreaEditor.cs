using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentalAudioArea))]
public class EnvironmentalAudioAreaEditor : Editor
{
    private const float VertexHandleSize = 0.15f;
    private const float GhostHandleSize = 0.1f;

    private void OnSceneGUI()
    {
        EnvironmentalAudioArea area = (EnvironmentalAudioArea)target;
        IReadOnlyList<Vector3> points = area.Points;

        if (points.Count < 3)
            return;

        DrawPolygon(area, points);
        DrawGhostHandles(area, points);
        DrawVertexHandles(area, points);
    }

    private void DrawPolygon(EnvironmentalAudioArea area, IReadOnlyList<Vector3> points)
    {
        Handles.color = Color.cyan;
        for (int i = 0; i < points.Count; i++)
        {
            int nextIndex = (i + 1) % points.Count;
            Vector3 pointA = area.transform.TransformPoint(points[i]);
            Vector3 pointB = area.transform.TransformPoint(points[nextIndex]);
            Handles.DrawLine(pointA, pointB, 2f);
        }
    }

    private void DrawVertexHandles(EnvironmentalAudioArea area, IReadOnlyList<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 localPoint = points[i];
            Vector3 worldPoint = area.transform.TransformPoint(points[i]);
            float handleSize = HandleUtility.GetHandleSize(worldPoint) * VertexHandleSize;

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPoint = Handles.FreeMoveHandle(
                worldPoint,
                handleSize,
                Vector3.zero,
                Handles.DotHandleCap
            );

            if (!EditorGUI.EndChangeCheck())
                continue;

            Vector3 newLocalPoint =
                area.transform.InverseTransformPoint(newWorldPoint);
            newLocalPoint.y = localPoint.y;

            Undo.RecordObject(
                area,
                "Move Environmental Audio Point"
            );

            area.SetPoint(i, newLocalPoint);
            EditorUtility.SetDirty(area);
        }
    }

    private void DrawGhostHandles(EnvironmentalAudioArea area, IReadOnlyList<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            int nextIndex = (i + 1) % points.Count;
            Vector3 pointA = area.transform.TransformPoint(points[i]);
            Vector3 pointB = area.transform.TransformPoint(points[nextIndex]);
            Vector3 midpoint = (pointA + pointB) * 0.5f;
            float handleSize = HandleUtility.GetHandleSize(midpoint) * GhostHandleSize;

            if (!Handles.Button(
                midpoint,
                Quaternion.identity,
                handleSize,
                handleSize,
                Handles.DotHandleCap))
            {
                continue;
            }

            Undo.RecordObject(
                area,
                "Add Environmental Audio Point"
            );

            Vector3 localPoint = area.transform.InverseTransformPoint(midpoint);
            localPoint.y = points[i].y;
            area.InsertPoint(i + 1, localPoint);

            EditorUtility.SetDirty(area);
            GUIUtility.ExitGUI();
        }
    }
}