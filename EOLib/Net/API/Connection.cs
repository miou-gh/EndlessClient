﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

namespace EOLib.Net.API
{
	partial class PacketAPI
	{
		/// <summary>
		/// Confirms initialization with server
		/// </summary>
		/// <param name="SendMulti">Multiplier for send (encrypt multi)</param>
		/// <param name="RecvMulti">Multiplier for recv (decrypt multi)</param>
		/// <param name="clientID">Connection identifier</param>
		/// <returns>True on successful send operation</returns>
		public bool ConfirmInit(byte SendMulti, byte RecvMulti, short clientID)
		{
			if (!m_client.ConnectedAndInitialized)
				return false;

			OldPacket confirm = new OldPacket(PacketFamily.Connection, PacketAction.Accept);
			confirm.AddShort(SendMulti);
			confirm.AddShort(RecvMulti);
			confirm.AddShort(clientID);

			if (m_client.SendPacket(confirm))
			{
				Initialized = true;
				return true;
			}

			return false;
		}

		private void _createConnectionMembers()
		{
			m_client.AddPacketHandler(new FamilyActionPair(PacketFamily.Connection, PacketAction.Player), _handleConnectionPlayer, false);
		}

		private void _handleConnectionPlayer(OldPacket pkt)
		{
			var seq_1 = pkt.GetShort();
			var seq_2 = pkt.GetChar();
			m_client.UpdateSequence(seq_1, seq_2);

			var reply = new OldPacket(PacketFamily.Connection, PacketAction.Ping);
			m_client.SendPacket(reply);
		}
	}
}
