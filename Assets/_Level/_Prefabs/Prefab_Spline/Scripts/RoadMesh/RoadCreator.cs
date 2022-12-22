using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BezierSpline))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class RoadCreator : MonoBehaviour
{
    public BezierSpline spline;

    [Range(0.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    //Vector3[] points;

    float frequency = 10;

    public void UpdateRoad()
    {
        List<Vector3> splinePoints = new List<Vector3>();

        //BezierSpline spline = GetComponent<BezierSpline>();
        int curveCount = spline.CurveCount;

        if (frequency <= 0)
        {
            return;
        }
        float stepSize = frequency * curveCount;
        if (spline.Loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }

        for (int i = 0; i < curveCount; i++)
        {
            //points[i] = spline.GetPoint(i);
            splinePoints.Add(spline.GetPoint(i * stepSize));
        }
        Vector3[] points = splinePoints.ToArray();

       GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);
    }

    Mesh CreateRoadMesh(Vector3[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        int[] tris = new int[2 * (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 forward = Vector3.zero;
            if (i < points.Length - 1)
            {
                forward += points[i + 1] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector3 left = new Vector3(forward.x, forward.y, forward.z);

            verts[vertIndex] = points[i] + left * roadWidth * 0.5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * 0.5f;

            if(i < points.Length - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 5] = vertIndex + 3;
                tris[triIndex + 4] = vertIndex + 2;

            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;

        return mesh;
    }
}
