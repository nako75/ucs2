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
    // Packet 14404: Dieksekusi pas pemain klik tab Top Players
    internal class AskForTopPlayersListMessage : Message
    {
        public AskForTopPlayersListMessage(UCS.PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            try { } catch (Exception) { }
        }

        public override void Process(Level level)
        {
            // Ambil semua avatar di memori server, urutkan dari Trophy tertinggi (Max 100 pemain)
            List<ClientAvatar> topPlayers = ObjectManager.GetInMemoryAvatars()
                .OrderByDescending(a => a.GetScore())
                .Take(100)
                .ToList();

            // Pengaman: Kalau list kosong karena suatu hal, masukin akun kita sendiri
            if (topPlayers.Count == 0)
            {
                topPlayers.Add(level.GetPlayerAvatar());
            }

            GlobalPlayersMessage message = new GlobalPlayersMessage(this.Client);
            message.SetPlayers(topPlayers);
            PacketManager.ProcessOutgoingPacket(message);
        }
    }
}
