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
            // Ambil semua Level yang aktif di memori server, ekstrak avatarnya, lalu urutkan dari Trophy tertinggi
            List<ClientAvatar> topPlayers = ObjectManager.GetLevels()
                .Values
                .Select(l => l.GetPlayerAvatar())
                .OrderByDescending(a => a.GetScore())
                .Take(100)
                .ToList();

            // Pengaman: Kalau list kosong, masukkan akun pemain yang sedang request
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
