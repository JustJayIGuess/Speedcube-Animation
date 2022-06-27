using System.Collections;
using UnityEngine;


public class StructureScene2B : CubeScene3x3
{
	protected override IEnumerator Scene()
	{
		yield return new WaitForSeconds(1f);

		cube.MoveSpeed = 1f;

		yield return Execute("X2");

		cube.MoveSpeed = 3f;

		//yield return StartCoroutine(cube.ExplodeCube<Piece3x3.TurnSmoothers.SmoothStep>());

		yield return Execute("R R2 R2' R'");

		yield return new WaitForSeconds(2f);

		yield return Highlight(F, HighlightColors.Green);
		yield return new WaitForSeconds(0.5f);
		yield return Execute("Y");
		yield return Highlight(B, HighlightColors.Blue);
		yield return new WaitForSeconds(0.5f);

		yield return Unhighlight(F);
		yield return Unhighlight(B);

		yield return StartCoroutine(cube.UnexplodeCube<Piece3x3.TurnSmoothers.SmoothStep>());


		yield return Highlight(R, HighlightColors.Red);
		yield return new WaitForSeconds(0.5f);
		yield return Execute("Y");
		yield return Highlight(L, HighlightColors.Orange);
		yield return new WaitForSeconds(0.5f);

		yield return Unhighlight(L);
		yield return Unhighlight(R);


		yield return Highlight(U, HighlightColors.White);
		yield return new WaitForSeconds(0.5f);
		yield return Execute("X");
		yield return Highlight(D, HighlightColors.Yellow);
		yield return new WaitForSeconds(0.5f);

		yield return Unhighlight(U);
		yield return Unhighlight(D);

		yield return new WaitForSeconds(0.5f);

		// B O Y
		yield return Execute("Y'");
		// B Y R
		yield return Focus(B | D | R);
		yield return new WaitForSeconds(0.5f);
		yield return Unhighlight(B | D | R);


		yield return Focus(D | R);
		yield return new WaitForSeconds(0.5f);
		yield return Unhighlight(D | R);

		yield return Execute("Y");
		// B O Y
		yield return Focus(B | D | L);
		yield return new WaitForSeconds(0.5f);
		yield return Unhighlight(B | D | L);

		yield return Focus(B | L);
		yield return new WaitForSeconds(1.5f);
		_ = Unhighlight(B | L);

		yield return Execute("Z2");
		yield return Execute("X'");
	}
}
