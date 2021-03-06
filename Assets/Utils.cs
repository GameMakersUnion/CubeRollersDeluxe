using System;
using UnityEngine;
using System.Collections.Generic;

public static class Utils
{
    public static Side opposite(this Side s){
	    switch(s){
	    case Side.bottom:
		    return Side.top;
	    case Side.top:
		    return Side.bottom;
	    case Side.left:
		    return Side.right;
	    case Side.right:
		    return Side.left;
	    }throw new WTFException();
    }
    public static GameObject GetParent(this GameObject child, string name = null)
    {
        if (child.transform.parent == null) return null;
        if (name == null || child.transform.parent.gameObject.name == name)return child.transform.parent.gameObject;
        return child.transform.parent.gameObject.GetParent(name);
    }
    public static bool HasParent(this GameObject child, string name)
    {
        return (child.GetParent(name) != null);
    }
    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    public static void FillAndBorder(this GameObject obj, Color fillColor)
    {
        try
        {
            SpriteRenderer fill = obj.transform.FindChild("Fill").GetComponent<SpriteRenderer>();
            SpriteRenderer border = obj.transform.FindChild("Border").GetComponent<SpriteRenderer>();

            fill.color = fillColor;
            border.color = fillColor.Invert();


        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Tried to call Fill and Border on a NonCompliant Item.");
            throw e;
        }
    }
    
    public static Color Invert(this Color color){
        return new Color(.3f * color.r, .3f * color.g, .3f * color.b);
    }
	public static bool isWithinGrid(this Vector2 worldPos){

        if (worldPos.x > Values.gridWidth * Values.blockSize || worldPos.x < 0) return false;
        if (worldPos.y > Values.gridHeight * Values.blockSize || worldPos.y < 0) return false;
		return true;
	}
    public static string UppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
	public static Vector2 WorldToWallPos(Vector2 worldPos, out Side s, out Orientation orientation)
	{


        int blockSize = Values.blockSize;
        int cellx = (int)Mathf.Floor(worldPos.x / blockSize);
        int celly = (int)Mathf.Floor(worldPos.y / blockSize);

		int originX = cellx * blockSize;
		int originY = celly * blockSize;

		if (!new Vector2(originX, originY).isWithinGrid()) throw new IndexOutOfRangeException("Don't Use a try Catch, check using isWithinGrid");



		float x = worldPos.x - originX;
		float y = worldPos.y - originY;

		Vector3 vect = Vector3.zero;
		if (x > y)
		{
            if (x < Values.blockSize - y)
			{
				s = Side.bottom;
                vect.x += Values.halfBlock;
				orientation = Orientation.Horizontal;
			}
			else
			{
				s = Side.right;
                vect.x += Values.blockSize;
                vect.y += Values.halfBlock;
				orientation = Orientation.Vertical;
			}
		}
		else
		{
            if (x < Values.blockSize - y)
			{
				s = Side.left;
                vect.y += Values.halfBlock;
				orientation = Orientation.Vertical;
			}
			else
			{
				s = Side.top;
                vect.x += Values.halfBlock;
                vect.y += Values.blockSize;
				orientation = Orientation.Horizontal;
			}
		}

        //if (cellx == RoomManager.roomManager.Grid.Length)
        //{
        //    s = s.opposite();
        //}
        //if (celly == RoomManager.roomManager.Grid[0].Length)
        //{
        //    s = s.opposite();
        //}

		vect.x += originX;
		vect.y += originY;
		return vect;
	}


}
	
	public class WTFException : Exception{
		public const string WTF= "What the Fjord?!?!";
		public WTFException():base(WTF){
		}
		public WTFException(String s):base(s){
			Debug.Log(WTF);
		}
		public WTFException(Exception e):base(WTF, e){
		}
		public WTFException(String s, Exception e):base(s, e){
			Debug.Log(WTF);
		}
	}


    //convert dictionary<primitive, List<primitive>> to 2 arrays for serialization to be accessible in Unity inspector
    public class DictionaryToArraysMagic
    {
        public List<string> zNames, zLevels;
        public List<int> zIndexes;
        public List<string> getNames { get { return zNames; } }
        public List<string> getLevels { get { return zLevels; } }
        public List<int> getIndexes { get { return zIndexes; } }
        public DictionaryToArraysMagic(Dictionary<string, List<string>> masterScenes)
        {
            zNames = new List<string>();
            zLevels = new List<string>();
            zIndexes = new List<int>();

            int count = 0;
            foreach (string name in masterScenes.Keys)
            {
                List<string> levelsAtName = masterScenes[name];
                if (levelsAtName.Count == 0) continue;

                zNames.Add(name);
                zIndexes.Add(levelsAtName.Count);
                foreach (string level in levelsAtName)
                {
                    zLevels.Add(level);
                    count++;
                }
            }
        }

        //private string[] names;
        //private string[][] scenes;
        //public DictionaryToArraysMagic(Dictionary<string, List<string>> masterScenes) 
        //{
        //    //simply initilize this array
        //    names = new string[masterScenes.Count];
        //
        //    
        //    int countMax = 0;
        //    int i = 0;
        //    foreach (KeyValuePair<string, List<string>> sceneSet in masterScenes) 
        //    {
        //        //determine other array max size
        //        if (sceneSet.Value.Count > countMax) countMax = sceneSet.Value.Count; 
        //
        //        //populate first array
        //        names [i++] = sceneSet.Key;
        //    }
        //
        //    //allocate (init?) other array
        //    scenes = new string[masterScenes.Count][];
        //    for (int j = 0; j < scenes.Length; j++) 
        //    {
        //        scenes[j] = new string[countMax];
        //    }
        //
        //    //populate second array
        //    int x = 0;
        //    foreach (KeyValuePair<string, List<string>> sceneSet in masterScenes)
        //    {
        //        int y = 0;
        //        foreach (string scene in sceneSet.Value) 
        //        {
        //            scenes[x][y] = scene;
        //            y++;
        //        }
        //        x++;
        //    }
        //}
        //public string[] get1d() { return names; }
        //public string[][] get2d() { return scenes; }
    }

    //convert dictionary<primitive, List<primitive>> to 2 arrays for serialization to be accessible in Unity inspector
    