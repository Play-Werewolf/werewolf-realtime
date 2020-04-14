using System;
using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public struct Message
    {
        public string Content { get; set; }

        public Message(string content)
        {
            Content = content;
        }

        public static implicit operator Message(string s)
        {
            return new Message(s);
        }
    }

    public class NightAction
    {
        public static NightAction Empty => new EmptyAction();
        public virtual bool ShouldAct => false;
        public virtual Player FirstTarget => null;
        public virtual Player SecondTarget => null;
    }

    public class EmptyAction : NightAction { }

    public class BooleanAction : NightAction
    {
        bool shouldAct;
        public BooleanAction(bool shouldAct)
        {
            this.shouldAct = shouldAct;
        }

        public override bool ShouldAct => shouldAct;
    }

    public class UnaryAction : NightAction
    {
        Player target;
        public UnaryAction(Player player)
        {
            target = player;
        }

        public override bool ShouldAct => target != null;
        public override Player FirstTarget => target;
        public override Player SecondTarget => null;
    }

    public class BinaryAction : NightAction
    {
        Player target1, target2;
        public BinaryAction(Player player1, Player player2)
        {
            target1 = player1;
            target2 = player2;
        }

        public override bool ShouldAct => target1 != null && target2 != null;
        public override Player FirstTarget => target1;
        public override Player SecondTarget => target2;
    }

    public class Night
    {
        public List<Attack> Attacks { get; set; } = new List<Attack>();
        public List<Defense> Defenses { get; set; } = new List<Defense>();
        public NightAction Action { get; set; } = NightAction.Empty;
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<Player> VisitedBy { get; set; } = new List<Player>();

        private Character _character;
        private Player player => _character.Player; // TODO: That's not a good way to do that

        public bool HasPlayed => !(Action is EmptyAction);

        public Night(Character character)
        {
            this._character = character;
            Reset();
        }

        public void Reset()
        {
            Attacks.Clear();
            Defenses.Clear();
            VisitedBy.Clear();
            Messages.Clear();
            Action = NightAction.Empty;
        }

        public void SendMessage(Message message)
        {
            Messages.Add(message);
        }

        public void AddAttack(Attack attack, bool addVisit = false)
        {
            Attacks.Add(attack);
            if (addVisit)
                AddVisit(attack.Attacker);
        }

        public void AddDefense(Defense defense, bool addVisit = false)
        {
            Defenses.Add(defense);
            if (addVisit)
                AddVisit(defense.Defender);
        }

        public void AddVisit(Player player)
        {
            VisitedBy.Add(player);
        }

        public void CalculateResults()
        {
            if (!player.IsOnline)
            {
                player.Character.Night.AddAttack(new Attack
                {
                    Attacker = null,
                    Target = player.Character,
                    Power = Power.Unstoppable,
                    Description = "have commited suicide",
                });
            }

            Power attack = Attacks.DefaultIfEmpty().Max(a => a.Power);

            if (attack <= player.Character.BaseDefense)
            {
                return;
            }

            foreach (var d in Defenses)
            {
                d.Defender?.Character.OnDefense(player);
            }

            Power defense = Defenses.DefaultIfEmpty().Max(d => d.Power);

            if (attack > defense)
            {
                foreach (var a in Attacks)
                    player.Character.SendMessage("You " + a.Description);

                player.Character.Die();

                foreach (var a in Attacks)
                    a.Attacker?.Character.OnAttackSuccess(player);
                foreach (var d in Defenses)
                    d.Defender?.Character.OnDefenseFailed(player);
            }
            else
            {
                foreach (var d in Defenses)
                    player.Character.SendMessage("You were attacked but " + d.Description);

                foreach (var a in Attacks)
                    a.Attacker?.Character.OnAttackFailed(player);
                foreach (var d in Defenses)
                    d.Defender?.Character.OnDefenseSuccess(player);
            }
        }
    }
}
