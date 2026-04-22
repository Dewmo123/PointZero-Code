using Core.EventSystem;
using DewmoLib.Dependencies;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.EventSystem
{
    [Provide]
    public class PacketResponsePublisher : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private EventChannelSO packetChannel;
        private Dictionary<PacketID, Action<bool>> _events = new();
        private void Awake()
        {
            packetChannel.AddListener<HandlePacketResponse>(HandleResponse);
        }

        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandlePacketResponse>(HandleResponse);
        }

        private void HandleResponse(HandlePacketResponse response)
        {
            if (_events.ContainsKey(response.packetId))
                _events[response.packetId]?.Invoke(response.success);
        }

        public void AddListener(PacketID id, Action<bool> handler)
        {
            if (_events.ContainsKey(id))
                _events[id] += handler;
            else
                _events[id] = handler;
        }
        public void RemoveListener(PacketID id, Action<bool> handler)
        {
            if (!_events.TryGetValue(id, out Action<bool> listeners))
                return;

            listeners -= handler;
            if (listeners == null)
                _events.Remove(id);
            else
                _events[id] = listeners;
        }
    }
}
