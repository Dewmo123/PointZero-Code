using Assets.Scripts.Combat;
using Assets.Scripts.Entities.Players.OtherPlayers;
using Core.EventSystem;
using Scripts.Core;
using Scripts.Core.EventSystem;
using Scripts.Core.Managers;
using Scripts.Entities.Players;
using Scripts.Entities.Players.MyPlayers;
using Scripts.Entities.Players.MyPlayers.Interact;
using Scripts.Entities.Players.OtherPlayers;
using Scripts.GameSystem.CheckPlayer;
using Scripts.Network;
using ServerCore;
using System;
using UnityEngine;
using ObjectManager = Scripts.Core.Managers.ObjectManager;

class PacketHandler
{
    private EventChannelSO _packetChannel;
    private PlayerManager _playerManager = PlayerManager.Instance;
    private ObjectManager _objectManager = ObjectManager.Instance;
    public PacketHandler(EventChannelSO packetChannel)
    {
        _packetChannel = packetChannel;
    }

    internal void S_CreateBombHandler(PacketSession session, IPacket packet)
    {
        var createBomb = packet as S_CreateBombArea;
        CubeInfo bombInfo = createBomb.bombInfo;
        BombArea bombArea = _objectManager.CreateObject<BombArea>(bombInfo.index, (ObjectType)bombInfo.objectType);
        bombArea.InitArea(bombInfo.size.ToVector3(), bombInfo.position.ToVector3(), bombInfo.rotation.ToQuaternion(), bombInfo.index);
        _playerManager.MyPlayer.GetCompo<PlayerInteractManager>().isBombPlant = true;
        var evt = PacketEvents.HandleTimerElapsed;
        evt.remainTime = 0;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_InitializeObjectsHandler(PacketSession session, IPacket packet)
    {
        var initObjects = packet as S_InitializeObjects;
        foreach (var item in initObjects.plantAreas)
        {
            CubeInfo cube = item.cube;
            PlantArea area = _objectManager.GetObjctOrCreate<PlantArea>(cube.index, (ObjectType)cube.objectType);
            area.InitArea(cube.size.ToVector3(), cube.position.ToVector3(), cube.rotation.ToQuaternion(), cube.index, item.area);
        }
        foreach (var item in initObjects.doors)
        {
            CubeInfo cube = item.cube;
            Door door = _objectManager.GetObjctOrCreate<Door>(cube.index, (ObjectType)cube.objectType);
            door.InitArea(cube.size.ToVector3(), cube.position.ToVector3(), cube.rotation.ToQuaternion(), cube.index, item.status);
        }
        foreach (var item in initObjects.breakableWalls)
        {
            CubeInfo cube = item.cube;
            BreakableWall wall = _objectManager.GetObjctOrCreate<BreakableWall>(cube.index, (ObjectType)cube.objectType);
            wall.InitArea(cube.size.ToVector3(), cube.position.ToVector3(), cube.rotation.ToQuaternion(), cube.index);
        }
    }

    internal void S_EnterRoomFirstHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("RoomFIrst");
        var firstPacket = packet as S_EnterRoomFirst;
        _playerManager.FirstInitPlayer(firstPacket);
        var evt = PacketEvents.HandleMyTeam;
        evt.myTeam = Team.None;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_GameEndHandler(PacketSession session, IPacket packet)
    {
        var evt = PacketEvents.HandleGameEnd;
        var gameEnd = packet as S_GameEnd;
        evt.winner = (Team)gameEnd.winner;
        evt.isWin = (Team)gameEnd.winner == _playerManager.MyPlayer.MyTeam;
        _packetChannel.InvokeEvent(evt);
        Debug.Log($"Winner: {evt.winner}");
    }

    internal void S_PacketResponseHandler(PacketSession session, IPacket packet)
    {
        var response = packet as S_PacketResponse;
        var evt = PacketEvents.HandlePacketResponse;
        evt.packetId = (PacketID)response.responsePacket;
        evt.success = response.success;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_RemoveObjectHandler(PacketSession session, IPacket packet)
    {
        var removeObject = packet as S_RemoveObject;
        _objectManager.RemoveObject(removeObject.index);
    }

    internal void S_RoomEnterHandler(PacketSession session, IPacket packet)
    {
        var roomEnter = packet as S_RoomEnter;
        var enterRoom = PacketEvents.HandleEnterRoom;
        enterRoom.packet = roomEnter;
        
        _packetChannel.InvokeEvent(enterRoom);
        _playerManager.InitOtherPlayer(roomEnter.newPlayer);
        _playerManager.SetPlayerName(roomEnter.playerName);

        var evt = PacketEvents.HandleUpdateRoomState;
        evt.state = RoomState.Lobby;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_RoomExitHandler(PacketSession session, IPacket packet)
    {
        var roomExit = packet as S_RoomExit;
        _playerManager.ExitOtherPlayer(roomExit.Index);
    }

    internal void S_RoomListHandler(PacketSession session, IPacket packet)
    {
        var evt = PacketEvents.HandleRoomList;
        evt.packet = packet as S_RoomList;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_RoundEndHandler(PacketSession session, IPacket packet)
    {
        var roundEnd = packet as S_RoundEnd;
        var evt = PacketEvents.HandleRoundEnd;
        evt.blueCount = roundEnd.blueCount;
        evt.redCount = roundEnd.redCount;
        evt.winner = (Team)roundEnd.winner;
        evt.isEnd = roundEnd.isEnd;
        Debug.Log("RoundEnd");
        _packetChannel.InvokeEvent(evt);
        _playerManager.MyPlayer.GetCompo<PlayerInteractManager>().isBombPlant = false;
    }

    internal void S_SwitchRoleHandler(PacketSession session, IPacket packet)
    {
        Player myPlayer = PlayerManager.Instance.MyPlayer;
        S_SwitchRole role = packet as S_SwitchRole;
        myPlayer.GetCompo<PlayerInteractManager>().SetRole((Team)role.Attacker, (Team)role.Defender);
    }

    internal void S_SyncTimerHandler(PacketSession session, IPacket packet)
    {
        var timer = packet as S_SyncTimer;
        var evt = PacketEvents.HandleTimerElapsed;
        evt.remainTime = timer.time;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_TeamInfosHandler(PacketSession session, IPacket packet)
    {
        var infos = packet as S_TeamInfos;
        var myPlayer = _playerManager.MyPlayer;
        foreach (var item in infos.teamInfos)
        {
            var player = _playerManager.GetPlayerById(item.index);
            player.SetTeam(item.team);
            Debug.Log($"index:{item.index}, Team:{item.team}");
        }
        var evt = PacketEvents.HandleMyTeam;
        evt.myTeam = myPlayer.MyTeam;
        _packetChannel.InvokeEvent(evt);
        var players = _playerManager.GetPlayers();
        foreach (var item in players)
        {
            item.GetCompo<PlayerTeamManager>().SetTeam(item.MyTeam == myPlayer.MyTeam);
        }
    }

    internal void S_UpdateInfosHandler(PacketSession session, IPacket packet)
    {
        var p = packet as S_UpdateInfos;
        foreach (var item in p.playerInfos)
        {
            var player = _playerManager.GetPlayerById(item.index);
            if (player == null)
                continue;
            player.Health.Value = item.Health;
            if (player.Index == _playerManager.MyIndex)
                continue;
            var movement = player.GetCompo<OtherPlayerMovement>();
            movement.HandleAim(item.isAiming);
        }
        foreach (var item in p.snapshots)
        {
            var player = _playerManager.GetPlayerById(item.index);
            if (player == null)
                continue;
            if (player.Index == _playerManager.MyIndex)
                continue;
            var movement = player.GetCompo<OtherPlayerMovement>();
            movement.AddSnapshot(item);
        }
        foreach (var item in p.attacks)
        {
            var attacker = _playerManager.GetPlayerById(item.attackerIndex);
            if (attacker == null)
                continue;
            if (item.attackerIndex != _playerManager.MyIndex)
            {
                attacker.GetCompo<OtherPlayerAttackCompo>().Shoot(item.firePos.ToVector3(), item.direction.ToVector3());
            }
            if (item.isDead)
            {
                var hitObj = _objectManager.GetObejct<IHittable>(item.hitObjIndex);
                hitObj.SetDead(attacker);
            }
        }
    }

    internal void S_UpdateLocations(PacketSession session, IPacket packet)
    {
        var p = packet as S_UpdateLocations;

        foreach (var item in p.locations)
        {
            var player = _playerManager.GetPlayerById(item.index);
            var movement = player.GetCompo<PlayerMovement>(true);
            movement.SetPosition(item.position.ToVector3());
        }
    }

    internal void S_UpdateRoomStateHandler(PacketSession session, IPacket packet)
    {
        var roomStatePacket = packet as S_UpdateRoomState;
        var evt = PacketEvents.HandleUpdateRoomState;
        evt.state = (RoomState)roomStatePacket.state;
        _packetChannel.InvokeEvent(evt);
    }

    internal void S_DoorStatusHandler(PacketSession session, IPacket packet)
    {
        var doorStatus = packet as S_DoorStatus;
        Door door = _objectManager.GetObejct<Door>(doorStatus.index);
        Debug.Log(door.gameObject);
        door.SetStatus((DoorStatus)doorStatus.status);
    }

    internal void S_LeaveRoomHandler(PacketSession session, IPacket packet)
    {
        var evt = PacketEvents.HandleLeaveRoom;
        _packetChannel.InvokeEvent(evt);
        _playerManager.DestroyAll();
        _objectManager.DestroyAll();
    }

    internal void S_ReloadHandler(PacketSession session, IPacket packet)
    {
        var reload = packet as S_Reload;
        Player player = _playerManager.GetPlayerById(reload.index);
        player.Reload();
    }

    internal void S_BroadcastTimeHandler(PacketSession session, IPacket packet)
    {
        var broadcastTime = packet as S_BroadcastTime;
        Debug.Log($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}, {broadcastTime.time}");
        OtherPlayerMovement.InterpolationBackTime = (long)((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - broadcastTime.time));
    }
}
