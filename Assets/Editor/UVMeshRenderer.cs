using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEditor;

public class UVMeshRenderer
{
    private Mesh m_UVsMesh;

    private static Material s_UVMaterial;
    //private List<Vector3> points;
    //private Matrix4x4 transform;
    private Vector2[] m_UVs;
    private Vector2[] m_vertices;
    private Vector2 m_scale;
    private Vector2 m_offset;
    private Vector2 m_pointSize;
    private List<List<int>> m_edges;


    //public UVMeshRenderer()
    //{
        //Vector2 m_Translation = new Vector2(0, 0);
        //Vector2 m_Scale = new Vector2(1, -1);
        //transform = Matrix4x4.TRS(m_Translation, Quaternion.identity, new Vector3(m_Scale.x, m_Scale.y, 1));
        //points = new List<Vector3>();
    //}

    public void Clear()
    {
        m_vertices = null;
        m_UVs = null;
        m_edges = null;
    }

    public Vector2 Offset
    {
        get => m_offset;
        set => m_offset = new Vector2(value.x - m_scale.x / 2, value.y - m_scale.y / 2);
    }

    public Vector2 Scale
    {
        get => m_scale;
        set { 
            m_scale = value;
            ScaleVertices();
        }
    }

    public Vector2 PointSize
    {
        get => m_pointSize;
        set
        {
            m_pointSize = value;
        }
    }

    public void SetPoints(Vector2[] UVs)
    {
        m_UVs = UVs;
        m_vertices = new Vector2[UVs.Length];
        ScaleVertices();
    }

    private void ScaleVertices()
    {
        for (int i = 0; i < m_UVs.Length; ++i)
        {
            m_vertices[i] = m_offset + (m_UVs[i] * m_scale);
        }
    }

    public void GenerateEdges(int[] triangles)
    {
        m_edges = new List<List<int>>();
        
        for (int i = 0; i < m_vertices.Length; ++i)
        {
            m_edges.Add(new List<int>());
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            if (v1 < v2)
            {
                if (!m_edges[v1].Contains(v2))
                    m_edges[v1].Add(v2);
            }
            else
            {
                if(!m_edges[v2].Contains(v1))
                    m_edges[v2].Add(v1);
            }
            
            if (v1 < v3)
            {
                if (!m_edges[v1].Contains(v3))
                    m_edges[v1].Add(v3);
            }
            else
            {
                if(!m_edges[v3].Contains(v1))
                    m_edges[v3].Add(v1);
            }

            if (v2 < v3)
            {
                if (!m_edges[v2].Contains(v3))
                    m_edges[v2].Add(v3);
            }
            else
            {
                if (!m_edges[v3].Contains(v2))
                    m_edges[v3].Add(v2);
            }
            
        }

        //PrintConnections();
    }

    private void PrintConnections()
    {
        for (int i = 0; i < m_edges.Count; ++i)
        {
            string connections = "";
            
            for (int j = 0; j < m_edges[i].Count; ++j)
                connections += $" {m_edges[i][j]} ";
            
            Debug.Log($"Vertex {i} : [{connections}]");
        }
    }
    

    public void DrawUVMesh()
    {
        if (m_vertices == null)
        {
            //Debug.Log("Points 2D are null");
            return;
        }

        
        /* Draw edges and vertices in one go */

        for (int i = 0; i < m_edges.Count; ++i)
        {
            Handles.color = Color.yellow;

            Handles.DrawLine(m_vertices[i] - m_pointSize, m_vertices[i] + new Vector2(m_pointSize.x,-m_pointSize.y));
            Handles.DrawLine(m_vertices[i] - m_pointSize, m_vertices[i] + new Vector2(-m_pointSize.x, m_pointSize.y));
            Handles.DrawLine(m_vertices[i] + new Vector2(-m_pointSize.x, m_pointSize.y), m_vertices[i] + m_pointSize);
            Handles.DrawLine(m_vertices[i] + new Vector2(m_pointSize.x,-m_pointSize.y), m_vertices[i] + m_pointSize);

            Handles.color = Color.green;

            for (int j = 0; j < m_edges[i].Count; ++j)
            { 
                Handles.DrawLine(m_vertices[i], m_vertices[m_edges[i][j]]);
            }
        }
        
    }
    

    // private void BuildCurveMesh()
    // {
    //     if (m_UVsMesh != null || points.Count == 0)
    //         return;
    //
    //     Vector3[] vertices = points.ToArray();
    //
    //     m_UVsMesh = new Mesh();
    //     m_UVsMesh.name = "UVMesh";
    //     m_UVsMesh.hideFlags |= HideFlags.DontSave;
    //     m_UVsMesh.vertices = vertices;
    //
    //     if (vertices.Length > 0)
    //     {
    //         int nIndices = vertices.Length - 1;
    //         int index = 0;
    //
    //         List<int> indices = new List<int>(nIndices * 2);
    //         while (index < nIndices)
    //         {
    //             indices.Add(index);
    //             indices.Add(++index);
    //         }
    //
    //         m_UVsMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
    //     }
        
        // Add to DrawMesh
        // if (m_UVsMesh == null)
        //    BuildCurveMesh();
        
        // Try rendering mesh at some point
        //Camera.SetupCurrent(null);
        // Material mat = uvMaterial;
        // mat.SetColor("_Color", Color.yellow);
        //
        // mat.SetPass(0);
        // Graphics.DrawMeshNow(m_UVsMesh, Handles.matrix * transform);
    //}


    // public static Material uvMaterial
    // {
    //     get
    //     {
    //         if (!s_UVMaterial)
    //         {
    //             Shader shader = (Shader)EditorGUIUtility.LoadRequired("Editors/AnimationWindow/Curve.shader");
    //             s_UVMaterial = new Material(shader);
    //             s_UVMaterial.hideFlags = HideFlags.HideAndDontSave;
    //         }
    //
    //         return s_UVMaterial;
    //     }
    // }
}
