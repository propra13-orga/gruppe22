using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    /// <summary>
    /// Tooltips for display on the mainmap (Helper class for MainMap)
    /// </summary>
    public class TileTooltip
    {
        #region Private Fields
        private Coords _currentTile;
        private string _currentTooltip = "";
        private bool _updating = true;
        private SpriteFont _font = null;
        
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
                if ((tile.coords.x != _currentTile.x) || (tile.coords.y != _currentTile.y))
                {
                    CreateToolTip(tile);
                }
                //tile.coords.x
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
                _currentTooltip = "";
                foreach (ActorTile enemy in tile.overlay.OfType<ActorTile>())
                {
                    _currentTooltip += _EnemyToolTip(enemy);
                }

                foreach (ItemTile item in tile.overlay.OfType<ItemTile>())
                {
                    _currentTooltip += _TreasureToolTip(item);
                }

                foreach (TrapTile trap in tile.overlay.OfType<TrapTile>())
                {
                    _currentTooltip += _TrapToolTip(trap);
                }

                foreach (TeleportTile teleport in tile.overlay.OfType<TeleportTile>())
                {
                    _currentTooltip += _TeleportToolTip(teleport);
                }

                foreach (WallTile wall in tile.overlay.OfType<WallTile>())
                {
                    _currentTooltip += _WallToolTip(wall);
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
            return "Trap";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public string _TeleportToolTip(Tile a)
        {
            if (!(a is TeleportTile)) return "";
            TeleportTile teleport = a as TeleportTile;
            return "Teleporter";
        }

        /// <summary>
        /// Tooltip for items on the floor
        /// </summary>
        /// <param name="a">A Walltile (does not display anything for other types of tile)</param>
        public string _TreasureToolTip(Tile a)
        {
            if (!(a is WallTile)) return "";
            WallTile wall = a as WallTile;
            return "";
        }

        /// <summary>
        /// Tooltip for enemies / NPCs
        /// </summary>
        /// <param name="a">An ActorTile (does not display anything for other types of tile)</param>
        private string _EnemyToolTip(Tile a)
        {
            if (!(a is ActorTile)) return "";
            ActorTile actor = a as ActorTile;
            return "Enemy";
        }

        /// <summary>
        /// Tooltip for walls (showing sturdiness)
        /// </summary>
        /// <param name="a">A Walltile (does not display anything for other types of tile)</param>
        private string _WallToolTip(Tile a)
        {
            if (!(a is WallTile)) return "";
            WallTile wall = a as WallTile;
            return "Wall";
        }
        #endregion

        #region Private Methods (Draw tooltip)
        /// <summary>
        /// A generic (text based) tooltip containing text and an (optional) header
        /// </summary>
        /// <param name="Text">Text to display in tooltip (normal font)</param>
        /// <param name="Header">Header to use in tooltip (bold)</param>
        private void GenericToolTip(Rectangle rect, string Text = "", string Header = "")
        {

        }

        /// <summary>
        /// A tooltip containing a progress bar (enemy health, wall sturdiness)
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="Text"></param>
        /// <param name="current"></param>
        /// <param name="max"></param>
        private void _ProgressBarToolTip(Rectangle rect, string Header = "", string Text = "", int current = 0, int max = 100)
        {

        }

        /// <summary>
        /// A tooltip containing data on multiple objects
        /// </summary>
        /// <param name="ToolTipData">A formatted string containing tooltip data (# separates header and text; %x%y% indicates progress of x towards total y)</param>
        private void _MixedToolTip(Rectangle rect, string ToolTipData)
        {

        }
        #endregion
    }

}
