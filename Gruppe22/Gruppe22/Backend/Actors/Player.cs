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
        #region private fields
        /// <summary>
        /// A list of all quests the player get from NPC.
        /// </summary>
        private List<Quest> _quests_obtained_from_NPC;
        #endregion

        #region contructors
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
        public Player( int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(ActorType.Player, health, armour, damage, maxHealth, name)
        {
            this._quests_obtained_from_NPC = new List<Quest>();
            _actorType = ActorType.Player;
            _viewRange = 4;
            _animationFile = ".\\content\\player.xml";
        }
        #endregion

        #region public methods
        /// <summary>
        /// Count of quests obtained from NPC
        /// </summary>
        public int QuestsCount { get { return this._quests_obtained_from_NPC.Count; } }

        /// <summary>
        /// This method update the status of all player quests and realise the rewards
        /// </summary>
        public void UpdateQuests()
        {
            foreach (Quest quest in _quests_obtained_from_NPC)
            {
                if (quest.IsDone) continue;
                switch (quest.GetQuestType())
                {
                    case Quest.QuestType.CollectItems:
                        Quest.Reward reward = quest.TestTheGoal(_inventory.Count);
                        _gold += reward.RewardXP;
                        break;
                    case Quest.QuestType.KillEnemys:
                        break;
                }
            }
        }

        /// <summary>
        /// return the liste of all quests to be displayed by Dialogsystem
        /// </summary>
        /// <returns></returns>
        public List<Quest> GetQuests() { return _quests_obtained_from_NPC; }

        /// <summary>
        /// Add quest to the player. This shoud be used from NPC(dialog).
        /// </summary>
        /// <param name="q">The quest.</param>
        public void AddQuest(Quest q)
        {
            this._quests_obtained_from_NPC.Add(q);
        }
        #endregion
    }
}
