using System;

namespace WerewolfServer.Network
{
    public class Error : Exception
    {
        public string Description { get; private set; }
        public string[] Args { get; private set; }

        public Error(string description, params string[] args)
        {
            Description = description;
            Args = args;
        }
    }

    public class BaseCommand
    {
        public virtual int ArgumentsNumber => 0;
        public virtual bool RequiresSession => false;
        public virtual bool Validate() { return true; }
        public virtual string CommandType => "";

        public NetworkConnection sender;
        public NetworkMessage message;

        public virtual void OnCommand() { }
        public virtual void AfterCommand() { }
        public virtual void OnError(Error error) { }

        public BaseCommand Init(NetworkConnection sender, NetworkMessage message)
        {
            this.sender = sender;
            this.message = message;
            return this;
        }

        public void ProcessCommand()
        {
            try
            {
                ExecCmd();
                AfterCommand();
            }
            catch (Error err)
            {
                OnError(err);
            }
        }

        void ExecCmd()
        {
            if (message.Type != CommandType)
                return;

            if (message.Args.Length != ArgumentsNumber)
                throw new Error("error", "Invalid number of arguments");

            if (RequiresSession && sender.Session == null)
                throw new Error("error", "Session required");

            if (!Validate())
                return;

            OnCommand();
        }
    }
}