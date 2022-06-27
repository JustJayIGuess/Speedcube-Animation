using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece3x3
{
	public abstract class TurnSmoother
	{
		public static readonly float twoOnRootTwo = 2f / Mathf.Sqrt(2f);
		public static readonly float twoOnTwoMinusRootTwo = 2f / (2f - Mathf.Sqrt(2f));
		public static readonly float pi = Mathf.PI;
		public static readonly float halfPi = Mathf.PI / 2f;

		public abstract float SmoothFloat(float t);

		public abstract float MidPoint { get; }
		public virtual float SmoothFloatFirstHalf(float t)
		{
			return 2f * SmoothFloat(MidPoint * t);
		}
		public virtual float SmoothFloatSecondHalf(float t)
		{
			return 2f * SmoothFloat((1f - MidPoint) * (t + 1f / ((1f / MidPoint) - 1f))) - 1f;
		}
	}

	//public sealed class 

	public sealed class TurnSmoothers
	{
		public class Lerp : TurnSmoother
		{
			public override float MidPoint { get { return 0.5f; } }

			public override float SmoothFloat(float t)
			{
				return t;
			}

			public override float SmoothFloatFirstHalf(float t)
			{
				return t;
			}

			public override float SmoothFloatSecondHalf(float t)
			{
				return t;
			}
		}

		public class QuadraticEaseOut : TurnSmoother
		{
			public override float MidPoint { get { return 0.292893f; } }

			public override float SmoothFloat(float t)
			{
				return -t * t + t * 2f;
			}
		}

		public class CubicEaseOut : TurnSmoother
		{
			public override float MidPoint { get { return 0.206299f; } }

			public override float SmoothFloat(float t)
			{
				return (t - 1f) * (t - 1f) * (t - 1f) + 1f;
			}
		}

		public class SineEaseOutExact : TurnSmoother
		{
			public override float MidPoint { get { return 1f / 3f; } }

			public override float SmoothFloat(float t)
			{
				return -2f * Mathf.Cos(halfPi * (2f * t + 4f) / 3f) - 1f;
			}
		}
		public class SineEaseOutApprox : TurnSmoother
		{
			public override float MidPoint { get { return 1f / 3f; } }

			public override float SmoothFloat(float t)
			{
				return -2f * Mathf.Cos(halfPi * (2f * t + 4f) / 3f) - 1f;
			}

			public override float SmoothFloatFirstHalf(float t)
			{
				return t;
			}
		}


		public class Bezier : TurnSmoother
		{
			public override float MidPoint { get { return 0.5f; } }

			public override float SmoothFloat(float t)
			{
				return t * t * (3f - (2f * t));
			}
		}

		public class SmoothStep : TurnSmoother
		{
			public override float MidPoint { get { return 0.5f; } }

			public override float SmoothFloat(float t)
			{
				return Mathf.SmoothStep(0f, 1f, t);
			}
		}

		public class SquareRoot : TurnSmoother
		{
			public override float MidPoint { get { return 0.25f; } }

			public override float SmoothFloat(float t)
			{
				return Mathf.Sqrt(t);
			}
		}
	}

	public enum PieceTypes
	{
		Center = 1,
		Edge = 2,
		Corner = 3
	}

	public enum StickerColors
	{
		White,
		Yellow,
		Blue,
		Orange,
		Green,
		Red
	}

	public enum SmoothingTypes
	{
		None,
		LerpExp,
		SineEaseOut,
		Sine
	}

	public static Dictionary<StickerColors, Color> ColorOptions { get; set; } = new Dictionary<StickerColors, Color>()
	{
		{ StickerColors.White, Color.white },
		{ StickerColors.Yellow, Color.yellow },
		{ StickerColors.Red, Color.red },
		{ StickerColors.Green, Color.green },
		{ StickerColors.Orange, new Color(1f, 0.6f, 0f, 1f) },
		{ StickerColors.Blue, Color.blue }
	};
	public Transform Root { get; set; }
	public GameObject PieceGameObject { get; set; }
	public PieceTypes PieceType { get; set; }
	public static float DoubleTurnSwing { get => doubleTurnSwing; set => doubleTurnSwing = value; }

	private readonly Material stickerBaseMaterial;
	private readonly Renderer pieceRenderer;

	private (Vector3 targetPosition, Quaternion targetRotation)? currentRotationTarget;
	private static float doubleTurnSwing = 0.75f;

	public Piece3x3(GameObject pieceGameObject, Transform root, Color[] stickerColors, Material stickerMaterial)
	{
		PieceGameObject = pieceGameObject;
		Root = root;
		stickerBaseMaterial = stickerMaterial;
		PieceType = (PieceTypes)stickerColors.Length;

		pieceRenderer = PieceGameObject.GetComponent<Renderer>();

		if (stickerColors != null)
		{
			Material[] stickerMaterials = new Material[stickerColors.Length + 1];
			stickerMaterials[0] = pieceRenderer.sharedMaterials[0];
			for (int i = 0; i < stickerColors.Length; i++)
			{
				stickerMaterials[i + 1] = stickerBaseMaterial;
				MaterialPropertyBlock stickerBlock = new MaterialPropertyBlock();

				pieceRenderer.GetPropertyBlock(stickerBlock, i + 1);
				stickerBlock.SetColor("_BaseColor", stickerColors[i]);

				pieceRenderer.SetPropertyBlock(stickerBlock, i + 1);
			}
			pieceRenderer.materials = stickerMaterials;
		}
	}

	public IEnumerator HighlightSticker(int stickerIndex, Color highlightColor, float targetIntensity = 1f, float speed = 1f, Action callback = null)
	{
		Color targetColor = highlightColor * targetIntensity;

		MaterialPropertyBlock stickerBlock = new MaterialPropertyBlock();
		pieceRenderer.GetPropertyBlock(stickerBlock, stickerIndex + 1);

		Color currColor = stickerBlock.GetColor("_EmissiveColor");
		float elapsed = 0f;
		while (elapsed + Time.deltaTime * speed < 1f)
		{
			pieceRenderer.GetPropertyBlock(stickerBlock, stickerIndex + 1);

			stickerBlock.SetColor("_EmissiveColor", Color.Lerp(currColor, targetColor, Utils.EaseInAndOut(elapsed)));

			pieceRenderer.SetPropertyBlock(stickerBlock, stickerIndex + 1);

			elapsed += Time.deltaTime * speed;
			yield return null;
		}

		pieceRenderer.GetPropertyBlock(stickerBlock, stickerIndex + 1);

		stickerBlock.SetColor("_EmissiveColor", targetColor);

		pieceRenderer.SetPropertyBlock(stickerBlock, stickerIndex + 1);

		callback?.Invoke();
	}

	public void RotateToInstant(float angle, Vector3 axis)
	{
		Vector3 rotationCenter = Root.position + 2f * axis;

		Quaternion targetRotationLocal = Quaternion.AngleAxis(angle, axis);
		Vector3 targetPositionLocal = targetRotationLocal * (PieceGameObject.transform.localPosition - rotationCenter);

		PieceGameObject.transform.localPosition = targetPositionLocal + rotationCenter;
		PieceGameObject.transform.localRotation = targetRotationLocal * PieceGameObject.transform.localRotation;
	}

	public IEnumerator RotateToSmooth<Smoother>(float angle, Vector3 axis, float speed = 1f, float postDelay = 0.1f, Action callback = null) where Smoother : TurnSmoother, new()
	{
		Smoother smoother = new Smoother();
		float originalSpeed = speed;
		bool isDoubleTurn = angle == 180f || angle == -180f;

		Vector3 rotationCenter = Root.localPosition + 2f * axis;

		if (isDoubleTurn)
		{
			angle /= 2f;
			speed = originalSpeed * (1f / smoother.MidPoint) * DoubleTurnSwing;
		}

		Quaternion targetRotationLocal = Quaternion.AngleAxis(angle, axis);

		Vector3 targetPositionLocal = targetRotationLocal * (PieceGameObject.transform.localPosition - rotationCenter);
		Quaternion targetRotation = targetRotationLocal * PieceGameObject.transform.localRotation;

		if (isDoubleTurn)
		{
			Quaternion targetRotationLocalTemp = Quaternion.AngleAxis(180f, axis);

			Vector3 targetPositionLocalTemp = targetRotationLocalTemp * (PieceGameObject.transform.localPosition - rotationCenter);
			Quaternion targetRotationTemp = targetRotationLocalTemp * PieceGameObject.transform.localRotation;

			currentRotationTarget = (targetPositionLocalTemp + rotationCenter, targetRotationTemp);
		}
		else
		{
			currentRotationTarget = (targetPositionLocal + rotationCenter, targetRotation);
		}

		Quaternion startRotation = PieceGameObject.transform.localRotation;
		Vector3 startPosition = PieceGameObject.transform.localPosition;

		float elapsed = 0f;
		while (elapsed + Time.deltaTime * speed < 1f)
		{
			float t = isDoubleTurn ? smoother.SmoothFloatFirstHalf(elapsed) : smoother.SmoothFloat(elapsed);
			PieceGameObject.transform.localPosition = Vector3.Slerp(startPosition - rotationCenter, targetPositionLocal, t) + rotationCenter;
			PieceGameObject.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
			elapsed += Time.deltaTime * speed;
			yield return null;
		}

		if (isDoubleTurn)
		{
			PieceGameObject.transform.localPosition = targetPositionLocal + rotationCenter;
			PieceGameObject.transform.localRotation = targetRotation;

			startPosition = PieceGameObject.transform.localPosition;
			startRotation = PieceGameObject.transform.localRotation;

			targetRotationLocal = Quaternion.AngleAxis(angle, axis);

			targetPositionLocal = targetRotationLocal * (PieceGameObject.transform.localPosition - rotationCenter);
			targetRotation = targetRotationLocal * PieceGameObject.transform.localRotation;

			elapsed = 0f;
			speed = originalSpeed * (1f / (1f - smoother.MidPoint)) * DoubleTurnSwing;
			while (elapsed + Time.deltaTime * speed < 1f)
			{
				float t = smoother.SmoothFloatSecondHalf(elapsed);
				PieceGameObject.transform.localPosition = Vector3.Slerp(startPosition - rotationCenter, targetPositionLocal, t) + rotationCenter;
				PieceGameObject.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
				elapsed += Time.deltaTime * speed;
				yield return null;
			}
		}

		PieceGameObject.transform.localPosition = targetPositionLocal + rotationCenter;
		PieceGameObject.transform.localRotation = targetRotation;
		currentRotationTarget = null;

		yield return new WaitForSeconds(postDelay);

		callback?.Invoke();
	}

	public void CompleteTurnsInstantly()
	{
		if (currentRotationTarget != null) {
			PieceGameObject.transform.localPosition = (Vector3)currentRotationTarget?.targetPosition;
			PieceGameObject.transform.localRotation = (Quaternion)currentRotationTarget?.targetRotation;

			currentRotationTarget = null;
		}
	}
}
