using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class SplineMesh : MonoBehaviour
{
    //--Variables--
    //-Public-
    //Parameters
    public float width = 1f;
    [Range(0.1f, 1f)]
    public float subdivisionLength = 0.1f;
    public bool update = true;
    public bool debug = true;
    //References
    public BezierSpline spline;
    Mesh mesh;
    //Data
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();


    //--Methods--
    void OnEnabled()
    {
        //mesh = GetComponent<MeshFilter>().mesh;
    }
    void Start()
    {
        if (update)
        {
            vertices.Clear();

            for (float t = 0f; t < 1f; t += subdivisionLength)
            {
                Vector3 sideVector = Vector3.Cross(spline.GetDirection(t), Vector3.forward).normalized;

                Vector3 rightVertice = transform.InverseTransformPoint((spline.GetPoint(t) + sideVector) * width);
                Vector3 leftVertice = transform.InverseTransformPoint((spline.GetPoint(t) - sideVector) * width);
                Vector3 nextRightVertice = transform.InverseTransformPoint((spline.GetPoint(t + subdivisionLength) + sideVector) * width);
                Vector3 nextLeftVertice = transform.InverseTransformPoint((spline.GetPoint(t + subdivisionLength) - sideVector) * width);

                Debug.DrawLine(spline.GetPoint(t), transform.TransformPoint(rightVertice));

                vertices.Add(rightVertice);
                vertices.Add(leftVertice);

                vertices.Add(nextLeftVertice);
                vertices.Add(nextRightVertice);
            }
            mesh.vertices = VertsToMesh(vertices).vertices;
            mesh.triangles = VertsToMesh(vertices).triangles;
            mesh.normals = SetNormals(mesh, Vector3.back);
        }
    }
    
    public void UpdateRoad()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        //mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices.Clear();

        for (float t = 0f; t < 1f; t += subdivisionLength)
        {
            Vector3 sideVector = Vector3.Cross(spline.GetDirection(t), Vector3.forward).normalized;

            Vector3 rightVertice = transform.InverseTransformPoint((spline.GetPoint(t) + sideVector) * width);
            Vector3 leftVertice = transform.InverseTransformPoint((spline.GetPoint(t) - sideVector) * width);
            Vector3 nextRightVertice = transform.InverseTransformPoint((spline.GetPoint(t + subdivisionLength) + sideVector) * width);
            Vector3 nextLeftVertice = transform.InverseTransformPoint((spline.GetPoint(t + subdivisionLength) - sideVector) * width);

            Debug.DrawLine(spline.GetPoint(t), transform.TransformPoint(rightVertice));

            vertices.Add(rightVertice);
            vertices.Add(leftVertice);

            vertices.Add(nextLeftVertice);
            vertices.Add(nextRightVertice);
        }
        //mesh.vertices = VertsToMesh(vertices).vertices;
        //mesh.triangles = VertsToMesh(vertices).triangles;
        //mesh.normals = SetNormals(mesh, Vector3.back);

        mesh.vertices = VertsToMesh(vertices).vertices;
        mesh.triangles = VertsToMesh(vertices).triangles;
        mesh.normals = SetNormals(mesh, Vector3.back);
    }
    
    public Mesh VertsToMesh(List<Vector3> vertices)
    {
        List<int> newTriangles = new List<int>();

        //If the vertex count is divisible by 4 it can be made into quads
        if (vertices.Count % 4 == 0)
        {
            for (int vertexPointer = 0; vertexPointer + 4 <= vertices.Count; vertexPointer += 4)
            {
                newTriangles.Add(vertexPointer + 0);
                newTriangles.Add(vertexPointer + 1);
                newTriangles.Add(vertexPointer + 2);

                //                newTriangles.Add(vertexPointer + 1);
                //                newTriangles.Add(vertexPointer + 2);
                //                newTriangles.Add(vertexPointer + 3);

                newTriangles.Add(vertexPointer + 0);
                newTriangles.Add(vertexPointer + 2);
                newTriangles.Add(vertexPointer + 3);
            }
        }
        //If the vertex count isn't divisible by 4 it will be made into triangles
        else
        {
            for (int vertexPointer = 0; vertexPointer + 3 <= vertices.Count; vertexPointer += 3)
            {
                newTriangles.Add(vertexPointer + 0);
                newTriangles.Add(vertexPointer + 1);
                newTriangles.Add(vertexPointer + 2);
            }
        }

        List<Vector2> textureCoords = new List<Vector2>();
        Vector2 emptyTextureCoords = new Vector2(0, 0);

        for (int texturePointer = 0; texturePointer < vertices.Count; texturePointer++)
        {
            //There'll be as many texture coordinates as vertices
            //For now textures won't be supported so we'll fill the uvs with empty coordinates
            textureCoords.Add(emptyTextureCoords);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        mesh.triangles = newTriangles.ToArray();
        mesh.uv = textureCoords.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
    public Vector3[] SetNormals(Mesh mesh, Vector3 normal)
    {
        Vector3[] normals = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
            normals[i] = normal;
        return normals;
    }
}