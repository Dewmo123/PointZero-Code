using Blade.FSM;
using Scripts.Core;
using Scripts.Core.GameSystem;
using System;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers
{
    public class MyPlayer : Player
    {
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StateDataSO[] states;
        private EntityStateMachine _stateMachine;
        public int CurrentAnimHash => _stateMachine.CurrentState.AnimHash;
        public Action<bool> OnAimEvent;
        private void Awake()
        {
            if (isTest)
                Init(new LocationInfoPacket() { position = transform.position.ToPacket(), index = 1, rotation = transform.rotation.ToPacket() }, true );
        }
        public override void Init(LocationInfoPacket packet, bool isOwner)
        {
            base.Init(packet, isOwner);
            _stateMachine = new EntityStateMachine(this, states);
            ChangeState("Idle");
            PlayerInput.OnAimEvent += HandleAim;
        }

        protected override void HandleHealthChange(int previousValue, int nextValue)
        {
            base.HandleHealthChange(previousValue,nextValue);
            if (nextValue <= 0)
                ChangeState("Dead");
            else if (nextValue == 100)//MaxHealth
                ChangeState("Idle");
        }
        private void OnDestroy()
        {
            PlayerInput.OnAimEvent -= HandleAim;
            _stateMachine.CurrentState?.Exit();
        }
        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        private void HandleAim(bool obj)
        {
            if (obj && !isDead)
            {
                ChangeState("AimIdle");
            }
            if (!isDead)
            {
                OnAimEvent?.Invoke(obj);
            }
        }

        public void ChangeState(string newStateName) => _stateMachine.ChangeState(newStateName);
        public override void SetTeam(ushort val)
        {
            base.SetTeam(val);

        }
    }
}
