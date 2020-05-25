using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
	[SerializeField] private Weapon weapon = null;

	private void Update()
	{
		if(weapon != null)
		{
			if(Input.GetButtonDown("Fire"))
			{
				weapon.pullTrigger();
			}
			if(Input.GetButtonUp("Fire"))
			{
				weapon.releaseTrigger();
			}
			if(Input.GetButtonDown("Aim"))
			{
				weapon.aim();
			}
			if(Input.GetButtonUp("Aim"))
			{
				weapon.unaim();
			}
			if(Input.GetButtonDown("Reload"))
			{
				weapon.reload();
			}
			if(Input.GetButtonDown("Firemode"))
			{
				weapon.switchFireMode();
			}
		}	
	}
}
