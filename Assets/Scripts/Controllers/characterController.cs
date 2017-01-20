using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterController : MonoBehaviour
{
	[Header("Variables")]
	public string spriteName;

	[Header("Objects")]
	[Space(5)]
	public GameObject worldController;
	public GameObject lookPointPrefab; // HACK find a better way than a look point

	Vector2 lastMoveDir;
	public bool isBlocked { get; set; }

	Dictionary<character, GameObject> charToGO;

	void Awake()
	{
		setup();
	}
	void setup()
	{
		charToGO = new Dictionary<character, GameObject>();
	}

	// Update is called once per frame
	void Update ()
	{
		this.moveCharacter();
	}
	void moveCharacter() 
	{
		foreach (KeyValuePair<character, GameObject> c in charToGO)
		{
			Vector2 moveDir;
			moveDir = new Vector2(inputManager.instance.getAxis("horizontal"), inputManager.instance.getAxis("vertical"));
			if (moveDir != Vector2.zero)
			{
				this.lastMoveDir = moveDir;
				c.Value.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg));
			}
			if (!isBlocked)
			{
				c.Key.moveChar(moveDir);
			}
		}

    }

	public void addGO(character c)
	{

		GameObject charGO = new GameObject("mainCharacter_" + c.name);
		charGO.transform.SetParent(this.transform);
		charGO.transform.position = new Vector3(c.position.x, c.position.y, -2); // setup the main character game object
		charGO.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		SpriteRenderer charSR = charGO.AddComponent<SpriteRenderer>();
		charSR.sprite = spriteManager.instance.getSprite("char", spriteName);

		GameObject lpGO = (GameObject)Instantiate(this.lookPointPrefab, charGO.transform);
		lpGO.transform.localPosition = new Vector3(0.1f, 0, 2);
		lpGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

		c.registerMoveCallback((character) => { this.onCharMoved(character, charGO); });
		charToGO.Add(c, charGO);
	}

	//?+ callbacks
	public void onCharMoved(character charData, GameObject charGO) // when a character moves, we update the GO pos
	{
		charGO.transform.position = new Vector3(charData.position.x, charData.position.y, -2);
	}
}
