namespace Scripts.Network
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
}
