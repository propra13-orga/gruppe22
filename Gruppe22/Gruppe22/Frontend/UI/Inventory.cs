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
                    _icons.Add(new GridElement(i, _actor.inventory[i].name + (_actor.inventory[i].equipped ? " (equipped)" : ""), _actor.inventory[i].icon, _actor.inventory[i].equipped));
            }
        }

        public override void OnMouseDown(int button)
        {
            int i = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            if ((i > -1) && (i < _actor.inventory.Count))
            {
                i = _icons[i].id;
                if (_actor.inventory[i].itemType == ItemType.Potion)
                {
                    _actor.inventory[i].UseItem();
                    _parent.HandleEvent(false, Events.ShowMessage, "You used " + _actor.inventory[i].name);
                }
                else
                {
                    _actor.inventory[i].EquipItem();
                    if (_actor.inventory[i].equipped)
                        _parent.HandleEvent(false, Events.ShowMessage, "You equipped " + _actor.inventory[i].name);
                    else
                        _parent.HandleEvent(false, Events.ShowMessage, "You removed " + _actor.inventory[i].name);
                }
                Update();
            }
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
