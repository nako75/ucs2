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
            // Dalam protokol CoC 7.156, client ngirim stream hasil pertempuran saat End Battle
            // Kita coba decode jumlah bintang dan persentase destroy dari binary stream
            try 
            {
                // Di UCS 0.4.1, beberapa byte awal berisi tick timer dan info command
                br.ReadInt32(); // Tick / Timer
                br.ReadInt32(); // Padding / Unknown
                
                m_vStars = br.ReadInt32(); // Jumlah Bintang (0 sampai 3)
                m_vDamagePercentage = br.ReadInt32(); // Persentase Destroy (0 sampai 100)

                // Pengaman kalau offset memori bergeser supaya angka tidak aneh/minus
                if (m_vStars < 0 || m_vStars > 3) m_vStars = 1; 
                if (m_vDamagePercentage < 0 || m_vDamagePercentage > 100) m_vDamagePercentage = 50;
            } 
            catch (Exception) 
            {
                // Fallback aman jika format stream beda: anggap menang 1 bintang
                m_vStars = 1;
                m_vDamagePercentage = 50;
            }
        }

        public override void Execute(Level level)
        {
            ClientAvatar attacker = level.GetPlayerAvatar();
            
            // --- LOGIKA REAL PVP BATTLE (WIN / LOSS) ---
            int currentScore = attacker.GetScore(); 
            int baseTrophy = 30; // Hadiah trophy maksimal untuk 3 bintang
            int trophyChange = 0;

            // Hitung perubahan trophy berdasarkan jumlah bintang yang didapat
            if (m_vStars == 3)
            {
                trophyChange = baseTrophy;          // 3 Bintang = Full +30 Trophy
            }
            else if (m_vStars == 2)
            {
                trophyChange = (baseTrophy * 2) / 3; // 2 Bintang = +20 Trophy
            }
            else if (m_vStars == 1)
            {
                trophyChange = baseTrophy / 3;       // 1 Bintang = +10 Trophy
            }
            else
            {
                // 0 Bintang (Kalah / Surrender) = Minus Trophy!
                trophyChange = -15; 
            }
            
            int newScore = currentScore + trophyChange;
            if (newScore < 0) newScore = 0; // Trophy tidak boleh di bawah 0

            // KERENNYA: Karena di ClientAvatar.cs sebelumnya kita sudah pasang UpdateLeague() 
            // dan pengecekan All Time Best di dalam fungsi SetScore(),
            // memanggil SetScore di sini otomatis langsung ngupdate 3 fitur sekaligus!
            attacker.SetScore(newScore);
            
            // Efek visual di CMD/Console server biar lu tahu detil pertempurannya
            Console.ForegroundColor = (m_vStars > 0) ? ConsoleColor.Green : ConsoleColor.Red;
            string resultText = (m_vStars > 0) ? "MENANG" : "KALAH";
            string sign = (trophyChange >= 0) ? "+" : "";
            
            Console.WriteLine($"[Battle Result] {attacker.GetAvatarName()} {resultText} ({m_vStars} Star - {m_vDamagePercentage}%) | Trophy: {sign}{trophyChange} (Total: {newScore})");
            Console.ResetColor();

            // PENTING: Lock-in ke database! 
            // Karena database langsung di-save saat battle berakhir, pasukan yang mati/dideploy 
            // tidak akan balik lagi ke Army Camp saat pemain pulang ke desa!
            new DatabaseManager().Save(new List<Level> { level });
        }
    }
}
