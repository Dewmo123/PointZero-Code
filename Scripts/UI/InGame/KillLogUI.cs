using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using UnityEngine;

namespace Scripts.UI.InGame
{
    public class KillLogUI : MonoBehaviour
    {
        [SerializeField] private GameObject attribute;
        [SerializeField] private EventChannelSO packetChannel;

        private void Awake()
        {
            packetChannel.AddListener<HandlePlayerDead>(SetKillLog);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandlePlayerDead>(SetKillLog);
        }
        private void SetKillLog(HandlePlayerDead dead)
        {
            KillLogAttribute att = Instantiate(attribute, transform).GetComponent<KillLogAttribute>();
            att.SetTexts(dead.attacker, dead.hitPlayer);
        }
    }
}
