using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Logic;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
    class GlobalPlayersMessage : Message
    {
        private List<ClientAvatar> m_vPlayers;

        public GlobalPlayersMessage(Client client) : base(client)
        {
            SetMessageType(24404);
            m_vPlayers = new List<ClientAvatar>();
        }

        public void SetPlayers(List<ClientAvatar> players)
        {
            m_vPlayers = players;
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();

            data.AddInt32(m_vPlayers.Count);
            int rank = 1;
            foreach (ClientAvatar player in m_vPlayers)
            {
                string name = player.GetAvatarName();
                if (string.IsNullOrEmpty(name)) name = "Chief";

                data.AddString(name);
                data.AddInt32(player.GetScore());       // Trophy sekarang
                data.AddInt32(rank++);                  // Ranking (1, 2, 3...)
                data.AddInt32(player.GetAvatarLevel()); // Level XP
                data.AddInt32(100);                     // Attacks Won
                data.AddInt32(10);                      // Attacks Lost
                data.AddInt32(player.GetScore());       // All Time Best Trophy
                data.AddInt32(0);                       // Padding
                data.AddInt32(player.GetLeagueId());    // League ID
                data.AddString("ID");                   // Region / Bendera (Indonesia)
                data.AddInt64(player.GetId());          // Avatar ID
                
                // FIX CRASH: Gunakan Int32 (4 bytes) untuk flag Clan, bukan 1 byte!
                if (player.GetAllianceId() != 0)
                {
                    data.AddInt32(1); // 1 = Punya Clan
                    data.AddInt64(player.GetAllianceId());
                    Alliance alliance = ObjectManager.GetAlliance(player.GetAllianceId());
                    if (alliance != null)
                    {
                        data.AddString(alliance.GetAllianceName() ?? "Clan");
                        data.AddInt32(alliance.GetAllianceBadgeData());
                    }
                    else
                    {
                        data.AddString("No Clan");
                        data.AddInt32(0);
                    }
                }
                else
                {
                    data.AddInt32(0); // 0 = Tidak punya Clan
                }
            }

            // Season info
            data.AddInt32(604800); // Sisa waktu season (detik)
            data.AddInt32(2026);
            data.AddInt32(7);

            SetData(data.ToArray());
        }
    }
}
