using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    public class Inventory : Grid
    {
        #region Private Fields
        private Backend.Actor _actor = null;
        private int _elapsed = 0;
        #endregion

        #region Public Fields
        public Backend.Actor actor
        {
            get
            {
                return _actor;
            }
            set
            {
                _actor = value;
            }
        }
        public void Update()
        {
            _icons.Clear();
            for (int i = 0; i < _actor.inventory.Count; ++i)
            {
                if (!_actor.inventory[i].destroyed)
                {
                    _icons.Add(new GridElement(-_actor.inventory[i].id, _actor.inventory[i].name + (_actor.inventory[i].equipped ? " (equipped)" : "") + _actor.inventory[i].abilityList,
                        _actor.inventory[i].icon, _content, _actor.inventory[i].equipped, true, ((_actor.inventory[i].isNew) ? -1 : 0)));
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {

            if (_visible)
            {
                _elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (_elapsed > 350)
                {
                    _elapsed = 0;
                    for (int i = 0; i < _icons.Count; ++i)
                    {
                        if (_icons[i].flash > 0)
                        {
                            _icons[i].flash -= 1;
                        }
                        if (_icons[i].flash < 0)
                        {
                            if (_icons[i].flash == -1)
                            {
                                _icons[i].flash = -2;
                            }
                            else
                            {
                                _icons[i].flash = -1;
                            }
                        }
                    }
                }
                for (int i = 0; i < _actor.inventory.Count; ++i)
                {
                    _actor.inventory[i].isNew = false;
                }
                _actor.newItems = 0;
            }
            base.Draw(gameTime);
        }
        public override bool OnMouseDown(int button)
        {
            if (_visible)
            {
                int selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
                if ((selected > -1) && (selected < _icons.Count))
                {
                    int i = -_icons[selected].id;
                    if (_actor.Items(i).itemType == Backend.ItemType.Potion)
                    {
                        //   _actor.inventory[i].UseItem();
                        //   _parent.HandleEvent(false, Backend.Events.ShowMessage, "You used " + _actor.inventory[i].name);
                        _parent.HandleEvent(false, Backend.Events.AddDragItem, _icons[selected]);

                    }
                    else
                    {
                        _actor.Items(i).EquipItem();
                        if (_actor.Items(i).equipped)
                        {
                            for (int j = 0; j < _icons.Count; ++j)
                            {
                                if ((_icons[j].id != -i) && (_actor.Items(-_icons[j].id).itemType == _actor.Items(i).itemType))
                                {
                                    if (_actor.Items(-_icons[j].id).equipped) _actor.Items(-_icons[j].id).EquipItem();
                                    _icons[j].check = false;
                                }
                            }
                            _parent.HandleEvent(false, Backend.Events.ShowMessage, "You equipped " + _actor.Items(i).name);
                        }
                        else
                        {
                            _parent.HandleEvent(false, Backend.Events.ShowMessage, "You removed " + _actor.Items(i).name);
                        }
                    }
                    Update();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Constructor
        public Inventory(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion
    }
}
