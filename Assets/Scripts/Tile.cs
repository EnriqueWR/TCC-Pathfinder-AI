using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	public Color altColor;
	public bool contadoNessaPassada;
	public bool ehhhhr;
	public bool isPath;
	public bool Pathable;
	public bool pathNessaPassada;
	public float PheromoneBias;
	public Vector2Int pos;
	public Text text;
	public List<int> visitasProximas;

	private void Start()
	{
		int x = (int) transform.position.x;
		int y = (int) transform.position.y;

		if (!TileMapController.Instance.SetTile(x, y, this))
			Pop();

		pos = new Vector2Int(x, y);
		PheromoneBias = 1f;
		visitasProximas = new List<int>();
		isPath = false;
	}

	private void Update()
	{
		if (text != null)
		{
			if (PheromoneBias == 1f)
				text.text = "";
			else
				text.text = PheromoneBias + "";
		}
	}

	public void Pop()
	{
		Destroy(gameObject);
	}

	public void RegistraVisita(int id)
	{
		RegistraVisitaAux(id, true);
		TileMapController tileController = TileMapController.Instance;

		foreach (Vector2Int neighbour in tileController.neighbourAux)
		{
			tileController.GetTile(pos + neighbour).RegistraVisitaAux(id);
		}
	}

	public void RegistraVisitaAux(int id, bool main = false)
	{
		//if (text == null) {
		//    return;
		//}
		if (ehhhhr == false)
			return;

		if (main)
			isPath = true;

		//text.color = altColor;

		for (int i = 0; i < visitasProximas.Count; i++)
		{
			if (visitasProximas[i] == id)
				return;
		}

		visitasProximas.Add(id);

		//text.gameObject.SetActive(true);
		//text.text = "" + visitasProximas.Count;
	}
}