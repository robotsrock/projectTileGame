
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class inputController : MonoBehaviour { // REFACTOR this should be split into multiple files

	[Header("Variables")]
	public float scaleFactor = 10f;
	public float floatScaleMultiplier = 2.5f;
	public float maxCameraSize = 10f;
	public float minCameraSize = 3f;
	public float zoomRate = 1f;
	public bool useGamePad = false;
	public int placeMode; // 0 is place "Stone", 1 is place wall 
						  // TODO change this so we place either tile or object
	[Header("Objects")]
	[Space(5)]
	public Camera mainCamera;
	public GameObject worldController;
	public GameObject selectPrefab; // this should be handled better later
	public GameObject deletePrefab;

	bool cameraFloat = false;
	Vector2 lastMousePos = Vector2.zero;
	Vector2 mouseStart; // position of the top left 
	Vector2 mousePos;
    List<GameObject> selectGOs;
	List<GameObject> deleteGOs;
	bool isDeleting = false;

	inputManager manager = new inputManager();
	// Use this for initialization
	void Start()
	{
		this.selectGOs = new List<GameObject>();
	}
	void Awake()
	{
		this.setupAxis();
	}
	void setupAxis()
	{
		inputManager.instance.setupManager(this.useGamePad);
		inputManager.instance.bindAxis("scrollWheel", "mse1_SCW", KeyCode.None, KeyCode.None, false);
		inputManager.instance.bindAxis("horizontal", "joy1_LSX", KeyCode.D, KeyCode.A);
		inputManager.instance.bindAxis("vertical", "joy1_LSY", KeyCode.W, KeyCode.S);
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.mousePos = this.mainCamera.ScreenToWorldPoint(Input.mousePosition);
		this.updateCamera();
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) // if we are not over a ui element
		{
			this.updateLeftMouse();
			this.updateRightMouse();
		}
	}
	public void setCameraFloat(bool shouldFloat)
	{
		this.cameraFloat = shouldFloat;
	}

	void updateCamera() // FIXME the float camera code is not very good, we should set a speed and continuoasly move at it, this will also work on GP
	{
		if (Input.GetMouseButtonDown(2))
		{
			lastMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		}

		if (Input.GetMouseButton(2) == true)
		{
			Vector2 curMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			Vector2 movement = curMousePos - lastMousePos;

			if (!this.cameraFloat) // if we are not supposed to float
			{
				mainCamera.transform.position = mainCamera.transform.position + new Vector3(movement.x, movement.y, 0f) / this.scaleFactor;
				lastMousePos = curMousePos;
			}
			else // if we are supposed to float
			{
				mainCamera.transform.position = mainCamera.transform.position + new Vector3(movement.x, movement.y, 0f) / (scaleFactor * this.floatScaleMultiplier);
			}
		}
		if (inputManager.instance.getAxis("scrollWheel") > 0 && mainCamera.orthographicSize > this.minCameraSize)
		{
			mainCamera.orthographicSize -= this.zoomRate;
		}
		else if (inputManager.instance.getAxis("scrollWheel") < 0 && mainCamera.orthographicSize < this.maxCameraSize)
		{
			mainCamera.orthographicSize += this.zoomRate;
		}
	}

	void updateLeftMouse() // FIXME should use inputmanager, but we dont have it implemented yet.
	{
		while (selectGOs.Count > 0)
		{
			GameObject go = selectGOs[0];
			selectGOs.RemoveAt(0);
			SimplePool.Despawn(go);
		}

		if (Input.GetMouseButtonDown(0)) // if the left mouse is clicked this frame then we set the start for the box and the isDeleting flag
		{
			this.mouseStart = mousePos;
			this.isDeleting = false;
		}
		//Debug.Log(this.mouseStart);

		if (Input.GetMouseButtonUp(0)) // if the left mouse is let up this frame 
		{
			if (!isDeleting)
			{
				if (mousePos.x < mouseStart.x) // invert them if the end is less than the start
				{
					float tmp;
					tmp = this.mouseStart.x;
					this.mouseStart.x = mousePos.x;
					mousePos.x = tmp;
				}
				if (mousePos.y < mouseStart.y) // invert them if the end is less than the start
				{
					float tmp;
					tmp = mouseStart.y;
					mouseStart.y = mousePos.y;
					mousePos.y = tmp;
				}
				for (int x = Mathf.FloorToInt(this.mouseStart.x); x <= Mathf.FloorToInt(mousePos.x); x++) // loop through all tiles and set their tiles
				{
					for (int y = Mathf.FloorToInt(this.mouseStart.y); y <= Mathf.FloorToInt(this.mousePos.y); y++)
					{
						if (this.placeMode == 0) // we are in tile place mode
						{
							this.worldController.GetComponent<worldController>().getWorld().setTileAt(x, y, "stone", 0);
							// TODO we should get what kind of tile the user wants to place
						}
						else if (this.placeMode == 1) // we are in object place mode
						{
						this.worldController.GetComponent<worldController>().getWorld().placeWorldObject(
							"wall",
							this.worldController.GetComponent<worldController>().getWorld().getTileAt(x, y)); // TODO use a selection system
						}
						else
						{
							Debug.Log("Not a valid place mode");
						}
					}
				}
			}
		}
		if (Input.GetMouseButton(0)) // if left mouse is held
		{
			if (!isDeleting)
			{
				int start_x = Mathf.FloorToInt(mouseStart.x);
				int end_x = Mathf.FloorToInt(mousePos.x);
				int start_y = Mathf.FloorToInt(mouseStart.y);
				int end_y = Mathf.FloorToInt(mousePos.y);
				if (end_x < start_x) // invert them if the end is less than the start
				{
					int tmp;
					tmp = end_x;
					end_x = start_x;
					start_x = tmp;
				}
				if (end_y < start_y) // invert them if the end is less than the start
				{
					int tmp;
					tmp = end_y;
					end_y = start_y;
					start_y = tmp;
				}
				for (int x = Mathf.FloorToInt(start_x); x <= Mathf.FloorToInt(end_x); x++) // loop through all tiles and set their tiles
				{
					for (int y = Mathf.FloorToInt(start_y); y <= Mathf.FloorToInt(end_y); y++)
					{
						GameObject GO = SimplePool.Spawn(this.selectPrefab, new Vector3(x, y, -1f), Quaternion.identity); // use simplePool to pool the selection prefabs
						GO.transform.SetParent(this.transform);
						selectGOs.Add(GO);
					}
				}
			} 
		}
	}
	void updateRightMouse()
	{
		if (Input.GetMouseButtonDown(1)) // if the right mouse is clicked this frame then we set the start for the box and the isDeleting flag
		{
			this.mouseStart = mousePos; // this way we are prioritizing deletion
			this.isDeleting = true;
		}
		if (Input.GetMouseButtonUp(1)) // if the right mouse is let up this frame 
		{
			if (isDeleting)
			{
				if (mousePos.x < mouseStart.x) // invert them if the end is less than the start
				{
					float tmp;
					tmp = this.mouseStart.x;
					this.mouseStart.x = mousePos.x;
					mousePos.x = tmp;
				}
				if (mousePos.y < mouseStart.y) // invert them if the end is less than the start
				{
					float tmp;
					tmp = mouseStart.y;
					mouseStart.y = mousePos.y;
					mousePos.y = tmp;
				}
				for (int x = Mathf.FloorToInt(this.mouseStart.x); x <= Mathf.FloorToInt(mousePos.x); x++) // loop through all tiles and set their tiles
				{
					for (int y = Mathf.FloorToInt(this.mouseStart.y); y <= Mathf.FloorToInt(this.mousePos.y); y++)
					{
						if (this.placeMode == 0) // we are in tile delete mode
						{
							this.worldController.GetComponent<worldController>().getWorld().setTileAt(x, y, "grass", 0);		// TODO use a break time system? 
																																// or maybe only can break one at a time?
						}
						else if (this.placeMode == 1)// we are in object delete mode
						{
							this.worldController.GetComponent<worldController>().getWorld().destroyWorldObject(
								this.worldController.GetComponent<worldController>().getWorld().getTileAt(x, y));
                        }
						else
						{
							Debug.Log("Not a valid place mode");
						}
					}
				}
			}
		}
		if (Input.GetMouseButton(1)) // if right mouse is held
		{
			if (isDeleting)
			{
				int start_x = Mathf.FloorToInt(mouseStart.x);
				int end_x = Mathf.FloorToInt(mousePos.x);
				int start_y = Mathf.FloorToInt(mouseStart.y);
				int end_y = Mathf.FloorToInt(mousePos.y);
				if (end_x < start_x) // invert them if the end is less than the start
				{
					int tmp;
					tmp = end_x;
					end_x = start_x;
					start_x = tmp;
				}
				if (end_y < start_y) // invert them if the end is less than the start
				{
					int tmp;
					tmp = end_y;
					end_y = start_y;
					start_y = tmp;
				}
				for (int x = Mathf.FloorToInt(start_x); x <= Mathf.FloorToInt(end_x); x++) // loop through all tiles and set their tiles
				{
					for (int y = Mathf.FloorToInt(start_y); y <= Mathf.FloorToInt(end_y); y++)
					{
						GameObject GO = SimplePool.Spawn(this.deletePrefab, new Vector3(x, y, -1f), Quaternion.identity); // use simplePool to pool the selection prefabs
						GO.transform.SetParent(this.transform);
						selectGOs.Add(GO);
					}
				}
			}
		}

	}
	public void setPlaceMode(int mode)
	{
		this.placeMode = mode;
	}
}
