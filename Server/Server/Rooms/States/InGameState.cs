using Server.Objects;
using Server.Objects.Areas;
using Server.Utiles;
using System;
using System.Numerics;

namespace Server.Rooms.States
{
    class InGameState : GamingState
    {
        private CountTime _gameCount;
        private S_SyncTimer _syncTimer = new();
        private static readonly Vector3 bombSize = new Vector3(1, 1, 1);

        public InGameState(GameRoom room) : base(room)
        {
            _gameCount = new(HandleTimerElapsed, HandleTimerEnd, 100);
        }
        public override void Enter()
        {
            base.Enter();
            _room.OnPlant += HandlePlant;

            Console.WriteLine(_room.Attacker);
            _gameCount.SetEndTime(_room.playTime);
            _gameCount.StartCount();
        }
        public override void Update()
        {
            base.Update();
            _gameCount.UpdateDeltaTime();
        }
        private void HandlePlant(Vector3 plantPos)
        {
            S_CreateBombArea createBomb = new();
            BombArea bomb = new(plantPos, bombSize,_room);
            createBomb.bombInfo = (CubeInfo)bomb.CreatePacket();
            _room.bombArea = bomb;
            _room.Broadcast(createBomb);
            _room.ChangeState(RoomState.Bomb);
        }

        public override void Exit()
        {
            base.Exit();
            if (_gameCount.IsRunning)
                _gameCount.Abort(false);
            _room.OnPlant -= HandlePlant;
        }
        #region Timer
        private void HandleTimerEnd()
        {
            _room.NextRound(_room.Defender);
        }

        private void HandleTimerElapsed(double time)
        {
            _syncTimer.time = _room.playTime - (float)time;
            _room.Broadcast(_syncTimer);
        }
        #endregion
    }
}
