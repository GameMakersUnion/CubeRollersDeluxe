using UnityEngine;
using System.Collections;

public class Canvas : GamePiece {
    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }
    public bool isPainted;

    public Material painted;
    public Material unPainted;

    private static AudioClip DripSound;

    static Canvas()
    {
        DripSound = Resources.Load<AudioClip>("paintsploosh");
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (piece is Player && (piece as Player).isPainting)
        {
            isPainted = !isPainted;
            this.Paint();
            ScoresScript.UpdateScore(); //ian was here
            AudioSource.PlayClipAtPoint(DripSound,transform.position);

        }
        RoomManager.roomManager.CheckAllCanvi();
        return base.onOccupy(piece);
    }

    private void Paint()
    {
        this.renderer.material = isPainted ? painted : unPainted;
    }
    public override void Awake() {
        base.Awake();
        Paint();
    }

    public void OnValidate(){
            Paint();
    }
}
