using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
	IPoolManager PoolManager { get; set; }

	void init();
}
