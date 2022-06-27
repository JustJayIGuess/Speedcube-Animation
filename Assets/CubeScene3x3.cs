using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScene3x3 : MonoBehaviour
{
	public static class HighlightColors
	{
		public static Color White { get; private set; } = new Color(1f, 1f, 1f, 1f);
		public static Color Red { get; private set; } = new Color(1f, 0.2f, 0.3f, 1f);
		public static Color Green { get; private set; } = new Color(0.2f, 1f, 0.3f, 1f);
		public static Color Blue { get; private set; } = new Color(0.4f, 0.4f, 1f, 1f);
		public static Color Yellow { get; private set; } = new Color(1f, 1f, 0.4f, 1f);
		public static Color Orange { get; private set; } = new Color(1f, 0.7f, 0f, 1f);
	}

    protected CubeController3x3 cube;
    public CubeScene3x3 nextScene = null;

    void Start()
    {
        cube = FindObjectOfType<CubeController3x3>();
    }

    protected virtual IEnumerator Scene()
	{
        StartCoroutine(nextScene.Scene());
        yield return null;
	}

    public void BeginSequence()
	{
        StartCoroutine(Scene());
	}
}
