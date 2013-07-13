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

        #region Public Fields
        /// <summary>
        /// The reward stucture to return reward to player.
        /// </summary>
        public struct Reward
        {
            private int _xp;
            private List<Item> _items;
            public int RewardXP { get { return _xp; } }
            public List<Item> RewardItems { get { return _items; } } //unnecessary but whatever
            public Reward(int xp, List<Item> items)
            {
                this._xp = xp;
                this._items = items;
            }
        }

        /// <summary>
        /// enumeration to distinguish the mission
        /// </summary>
        public enum QuestType
        {
            CollectItems,
            KillEnemys
        };

        #endregion

        #region Private Fields
        private int _rewardXP;
        private List<Item> _rewardItem;
        private string _description;
        private int _goal;
        private bool _done;
        private QuestType _qtype;
        #endregion

        #region Public Fields
        /// <summary>
        /// Get Experience Reward
        /// </summary>
        public int xp
        {
            get
            {
                return _rewardXP;
            }
        }

        /// <summary>
        /// Get list of items granted as reward
        /// </summary>
        public Item[] itemList
        {
            get
            {
                return _rewardItem.ToArray();
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        public string text
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Number of items to find / monsters to kill
        /// </summary>
        public int goal
        {
            get
            {
                return _goal;
            }
        }

        /// <summary>
        /// Type of quest (fetch or kill)
        /// </summary>
        public QuestType type
        {
            get
            {
                return _qtype;
            }
        }

        /// <summary>
        /// True if the quest is completed.
        /// </summary>
        public bool done { get { return _done; } }
        #endregion
  

        #region Public Methods

        /// <summary>
        /// Testing if goal is already done.
        /// </summary>
        /// <param name="currentpoints">Abstrakt comparability number.</param>
        public Reward TestTheGoal(int currentpoints)
        {
            if (currentpoints >= _goal)
            {
                this._done = true;
                return new Reward(_rewardXP, _rewardItem);
            }
            else return new Reward(0, null); //empty Reward
        }
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for a quest
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xp">The amount of exp granted after completing the quest.</param>
        /// <param name="itemlist">A list of items the player will be rewarded with for completing the quest</param>
        public Quest(QuestType q = QuestType.CollectItems, string text = "Find an item", int xp = 100, List<Item> itemlist = null, int goal = 1)
        {
            _qtype = q;
            _done = false;
            _description = text;
            if (_rewardItem != null)
            {
                _rewardItem = itemlist;
            }
            else
            {
                _rewardItem = new List<Item>();
            }
            _rewardXP = xp;
            _goal = goal;
        }

        #endregion
    }
}
