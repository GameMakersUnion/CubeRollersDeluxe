using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public abstract class GamePiece : MonoBehaviour
{
    public static Dictionary<Type, int> spawnNumbers = new Dictionary<Type, int>();
    public Cell _cell ;
    public Cell cell { get { return _cell; } set { _cell = value; if (value!=null)transform.position = value.WorldPos(); } }
	
	Cell destination;
    protected bool isMoving = false;
    private float moveSpeed = 5f, teleportSpeed = 10f;
    private float tempSpeed = 5f;
    private float currentLerp = 0f, maxLerp = 100f;
    private Vector2 StartPos;

    public GamePiece previousPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            int index = cell.pieces.IndexOf(this);
            if (index == 0) return null;
            return cell.pieces[index - 1];
        }
    }

    public int getZPosition()
    {
        return cell.pieces.IndexOf(this);
    }

    public GamePiece nextPiece
    {
        get
        {
            if (cell == null) return null;
            if (!cell.pieces.Contains(this)) throw new WTFException("Piece was expected in cell pieces list.");
            int index = cell.pieces.IndexOf(this);
            if (index == cell.pieces.Count - 1) return null;
            return cell.pieces[index + 1];
        }
    }
    public abstract bool isSolid { get; set; }

    private int _weight = Values.defaultWeight;
	public virtual int weight { get{return _weight;} set{_weight=value;}}

    public abstract bool isPushable { get; set; }

    public bool IsOccupied { get { return nextPiece != null; } }

    public virtual void Awake() 
    {
        
    }
    public virtual void Start() 
    {
        Cell get = Cell.GetFromWorldPos(transform.position);
        int z = -(int)transform.position.z;
        get.QueuedOccupy(z, this);
    }

    public virtual void Update()
    {
        if (isMoving)
        {
            if (currentLerp >= maxLerp)
            {
                currentLerp = 0f;

                Debug.Log(transform.eulerAngles);
                
                //transform.rotation = Quaternion.Euler(new Vector3(currentRot.x + x, currentRot.y + y, currentRot.z));
                isMoving = false;
                Detatch();
                destination.Unreserve();
                Cell temp = destination;
                destination = null;

                if (this is Player)
                {
                    Player myself = (Player)this;
                    myself.isPainting = !((int)transform.eulerAngles.x == 0 && (int)transform.eulerAngles.y == 180);
                    Debug.Log(transform.eulerAngles + " rains? : " + myself.isPainting);

                }
                temp.Occupy(this);

                

            }
            else
            {
                transform.position = Vector2.Lerp(StartPos, destination.WorldPos(), currentLerp / 100f);

                float TimesYouWillRotate = 100f/tempSpeed;
                float RotIncrement = 90f / TimesYouWillRotate;
                float x = 0, y = 0;
                switch (targetRot)
                {
                    case Side.bottom:
                        transform.Rotate(Vector3.left, RotIncrement,Space.World);
                        break;
                    case Side.top:
                        transform.Rotate(Vector3.left, -RotIncrement, Space.World);
                        break;
                    case Side.right:
                        transform.Rotate(Vector3.up, -RotIncrement, Space.World);
                        break;
                    case Side.left:
                        transform.Rotate(Vector3.up, RotIncrement, Space.World);
                        break;
                    default:
                        break;
                }

                currentLerp += tempSpeed;
            }
        }
        if (cell != null)
            transform.position = new Vector3(transform.position.x, transform.position.y, -getZPosition());
        }

    public List<GamePiece> GetPiecesAbove()
    {
        List<GamePiece> above = new List<GamePiece>();
        if (cell == null || cell.pieces == null) return above;
        bool found = false;
        foreach(var piece in cell.pieces)
        {
            if (found) above.Add(piece);
            else if (piece == this) found = true;
        }
        return above;
    }
    public List<GamePiece> GetPiecesBelow()
    {
        List<GamePiece> below = new List<GamePiece>();
        if (cell == null || cell.pieces == null) return below;
        foreach (var piece in cell.pieces)
        {
            if (piece == this) break;
            else below.Add(piece);
        }
        return below;
    }

    [Flags]public enum Axis
    {
        Xaxis = 1,
        Yaxis = 2,
        Zaxis = 4,
    }
    public static Vector3 getAngleVector(float angle, Axis axis)
    {
        switch (axis)
        {
            case Axis.Xaxis: return new Vector3(angle, 0, 0);
            case Axis.Yaxis: return new Vector3(0, angle, 0);
            case Axis.Xaxis | Axis.Yaxis: return new Vector3(angle, angle, 0);
            case Axis.Zaxis: return new Vector3(0, 0, angle);
            case Axis.Xaxis | Axis.Zaxis: return new Vector3(angle, 0, angle);
            case Axis.Yaxis | Axis.Zaxis: return new Vector3(0, angle, angle);
            case Axis.Xaxis | Axis.Yaxis | Axis.Zaxis: return new Vector3(angle, angle, angle);
            default: return new Vector3(0, 0, 0);
        }
    }
    public virtual bool onOccupy(GamePiece piece)
    {
        return true;
    }

    public virtual bool canBeOccupiedBy(GamePiece piece)
    {
        return true;
    }
    public virtual void onDeOccupy(GamePiece piece) 
    {
    }
    public virtual bool moveTo(Side side) {
		if (isMoving) return false;
        if (cell == null)
            return false;
		Cell dest = cell.getNeighbour(side);
        Wall w = cell.getWall(side);
        if(w!=null) return false;
        if (dest == null) return false;
        return StartLerp(cell, dest, moveSpeed, side);
	}

    Vector3 currentRot;
    private Side? targetRot = null;
    public bool StartLerp(Cell source, Cell dest, float speed, Side? s = null)
    {
        bool available = dest.Reserve() && dest != null;
        if (available)
        {
            isMoving = true;
            StartPos = source.WorldPos();
            destination = dest;
            tempSpeed = speed;
            currentLerp = 0f;

            currentRot = transform.eulerAngles;
            targetRot = s;

        }
        return available;
    }
    
	public virtual bool TeleportTo(Cell target)
    {
		Cell currentCell = cell;
		if (target.IsSolidlyOccupied())return false;
		Detatch();

        return StartLerp(currentCell, target, teleportSpeed);
	}
    public void Detatch()
    {
        if (cell == null) return;
        //call onDeOccupy only for pieces Under you (possibly revisable design decision)
        List<GamePiece> piecesCopy = cell.pieces.ToList();
        cell.pieces.Remove(this);
        cell = null;
        foreach (GamePiece piece in piecesCopy)
        {
            if (piece == this) break;
            piece.onDeOccupy(this);
        }
    }
    //detatches with all children, calls onDeOccupy for all pieces underneath, and returns full list of detatched pieces
    public List<GamePiece> DetatchWithChilren()
    {
        if (cell == null) return null;
        var detatchList = new List<GamePiece>();
        var currentPieces = cell.pieces.ToList();
        int thisPieceIndex = currentPieces.IndexOf(this);
        for (int i = thisPieceIndex; i < currentPieces.Count; i++)
        {
            GamePiece piece = currentPieces[i];
            piece.Detatch();
            detatchList.Add(piece);
        }
        return detatchList;

    }
    public void Destroy()
    {
        Detatch();
        if (this.gameObject) DestroyImmediate(this.gameObject);
        
    }
    //----------------
    public void DestroyWithChildren()
    {
        List<GamePiece> detatched = DetatchWithChilren();
        foreach(var piece in detatched)
        {
            if (piece.gameObject) DestroyImmediate(piece.gameObject);
        }
    }
    public virtual void OnDestroy(){
        if (cell != null) Detatch();
    }

    public static Dictionary<Type, GameObject> PrefabCache = new Dictionary<Type, GameObject>();
    public static GameObject GetPrefab(Type piece)
    {
        if (piece != typeof(Wall) && (!piece.IsSubclassOf(typeof(GamePiece)) || piece.IsAbstract)) throw new WTFException("Trying to get a prefab of a non-piece/non-wall class");
        if (!PrefabCache.ContainsKey(piece))
            PrefabCache[piece] = Resources.Load<GameObject>("Prefabs/" + piece.Name);
        return PrefabCache[piece];
    }
}
