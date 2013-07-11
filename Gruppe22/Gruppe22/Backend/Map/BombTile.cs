using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile which could be used to manage something like a bomb.
    /// </summary>
    class BombTile:Tile
    {
        /// <summary>
        /// Constructor for a bomb.
        /// </summary>
        /// <param name="parent">parentobject.</param>
        public BombTile(object parent)
            : base(parent)
        {
        }
    }
}
