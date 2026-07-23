using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Logic;
using UCS.PacketProcessing;
using UCS.Network;

namespace UCS.PacketProcessing.Messages.Client
{
    // Packet 14403: Dieksekusi pas pemain klik tab Top Clans
    internal class AskForTopClansListMessage : Message
    {
        public AskForTopClansListMessage(UCS.PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            try { } catch (Exception) { }
        }

        public override void Process(Level level)
        {
            // Ambil semua Clan di memori server, urutkan dari Trophy tertinggi (Max 100 clan)
            List<Alliance> topClans = ObjectManager.GetInMemoryAlliances()
                .OrderByDescending(a => a.GetScore())
                .Take(100)
                .ToList();

            GlobalAlliancesMessage message = new GlobalAlliancesMessage(this.Client);
            message.SetAlliances(topClans);
            PacketManager.ProcessOutgoingPacket(message);
        }
    }
}
