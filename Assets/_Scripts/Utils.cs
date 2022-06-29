using UnityEngine;

public static class Utils
{
	public static readonly float twoOnRootTwo = 2f / Mathf.Sqrt(2f);
	public static readonly float twoOnTwoMinusRootTwo = 2f / (2f - Mathf.Sqrt(2f));
	public static readonly float pi = Mathf.PI;
	public static readonly float halfPi = Mathf.PI / 2f;

    public static float EaseOut(float t)
	{
		return -2f * Mathf.Cos(halfPi * (2f * t + 4f) * 0.333333f) - 1f;
	}

	public static float EaseIn(float t)
	{
		return 1f - Mathf.Cos(halfPi * t);
	}

	public static float EaseInAndOut(float t)
	{
		return 0.5f - 0.5f * Mathf.Cos(Mathf.PI * t);
	}

	public static float EaseOutFirstHalf(float t)
	{
		return t; // approx
	}

	public static float EaseOutSecondHalf(float t)
	{
		return -twoOnTwoMinusRootTwo * (Mathf.Cos(pi * (t + 3f) / 4f) + 1f) + 1f;
	}
}
