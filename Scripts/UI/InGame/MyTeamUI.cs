using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.InGame
{
    public class MyTeamUI : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        private TextMeshProUGUI _text;
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            packetChannel.AddListener<HandleMyTeam>(SetMyTeam);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleMyTeam>(SetMyTeam);
        }

        private void SetMyTeam(HandleMyTeam team)
        {
            _text.text = $"내 팀: {team.myTeam.ToString()}";
        }
    }
}
