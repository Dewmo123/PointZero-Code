using Scripts.Core.GameSystem;
using Scripts.UI;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers
{
    public class CameraPivot : MonoBehaviour, IEntityComponent
    {
        private PlayerInputSO _playerInput;
        private MyPlayer _player;
        [SerializeField] private Transform model;
        [SerializeField] private CinemachineCamera cam;
        [Range(0, 2)]
        [SerializeField] private float _smooth;
        [SerializeField] private Transform cameraParent;
        [SerializeField] private LensSettings _defaultLens;
        private Queue<bool> _aimingQueue = new();
        private Vector3 mouse;
        public void Initialize(NetworkEntity entity)
        {
            _player = entity as MyPlayer;
            _player.OnDead.AddListener(HandleDead);
            _playerInput = _player.PlayerInput;
            _playerInput.OnCameraRotateEvent += HandleAim;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void HandleDead()
        {
            _aimingQueue.Clear();
        }

        private void OnDestroy()
        {
            _playerInput.OnCameraRotateEvent -= HandleAim;
        }
        private async void LateUpdate()
        {
            _smooth = PlayerPrefs.GetFloat(SettingUI.SMOOTH_KEY);
            if (_aimingQueue.Count > 0)
            {
                Vector2 mouseDelta = _playerInput.MouseDelta * _smooth;
                mouse.y += mouseDelta.x;
                var lens = cam.Lens;
                lens.FieldOfView = Mathf.Clamp(lens.FieldOfView + 0.5f, _defaultLens.FieldOfView, 80);//하드코딩
                await Awaitable.NextFrameAsync();
                cam.Lens = lens;
                cameraParent.rotation = Quaternion.Euler(mouse);
            }
            else
            {
                LensSettings lens = cam.Lens;
                lens.FieldOfView = Mathf.Max(_defaultLens.FieldOfView, cam.Lens.FieldOfView - 0.5f);
                cam.Lens = lens;
            }
        }
        private void HandleAim(bool obj)
        {
            if (obj)
                _aimingQueue.Enqueue(obj);
            else
                _aimingQueue.Dequeue();
        }
    }
}
