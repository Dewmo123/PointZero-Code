using UnityEngine;

namespace Scripts.Entities.Players.States
{
    public class PlayerAimWalkState : PlayerAimState
    {

        public PlayerAimWalkState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {

        }
        public override void Enter()
        {
            base.Enter();
            _movement.SetMoveState(MyPlayers.MyPlayerMovement.WalkMode.Aim);
        }
        public override void Update()
        {
            base.Update();
            Vector2 moveVec = _player.PlayerInput.MovementKey;
            _movement.SetMovement(moveVec);
            if (moveVec.magnitude == 0)
                _player.ChangeState("AimIdle");
        }
    }
}
