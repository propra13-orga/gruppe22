using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class Quest
    {
        private int _rewardXP;
        private List<Item> _rewardItem;
        private string _description;
        private bool _done;

        public bool IsDone { get { return _done; } }

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
