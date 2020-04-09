using System;

namespace WerewolfServer.Game
{
    public class Werewolf : Character
    {
        public override Alignment Alignment => Alignment.Evil;

        public Werewolf(string name) : base(name) {}

        public override void SetAction(NightAction action)
        {
            // TODO: Notify werewolves about ww vote...
        }
        
        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddAttack(new Attack
            {
                Attacker=this,
                Target=Night.Action.FirstTarget,
                Power=Power.Basic,
                Description="attacked by a werewolf",
            });
        }
    }
}
