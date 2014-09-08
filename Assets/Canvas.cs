using UnityEngine;
using System.Collections;

public class Canvas : GamePiece {
    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }
    public bool isPainted;

    public Material painted;
    public Material unPainted;

    private static AudioClip DripSound;
    private static AudioClip ConcreteSound;
    private static AudioClip SuckSound;

    static Canvas()
    {
        DripSound = Resources.Load<AudioClip>("paintsploosh");
        ConcreteSound = Resources.Load<AudioClip>("conrcreteStep");
        SuckSound = Resources.Load<AudioClip>("suck");
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (piece is Player && (piece as Player).isPainting)
        {
            isPainted = !isPainted;
            this.Paint();
            ScoresScript.UpdateScore(); //ian was here
            AudioClip sfxToPlay = isPainted ? DripSound : SuckSound;
            AudioSource.PlayClipAtPoint(sfxToPlay, Camera.main.transform.position, 0.5f);

        }
        else if (piece is Player && !(piece as Player).isPainting)
        {
            AudioSource.PlayClipAtPoint(ConcreteSound, Camera.main.transform.position, 1.0f);
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
