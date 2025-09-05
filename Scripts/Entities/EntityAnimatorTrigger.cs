using Scripts.Entities;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Entities
{
    public class EntityAnimatorTrigger : MonoBehaviour, IEntityComponent
    {
        public event Action OnAnimationEndTrigger;
        public event Action OnAttackTrigger;
        public event Action OnReloadEndTrigger;
        public UnityEvent OnStepTrigger;
        public void Initialize(NetworkEntity entity)
        {
        }
        public void OnReloadEnd()
        {
            OnReloadEndTrigger?.Invoke();
        }
        public void AnimationEnd()
        {
            OnAnimationEndTrigger?.Invoke();
        }
        public void AttackTrigger()
        {
            OnAttackTrigger?.Invoke();
        }
        public void StepTrigger()
        {
            OnStepTrigger?.Invoke();
        }
    }
}
