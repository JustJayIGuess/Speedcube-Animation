using System.Collections;
using UnityEngine;

public class PiecesScene2A : CubeScene3x3
{
	private Coroutine[] whiteStickersCoroutines = new Coroutine[9];
	private IEnumerator HighlightUpperFaceWhiteStickers()
	{
		// I know i can probably loop over the enums but this isnt scaling to a larger number anytime soon so just look away
		whiteStickersCoroutines[0] = Focus(U, 0);
		whiteStickersCoroutines[1] = Focus(U | R, 0);
		whiteStickersCoroutines[2] = Focus(U | L, 0);
		whiteStickersCoroutines[3] = Focus(U | F, 0);
		whiteStickersCoroutines[4] = Focus(U | B, 0);
		whiteStickersCoroutines[5] = Focus(U | F | R, 0);
		whiteStickersCoroutines[6] = Focus(U | F | L, 0);
		whiteStickersCoroutines[7] = Focus(U | B | R, 0);
		whiteStickersCoroutines[8] = Focus(U | B | L, 0);

		foreach (Coroutine coroutine in whiteStickersCoroutines)
		{
			yield return coroutine;
		}
	}

	protected override IEnumerator Scene()
	{
		//yield return new WaitForSeconds(1f);

		//cube.MoveSpeed = 9f;
		//cube.AlgorithmInterMoveDelay = 0f;
		//yield return Execute("R U R' U' R U R' U'");

		//cube.AlgorithmInterMoveDelay = 0.1f;
		//cube.MoveSpeed = 7f;
		//yield return Execute("R U R' U' R U R' U'");

		//cube.MoveSpeed = 1.8f;
		//yield return Execute("R U R' U' R U R' U'");

		yield return new WaitForSeconds(1f);

		yield return Execute("Y2 X2 Z2");

		yield return new WaitForSeconds(1f);

		yield return Focus(U);
		yield return new WaitForSeconds(1f);
		yield return Unhighlight(U);
		yield return Highlight(U, HighlightColors.White);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U);

		yield return Focus(U | R);
		yield return new WaitForSeconds(1f);
		yield return Unhighlight(U | R);
		yield return Highlight(U | R, 0, HighlightColors.White);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U | R);
		yield return Highlight(U | R, 1, HighlightColors.Red);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U | R);

		yield return Focus(U | R | F);
		yield return new WaitForSeconds(1f);
		yield return Unhighlight(U | R | F);
		yield return Highlight(U | R | F, 0, HighlightColors.White);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U | R | F);
		yield return Highlight(U | R | F, 1, HighlightColors.Red);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U | R | F);
		yield return Highlight(U | R | F, 2, HighlightColors.Green);
		yield return new WaitForSeconds(0.2f);
		yield return Unhighlight(U | R | F);

		yield return new WaitForSeconds(1f);

		yield return Execute("Y2");
		yield return Focus(U | B);
		yield return new WaitForSeconds(0.5f);

		cube.MoveSpeed = 20f;
		cube.AlgorithmInterMoveDelay = 0f;
		yield return Execute<Piece3x3.TurnSmoothers.Lerp>("D' R' B' D R2 U' F' D U F' R' D2 B2 L2 R' U F' D L' U2 L2 R2 B' F U2 L2 U B' R2 B2");
		cube.MoveSpeed = 2f;
		cube.AlgorithmInterMoveDelay = 0.1f;
		yield return Execute("X");

		yield return new WaitForSeconds(1f);

		yield return Unhighlight(D | B);

		yield return Execute("X' Y2");
		cube.MoveSpeed = 5f;
		cube.AlgorithmInterMoveDelay = 0.05f;
		yield return Execute("F' L' B' R' B2' D B D' R' D R D' L D L'");
		cube.MoveSpeed = 2f;
		cube.AlgorithmInterMoveDelay = 0.1f;

		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(HighlightUpperFaceWhiteStickers());
		yield return new WaitForSeconds(0.5f);
		yield return Unhighlight(CubeController3x3.CubeMotionGroups.U);

		yield return new WaitForSeconds(1f);
		yield return Focus(CubeController3x3.CubeMotionGroups.U);
		yield return new WaitForSeconds(0.5f);
		yield return Unhighlight(CubeController3x3.CubeMotionGroups.U);

		yield return new WaitForSeconds(1f);

		cube.MoveSpeed = 5f;
		cube.AlgorithmInterMoveDelay = 0.05f;
		yield return Execute("F2 L2 D' L2 D2 F2 D R D2' R' D' R D R' D2 L' D' L2 D' L' D' R' D R");
		cube.MoveSpeed = 2f;
		cube.AlgorithmInterMoveDelay = 0.1f;
		yield return Execute("Y Y Y Y");

		cube.MoveSpeed = 20f;
		cube.AlgorithmInterMoveDelay = 0f;
		yield return new WaitForSeconds(1f);
		yield return Execute("B2 D B2 U F' U D' F L F2 B2 D' L2 U R2 U B2 D B2");

	}

}
