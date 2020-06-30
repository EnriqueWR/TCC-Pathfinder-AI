using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestController : MonoBehaviour {
    public float BiasFactor;
    public float BiasCap;

    public GameObject TileObj;
    public GameObject MainCamera;
    public Tile PathableTile;
    public Tile BlockedTile;
    public Tile StartTile;
    public Tile EndTile;
    public GameObject DebugMark;
    public GameObject DebugPathMark;
    private Vector2Int _targetPos;
    private List<Vector2Int> _unitiesToTest;
    private List<Obstacle> _obstacleList;

    private Vector2Int[] neighbourAux = {
        new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1)
    };

    private float mediaTeste;
    private float fraternidadeTeste;

    private string nomeCenario;
    private string nomePathfinder;
    private string valorBias;


    int totalTestes = 0;
    private int mapType = 0;
    private int pathType = 0;
    private int biasType = 0;

    private bool Control1 = false;
    private bool Control2 = false;

    // Start is called before the first frame update
    void Start() {
        _unitiesToTest = new List<Vector2Int>();
        _obstacleList = new List<Obstacle>();
    }

    // Update is called once per frame
    void Update() {
        if (Control1) {
            Control1 = false;
            StartTest();
            Control2 = true;
        }

        if (Control2 || Input.GetKeyDown(KeyCode.Space)) {
            Control2 = false;
            Control1 = ConfigMixer();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //    ChineseWall();
        //    //OrderUnitsByDistanceTarget();
        //    BiasFactor = 1f;
        //}

        //if (Input.GetKeyDown(KeyCode.T)) {
        //    StartTest();
        //}
    }

    public bool ConfigMixer() {
        switch (mapType) {
            case 0:
                LonelyTree();
                break;
            case 1:
                ChineseWall();
                break;
            case 2:
                BigRock();
                break;
        }

        switch (pathType) {
            case 0:
                OrderUnitsByDistanceTarget();
                break;
            case 1:
                OrderUnitsByDistanceCenter();
                break;
            case 2:
                OrderUnitsByDistanceMixed();
                break;
        }

        switch (biasType) {
            case 0:
                BiasFactor = 1;
                break;
            case 1:
                BiasFactor = 0.99f;
                break;
            case 2:
                BiasFactor = 0.95f;
                break;
            case 3:
                BiasFactor = 0.9f;
                break;
            case 4:
                BiasFactor = 0.75f;
                break;
        }

        return NextConfig();
    }

    bool NextConfig() {
        if (mapType == 3) {
            print("DONE!");
            return false;
        }

        biasType++;
        if (biasType == 5) {
            biasType = 0;
            pathType++;
            if (pathType == 3) {
                pathType = 0;
                mapType++;
            }
        }
        
        return true;
    }


    public void StartTest() {
        if (_targetPos == null || _unitiesToTest == null || _unitiesToTest.Count == 0) {
            Debug.LogError("Selecione um teste!");
            return;
        }

        totalTestes++;
        string toWrite = totalTestes + ", " + nomeCenario + ", ";
        toWrite += nomePathfinder + ", ";
        toWrite += BiasFactor;

        float distanciasAcumuladas = 0;
        for (int i = 0; i < _unitiesToTest.Count; i++) {
            var resp = Pathfinder(i, _unitiesToTest[i], _targetPos);
            if (resp >= 0) {
                distanciasAcumuladas += resp;
            }
        }

        mediaTeste = (distanciasAcumuladas / _unitiesToTest.Count);
        fraternidadeTeste = TileMapController.Instance.CalculateFraternity();

        //print("Media de paths = " + mediaTeste);
        //print("Fraternidade = " + fraternidadeTeste);
        
        WriteLog();
    }

    public void SetStartingPos(int x, int y) {
        SpawnTile(x, y, TilesEnum.Start);
        _unitiesToTest.Add(new Vector2Int(x, y));
    }

    public void SetTargetPos(int x, int y) {
        SpawnTile(x, y, TilesEnum.Ending);
        _targetPos = new Vector2Int(x, y);
    }

    public void SetObstacle(int x, int y, string id) {
        SpawnTile(x, y, TilesEnum.Blocked);

        var obstacle = _obstacleList.Find(elem => elem.id == id);
        if (obstacle == null) {
            obstacle = new Obstacle(id);
            _obstacleList.Add(obstacle);
        }
        obstacle.AddPosition(new Vector2Int(x, y));
    }

    public void LonelyTree() {
        nomeCenario = "Árvore solitária";
        ClearTiles();

        int maxX = 16;
        int maxY = 16;
        int midX = maxX / 2;
        int midY = maxY / 2;
        int unitsX = 9;
        int unitsY = 2;

        SetBorders(maxX, maxY);
        _unitiesToTest.Clear();

        SetObstacle(midX, midY, "Tree");

        for (int i = 0; i < unitsY; i++) {
            for (int j = 0; j < unitsX; j++) {
                SetStartingPos(j + (midX / 2), i + 1);
            }
        }

        SetTargetPos(midX, midY + 6);

        for (int y = 1; y < maxY; y++) {
            for (int x = 1; x < maxX; x++) {
                SpawnTile(x, y, TilesEnum.Pathable);
            }
        }
    }

    public void ChineseWall() {
        nomeCenario = "Muralha da China";
        ClearTiles();

        int maxX = 16;
        int maxY = 32;
        int midX = maxX / 2;
        int midY = maxY / 2;
        int unitsX = 9;
        int unitsY = 2;

        SetBorders(maxX, maxY);
        _unitiesToTest.Clear();

        for (int i = 0; i < 19; i++) {
            SetObstacle(midX, i + (midY / 2) - 1, "Tree");
        }

        for (int i = 0; i < unitsY; i++) {
            for (int j = 0; j < unitsX; j++) {
                SetStartingPos(j + (midX / 2), i + 1);
            }
        }

        SetTargetPos(midX, maxY - 2);

        for (int y = 1; y < maxY; y++) {
            for (int x = 1; x < maxX; x++) {
                SpawnTile(x, y, TilesEnum.Pathable);
            }
        }
    }

    public void BigRock() {
        nomeCenario = "Pedregulho";
        ClearTiles();

        int maxX = 24;
        int maxY = 24;
        int midX = maxX / 2;
        int midY = maxY / 2;
        

        SetBorders(maxX, maxY);
        _unitiesToTest.Clear();

        int treeX = 11;
        int treeY = 11;

        for (int i = 0; i < treeY; i++) {
            for (int j = 0; j < treeX; j++) {
                SetObstacle(j + midX - (treeX / 2), i + midY - (treeX / 2), "Tree");
            }
        }

        int unitsX = 9;
        int unitsY = 2;

        for (int i = 0; i < unitsY; i++) {
            for (int j = 0; j < unitsX; j++) {
                SetStartingPos(j + midX - (unitsX / 2), i + 1);
            }
        }

        SetTargetPos(midX, maxY - 2);

        for (int y = 1; y < maxY; y++) {
            for (int x = 1; x < maxX; x++) {
                SpawnTile(x, y, TilesEnum.Pathable);
            }
        }
    }

    public void SetBorders(int maxX, int maxY) {
        MainCamera.transform.position = new Vector3(((float) maxX) / 2, ((float) maxY) / 2, -10f);

        for (int y = 0; y <= maxY; y++) {
            for (int x = 0; x <= maxX; x++) {
                if (x == 0 || x == maxX || y == 0 || y == maxY) {
                    SpawnTile(x, y, TilesEnum.Blocked);
                }
            }
        }
    }

    public void SpawnTile(int x, int y, TilesEnum tile) {
        Vector3 pos = new Vector3(x, y);

        switch (tile) {
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

    public void ClearTiles() {
        _obstacleList.Clear();
        _unitiesToTest.Clear();
        TileMapController.Instance.Tiles.Clear();
        foreach (Transform child in TileObj.transform) {
            Destroy(child.gameObject);
        }
    }

    public void OrderUnitsByDistanceTarget() {
        nomePathfinder = "Alvo";
        _unitiesToTest.Sort(
            (a, b) => {
                var distanciaA = Vector2Int.Distance(a, _targetPos);
                var distanciaB = Vector2Int.Distance(b, _targetPos);

                return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
            });
    }

    public void OrderUnitsByDistanceCenter() {
        nomePathfinder = "Centro";
        Vector2Int centerPoint = new Vector2Int(0, 0);
        foreach (var point in _unitiesToTest) {
            centerPoint += point;
        }
        centerPoint /= _unitiesToTest.Count;

        _unitiesToTest.Sort(
            (a, b) => {
                var distanciaA = Vector2Int.Distance(a, centerPoint);
                var distanciaB = Vector2Int.Distance(b, centerPoint);

                return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
            });
    }

    public void OrderUnitsByDistanceMixed() {
        nomePathfinder = "Misto";
        Vector2Int centerPoint = new Vector2Int(0, 0);
        foreach (var point in _unitiesToTest) {
            centerPoint += point;
        }
        centerPoint /= _unitiesToTest.Count;

        _unitiesToTest.Sort(
            (a, b) => {
                var distanciaA = (Vector2Int.Distance(a, centerPoint) + Vector2Int.Distance(a, _targetPos)) / 2;
                var distanciaB = (Vector2Int.Distance(b, centerPoint) + Vector2Int.Distance(b, _targetPos)) / 2;

                return distanciaA == distanciaB ? 0 : distanciaA < distanciaB ? -1 : 1;
            });
    }

    public float Pathfinder(int id, Vector2Int startingPos, Vector2Int endingPos) {
        Tile firstTile = TileMapController.Instance.GetTile(startingPos);
        AStarNode firstNode = new AStarNode(startingPos, null, endingPos, firstTile.PheromoneBias);

        List<AStarNode> toCheckList = new List<AStarNode>();
        List<AStarNode> checkedList = new List<AStarNode>();
        //Dictionary<Vector2Int, float> toCheckDict = new Dictionary<Vector2Int, float>();
        //Dictionary<Vector2Int, float> checkedDict = new Dictionary<Vector2Int, float>();

        int wtf = 0;
        //toCheckDict.Add(firstNode.pos, firstNode.totalDistance);

        toCheckList.Add(firstNode);
        while (toCheckList.Count != 0) {
            wtf++;

            //if (wtf > 100) {
            //    return 0f;
            //}

            AStarNode nodeAtual = toCheckList[0];
            toCheckList.RemoveAt(0);

            AStarNode auxNode;
            foreach (var neighbour in neighbourAux) {
                Vector2Int posAux = nodeAtual.pos + neighbour;
                if (posAux == endingPos) {
                    List<Vector2Int> caminhoFinal = new List<Vector2Int>();
                    caminhoFinal.Add(posAux);
                    caminhoFinal.Add(nodeAtual.pos);

                    auxNode = nodeAtual.parent;
                    while (auxNode != null) {
                        caminhoFinal.Add(auxNode.pos);
                        auxNode = auxNode.parent;
                    }
                    caminhoFinal.Reverse();
                    foreach (var elem in caminhoFinal) {
                        Tile auxTile = TileMapController.Instance.GetTile(elem);
                        auxTile.PheromoneBias *= BiasFactor;
                        if (auxTile.PheromoneBias < BiasCap) {
                            auxTile.PheromoneBias = BiasCap;
                        }
                        TileMapController.Instance.GetTile(elem).RegistraVisita(id);
                    }

                    var daddy = MarkPath(caminhoFinal[0], null);

                    float acumulado = 0;
                    for (int i = 1; i < caminhoFinal.Count; i++) {
                        acumulado += Mathf.Abs(Vector2Int.Distance(caminhoFinal[i - 1], caminhoFinal[i]));

                        daddy = MarkPath(caminhoFinal[i], daddy);

                        //Debug.DrawLine(
                        //    new Vector3(caminhoFinal[i - 1].x, caminhoFinal[i - 1].y, -1f),
                        //    new Vector3(caminhoFinal[i].x, caminhoFinal[i].y, -1f),
                        //    Color.blue,
                        //    1000);
                    }

                    return acumulado;
                }
                
                Tile tileTest = TileMapController.Instance.GetTile(posAux);
                TileType tileType = tileTest == null ? TileType.Invalid : (tileTest.Pathable ? TileType.Pathable : TileType.Blocked);

                if (tileType == TileType.Pathable) {
                    auxNode = new AStarNode(posAux, nodeAtual, endingPos, tileTest.PheromoneBias);

                    var auxAuxNode1 = toCheckList.Find(x => {
                        if (x.pos == auxNode.pos) {
                            return x.totalDistance <= auxNode.totalDistance;
                        }
                        return false;
                    });

                    var auxAuxNode2 = checkedList.Find(x => {
                        if (x.pos == auxNode.pos) {
                            return x.totalDistance <= auxNode.totalDistance;
                        }
                        return false;
                    });

                    if (auxAuxNode1 == null && auxAuxNode2 == null) {
                        int i;
                        for (i = 0; i < toCheckList.Count; i++) {
                            if (auxNode.totalDistance < toCheckList[i].totalDistance) {
                                break;
                            }
                        }
                        toCheckList.Insert(i, auxNode);
                        Instantiate(DebugMark, new Vector3(auxNode.pos.x, auxNode.pos.y, -1f), Quaternion.identity, TileObj.transform);
                    }
                } else if (tileType == TileType.Blocked) {
                    // tratamento da pedra
                    /**
                     * Expandir e encontrar todos os bloqueios adjecentes, salvar posicao, largura e altura da pedra
                     */
                    //FindObstacleByTile(posAux);
                }
            }

            int nodeAtualIndex;
            for (nodeAtualIndex = 0; nodeAtualIndex < checkedList.Count; nodeAtualIndex++) {
                if (nodeAtual.totalDistance < checkedList[nodeAtualIndex].totalDistance) {
                    break;
                }
            }
            checkedList.Insert(nodeAtualIndex, nodeAtual);
        }

        return -1;
    }

    public GameObject MarkPath(Vector2Int pos, GameObject parent) {
        Vector3 posAux = new Vector3(pos.x, pos.y, -2);

        GameObject pPoint = Instantiate(DebugPathMark, posAux, Quaternion.identity, TileObj.transform);

        if (parent != null) {
            PathDrawer pDrawer = pPoint.GetComponent<PathDrawer>();
            pDrawer.SetParent(parent);
        }

        return pPoint;
    }

    public Obstacle FindObstacleByTile(Vector2Int pos) {
        return _obstacleList.Find(elem => {
            var aux = elem.obstaclePositions.Find(position => position == pos);
            if (aux != null) {
                return true;
            }
            return false;
        });
    }

    public void PathfinderColapso() {
        if (_targetPos == null || _unitiesToTest == null || _unitiesToTest.Count == 0) {
            Debug.LogError("Selecione um teste!");
            return;
        }

        List<ColapseNode> checkedList = new List<ColapseNode>();
        UnitQueue unitQueue = new UnitQueue(_unitiesToTest, _targetPos);

        ColapseNode nodeAtual = unitQueue.next();
        while (nodeAtual != null) {
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

    public void WriteLog() {
        string date = System.DateTime.Now.ToString("yyyy-MM-dd");
        string path = "Assets/Logs/Resultados-" + date + ".txt";

        valorBias = "" + BiasFactor;

        string toWrite = totalTestes + ", ";
        toWrite += nomeCenario + ", ";
        toWrite += nomePathfinder + ", ";
        toWrite += valorBias + ", ";
        toWrite += trataVirgula(mediaTeste) + ", ";
        toWrite += trataVirgula(fraternidadeTeste);

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(toWrite);
            writer.Close();
        //print(toWrite);
    }

    public string trataVirgula(float number) {
        string aux = "" + number;
        return aux.Replace(",", ".");
    }
}

public class UnitQueue
{
    List<List<ColapseNode>> queue;
    int iterator;


    public UnitQueue(List<Vector2Int> unities, Vector2Int target) {
        if (unities.Count == 0 || target == null) {
            Debug.LogError("unidades ou posicao alvo invalidas");
            return;
        }
        queue = new List<List<ColapseNode>>();

        foreach (var unit in unities) {
            List<ColapseNode> listAux = new List<ColapseNode>();
            ColapseNode nodeAux = new ColapseNode(unit, null, target);
            listAux.Add(nodeAux);
            queue.Add(listAux);
        }
        iterator = queue.Count;
    }

    public void print() {
        foreach (var list in queue) {
            foreach (var node in list) {
                Debug.Log(node.pos + " - " + node.totalDistance);
            }
        }
    }

    public ColapseNode next() {
        if (iterator >= queue.Count - 1) {
            iterator = 0;
        } else {
            iterator++;
        }

        if (queue.Count == 0) {
            Debug.LogError("Queue vazia!");
            return null;
        } else if (queue[iterator].Count > 0) {
            ColapseNode aux = queue[iterator][0];
            queue[iterator].RemoveAt(0);
            return aux;
        } else {
            queue.RemoveAt(iterator);
            return next();
        }
    }

    public void add(ColapseNode node) {
        int i;
        for (i = 0; i < queue.Count; i++) {
            if (queue[iterator][i].totalDistance > node.totalDistance) {
                break;
            }
        }
        queue[iterator].Insert(i, node);
    }

    public void toCheck(ColapseNode node) {
        foreach (var list in queue) {
            list.Find(elem => {
                return elem.pos == node.pos;
            });
        }
    }
}

public class ColapseUnit {
    public ColapseUnit() { }


}

public class ColapseNode {
    public Vector2Int pos;
    public float heuristicDistance;
    public float realDistance;
    public float totalDistance;
    public ColapseNode parent;
    public int ownerUnit;
    //public List<AStarNode> parent;

    public ColapseNode() { }

    public ColapseNode(Vector2Int pos, ColapseNode parent, Vector2Int endingPos) {
        this.pos = pos;
        //this.parent = new List<AStarNode>();
        this.parent = parent;
        heuristicDistance = Mathf.Sqrt(Mathf.Pow(pos.x - endingPos.x, 2) + Mathf.Pow(pos.y - endingPos.y, 2));
        if (parent != null) {
            //this.parent.Add(parent);
            realDistance = parent.realDistance + Mathf.Sqrt(Mathf.Pow(pos.x - parent.pos.x, 2) + Mathf.Pow(pos.y - parent.pos.y, 2));
            totalDistance = realDistance + heuristicDistance;
        } else {
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

    public AStarNode(Vector2Int pos, AStarNode parent, Vector2Int endingPos, float pheromoneWeight) {
        this.pos = pos;
        //this.parent = new List<AStarNode>();
        this.parent = parent;
        heuristicDistance = Mathf.Sqrt(Mathf.Pow(pos.x - endingPos.x, 2) + Mathf.Pow(pos.y - endingPos.y, 2));
        realDistance = 0;
        if (parent != null) {
            realDistance = parent.realDistance + Mathf.Sqrt(Mathf.Pow(pos.x - parent.pos.x, 2) + Mathf.Pow(pos.y - parent.pos.y, 2));
        }
        totalDistance = realDistance + heuristicDistance;
        totalDistance *= pheromoneWeight;
    }
}

public class Obstacle {
    public string id;
    public List<Vector2Int> obstaclePositions;
    public Vector2Int center;
    public int xSize;
    public int ySize;

    public Obstacle(string id) {
        this.id = id;
        obstaclePositions = new List<Vector2Int>();
    }

    public void AddPosition(Vector2Int pos) {
        obstaclePositions.Add(pos);

        int minX = obstaclePositions[0].x, maxX = obstaclePositions[0].x, minY = obstaclePositions[0].y, maxY = obstaclePositions[0].y;
        float totalX = 0, totalY = 0;
        for (int i = 1; i < obstaclePositions.Count; i++) {
            var obsPos = obstaclePositions[i];
            if (obsPos.x > maxX) {
                maxX = obsPos.x;
            } else if (obsPos.x < minX) {
                minX = obsPos.x;
            }

            if (obsPos.y > maxY) {
                maxY = obsPos.y;
            } else if (obsPos.y < minY) {
                minY = obsPos.y;
            }

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

public class PathCorrectionAuxiliar {
    public Obstacle obstacle;
    public Vector2Int unit;
    public List<Vector2Int> unitPath;

}

public enum TilesEnum {
    Start,
    Ending,
    Pathable,
    Blocked
}
