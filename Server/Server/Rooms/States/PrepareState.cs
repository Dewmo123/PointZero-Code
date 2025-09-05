using Server.Objects;
using Server.Utiles;
using System;
using System.Collections.Generic;

namespace Server.Rooms.States
{
    internal class PrepareState : SyncObjectsState
    {
        #region StartPos Settings
        private static readonly VectorPacket[] attackStartPos =
        {
            new VectorPacket() { x=-27,y=1,z=-14},
            new VectorPacket() { x=-26,y=1,z=-15},
            new VectorPacket() { x=-25,y=1,z=-14},
            new VectorPacket() { x=-28,y=1,z=-15},
            new VectorPacket() { x=-29,y=1,z=-14},
        };
        private static readonly VectorPacket[] defenseStartPos =
        {
            new VectorPacket() { x=-3,y=1,z=16},
            new VectorPacket() { x=-2,y=1,z=15},
            new VectorPacket() { x=-4,y=1,z=16},
            new VectorPacket() { x=-5,y=1,z=15},
            new VectorPacket() { x=-6,y=1,z=16},
        };
        private static ushort[] randomTable = { 0, 1, 2, 3, 4 };
        #endregion


        private S_SyncTimer _syncTime = new();
        private Random _rand = new Random();
        private CountTime _roundCount;
        private static readonly int _prepareTime = 5;

        public PrepareState(GameRoom room) : base(room)
        {
            _roundCount = new (HandleTimerElapsed, CompleteTimer, _prepareTime, 100);
        }
        public override void Enter()
        {
            base.Enter();
            Console.WriteLine($"Current: {_room.CurrentRound}");
            SetStartPosition();
            _room.ReviveAllPlayer();
            _roundCount.StartCount();
        }
        public override void Update()
        {
            base.Update();
            _roundCount.UpdateDeltaTime();
        }
        private void CompleteTimer()
        {
            _room.ChangeState(RoomState.InGame);
        }
        private void HandleTimerElapsed(double time)
        {
            _syncTime.time = _prepareTime - (float)time;
            _room.Broadcast(_syncTime);
        }
        private void SetStartPosition()
        {
            for (int i = 0; i < 10; i++)
            {
                int change = _rand.Next(0, randomTable.Length);
                (randomTable[0], randomTable[change]) = (randomTable[change], randomTable[0]);
            }
            int index = 0;
            S_UpdateLocations updateLocations = new();
            updateLocations.locations = new List<LocationInfoPacket>();
            foreach (var item in _room.Sessions.Values)
            {
                Player player = _room.GetObject<Player>(item.PlayerId);
                updateLocations.locations.Add(new LocationInfoPacket()
                {
                    animHash = 0,
                    index = player.index,
                    gunRotation = new QuaternionPacket(),
                    position = _room.Attacker == player.team ? attackStartPos[index%5] : defenseStartPos[index%5],
                    rotation = new()
                });
                index++;
            }
            _room.Broadcast(updateLocations);
        }
    }
}
