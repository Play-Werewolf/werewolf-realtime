namespace WerewolfServer.Network
{
    public abstract class SessionCommand : BaseCommand
    {
        public override bool RequiresSession => false;
        public override void AfterCommand()
        {
            sender.SendSessionStatus();
        }
        public override void OnError(Error error)
        {
            sender.SendAuthenticationError(error.Description);
        }
    }

    public class AuthenticateAnonymous : SessionCommand
    {
        public override string CommandType => "authenticate_anonymous";
        public override int ArgumentsNumber => 1;

        public override bool Validate()
        {
            return sender.Session == null;
        }

        public override void OnCommand()
        {
            var nickname = message.Args[0];
            sender.Login(new NetworkSession(null, new LoginState
            {
                IsAnonymous = true,
                IsAuthenticated = true,
                Nickname = nickname,
                UserID = ""
            }));
        }
    }

    public class Authenticate : SessionCommand
    {
        public override string CommandType => "authenticate";
        public override bool Validate()
        {
            throw new Error("Not implemented");
        }
    }

    public class RestoreSession : SessionCommand
    {
        public override string CommandType => "restore_session";
        public override int ArgumentsNumber => 1;
        public override bool RequiresSession => false;

        public override bool Validate()
        {
            return (sender.Session == null);
        }

        public override void OnCommand()
        {
            var sessionId = message.Args[0];
            var session = sender.Manager.Sessions.GetSession(sessionId);
            
            if (session == null)
            {
                throw new Error("Session not found");
            }

            if (session.Connection != null)
            {
                throw new Error("Session is already bound");
            }

            sender.Login(session);
            sender.SendRoomStatus();
        }
    }
}