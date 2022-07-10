using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler : Worker
{
	private static PacketHandler instance = new PacketHandler();
	public static PacketHandler Instance { get { return instance; } }

	public override void Execute()
    {
		var producer = base.Pop();

		if (producer != null)
        {
			var recvBuffer = producer as RecvBuffer;
			var packetSession = recvBuffer.Session as PacketSession;
			PacketManager.Instance.OnRecvPacket(packetSession, recvBuffer.ReadSegment);
			RecvBuffer.Push(recvBuffer);
        }
    }

	public static void C_ChatHandler(PacketSession session, IMessage packet)
	{
		C_Chat chatPacket = packet as C_Chat;
		ClientSession serverSession = session as ClientSession;

		Console.WriteLine(chatPacket.Context);
	}
}
