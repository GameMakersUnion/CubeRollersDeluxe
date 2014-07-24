using UnityEngine;
using System.Collections;

public enum Orientation { Horizontal, Vertical };

[ExecuteInEditMode]
 public class Wall : MonoBehaviour {


    public Color colorPreview;
    public Orientation orientation = Orientation.Vertical;

    // Use this for initialization
    protected virtual void Start()
    {
        if (transform.rotation.Equals(Quaternion.identity))
        {
            orientation = Orientation.Vertical;
            transform.position = new Vector3(
                ((int)Mathf.Round(transform.position.x / Values.blockSize)) * Values.blockSize,
                ((int)Mathf.Round((transform.position.y - Values.halfBlock) / Values.blockSize)) * Values.blockSize + Values.halfBlock,
                transform.position.z);

            if ((int)transform.rotation.eulerAngles.z % 180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            orientation = Orientation.Horizontal;
            transform.position = new Vector3(
                ((int)Mathf.Round((transform.position.x - Values.halfBlock) / Values.blockSize)) * Values.blockSize + Values.halfBlock,
                ((int)Mathf.Round(transform.position.y / Values.blockSize)) * Values.blockSize,
                transform.position.z);

            if ((int)(transform.rotation.eulerAngles.z + 90) % 180 != 0)
                transform.eulerAngles = new Vector3(0, 0, 90);
        }
	}    
 }
