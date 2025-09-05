
using Assets.Scripts.Entities;
using Scripts.Entities;
using Scripts.Entities.Players;

namespace Blade.FSM
{
    public abstract class EntityState
    {
        protected NetworkEntity _entity;
        protected int _animationHash;
        protected EntityAnimator _entityAnimator;
        protected EntityAnimatorTrigger _animatorTrigger;
        protected bool _isTriggerCall;
        public int AnimHash => _animationHash;
        protected EntityState(NetworkEntity entity, int animationHash)
        {
            _entity = entity;
            _animationHash = animationHash;
            _entityAnimator = entity.GetCompo<PlayerAnimator>();
            _animatorTrigger = entity.GetCompo<EntityAnimatorTrigger>();
        }

        public virtual void Enter()
        {
            _entityAnimator.SetParam(_animationHash, true);
            _isTriggerCall = false;
            _animatorTrigger.OnAnimationEndTrigger += AnimationEndTrigger;
        }

        public virtual void Update(){ }

        public virtual void Exit()
        {
            _animatorTrigger.OnAnimationEndTrigger -= AnimationEndTrigger;
            _entityAnimator.SetParam(_animationHash, false);
        }

        public virtual void AnimationEndTrigger() => _isTriggerCall = true;
    }
}