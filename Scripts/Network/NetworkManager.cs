using Core.EventSystem;
using DummyClient;
using Scripts.Network.Packets;
using ServerCore;
using System;
using System.Net;
using UnityEngine;

namespace Scripts.Network
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        private static NetworkManager _instance;
        public static NetworkManager Instance => _instance;

        private Connector _connector;
        private ServerSession _session;
        private PacketQueue _packetQueue;
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            _connector = new Connector();
            _packetQueue = new PacketQueue(packetChannel);
            try
            {
                IPAddress ip = Dns.GetHostEntry("akhge.duckdns.org").AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(ip, 3303);
                _connector.Connect(endPoint, () => _session = new ServerSession(_packetQueue), 1);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        private void OnDestroy()
        {
            _session.Disconnect();
            _packetQueue.Clear();
        }
        private void OnApplicationQuit()
        {
            _session.Disconnect();
            _packetQueue.Clear();
        }
        public void SendPacket(IPacket packet)
            => _session.Send(packet.Serialize());

        private void Update()
        {
            _packetQueue.FlushPackets(_session);
        }
    }
}