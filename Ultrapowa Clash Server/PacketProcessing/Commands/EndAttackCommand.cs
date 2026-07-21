using System;
using System.IO;
using UCS.Core;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    // Command ID: 603 (Dieksekusi pas pemain pencet End Battle / waktu perang habis)
    public class EndAttackCommand : Command
    {
        public EndAttackCommand(BinaryReader br) : base(br)
        {
        }

        public override void Decode(BinaryReader br)
        {
            try 
            {
                // Di sini sebenarnya client COC ngirim stream data sisa waktu, 
                // jumlah bintang, destroy percentage, dan loot yang didapat.
                // Untuk sementara kita jalankan base-nya dulu supaya gak crash saat baca memori.
                base.Decode(br);
            } 
            catch (Exception ex) 
            {
                Logger.Write("Error decoding EndAttackCommand: " + ex.Message);
            }
        }

        public override void Execute(Level level)
        {
            ClientAvatar attacker = level.GetPlayerAvatar();
            
            // --- LOGIKA FITUR 1: BATTLE SCORE (TROPHY SYSTEM) ---
            int currentScore = attacker.GetScore(); 
            
            // SIMULASI TAMBAH TROPHY SAAT MENANG / END BATTLE
            // Nanti kalau mau lebih canggih, nilai 25 ini bisa kita ubah 
            // berdasarkan jumlah bintang (1 bintang = +10, 3 bintang = +30, 0 bintang = -15)
            int trophyGain = 25; 
            
            int newScore = currentScore + trophyGain;
            if (newScore < 0) newScore = 0; // Trophy gak boleh di bawah 0

            attacker.SetScore(newScore);
            
            // Efek visual di CMD/Console server biar lu tau fiturnya jalan
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Battle Score] " + attacker.GetAvatarName() + " selesai PvP! Trophy naik jadi: " + newScore);
            Console.ResetColor();

            // PENTING: Save data ke database (SQLite/MySQL) biar pas relog trophy gak hilang
            DatabaseManager.Save(level);
        }
    }
}