// Draw lines to the connected game objects that a script has.
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(PathDrawer))]
class PathDrawerHandle : Editor {
    //void OnSceneGUI() {
    //    PathDrawer connectedObjects = target as PathDrawer;
    //    if (connectedObjects.target == null)
    //        return;

    //    Vector3 center = connectedObjects.transform.position;
    //    Handles.color = Color.blue;
    //    Handles.DrawLine(center, connectedObjects.target.transform.position);
    //}

    //void OnEnable() {
    //    SceneView.duringSceneGui += CustomOnSceneGUI;
    //}

    //void CustomOnSceneGUI(SceneView sceneview) {
    //    PathDrawer connectedObjects = target as PathDrawer;
    //    if (connectedObjects.target == null)
    //        return;

    //    Vector3 center = connectedObjects.transform.position;
    //    Handles.color = Color.blue;
    //    Handles.DrawLine(center, connectedObjects.target.transform.position);
    //}


}