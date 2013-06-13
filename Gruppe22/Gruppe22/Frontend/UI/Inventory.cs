using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public class Inventory : Grid
    {
        #region Private Fields
        Actor _actor = null;
        #endregion

        #region Public Fields
        public Actor actor
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
                    _icons.Add(new GridElement(-_actor.inventory[i].id, _actor.inventory[i].name + (_actor.inventory[i].equipped ? " (equipped)" : "") + _actor.inventory[i].abilityList, _actor.inventory[i].icon, _actor.inventory[i].equipped));
            }
        }

        public override bool OnMouseDown(int button)
        {

            int selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            if ((selected > -1) && (selected < _icons.Count))
            {
                int i = -_icons[selected].id;
                if (_actor.Items(i).itemType == ItemType.Potion)
                {
                    //   _actor.inventory[i].UseItem();
                    //   _parent.HandleEvent(false, Events.ShowMessage, "You used " + _actor.inventory[i].name);
                    _parent.HandleEvent(false, Events.AddDragItem, _icons[selected]);

                }
                else
                {
                    _actor.Items(i).EquipItem();
                    if (_actor.Items(i).equipped)
                        _parent.HandleEvent(false, Events.ShowMessage, "You equipped " + _actor.Items(i).name);
                    else
                        _parent.HandleEvent(false, Events.ShowMessage, "You removed " + _actor.Items(i).name);
                }
                Update();
                return true;
            }
            return false;
        }
        #endregion

        #region Constructor
        public Inventory(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion
    }
}
