using Server.Utiles;

namespace Server.Rooms.States
{
    internal class GameEndState : SyncObjectsState
    {

        private CountTime _endCount;
        private static readonly int _endTime = 5;
        public GameEndState(GameRoom room) : base(room)
        {
            _endCount = new CountTime(HandleElapsed, OnCountEnd, _endTime, 100);
        }
        private bool isEnd = false;
        public override void Enter()
        {
            base.Enter();
            _endCount.StartCount();
        }
        private void OnCountEnd()
        {
            S_LeaveRoom leavePacket = new();
            isEnd = true;
            ResetPacket();
            foreach (var item in _room.Sessions)
            {
                item.Value.Send(leavePacket.Serialize());
                item.Value.Room = null;
            }
            _room.AllPlayerExit();
        }
        public override void Update()
        {
            if (!isEnd)
            {
                base.Update();
                _endCount.UpdateDeltaTime();
            }
        }
        S_SyncTimer _timerPacket = new();
        private void HandleElapsed(double obj)
        {
            _timerPacket.time = (float)(_endTime - obj);
            _room.Broadcast(_timerPacket);
        }
    }
}
