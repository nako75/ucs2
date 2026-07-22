using System;
using System.IO;
using System.Collections.Generic;
using UCS.Core;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    internal class EndAttackCommand : Command
    {
        private int m_vStars;
        private int m_vDamagePercentage;

        public EndAttackCommand(BinaryReader br)
        {
            try 
            {
                br.ReadInt32(); // Tick timer
                br.ReadInt32(); // Unknown / Padding
                m_vStars = br.ReadInt32(); // Bintang (0-3)
                m_vDamagePercentage = br.ReadInt32(); // Persentase (0-100)

                if (m_vStars < 0 || m_vStars > 3) m_vStars = 1; 
                if (m_vDamagePercentage < 0 || m_vDamagePercentage > 100) m_vDamagePercentage = 50;
            } 
            catch (Exception) 
            {
                m_vStars = 1;
                m_vDamagePercentage = 50;
            }
        }

        public override void Execute(Level level)
        {
            ClientAvatar attacker = level.GetPlayerAvatar();
            
            // --- 1. LOGIKA TROPHY PVP ---
            int currentScore = attacker.GetScore(); 
            int baseTrophy = 30;
            int trophyChange = 0;

            if (m_vStars == 3) trophyChange = baseTrophy;
            else if (m_vStars == 2) trophyChange = (baseTrophy * 2) / 3;
            else if (m_vStars == 1) trophyChange = baseTrophy / 3;
            else trophyChange = -15; // Kalah / Surrender
            
            int newScore = currentScore + trophyChange;
            if (newScore < 0) newScore = 0;
            attacker.SetScore(newScore);
            
            Console.ForegroundColor = (m_vStars > 0) ? ConsoleColor.Green : ConsoleColor.Red;
            string resultText = (m_vStars > 0) ? "MENANG" : "KALAH";
            string sign = (trophyChange >= 0) ? "+" : "";
            Console.WriteLine($"[Battle Result] {attacker.GetAvatarName()} {resultText} ({m_vStars} Star - {m_vDamagePercentage}%) | Trophy: {sign}{trophyChange} (Total: {newScore})");
            Console.ResetColor();

            // --- 2. FITUR WAR EVENT: HADIAH 5.000 GEMS JIKA RATA 3 BINTANG ---
            long allianceId = attacker.GetAllianceId();
            if (m_vStars == 3 && allianceId != 0)
            {
                Alliance alliance = ObjectManager.GetAlliance(allianceId);
                if (alliance != null && alliance.IsInWar)
                {
                    int currentEpoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    
                    // Cek apakah waktu war 1 jam (3600 detik) belum habis
                    if (currentEpoch < alliance.WarEndTime)
                    {
                        int currentGems = attacker.GetDiamonds();
                        attacker.SetDiamonds(currentGems + 5000); // Kucurkan 5.000 Gems!

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"[CLAN WARS REWARD] {attacker.GetAvatarName()} RATA 3 BINTANG di masa War! Klaim hadiah +5.000 Gems!");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Waktu 1 jam sudah lewat, matikan status war clan
                        alliance.IsInWar = false;
                        Console.WriteLine($"[CLAN WARS] Waktu War 1 jam untuk clan '{alliance.GetAllianceName()}' telah berakhir.");
                    }
                }
            }

            
}

            // PENTING: Lock-in ke database! 
            // Karena database langsung di-save saat battle berakhir, pasukan yang mati/dideploy 
            // tidak akan balik lagi ke Army Camp saat pemain pulang ke desa!
            new DatabaseManager().Save(new List<Level> { level });
        }
    }
}
