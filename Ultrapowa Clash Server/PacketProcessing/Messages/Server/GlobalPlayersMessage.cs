using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Logic;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
    // Packet 24404: Server response untuk Top Players
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
                data.AddString(player.GetAvatarName());
                data.AddInt32(player.GetScore());       // Trophy sekarang
                data.AddInt32(rank++);                  // Ranking ke (1, 2, 3...)
                data.AddInt32(player.GetAvatarLevel()); // Level XP
                data.AddInt32(100);                     // Attack won (dummy)
                data.AddInt32(10);                      // Attack lost (dummy)
                data.AddInt32(player.GetScore());       // All time best trophy
                data.AddInt32(1);                       // Unknown padding
                data.AddInt32(player.GetLeagueId());    // Emblem Liga
                data.AddString("ID");                   // Bendera Negara (ID = Indonesia!)
                data.AddInt64(player.GetId());          // Avatar ID
                
                // Cek apakah pemain masuk Clan
                if (player.GetAllianceId() != 0)
                {
                    data.Add(1); // Has clan
                    data.AddInt64(player.GetAllianceId());
                    Alliance alliance = ObjectManager.GetAlliance(player.GetAllianceId());
                    if (alliance != null)
                    {
                        data.AddString(alliance.GetAllianceName());
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
                    data.Add(0); // Gak punya clan
                }
            }

            // Info penutupan season (Misal sisa 7 hari = 604800 detik)
            data.AddInt32(604800);
            data.AddInt32(2026);
            data.AddInt32(7);

            SetData(data.ToArray());
        }
    }
}
