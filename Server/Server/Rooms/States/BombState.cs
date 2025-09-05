using Server.Utiles;
using System;

namespace Server.Rooms.States
{
    internal class BombState : GamingState
    {
        private CountTime _bombCount;
        public BombState(GameRoom room) : base(room)
        {
            _bombCount = new(HandleElapsed, HandleBomb, 100);
        }
        public override void Enter()
        {
            base.Enter();
            Console.WriteLine("BombState");
            _room.OnDefuse += HandleDefuse;
            _bombCount.SetEndTime(_room.bombTime);
            _bombCount.StartCount();
        }
        public override void Update()
        {
            base.Update();
            _bombCount.UpdateDeltaTime();
        }
        public override void Exit()
        {
            base.Exit();
            if (_bombCount.IsRunning)
                _bombCount.Abort(false);
            _room.OnDefuse -= HandleDefuse;
        }
        private void HandleDefuse()
        {
            _bombCount.Abort(false);
            _room.NextRound(_room.Defender);
        }

        private void HandleElapsed(double obj)
        {
            Console.WriteLine(_room.bombTime - obj);
        }

        private void HandleBomb()
        {
            _room.NextRound(_room.Attacker);
        }
    }
}
