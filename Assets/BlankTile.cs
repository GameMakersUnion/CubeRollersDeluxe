using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class BlankTile : GamePiece
    {
        public override bool isSolid { get; set; }
        public override bool isPushable { get; set; }
    }
}
