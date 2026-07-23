using System;
using System.IO;
using UCS.Core;
using UCS.Logic;
using UCS.PacketProcessing;

namespace UCS.PacketProcessing.Messages.Client
{
    internal class StartWarSearchMessage : Message
    {
        public StartWarSearchMessage(UCS.PacketProcessing.Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            try { } catch (Exception) { }
        }

        public override void Process(Level level)
        {
            ClientAvatar avatar = level.GetPlayerAvatar();
            long allianceId = avatar.GetAllianceId();

            if (allianceId != 0)
            {
                Alliance alliance = ObjectManager.GetAlliance(allianceId);
                if (alliance != null)
                {
                    // 1. Aktifkan status War 1 Jam di Clan
                    int currentEpoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    alliance.IsInWar = true;
                    alliance.WarEndTime = currentEpoch + 3600; // +1 Jam

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[CLAN WARS] {avatar.GetAvatarName()} memicu War! Teleportasi ke arena Goblin!");
                    Console.ResetColor();

                    // 2. INSTANT TELEPORT KE GOBLIN MAP
                    try 
                    {
                        // 17000040 adalah ID untuk base Goblin level tinggi (sekitar level 41)
                        int warGoblinLevelId = 17000040; 
                        
                        // Pakai konstruktor baru yang kita pasang di NpcDataMessage
                        NpcDataMessage npcMessage = new NpcDataMessage(this.Client, level, warGoblinLevelId);
                        
                        // Cara resmi mengirim paket pesan ke HP pemain di UCS 0.4.1
                        PacketManager.ProcessOutgoingPacket(npcMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Error War Drop] Gagal melempar ke map Goblin: " + ex.Message);
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CLAN WARS] {avatar.GetAvatarName()} mencoba war tapi belum join Clan!");
                Console.ResetColor();
            }
        }
    }
}
