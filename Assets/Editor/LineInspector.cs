using UnityEditor;
using System.Collections;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    Line line;
    private void OnEnable()//Enables GUI elements to be shown on editor
    {
        line = target as Line;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        //Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local?


        Transform handleTransform = line.transform;
        //Quaternion handleRotation = handleTransform.rotation;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);


        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
        Handles.DoPositionHandle(p0, handleRotation);
        Handles.DoPositionHandle(p1, handleRotation);


        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }
        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }

}
