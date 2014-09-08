using UnityEngine;
using System.Collections;
using System.Linq;

public class ScoresScript : MonoBehaviour {

    public GUIText guiText;
    public GUIText guiLevel;

    Canvas[] g;
    private static int score { get; set; }
    private static int moves { get; set; }

    float timePassed = 0;
	// Use this for initialization
	void Start () {

        float w = Screen.width * 0.35f;
        float h = Screen.height * 0.35f;

        guiText.pixelOffset = new Vector2(w, h);
        guiLevel.pixelOffset = new Vector2(-w, h);

        score = 0;
        g = GameObject.FindObjectsOfType<Canvas>();
        moves = 0;

        guiLevel.text = PersistentInteger.SceneCounter.ToString();

	}
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime    ;

        string timerString = "";
        int currentPainted = score; // 0;
        int numberOfTiles = g.Length; //40

        string output = currentPainted + "/" + numberOfTiles;
        if (timePassed > 3600)
        {
            timerString = "Need help?";
        }
        //else
        //{
        //    timerString = string.Format("{0}:{1:#00.00}", (int)timePassed / 60, timePassed % 60); //timePassed.ToString();
        //}
        guiText.text = output + "\n" + moves + "\n" + timerString;
	}

    public static void UpdateScore()
    {
        score = GameObject.FindObjectsOfType<Canvas>().Where(gg => gg.isPainted).ToArray().Length; //ian was here
        moves++;

    }

}
