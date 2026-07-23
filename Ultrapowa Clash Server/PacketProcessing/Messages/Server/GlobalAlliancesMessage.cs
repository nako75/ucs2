using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Logic;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
    class GlobalAlliancesMessage : Message
    {
        private List<Alliance> m_vAlliances;

        public GlobalAlliancesMessage(Client client) : base(client)
        {
            SetMessageType(24403);
            m_vAlliances = new List<Alliance>();
        }

        public void SetAlliances(List<Alliance> alliances)
        {
            m_vAlliances = alliances;
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();

            data.AddInt32(m_vAlliances.Count);
            int rank = 1;
            foreach (Alliance alliance in m_vAlliances)
            {
                string allianceName = alliance.GetAllianceName();
                if (string.IsNullOrEmpty(allianceName)) allianceName = "Clan";

                data.AddInt64(alliance.GetAllianceId());
                data.AddString(allianceName);
                data.AddInt32(rank++);                              // Ranking Clan
                data.AddInt32(alliance.GetScore());                 // Total Trophy Clan
                data.AddInt32(alliance.GetAllianceMembers().Count); // Jumlah Member
                data.AddInt32(alliance.GetAllianceBadgeData());     // Badge / Emblem Clan
                data.AddInt32(alliance.GetAllianceLevel());         // Level Clan
                data.AddString("ID");                               // Bendera (Indonesia)
            }

            // Season info
            data.AddInt32(604800);
            data.AddInt32(2026);
            data.AddInt32(7);

            SetData(data.ToArray());
        }
    }
}
