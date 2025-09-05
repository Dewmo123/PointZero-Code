using Server.Objects;
using Server.Objects.Areas;
using Server.Rooms.States;
using Server.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Server.Rooms
{
    internal class GameRoom : Room
    {
        public bool CanAddPlayer => SessionCount < MaxSessionCount && _stateMachine.CurrentStateEnum == RoomState.Lobby;
        private RoomStateMachine _stateMachine;
        public RoomState CurrentState => _stateMachine.CurrentStateEnum;
        #region Game
        public Team Attacker { get; private set; }
        public Team Defender { get; private set; }
        public int CurrentRound { get; private set; }
        #endregion

        #region infos
        public Dictionary<Team, TeamInfo> teamInfos = new()
        {
            {Team.Blue,new TeamInfo() },
            {Team.Red,new TeamInfo() }
        };
        public Dictionary<Area, PlantArea> areaInfos;
        public BombArea bombArea;
        #endregion

        #region Events
        public event Action<ClientSession, C_ShootReq> OnAttack;
        public event Action<Vector3> OnPlant;
        public event Action OnDefuse;
        public event Action<DoorStatus, Door, Player> OnDoorStatusChange;
        #endregion

        #region roomSettings
        public int playTime { get; private set; }
        public int bombTime { get; private set; }
        public int roundCount { get; private set; }
        public int endRound { get; private set; }
        #endregion

        private S_InitializeObjects _initObjects = new();
        #region Initializer
        public GameRoom(RoomManager roomManager, string name, int roomId) : base(roomManager, roomId, name)
        {
            _stateMachine = new RoomStateMachine(this);
            _initObjects.plantAreas = new List<PlantAreaInfo>();
            _initObjects.doors = new List<DoorInfo>();
            _initObjects.breakableWalls = new List<BreakableWallInfo>();

            ChangeState(RoomState.Lobby);
            areaInfos = new()
            {
                {Area.A,new PlantArea(new(x:-11.92f,y:0,z:12.35f),new(x:12f,y:1,z:12f),myArea:Area.A,this) },
                {Area.B,new PlantArea(new(x:0.12f,y:0,z:3.65f),new(x:11,y:1,z:11),myArea:Area.B,this) }
            };
            foreach (var item in areaInfos.Values)
            {
                _initObjects.plantAreas.Add((PlantAreaInfo)item.CreatePacket());
            }
            #region Init Region
            InitDoor(new(x: -30.36f, y: 1.31f, z: 4.03f), new(1.2f, 2.2f, 0.2f), MathF.PI / 2);
            InitDoor(new(x: -0.05f, y: 1.31f, z: -2.27f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -7.85f, y: 1.31f, z: -10.325f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -20f, y: 1.31f, z: -6.287f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -20f, y: 1.31f, z: 1.654f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -12f, y: 1.31f, z: 1.654f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -16.532f, y: 1.31f, z: 6.175f), new(1.2f, 2.2f, 0.2f));
            InitDoor(new(x: -18.361f, y: 1.31f, z: 8.13f), new(1.2f, 2.2f, 0.2f), MathF.PI / 2);
            InitDoor(new(x: -25.856f, y: 1.31f, z: 12.13f), new(1.2f, 2.2f, 0.2f), MathF.PI / 2);
            InitDoor(new(x: -6.38f, y: 1.31f, z: -7.976f), new(1.2f, 2.2f, 0.2f), MathF.PI / 2);

            InitWall(new(x: -5.81859f, y: 1.5f, z: 3.97f), new(4, 3, 0.5f), MathF.PI / 2);
            #endregion
        }

        private void InitDoor(Vector3 position, Vector3 size, float yaw = 0, float pitch = 0, float roll = 0)
        {
            Door door = new(position, size, Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll), DoorStatus.Close, this);
            _initObjects.doors.Add((DoorInfo)door.CreatePacket());
        }
        private void InitWall(Vector3 position, Vector3 size, float yaw = 0, float pitch = 0, float roll = 0)
        {
            BreakableWall door = new(position, size, Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll), DoorStatus.Close, this);
            _initObjects.breakableWalls.Add((BreakableWallInfo)door.CreatePacket());
        }

        public void SetUpRoom(C_CreateRoom packet)
        {
            playTime = packet.playTime;
            MaxSessionCount = packet.playerCount;
            roundCount = packet.roundCount;
            endRound = roundCount / 2 + 1;
            bombTime = packet.bombTime;
        }

        #endregion

        private Team[] _teamTable = { Team.Blue, Team.Red };
        public void GameStart()
        {
            for (int i = 0; i < 5; i++)
            {
                int c = Random.Shared.Next(0, 2);
                Console.WriteLine(c);
                (_teamTable[0], _teamTable[c]) = (_teamTable[c], _teamTable[0]);
            }
            Attacker = _teamTable[0];
            Defender = _teamTable[1];
            SendSwitchRole();
            Console.WriteLine($"{Attacker} {Defender}");
            CurrentRound = 1;
            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                if (team == Team.None)
                    continue;
                teamInfos[team].BaseCount = MaxSessionCount / 2;
                teamInfos[team].ResetCurrentCount();
            }
            BroadcastInit();
        }
        public void BroadcastInit()
            => Broadcast(_initObjects);
        private void SendSwitchRole()
        {
            S_SwitchRole role = new();
            role.Attacker = (ushort)Attacker;
            role.Defender = (ushort)Defender;
            Broadcast(role);
        }

        public void NextRound(Team winTeam)
        {
            teamInfos[winTeam].Win();
            if (bombArea != null)
            {
                RemoveObject(bombArea.index);
                bombArea = null;
            }
            if (CheckGameEnd(teamInfos[Team.Red].WinCount, teamInfos[Team.Blue].WinCount))
            {
                SendRoundEnd(winTeam, true);
                return;
            }
            SendRoundEnd(winTeam, false);
            if (CurrentRound == roundCount / 2)
            {
                Attacker = Attacker == Team.Blue ? Team.Red : Team.Blue;
                Defender = Attacker == Team.Blue ? Team.Red : Team.Blue;
                SendSwitchRole();
            }

            foreach (Team team in Enum.GetValues(typeof(Team)))
            {
                if (team == Team.None)
                    continue;
                teamInfos[team].ResetCurrentCount();
            }
            CurrentRound++;
            ChangeState(RoomState.Between);
        }

        private void SendRoundEnd(Team winTeam, bool isEnd)
        {
            S_RoundEnd roundEnd = new();
            roundEnd.winner = (ushort)winTeam;
            roundEnd.redCount = teamInfos[Team.Red].WinCount;
            roundEnd.blueCount = teamInfos[Team.Blue].WinCount;
            Broadcast(roundEnd);
        }

        private bool CheckGameEnd(int redCount, int blueCount)
        {
            short winner = -1;
            if (redCount == endRound)
                winner = (short)Team.Red;
            else if (blueCount == endRound)
                winner = (short)Team.Blue;
            else if (redCount == roundCount / 2 && blueCount == roundCount / 2)
                winner = (short)Team.None;
            if (winner == -1)
                return false;
            SendGameEnd(winner);
            return true;
        }

        private void SendGameEnd(short winner)
        {
            ChangeState(RoomState.GameEnd);
            S_GameEnd gameEnd = new();
            gameEnd.winner = (ushort)winner;
            Broadcast(gameEnd);
        }

        public void FirstEnterProcess(ClientSession session)
        {
            S_EnterRoomFirst players = new();
            players.playerLocations = new List<LocationInfoPacket>();
            players.playerNames = new List<PlayerNamePacket>();
            Console.WriteLine("SendAllPlayer");
            players.myIndex = session.PlayerId;
            foreach (var item in _sessions.Values)
            {
                var player = GetObject<Player>(item.PlayerId);
                players.playerLocations.Add(new LocationInfoPacket()
                {
                    animHash = player.animHash,
                    gunRotation = player.gunRotation.ToPacket(),
                    index = player.index,
                    position = player.position.ToPacket(),
                    rotation = player.rotation.ToPacket()
                });
                players.playerNames.Add(new PlayerNamePacket()
                {
                    index = player.index,
                    nickName = player.nickName
                });
            }
            session.Send(players.Serialize());
        }

        #region Event
        public void Attack(ClientSession session, C_ShootReq req)
        {
            OnAttack?.Invoke(session, req);
        }
        public void SetDoorStatus(DoorStatus status, int doorIndex, int playerIndex)
        {
            OnDoorStatusChange?.Invoke(status, GetObject<Door>(doorIndex), GetObject<Player>(playerIndex));
        }
        public void Plant(int playerId, C_PlantBomb plant)
        {
            var player = GetObject<Player>(playerId);
            if (player.team == Attacker && areaInfos[(Area)plant.area].CheckInCube(player.position))
            {
                Console.WriteLine("Plant");
                OnPlant?.Invoke(player.position);
            }
            else
            {
                Console.WriteLine("Fail Plant");
            }
        }
        public void Defuse(int playerId)
        {
            if (bombArea == null)
                return;
            var player = GetObject<Player>(playerId);
            if (player.team == Defender && bombArea.CheckInCube(player.position))
            {
                OnDefuse?.Invoke();
            }
        }
        #endregion

        public List<int> GetSessionKeys()
            => _sessions.Keys.ToList();
        public override void Leave(ClientSession session)
        {
            Player player = GetObject<Player>(session.PlayerId);
            if (player.team != Team.None)
            {
                LeaveLogic(player);
            }
            base.Leave(session);

        }

        private void LeaveLogic(Player player)
        {
            TeamInfo teamInfo = teamInfos[player.team];

            switch (_stateMachine.CurrentStateEnum)
            {
                case RoomState.Between:
                    teamInfo.BaseCount--;
                    break;
                case RoomState.Prepare:
                case RoomState.InGame:
                    teamInfo.BaseCount--;
                    teamInfo.CurrentCount--;
                    break;
            }
            Team win = TeamEnumHelper.GetNegate(player.team);
            if (teamInfo.BaseCount == 0)
                SendGameEnd((short)win);
            else if (teamInfo.CurrentCount == 0)
                NextRound(win);
        }

        public override void AllPlayerExit()
        {
            base.AllPlayerExit();
            Console.WriteLine("Dispose");
            _stateMachine = null;
        }
        public override void ObjectDead(ObjectBase obj)
        {
            if (obj is Player player && CurrentState != RoomState.Between)
            {
                teamInfos[player.team].CurrentCount--;
                if (teamInfos[player.team].CurrentCount == 0)
                {
                    NextRound(TeamEnumHelper.GetNegate(player.team));
                }
            }
        }

        #region StateMachine
        private S_UpdateRoomState _updateState = new();
        public void ChangeState(RoomState state)
        {
            _stateMachine.ChangeState(state);
            _updateState.state = (ushort)state;
            Broadcast(_updateState);
        }
        public override void UpdateRoom()
            => _stateMachine?.UpdateRoom();
        #endregion
    }
}
