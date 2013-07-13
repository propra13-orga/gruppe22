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

#region public fields
        /// <summary>
        /// The reward stucture to return reward to player.
        /// </summary>
        public struct Reward
        {
            private int _xp;
            private List<Item> _items;
            public int RewardXP { get{ return _xp;} }
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

#region private fields
        private int _rewardXP;
        private List<Item> _rewardItem;
        private string _description;
        private int _goal;
        private bool _done;
        private QuestType _qtype;
#endregion

#region Constructors
        /// <summary>
        /// The constructor for a quest
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xp">The amount of exp granted after completing the quest.</param>
        /// <param name="itemlist">A list of items the player will be rewarded with for completing the quest</param>
        public Quest(QuestType q, string text, int xp, List<Item> itemlist, int goal)
        {
            this._qtype = q;
            this._done = false;
            this._description = text;
            this._rewardItem = itemlist;
            this._rewardXP = xp;
            this._goal = goal;
        }

        /// <summary>
        /// Constructor for quests without item-reward.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="xp"></param>
        /// <param name="goal"></param>
        public Quest(QuestType q,string text, int xp, int goal)
        {
            this._qtype = q;
            this._done = false;
            this._description = text;
            this._rewardXP = xp;
            this._rewardItem = null;
            this._goal = goal;
        }

        /// <summary>
        /// Constructor for quests without numeric-reward.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="itemlist"></param>
        /// <param name="goal"></param>
        public Quest(QuestType q, string text, List<Item> itemlist, int goal)
        {
            this._qtype = q;
            this._done = false;
            this._description = text;
            this._rewardXP = 0;
            this._rewardItem = itemlist;
            this._goal = goal;
        }
# endregion

#region public methods
        /// <summary>
        /// True if the quest is completed.
        /// </summary>
        public bool IsDone { get { return _done; } }

        /// <summary>
        /// return the questtype so that the player can chose the meaning of goal.
        /// </summary>
        /// <returns></returns>
        public QuestType GetQuestType() { return this._qtype; }

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

    }
}
