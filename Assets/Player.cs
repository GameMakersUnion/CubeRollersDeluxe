using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Player : GamePiece
{
    public bool isPainting = true;
    public Color color;


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }


    public override void Start()
    {
        base.Start();
        RoomManager.roomManager.player = this;
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.UpArrow)) { TryMove(Side.top); }
        if (Input.GetKey(KeyCode.DownArrow)) { TryMove(Side.bottom); }
        if (Input.GetKey(KeyCode.LeftArrow)) { TryMove(Side.left); }
        if (Input.GetKey(KeyCode.RightArrow)) { TryMove(Side.right); }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

    }

    public bool TryMove(Side s)
    {
        if (!isMoving) return moveTo(s);
        return false;
    }

}
