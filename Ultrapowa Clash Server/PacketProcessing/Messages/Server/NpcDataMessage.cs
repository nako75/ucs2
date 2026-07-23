using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Logic;
using UCS.Core;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
    //Packet 24133
    class NpcDataMessage : Message
    {
        // 1. Konstruktor Asli Bawaan UCS (Jangan dihapus biar fitur lain gak error)
        public NpcDataMessage(Client client, Level level, AttackNpcMessage cnam) : base (client)
        {
            SetMessageType(24133);
            this.Player = level;
            JsonBase = ObjectManager.NpcLevels[(int)cnam.LevelId - 0x01036640];
            LevelId = cnam.LevelId;
        }

        // 2. KONSTRUKTOR BARU KHUSUS TELEPORTASI CLAN WAR!
        public NpcDataMessage(Client client, Level level, int customLevelId) : base(client)
        {
            SetMessageType(24133);
            this.Player = level;
            this.LevelId = customLevelId;
            
            // Pengaman array supaya tidak error OutOfBounds jika ID meleset
            int index = customLevelId - 0x01036640;
            if (index < 0 || index >= ObjectManager.NpcLevels.Count)
            {
                index = 0; // Fallback ke map Goblin pertama jika index kelebihan
            }
            this.JsonBase = ObjectManager.NpcLevels[index];
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();

            data.AddInt32(0);
            data.AddInt32(JsonBase.Length);
            data.AddRange(System.Text.Encoding.ASCII.GetBytes(JsonBase));
            data.AddRange(Player.GetPlayerAvatar().Encode());
            data.AddInt32(0);
            data.AddInt32(LevelId);

            SetData(data.ToArray());
        }

        public String JsonBase { get; set; }
        public int LevelId { get; set; }
        public Level Player { get; set; }
    }
}
