using UnityEngine;
using System.Collections;

public class PersistentInteger : MonoBehaviour {
    public static int SceneCounter = 1;
    private static PersistentInteger _instance = null;
    public static PersistentInteger instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PersistentInteger>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
	// Use this for initialization
	void Start () {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }

	}
	public void SwitchScene()
    {
        try
        {
            Application.LoadLevel(++SceneCounter);
        }
        catch
        {
            Debug.Log("No more levels to load.");
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
