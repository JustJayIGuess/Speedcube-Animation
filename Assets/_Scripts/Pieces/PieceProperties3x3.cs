using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceProperties3x3
{
	public Piece3x3.PieceTypes PieceType { get; private set; }

	public Vector3 DefaultPosition { get; private set; }

	public Quaternion DefaultRotation { get; private set; }

	public Color[] StickerColors { get; private set; }

	public PieceProperties3x3(Vector3 position, Quaternion rotation, Color[] colors)
	{
		DefaultPosition = position;
		DefaultRotation = rotation;
		StickerColors = colors;
		PieceType = (Piece3x3.PieceTypes)colors.Length;
	}
}
