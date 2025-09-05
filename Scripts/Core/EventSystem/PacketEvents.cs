using Core.EventSystem;
using Scripts.Entities.Players;
using Scripts.Network;
using UnityEngine;

namespace Scripts.Core.EventSystem
{
    public class PacketEvents
    {
        public static readonly HandleEnterRoom HandleEnterRoom = new HandleEnterRoom();
        public static readonly HandleRoomList HandleRoomList= new HandleRoomList();
        public static readonly HandleTimerElapsed HandleTimerElapsed = new HandleTimerElapsed();
        public static readonly HandleUpdateRoomState HandleUpdateRoomState= new HandleUpdateRoomState();
        public static readonly HandlePacketResponse HandlePacketResponse = new();
        public static readonly HandleRoundEnd HandleRoundEnd = new();
        public static readonly HandleGameEnd HandleGameEnd= new();
        public static readonly HandleLeaveRoom HandleLeaveRoom= new();
        public static readonly HandleMyTeam HandleMyTeam= new();
    }
    public class HandleEnterRoom : GameEvent
    {
        public S_RoomEnter packet;
    }
    public class HandleRoomList : GameEvent
    {
        public S_RoomList packet;
    }
    public class HandleTimerElapsed : GameEvent
    {
        public float remainTime;
    }
    public class HandleUpdateRoomState : GameEvent
    {
        public RoomState state;
    }
    public class HandlePacketResponse : GameEvent
    {
        public PacketID packetId;
        public bool success;
    }
    public class HandleRoundEnd : GameEvent
    {
        public bool isEnd;
        public int redCount;
        public int blueCount;
        public Team winner;
    }
    public class HandleGameEnd : GameEvent
    {
        public Team winner;
        public bool isWin;
    }
    public class HandleLeaveRoom : GameEvent
    {

    }
    public class HandleMyTeam : GameEvent
    {
        public Team myTeam;
    }
}
