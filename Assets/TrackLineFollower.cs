using UnityEngine;
using System.Collections.Generic;

public class TrackLineFollower : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform carTransform;
    public Transform pathRoot; // Yolun noktalarýnýn (Waypoint) olduðu ebeveyn obje
    public int lookAheadCount = 20; // Arabanýn önünde kaç nokta görünecek?

    private List<Transform> pathPoints = new List<Transform>();

    void Start()
    {
        // Yol üzerindeki tüm noktalarý listeye al
        foreach (Transform child in pathRoot)
        {
            pathPoints.Add(child);
        }
    }

    void Update()
    {
        int closestIndex = GetClosestPointIndex();
        DrawLine(closestIndex);
    }

    int GetClosestPointIndex()
    {
        int closest = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < pathPoints.Count; i++)
        {
            float dist = Vector3.Distance(carTransform.position, pathPoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }
        return closest;
    }

    void DrawLine(int startIndex)
    {
        // Sadece startIndex'ten itibaren lookAheadCount kadar nokta çiz
        int pointsToDraw = Mathf.Min(lookAheadCount, pathPoints.Count - startIndex);
        lineRenderer.positionCount = pointsToDraw;

        for (int i = 0; i < pointsToDraw; i++)
        {
            lineRenderer.SetPosition(i, pathPoints[startIndex + i].position);
        }
    }
}