using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Frontend.UI
{
    class CharacterWindow : Window
    {
        private Actor _actor;
        private TextInput _name;
        private Grid _abilities;
        private NumberEntry _evade;
        private NumberEntry _block = null;
        private NumberEntry _penetrate = null;
        private NumberEntry _healthReg = null;
        private NumberEntry _stealHealth = null;
        private NumberEntry _stealMana = null;
        private NumberEntry _fireDamage = null;
        private NumberEntry _iceDamage = null;
        private NumberEntry _fireDefense = null;
        private NumberEntry _iceDefense = null;
        private NumberEntry _destroyWeapon = null;
        private NumberEntry _destroyArmor = null;
        private NumberEntry _maxMana = null;
        private NumberEntry _manaReg = null;
        private NumberEntry _gold = null;
        private NumberEntry _level = null;
        private NumberEntry _damage = null;
        private NumberEntry _resist = null;
        private NumberEntry _exp = null;
        private NumberEntry _expNeeded = null;
        private NumberEntry _maxhealth = null;
        private NumberEntry _health = null;
        private NumberEntry _mana = null;
        private NumberEntry _armor = null;
        private NumberEntry _abilityPoints = null;
        private NumberEntry _skills = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterWindow(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
    }
}
