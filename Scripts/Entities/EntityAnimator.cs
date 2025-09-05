using Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected Animator animator;

        private NetworkEntity _entity;
        public virtual void Initialize(NetworkEntity entity)
        {
            _entity = entity;
        }
        public float GetAnimationProgress()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        #region SetParam
        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);
        #endregion
    }
}
