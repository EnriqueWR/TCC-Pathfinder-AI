using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapController : MonoBehaviour {
    public static TileMapController Instance;

    public Dictionary<Vector2Int, Tile> Tiles;

    public Vector2Int[] neighbourAux = {
        new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1)
    };

    private void Awake() {
        Instance = this;
        Tiles = new Dictionary<Vector2Int, Tile>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public bool SetTile(int x, int y, Tile tile) {
        Vector2Int key = new Vector2Int(x, y);

        if (!Tiles.ContainsKey(key)) {
            Tiles.Add(key, tile);
            return true;
        } else {
            return false;
        }
    }

    public TileType ValidTile(Vector2Int pos) {
        Tile aux;
        if (Tiles.TryGetValue(pos, out aux)) {
            return aux.Pathable ? TileType.Pathable : TileType.Blocked;
        }

        return TileType.Invalid;
    }

    public Tile GetTile(Vector2Int pos) {
        Tile aux;
        Tiles.TryGetValue(pos, out aux);
        return aux;
    }

    public void PrintTiles() {
        foreach (var key in Tiles.Keys) {
            Debug.Log(key);
            Debug.Log(Tiles[key].Pathable);
        }
    }

    public float CalculateFraternity() {
        float total = 0;
        float qtdCell = 0;

        foreach(var tileInDictionary in Tiles) {
            var tile = tileInDictionary.Value;

            if (tile.isPath) {
                total += tile.visitasProximas.Count;
                qtdCell++;
            }
        }


        return qtdCell > 0 ? (total / qtdCell) : 0;
    }
}

public enum TileType {
    Pathable,
    Blocked,
    Invalid
}
