using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<CubeScene3x3> scenes = new List<CubeScene3x3>();

	private void Start()
	{
		for (int i = 0; i < scenes.Count - 1; i++)
		{
			scenes[i].nextScene = scenes[i + 1];
		}

		scenes[0].BeginSequence();
	}
}
