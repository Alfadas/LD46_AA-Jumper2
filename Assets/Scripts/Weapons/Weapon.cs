using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public virtual bool Safety
	{
		get; set;
	}

	public virtual void pullTrigger()
	{
	}

	public virtual void releaseTrigger()
	{
	}

	public virtual void aim()
	{
	}

	public virtual void unaim()
	{
	}

	public virtual void reload()
	{
	}

	public virtual void switchFireMode(int firemode = -1)
	{
	}
}
