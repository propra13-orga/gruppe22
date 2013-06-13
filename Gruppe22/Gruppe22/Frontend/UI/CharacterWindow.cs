using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    class CharacterWindow : Window
    {
        private uint _page = 0;
        private Actor _actor;
        private TextInput _name;
        private Abilities _abilities;
        private Inventory _inventory;
        private AbilityChoice _abilitychoice;
        private int _selected = -1;
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
        protected SpriteFont _font = null;

        public uint page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
                switch (_page)
                {
                    case 0:
                        foreach (UIElement child in _children)
                        {
                            if ((child is NumberEntry) || (child is TextInput))
                                child.Show();
                            else
                                child.Hide();
                        }
                        _skills.Hide();
                        break;
                    case 2:
                        foreach (UIElement child in _children)
                        {
                            if ((child is NumberEntry) || (child is TextInput)) child.Hide();
                        }
                        _inventory.Hide();
                        _abilities.Show();
                        _skills.Show();
                        _level.Show();
                        if (_skills.value > 0)
                        {
                            _abilitychoice.Show();
                        }
                        else
                        {
                            _abilitychoice.Hide();
                        }
                        break;
                    case 1:
                        foreach (UIElement child in _children)
                        {
                            if ((child is NumberEntry) || (child is TextInput)) child.Hide();
                        }
                        _inventory.Show();
                        _abilities.Hide();
                        _abilitychoice.Hide();

                        break;
                }
            }
        }
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            switch (eventID)
            {
                case Events.Settings:
                    if ((int)data[0] > 0)
                        _abilityPoints.value -= 1;
                    else _abilityPoints.value += 1;
                    foreach (UIElement element in _children)
                    {
                        if ((element is NumberEntry) && (element != _abilityPoints) && (element != _skills))
                        {
                            if ((((NumberEntry)element).value > ((NumberEntry)element).originalValue))
                            {
                                ((NumberEntry)element).allowDecrease = true;
                            }
                            else
                            {
                                if (((NumberEntry)element).value < ((NumberEntry)element).originalValue)
                                {
                                    ((NumberEntry)element).value = ((NumberEntry)element).originalValue;
                                }
                                ((NumberEntry)element).allowDecrease = false;
                            }
                        }
                    }
                    _RefreshSkills();
                    break;
                case Events.ButtonPressed:
                    switch ((Buttons)data[0])
                    {
                        case Buttons.Close:
                            _actor.skills = _skills.value;
                            _actor.evade = _evade.value;
                            _actor.name = _name.text;
                            _actor.block = _block.value;
                            _actor.penetrate = _penetrate.value;
                            _actor.healthReg = _healthReg.value;
                            _actor.stealHealth = _stealHealth.value;
                            _actor.stealMana = _stealMana.value;
                            _actor.fireDamage = _fireDamage.value;
                            _actor.iceDamage = _iceDamage.value;
                            _actor.fireDefense = _fireDefense.value;
                            _actor.iceDefense = _iceDefense.value;
                            _actor.destroyWeapon = _destroyWeapon.value;
                            _actor.destroyArmor = _destroyArmor.value;
                            _actor.maxMana = _maxMana.value;
                            _actor.manaReg = _manaReg.value;
                            _actor.damage = _damage.value;
                            _actor.resist = _resist.value;
                            _actor.maxHealth = _maxhealth.value;
                            _actor.armor = _armor.value;
                            _parent.HandleEvent(false, Events.ContinueGame, null);
                            break;
                    }
                    break;

                default:
                    base.HandleEvent(DownStream, eventID, data);
                    break;

            };
        }


        private void _RefreshSkills()
        {

            if (_abilityPoints.value > 0)
            {
                _evade.allowIncrease = true;
                _block.allowIncrease = true;
                _penetrate.allowIncrease = true;
                _healthReg.allowIncrease = true;
                _stealHealth.allowIncrease = true;
                _stealMana.allowIncrease = true;
                _fireDamage.allowIncrease = true;
                _iceDamage.allowIncrease = true;
                _fireDefense.allowIncrease = true;
                _iceDefense.allowIncrease = true;
                _destroyWeapon.allowIncrease = true;
                _destroyArmor.allowIncrease = true;
                _maxMana.allowIncrease = true;
                _manaReg.allowIncrease = true;
                _damage.allowIncrease = true;
                _resist.allowIncrease = true;
                _maxhealth.allowIncrease = true;
                _armor.allowIncrease = true;
            }
            else
            {
                _evade.allowIncrease = false;
                _block.allowIncrease = false;
                _penetrate.allowIncrease = false;
                _healthReg.allowIncrease = false;
                _stealHealth.allowIncrease = false;
                _stealMana.allowIncrease = false;
                _fireDamage.allowIncrease = false;
                _iceDamage.allowIncrease = false;
                _fireDefense.allowIncrease = false;
                _iceDefense.allowIncrease = false;
                _destroyWeapon.allowIncrease = false;
                _destroyArmor.allowIncrease = false;
                _maxMana.allowIncrease = false;
                _manaReg.allowIncrease = false;
                _damage.allowIncrease = false;
                _resist.allowIncrease = false;
                _maxhealth.allowIncrease = false;
                _armor.allowIncrease = false;
            }
        }

        public override bool OnMouseDown(int button)
        {
            Point pos = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            if (new Rectangle(_displayRect.Left, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _parent.HandleEvent(false, Events.ShowCharacter);
                return true;
            }
            if (new Rectangle(_displayRect.Left + 135, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _parent.HandleEvent(false, Events.ShowInventory);
                return true;
            }
            if (new Rectangle(_displayRect.Left + 270, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _parent.HandleEvent(false, Events.ShowAbilities);
                return true;
            }
            return base.OnMouseDown(button);
        }


        private void _UpdateSelection()
        {
            Point pos = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            _selected = -1;
            if (new Rectangle(_displayRect.Left, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _selected = 0;
            }
            if (new Rectangle(_displayRect.Left + 135, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _selected = 1;
            }
            if (new Rectangle(_displayRect.Left + 270, _displayRect.Top - 37, 130, 37).Contains(pos))
            {
                _selected = 2;
            }
        }

        private void _drawTab(int pos, string text)
        {
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left - 1 + 135 * pos, _displayRect.Top - 34 - ((_page == pos) ? 4 : 0), 130, 32 + ((_page == pos) ? 5 : 0)), new Rectangle(39, 6, 1, 1), (_page == pos) ? Color.White : Color.Gray);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 135 * pos, _displayRect.Top - 33 - ((_page == pos) ? 4 : 0), 128, 32 + ((_page == pos) ? 6 : 0)), new Rectangle(39, 6, 1, 1), (_selected == pos) ? Color.LightBlue : Color.Black);
            int center = (int)((128 - _font.MeasureString(text).X * 0.8f) / 2f);
            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left - 1 + 135 * pos + center, _displayRect.Top - 31 - ((_page == pos) ? 5 : 0)), (_page == pos) ? Color.White : Color.Gray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
        }



        public override void Draw(GameTime gameTime)
        {
            _UpdateSelection();
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X - 1, _displayRect.Y - 1, _displayRect.Width + 2, _displayRect.Height + 2), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.Black);
            _drawTab(0, "Character" + ((_actor.abilityPoints > 0) ? (" (" + _actor.abilityPoints.ToString() + ")") : ""));
            _drawTab(1, "Inventory" + ((_actor.newItems > 0) ? (" (" + _actor.newItems.ToString() + ")") : ""));
            _drawTab(2, "Skills" + ((_actor.skills > 0) ? (" (" + _actor.skills.ToString() + ")") : ""));
            _spriteBatch.End();

            foreach (UIElement child in _children)
            {
                child.Draw(gameTime);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterWindow(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
            _name = new TextInput(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 15, 25), "Name:", _actor.name, "This is the name used by the character.", 20, true);

            _block = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 35, (_displayRect.Width - 10) / 2 - 10, 25), "Block:", _actor.block, "Blocking prevents damage, unless the attacks penetration rating is higher", 2, false);
            _evade = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 35, (_displayRect.Width - 10) / 2 - 10, 25), "Evade:", _actor.evade, "Evasion determines whether an attack hits (i.e. attackers and defenders values are compared).", 2, false);

            _penetrate = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 65, (_displayRect.Width - 10) / 2 - 10, 25), "Penetrate:", _actor.penetrate, "Penetration determines the chance to cut through a block.", 2, false);
            _healthReg = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
      + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 65, (_displayRect.Width - 10) / 2 - 10, 25), "Regeneration:", _actor.healthReg, "This determines how long it takes for health to regenerate.", 2, false);

            _stealHealth = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 95, (_displayRect.Width - 10) / 2 - 10, 25), "Health Drain:", _actor.stealHealth, "This determines how much a successful attack will steal.", 2, false);
            _stealMana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 95, (_displayRect.Width - 10) / 2 - 10, 25), "Mana Drain:", _actor.stealMana, "This determines how much mana a successful attack will steal.", 2, false);

            _fireDamage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 125, (_displayRect.Width - 10) / 2 - 10, 25), "Fire Damage:", _actor.fireDamage, "This determines how much fire damage a successful attack does.", 2, false);
            _iceDamage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 125, (_displayRect.Width - 10) / 2 - 10, 25), "Ice Damage:", _actor.iceDamage, "This determines how much ice damage a successful attack does.", 2, false);

            _fireDefense = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 155, (_displayRect.Width - 10) / 2 - 10, 25), "Fire Defense:", _actor.fireDefense, "This determines resistance against a successful fire attack.", 2, false);
            _iceDefense = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 155, (_displayRect.Width - 10) / 2 - 10, 25), "Ice Defense:", _actor.iceDefense, "This determines resistance against a successful ice attack.", 2, false);

            _destroyWeapon = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 185, (_displayRect.Width - 10) / 2 - 10, 25), "Destroy Weapon:", _actor.destroyWeapon, "This determines the chance to destroy an opponents' weapon.", 2, false);
            _destroyArmor = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 185, (_displayRect.Width - 10) / 2 - 10, 25), "Destroy Armor:", _actor.destroyArmor, "This determines the chance to destroy an opponents' armor.", 2, false);

            _maxMana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 215, (_displayRect.Width - 10) / 2 - 10, 25), "Max. Mana:", _actor.maxMana, "This determines the maximum mana available .", 2, false);
            _manaReg = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 215, (_displayRect.Width - 10) / 2 - 10, 25), "Mana Regeneration:", _actor.manaReg, "This determines how long it takes for mana to regenate.", 2, false);

            _gold = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 245, (_displayRect.Width - 10) / 2 - 10, 25), "Gold:", _actor.gold, "This is the current amount of gold available to you.", 2, false);
            _damage = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 245, (_displayRect.Width - 10) / 2 - 10, 25), "Damage:", _actor.damage, "This determines physical damage done to an opponent", 2, false);

            _resist = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 275, (_displayRect.Width - 10) / 2 - 10, 25), "Resistance:", _actor.resist, "This determines resistance against adverse effects.", 2, false);
            _exp = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 275, (_displayRect.Width - 10) / 2 - 10, 25), "Experience:", _actor.exp, "This determines your current amount of experience.", 2, false);

            _expNeeded = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 305, (_displayRect.Width - 10) / 2 - 10, 25), "Block:", _actor.expNeeded, "This is the amount of experience needed for the next level.", 2, false);
            _maxhealth = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 305, (_displayRect.Width - 10) / 2 - 10, 25), "Max. Health:", _actor.maxhealth, "This is the maximum amount of health achievable by healing.", 2, false);

            _health = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 335, (_displayRect.Width - 10) / 2 - 10, 25), "Health:", _actor.health, "This is your current health (reduced when taking damage).", 2, false);
            _mana = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 335, (_displayRect.Width - 10) / 2 - 10, 25), "Mana:", _actor.mana, "Mana is used for casting spells.", 2, false);

            _armor = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 365, (_displayRect.Width - 10) / 2 - 10, 25), "Armor:", _actor.armor, "Armor is used to reduce damage.", 2, false);
            _abilityPoints = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 365, (_displayRect.Width - 10) / 2 - 10, 25), "Abilitypoints:", _actor.abilityPoints, "Abilitypoints are used to improve your statistics.", 2, false);

            _skills = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 395, (_displayRect.Width - 10) / 2 - 10, 25), "New Skills:", _actor.skills, "Skills represent abilities and spells used in combat.", 2, false);
            _level = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left
                + (_displayRect.Width - 10) / 2 + 10, _displayRect.Top + 395, (_displayRect.Width - 10) / 2 - 10, 25), "Level:", _actor.level, "Your level determines your general character state.", 2, false);


            _abilities = new Abilities(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 10, _displayRect.Height - 260), _actor);
            _abilitychoice = new AbilityChoice(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Bottom - 240, _displayRect.Width - 10, 150), _actor);

            _inventory = new Inventory(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 10, _displayRect.Height - 40), _actor);
            _children.Add(_name);
            _children.Add(_block);
            _children.Add(_evade);
            _children.Add(_penetrate);
            _children.Add(_healthReg);
            _children.Add(_stealHealth);
            _children.Add(_stealMana);
            _children.Add(_fireDamage);
            _children.Add(_iceDamage);
            _children.Add(_fireDefense);
            _children.Add(_iceDefense);
            _children.Add(_destroyWeapon);
            _children.Add(_destroyArmor);
            _children.Add(_maxMana);
            _children.Add(_mana);
            _children.Add(_manaReg);
            _children.Add(_gold);
            _children.Add(_damage);
            _children.Add(_resist);
            _children.Add(_exp);
            _children.Add(_expNeeded);
            _children.Add(_maxhealth);
            _children.Add(_health);
            _children.Add(_armor);
            _children.Add(_abilityPoints);
            _children.Add(_skills);
            _children.Add(_level);
            _children.Add(_inventory);
            _children.Add(_abilities);
            _inventory.Update();
            _children.Add(_abilitychoice);
            _font = _content.Load<SpriteFont>("font");
            _children.Add(new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + (_displayRect.Width - 100) / 2, _displayRect.Top + _displayRect.Height - 45, 100, 30), "Ok", (int)Buttons.Close, false));
            _focusID = _children.Count - 1;
            page = 0;
            ChangeFocus();
            _RefreshSkills();
        }
    }
}
