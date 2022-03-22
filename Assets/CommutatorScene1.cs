using System.Collections;
using UnityEngine;


public class CommutatorScene1 : CubeScene3x3
{
	protected override IEnumerator Scene()
	{
		cube.ExecuteAlgorithmInstant("R U R' U' R' F R2 U' R' U' R U R' F' U2");

		yield return new WaitForSeconds(1f);

		yield return StartCoroutine(cube.ExecuteMoveSmoothCoroutine<Piece3x3.TurnSmoothers.CubicEaseOut>(new CubeController3x3.Move(CubeController3x3.CubeMotionGroups.U, CubeController3x3.CubeTurns.DoubleClockwise)));

		cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.L, HighlightColors.White);
		cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R, HighlightColors.Blue);
		yield return new WaitForSeconds(1f);

		cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R | CubeController3x3.CubeFaces.F, HighlightColors.Green);
		cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R | CubeController3x3.CubeFaces.B, HighlightColors.Red);
		yield return new WaitForSeconds(2f);

		cube.MoveSpeed = 5f;
		yield return StartCoroutine(cube.ExecuteAlgorithmSmoothCoroutine<Piece3x3.TurnSmoothers.CubicEaseOut>("R U R' U' R' F R2 U' R' U' R U R' F'"));
		yield return new WaitForSeconds(0.2f);

		for (int i = 0; i < 7; i++)
		{
			cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R | CubeController3x3.CubeFaces.B, HighlightColors.Green, i % 2, 5f);
			cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R | CubeController3x3.CubeFaces.F, HighlightColors.Red, i % 2, 5f);
			cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.R, HighlightColors.White, i % 2, 5f);
			cube.HighlightPieceAsync(CubeController3x3.CubeFaces.U | CubeController3x3.CubeFaces.L, HighlightColors.Blue, i % 2, 5f);
			yield return new WaitForSeconds(0.2f);
		}

	}
}
