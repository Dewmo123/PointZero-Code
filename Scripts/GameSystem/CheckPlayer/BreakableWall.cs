using Assets.Scripts.Combat;
using Scripts.Core.Managers;
using Scripts.Entities;
using UnityEngine.Events;

namespace Scripts.GameSystem.CheckPlayer
{
    public class BreakableWall : Area, IHittable
    {
        public int index => base.Index;
        public int Health { get; set; }
        public UnityEvent OnHit;
        public UnityEvent OnDead;

        public void Hit()
        {
            OnHit?.Invoke();
        }

        public void SetDead(NetworkEntity attacker)
        {
            OnDead?.Invoke();
            ObjectManager.Instance.RemoveObject(index);
        }
    }
}
