﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    /// <summary>
    /// The tile for projectiles shot by actors
    /// e.g. a arrow or a spell
    /// </summary>
    public class ProjectileTile : Backend.Tile
    {
        private Backend.Direction _direction = Backend.Direction.None;
        private uint _id = 0;

        public uint id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">parentobject</param>
        /// <param name="dir">The direction in which the projectile will be shot</param>
        /// <param name="id"></param>
        public ProjectileTile(Backend.FloorTile parent, Backend.Direction dir, uint id = 0)
            : base(parent)
        {
            _direction = dir;
            _id = 0;
        }

        public void NextTile(bool doMove = false)
        {
            Backend.Map map = (Backend.Map)((Backend.FloorTile)_parent).parent;
            Backend.FloorTile target = map.TileByCoords(Backend.Map.DirectionTile(coords, _direction));
            if (target.coords.x == -1)
            {
                ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.ExplodeProjectile, this, coords, null);
                return;
            }
            if (doMove)
            {


                ((Backend.FloorTile)_parent).Remove(this);

                target.Add(this);
                target = map.TileByCoords(Backend.Map.DirectionTile(coords, _direction));

                if (target.coords.x == -1)
                {
                    ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.ExplodeProjectile, this, coords, null);
                    return;
                }

            }
            if (!target.hasWall && ((!target.hasPlayer &&
    !target.hasEnemy)
    || target.firstActor.isDead)

    )
            {
                ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.MoveProjectile, this, target.coords);
            }
            else
            {
                ((Backend.IHandleEvent)parent).HandleEvent(false, Backend.Events.ExplodeProjectile, this, target.coords, map.TileByCoords(Backend.Map.DirectionTile(coords, _direction)).firstActor);
            }
        }

        /// <summary>
        /// Method to save the PrjectileTile in a .xml file
        /// </summary>
        /// <param name="xmlw"></param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ProjectileTile");
            xmlw.WriteAttributeString("direction", _direction.ToString());
            xmlw.WriteAttributeString("id", _id.ToString());
            xmlw.WriteEndElement();
        }
    }
}
