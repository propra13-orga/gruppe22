using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The class used for players, a type of an actor.
    /// </summary>
    public class Player : Actor
    {
        #region Private Fields
        /// <summary>
        /// A list of current and finished quests
        /// </summary>
        private List<Quest> _quests;
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor for a player.
        /// Sets the default values.
        /// See actor for the params.
        /// </summary>
        /// <param name="health"></param>
        /// <param name="armour"></param>
        /// <param name="damage"></param>
        /// <param name="maxHealth"></param>
        /// <param name="name"></param>
        public Player(int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(ActorType.Player, health, armour, damage, maxHealth, name)
        {
            _quests = new List<Quest>();
            _actorType = ActorType.Player;
            _viewRange = 4;
            _animationFile = ".\\content\\player.xml";
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method update the status of all player quests and grants the rewards
        /// </summary>
        public void UpdateQuests()
        {
            foreach (Quest quest in _quests)
            {
                if (quest.done) continue;
                switch (quest.type)
                {
                    case Quest.QuestType.CollectItems:
                        Quest.Reward reward = quest.TestTheGoal(_inventory.Count);
                        exp += reward.RewardXP;
                        break;
                    case Quest.QuestType.KillEnemys:
                        break;
                }
            }
        }

        /// <summary>
        /// Reset the list of quests
        /// </summary>
        public void ClearQuests()
        {
            _quests.Clear();
        }

        /// <summary>
        /// Remove all finished quests
        /// </summary>
        public void CleanupQuests()
        {
            for (int i = 0; i < quests.Length; ++i)
            {
                if (_quests[i].done)
                {
                    _quests.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// return all quests assigned to the player/// </summary>
        /// <returns></returns>
        public Quest[] quests
        {
            get
            {
                return _quests.ToArray();
            }
        }

        /// <summary>
        /// Add quest to the player. This shoud be used from NPC(dialog).
        /// </summary>
        /// <param name="q">The quest.</param>
        public void AddQuest(Quest q)
        {
            _quests.Add(q);
        }
        #endregion
    }
}
