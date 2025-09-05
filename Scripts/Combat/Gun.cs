using Scripts.Core.GameSystem;
using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Entities.Combat
{
    public class Gun : MonoBehaviour
    {
        [field: SerializeField] public Transform FirePos { get; private set; }
        [field: SerializeField] public float attackDelay { get; private set; } = 0.2f;
        private WaitForSeconds _wait;
        public WaitForSeconds Wait => _wait ??= new WaitForSeconds(attackDelay);
        public int currentBulletCount { get; private set; }
        public int MaxBulletCount;
        private void Awake()
        {
            Reload();
        }
        public void Reload()
        {
            currentBulletCount = MaxBulletCount;
        }
        public void Shoot()
        {
            currentBulletCount = Mathf.Max(currentBulletCount - 1, 0);
        }
    }
}
