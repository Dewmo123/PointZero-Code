using Scripts.Core;
using Scripts.Core.GameSystem;
using Scripts.Network;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Entities.Players.MyPlayers
{
    public class MyPlayerMovement : PlayerMovement
    {
        [SerializeField] private float gravity = -9.81f;

        protected PlayerInputSO _playerInput;
        private CharacterController _controller;
        public bool IsGround => _controller.isGrounded;

        private float _verticalVelocity;

        public bool IsSprint { get; protected set; }

        [SerializeField] private int walkSpeed;
        [SerializeField] private int sprintSpeed;
        [SerializeField] private int aimSpeed;
        [FormerlySerializedAs("sendInterval")]
        [SerializeField] private float sendInterval = 0.02f;

        private int _currentSpeed;
        private MyPlayer _myPlayer;
        protected Vector3 _direction;
        protected Vector3 _velocity;
        private MyPlayerAttackCompo _attackCompo;
        private float _sendTimer;

        public enum WalkMode
        {
            Idle,
            Walk,
            Sprint,
            Aim
        }

        public override void Initialize(NetworkEntity entity)
        {
            base.Initialize(entity);
            _myPlayer = (_player as MyPlayer);
            _playerInput = _myPlayer.PlayerInput;
            _myPlayer.OnAimEvent += HandleAim;
            _playerInput.OnSprintEvent += HandleSprint;
            _currentSpeed = walkSpeed;
            _attackCompo = entity.GetCompo<MyPlayerAttackCompo>();
            _controller = entity.GetComponent<CharacterController>();
            _myPlayer.OnDead.AddListener(StopImmediately);
            _sendTimer = sendInterval;
        }

        private void OnDestroy()
        {
            _playerInput.OnSprintEvent -= HandleSprint;

            _myPlayer.OnAimEvent -= HandleAim;
            _myPlayer.OnDead.RemoveListener(StopImmediately);
        }

        #region Movement
        protected void FixedUpdate()
        {
            CalculateMovement();
            CalculateRotation();
            ApplyGravity();
            Move();
            if (!_player.isTest)
                TrySendMyInfo();
        }

        private void ApplyGravity()
        {
            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = -0.03f;
            else
                _verticalVelocity += gravity * Time.fixedDeltaTime;
            _velocity.y = _verticalVelocity;
        }

        private void Move()
        {
            _controller.Move(_velocity);
        }

        protected void CalculateMovement()
        {
            _velocity = _direction * _currentSpeed * Time.fixedDeltaTime;
        }

        protected void CalculateRotation()
        {
            Quaternion rotation;
            if (IsAiming)
            {
                Vector3 dir = (_playerInput.GetWorldPosition() - transform.position).normalized;
                dir.y = 0;
                rotation = Quaternion.LookRotation(dir);
                float forwardDot = Vector3.Dot(model.forward, _direction);
                float rightDot = Vector3.Dot(model.right, _direction);
                _animator.SetParam(_xHash, rightDot);
                _animator.SetParam(_zHash, forwardDot);
            }
            else
            {
                rotation = _direction == Vector3.zero ? model.rotation
                   : Quaternion.LookRotation(_velocity);
            }
            float rotateSpeed = 20f;
            model.rotation = Quaternion.Lerp(model.rotation, rotation, Time.fixedDeltaTime * rotateSpeed);
        }
        #endregion

        private void TrySendMyInfo()
        {
            if (_player.isDead)
                return;

            _sendTimer += Time.fixedDeltaTime;
            float interval = sendInterval;
            if (_sendTimer < interval)
                return;

            _sendTimer = 0f;
            SendMyInfo();
        }

        private void SendMyInfo()
        {
            C_UpdateLocation info = new C_UpdateLocation()
            {
                location = new LocationInfoPacket()
                {
                    rotation = model.rotation.ToPacket(),
                    position = _player.transform.position.ToPacket(),
                    index = _player.Index,
                    gunRotation = _attackCompo.CurrentGun.transform.rotation.ToPacket(),
                    animHash = _myPlayer.CurrentAnimHash
                },
                isAiming = IsAiming,
            };

            NetworkManager.Instance.SendPacket(info);
        }

        #region SetParameters
        public void SetMoveState(WalkMode state)
         => _currentSpeed = state switch
         {
             WalkMode.Idle => 0,
             WalkMode.Walk => walkSpeed,
             WalkMode.Aim => aimSpeed,
             WalkMode.Sprint => sprintSpeed,
             _ => 0
         };

        public override void SetPosition(Vector3 position)
        {
            _controller.enabled = false;
            _player.transform.position = position;
            _controller.enabled = true;
        }

        public void SetMovement(Vector2 dir)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            _direction = camRight * dir.x + camForward * dir.y;
        }

        public void StopImmediately()
            => _direction = Vector3.zero;

        private void HandleSprint(bool obj)
            => IsSprint = obj;
        #endregion
    }
}
