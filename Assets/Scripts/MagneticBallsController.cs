using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticBallsController : MonoBehaviour 
{
	public Material[] ballMaterials;
	public float G = 9.8f;
	public GameObject spawnBall;
	public Transform startPosition;
	public float timeToSpawnBalls = 1.0f;
	public float spawnPower = 10.0f;
	public float magneticDistance = 1.0f;

	private GameObject newBall;

	private List<GameObject> balls;
	private List<Transform> draggedBall = new List<Transform>();

	void Start()
	{
		StartGame();
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


	void StartGame()
	{
		balls = new List<GameObject>();
		draggedBall = new List<Transform>();
		
		//ONLY FOR TESTING!!! This sucks
		GameObject[] startBalls = GameObject.FindGameObjectsWithTag("Ball");
		
		foreach (GameObject ball in startBalls)
		{
			balls.Add(ball);
			ball.renderer.material = ballMaterials[Random.Range(0, ballMaterials.Length)];
		}

		StartCoroutine("SpawnBalls");
	}

	//NOT RIGHT!
	IEnumerator SpawnBalls()
	{
		do
		{
			yield return new WaitForSeconds(timeToSpawnBalls);

			if (newBall == null)
			{
				newBall = (GameObject)Instantiate(spawnBall);
				newBall.transform.parent = this.transform;
				newBall.transform.position = startPosition.position;
				newBall.renderer.material = ballMaterials[Random.Range(0, ballMaterials.Length)];
				newBall.rigidbody2D.isKinematic = true;
			}

		} while (true);
	}


	//Magnet balls to each other
	//F = G * m1 * m2 / r^2
	void MagneticForce()
	{
		for (int i = 0; i < balls.Count; i++)
		{
			for (int j = 0; j < balls.Count; j++)
			{
				if (i != j && Vector2.Distance(balls[i].transform.localPosition, balls[j].transform.localPosition) < magneticDistance/*&& !draggedBall.Contains(balls[j].transform)*/)
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
		RaycastHit2D hit = Physics2D.Raycast(GetTouchPosition(number), Vector2.zero, 20.0f, 1 << 8);
		
		if(hit.collider != null/* && hit.transform != newBall.transform*/) //Drag old ball just for fun
		{
			draggedBall.Add(hit.transform);
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
		}
		/*else if (newBall != null) //spawn new ball
		{
			Vector2 force = (GetTouchPosition(number) - newBall.transform.position).normalized * spawnPower;
			newBall.rigidbody2D.isKinematic = false;
			newBall.rigidbody2D.AddForce(force);
			balls.Add(newBall);
			newBall = null;
		}*/
	}

	//Drag ball by finger
	void DragBall(int number = 0)
	{
		if (draggedBall.Count > number)
		{
			draggedBall[number].position = (Vector2)GetTouchPosition(number);
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
		}
	}

	void ReleaseBall(int number = 0)
	{
		if (draggedBall.Count > number)
		{
			draggedBall[number].rigidbody2D.velocity = Vector2.zero;
			draggedBall.RemoveAt(number);
		}
		else if (newBall != null) //spawn new ball
		{
			Vector2 force = (GetTouchPosition(number) - newBall.transform.position).normalized * spawnPower;
			newBall.rigidbody2D.isKinematic = false;
			newBall.rigidbody2D.AddForce(force);
			balls.Add(newBall);
			newBall = null;
		}
	}


	Vector3 GetTouchPosition(int number)
	{
#if !UNITY_EDITOR
		return Camera.main.ScreenToWorldPoint(Input.GetTouch(number).position);
#else
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
	}

}
