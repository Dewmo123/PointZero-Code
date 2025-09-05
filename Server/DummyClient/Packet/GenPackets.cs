using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	C_RoomEnter = 1,
	S_RoomEnter = 2,
	C_RoomExit = 3,
	S_RoomExit = 4,
	C_CreateRoom = 5,
	S_RoomList = 6,
	S_PacketResponse = 7,
	C_RoomList = 8,
	S_TestText = 9,
	S_EnterRoomFirst = 10,
	S_UpdateInfos = 11,
	C_UpdateLocation = 12,
	S_TeamInfos = 13,
	S_UpdateLocations = 14,
	S_SyncTimer = 15,
	S_UpdateRoomState = 16,
	C_ShootReq = 17,
	S_RoundEnd = 18,
	S_GameEnd = 19,
	C_PlantBomb = 20,
	C_DefuseBomb = 21,
	S_InitializeObjects = 22,
	S_CreateBombArea = 23,
	S_SwitchRole = 24,
	S_RemoveObject = 25,
	S_DoorStatus = 26,
	C_DoorStatus = 27,
	C_SetName = 28,
	S_LeaveRoom = 29,
	C_Reload = 30,
	S_Reload = 31,
	S_BroadcastTime = 32,
	
}

public struct VectorPacket : IDataPacket
{
	public float x;
	public float y;
	public float z;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadFloatData(segment, count, out x);
		count += PacketUtility.ReadFloatData(segment, count, out y);
		count += PacketUtility.ReadFloatData(segment, count, out z);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendFloatData(this.x, segment, count);
		count += PacketUtility.AppendFloatData(this.y, segment, count);
		count += PacketUtility.AppendFloatData(this.z, segment, count);
		return (ushort)(count - offset);
	}
}

public struct QuaternionPacket : IDataPacket
{
	public float x;
	public float y;
	public float z;
	public float w;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadFloatData(segment, count, out x);
		count += PacketUtility.ReadFloatData(segment, count, out y);
		count += PacketUtility.ReadFloatData(segment, count, out z);
		count += PacketUtility.ReadFloatData(segment, count, out w);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendFloatData(this.x, segment, count);
		count += PacketUtility.AppendFloatData(this.y, segment, count);
		count += PacketUtility.AppendFloatData(this.z, segment, count);
		count += PacketUtility.AppendFloatData(this.w, segment, count);
		return (ushort)(count - offset);
	}
}

public struct RoomInfoPacket : IDataPacket
{
	public int roomId;
	public int maxCount;
	public int currentCount;
	public string roomName;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out roomId);
		count += PacketUtility.ReadIntData(segment, count, out maxCount);
		count += PacketUtility.ReadIntData(segment, count, out currentCount);
		count += PacketUtility.ReadStringData(segment, count, out roomName);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.roomId, segment, count);
		count += PacketUtility.AppendIntData(this.maxCount, segment, count);
		count += PacketUtility.AppendIntData(this.currentCount, segment, count);
		count += PacketUtility.AppendStringData(this.roomName, segment, count);
		return (ushort)(count - offset);
	}
}

public struct PlayerInfoPacket : IDataPacket
{
	public int index;
	public bool isAiming;
	public int Health;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadBoolData(segment, count, out isAiming);
		count += PacketUtility.ReadIntData(segment, count, out Health);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendBoolData(this.isAiming, segment, count);
		count += PacketUtility.AppendIntData(this.Health, segment, count);
		return (ushort)(count - offset);
	}
}

public struct PlayerNamePacket : IDataPacket
{
	public string nickName;
	public int index;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadStringData(segment, count, out nickName);
		count += PacketUtility.ReadIntData(segment, count, out index);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendStringData(this.nickName, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		return (ushort)(count - offset);
	}
}

public struct LocationInfoPacket : IDataPacket
{
	public int index;
	public int animHash;
	public VectorPacket position;
	public QuaternionPacket rotation;
	public QuaternionPacket gunRotation;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadIntData(segment, count, out animHash);
		count += PacketUtility.ReadDataPacketData(segment, count, out position);
		count += PacketUtility.ReadDataPacketData(segment, count, out rotation);
		count += PacketUtility.ReadDataPacketData(segment, count, out gunRotation);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendIntData(this.animHash, segment, count);
		count += PacketUtility.AppendDataPacketData(this.position, segment, count);
		count += PacketUtility.AppendDataPacketData(this.rotation, segment, count);
		count += PacketUtility.AppendDataPacketData(this.gunRotation, segment, count);
		return (ushort)(count - offset);
	}
}

public struct SnapshotPacket : IDataPacket
{
	public int index;
	public int animHash;
	public long timestamp;
	public VectorPacket position;
	public QuaternionPacket rotation;
	public QuaternionPacket gunRotation;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadIntData(segment, count, out animHash);
		count += PacketUtility.ReadLongData(segment, count, out timestamp);
		count += PacketUtility.ReadDataPacketData(segment, count, out position);
		count += PacketUtility.ReadDataPacketData(segment, count, out rotation);
		count += PacketUtility.ReadDataPacketData(segment, count, out gunRotation);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendIntData(this.animHash, segment, count);
		count += PacketUtility.AppendLongData(this.timestamp, segment, count);
		count += PacketUtility.AppendDataPacketData(this.position, segment, count);
		count += PacketUtility.AppendDataPacketData(this.rotation, segment, count);
		count += PacketUtility.AppendDataPacketData(this.gunRotation, segment, count);
		return (ushort)(count - offset);
	}
}

public struct TeamInfoPacket : IDataPacket
{
	public int index;
	public ushort team;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadUshortData(segment, count, out team);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendUshortData(this.team, segment, count);
		return (ushort)(count - offset);
	}
}

public struct AttackInfoBr : IDataPacket
{
	public int hitObjIndex;
	public int attackerIndex;
	public bool isDead;
	public ushort objectType;
	public VectorPacket firePos;
	public VectorPacket direction;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out hitObjIndex);
		count += PacketUtility.ReadIntData(segment, count, out attackerIndex);
		count += PacketUtility.ReadBoolData(segment, count, out isDead);
		count += PacketUtility.ReadUshortData(segment, count, out objectType);
		count += PacketUtility.ReadDataPacketData(segment, count, out firePos);
		count += PacketUtility.ReadDataPacketData(segment, count, out direction);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.hitObjIndex, segment, count);
		count += PacketUtility.AppendIntData(this.attackerIndex, segment, count);
		count += PacketUtility.AppendBoolData(this.isDead, segment, count);
		count += PacketUtility.AppendUshortData(this.objectType, segment, count);
		count += PacketUtility.AppendDataPacketData(this.firePos, segment, count);
		count += PacketUtility.AppendDataPacketData(this.direction, segment, count);
		return (ushort)(count - offset);
	}
}

public struct CubeInfo : IDataPacket
{
	public ushort objectType;
	public int index;
	public VectorPacket position;
	public QuaternionPacket rotation;
	public VectorPacket size;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadUshortData(segment, count, out objectType);
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadDataPacketData(segment, count, out position);
		count += PacketUtility.ReadDataPacketData(segment, count, out rotation);
		count += PacketUtility.ReadDataPacketData(segment, count, out size);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendUshortData(this.objectType, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendDataPacketData(this.position, segment, count);
		count += PacketUtility.AppendDataPacketData(this.rotation, segment, count);
		count += PacketUtility.AppendDataPacketData(this.size, segment, count);
		return (ushort)(count - offset);
	}
}

public struct PlantAreaInfo : IDataPacket
{
	public ushort area;
	public CubeInfo cube;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadUshortData(segment, count, out area);
		count += PacketUtility.ReadDataPacketData(segment, count, out cube);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendUshortData(this.area, segment, count);
		count += PacketUtility.AppendDataPacketData(this.cube, segment, count);
		return (ushort)(count - offset);
	}
}

public struct DoorInfo : IDataPacket
{
	public ushort status;
	public CubeInfo cube;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadUshortData(segment, count, out status);
		count += PacketUtility.ReadDataPacketData(segment, count, out cube);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendUshortData(this.status, segment, count);
		count += PacketUtility.AppendDataPacketData(this.cube, segment, count);
		return (ushort)(count - offset);
	}
}

public struct BreakableWallInfo : IDataPacket
{
	public int Health;
	public CubeInfo cube;

	public ushort Deserialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.ReadIntData(segment, count, out Health);
		count += PacketUtility.ReadDataPacketData(segment, count, out cube);
		return (ushort)(count - offset);
	}

	public ushort Serialize(ArraySegment<byte> segment, int offset)
	{
		ushort count = (ushort)offset;
		count += PacketUtility.AppendIntData(this.Health, segment, count);
		count += PacketUtility.AppendDataPacketData(this.cube, segment, count);
		return (ushort)(count - offset);
	}
}

public class C_RoomEnter : IPacket
{
	public int roomId;

	public ushort Protocol { get { return (ushort)PacketID.C_RoomEnter; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out roomId);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.roomId, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_RoomEnter : IPacket
{
	public PlayerNamePacket playerName;
	public LocationInfoPacket newPlayer;

	public ushort Protocol { get { return (ushort)PacketID.S_RoomEnter; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadDataPacketData(segment, count, out playerName);
		count += PacketUtility.ReadDataPacketData(segment, count, out newPlayer);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendDataPacketData(this.playerName, segment, count);
		count += PacketUtility.AppendDataPacketData(this.newPlayer, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_RoomExit : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_RoomExit; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_RoomExit : IPacket
{
	public int Index;

	public ushort Protocol { get { return (ushort)PacketID.S_RoomExit; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out Index);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.Index, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_CreateRoom : IPacket
{
	public string roomName;
	public ushort playerCount;
	public ushort roundCount;
	public ushort playTime;
	public ushort bombTime;

	public ushort Protocol { get { return (ushort)PacketID.C_CreateRoom; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadStringData(segment, count, out roomName);
		count += PacketUtility.ReadUshortData(segment, count, out playerCount);
		count += PacketUtility.ReadUshortData(segment, count, out roundCount);
		count += PacketUtility.ReadUshortData(segment, count, out playTime);
		count += PacketUtility.ReadUshortData(segment, count, out bombTime);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendStringData(this.roomName, segment, count);
		count += PacketUtility.AppendUshortData(this.playerCount, segment, count);
		count += PacketUtility.AppendUshortData(this.roundCount, segment, count);
		count += PacketUtility.AppendUshortData(this.playTime, segment, count);
		count += PacketUtility.AppendUshortData(this.bombTime, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_RoomList : IPacket
{
	public List<RoomInfoPacket> roomInfos;

	public ushort Protocol { get { return (ushort)PacketID.S_RoomList; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadListData(segment, count, out roomInfos);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendListData(this.roomInfos, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_PacketResponse : IPacket
{
	public ushort responsePacket;
	public bool success;

	public ushort Protocol { get { return (ushort)PacketID.S_PacketResponse; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadUshortData(segment, count, out responsePacket);
		count += PacketUtility.ReadBoolData(segment, count, out success);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendUshortData(this.responsePacket, segment, count);
		count += PacketUtility.AppendBoolData(this.success, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_RoomList : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_RoomList; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_TestText : IPacket
{
	public string text;

	public ushort Protocol { get { return (ushort)PacketID.S_TestText; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadStringData(segment, count, out text);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendStringData(this.text, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_EnterRoomFirst : IPacket
{
	public int myIndex;
	public List<LocationInfoPacket> playerLocations;
	public List<PlayerNamePacket> playerNames;

	public ushort Protocol { get { return (ushort)PacketID.S_EnterRoomFirst; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out myIndex);
		count += PacketUtility.ReadListData(segment, count, out playerLocations);
		count += PacketUtility.ReadListData(segment, count, out playerNames);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.myIndex, segment, count);
		count += PacketUtility.AppendListData(this.playerLocations, segment, count);
		count += PacketUtility.AppendListData(this.playerNames, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_UpdateInfos : IPacket
{
	public List<PlayerInfoPacket> playerInfos;
	public List<SnapshotPacket> snapshots;
	public List<AttackInfoBr> attacks;

	public ushort Protocol { get { return (ushort)PacketID.S_UpdateInfos; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadListData(segment, count, out playerInfos);
		count += PacketUtility.ReadListData(segment, count, out snapshots);
		count += PacketUtility.ReadListData(segment, count, out attacks);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendListData(this.playerInfos, segment, count);
		count += PacketUtility.AppendListData(this.snapshots, segment, count);
		count += PacketUtility.AppendListData(this.attacks, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_UpdateLocation : IPacket
{
	public bool isAiming;
	public LocationInfoPacket location;

	public ushort Protocol { get { return (ushort)PacketID.C_UpdateLocation; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadBoolData(segment, count, out isAiming);
		count += PacketUtility.ReadDataPacketData(segment, count, out location);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendBoolData(this.isAiming, segment, count);
		count += PacketUtility.AppendDataPacketData(this.location, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_TeamInfos : IPacket
{
	public List<TeamInfoPacket> teamInfos;

	public ushort Protocol { get { return (ushort)PacketID.S_TeamInfos; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadListData(segment, count, out teamInfos);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendListData(this.teamInfos, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_UpdateLocations : IPacket
{
	public List<LocationInfoPacket> locations;

	public ushort Protocol { get { return (ushort)PacketID.S_UpdateLocations; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadListData(segment, count, out locations);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendListData(this.locations, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_SyncTimer : IPacket
{
	public float time;

	public ushort Protocol { get { return (ushort)PacketID.S_SyncTimer; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadFloatData(segment, count, out time);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendFloatData(this.time, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_UpdateRoomState : IPacket
{
	public ushort state;

	public ushort Protocol { get { return (ushort)PacketID.S_UpdateRoomState; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadUshortData(segment, count, out state);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendUshortData(this.state, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_ShootReq : IPacket
{
	public int hitObjIndex;
	public VectorPacket firePos;
	public VectorPacket direction;

	public ushort Protocol { get { return (ushort)PacketID.C_ShootReq; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out hitObjIndex);
		count += PacketUtility.ReadDataPacketData(segment, count, out firePos);
		count += PacketUtility.ReadDataPacketData(segment, count, out direction);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.hitObjIndex, segment, count);
		count += PacketUtility.AppendDataPacketData(this.firePos, segment, count);
		count += PacketUtility.AppendDataPacketData(this.direction, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_RoundEnd : IPacket
{
	public int redCount;
	public int blueCount;
	public bool isEnd;
	public ushort winner;

	public ushort Protocol { get { return (ushort)PacketID.S_RoundEnd; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out redCount);
		count += PacketUtility.ReadIntData(segment, count, out blueCount);
		count += PacketUtility.ReadBoolData(segment, count, out isEnd);
		count += PacketUtility.ReadUshortData(segment, count, out winner);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.redCount, segment, count);
		count += PacketUtility.AppendIntData(this.blueCount, segment, count);
		count += PacketUtility.AppendBoolData(this.isEnd, segment, count);
		count += PacketUtility.AppendUshortData(this.winner, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_GameEnd : IPacket
{
	public ushort winner;

	public ushort Protocol { get { return (ushort)PacketID.S_GameEnd; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadUshortData(segment, count, out winner);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendUshortData(this.winner, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_PlantBomb : IPacket
{
	public ushort area;

	public ushort Protocol { get { return (ushort)PacketID.C_PlantBomb; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadUshortData(segment, count, out area);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendUshortData(this.area, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_DefuseBomb : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_DefuseBomb; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_InitializeObjects : IPacket
{
	public List<PlantAreaInfo> plantAreas;
	public List<DoorInfo> doors;
	public List<BreakableWallInfo> breakableWalls;

	public ushort Protocol { get { return (ushort)PacketID.S_InitializeObjects; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadListData(segment, count, out plantAreas);
		count += PacketUtility.ReadListData(segment, count, out doors);
		count += PacketUtility.ReadListData(segment, count, out breakableWalls);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendListData(this.plantAreas, segment, count);
		count += PacketUtility.AppendListData(this.doors, segment, count);
		count += PacketUtility.AppendListData(this.breakableWalls, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_CreateBombArea : IPacket
{
	public CubeInfo bombInfo;

	public ushort Protocol { get { return (ushort)PacketID.S_CreateBombArea; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadDataPacketData(segment, count, out bombInfo);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendDataPacketData(this.bombInfo, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_SwitchRole : IPacket
{
	public ushort Attacker;
	public ushort Defender;

	public ushort Protocol { get { return (ushort)PacketID.S_SwitchRole; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadUshortData(segment, count, out Attacker);
		count += PacketUtility.ReadUshortData(segment, count, out Defender);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendUshortData(this.Attacker, segment, count);
		count += PacketUtility.AppendUshortData(this.Defender, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_RemoveObject : IPacket
{
	public int index;

	public ushort Protocol { get { return (ushort)PacketID.S_RemoveObject; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out index);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_DoorStatus : IPacket
{
	public int index;
	public ushort status;

	public ushort Protocol { get { return (ushort)PacketID.S_DoorStatus; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadUshortData(segment, count, out status);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendUshortData(this.status, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_DoorStatus : IPacket
{
	public int index;
	public ushort status;

	public ushort Protocol { get { return (ushort)PacketID.C_DoorStatus; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out index);
		count += PacketUtility.ReadUshortData(segment, count, out status);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		count += PacketUtility.AppendUshortData(this.status, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_SetName : IPacket
{
	public string nickName;

	public ushort Protocol { get { return (ushort)PacketID.C_SetName; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadStringData(segment, count, out nickName);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendStringData(this.nickName, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_LeaveRoom : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.S_LeaveRoom; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class C_Reload : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_Reload; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_Reload : IPacket
{
	public int index;

	public ushort Protocol { get { return (ushort)PacketID.S_Reload; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadIntData(segment, count, out index);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendIntData(this.index, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastTime : IPacket
{
	public long time;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastTime; } }

	public void Deserialize(ArraySegment<byte> segment)
	{
		ushort count = 0;

		count += sizeof(ushort);
		count += sizeof(ushort);
		count += PacketUtility.ReadLongData(segment, count, out time);
	}

	public ArraySegment<byte> Serialize()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		count += PacketUtility.AppendUshortData(this.Protocol, segment, count);
		count += PacketUtility.AppendLongData(this.time, segment, count);
		PacketUtility.AppendUshortData(count, segment, 0);
		return SendBufferHelper.Close(count);
	}
}

