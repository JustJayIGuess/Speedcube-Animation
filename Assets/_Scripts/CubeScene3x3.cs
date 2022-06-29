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

	protected const CubeController3x3.CubeFaces U = CubeController3x3.CubeFaces.U;
	protected const CubeController3x3.CubeFaces D = CubeController3x3.CubeFaces.D;
	protected const CubeController3x3.CubeFaces L = CubeController3x3.CubeFaces.L;
	protected const CubeController3x3.CubeFaces R = CubeController3x3.CubeFaces.R;
	protected const CubeController3x3.CubeFaces F = CubeController3x3.CubeFaces.F;
	protected const CubeController3x3.CubeFaces B = CubeController3x3.CubeFaces.B;

    void Start()
    {
        cube = FindObjectOfType<CubeController3x3>();
    }

    protected virtual IEnumerator Scene()
	{
        StartCoroutine(nextScene.Scene());
        yield return null;
	}

	protected Coroutine Execute(string move)
	{
		return StartCoroutine(cube.ExecuteAlgorithmSmoothCoroutine<Piece3x3.TurnSmoothers.SmoothStep>(move));
	}

	protected Coroutine Execute<Smoother>(string move) where Smoother : Piece3x3.TurnSmoother, new()
	{
		return StartCoroutine(cube.ExecuteAlgorithmSmoothCoroutine<Smoother>(move));
	}

	protected Coroutine Highlight(CubeController3x3.CubeFaces faceMask, Color color, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, color, 1f, speed));
	}

	protected Coroutine Focus(CubeController3x3.CubeFaces faceMask, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, HighlightColors.White, 0.5f, speed));
	}

	protected Coroutine Unhighlight(CubeController3x3.CubeFaces faceMask, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, HighlightColors.White, 0f, speed));
	}

	protected Coroutine Highlight(CubeController3x3.CubeFaces faceMask, int stickerIndex, Color color, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, stickerIndex, color, 1f, speed));
	}

	protected Coroutine Focus(CubeController3x3.CubeFaces faceMask, int stickerIndex, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, stickerIndex, HighlightColors.White, 0.5f, speed));
	}

	protected Coroutine Unhighlight(CubeController3x3.CubeFaces faceMask,  int stickerIndex, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightPieceCoroutine(faceMask, stickerIndex, HighlightColors.White, 0f, speed));
	}


	protected Coroutine Highlight(CubeController3x3.CubeMotionGroups layerMask, Color color, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightLayerCoroutine(layerMask, color, 1f));
	}

	protected Coroutine Focus(CubeController3x3.CubeMotionGroups layerMask, float speed = 5f)
	{
		return StartCoroutine(cube.HighlightLayerCoroutine(layerMask, HighlightColors.White, 0.5f));
	}

	protected Coroutine Unhighlight(CubeController3x3.CubeMotionGroups layerMask)
	{
		return StartCoroutine(cube.HighlightLayerCoroutine(layerMask, HighlightColors.White, 0f));
	}

	public void BeginSequence()
	{
        StartCoroutine(Scene());
	}
}
