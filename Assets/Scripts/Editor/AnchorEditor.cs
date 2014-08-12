using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Anchor))]
public class AnchorEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Anchor myScript = (Anchor)target;
		if (GUILayout.Button("Apply position"))
		{
			myScript.ApplyPosition();
		}

		if (GUILayout.Button("Refresh"))
		{
			myScript.Refresh();
		}

		if (GUILayout.Button("Reset position"))
		{
			myScript.ResetPosition();
		}
	}

	[MenuItem ("MyMenu/Refresh Anchors")]
	static void RefreshAnchors()
	{
		Anchor[] anchors = FindObjectsOfType(typeof(Anchor)) as Anchor[];
		foreach (Anchor anchor in anchors)
		{
			anchor.Refresh();
		}

//		Anchor myScript = (Anchor)target;
//		myScript.Refresh();
	}

}
