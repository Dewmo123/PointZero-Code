using UnityEngine;

namespace Scripts.Entities.Players.States
{
    public class PlayerWalkState : PlayerState
    {
        public PlayerWalkState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _movement.SetMoveState(MyPlayers.MyPlayerMovement.WalkMode.Walk);
        }
        public override void Update()
        {
            base.Update();
            Vector2 moveVec = _player.PlayerInput.MovementKey;
            _movement.SetMovement(moveVec);
            if (moveVec.magnitude == 0)
                _player.ChangeState("Idle");
            if (_movement.IsSprint)
                _player.ChangeState("Sprint");
        }
    }
}
