using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    /// <summary>
    /// Tooltips for display on the mainmap (Helper class for MainMap)
    /// </summary>
    public class TileTooltip
    {
        #region Private Fields
        private Coords _currentTile;
        private List<string> _toolTipLines = null;
        private bool _updating = false;
        private SpriteFont _font = null;
        private Texture2D _background = null;
        private SpriteBatch _spriteBatch = null;
        private ContentManager _content = null;
        private Rectangle _displayRect;
        private Mainmap _parent;
        private int _lineHeight = 0;
        int _width = 200;
        int _height = 300;
        #endregion

        #region Public Methods
        /// <summary>
        /// Display the current tooltip on current tile (or update text if tile was changed)
        /// </summary>
        /// <param name="tile">The floortile to display the tooltip for</param>
        public void DisplayToolTip(FloorTile tile)
        {
            if (!_updating)
            {
                if (tile.coords != null)
                {
                    if ((_currentTile == null) || ((tile.coords.x != _currentTile.x) || (tile.coords.y != _currentTile.y)))
                    {
                        CreateToolTip(tile);
                    }
                    //tile.coords.x
                    Coords pos = Mainmap._map2screen(_currentTile.x, _currentTile.y + 1, false);

                    Vector2 result = new Vector2(pos.x, pos.y);
                    result = Vector2.Transform(result, _parent.transformMatrix);
                    result.Y += +_displayRect.Y;
                    result.X += +_displayRect.X;
                    if (_height > 10)
                    {
                        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        _spriteBatch.Draw(_background, new Rectangle((int)result.X, (int)result.Y, _width, _height), new Rectangle(39, 6, 1, 1), new Color(0.3f, 0f, 0f, 0.3f));
                        int i = 0;
                        foreach (string s in _toolTipLines)
                        {
                            _spriteBatch.DrawString(_font, s, new Vector2(result.X + 5, result.Y + 5 + _lineHeight * i), Color.White);
                            ++i;

                        }
                        _spriteBatch.End();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a tooltip for a specific tile
        /// </summary>
        /// <param name="tile"></param>
        public void CreateToolTip(FloorTile tile)
        {
            if (!_updating)
            {
                _updating = true;
                _toolTipLines.Clear();
                _height = 10;
                _width = 160;
                foreach (ActorTile enemy in tile.overlay.OfType<ActorTile>())
                {
                    _toolTipLines.Add(_EnemyToolTip(enemy));
                    _height += _lineHeight;
                }

                foreach (ItemTile item in tile.overlay.OfType<ItemTile>())
                {
                    _toolTipLines.Add(_TreasureToolTip(item));
                    _height += _lineHeight;
                }

                foreach (TrapTile trap in tile.overlay.OfType<TrapTile>())
                {
                    _toolTipLines.Add(_TrapToolTip(trap));
                    _height += _lineHeight;
                }

                foreach (TeleportTile teleport in tile.overlay.OfType<TeleportTile>())
                {
                    _toolTipLines.Add(_TeleportToolTip(teleport));
                    _height += _lineHeight;
                }

                foreach (WallTile wall in tile.overlay.OfType<WallTile>())
                {
                    _toolTipLines.Add(_WallToolTip(wall));
                    _height += _lineHeight;
                }
                _currentTile.x = tile.coords.x;
                _currentTile.y = tile.coords.y;
                _updating = false;
            }
        }
        #endregion

        #region Private Methods (Tooltips for different types of objects)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public string _TrapToolTip(Tile a)
        {
            if (!(a is TrapTile)) return "";
            TrapTile trap = a as TrapTile;
            return "Trap (damage: " + trap.damage.ToString() + ")\n";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public string _TeleportToolTip(Tile a)
        {
            if (!(a is TeleportTile)) return "";
            TeleportTile teleport = a as TeleportTile;
            return "Exit to another room\n";
        }

        /// <summary>
        /// Tooltip for items on the floor
        /// </summary>
        /// <param name="a">A Walltile (does not display anything for other types of tile)</param>
        public string _TreasureToolTip(Tile a)
        {
            if (!(a is ItemTile)) return "";
            ItemTile item = a as ItemTile;
            return item.item.name.ToString() + " (" + item.item.strength.ToString() + ")\n";
        }

        /// <summary>
        /// Tooltip for enemies / NPCs
        /// </summary>
        /// <param name="a">An ActorTile (does not display anything for other types of tile)</param>
        private string _EnemyToolTip(Tile a)
        {
            if (!(a is ActorTile)) return "";
            ActorTile actor = a as ActorTile;
            return actor.actor.name + ": " + actor.actor.health.ToString() + "/" + actor.actor.maxHealth.ToString() + "\n";
        }

        /// <summary>
        /// Tooltip for walls (showing sturdiness)
        /// </summary>
        /// <param name="a">A Walltile (does not display anything for other types of tile)</param>
        private string _WallToolTip(Tile a)
        {
            if (!(a is WallTile)) return "";
            WallTile wall = a as WallTile;
            return "Wall\n";
        }
        #endregion

        public TileTooltip(Mainmap parent, SpriteBatch spritebatch, ContentManager content, Rectangle displayRect)
        {
            _spriteBatch = spritebatch;
            _content = content;
            _font = _content.Load<SpriteFont>("SmallFont");
            _background = _content.Load<Texture2D>("Minimap");
            _displayRect = displayRect;
            _lineHeight = (int)(_font.MeasureString("WgjITt").Y) + 1;
            _toolTipLines = new List<String>();
            _currentTile = new Coords(-1, -1);
            _parent = parent;
        }
    }

}
