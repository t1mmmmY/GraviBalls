using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticBallsController : MonoBehaviour 
{
	public Color[] typeColors;

	public float G = 9.8f;
	public GameObject[] balls;

	public List<Transform> draggedBall = new List<Transform>();

	void Start()
	{
		//ONLY FOR TESTING!!! This sucks
		balls = GameObject.FindGameObjectsWithTag("Ball");
		foreach (GameObject ball in balls)
		{
			ball.renderer.material.color = typeColors[Random.Range(0, typeColors.Length)];
		}

		Debug.Log("Start");
		//Slow down
		//Time.timeScale = 0.1f;
	}

	void FixedUpdate()
	{
		MagneticForce();
	}

	void Update()
	{

#if !UNITY_EDITOR
		if (Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				switch (Input.GetTouch(i).phase)
				{
				case TouchPhase.Began:
					SelectBall(i);
					break;
				case TouchPhase.Moved:
					DragBall(i);
					break;
				case TouchPhase.Stationary:
					DragBall(i);
					break;
				case TouchPhase.Ended:
					ReleaseBall(i);
					break;
				}
			}
		}
#else
		if (Input.GetMouseButtonDown(0))
		{
			SelectBall();

		}
		if (Input.GetMouseButton(0))
		{
			DragBall();
		}
		if (Input.GetMouseButtonUp(0))
		{
			ReleaseBall();
		}
#endif
	}

	//Magnet balls to each other
	//F = G * m1 * m2 / r^2
	void MagneticForce()
	{
		for (int i = 0; i < balls.Length; i++)
		{
			for (int j = 0; j < balls.Length; j++)
			{
				if (i != j /*&& !draggedBall.Contains(balls[j].transform)*/)
				{
					Vector2 force = (balls[i].transform.localPosition - balls[j].transform.localPosition).normalized;
					//force = force.normalized;
					force = force * G * balls[i].rigidbody2D.mass * balls[j].rigidbody2D.mass / 
							Mathf.Pow(Vector2.Distance(balls[i].transform.localPosition, balls[j].transform.localPosition), 2.0f);

					balls[j].rigidbody2D.AddForce(force);
				}
			}
		}
	}

	void SelectBall(int number = 0)
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(GetTouchPosition(number)), Vector2.zero, 20.0f, 1 << 8);
		
		if(hit.collider != null)
		{
			draggedBall.Add(hit.transform);
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
			//draggedBall = hit.transform;
			//draggedBall.rigidbody2D.velocity = Vector2.zero;
		}

	}

	//Drag ball by finger
	void DragBall(int number = 0)
	{
		if (draggedBall.Count > number)
		{
			draggedBall[number].position = (Vector2)Camera.main.ScreenToWorldPoint(GetTouchPosition(number));
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
		}

//		if (draggedBall != null)
//		{
//			draggedBall.position = (Vector2)Camera.main.ScreenToWorldPoint(GetTouchPosition(number));
//			draggedBall.rigidbody2D.velocity = Vector2.zero;
//		}
	}

	void ReleaseBall(int number = 0)
	{
		if (draggedBall.Count > number)
		{
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
			draggedBall.RemoveAt(number);
		}

//		if (draggedBall != null)
//		{
//			draggedBall.rigidbody2D.velocity = Vector2.zero;
//			draggedBall = null;
//		}
	}


	Vector3 GetTouchPosition(int number)
	{
#if !UNITY_EDITOR
		return Input.GetTouch(number).position;
#else
		return Input.mousePosition;
#endif
	}

}
