using Server.Objects.Areas;
using Server.Utiles;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Rooms.States
{
    internal class BetweenState : GamingState
    {
        private CountTime _countBetween;
        private S_SyncTimer _syncTimer = new();
        private static readonly int _betweenTime = 5;
        public BetweenState(GameRoom room) : base(room)
        {
            _countBetween = new(HandleElapsed, HandleTimerEnd, _betweenTime, 100);
        }
        public override void Enter()
        {
            base.Enter();
            _countBetween.StartCount();
        }
        private void HandleElapsed(double obj)
        {
            _syncTimer.time = _betweenTime - (float)obj;
            _room.Broadcast(_syncTimer);
        }
        public override void Update()
        {
            base.Update();
            _countBetween.UpdateDeltaTime();
        }
        public override void Exit()
        {
            base.Exit();
            if (_countBetween.IsRunning)
                _countBetween.Abort(false);
            var doors = _room.GetObjects<Door>();
            foreach (var door in doors)
                door.Status = DoorStatus.Close;
            var walls = _room.GetObjects<BreakableWall>();
            foreach (var item in walls)
                item.Revive();
            _room.BroadcastInit();
        }
        private void HandleTimerEnd()
        {
            _room.ChangeState(RoomState.Prepare);
        }
    }
}
