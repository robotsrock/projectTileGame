using UnityEngine;
using System.Collections;
using System;

public class character // REFACTOR use properties
{
	Vector2 position;
	string name;
	float moveSpeed;
	Action<character> onMoveCB;

	public character(Vector2 position, string name, float moveSpeed, world theWorld)
	{
		this.position = position;
		this.name = name;
		this.moveSpeed = moveSpeed;
	}
	public void moveChar(Vector2 dir)
	{
		Vector2 normDir = Vector3.Normalize(dir);
		this.position += normDir * this.moveSpeed;
		if (this.onMoveCB != null)
		{
			this.onMoveCB(this);
		}
    }
	public Vector2 getPosition()
	{
		return this.position;
	}
	public void registerMoveCallback(Action<character> callback)
	{
		onMoveCB += callback;
	}
	public void unRegisterMoveCallback(Action<character> callback)
	{
		onMoveCB -= callback;
	}
	public string getName()
	{
		return this.name;
	}
}
