using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.Remoting.Channels;
using Object = UnityEngine.Object;

public class UVEditorWindow : EditorWindow
{
    string selectedObjectText = "Selected Object";
    string selectedMeshText = "Selected Mesh";

    private GameObject m_GO;
    private Mesh m_Mesh;
    private int m_vertexCount;
    private Vector2[] m_UV1;
    private Vector2 rendererPointSize = new Vector2(3,3);
    private UVMeshRenderer m_uvMeshRenderer;
    
    [MenuItem("Window/Rendering/UVEditor")]
    static void Init()
    {
        UVEditorWindow window = (UVEditorWindow) EditorWindow.GetWindow(typeof(UVEditorWindow));
        window.Show();
    }
    
    public void OnFocus()
    {
        OnSelectionChange();
    }

    public void OnSelectionChange()
    {
        if (m_uvMeshRenderer != null)
        {
            m_uvMeshRenderer.Clear();
            m_uvMeshRenderer = null;
        }
        
        Object activeObject = Selection.activeObject;
        GameObject activeGameObject = activeObject as GameObject;
        m_GO = null;
        m_Mesh = null;

        if (activeGameObject != null)
        {
            m_GO = activeGameObject;

            MeshFilter filter = m_GO.GetComponentInChildren<MeshFilter>();
            

            if (filter == null)
            {
                SkinnedMeshRenderer smr = m_GO.GetComponentInChildren<SkinnedMeshRenderer>();
               
                if (smr != null)
                    m_Mesh = smr.sharedMesh;
            }
            else
            {
                m_Mesh = filter.sharedMesh;
            }
            
            
            if (m_Mesh != null)
            {
                m_vertexCount = m_Mesh.vertexCount;
                m_UV1 = m_Mesh.uv;
                selectedMeshText = "#" + m_vertexCount.ToString() + "=";
                for (int i = 0; i < m_vertexCount; ++i)
                    selectedMeshText += "["+m_UV1[i].x + "," + m_UV1[i].y+"] ";

                if (m_uvMeshRenderer != null)
                {
                    m_uvMeshRenderer.SetPoints(m_UV1);
                }
                else
                {
                    m_uvMeshRenderer = new UVMeshRenderer();
                    m_uvMeshRenderer.SetPoints(m_UV1);
                    m_uvMeshRenderer.GenerateEdges(m_Mesh.triangles);
                }
            }

        }


        Repaint();
    }

    //private float a;
    void OnGUI()
    {
        //a += Input.mouseScrollDelta.y;
            
        if (m_GO != null) 
            selectedObjectText = m_GO.name.ToString();
        else
            selectedObjectText = "No Selected Object";

        if (m_Mesh == null)
        {
            selectedMeshText = "No Mesh";
        }
 

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("  Selected Object ", selectedObjectText,EditorStyles.boldLabel);
        EditorGUILayout.LabelField("  Selected Mesh ", selectedMeshText);

        //_boxStyle = new GUIStyle();
        float WindowSize = Mathf.Min(position.width, position.height) - 100;
        float WindowSizeHalf = WindowSize / 2;

        float UVScale = WindowSize - 100;
        float UVScaleHalf = UVScale / 2;
        
        // Constrain all drawing to be within a 800x600 pixel area centered on the screen.
       GUI.BeginGroup(new Rect(50,  100, WindowSize, WindowSize));

        // Draw a box in the new coordinate space defined by the BeginGroup.
        // Notice how (0,0) has now been moved on-screen
       GUI.Box(new Rect(0, 0, WindowSize, WindowSize), "");

        // We need to match all BeginGroup calls with an EndGroup
       
        if (m_uvMeshRenderer != null)
        {
            
            m_uvMeshRenderer.Offset = new Vector2(WindowSizeHalf, WindowSizeHalf);
            m_uvMeshRenderer.Scale = new Vector2(UVScale, UVScale);
            m_uvMeshRenderer.PointSize = new Vector2(UVScale/250, UVScale/250);
            m_uvMeshRenderer.DrawUVMesh();
        }
        
        GUI.EndGroup();
    }
}
