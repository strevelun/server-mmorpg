using System;
using System.Collections.Generic;
using System.Text;
using ServerCore.Utility;

namespace Server
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		int _sessionId = 0;
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		object _lock = new object();

		private ObjectPool<ClientSession> _clientSessionPool = null;
		

		public void Init()
        {
			_clientSessionPool = new ObjectPool<ClientSession>(
				() => // generator
			{
				return new ClientSession();
			},
			1024,  // capacity
			null,  // onAlloc
			null); // onFree
        }

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = _clientSessionPool.Pop();
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);

				Console.WriteLine($"Connected : {sessionId}");

				return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				_sessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
                _clientSessionPool.Push(session);
            }
        }
	}
}
