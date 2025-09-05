using UnityEngine;

namespace Scripts.Entities.Players.States
{
    public class PlayerAimIdleState : PlayerAimState
    {
        public PlayerAimIdleState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _movement.StopImmediately();
            _movement.SetMoveState(MyPlayers.MyPlayerMovement.WalkMode.Idle);
        }
        public override void Update()
        {
            base.Update();
            if (_player.PlayerInput.MovementKey.magnitude > 0)
                _player.ChangeState("AimWalk");
        }
    }
}
