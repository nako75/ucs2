using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Logic;
using UCS.Helpers;
using UCS.Core;

namespace UCS.PacketProcessing
{
    //Packet 24334
    class AvatarProfileMessage : Message
    {
        private Level m_vLevel;

        public AvatarProfileMessage(Client client)
            : base(client)
        {
            SetMessageType(24334);
        }

        public override void Encode()
        {
            List<Byte> pack = new List<Byte>();
            ClientHome ch = new ClientHome(m_vLevel.GetPlayerAvatar().GetId());
            ch.SetHomeJSON(m_vLevel.SaveToJSON());

            pack.AddRange(m_vLevel.GetPlayerAvatar().Encode());
            pack.AddInt32(ch.GetHomeJSON().Length + 4);
            pack.AddInt32(unchecked((int)0xFFFF0000));
            pack.AddRange(ch.GetHomeJSON());

            // --- BAGIAN YANG GW PERBAIKI BREE ---
            ClientAvatar avatar = (ClientAvatar)m_vLevel.GetPlayerAvatar();
            
            // Mengganti 13 byte nol bawaan Ultrapowa dengan struktur data profil yang benar
            pack.AddInt32(0); // 1. Troops Donated 
            pack.AddInt32(0); // 2. Troops Received
            pack.AddInt32(0); // 3. War Stars Won
            pack.AddInt32(avatar.GetAllTimeBestScore()); // 4. <-- INI ALL TIME BEST-NYA MASUK SINI!
            pack.Add(0);      // 5. 1 Byte flag/padding terakhir biar klien nggak crash
            // ------------------------------------

            SetData(pack.ToArray());
        }

        public void SetLevel(Level level)
        {
            m_vLevel = level;
        }
    }
}
