using UnityEngine;

public class Edge3x3 : Piece3x3
{
	public static PieceProperties3x3[] EdgePropertyDefaults { get; set; } = new PieceProperties3x3[]
	{
		new PieceProperties3x3(new Vector3(-1f,  0f, -1f), Quaternion.Euler(  0f, 180f, -90f), new Color[] { ColorOptions[StickerColors.Blue],   ColorOptions[StickerColors.Orange]    }),
		new PieceProperties3x3(new Vector3(-1f, -1f,  0f), Quaternion.Euler(  0f, -90f, 180f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Blue]   }),
		new PieceProperties3x3(new Vector3(-1f,  0f,  1f), Quaternion.Euler(  0f,   0f,  90f), new Color[] { ColorOptions[StickerColors.Blue],   ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3(-1f,  1f,  0f), Quaternion.Euler(  0f, -90f,   0f), new Color[] { ColorOptions[StickerColors.White],  ColorOptions[StickerColors.Blue]   }),
		new PieceProperties3x3(new Vector3( 0f, -1f, -1f), Quaternion.Euler(180f,   0f,   0f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Orange]    }),
		new PieceProperties3x3(new Vector3( 0f, -1f,  1f), Quaternion.Euler(  0f,   0f, 180f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3( 0f,  1f,  1f), Quaternion.Euler(  0f,   0f,   0f), new Color[] { ColorOptions[StickerColors.White],  ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3( 0f,  1f, -1f), Quaternion.Euler(  0f, 180f,   0f), new Color[] { ColorOptions[StickerColors.White],  ColorOptions[StickerColors.Orange]    }),
		new PieceProperties3x3(new Vector3( 1f,  0f, -1f), Quaternion.Euler(  0f, 180f,  90f), new Color[] { ColorOptions[StickerColors.Green],  ColorOptions[StickerColors.Orange]    }),
		new PieceProperties3x3(new Vector3( 1f, -1f,  0f), Quaternion.Euler(  0f,  90f, 180f), new Color[] { ColorOptions[StickerColors.Yellow], ColorOptions[StickerColors.Green]  }),
		new PieceProperties3x3(new Vector3( 1f,  0f,  1f), Quaternion.Euler(  0f,   0f, -90f), new Color[] { ColorOptions[StickerColors.Green],  ColorOptions[StickerColors.Red] }),
		new PieceProperties3x3(new Vector3( 1f,  1f,  0f), Quaternion.Euler(  0f,  90f,   0f), new Color[] { ColorOptions[StickerColors.White],  ColorOptions[StickerColors.Green]  })
	};

	public Edge3x3(GameObject pieceGameObject, Transform root, Color[] stickerColors, Material stickerBaseMaterial) : base(pieceGameObject, root, stickerColors, stickerBaseMaterial)
	{

	}
}
