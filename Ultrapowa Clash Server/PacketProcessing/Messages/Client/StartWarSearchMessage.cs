using System;
using System.IO;
using UCS.Core;
using UCS.Logic;

namespace UCS.PacketProcessing.Messages.Client
{
    // ID Paket: 14331 (Dieksekusi pas Leader/Co-Leader klik tombol Start War)
    internal class StartWarSearchMessage : Message
    {
        public StartWarSearchMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            // Client ngirim ID clan dan parameter search, kita tangkap dengan try-catch agar aman
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
                    // Aktifkan status war dan set durasi 1 Jam (3600 detik dari waktu sekarang)
                    int currentEpoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    alliance.IsInWar = true;
                    alliance.WarEndTime = currentEpoch + 3600; // +1 Jam

                    // Efek visual di Console Server
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[CLAN WARS EVENT] Clan '{alliance.GetAllianceName()}' memulai War 1 Jam! Target: PvE Goblin Map.");
                    Console.ResetColor();
                }
            }
        }
    }
}
