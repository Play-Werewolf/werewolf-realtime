namespace WerewolfServer.Game
{
    public abstract class Character
    {
        public GameRoom Game {get;set;}
        public Player Player {get;set;}
        public static readonly int NotDead = -1;
        
        public int DeathNight {get;set;} = NotDead;
        
        public bool Alive => DeathNight == NotDead;
        
        public string Name {get;set;}

        public virtual Alignment Alignment => Alignment.Good;

        public Night Night {get;set;}

        public virtual Power BaseDefense => Power.None;


        public abstract FortuneTellerResult FortuneTellerResult {get;}


        public Character(string name)
        {
            Name = name;
            Night = new Night(this);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", GetType().Name, Name, Alive ? "alive" : "dead");
        }

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
            DeathNight = Game.Night;
            SendMessage("You have died!");
        }

        // Called just before the night is reset and starts
        public virtual void OnBeforeNight() {}

        // Called jsut after the night is reset and started
        public virtual void OnNightStart() {}

        // Called when the user asks for an action (day or night)
        public virtual void SetAction(NightAction action)
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
        // Prepare stuff here
        public virtual void PreAction() {}
        
        // Perform the main action of the character.
        // Visit other players here
        public virtual void DoAction() {}

        // Respond to visits here (e.g. veteran)
        // Refrain from making visits here
        public virtual void PostAction() {}
        public virtual void OnNightEnd() {}

        public virtual void OnAttackSuccess(Character p) {}
        public virtual void OnAttackFailed(Character player) {}

        // OnDefense will be called before attack and defense consequences take place.
        // Useful, for example, for bodyguard (adding attack to all attackers of target
        // before the attacks are calculated)
        public virtual void OnDefense(Character p) {}
        public virtual void OnDefenseSuccess(Character p) {}
        public virtual void OnDefenseFailed(Character p) {}
    }
}