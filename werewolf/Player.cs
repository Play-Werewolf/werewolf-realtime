namespace WerewolfServer.Game
{
    public class Player
    {
        public Character Character;
        public Player(Character player)
        {
            Character = player;
            Character.Player = this; // One to one relationship between the player and the character
        }

        public static implicit operator Character(Player player)
        {
            return player.Character;
        }

        public static implicit operator Player(Character player)
        {
            return player.Player;
        }
    }

    public class Character
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

        public virtual void OnBeforeNight() {}
        public virtual void OnNightStart() {}
        public virtual void SetAction(NightAction action)
        {
            this.Night.Action = action;
        }
        public virtual void PreAction() {}
        public virtual bool ShouldAct()
        {
            return Night.Action != null && Night.Action.ShouldAct;
        }
        public virtual void DoAction() {}
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
