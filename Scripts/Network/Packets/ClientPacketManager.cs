using Core.EventSystem;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PacketManager
{
    private PacketHandler _packetHandler;
    public PacketManager(EventChannelSO packetChannel)
    {
        _packetHandler = new PacketHandler(packetChannel);
        Register();
    }

    Dictionary<ushort, Func<ArraySegment<byte>, IPacket>> _onRecv = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        RegisterHandler<S_RoomEnter>(PacketID.S_RoomEnter, _packetHandler.S_RoomEnterHandler);
        RegisterHandler<S_RoomExit>(PacketID.S_RoomExit, _packetHandler.S_RoomExitHandler);
        RegisterHandler<S_RoomList>(PacketID.S_RoomList, _packetHandler.S_RoomListHandler);
        RegisterHandler<S_EnterRoomFirst>(PacketID.S_EnterRoomFirst, _packetHandler.S_EnterRoomFirstHandler);
        RegisterHandler<S_UpdateInfos>(PacketID.S_UpdateInfos, _packetHandler.S_UpdateInfosHandler);
        RegisterHandler<S_TeamInfos>(PacketID.S_TeamInfos, _packetHandler.S_TeamInfosHandler);
        RegisterHandler<S_UpdateLocations>(PacketID.S_UpdateLocations, _packetHandler.S_UpdateLocations);
        RegisterHandler<S_SyncTimer>(PacketID.S_SyncTimer, _packetHandler.S_SyncTimerHandler);
        RegisterHandler<S_UpdateRoomState>(PacketID.S_UpdateRoomState, _packetHandler.S_UpdateRoomStateHandler);
        RegisterHandler<S_PacketResponse>(PacketID.S_PacketResponse, _packetHandler.S_PacketResponseHandler);
        RegisterHandler<S_RoundEnd>(PacketID.S_RoundEnd, _packetHandler.S_RoundEndHandler);
        RegisterHandler<S_GameEnd>(PacketID.S_GameEnd, _packetHandler.S_GameEndHandler);
        RegisterHandler<S_InitializeObjects>(PacketID.S_InitializeObjects, _packetHandler.S_InitializeObjectsHandler);
        RegisterHandler<S_CreateBombArea>(PacketID.S_CreateBombArea, _packetHandler.S_CreateBombHandler);
        RegisterHandler<S_SwitchRole>(PacketID.S_SwitchRole, _packetHandler.S_SwitchRoleHandler);
        RegisterHandler<S_RemoveObject>(PacketID.S_RemoveObject, _packetHandler.S_RemoveObjectHandler);
        RegisterHandler<S_DoorStatus>(PacketID.S_DoorStatus, _packetHandler.S_DoorStatusHandler);
        RegisterHandler<S_LeaveRoom>(PacketID.S_LeaveRoom, _packetHandler.S_LeaveRoomHandler);
        RegisterHandler<S_Reload>(PacketID.S_Reload, _packetHandler.S_ReloadHandler);
        RegisterHandler<S_BroadcastTime>(PacketID.S_BroadcastTime, _packetHandler.S_BroadcastTimeHandler);
    }

    private void RegisterHandler<T>(PacketID id, Action<PacketSession, IPacket> handler) where T : IPacket, new()
    {
        _onRecv.Add((ushort)id, PacketUtility.CreatePacket<T>);
        _handler.Add((ushort)id, handler);
    }

    public IPacket OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort packetId = PacketUtility.ReadPacketID(buffer);
        Func<ArraySegment<byte>, IPacket> func = null;
        if (_onRecv.TryGetValue(packetId, out func))
            return func.Invoke(buffer);
        return default;
    }
    public void HandlePacket(PacketSession session, IPacket packet)
    {
        if (_handler.ContainsKey(packet.Protocol))
            _handler[packet.Protocol].Invoke(session, packet);
        else
        {
            Debug.Log("Fail: "+packet.Protocol);
            throw new NullReferenceException();
        }
    }
}