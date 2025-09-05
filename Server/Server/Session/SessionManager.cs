using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;

        ConcurrentDictionary<int, ClientSession> _sessions = new ConcurrentDictionary<int, ClientSession>();

        public ClientSession Generate()
        {
            int sessionId = Interlocked.Increment(ref _sessionId);
            ClientSession session = new ClientSession();
            session.SessionId = sessionId;
            _sessions.TryAdd(sessionId, session);

            Console.WriteLine($"Connected : {sessionId}");

            return session;
        }

        public ClientSession Find(int id)
        {
            ClientSession session = null;
            _sessions.TryGetValue(id, out session);
            return session;
        }

        public void Remove(ClientSession session)
        {
            _sessions.Remove(session.SessionId, out session);
        }
        public void BroadcastAll(IPacket packet)
        {
            foreach (var item in _sessions)
                item.Value.Send(packet.Serialize());
        }
    }
}
