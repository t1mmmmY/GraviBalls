using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Anchor : MonoBehaviour 
{
	public Vector3 v3Pos;
	public bool hasPosition = false;

	Vector2 oldScreenSize = Vector2.zero;

	void Start()
	{
//		if (hasPosition)
//		{
//			Refresh();
//		}
		//oldScreenSize = new Vector2(Screen.width, Screen.height);
	}

	public void ApplyPosition() 
	{
		v3Pos = Camera.main.WorldToViewportPoint(transform.position);
		hasPosition = true;
		//Vector3 v3Pos = Vector3(0.0, 1.0, 0.25);
	}

	public void ResetPosition()
	{
		hasPosition = false;
	}

	void Update () 
	{
		Refresh();
	}

	public void Refresh()
	{
		if (hasPosition/* && oldScreenSize != new Vector2(Screen.width, Screen.height)*/)
		{
			transform.position = Camera.main.ViewportToWorldPoint(v3Pos);
		}
	}


}
