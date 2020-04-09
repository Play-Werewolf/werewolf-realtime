namespace WerewolfServer.Game
{
    public struct Attack
    {
        public Player Attacker {get;set;}
        
        public Player Target {get;set;}
        
        public Power Power{get;set;}

        public string Description {get;set;}
    }

    public struct Defense
    {
        public Player Defender {get;set;}
        
        public Player Target {get;set;}
        
        public Power Power {get;set;}

        public string Description;
    }
}
