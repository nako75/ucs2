using System;
using System.IO;
using System.Collections.Generic;
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
            // Ambil avatar pemain yang sedang aktif dan masukkan sebagai Top Player pertama
            List<ClientAvatar> topPlayers = new List<ClientAvatar>();
            ClientAvatar currentAvatar = level.GetPlayerAvatar();
            
            if (currentAvatar != null)
            {
                topPlayers.Add(currentAvatar);
            }

            GlobalPlayersMessage message = new GlobalPlayersMessage(this.Client);
            message.SetPlayers(topPlayers);
            PacketManager.ProcessOutgoingPacket(message);
        }
    }
}
