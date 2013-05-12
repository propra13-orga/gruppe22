using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class WallTile : Tile
    {
        public WallTile(object parent):base(parent)
        {
            _canEnter = false;
        }
    }
}
