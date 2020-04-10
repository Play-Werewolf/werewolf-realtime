using System;

namespace WerewolfServer.Game
{
    public class Werewolf : SingleTargetCharacter
    {
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Evil;
        public override Alignment Alignment => Alignment.Evil;
        
        private void PromoteToAlpha()
        {
            SendMessage("You have been promoted to Alpha Werewolf");
            Player.ChangeRole(new AlphaWerewolf());
            // This is the end of life of this object. Do not continue modifying it.
        }

        public override void OnNightStart()
        {
            Console.WriteLine("Werewolf onBeforeNight");
            if (Player.Game.GetAlphaWerewolf() == null)
            {
                var wws = Player.Game.GetWerewolves();
                if (wws.Count == 0)
                    return;

                var ww = wws[Player.Game.Random.Next(wws.Count)];
                ww.PromoteToAlpha();
            }
        }

        public override void DoAction()
        {
            var alpha = Player.Game.GetAlphaWerewolf();
            if (alpha.Attacker != Player)
                return;

            Night.Action.FirstTarget.Character.Night.AddAttack(new Attack
            {
                Attacker = this,
                Target = Night.Action.FirstTarget,
                Power = Power.Basic,
                Description = "attacked by a werewolf",
            }, addVisit: true);
        }
    }
}
