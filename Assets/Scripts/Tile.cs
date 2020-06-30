using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    public bool Pathable;
    public float PheromoneBias;
    public Vector2Int pos;
    public List<int> visitasProximas;
    public Text text;
    public Color altColor;
    public bool isPath;

    public bool ehhhhr;

    // Start is called before the first frame update
    void Start() {
        int x = (int) transform.position.x;
        int y = (int) transform.position.y;

        if (!TileMapController.Instance.SetTile(x, y, this)) {
            Pop();
        }
        this.pos = new Vector2Int(x, y);
        PheromoneBias = 1f;
        visitasProximas = new List<int>();
        isPath = false;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Pop() {
        Destroy(gameObject);
    }

    public void RegistraVisita(int id) {
        RegistraVisitaAux(id, true);
     
        var tileController = TileMapController.Instance;
        foreach (var neighbour in tileController.neighbourAux) {
            tileController.GetTile(pos + neighbour).RegistraVisitaAux(id);
        }
    }

    public void RegistraVisitaAux(int id, bool main = false) {
        //if (text == null) {
        //    return;
        //}
        if (ehhhhr == false) {
            return;
        }
        
        if (main) {
            isPath = true;
            //text.color = altColor;
        }

        for (int i = 0; i < visitasProximas.Count; i++) {
            if (visitasProximas[i] == id) {
                return;
            }
        }
        visitasProximas.Add(id);
        //text.gameObject.SetActive(true);
        //text.text = "" + visitasProximas.Count;
    }
}
