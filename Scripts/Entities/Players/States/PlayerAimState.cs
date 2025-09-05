using Assets.Scripts.Entities;
using Scripts.Entities.Players.MyPlayers;

namespace Scripts.Entities.Players.States
{
    public abstract class PlayerAimState : PlayerState
    {
        private MyPlayerAttackCompo _attackCompo;
        public PlayerAimState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<MyPlayerAttackCompo>();
        }
        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnAttackEvent += _attackCompo.HandleAttack;
            _player.OnAimEvent += HandleAim;
        }
        public override void Update()
        {
            base.Update();
            _attackCompo.LookCameraPos();
            _attackCompo.SetLine();
        }
        public override void Exit()
        {
            _player.PlayerInput.OnAttackEvent -= _attackCompo.HandleAttack;
            _player.OnAimEvent -= HandleAim;
            base.Exit();
        }
        private void HandleAim(bool obj)
        {
            if (obj == false)
            {
                _attackCompo.HandleAttack(false);
                _player.ChangeState("Idle");
            }
        }

    }
}
