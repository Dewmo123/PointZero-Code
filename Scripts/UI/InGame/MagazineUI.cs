using Core.EventSystem;
using Scripts.Core.EventSystem;
using Scripts.Core.Managers;
using Scripts.Entities.Players;
using Scripts.Entities.Players.MyPlayers;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI.InGame
{
    public class MagazineUI : MonoBehaviour
    {
        private MyPlayerAttackCompo _attackCompo;
        [SerializeField] private EventChannelSO packetChannel;
        private TextMeshProUGUI _text;
        private bool _isBound;
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            packetChannel.AddListener<HandleEnterRoom>(EnterRoom);
            packetChannel.AddListener<HandleLeaveRoom>(LeaveRoom);
        }

        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleEnterRoom>(EnterRoom);
            packetChannel.RemoveListener<HandleLeaveRoom>(LeaveRoom);
            ReleaseAttackCompo();
        }

        private void EnterRoom(HandleEnterRoom room)
        {
            if (_isBound)
                return;

            PlayerManager playerManager = PlayerManager.Instance;
            if (playerManager == null || playerManager.MyPlayer == null)
                return;

            _attackCompo = playerManager.MyPlayer.GetCompo<MyPlayerAttackCompo>();
            if (_attackCompo == null)
                return;

            _attackCompo.OnAttack.AddListener(HandleAttack);
            _attackCompo.OnReloadComplete.AddListener(HandleAttack);
            _isBound = true;
            HandleAttack();
        }

        private void LeaveRoom(HandleLeaveRoom room)
        {
            ReleaseAttackCompo();
            _text.text = string.Empty;
        }

        private void ReleaseAttackCompo()
        {
            if (!_isBound || _attackCompo == null)
            {
                _attackCompo = null;
                _isBound = false;
                return;
            }

            _attackCompo.OnAttack.RemoveListener(HandleAttack);
            _attackCompo.OnReloadComplete.RemoveListener(HandleAttack);
            _attackCompo = null;
            _isBound = false;
        }

        private void HandleAttack()
        {
            if (_attackCompo == null)
                return;

            _text.text = $"{_attackCompo.CurrentGun.currentBulletCount}/{_attackCompo.CurrentGun.MaxBulletCount}";
        }
    }
}
