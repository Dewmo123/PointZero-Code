using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
	

	class Program
	{
		static void Main(string[] args)
		{
			// DNS (Domain Name System)
			IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry("akhge.duckdns.org").AddressList[0], 3303);

			Connector connector = new Connector();

			connector.Connect(endPoint, 
				() => { return SessionManager.Instance.Generate(); },
				1);

			while (true)
			{
			}
		}
	}
}
