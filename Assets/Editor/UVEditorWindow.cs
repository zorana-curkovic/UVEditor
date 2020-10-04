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
    private Vector2 rendererOffset = new Vector2(250,300);
    private Vector2 rendererScale = new Vector2(100, 100);
    private Vector2 rendererPointSize = new Vector2(3,3);
    private UVMeshRenderer m_uvMeshRenderer;
    
    //private GUIStyle _boxStyle;
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
        Object activeObject = Selection.activeObject;
        GameObject activeGameObject = activeObject as GameObject;
        MeshFilter activeMesh = activeObject as MeshFilter;

        if (activeGameObject != null)
        {
            m_GO = activeGameObject;

            MeshFilter filter = m_GO.GetComponent<MeshFilter>();
            m_Mesh = null;
            
            if (filter != null)
            {
                m_Mesh = filter.sharedMesh;

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
                    rendererScale = new Vector2(100, 100);
                    rendererOffset = new Vector2(400, 300);

                    m_uvMeshRenderer = new UVMeshRenderer(rendererScale, rendererOffset, rendererPointSize);
                    m_uvMeshRenderer.SetPoints(m_UV1);
                    m_uvMeshRenderer.GenerateEdges(m_Mesh.triangles);
                }
            }
            else
            {
                if (m_uvMeshRenderer != null)
                {
                    m_uvMeshRenderer.Clear();
                    m_uvMeshRenderer = null;
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
        EditorGUILayout.LabelField("Selected Object", selectedObjectText,EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Selected Mesh", selectedMeshText);

        //_boxStyle = new GUIStyle();
        
        // Constrain all drawing to be within a 800x600 pixel area centered on the screen.
       GUI.BeginGroup(new Rect(Screen.width / 2 - 400, Screen.height / 2 - 300, 800, 600));

        // Draw a box in the new coordinate space defined by the BeginGroup.
        // Notice how (0,0) has now been moved on-screen
       // GUI.Box(new Rect(0, 0, 800, 600), "This box is now centered! - here you would put your main menu");

        // We need to match all BeginGroup calls with an EndGroup
       
        if (m_uvMeshRenderer != null)
        {
            m_uvMeshRenderer.Offset = new Vector2(400, 300);
            m_uvMeshRenderer.DrawUVMesh();
        }
        
        GUI.EndGroup();
    }
}
