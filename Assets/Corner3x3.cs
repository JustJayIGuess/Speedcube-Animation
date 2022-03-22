using UnityEngine;

public class Corner3x3 : Piece3x3
{
	public static PieceProperties3x3[] CornerPropertyDefaults { get; set; } = new PieceProperties3x3[]
	{
		new PieceProperties3x3(new Vector3(-1f, -1f, -1f), Quaternion.Euler(180f,  90f,   0f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Blue], ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3(-1f, -1f,  1f), Quaternion.Euler(  0f,   0f, 180f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Orange], ColorOptions[StickerColors.Blue] }),
		new PieceProperties3x3(new Vector3(-1f,  1f, -1f), Quaternion.Euler(  0f, 180f,   0f), new Color[] { ColorOptions[StickerColors.White], ColorOptions[StickerColors.Red], ColorOptions[StickerColors.Blue] }),
		new PieceProperties3x3(new Vector3(-1f,  1f,  1f), Quaternion.Euler(  0f, -90f,   0f), new Color[] { ColorOptions[StickerColors.White], ColorOptions[StickerColors.Blue], ColorOptions[StickerColors.Orange] }),
		new PieceProperties3x3(new Vector3( 1f, -1f, -1f), Quaternion.Euler(180f,   0f,   0f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Red], ColorOptions[StickerColors.Green] }),
		new PieceProperties3x3(new Vector3( 1f, -1f,  1f), Quaternion.Euler(180f, -90f,   0f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Green], ColorOptions[StickerColors.Orange] }),
		new PieceProperties3x3(new Vector3( 1f,  1f, -1f), Quaternion.Euler(  0f,  90f,   0f), new Color[] { ColorOptions[StickerColors.White], ColorOptions[StickerColors.Green], ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3( 1f,  1f,  1f), Quaternion.Euler(  0f,   0f,   0f), new Color[] { ColorOptions[StickerColors.White], ColorOptions[StickerColors.Orange], ColorOptions[StickerColors.Green] })
	};

	public Corner3x3(GameObject pieceGameObject, Transform root, Color[] stickerColors, Material stickerBaseMaterial) : base (pieceGameObject, root, stickerColors, stickerBaseMaterial)
	{

	}
}
