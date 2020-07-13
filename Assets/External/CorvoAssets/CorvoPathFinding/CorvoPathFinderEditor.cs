#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CorvoPathFinder))]
public class CorvoPathFinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CorvoPathFinder myScript = (CorvoPathFinder)target;
        if (GUILayout.Button("Test grid build"))
        {
            myScript.generateGrid(true);
        }

        if (GUILayout.Button("Find Path"))
        {
            myScript.clearPath();
            if (myScript.testPathDestination)
                myScript.findPath(myScript.testPathDestination.position);
            else
                Debug.LogError("Destination not assigned!");
        }
        if (GUILayout.Button("Clear"))
        {
            myScript.forceStop();
        }
    }
}
#endif
