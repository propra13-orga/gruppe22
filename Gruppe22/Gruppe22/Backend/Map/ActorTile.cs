﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        #region Private Fields
        private Actor _actor;
        private int _count = 0;
        private bool _disabled = false;
        #endregion

        #region Public Fields
        public Actor actor
        {
            get { return _actor; }
        }
        
        public ActorType actorType
        {
            get
            {
                return _actor.actorType;
            }
        }

        public bool enabled
        {
            get
            {
                return !_disabled;
            }
            set
            {
                _disabled = !value;
            }
        }
        override public void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ActorTile");
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            xmlw.WriteAttributeString("connected", Convert.ToString(connected));
            xmlw.WriteAttributeString("connection", Convert.ToString(connection));
            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor
        public ActorTile(object parent, Actor actor)
            : this(parent)
        {
            _actor = actor;
        }



        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!(actor is Player))
            {
                Random r = new Random();
                if (!actor.IsDead() && (_count > 20))
                {
                    Direction dir = (Direction)r.Next(4);
                    ((IHandleEvent)parent).HandleEvent(null, Events.MoveActor, actor.id, dir);
                    _count = 0;
                }
                 _count += 1;
            }
        }
        public ActorTile()
            : this(null)
        {
        }

        public ActorTile(object parent)
            : base(parent, null, true)
        {
            _parent = parent;
        }
        #endregion

    }
}
