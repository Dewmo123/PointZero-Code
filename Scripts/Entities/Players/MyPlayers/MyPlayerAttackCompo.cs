using Assets.Scripts.Combat;
using Assets.Scripts.Entities;
using Scripts.Core;
using Scripts.Core.GameSystem;
using Scripts.Entities.Combat;
using Scripts.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Entities.Players.MyPlayers
{
    public class MyPlayerAttackCompo : MonoBehaviour, IEntityComponent
    {

        private PlayerInputSO playerInput;

        [SerializeField] private LineRenderer line;
        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private List<Gun> guns;

        public UnityEvent OnAttack;
        public UnityEvent OnReloadComplete;

        private Gun _currentGun;
        private Vector3 _direction;
        private Coroutine _shooting;
        private float _lastAttackTime;
        #region Props
        public Gun CurrentGun => _currentGun;
        private Transform firePos => _currentGun.FirePos;
        private float attackDelay => _currentGun.attackDelay;
        #endregion
        private MyPlayer _player;
        private EntityAnimatorTrigger _trigger;
        public void Initialize(NetworkEntity entity)
        {
            _currentGun = guns[0];
            _player = entity as MyPlayer;
            _player.OnAimEvent += HandleAim;
            _player.OnDead.AddListener(HandleDead);
            _player.PlayerInput.OnReloadEvent += HandleReloadEvent;
            playerInput = _player.PlayerInput;
            _trigger = _player.GetCompo<EntityAnimatorTrigger>();
            _trigger.OnReloadEndTrigger += HandleReloadEnd;
            line.transform.parent = null;
            line.transform.position = Vector3.zero;
            _direction = Vector3.one;
        }

        private void HandleDead()
        {
            if (_shooting != null)
                StopCoroutine(_shooting);
            line.gameObject.SetActive(false);
        }

        private void HandleReloadEvent()
        {
            C_Reload reload = new();
            NetworkManager.Instance.SendPacket(reload);
        }

        private void HandleReloadEnd()
        {
            _currentGun.Reload();
            OnReloadComplete?.Invoke();
        }

        private void OnDestroy()
        {
            _player.OnAimEvent -= HandleAim;
            _trigger.OnReloadEndTrigger -= HandleReloadEnd;
            if (_shooting != null)
                StopCoroutine(_shooting);
        }
        public void HandleAttack(bool obj)
        {
            if (obj && _currentGun.currentBulletCount > 0 && !_player.IsReload)
            {
                if (Time.time - _lastAttackTime >= attackDelay)
                    _shooting = StartCoroutine(Shoot());
            }
            else if (!obj && _shooting != null)
                StopCoroutine(_shooting);
        }

        private IEnumerator Shoot()
        {
            while (true)
            {
                if (_currentGun.currentBulletCount <= 0 || _player.IsReload)
                    break;
                _lastAttackTime = Time.time;
                Instantiate(bulletPrefab, firePos.position, Quaternion.LookRotation(_direction));
                _currentGun.Shoot();
                OnAttack?.Invoke();
                SendAttackReq();
                yield return _currentGun.Wait;
            }
        }

        private void SendAttackReq()
        {
            var ray = Physics.Raycast(firePos.position - firePos.forward, _direction, out RaycastHit hit, 123, _wallLayer);
            C_ShootReq req = new C_ShootReq();
            req.firePos = firePos.position.ToPacket();
            req.direction = _direction.ToPacket();
            if (ray && hit.collider.TryGetComponent(out IHittable hittable))
            {
                req.hitObjIndex = hittable.index;
                hittable.Hit();
            }
            else
                req.hitObjIndex = 0;
            NetworkManager.Instance.SendPacket(req);
        }

        private void HandleAim(bool obj)
        {
            if (obj)
                line.gameObject.SetActive(true);
            else
                line.gameObject.SetActive(false);
        }
        public void LookCameraPos()
        {
            Ray ray = playerInput.GetCameraRay();

            float y = _currentGun.transform.position.y;
            float t = (ray.origin.y - y) / ray.direction.y;
            Vector3 hitPoint = ray.origin - ray.direction * t;

            Vector3 dir = hitPoint - _currentGun.transform.position;
            dir.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(dir);
            if (dir != Vector3.zero)
            {
                Quaternion slerp = Quaternion.Slerp(_currentGun.transform.rotation, rotation, Time.fixedDeltaTime * 20);
                _currentGun.transform.rotation = slerp;
                _direction = Quaternion.Slerp(Quaternion.LookRotation(_direction), rotation, Time.fixedDeltaTime * 20) * Vector3.forward;
            }
        }

        Vector3[] positions = new Vector3[2];
        public void SetLine()
        {
            positions[0] = firePos.position;
            if (Physics.Raycast(_currentGun.transform.position, _direction, out RaycastHit ray, 100, _wallLayer))
                positions[1] = ray.point;
            else
                positions[1] = firePos.position;
            line.SetPositions(positions);
        }
    }
}
