using Scripts.Core.GameSystem;
using Scripts.Entities.Players.MyPlayers.Interact;
using Scripts.Network;
using System;

namespace Scripts.Entities.Players.States
{
    public class PlayerPlantState : PlayerState
    {
        private PlayerInputSO _playerInput;
        private PlayerInteractManager _interactManager;
        public PlayerPlantState(NetworkEntity entity, int animationHash) : base(entity, animationHash)
        {
            _playerInput = _player.PlayerInput;
            _interactManager = entity.GetCompo<PlayerInteractManager>();
        }
        public override void Enter()
        {
            base.Enter();
            _playerInput.OnPlantEvent += HandlePlant;
            _movement.StopImmediately();
            _interactManager.SetGaugeActive(true);
        }
        public override void Exit()
        {
            base.Exit();
            _playerInput.OnPlantEvent -= HandlePlant;
            _interactManager.SetGaugeActive(false);
        }
        private void HandlePlant(bool obj)
        {
            if (!obj)
            {
                _player.ChangeState("Idle");
            }
        }

        public override void Update()
        {
            base.Update();
            float fillAmount = _entityAnimator.GetAnimationProgress();
            _interactManager.SetGaugeUI("폭탄 설치중", fillAmount);
            if (_isTriggerCall)
            {
                C_PlantBomb bomb = new();
                bomb.area = (ushort)_player.GetCompo<PlayerPlant>().CurrentArea;
                NetworkManager.Instance.SendPacket(bomb);
                _player.ChangeState("Idle");
            }

        }
    }
}
