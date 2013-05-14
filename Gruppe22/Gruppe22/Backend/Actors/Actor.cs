using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public enum ActorType
    {
        Player = 0,
        NPC = 1,
        Enemy = 2
    }

    public class Actor
    {
        private ActorTile _tile;
        protected  ActorType _actorType;

        //Lebenspunkte, Rüstung, Schaden/Angriffsstärke
        protected int _health, _armour, _damage;

        public ActorTile tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public ActorType actorType
        {
            get
            {
                return _actorType;
            }
        }

        #region Public-Methods

        public int health { get { return _health; } }
        public int armour { get { return _armour; } }
        public int damage { get { return _damage; } }

        public bool IsDead() { return _health <= 0 ? true : false; }

        public void SetDamage(Actor actor) { SetDamage(actor.damage); }
        /// <summary>
        /// Schaden nach einem Angriff für diesen Actor setzen.
        /// Erst Rüstung, dann Lebenspunkte;
        /// </summary>
        /// <param name="damage"></param>
        public void SetDamage(int damage)
        {
            int tmp = _armour - damage;
            if (tmp >= 0)
                _armour = tmp;
            else
            {
                _armour = 0;
                _health = (_health - tmp > 0) ? (_health - tmp) : 0;
            }

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actorType"></param>
        /// <param name="health"></param>
        /// <param name="armour"></param>
        /// <param name="damage"></param>
        public Actor(ActorType actorType, int health, int armour, int damage)
        {
            this._actorType = actorType;
            this._health = health;
            this._armour = armour;
            this._damage = damage;
        }

        #endregion

    }
}
