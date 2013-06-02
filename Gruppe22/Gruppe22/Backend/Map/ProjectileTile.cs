using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    public class ProjectileTile : Tile
    {
        private Direction _direction = Direction.None;
        private bool _working = false;
        private uint _timetothink = 60;
        private uint _elapsed = 0;
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
        public ProjectileTile(FloorTile parent, Direction dir, uint id = 0)
            : base(parent)
        {
            _direction = dir;
            _id = 0;
        }

        public void NextTile(bool doMove = false)
        {
            Map map = (Map)((FloorTile)_parent).parent;
            FloorTile target = map.TileByCoords(Map.DirectionTile(coords, _direction));

            if (doMove)
            {
                ((FloorTile)_parent).Remove(this);
                if ((target.coords.x > -1) &&
                    (target.coords.y > -1))
                {
                    target.Add(this);

                }
                else
                {
                    ((IHandleEvent)parent).HandleEvent(false, Events.ExplodeProjectile, this, coords, null);
                    return;
                }
                target = map.TileByCoords(Map.DirectionTile(coords, _direction));

            }
            if (!target.hasWall && ((!target.hasPlayer &&
    !target.hasEnemy)
    || target.firstActor.isDead)

    )
            {
                ((IHandleEvent)parent).HandleEvent(false, Events.MoveProjectile, this, target.coords);
            }
            else
            {
                ((IHandleEvent)parent).HandleEvent(false, Events.ExplodeProjectile, this, target, map.TileByCoords(Map.DirectionTile(coords, _direction)).firstActor);
            }
        }


        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ProjectileTile");
            xmlw.WriteEndElement();
        }
    }
}
