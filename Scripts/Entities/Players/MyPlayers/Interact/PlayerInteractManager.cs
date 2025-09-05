using Core.EventSystem;
using Scripts.Core.EventSystem;
using Scripts.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Entities.Players.MyPlayers.Interact
{
    public class PlayerInteractManager : MonoBehaviour, IEntityComponent
    {
        private MyPlayer _myPlayer;
        private Team _attacker, _defender;
        private Dictionary<Role, PlayerPlantBase> _interactActions;
        [SerializeField] private EventChannelSO uiChannel;
        public bool isBombPlant { get; set; }
        private bool isPlant;
        public void Initialize(NetworkEntity entity)
        {
            _myPlayer = entity as MyPlayer;
            _interactActions = new();
            _myPlayer.PlayerInput.OnPlantEvent += HandlePlant;
            GetComponentsInChildren<PlayerPlantBase>().ToList().ForEach(item => _interactActions.Add(item.Role, item));
        }
        private void OnDestroy()
        {
            _myPlayer.PlayerInput.OnPlantEvent -= HandlePlant;
        }

        private void HandlePlant(bool obj)
        {
            if (obj)
            {
                if (_myPlayer.MyTeam == _attacker && !isBombPlant)
                    _interactActions[Role.Attack].HandlePlant();
                else if (_myPlayer.MyTeam == _defender && isBombPlant)
                    _interactActions[Role.Defend].HandlePlant();
            }
        }
        public void SetGaugeUI(string text, float percent)
        {
            var evt = UIEvents.SetGaugeEvent;
            evt.text = text;
            evt.fillAmount = percent;
            uiChannel.InvokeEvent(evt);
        }
        public void SetGaugeActive(bool active)
        {
            var evt = UIEvents.SetGaugeActiveEvent;
            evt.active = active;
            uiChannel.InvokeEvent(evt);
        }
        public void SetRole(Team attacker, Team defender)
        {
            _attacker = attacker;
            _defender = defender;
        }
    }
}
