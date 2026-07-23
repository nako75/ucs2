using System;
using System.IO;
using UCS.Core;
using UCS.Logic;
using UCS.PacketProcessing; // <-- Penting buat ngirim data map Goblin

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
                    Console.WriteLine($"[CLAN WARS] {avatar.GetAvatarName()} memicu War! Melempar langsung ke arena Raja Goblin!");
                    Console.ResetColor();

                    // 2. TRIK INSTANT TELEPORT KE PVE GOBLIN!
                    // Begitu tombol war ditekan, kita langsung kirim paket base Goblin ke HP pemain.
                    // Layar HP pemain bakal otomatis loading awan putih dan langsung masuk ke battle!
                    try 
                    {
                        // ID 17000050 adalah level tertinggi Goblin (Sherbet Towers / Raja Goblin)
                        // Kalau mau level awal, ganti jadi 17000001
                        int warGoblinLevelId = 17000050; 
                        
                        NpcLevel npcLevel = ObjectManager.GetNpcLevel(warGoblinLevelId);
                        if (npcLevel != null)
                        {
                            NpcDataMessage npcMessage = new NpcDataMessage(this.Client, npcLevel);
                            npcMessage.Send();
                        }
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
