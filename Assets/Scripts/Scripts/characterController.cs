using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour
{
	[Header("Variables")]

	[Header("Objects")]
	[Space(5)]
	public GameObject worldController;

	character mainChar;
	GameObject mainCharGO;
	Vector2 lastMoveDir;
	// Use this for initialization
	void Start ()
	{
		mainChar = this.worldController.GetComponent<worldController>().getMainChar();
		mainCharGO = this.worldController.GetComponent<worldController>().getMainCharGO();
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.moveCharacter();
	}
	void moveCharacter() 
	{
		Vector2 moveDir;
		moveDir = new Vector2(inputManager.instance.getAxis("horizontal"), inputManager.instance.getAxis("vertical"));
		this.mainChar.moveChar(moveDir);
		if (moveDir != Vector2.zero)
		{
			this.lastMoveDir = moveDir;
			this.mainCharGO.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg));
		}
	}
}
