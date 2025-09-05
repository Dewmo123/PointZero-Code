namespace Server.Utiles
{
    public enum Team
    {
        None,
        Blue,
        Red
    }
    public enum Area
    {
        None,
        A,
        B
    }
    public enum DoorStatus
    {
        Open, Close
    }
    public enum ObjectType
    {
        None,
        BombArea,
        PlantArea,
        Door,
        BreakableWall,
        Player
    }
    public enum RoomState
    {
        Lobby,
        Prepare,
        InGame,
        Between,
        Bomb,
        GameEnd
    }
    public static class TeamEnumHelper
    {
        public static Team GetNegate(Team team)
        {
            switch (team)
            {
                case Team.None:
                    return 0;
                case Team.Blue:
                    return Team.Red;
                case Team.Red:
                    return Team.Blue;
            }
            return Team.None;
        }
    }
}
