using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineMesh))]
public class RoadInspector : Editor
{
    SplineMesh creator;


    private void OnEnable()
    {
        creator = (SplineMesh)target;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //if(creator.autoUpdate && Event)
        creator.UpdateRoad();
    }
}
