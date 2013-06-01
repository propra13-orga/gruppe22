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
            _name = new TextInput(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Name:", _actor.name, "This is the name used by the character.");
            _block = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Block:", _actor.block, "Blocking prevents damage, unless the attacks penetration rating is higher");
            _evade = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Evade:", _actor.evade, "Evasion determines whether an attack hits (i.e. attackers and defenders values are compared).");
            _penetrate = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Penetrate:", _actor.penetrate, "Penetration determines the chance to cut through a block.");
            _healthReg = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Regeneration:", _actor.healthReg, "This determines how long it takes for health to regenerate.");
            _stealHealth = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Health Drain:", _actor.stealHealth, "This determines how much a successful attack will steal.");
            _stealMana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Mana Drain:", _actor.stealMana, "This determines how much mana a successful attack will steal.");
            _fireDamage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Fire Damage:", _actor.fireDamage, "This determines how much fire damage a successful attack does.");
            _iceDamage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Ice Damage:", _actor.iceDamage, "This determines how much ice damage a successful attack does.");
            _fireDefense = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Fire Defense:", _actor.fireDefense, "This determines resistance against a successful fire attack.");
            _iceDefense = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Ice Defense:", _actor.iceDefense, "This determines resistance against a successful ice attack.");
            _destroyWeapon = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Destroy Weapon:", _actor.destroyWeapon, "This determines the chance to destroy an opponents' weapon.");
            _destroyArmor = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Destroy Armor:", _actor.destroyArmor, "This determines the chance to destroy an opponents' armor.");
            _maxMana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Max. Mana:", _actor.maxMana, "This determines the maximum mana available .");
            _manaReg = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Mana Regeneration:", _actor.manaReg, "This determines how long it takes for mana to regenate.");
            _gold = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Gold:", _actor.gold, "This is the current amount of gold available to you.");
            _damage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Damage:", _actor.damage, "This determines physical damage done to an opponent");
            _resist = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Resistance:", _actor.resist, "This determines resistance against adverse effects.");
            _exp = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Experience:", _actor.exp, "This determines your current amount of experience.");
            _expNeeded = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Block:", _actor.expNeeded, "This is the amount of experience needed for the next level.");
            _maxhealth = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Max. Health:", _actor.maxhealth, "This is the maximum amount of health achievable by healing.");
            _health = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Health:", _actor.health, "This is your current health (reduced when taking damage).");
            _mana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Mana:", _actor.mana, "Mana is used for casting spells.");
            _armor = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Armor:", _actor.armor, "Armor is used to reduce damage.");
            _abilityPoints = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Abilitypoints:", _actor.abilityPoints, "Abilitypoints are to learn new abilities or spells.");
            _skills = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Skillpoints:", _actor.block, "Skillpoints are used used to improve your statistics.");
            _level = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Top + 5, _displayRect.Left + 5, _displayRect.Width - 10, 40), "Level:", _actor.level, "Your level determines your general character state.");
        }
    }
}
