using Scripts.Network.Packets;
using ServerCore;
using System;
using System.Net;
using UnityEngine;

namespace DummyClient
{
    public class ServerSession : PacketSession
    {
        private PacketQueue _packetQueue;
        public ServerSession(PacketQueue packetQueue)
        {
            _packetQueue = packetQueue;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            C_RoomList pak = new C_RoomList();
            Send(pak.Serialize());
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            Debug.Log("Recv");
            _packetQueue.Push(buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("SEND");
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
