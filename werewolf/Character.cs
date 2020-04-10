using System.Text;

namespace WerewolfServer.Game
{
    public abstract class Character
    {
        public Player Player {get;set;}
        public static readonly int NotDead = -1;
        
        public int DeathNight {get;set;} = NotDead;
        
        public bool Alive => DeathNight == NotDead;
        

        public Night Night {get;set;}

        public virtual Power BaseDefense => Power.None;

        public abstract Alignment Alignment {get;}
        public abstract FortuneTellerResult FortuneTellerResult {get;}

        public Character()
        {
            Night = new Night(this);
        }

    #if(DEBUG)
        public string _name;
        public Character WithName(string name)
        {
            _name = name;
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", GetType().Name, _name, Alive ? "alive" : "dead");
        }
    #else
        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Alive ? "alive" : "dead");
        }
    #endif

        public void SendMessage(Message message)
        {
            Night.SendMessage(message);
        }

        public void Visit(Character other)
        {
            other.Night.AddVisit(this); 
        }

        public virtual void Die()
        {
            DeathNight = Player.Game.CurrentNight;
            SendMessage("You have died!");
        }

        // Called just before the night is reset and starts
        public virtual void OnBeforeNight() {}

        // Called jsut after the night is reset and started
        public virtual void OnNightStart() {}

        // Called when the user asks for an action (day or night)
        public void SetAction(NightAction action)
        {
            this.Night.Action = action;
        }

        // If this function returns true, then PreAction, DoAction and PostAction are called
        // (function is called separately 3 times to check for each of the ditto functions)
        public virtual bool ShouldAct()
        {
            return Night.Action != null && Night.Action.ShouldAct;
        }

        // Called before the action, if ShouldAct() returns true.
        // Prepare stuff here (role blockings, redirects, etc...)
        public virtual void PreAction() {}
        
        // Perform the main action of the character.
        // Visit other players here
        public virtual void DoAction() {}

        // Respond to visits here (e.g. veteran)
        // Refrain from making visits here
        public virtual void PostAction() {}
        public virtual void OnDeath() {}
        public virtual void OnNightEnd() {}

        public virtual void OnAttackSuccess(Character p) {}
        public virtual void OnAttackFailed(Character player) {}

        // OnDefense will be called before attack and defense consequences take place.
        // Useful, for example, for bodyguard (adding attack to all attackers of target
        // before the attacks are calculated)
        public virtual void OnDefense(Character p) {}
        public virtual void OnDefenseSuccess(Character p) {}
        public virtual void OnDefenseFailed(Character p) {}

        public virtual string FormatDeathMessage()
        {
            var attacks = Night.Attacks;
            var sb = new StringBuilder("They were apparently ");
            for (int i = 0; i < attacks.Count; i++)
            {
                if (i != 0)
                {
                    sb.Append(". They were also ");
                }

                sb.Append(attacks[i].Description);
            }

            sb.Append(".");
            return sb.ToString();
        }
    }
}
