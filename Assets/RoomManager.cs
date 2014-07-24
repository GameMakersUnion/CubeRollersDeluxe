using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;
    public static GameObject masterParent;
    public static int SceneCounter = 0;

    public static Dictionary<Type, GameObject> pieceParents = new Dictionary<Type,GameObject>();
    public Cell[][] Grid;
    public Player player;

    public enum ButtonOptions
    {
        ActivateOnAllPushed,
        ActivateOnOnePushed,
    }
    public ButtonOptions buttonOptions = ButtonOptions.ActivateOnAllPushed;

    void Awake() {
        if (Application.isPlaying)
        {
            GameObject preexisting = GameObject.Find("Indicator");
            if (preexisting != null) DestroyImmediate(preexisting);
        }
        if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");

        roomManager = this;
        Grid = new Cell[Values.gridWidth][];
        for (int i = 0; i < Values.gridWidth; i++)
        {
            Grid[i] = new Cell[Values.gridHeight];
            for (int j = 0; j < Values.gridHeight; j++)
            {
                Grid[i][j] = new Cell(i, j);
            }
        }
        List<GameObject> walls = GameObject.FindObjectsOfType<Wall>().Select(w => w.gameObject).ToList();

		foreach (GameObject wallobj in walls)
        {
            //if (wallobj.GetComponent<Door>() != null) Debug.Log("FOUND DOOR");
			Wall wall  = wallobj.GetComponent<Wall>();
			if(wall != null){
				if(((Vector2)wall.transform.position).isWithinGrid()){
                    AddWall(wall);
				}
				else{
                    Debug.Log("Wall was found out of Grid Range @ " + wall.transform.position);
				}
			}
        }
        if (player == null)
        {
            Debug.LogWarning("Level needs <color=magenta>player</color>, add with <color=magenta>PuzzleMaker plugin</color>");
        }
    }
    public List<T> GetPiecesOfType<T>() where T : GamePiece
    {
        List<T> list = new List<T>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell cell = Grid[i][j];
                if (cell == null) continue;
                var cellList = cell.pieces.ToList();
                foreach (var gamepiece in cellList)
                {
                    if (gamepiece is T && !list.Contains((T)gamepiece))
                    {
                        list.Add((T)gamepiece);
                    }
                }
            }
        }
        return list;
    }



    public void AddPiece(GameObject gameobject, Type t)
    {
        if (!t.IsSubclassOf(typeof(GamePiece))) throw new System.Exception("Tried to add a non-GamePiece to a Cell");
        GamePiece gamePiece;
        gamePiece = (GamePiece)gameobject.GetComponent(t);
        if (gamePiece == null)
        {
            gamePiece = (GamePiece)gameobject.AddComponent(t);
        }
        //gameobject.name = 
        if (t == typeof(Player))
        {
            player = (Player)gamePiece;
        }
    }

	void Update () {
        if(!Application.isPlaying && roomManager == null)
        { roomManager = this; Awake(); }
	}
    public void RemoveTopPiece(Cell target)
    {
		if (target != null)
        {
			var gamepieces = target.pieces.ToList();
			if (!target.HasPiece()) return;
            GamePiece g = gamepieces.Last();
            g.Destroy();
        }
    }

	public void RemoveWall(Vector2 position)
    {
        Side side; Orientation orient;
        Utils.WorldToWallPos(position, out side, out orient);
        Cell cell = Cell.GetFromWorldPos(position);

        if (cell == null)
        {
            cell = FindBorderCell(position);
            if (cell != null)
            {
                side = side.opposite();
            }
            else
            {
                return;
            }
        }
		if (cell.getWall(side) != null)
        {
            if (cell.getWall(side).gameObject) DestroyImmediate(cell.getWall(side).gameObject);
        }
		cell.setWall(side, null);
		Cell neighbour = cell.getNeighbour(side);
		if (neighbour != null){
			neighbour.setWall(Utils.opposite(side), null);
	    }
	}
    public void AddWall(Wall wall)
    {
        Side side; Orientation orient;
        Utils.WorldToWallPos(wall.transform.position, out side, out orient);


        Cell cell = Cell.GetFromWorldPos(wall.transform.position);
        if (cell == null)
        {
            cell = FindBorderCell(wall.transform.position);
            if (cell != null)
            {
                side = side.opposite();
            }
            else
            {
                return;
            }
        }
        Wall alreadyThere = cell.getWall(side);
        if (alreadyThere != null)
        {
            RemoveWall(wall.transform.position);
        }
        cell.setWall(side, wall);
        Cell neighbour = cell.getNeighbour(side);
        if (neighbour != null)
        {
            neighbour.setWall(Utils.opposite(side), wall);
        }
    }
    public Cell FindBorderCell(Vector3 position)
    {
        Cell cell = null;
        int x = Cell.GetCellX(position.x);
        int y = Cell.GetCellX(position.y);
        if (position.x / Values.blockSize == Grid.Length)
        {
            cell = Grid[x - 1][y];
        }
        else if (position.y / Values.blockSize == Grid[0].Length)
        {
            cell = Grid[x][y - 1];
        }
        return cell;
    }

    private static AudioClip tada;

    static RoomManager()
    {
        tada = Resources.Load<AudioClip>("tada");
    }
    public void CheckAllCanvi()
    {
        var canvi = GetPiecesOfType<Canvas>();
        if (canvi.Count == 0) return;
        foreach(var canva in canvi)
        {
            if (!canva.isPainted)
            {
                return;
            }
        }
        AudioSource.PlayClipAtPoint(tada, Vector3.zero);
        PersistentInteger.instance.SwitchScene();
    }
}
