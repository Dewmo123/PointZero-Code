using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_onRecv.Add((ushort)PacketID.S_RoomEnter, MakePacket<S_RoomEnter>);
		_handler.Add((ushort)PacketID.S_RoomEnter, PacketHandler.S_RoomEnterHandler);
		_onRecv.Add((ushort)PacketID.S_RoomExit, MakePacket<S_RoomExit>);
		_handler.Add((ushort)PacketID.S_RoomExit, PacketHandler.S_RoomExitHandler);
		_onRecv.Add((ushort)PacketID.S_RoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)PacketID.S_RoomList, PacketHandler.S_RoomListHandler);
		_onRecv.Add((ushort)PacketID.S_PacketResponse, MakePacket<S_PacketResponse>);
		_handler.Add((ushort)PacketID.S_PacketResponse, PacketHandler.S_PacketResponseHandler);
		_onRecv.Add((ushort)PacketID.S_TestText, MakePacket<S_TestText>);
		_handler.Add((ushort)PacketID.S_TestText, PacketHandler.S_TestTextHandler);
		_onRecv.Add((ushort)PacketID.S_EnterRoomFirst, MakePacket<S_EnterRoomFirst>);
		_handler.Add((ushort)PacketID.S_EnterRoomFirst, PacketHandler.S_EnterRoomFirstHandler);
		_onRecv.Add((ushort)PacketID.S_UpdateInfos, MakePacket<S_UpdateInfos>);
		_handler.Add((ushort)PacketID.S_UpdateInfos, PacketHandler.S_UpdateInfosHandler);
		_onRecv.Add((ushort)PacketID.S_TeamInfos, MakePacket<S_TeamInfos>);
		_handler.Add((ushort)PacketID.S_TeamInfos, PacketHandler.S_TeamInfosHandler);
		_onRecv.Add((ushort)PacketID.S_UpdateLocations, MakePacket<S_UpdateLocations>);
		_handler.Add((ushort)PacketID.S_UpdateLocations, PacketHandler.S_UpdateLocationsHandler);
		_onRecv.Add((ushort)PacketID.S_SyncTimer, MakePacket<S_SyncTimer>);
		_handler.Add((ushort)PacketID.S_SyncTimer, PacketHandler.S_SyncTimerHandler);
		_onRecv.Add((ushort)PacketID.S_UpdateRoomState, MakePacket<S_UpdateRoomState>);
		_handler.Add((ushort)PacketID.S_UpdateRoomState, PacketHandler.S_UpdateRoomStateHandler);
		_onRecv.Add((ushort)PacketID.S_RoundEnd, MakePacket<S_RoundEnd>);
		//_handler.Add((ushort)PacketID.S_RoundEnd, PacketHandler.S_RoundEndHandler);
		//_onRecv.Add((ushort)PacketID.S_GameEnd, MakePacket<S_GameEnd>);
		//_handler.Add((ushort)PacketID.S_GameEnd, PacketHandler.S_GameEndHandler);
		//_onRecv.Add((ushort)PacketID.S_InitializeObjects, MakePacket<S_InitializeObjects>);
		//_handler.Add((ushort)PacketID.S_InitializeObjects, PacketHandler.S_InitializeObjectsHandler);
		//_onRecv.Add((ushort)PacketID.S_CreateBombArea, MakePacket<S_CreateBombArea>);
		//_handler.Add((ushort)PacketID.S_CreateBombArea, PacketHandler.S_CreateBombAreaHandler);
		//_onRecv.Add((ushort)PacketID.S_SwitchRole, MakePacket<S_SwitchRole>);
		//_handler.Add((ushort)PacketID.S_SwitchRole, PacketHandler.S_SwitchRoleHandler);
		//_onRecv.Add((ushort)PacketID.S_RemoveObject, MakePacket<S_RemoveObject>);
		//_handler.Add((ushort)PacketID.S_RemoveObject, PacketHandler.S_RemoveObjectHandler);
		//_onRecv.Add((ushort)PacketID.S_DoorStatus, MakePacket<S_DoorStatus>);
		//_handler.Add((ushort)PacketID.S_DoorStatus, PacketHandler.S_DoorStatusHandler);
		//_onRecv.Add((ushort)PacketID.S_LeaveRoom, MakePacket<S_LeaveRoom>);
		//_handler.Add((ushort)PacketID.S_LeaveRoom, PacketHandler.S_LeaveRoomHandler);
		//_onRecv.Add((ushort)PacketID.S_Reload, MakePacket<S_Reload>);
		//_handler.Add((ushort)PacketID.S_Reload, PacketHandler.S_ReloadHandler);
		//_onRecv.Add((ushort)PacketID.S_BroadcastTime, MakePacket<S_BroadcastTime>);
		//_handler.Add((ushort)PacketID.S_BroadcastTime, PacketHandler.S_BroadcastTimeHandler);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort packetId = PacketUtility.ReadPacketID(buffer);

		Action<PacketSession, ArraySegment<byte>> action = null;
		if (_onRecv.TryGetValue(packetId, out action))
			action.Invoke(session, buffer);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Deserialize(buffer);
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(pkt.Protocol, out action))
			action.Invoke(session, pkt);
	}
}