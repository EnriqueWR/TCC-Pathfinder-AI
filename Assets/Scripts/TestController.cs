using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{
	[SerializeField]
	private Text biasName;

	[SerializeField]
	private Text capName;

	[SerializeField]
	private Text lengthName;

	[SerializeField]
	private Text mapName;

	public float BiasFactor;
	public Tile BlockedTile;
	public GameObject DebugMark;
	public GameObject DebugPathMark;
	public Tile EndTile;
	public GameObject MainCamera;
	public Tile PathableTile;
	public Tile StartTile;
	public GameObject TileObj;
	private List<Obstacle> _obstacleList;
	private Vector2Int _targetPos;
	private List<Vector2Int> _unitiesToTest;
	private float BiasCap = 0.75f;
	private int biasType;
	private bool Control1;
	private bool Control2;
	private int debugId = -1;
	private float fraternidadeTeste;
	private int mapType;
	private float mediaTeste;

	private readonly Vector2Int[] neighbourAux =
	{
		new Vector2Int(1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(-1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(1, -1),
		new Vector2Int(0, -1),
		new Vector2Int(-1, -1),
	};

	private string nomeCenario;
	private string nomePathfinder;
	private List<Vector2Int> pathsDessaPassada;
	private int pathType;
	private readonly int propagacoes = 2;
	private int totalTestes;
	private string valorBias;

	// Start is called before the first frame update
	private void Start()
	{
		_unitiesToTest = new List<Vector2Int>();
		_obstacleList = new List<Obstacle>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (Control1)
		{
			Control1 = false;
			StartTest();

			// Control2 = true;
		}

		if (Control2 || Input.GetKeyDown(KeyCode.Space))
		{
			Control2 = false;
			Control1 = ConfigMixer();

			//LonelyTree();
			//ChineseWall();
			//BigRock();
		}

		//if (Input.GetKeyDown(KeyCode.Alpha1)) {
		//    ChineseWall();
		//    //OrderUnitsByDistanceTarget();
		//    BiasFactor = 1f;
		//}

		if (Input.GetKeyDown(KeyCode.T))
			StartTest();
	}

	public bool ConfigMixer()
	{
		// // LonelyTree();
		// // mapName.text = "Map: Lonely Tree";
		// // ChineseWall();
		// // mapName.text = "Map: Chinese Wall";
		// BigRock();
		// mapName.text = "Map: Boulder";

		// // SuperStress();
		// // mapName.text = "Map: Stress";

		// // BiasCap = 0.75f;
		// // BiasFactor = 0.9f;
		// // capName.text = "Cap: " + BiasCap;
		// // biasName.text = "Bias: " + BiasFactor;

		// BiasCap = 1f;
		// BiasFactor = 1f;
		// capName.text = "Cap: " + "Control";
		// biasName.text = "Bias: " + "Control";

		// OrderUnitsByDistanceTarget();
		// return true;
		print(mapType + "/" + biasType + "/" + pathType);

		switch (mapType)
		{
			case 0:
				LonelyTree();
				mapName.text = "Map: Lonely Tree";
				break;
			case 1:
				ChineseWall();
				mapName.text = "Map: Chinese Wall";
				break;
			case 2:
				BigRock();
				mapName.text = "Map: Boulder";
				break;
		}

		switch (pathType)
		{
			case 0:
				BiasCap = 0.75f;

				// OrderUnitsByDistanceTarget();
				break;
			case 1:
				BiasCap = 0.5f;

				// OrderUnitsByDistanceCenter();
				break;
			case 2:
				BiasCap = 0.01f;

				// OrderUnitsByDistanceMixed();
				break;
		}

		capName.text = "Cap: " + BiasCap;

		switch (biasType)
		{
			case 0:
				BiasFactor = 0.5f;
				break;
			case 1:
				BiasFactor = 0.6f;
				break;
			case 2:
				BiasFactor = 0.75f;
				break;
			case 3:
				BiasFactor = 0.9f;
				break;
			case 4:
				BiasFactor = 0.95f;
				break;
			case 5:
				BiasFactor = 0.99f;
				break;
		}

		biasName.text = "Bias: " + BiasFactor;

		// Agressivo 0.922263f
		// Medio 0.956f
		// Leve 0.958f
		OrderUnitsByDistanceTarget();
		return NextConfig();
	}

	public void StartTest()
	{
		if (_targetPos == null || _unitiesToTest == null || _unitiesToTest.Count == 0)
		{
			Debug.LogError("Selecione um teste!");
			return;
		}

		totalTestes++;
		string toWrite = totalTestes + ", " + nomeCenario + ", ";
		toWrite += nomePathfinder + ", ";
		toWrite += BiasFactor;
		float distanciasAcumuladas = 0;

		for (int i = 0; i < _unitiesToTest.Count; i++)
		{
			float resp = Pathfinder(i, _unitiesToTest[i], _targetPos);

			if (resp >= 0)
				distanciasAcumuladas += resp;
		}

		mediaTeste = distanciasAcumuladas / _unitiesToTest.Count;
		fraternidadeTeste = TileMapController.Instance.CalculateFraternity();

		//print("Media de paths = " + mediaTeste);
		//print("Fraternidade = " + fraternidadeTeste);
		lengthName.text = "Avg. Length: " + mediaTeste;
		WriteLog();
	}

	public void SetStartingPos(int x, int y)
	{
		SpawnTile(x, y, TilesEnum.Start);
		_unitiesToTest.Add(new Vector2Int(x, y));
	}

	public void SetTargetPos(int x, int y)
	{
		SpawnTile(x, y, TilesEnum.Ending);
		_targetPos = new Vector2Int(x, y);
	}

	public void SetObstacle(int x, int y, string id)
	{
		SpawnTile(x, y, TilesEnum.Blocked);
		Obstacle obstacle = _obstacleList.Find(elem => elem.id == id);

		if (obstacle == null)
		{
			obstacle = new Obstacle(id);
			_obstacleList.Add(obstacle);
		}

		obstacle.AddPosition(new Vector2Int(x, y));
	}

	public void LonelyTree()
	{
		nomeCenario = "Árvore solitária";
		ClearTiles();
		int maxX = 29;
		int maxY = 29;
		SetBorders(maxX, maxY);
		_unitiesToTest.Clear();
		SetTargetPos(14, 24);
		int startX = 10;
		int startY = 5;
		int qtdX = 9;
		int qtdY = 2;

		for (int i = 0; i < qtdY; i++)
		{
			for (int j = 0; j < qtdX; j++)
			{
				SetStartingPos(j + startX, startY + i);
			}
		}

		SetObstacle(14, 15, "Tree");

		//int maxX = 16;
		//int maxY = 16;
		//int midX = maxX / 2;
		//int midY = maxY / 2;
		//int unitsX = 9;
		//int unitsY = 2;

		//SetBorders(maxX, maxY);
		//_unitiesToTest.Clear();

		//SetObstacle(midX, midY, "Tree");

		//for (int i = 0; i < unitsY; i++) {
		//	for (int j = 0; j < unitsX; j++) {
		//		SetStartingPos(j + (midX / 2), i + 1);
		//	}
		//}

		//SetTargetPos(midX, midY + 6);

		for (int y = 1; y < maxY; y++)
		{
			for (int x = 1; x < maxX; x++)
			{
				SpawnTile(x, y, TilesEnum.Pathable);
			}
		}
	}

	public void ChineseWall()
	{
		nomeCenario = "Muralha da China";
		ClearTiles();
		int maxX = 29;
		int maxY = 29;
		SetBorders(maxX, maxY);
		_unitiesToTest.Clear();
		SetTargetPos(14, 27);
		int startX = 10;
		int startY = 2;
		int qtdX = 9;
		int qtdY = 2;

		for (int i = 0; i < qtdY; i++)
		{
			for (int j = 0; j < qtdX; j++)
			{
				SetStartingPos(j + startX, startY + i);
			}
		}

		int treeX = 1;
		int treeY = 17;

		for (int i = 0; i < treeY; i++)
		{
			for (int j = 0; j < treeX; j++)
			{
				SetObstacle(j + 14, i + 6, "Tree");
			}
		}

		//int maxX = 16;
		//int maxY = 32;
		//int midX = maxX / 2;
		//int midY = maxY / 2;
		//int unitsX = 9;
		//int unitsY = 2;

		//SetBorders(maxX, maxY);
		//_unitiesToTest.Clear();

		//for (int i = 0; i < 19; i++) {
		//	SetObstacle(midX, i + (midY / 2) - 1, "Tree");
		//}

		//for (int i = 0; i < unitsY; i++) {
		//	for (int j = 0; j < unitsX; j++) {
		//		SetStartingPos(j + (midX / 2), i + 1);
		//	}
		//}

		//SetTargetPos(midX, maxY - 2);

		for (int y = 1; y < maxY; y++)
		{
			for (int x = 1; x < maxX; x++)
			{
				SpawnTile(x, y, TilesEnum.Pathable);
			}
		}
	}

	public void BigRock()
	{
		nomeCenario = "Pedregulho";
		ClearTiles();
		int maxX = 29;
		int maxY = 29;
		int midX = maxX / 2;
		int midY = maxY / 2;
		SetBorders(maxX, maxY);
		_unitiesToTest.Clear();
		SetTargetPos(14, 24);
		int startX = 10;
		int startY = 5;
		int qtdX = 9;
		int qtdY = 2;

		for (int i = 0; i < qtdY; i++)
		{
			for (int j = 0; j < qtdX; j++)
			{
				SetStartingPos(j + startX, startY + i);
			}
		}

		int treeX = 11;
		int treeY = 10;

		for (int i = 0; i < treeY; i++)
		{
			for (int j = 0; j < treeX; j++)
			{
				SetObstacle(j + 9, i + 10, "Tree");
			}
		}

		//int treeX = 10;
		//int treeY = 10;

		//for (int i = 0; i < treeY; i++) {
		//	for (int j = 0; j < treeX; j++) {
		//		SetObstacle(j + midX - (treeX / 2), i + midY - (treeX / 2), "Tree");
		//	}
		//}

		//int unitsX = 8;
		//int unitsY = 2;

		//for (int i = 0; i < unitsY; i++) {
		//	for (int j = 0; j < unitsX; j++) {
		//		SetStartingPos(j + midX - (unitsX / 2), i + 1);
		//	}
		//}

		//SetTargetPos(midX, maxY - 2);

		for (int y = 1; y < maxY; y++)
		{
			for (int x = 1; x < maxX; x++)
			{
				SpawnTile(x, y, TilesEnum.Pathable);
			}
		}
	}

	public void SuperStress()
	{
		nomeCenario = "Stress";
		ClearTiles();
		int maxX = 29;
		int maxY = 49;
		int midX = maxX / 2;
		int midY = maxY / 2;
		SetBorders(maxX, maxY);
		_unitiesToTest.Clear();
		SetTargetPos(midX, maxY - 2);
		int qtdX = 9;
		int qtdY = 2;
		int startX = midX - (qtdX / 2);
		int startY = qtdY;

		for (int i = 0; i < qtdY; i++)
		{
			for (int j = 0; j < qtdX; j++)
			{
				SetStartingPos(j + startX, startY + i);
			}
		}

		int treeX = 1;
		int treeY = 1;
		int treeStartingX = midX - (treeX / 2);
		int treeStartingY = midY;

		for (int i = 0; i < treeY; i++)
		{
			for (int j = 0; j < treeX; j++)
			{
				SetObstacle(j + treeStartingX, i + treeStartingY, "Tree");
				print(j + treeStartingX + "/" + (treeStartingY + i));
			}
		}

		for (int y = 1; y < maxY; y++)
		{
			for (int x = 1; x < maxX; x++)
			{
				SpawnTile(x, y, TilesEnum.Pathable);
			}
		}
	}

	public void SetBorders(int maxX, int maxY)
	{
		MainCamera.transform.position = new Vector3((float) maxX / 2, (float) maxY / 2, -10f);

		for (int y = 0; y <= maxY; y++)
		{
			for (int x = 0; x <= maxX; x++)
			{
				if (x == 0 || x == maxX || y == 0 || y == maxY)
					SpawnTile(x, y, TilesEnum.Blocked);
			}
		}
	}

	public void SpawnTile(int x, int y, TilesEnum tile)
	{
		Vector3 pos = new Vector3(x, y);

		switch (tile)
		{
			case TilesEnum.Start:
				Instantiate(StartTile, pos, Quaternion.identity, TileObj.transform);
				break;
			case TilesEnum.Ending:
				Instantiate(EndTile, pos, Quaternion.identity, TileObj.transform);
				break;
			case TilesEnum.Pathable:
				Instantiate(PathableTile, pos, Quaternion.identity, TileObj.transform);
				break;
			case TilesEnum.Blocked:
				Instantiate(BlockedTile, pos, Quaternion.identity, TileObj.transform);
				break;
		}
	}

	public void ClearTiles()
	{
		_obstacleList.Clear();
		_unitiesToTest.Clear();
		TileMapController.Instance.Tiles.Clear();

		foreach (Transform child in TileObj.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void OrderUnitsByDistanceTarget()
	{
		nomePathfinder = "Alvo";

		_unitiesToTest.Sort(
			(a, b) =>
			{
				float distanciaA = Vector2Int.Distance(a, _targetPos);
				float distanciaB = Vector2Int.Distance(b, _targetPos);
				return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
			}
		);
	}

	public void OrderUnitsByDistanceCenter()
	{
		nomePathfinder = "Centro";
		Vector2Int centerPoint = new Vector2Int(0, 0);

		foreach (Vector2Int point in _unitiesToTest)
		{
			centerPoint += point;
		}

		centerPoint /= _unitiesToTest.Count;

		_unitiesToTest.Sort(
			(a, b) =>
			{
				float distanciaA = Vector2Int.Distance(a, centerPoint);
				float distanciaB = Vector2Int.Distance(b, centerPoint);
				return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
			}
		);
	}

	public void OrderUnitsByDistanceMixed()
	{
		nomePathfinder = "Misto";
		Vector2Int centerPoint = new Vector2Int(0, 0);

		foreach (Vector2Int point in _unitiesToTest)
		{
			centerPoint += point;
		}

		centerPoint /= _unitiesToTest.Count;

		_unitiesToTest.Sort(
			(a, b) =>
			{
				float distanciaA = (Vector2Int.Distance(a, centerPoint) + Vector2Int.Distance(a, _targetPos)) / 2;
				float distanciaB = (Vector2Int.Distance(b, centerPoint) + Vector2Int.Distance(b, _targetPos)) / 2;
				return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
			}
		);
	}

	public void SpreadMarker(Vector2Int tilePos, Vector2Int originalTilePos, int propagacaoAtual)
	{
		Tile tile = TileMapController.Instance.GetTile(tilePos);

		// if (tile.pathNessaPassada && tilePos != originalTilePos) return;
		// if (tile.contadoNessaPassada) return;
		if (!tile.Pathable)
			return;

		if (!pathsDessaPassada.Contains(tilePos) && !tile.pathNessaPassada)
		{
			float distance = tilePos == originalTilePos ? 0 : 1f; // Vector2Int.Distance(tilePos, originalTilePos);
			float factor = 0.02f * distance;
			pathsDessaPassada.Add(tilePos);

			// tile.contadoNessaPassada = true;
			tile.PheromoneBias *= Mathf.Clamp(BiasFactor + factor, 0.9f, 1f);

			if (tile.PheromoneBias < BiasCap)
				tile.PheromoneBias = BiasCap;
		}
		else if (!pathsDessaPassada.Contains(tilePos) && tile.pathNessaPassada)
		{
			pathsDessaPassada.Add(tilePos);

			// tile.contadoNessaPassada = true;
			tile.PheromoneBias *= BiasFactor;

			if (tile.PheromoneBias < BiasCap)
				tile.PheromoneBias = BiasCap;
		}

		if (propagacaoAtual < propagacoes)
			foreach (Vector2Int neighbour in neighbourAux)
			{
				SpreadMarker(tile.pos + neighbour, originalTilePos, propagacaoAtual + 1);
			}
	}

	public float Pathfinder(int id, Vector2Int startingPos, Vector2Int endingPos)
	{
		Tile firstTile = TileMapController.Instance.GetTile(startingPos);
		AStarNode firstNode = new AStarNode(startingPos, null, endingPos, firstTile.PheromoneBias);
		List<AStarNode> toCheckList = new List<AStarNode>();
		List<AStarNode> checkedList = new List<AStarNode>();

		//Dictionary<Vector2Int, float> toCheckDict = new Dictionary<Vector2Int, float>();
		//Dictionary<Vector2Int, float> checkedDict = new Dictionary<Vector2Int, float>();
		int wtf = 0;

		//toCheckDict.Add(firstNode.pos, firstNode.totalDistance);
		toCheckList.Add(firstNode);

		while (toCheckList.Count != 0)
		{
			wtf++;

			//if (wtf > 100) {
			//    return 0f;
			//}
			AStarNode nodeAtual = toCheckList[0];
			toCheckList.RemoveAt(0);
			AStarNode auxNode;

			foreach (Vector2Int neighbour in neighbourAux)
			{
				Vector2Int posAux = nodeAtual.pos + neighbour;

				if (posAux == endingPos)
				{
					List<Vector2Int> caminhoFinal = new List<Vector2Int>();
					caminhoFinal.Add(posAux);
					caminhoFinal.Add(nodeAtual.pos);
					auxNode = nodeAtual.parent;

					while (auxNode != null)
					{
						caminhoFinal.Add(auxNode.pos);
						auxNode = auxNode.parent;
					}

					caminhoFinal.Reverse();

					foreach (Vector2Int elem in caminhoFinal)
					{
						Tile auxTile = TileMapController.Instance.GetTile(elem);
						auxTile.pathNessaPassada = true;
					}

					pathsDessaPassada = new List<Vector2Int>();

					foreach (Vector2Int elem in caminhoFinal)
					{
						// Tile auxTile = TileMapController.Instance.GetTile(elem);
						// auxTile.PheromoneBias *= BiasFactor;
						// if (auxTile.PheromoneBias < BiasCap) {
						// 	auxTile.PheromoneBias = BiasCap;
						// }
						SpreadMarker(elem, elem, 1);
						TileMapController.Instance.GetTile(elem).RegistraVisita(id);
					}

					foreach (Vector2Int elem in caminhoFinal)
					{
						Tile auxTile = TileMapController.Instance.GetTile(elem);
						auxTile.contadoNessaPassada = false;
						auxTile.pathNessaPassada = false;
					}

					GameObject daddy = MarkPath(caminhoFinal[0], null, id);
					float acumulado = 0;

					for (int i = 1; i < caminhoFinal.Count; i++)
					{
						acumulado += Mathf.Abs(Vector2Int.Distance(caminhoFinal[i - 1], caminhoFinal[i]));
						daddy = MarkPath(caminhoFinal[i], daddy, id);

						Debug.DrawLine(
							new Vector3(caminhoFinal[i - 1].x, caminhoFinal[i - 1].y, -1f),
							new Vector3(caminhoFinal[i].x, caminhoFinal[i].y, -1f),
							Color.blue,
							1000
						);
					}

					return acumulado;
				}

				Tile tileTest = TileMapController.Instance.GetTile(posAux);

				TileType tileType = tileTest == null ? TileType.Invalid :
					tileTest.Pathable ? TileType.Pathable : TileType.Blocked;

				if (tileType == TileType.Pathable)
				{
					auxNode = new AStarNode(posAux, nodeAtual, endingPos, tileTest.PheromoneBias);

					AStarNode auxAuxNode1 = toCheckList.Find(
						x =>
						{
							if (x.pos == auxNode.pos)
								return x.totalDistance <= auxNode.totalDistance;

							return false;
						}
					);

					AStarNode auxAuxNode2 = checkedList.Find(
						x =>
						{
							if (x.pos == auxNode.pos)
								return x.totalDistance <= auxNode.totalDistance;

							return false;
						}
					);

					if (auxAuxNode1 == null && auxAuxNode2 == null)
					{
						int i;

						for (i = 0; i < toCheckList.Count; i++)
						{
							if (auxNode.totalDistance < toCheckList[i].totalDistance)
								break;
						}

						toCheckList.Insert(i, auxNode);

						Instantiate(
							DebugMark,
							new Vector3(auxNode.pos.x, auxNode.pos.y, -1f),
							Quaternion.identity,
							TileObj.transform
						);
					}
				}
				else if (tileType == TileType.Blocked)
				{
					// tratamento da pedra
					/**
                     * Expandir e encontrar todos os bloqueios adjecentes, salvar posicao, largura e altura da pedra
                     */
					//FindObstacleByTile(posAux);
				}
			}

			int nodeAtualIndex;

			for (nodeAtualIndex = 0; nodeAtualIndex < checkedList.Count; nodeAtualIndex++)
			{
				if (nodeAtual.totalDistance < checkedList[nodeAtualIndex].totalDistance)
					break;
			}

			checkedList.Insert(nodeAtualIndex, nodeAtual);
		}

		return -1;
	}

	public GameObject MarkPath(Vector2Int pos, GameObject parent, int id)
	{
		Vector3 posAux = new Vector3(pos.x, pos.y, -2);
		GameObject pPoint = Instantiate(DebugPathMark, posAux, Quaternion.identity, TileObj.transform);
		List<Color32> colors = new List<Color32>();
		colors.Add(new Color32(0x80, 0x00, 0x00, 0xFF));
		colors.Add(new Color32(0x9A, 0x63, 0x24, 0xFF));
		colors.Add(new Color32(0x80, 0x80, 0x00, 0xFF));
		colors.Add(new Color32(0x46, 0x99, 0x90, 0xFF));
		colors.Add(new Color32(0x00, 0x00, 0x00, 0xFF));
		colors.Add(new Color32(0xe6, 0x19, 0x4B, 0xFF));
		colors.Add(new Color32(0xf5, 0x82, 0x31, 0xFF));
		colors.Add(new Color32(0xff, 0xe1, 0x19, 0xFF));
		colors.Add(new Color32(0xbf, 0xef, 0x45, 0xFF));
		colors.Add(new Color32(0x3c, 0xb4, 0x4b, 0xFF));
		colors.Add(new Color32(0x42, 0xd4, 0xf4, 0xFF));
		colors.Add(new Color32(0x43, 0x63, 0xd8, 0xFF));
		colors.Add(new Color32(0x91, 0x1e, 0xb4, 0xFF));
		colors.Add(new Color32(0xf0, 0x32, 0xe6, 0xFF));
		colors.Add(new Color32(0xfa, 0xbe, 0xd4, 0xFF));
		colors.Add(new Color32(0xff, 0xd8, 0xb1, 0xFF));
		colors.Add(new Color32(0x00, 0x00, 0x75, 0xFF));
		colors.Add(new Color32(0xff, 0xff, 0xff, 0xFF));

		if (parent != null && id >= 0 && id < 99 && id != -1)
		{
			PathDrawer pDrawer = pPoint.GetComponent<PathDrawer>();
			pDrawer.SetParent(id, parent, colors[id]);
		}

		return pPoint;
	}

	public Obstacle FindObstacleByTile(Vector2Int pos)
	{
		return _obstacleList.Find(
			elem =>
			{
				Vector2Int aux = elem.obstaclePositions.Find(position => position == pos);

				if (aux != null)
					return true;

				return false;
			}
		);
	}

	public void PathfinderColapso()
	{
		if (_targetPos == null || _unitiesToTest == null || _unitiesToTest.Count == 0)
		{
			Debug.LogError("Selecione um teste!");
			return;
		}

		List<ColapseNode> checkedList = new List<ColapseNode>();
		UnitQueue unitQueue = new UnitQueue(_unitiesToTest, _targetPos);
		ColapseNode nodeAtual = unitQueue.next();

		while (nodeAtual != null)
		{
			//foreach (var neighbour in neighbourAux) {
			//    Vector2Int posAux = nodeAtual.pos + neighbour;
			//    if (posAux == _targetPos) {
			//        print("Encontrado!");
			//        List<Vector2Int> caminhoFinal = new List<Vector2Int>();
			//        caminhoFinal.Add(posAux);
			//        caminhoFinal.Add(nodeAtual.pos);
			//        auxNode = nodeAtual.parent;
			//        while (auxNode != null) {
			//            caminhoFinal.Add(auxNode.pos);
			//            auxNode = auxNode.parent;
			//        }
			//        caminhoFinal.Reverse();
			//        foreach (var elem in caminhoFinal) {
			//            print(elem);
			//            //Instantiate(DebugPathMark, new Vector3(elem.x, elem.y, -2f), Quaternion.identity, transform);
			//        }

			//        for (int i = 1; i < caminhoFinal.Count; i++) {
			//            Debug.DrawLine(
			//                new Vector3(caminhoFinal[i - 1].x, caminhoFinal[i - 1].y, 0f),
			//                new Vector3(caminhoFinal[i].x, caminhoFinal[i].y, 0f),
			//                Color.blue,
			//                30);
			//        }
			//        return;
			//    }

			//    TileType tileType = TileMapController.Instance.ValidTile(posAux);
			//    if (tileType == TileType.Pathable) {
			//        auxNode = new AStarNode(posAux, nodeAtual, _targetPos);

			//        var auxAuxNode1 = toCheckList.Find(x => {
			//            if (x.pos == auxNode.pos) {
			//                return x.totalDistance < auxNode.totalDistance;
			//            }
			//            return false;
			//        });
			//        var auxAuxNode2 = checkedList.Find(x => {
			//            if (x.pos == auxNode.pos) {
			//                return x.totalDistance < auxNode.totalDistance;
			//            }
			//            return false;
			//        });
			//        if (auxAuxNode1 == null && auxAuxNode2 == null) {
			//            unitQueue.add(auxNode);
			//            Instantiate(DebugMark, new Vector3(auxNode.pos.x, auxNode.pos.y, -1f), Quaternion.identity, transform);
			//        }
			//    } else if (tileType == TileType.Blocked) {
			//        // tratamento da pedra
			//        /**
			//         * Expandir e encontrar todos os bloqueios adjecentes, salvar posicao, largura e altura da pedra
			//         */
			//        FindObstacleByTile(posAux);
			//    }
			//}

			//checkedList.Add(nodeAtual);
			nodeAtual = unitQueue.next();
		}

		// Botar uma lista pra cada unidade e só botar a melhor opcao de cada uma na fila pro proximo ciclo?
		// Isso + o colapso pode fazer bem menos choques de checks
	}

	public void WriteLog()
	{
		string date = DateTime.Now.ToString("yyyy-MM-dd");
		string path = "Assets/Logs/Resultados-" + date + "-v3.txt";

		// string separator = ", ";
		string separator = "; ";
		valorBias = "" + BiasFactor;
		string toWrite = totalTestes + separator;
		toWrite += nomeCenario + separator;
		toWrite += nomePathfinder + separator;
		toWrite += trataVirgula(BiasFactor) + separator;
		toWrite += trataVirgula(BiasCap) + separator;
		toWrite += trataVirgula(mediaTeste) + separator;
		toWrite += trataVirgula(fraternidadeTeste);

		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(toWrite);
		writer.Close();

		//print(toWrite);
	}

	public string trataVirgula(float number)
	{
		string aux = "" + number;

		// return aux.Replace(",", ".");
		return aux;
	}

	private bool NextConfig()
	{
		// if (mapType == 3)
		// {
		// 	print("DONE!");
		// 	return false;
		// }

		// biasType++;
		// if (biasType == 5)
		// {
		// 	biasType = 0;
		// 	pathType++;
		// 	if (pathType == 5)
		// 	{
		// 		pathType = 0;
		// 		mapType++;
		// 	}
		// }

		// ########################### //

		if (pathType == 3)
		{
			print("DONE!");
			return false;
		}

		mapType++;

		if (mapType == 3)
		{
			mapType = 0;
			biasType++;

			if (biasType == 6)
			{
				biasType = 0;
				pathType++;
			}
		}

		return true;
	}
}

public class UnitQueue
{
	private readonly List<List<ColapseNode>> queue;
	private int iterator;

	public UnitQueue(List<Vector2Int> unities, Vector2Int target)
	{
		if (unities.Count == 0 || target == null)
		{
			Debug.LogError("unidades ou posicao alvo invalidas");
			return;
		}

		queue = new List<List<ColapseNode>>();

		foreach (Vector2Int unit in unities)
		{
			List<ColapseNode> listAux = new List<ColapseNode>();
			ColapseNode nodeAux = new ColapseNode(unit, null, target);
			listAux.Add(nodeAux);
			queue.Add(listAux);
		}

		iterator = queue.Count;
	}

	public void print()
	{
		foreach (List<ColapseNode> list in queue)
		{
			foreach (ColapseNode node in list)
			{
				Debug.Log(node.pos + " - " + node.totalDistance);
			}
		}
	}

	public ColapseNode next()
	{
		if (iterator >= (queue.Count - 1))
			iterator = 0;
		else
			iterator++;

		if (queue.Count == 0)
		{
			Debug.LogError("Queue vazia!");
			return null;
		}

		if (queue[iterator].Count > 0)
		{
			ColapseNode aux = queue[iterator][0];
			queue[iterator].RemoveAt(0);
			return aux;
		}

		queue.RemoveAt(iterator);
		return next();
	}

	public void add(ColapseNode node)
	{
		int i;

		for (i = 0; i < queue.Count; i++)
		{
			if (queue[iterator][i].totalDistance > node.totalDistance)
				break;
		}

		queue[iterator].Insert(i, node);
	}

	public void toCheck(ColapseNode node)
	{
		foreach (List<ColapseNode> list in queue)
		{
			list.Find(
				elem =>
				{
					return elem.pos == node.pos;
				}
			);
		}
	}
}

public class ColapseUnit { }

public class ColapseNode
{
	public Vector2Int pos;
	public float heuristicDistance;
	public float realDistance;
	public float totalDistance;
	public ColapseNode parent;
	public int ownerUnit;

	//public List<AStarNode> parent;

	public ColapseNode() { }

	public ColapseNode(Vector2Int pos, ColapseNode parent, Vector2Int endingPos)
	{
		this.pos = pos;

		//this.parent = new List<AStarNode>();
		this.parent = parent;
		heuristicDistance = Mathf.Sqrt(Mathf.Pow(pos.x - endingPos.x, 2) + Mathf.Pow(pos.y - endingPos.y, 2));

		if (parent != null)
		{
			//this.parent.Add(parent);
			realDistance = parent.realDistance
							+ Mathf.Sqrt(Mathf.Pow(pos.x - parent.pos.x, 2) + Mathf.Pow(pos.y - parent.pos.y, 2));

			totalDistance = realDistance + heuristicDistance;
		}
		else
		{
			totalDistance = heuristicDistance;
		}
	}
}

public class AStarNode
{
	public Vector2Int pos;
	public float heuristicDistance;
	public float realDistance;
	public float totalDistance;
	public AStarNode parent;
	public int ownerUnit;

	//public List<AStarNode> parent;

	//public AStarNode() { }

	//public AStarNode(Vector2Int pos, AStarNode parent, Vector2Int endingPos) {
	//    this.pos = pos;
	//    //this.parent = new List<AStarNode>();
	//    this.parent = parent;
	//    heuristicDistance = Mathf.Sqrt(Mathf.Pow(pos.x - endingPos.x, 2) + Mathf.Pow(pos.y - endingPos.y, 2));
	//    realDistance = 0;
	//    if (parent != null) {
	//        realDistance = parent.realDistance + Mathf.Sqrt(Mathf.Pow(pos.x - parent.pos.x, 2) + Mathf.Pow(pos.y - parent.pos.y, 2));
	//    }
	//    totalDistance = realDistance + heuristicDistance;
	//}

	public AStarNode(Vector2Int pos, AStarNode parent, Vector2Int endingPos, float pheromoneWeight)
	{
		this.pos = pos;

		//this.parent = new List<AStarNode>();
		this.parent = parent;
		heuristicDistance = Mathf.Sqrt(Mathf.Pow(pos.x - endingPos.x, 2) + Mathf.Pow(pos.y - endingPos.y, 2));
		realDistance = 0;

		if (parent != null)
			realDistance = parent.realDistance
							+ Mathf.Sqrt(Mathf.Pow(pos.x - parent.pos.x, 2) + Mathf.Pow(pos.y - parent.pos.y, 2));

		//totalDistance = realDistance + (heuristicDistance * pheromoneWeight);
		totalDistance = (realDistance + heuristicDistance) * pheromoneWeight;
	}
}

public class Obstacle
{
	public string id;
	public List<Vector2Int> obstaclePositions;
	public Vector2Int center;
	public int xSize;
	public int ySize;

	public Obstacle(string id)
	{
		this.id = id;
		obstaclePositions = new List<Vector2Int>();
	}

	public void AddPosition(Vector2Int pos)
	{
		obstaclePositions.Add(pos);

		int minX = obstaclePositions[0].x,
			maxX = obstaclePositions[0].x,
			minY = obstaclePositions[0].y,
			maxY = obstaclePositions[0].y;

		float totalX = 0, totalY = 0;

		for (int i = 1; i < obstaclePositions.Count; i++)
		{
			Vector2Int obsPos = obstaclePositions[i];

			if (obsPos.x > maxX)
				maxX = obsPos.x;
			else if (obsPos.x < minX)
				minX = obsPos.x;

			if (obsPos.y > maxY)
				maxY = obsPos.y;
			else if (obsPos.y < minY)
				minY = obsPos.y;

			totalX += obsPos.x;
			totalY += obsPos.y;
		}

		xSize = Mathf.Abs(minX - maxX);
		ySize = Mathf.Abs(minY - maxY);

		center = new Vector2Int(
			Mathf.RoundToInt(totalX / obstaclePositions.Count),
			Mathf.RoundToInt(totalY / obstaclePositions.Count)
		);
	}
}

public class PathCorrectionAuxiliar
{
	public Obstacle obstacle;
	public Vector2Int unit;
	public List<Vector2Int> unitPath;
}

public enum TilesEnum
{
	Start,
	Ending,
	Pathable,
	Blocked,
}