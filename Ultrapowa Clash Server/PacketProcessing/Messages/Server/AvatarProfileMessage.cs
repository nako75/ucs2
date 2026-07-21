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

            SetData(pack.ToArray());
        }

        public void SetLevel(Level level)
        {
            m_vLevel = level;
        }
    }
}
