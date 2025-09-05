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
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            packetChannel.AddListener<HandleEnterRoom>(EnterRoom);
        }

        private void EnterRoom(HandleEnterRoom room)
        {
            _attackCompo = PlayerManager.Instance.MyPlayer.GetCompo<MyPlayerAttackCompo>();
            _attackCompo.OnAttack.AddListener(HandleAttack);
            _attackCompo.OnReloadComplete.AddListener(HandleAttack);
            HandleAttack();
        }

        private void HandleAttack()
        {
            _text.text = $"{_attackCompo.CurrentGun.currentBulletCount}/{_attackCompo.CurrentGun.MaxBulletCount}";
        }
    }
}
