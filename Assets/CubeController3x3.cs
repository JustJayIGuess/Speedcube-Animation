using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.InputSystem;

public class CubeController3x3 : MonoBehaviour
{
	public struct Move
	{
		public enum MoveType
		{
			LayerRotation,
			CubeRotation
		}

		public MoveType moveType;
		public CubeMotionGroups motionGroup;
		public CubeTurns turn;

		public Move(CubeMotionGroups motionGroup, CubeTurns turn)
		{
			this.motionGroup = motionGroup;
			this.turn = turn;
			moveType = ((int)motionGroup & 128) != 0 ? MoveType.CubeRotation : MoveType.LayerRotation;
		}

		public override bool Equals(object obj)
		{
			return obj is Move other &&
				   motionGroup == other.motionGroup &&
				   turn == other.turn;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(motionGroup, turn);
		}

		public void Deconstruct(out CubeMotionGroups motionGroup, out CubeTurns turn)
		{
			motionGroup = this.motionGroup;
			turn = this.turn;
		}

		public static implicit operator (CubeMotionGroups face, CubeTurns turn)(Move value)
		{
			return (value.motionGroup, value.turn);
		}

		public static implicit operator Move((CubeMotionGroups face, CubeTurns turn) value)
		{
			return new Move(value.face, value.turn);
		}
	}

	public enum CubeMotionGroups
	{
        U = 1,
        D = 2,
        L = 4,
        R = 8,
        F = 16,
        B = 32,
		E = 64,
		M = 65,
		S = 66,
		X = 128,
		Y = 129,
		Z = 130
	}

	public enum CubeFaces
	{
		U = 1,
		D = 2,
		L = 4,
		R = 8,
		F = 16,
		B = 32
	}

	public enum CubeTurns
	{
        Clockwise,
        AntiClockwise,
        DoubleClockwise,
        DoubleAntiClockwise,
        None
	}

    public static Dictionary<CubeMotionGroups, int[][]> LayerToCubeIndex { get; private set; } = new Dictionary<CubeMotionGroups, int[][]>()
    {
        { CubeMotionGroups.U, new int[][] { new int[] { 1 }, new int[] { 3,  6, 11, 7 }, new int[] { 2, 3, 7, 6 } } },
        { CubeMotionGroups.D, new int[][] { new int[] { 4 }, new int[] { 4,  9,  5, 1 }, new int[] { 1, 0, 4, 5 } } },
        { CubeMotionGroups.L, new int[][] { new int[] { 3 }, new int[] { 7,  8,  4, 0 }, new int[] { 0, 2, 6, 4 } } },
        { CubeMotionGroups.R, new int[][] { new int[] { 0 }, new int[] { 2,  5, 10, 6 }, new int[] { 3, 1, 5, 7 } } },
        { CubeMotionGroups.F, new int[][] { new int[] { 2 }, new int[] { 11, 10, 9, 8 }, new int[] { 5, 4, 6, 7 } } },
        { CubeMotionGroups.B, new int[][] { new int[] { 5 }, new int[] { 0,   1, 2, 3 }, new int[] { 0, 1, 3, 2 } } },
        { CubeMotionGroups.E, new int[][] { new int[] { 5 }, new int[] { 0,   1, 2, 3 }, new int[] { 0, 1, 3, 2 } } },	// TODO: EMS (not done yet)
        { CubeMotionGroups.M, new int[][] { new int[] { 5 }, new int[] { 0,   1, 2, 3 }, new int[] { 0, 1, 3, 2 } } },
        { CubeMotionGroups.S, new int[][] { new int[] { 5 }, new int[] { 0,   1, 2, 3 }, new int[] { 0, 1, 3, 2 } } }
    };

    public static Dictionary<CubeMotionGroups, Vector3> MotionGroupToAxis { get; private set; } = new Dictionary<CubeMotionGroups, Vector3>()
    {
        { CubeMotionGroups.U, Vector3.up      },
        { CubeMotionGroups.D, Vector3.down    },
        { CubeMotionGroups.L, Vector3.back    },
        { CubeMotionGroups.R, Vector3.forward },
        { CubeMotionGroups.F, Vector3.right   },
        { CubeMotionGroups.B, Vector3.left    },
        { CubeMotionGroups.E, Vector3.up      },
        { CubeMotionGroups.M, Vector3.forward },
        { CubeMotionGroups.S, Vector3.right   },
        { CubeMotionGroups.X, Vector3.forward },
        { CubeMotionGroups.Y, Vector3.up      },
        { CubeMotionGroups.Z, Vector3.right   }
    };

	public static Dictionary<char, CubeMotionGroups> StringToFace { get; private set; } = new Dictionary<char, CubeMotionGroups>()
	{
		{ 'U', CubeMotionGroups.U },
		{ 'D', CubeMotionGroups.D },
		{ 'L', CubeMotionGroups.L },
		{ 'R', CubeMotionGroups.R },
		{ 'F', CubeMotionGroups.F },
		{ 'B', CubeMotionGroups.B },
		{ 'E', CubeMotionGroups.E },
		{ 'M', CubeMotionGroups.M },
		{ 'S', CubeMotionGroups.S },
		{ 'X', CubeMotionGroups.X },
		{ 'Y', CubeMotionGroups.Y },
		{ 'Z', CubeMotionGroups.Z }
	};

	[Header("Camera")]
	[SerializeField]
	private Camera cubeCamera;
	
	[Space(10f)]
	[Header("Roots")]
	public GameObject edgeRoot;
    public GameObject cornerRoot;
    public GameObject centerRoot;

	[Header("Material")]
	[Space(10f)]
	public Material stickerBaseMaterial;

    private readonly Edge3x3[] edges = new Edge3x3[12];
    private readonly Corner3x3[] corners = new Corner3x3[8];
    private readonly Center3x3[] centers = new Center3x3[6];
    private Piece3x3[][] cube = new Piece3x3[3][];

	private List<(Coroutine, Piece3x3)> pieceTurnCoroutines = new List<(Coroutine, Piece3x3)>();
	private Coroutine cameraRotationCoroutine;

	public float MoveSpeed { get; set; } = 2f;
	public float AlgorithmInterMoveDelay { get; set; } = 0.0f;

	private Queue<Move> moveQueue = new Queue<Move>();
	private Coroutine[][][] stickerHighlightCoroutines = new Coroutine[3][][];

	void Awake()
	{
		cube[0] = new Piece3x3[6];
		cube[1] = new Piece3x3[12];
		cube[2] = new Piece3x3[8];

		stickerHighlightCoroutines[0] = new Coroutine[6][];
		stickerHighlightCoroutines[1] = new Coroutine[12][];
		stickerHighlightCoroutines[2] = new Coroutine[8][];

		// Centers
		for (int i = 0; i < 6; i++)
		{
			Vector3 centerPiecePosition = transform.position + 2f * Center3x3.CenterPropertyDefaults[i].DefaultPosition;
			Quaternion centerPieceRotation = transform.rotation * Center3x3.CenterPropertyDefaults[i].DefaultRotation;
			Color[] stickerColors = Center3x3.CenterPropertyDefaults[i].StickerColors;

			centers[i] = new Center3x3(Instantiate(centerRoot, centerPiecePosition, centerPieceRotation, transform), transform, stickerColors, stickerBaseMaterial);
			centers[i].PieceGameObject.name = $"center {i}: " +
				$"{Mathf.RoundToInt(Center3x3.CenterPropertyDefaults[i].DefaultPosition.x)}, " +
				$"{Mathf.RoundToInt(Center3x3.CenterPropertyDefaults[i].DefaultPosition.y)}, " +
				$"{Mathf.RoundToInt(Center3x3.CenterPropertyDefaults[i].DefaultPosition.z)}";

			cube[0][i] = centers[i];
			stickerHighlightCoroutines[0][i] = new Coroutine[3];
		}

		// Edges
		for (int i = 0; i < 12; i++)
		{
			Vector3 edgePiecePosition = transform.position + 2f * Edge3x3.EdgePropertyDefaults[i].DefaultPosition;
			Quaternion edgePieceRotation = transform.rotation * Edge3x3.EdgePropertyDefaults[i].DefaultRotation;
			Color[] stickerColors = Edge3x3.EdgePropertyDefaults[i].StickerColors;


			edges[i] = new Edge3x3(Instantiate(edgeRoot, edgePiecePosition, edgePieceRotation, transform), transform, stickerColors, stickerBaseMaterial);
			edges[i].PieceGameObject.name = $"edge {i}: " +
				$"{Mathf.RoundToInt(Edge3x3.EdgePropertyDefaults[i].DefaultPosition.x)}, " +
				$"{Mathf.RoundToInt(Edge3x3.EdgePropertyDefaults[i].DefaultPosition.y)}, " +
				$"{Mathf.RoundToInt(Edge3x3.EdgePropertyDefaults[i].DefaultPosition.z)}";

			cube[1][i] = edges[i];
			stickerHighlightCoroutines[1][i] = new Coroutine[3];
		}

		// Corners
		for (int i = 0; i < 8; i++)
		{
			Vector3 cornerPiecePosition = transform.position + 2f * Corner3x3.CornerPropertyDefaults[i].DefaultPosition;
			Quaternion cornerPieceRotation = transform.rotation * Corner3x3.CornerPropertyDefaults[i].DefaultRotation;
			Color[] stickerColors = Corner3x3.CornerPropertyDefaults[i].StickerColors;

			corners[i] = new Corner3x3(Instantiate(cornerRoot, cornerPiecePosition, cornerPieceRotation, transform), transform, stickerColors, stickerBaseMaterial);
			corners[i].PieceGameObject.name = $"corner {i}: " +
				$"{Mathf.RoundToInt(Corner3x3.CornerPropertyDefaults[i].DefaultPosition.x)}, " +
				$"{Mathf.RoundToInt(Corner3x3.CornerPropertyDefaults[i].DefaultPosition.y)}, " +
				$"{Mathf.RoundToInt(Corner3x3.CornerPropertyDefaults[i].DefaultPosition.z)}";

			cube[2][i] = corners[i];
			stickerHighlightCoroutines[2][i] = new Coroutine[3];
		}
	}

	private CubeTurns GetTurn(bool isPrime, bool isDouble, bool isNone = false)
	{
		if (isNone)
		{
			return CubeTurns.None;
		}
		else if (isPrime)
		{
			return isDouble ? CubeTurns.DoubleAntiClockwise : CubeTurns.AntiClockwise;
		}
		else
		{
			return isDouble ? CubeTurns.DoubleClockwise : CubeTurns.Clockwise;
		}
	}

	private Move StringToMove(string str)
	{
		char faceCode = str[0];
		bool isPrime = str.Last() == '\'';
		bool isDouble = str.Length > 1 && str[1] == '2';

		CubeTurns turn = GetTurn(isPrime, isDouble);

		if (!StringToFace.TryGetValue(faceCode, out CubeMotionGroups layer))
		{
			Debug.LogError($"StringToMove(string str) encountered char, {faceCode}, which was not present in the move dictionary!");
			throw new Exception($"StringToMove(string str) encountered char, {faceCode}, which was not present in the move dictionary!");
		}

		return new Move(layer, turn);
	}

	// Start is called before the first frame update

	public void SetLayerActive(CubeMotionGroups layer, bool active = false)
	{
        int[][] selectedPieces = LayerToCubeIndex[layer];
		for (int i = 0; i < selectedPieces.Length; i++)
		{
			for (int j = 0; j < selectedPieces[i].Length; j++)
			{
				cube[i][selectedPieces[i][j]].PieceGameObject.SetActive(active);
			}
		}
	}

	public void ExecuteAlgorithmSmoothAsync<Smoother>(string algorithm, Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		string[] moveStrings = algorithm.Split(new char[] { ' ', '.', ',', '/', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string moveString in moveStrings)
		{
			Move move = StringToMove(moveString);
			moveQueue.Enqueue(move);
		}

		ExecuteSmoothQueueRecursiveAsync<Smoother>(callback);
	}

	public IEnumerator ExecuteAlgorithmSmoothCoroutine<Smoother>(string algorithm, Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		string[] moveStrings = algorithm.Split(new char[] { ' ', '.', ',', '/', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string moveString in moveStrings)
		{
			Move move = StringToMove(moveString);
			moveQueue.Enqueue(move);
		}

		yield return StartCoroutine(ExecuteSmoothQueueCoroutine<Smoother>());
		callback?.Invoke();
	}

	//private IEnumerator ExecuteAlgorithmAfterDelayCoroutine(string algorithm, float delay, Action callback = null)
	//{
	//	yield return new WaitForSeconds(delay);
	//	ExecuteAlgorithmSmooth(algorithm, callback);
	//}

	//public void ExecuteAlgorithmSmoothAfterDelay(string algorithm, float delay = 1f, Action callback = null)
	//{
	//	if (algorithmCoroutine != null)
	//	{
	//		StopCoroutine(algorithmCoroutine);
	//	}
	//	algorithmCoroutine = StartCoroutine(ExecuteAlgorithmAfterDelayCoroutine(algorithm, delay, callback));
	//}

	public void ExecuteSmoothQueueRecursiveAsync<Smoother>(Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		if (moveQueue.Count == 0)
		{
			callback?.Invoke();
			return;
		}
		ExecuteMoveSmoothAsync<Smoother>(moveQueue.Dequeue(), MoveSpeed, AlgorithmInterMoveDelay, () => ExecuteSmoothQueueRecursiveAsync<Smoother>(callback));
	}

	public IEnumerator ExecuteSmoothQueueCoroutine<Smoother>(Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		while (moveQueue.Count != 0)
		{
			yield return StartCoroutine(ExecuteMoveSmoothCoroutine<Smoother>(moveQueue.Dequeue(), MoveSpeed));
		}
		callback?.Invoke();
	}

	public void ExecuteMoveInstant(Move move)
	{
		int turnIndexOffset;
		float turnAngle;
		switch (move.turn)
		{
			case CubeTurns.Clockwise:
				turnIndexOffset = 1;
				turnAngle = 90f;
				break;
			case CubeTurns.AntiClockwise:
				turnIndexOffset = 3;
				turnAngle = -90f;
				break;
			case CubeTurns.DoubleClockwise:
				turnIndexOffset = 2;
				turnAngle = 180f;
				break;
			case CubeTurns.DoubleAntiClockwise:
				turnIndexOffset = 2;
				turnAngle = -180f;
				break;
			case CubeTurns.None:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
			default:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
		}

		if (move.moveType == Move.MoveType.LayerRotation)
		{
			foreach ((Coroutine, Piece3x3) pieceCoroutine in pieceTurnCoroutines)
			{
				StopCoroutine(pieceCoroutine.Item1);
				pieceCoroutine.Item2.CompleteTurnsInstantly();
			}
			pieceTurnCoroutines.Clear();

			int[][] selectedPieces = LayerToCubeIndex[move.motionGroup];
			Piece3x3[][] faceTemp = new Piece3x3[][] { new Piece3x3[1], new Piece3x3[4], new Piece3x3[4] };

			for (int i = 0; i < selectedPieces.Length; i++)
			{
				for (int j = 0; j < selectedPieces[i].Length; j++)
				{
					Piece3x3 currentPiece = cube[i][selectedPieces[i][j]];

					currentPiece.RotateToInstant(turnAngle, MotionGroupToAxis[move.motionGroup]);
					faceTemp[i][(j + turnIndexOffset) % selectedPieces[i].Length] = currentPiece;
				}
			}

			for (int i = 0; i < selectedPieces.Length; i++)
			{
				for (int j = 0; j < selectedPieces[i].Length; j++)
				{
					cube[i][selectedPieces[i][j]] = faceTemp[i][j];
				}
			}
		}
		else
		{
			cubeCamera.transform.RotateAround(transform.position, MotionGroupToAxis[move.motionGroup], -turnAngle);
		}
	}

	(int pieceType, int index) GetCubeReferenceFromFaceIntersection(CubeFaces faceIntersection)
	{
		List<CubeMotionGroups> layers = new List<CubeMotionGroups>();
		for (int i = 0; i < 6; i++)
		{
			if (((int)faceIntersection & (1 << i)) != 0)
			{
				layers.Add((CubeMotionGroups)(1 << i));
			}
		}

		int pieceTypeIndex = layers.Count - 1;

		int[] intersection;
		switch (pieceTypeIndex)
		{
			case 0:
				return (pieceTypeIndex, LayerToCubeIndex[layers[0]][0][0]);
			case 1:
				intersection = LayerToCubeIndex[layers[0]][1].Intersect(LayerToCubeIndex[layers[1]][1]).ToArray();
				if (intersection.Length != 1)
				{
					throw new Exception($"Given faces ({layers[0]} and {layers[1]}) do not intersect to one piece in: (int pieceType, int index) GetCubeReferenceFromFaceIntersection(CubeFaces faceIntersection)!\nGave intersection of size: {intersection.Length}");
				}
				else
				{
					return (pieceTypeIndex, intersection[0]);
				}
			case 2:
				intersection = LayerToCubeIndex[layers[0]][2].Intersect(LayerToCubeIndex[layers[1]][2].Intersect(LayerToCubeIndex[layers[2]][2])).ToArray();
				if (intersection.Length != 1)
				{
					throw new Exception($"Given faces ({layers[0]}, {layers[1]} and {layers[2]}) do not intersect to one piece in: (int pieceType, int index) GetCubeReferenceFromFaceIntersection(CubeFaces faceIntersection)!\nGave intersection of size: {intersection.Length}");
				}
				else
				{
					return (pieceTypeIndex, intersection[0]);
				}
			default:
				throw new Exception($"Incorrect number for intersecting faces ({pieceTypeIndex}) in: (int pieceType, int index) GetCubeReferenceFromFaceIntersection(CubeFaces faceIntersection)!");
		}
	}

	public void ExecuteMoveSmoothAsync<Smoother>(Move move, float speed = 2f, float postDelay = 0.1f, Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		foreach ((Coroutine, Piece3x3) pieceCoroutine in pieceTurnCoroutines)
		{
			StopCoroutine(pieceCoroutine.Item1);
			pieceCoroutine.Item2.CompleteTurnsInstantly();
		}
		pieceTurnCoroutines.Clear();

		int[][] selectedPieces = LayerToCubeIndex[move.motionGroup];
		Piece3x3[][] faceTemp = new Piece3x3[][] { new Piece3x3[1], new Piece3x3[4], new Piece3x3[4] };

        int turnIndexOffset;
		float turnAngle;
		switch (move.turn)
		{
			case CubeTurns.Clockwise:
				turnIndexOffset = 1;
				turnAngle = 90f;
				break;
			case CubeTurns.AntiClockwise:
				turnIndexOffset = 3;
				turnAngle = -90f;
				break;
			case CubeTurns.DoubleClockwise:
				turnIndexOffset = 2;
				turnAngle = 180f;
				break;
			case CubeTurns.DoubleAntiClockwise:
				turnIndexOffset = 2;
				turnAngle = -180f;
				break;
			case CubeTurns.None:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
			default:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
		}

		for (int i = 0; i < selectedPieces.Length; i++)
        {
            for (int j = 0; j < selectedPieces[i].Length; j++)
            {
				Piece3x3 currentPiece = cube[i][selectedPieces[i][j]];


				pieceTurnCoroutines.Add((StartCoroutine(currentPiece.RotateToSmooth<Smoother>(
					turnAngle,
					MotionGroupToAxis[move.motionGroup],
					speed,
					postDelay,
					callback)),
					currentPiece));
				faceTemp[i][(j + turnIndexOffset) % selectedPieces[i].Length] = currentPiece;
            }
        }

        for (int i = 0; i < selectedPieces.Length; i++)
        {
            for (int j = 0; j < selectedPieces[i].Length; j++)
            {
                cube[i][selectedPieces[i][j]] = faceTemp[i][j];
			}
		}
    }

	public IEnumerator ExecuteMoveSmoothCoroutine<Smoother>(Move move, float speed = 2f, Action callback = null) where Smoother : Piece3x3.TurnSmoother, new()
	{
		int turnIndexOffset;
		float turnAngle;
		switch (move.turn)
		{
			case CubeTurns.Clockwise:
				turnIndexOffset = 1;
				turnAngle = 90f;
				break;
			case CubeTurns.AntiClockwise:
				turnIndexOffset = 3;
				turnAngle = -90f;
				break;
			case CubeTurns.DoubleClockwise:
				turnIndexOffset = 2;
				turnAngle = 180f;
				break;
			case CubeTurns.DoubleAntiClockwise:
				turnIndexOffset = 2;
				turnAngle = -180f;
				break;
			case CubeTurns.None:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
			default:
				turnIndexOffset = 0;
				turnAngle = 0f;
				break;
		}

		if (move.moveType == Move.MoveType.LayerRotation)
		{
			foreach ((Coroutine, Piece3x3) pieceCoroutine in pieceTurnCoroutines)
			{
				StopCoroutine(pieceCoroutine.Item1);
				pieceCoroutine.Item2.CompleteTurnsInstantly();
			}
			pieceTurnCoroutines.Clear();

			int[][] selectedPieces = LayerToCubeIndex[move.motionGroup];
			Piece3x3[][] faceTemp = new Piece3x3[][] { new Piece3x3[1], new Piece3x3[4], new Piece3x3[4] };

			for (int i = 0; i < selectedPieces.Length; i++)
			{
				for (int j = 0; j < selectedPieces[i].Length; j++)
				{
					Piece3x3 currentPiece = cube[i][selectedPieces[i][j]];

					pieceTurnCoroutines.Add((StartCoroutine(currentPiece.RotateToSmooth<Smoother>(
						turnAngle,
						MotionGroupToAxis[move.motionGroup],
						speed)),
						currentPiece));
					faceTemp[i][(j + turnIndexOffset) % selectedPieces[i].Length] = currentPiece;
				}
			}

			for (int i = 0; i < selectedPieces.Length; i++)
			{
				for (int j = 0; j < selectedPieces[i].Length; j++)
				{
					cube[i][selectedPieces[i][j]] = faceTemp[i][j];
				}
			}

			foreach ((Coroutine, Piece3x3) pieceRoutine in pieceTurnCoroutines)
			{
				yield return pieceRoutine.Item1;
			}
		}
		else
		{
			if (cameraRotationCoroutine != null)
			{
				StopCoroutine(cameraRotationCoroutine);
			}

			Quaternion startRotation = cubeCamera.transform.rotation;
			Quaternion targetRotation = Quaternion.AngleAxis(-turnAngle, MotionGroupToAxis[move.motionGroup]);

			//Vector3 startPositionLocal = cubeCamera.transform.position - transform.position;
			//Vector3 targetPositionLocal = ;

			float elapsed = 0f;

			while (elapsed + speed * Time.deltaTime < 1f)
			{
				float t = 1f - Mathf.Cos(Mathf.PI * elapsed);

				cubeCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
			}

			cubeCamera.transform.rotation = targetRotation;
		}
		callback?.Invoke();
	}

	public void HighlightFaceAsync(CubeFaces face)
	{

	}

	public void HighlightLayerAsync(CubeMotionGroups layer, Color highlightColor, float highlightIntensity)
	{
		int[][] selectedPieces = LayerToCubeIndex[layer];
		HighlightPiecesAsync(selectedPieces, highlightColor, highlightIntensity);
	}

	public void HighlightPiecesAsync(int[][] selectedPieces, Color highlightColor, float highlightIntensity)
	{
		for (int pieceType = 0; pieceType < selectedPieces.Length; pieceType++)
		{
			for (int pieceIndex = 0; pieceIndex < selectedPieces[pieceType].Length; pieceIndex++)
			{
				Piece3x3 currentPiece = cube[pieceType][selectedPieces[pieceType][pieceIndex]];
				for (int sticker = 0; sticker < (int)currentPiece.PieceType; sticker++)
				{
					if (stickerHighlightCoroutines[pieceType][selectedPieces[pieceType][pieceIndex]][sticker] != null)
					{
						StopCoroutine(stickerHighlightCoroutines[pieceType][selectedPieces[pieceType][pieceIndex]][sticker]);
					}
					stickerHighlightCoroutines[pieceType][selectedPieces[pieceType][pieceIndex]][sticker] = StartCoroutine(currentPiece.HighlightSticker(sticker, highlightColor, highlightIntensity));
				}
			}
		}
	}

	public void HighlightPieceAsync(CubeFaces intersection, Color highlightColor, float highlightIntensity = 1f, float speed = 1f)
	{
		(int pieceType, int index) = GetCubeReferenceFromFaceIntersection(intersection);
		Piece3x3 currentPiece = cube[pieceType][index];

		for (int sticker = 0; sticker < (int)currentPiece.PieceType; sticker++)
		{
			if (stickerHighlightCoroutines[pieceType][index][sticker] != null)
			{
				StopCoroutine(stickerHighlightCoroutines[pieceType][index][sticker]);
			}
			stickerHighlightCoroutines[pieceType][index][sticker] = StartCoroutine(currentPiece.HighlightSticker(sticker, highlightColor, highlightIntensity, speed));
		}
	}

	public void ExecuteAlgorithmInstant(string algorithm)
	{
		string[] moveStrings = algorithm.Split(new char[] { ' ', '.', ',', '/', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string moveString in moveStrings)
		{
			ExecuteMoveInstant(StringToMove(moveString));
		}
	}

	//private void Update()
	//{
	//	//      if (Keyboard.current[Key.U].wasPressedThisFrame)
	//	//{
	//	//	SetFaceActive(CubeFaces.Up, Keyboard.current[Key.LeftShift].isPressed);
	//	//}
	//	//if (Keyboard.current[Key.D].isPressed)
	//	//{
	//	//	SetFaceActive(CubeFaces.Down, Keyboard.current[Key.LeftShift].isPressed);
	//	//}
	//	//if (Keyboard.current[Key.L].isPressed)
	//	//{
	//	//	SetFaceActive(CubeFaces.Left, Keyboard.current[Key.LeftShift].isPressed);
	//	//}
	//	//if (Keyboard.current[Key.R].isPressed)
	//	//{
	//	//	SetFaceActive(CubeFaces.Right, Keyboard.current[Key.LeftShift].isPressed);
	//	//}
	//	//if (Keyboard.current[Key.B].isPressed)
	//	//{
	//	//	SetFaceActive(CubeFaces.Back, Keyboard.current[Key.LeftShift].isPressed);
	//	//}
	//	//if (Keyboard.current[Key.F].isPressed)
	//	//{
	//	//	SetFaceActive(CubeFaces.Front, Keyboard.current[Key.LeftShift].isPressed);
	//	//}

	//	//CubeTurns turn = Keyboard.current[Key.LeftShift].isPressed ? CubeTurns.AntiClockwise : CubeTurns.Clockwise;

	//	//if (Keyboard.current[Key.LeftCtrl].isPressed)
	//	//{
	//	//	turn = turn == CubeTurns.Clockwise ? CubeTurns.DoubleClockwise : CubeTurns.DoubleAntiClockwise;
	//	//}
	//	//if (Keyboard.current[Key.W].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Up, turn));
	//	//}
	//	//if (Keyboard.current[Key.S].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Down, turn));
	//	//}
	//	//if (Keyboard.current[Key.A].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Left, turn));
	//	//}
	//	//if (Keyboard.current[Key.D].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Right, turn));
	//	//}
	//	//if (Keyboard.current[Key.Q].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Back, turn));
	//	//}
	//	//if (Keyboard.current[Key.E].wasPressedThisFrame)
	//	//{
	//	//	RotateFaceInstant(new Move(CubeLayers.Front, turn));
	//	//}
	//}
}