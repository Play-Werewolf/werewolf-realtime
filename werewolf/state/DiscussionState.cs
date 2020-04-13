using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    class DiscussionState : TimedState
    {
        public override GameState NextGamestate => new NightTransitionState(Game);
        public DiscussionState(GameRoom game) : base(game, 2 * 60 + 30) { }

        Player GetVotedupPlayer()
        {
            Dictionary<Player, int> votes = new Dictionary<Player, int>();
            foreach (var player in Game.Players)
            {
                if (player.VoteAgainst == null)
                    continue;

                if (votes.ContainsKey(player.VoteAgainst))
                {
                    votes[player.VoteAgainst]++;
                }
                else
                {
                    votes[player.VoteAgainst] = 1;
                }
            }

            if (votes.Count == 0)
                return null;

            var kvp = votes.OrderBy(kvp => kvp.Value).FirstOrDefault();
            if (kvp.Value > Game.Players.Count / 2)
                return kvp.Key;

            return null;
        }

        public override void OnStart()
        {
            foreach (var player in Game.Players)
            {
                player.VoteAgainst = null;
            }
        }

        public override void OnTimer(float timeDelta)
        {
            base.OnTimer(timeDelta);

            var player = GetVotedupPlayer();
            if (player == null)
                return;

            ChangeState(new TrialState(Game, this));
        }
    }

    public abstract class CircularState : TimedState
    {
        public GameState BaseState;
        public CircularState(GameRoom room, float timer, GameState baseState)
            : base(room, timer)
        {
            BaseState = baseState;
        }
    }

    public class TrialState : CircularState
    {
        public override GameState NextGamestate => new VotingState(Game, BaseState);

        public TrialState(GameRoom game, GameState state) : base(game, 30, state) { }
    }

    public class VotingState : CircularState
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

    public class VoteShowState : CircularState
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

        public VoteShowState(GameRoom game, GameState state) : base(game, 30, state) { }
    }
}
