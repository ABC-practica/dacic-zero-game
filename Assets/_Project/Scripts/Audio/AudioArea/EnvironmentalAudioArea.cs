using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalAudioArea : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> points = new()
    {
        new Vector3(-5f, 0f, -5f),
        new Vector3(5f, 0f, -5f),
        new Vector3(0f, 0f, 5f)
    };
    public IReadOnlyList<Vector3> Points => points;
    private List<Vector3> projectedPoints = new();

    private void Awake()
    {
        projectedPoints.Clear();
        foreach (Vector3 point in points)
        {
            Vector3 realPoint = gameObject.transform.TransformPoint(point);
            projectedPoints.Add(ProjectVector3(realPoint));
        }
    }

    public void RemovePoint(int index)
    {
        if (points.Count <= 3)
            return;

        points.RemoveAt(index);
    }

    public void InsertPoint(int index, Vector3 point)
    {
        points.Insert(index, point);
    }

    public void SetPoint(int index, Vector3 point)
    {
        points[index] = point;
    }

    private Vector2 ProjectVector3(Vector3 point)
    {
        return new Vector2(point.x, point.z);
    }

    private float SquaredDistanceToSegment(Vector2 pointA, Vector2 pointB, Vector2 point)
    {
        Vector2 segment = pointB - pointA;
        Vector2 toPoint = point - pointA;

        float lengthSquared = segment.sqrMagnitude;

        if (lengthSquared == 0f)
            return toPoint.sqrMagnitude;

        float t = Mathf.Clamp01(
            Vector2.Dot(toPoint, segment) / lengthSquared
        );

        Vector2 difference = toPoint - t * segment;

        return difference.sqrMagnitude;
    }

    public float GetDistanceToPolygon(Vector3 pos)
    {
        Vector2 pos2D = ProjectVector3(pos);
        int numberIntersections = 0;
        for (int i = 0; i < projectedPoints.Count; i++)
        {
            Vector2 point1 = projectedPoints[i];
            Vector2 point2 = projectedPoints[(i + 1) % projectedPoints.Count];
            if ( (point1.y > pos2D.y) != (point2.y > pos2D.y) )
            {
                float intersectionX = point1.x + (pos2D.y - point1.y) * (point2.x - point1.x) / (point2.y - point1.y);
                if (intersectionX > pos2D.x)
                    numberIntersections++;
            }
        }
        if (numberIntersections % 2 == 1)
            return Mathf.Abs(pos.y);

        float minDistance = SquaredDistanceToSegment(
            projectedPoints[0],
            projectedPoints[1],
            pos2D
            );

        for (int i = 1; i < projectedPoints.Count; i++)
        {
            Vector2 point1 = projectedPoints[i];
            Vector2 point2 = projectedPoints[(i + 1) % projectedPoints.Count];
            minDistance = Mathf.Min(
                minDistance,
                SquaredDistanceToSegment(point1, point2, pos2D)
                );
        }

        return Mathf.Sqrt(minDistance + pos.y * pos.y);
    }
}
