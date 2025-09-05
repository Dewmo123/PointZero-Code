using Assets.Scripts.Combat;
using Core.EventSystem;
using Scripts.Core;
using Scripts.Core.EventSystem;
using Scripts.Core.Utiles;
using Scripts.Network;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Entities.Players
{
    public class Player : NetworkEntity, IHittable
    {
        public bool IsOwner { get; private set; } = false;
        public NotifyValue<string> Name;
        public NotifyValue<int> Health;
        public bool isDead => Health.Value <= 0;
        public UnityEvent OnDead;
        public UnityEvent OnHit;
        public UnityEvent OnRevive;
        public UnityEvent OnReload;
        [SerializeField] private EventChannelSO uiChannel;

        public bool isTest;
        public Team MyTeam { get; private set; }
        public bool IsReload { get; set; }

        public int index => Index;

        int IHittable.Health { get => Health.Value; set => Health.Value = value; }

        public virtual void Init(LocationInfoPacket packet, bool isOwner)
        {
            Index = packet.index;
            IsOwner = isOwner;
            transform.position = packet.position.ToVector3();
            Health = new();
            Name = new();
            Health.OnValueChanged += HandleHealthChange;

            base.InitEntity();
        }
        public void Reload()
        {
            if (!isDead)
                OnReload?.Invoke();
        }
        protected virtual void HandleHealthChange(int previousValue, int nextValue)
        {
            if (previousValue > nextValue)
            {
                OnHit?.Invoke();
            }
            else if (nextValue == 100)
            {
                OnRevive?.Invoke();
            }
        }
        public void SetDead(NetworkEntity attacker)
        {
            OnDead?.Invoke();
            var evt = UIEvents.HandlePlayerDead;
            evt.attacker = attacker as Player;
            evt.hitPlayer = this;
            uiChannel.InvokeEvent(evt);
        }
        public virtual void SetTeam(ushort val) => MyTeam = (Team)val;

        public void Hit()
        {
        }
    }
}
