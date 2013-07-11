using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A class to construct a quest in the game.
    /// </summary>
    public class Quest
    {
        private int _rewardXP;
        private List<Item> _rewardItem;
        private string _description;
        private bool _done;

        /// <summary>
        /// True if the quest is completed.
        /// </summary>
        public bool IsDone { get { return _done; } }

        /// <summary>
        /// The constructor for a quest
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xp">The amount of exp granted after completing the quest.</param>
        /// <param name="itemlist">A list of items the player will be rewarded with for completing the quest</param>
        public Quest(string text, int xp, List<Item> itemlist)
        {
            this._done = false;
            this._description = text;
            this._rewardItem = itemlist;
            this._rewardXP = xp;
        }

        public virtual void Goal()
        {
            this._done = true;
        }
    }
}
