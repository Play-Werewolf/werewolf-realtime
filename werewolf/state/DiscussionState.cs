using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    class DiscussionState : TimedState // TODO: Vote to skip
    {
        public override GameState NextGamestate => new NightTransitionState(Game);
        public DiscussionState(GameRoom game) : base(game, 2 * 60 + 30) { }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                Votes = Game.Players
                    .Where(p => p.VotesAgainst != null)    
                    .Select(p => new {Voter=p.Id, Target=p.VotesAgainst.Id}).ToArray()
            };
        }

        public override void OnStart()
        {
            Game.PlayerOnStand = null;
            foreach (var player in Game.Players)
            {
                player.VotesAgainst = null;
                player.Votes = 0;
            }
        }

        public override void OnTimer(float timeDelta)
        {
            base.OnTimer(timeDelta);

            if (Game.PlayerOnStand == null)
                return;

            ChangeState(new TrialState(Game, this));
        }
    }

    public abstract class JudgementState : TimedState
    {
        public GameState BaseState;

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                PlayerOnStand = Game.PlayerOnStand.Id,
            };
        }

        public JudgementState(GameRoom room, float timer, GameState baseState)
            : base(room, timer)
        {
            BaseState = baseState;
        }
    }

    public class TrialState : JudgementState
    {
        public override GameState NextGamestate => new VotingState(Game, BaseState);

        public TrialState(GameRoom game, GameState state) : base(game, 30, state) { }
    }

    public class VotingState : JudgementState
    {
        public override GameState NextGamestate => new VoteShowState(Game, BaseState);

        public VotingState(GameRoom game, GameState state) : base(game, 30, state) { }

        public override void OnStart()
        {
            foreach (var player in Game.Players)
            {
                player.TrialVote = null;
            }
        }
    }

    public class VoteShowState : JudgementState
    {
        public override GameState NextGamestate
        {
            get
            {
                if (Game.Players.Count(p => p.TrialVote == true) > Game.Players.Count(p => p.TrialVote == false))
                {
                    return new ExecutionState(Game);
                }

                return BaseState; // Go back to discussion
            }
        }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                PlayerOnStand = Game.PlayerOnStand.Id,
                Votes = Game.Players.Select(p => new { p.Id, p.TrialVote }).ToArray()
            };
        }

        public VoteShowState(GameRoom game, GameState state) : base(game, 10, state) { }
    }
}
