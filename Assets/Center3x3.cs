using UnityEngine;

public class Center3x3 : Piece3x3
{
	public static PieceProperties3x3[] CenterPropertyDefaults { get; set; } = new PieceProperties3x3[]
	{
		new PieceProperties3x3(new Vector3( 0f,  0f,  1f), Quaternion.Euler( 90f, 0f,   0f), new Color[] { ColorOptions[StickerColors.Orange] }),
		new PieceProperties3x3(new Vector3( 0f,  1f,  0f), Quaternion.Euler(  0f, 0f,   0f), new Color[] { ColorOptions[StickerColors.White] }),
		new PieceProperties3x3(new Vector3( 1f,  0f,  0f), Quaternion.Euler(  0f, 0f, -90f), new Color[] { ColorOptions[StickerColors.Green] }),
		new PieceProperties3x3(new Vector3( 0f,  0f, -1f), Quaternion.Euler(-90f, 0f,   0f), new Color[] { ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3( 0f, -1f,  0f), Quaternion.Euler(  0f, 0f, 180f), new Color[] { ColorOptions[StickerColors.Yellow] }),
		new PieceProperties3x3(new Vector3(-1f,  0f,  0f), Quaternion.Euler(  0f, 0f,  90f), new Color[] { ColorOptions[StickerColors.Blue] })
	};

	public Center3x3(GameObject pieceGameObject, Transform root, Color[] stickerColors, Material stickerBaseMaterial) : base(pieceGameObject, root, stickerColors, stickerBaseMaterial)
	{

	}
}
