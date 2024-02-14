using System.Drawing;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem.Switch;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class MeshScript : MonoBehaviour
{
    // float width;
    //public float height;

    public Material mat;


    /*
    void Start()
    {
        CreateQuad(new Vector3(-width/2,-height/2, -width / 2), new Vector3(width / 2, -height / 2, -width / 2) ,new Vector3(width / 2, -height/2,width / 2)); //buttom
        CreateQuad(new Vector3(width/2,height/2, -width / 2), new Vector3(-width / 2, height / 2, -width / 2),new Vector3(-width / 2, height/2,width / 2)); //top
        CreateQuad(new Vector3(width / 2, -height / 2, -width / 2), new Vector3(-width / 2, -height / 2, -width / 2), new Vector3(-width / 2, height / 2, -width / 2)); //Z back
        CreateQuad(new Vector3(-width / 2, -height / 2, width / 2),new Vector3(width / 2, -height / 2, width / 2), new Vector3(width / 2, height / 2, width / 2)); //Z front
        CreateQuad(new Vector3(width / 2, -height / 2, width / 2), new Vector3(width / 2, -height / 2, -width / 2), new Vector3(width / 2, height / 2, -width / 2)); //X front
        CreateQuad(new Vector3(-width / 2, -height / 2, -width / 2), new Vector3(-width / 2, -height / 2, width / 2), new Vector3(-width / 2, height / 2, width / 2)); //X back
    }
    */
    ///edit to width and height
    public GameObject CreateQuad(Vector3 A, Vector3 C, Direction pos)
    {
        //set relative position
        A += transform.position;
        C += transform.position;
        Mesh rectangle = new Mesh();
        //creating gameobject
        GameObject g = new GameObject();
        //g.transform.parent = transform;
        //g.transform.localPosition = new Vector3(0, 0, 0);
        g.name = "rectangle";
        g.AddComponent<MeshFilter>();
        var boxCollider = g.AddComponent<BoxCollider>();
        var meshRenderer = g.AddComponent<MeshRenderer>();
        g.GetComponent<MeshFilter>().mesh = rectangle;
        meshRenderer.sharedMaterial = mat;
        ///////////////        if(pos.Multiplier == -1)
        Vector3 B = Vector3.Scale(pos.ValueX, C) + Vector3.Scale(pos.ValueY, A) + Vector3.Scale(pos.Value, A);
        Vector3 D = Vector3.Scale(pos.ValueX, A) + Vector3.Scale(pos.ValueY, C) + Vector3.Scale(pos.Value, C);
        var vertices = new Vector3[]{
            A,
            B,
            C,
            D,
        };

        var triangles = new int[6];
        if(pos.Rotation)
        {
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 3;
            triangles[4] = 0;
            triangles[5] = 2;
        }
        else
        {
            triangles[0] = 2;
            triangles[1] = 1;
            triangles[2] = 0;
            triangles[3] = 2;
            triangles[4] = 0;
            triangles[5] = 3;
        }
        float distanceToB = Vector3.Distance(A, B);
        float distanceToD = Vector3.Distance(A, D);
        var uvs = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(distanceToB,0),
            new Vector2(distanceToB,distanceToD),
            new Vector2(0,distanceToD),
        };

        rectangle.vertices = vertices;
        rectangle.triangles = triangles;
        rectangle.uv = uvs;
        rectangle.RecalculateNormals();
        boxCollider.center = meshRenderer.bounds.center;
        boxCollider.size = meshRenderer.bounds.size;
        return g;
    }
}
public enum Rotation {
up,
down,
front,
back,
left,
right,
}
