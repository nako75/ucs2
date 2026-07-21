using System;
using System.IO;
using System.Collections.Generic;
using UCS.Core;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    internal class EndAttackCommand : Command
    {
        // Hapus : base(br) supaya gak memicu error CS1729
        public EndAttackCommand(BinaryReader br)
        {
        }

        public override void Execute(Level level)
        {
            ClientAvatar attacker = level.GetPlayerAvatar();
            
            // --- LOGIKA FITUR 1: BATTLE SCORE (TROPHY SYSTEM) ---
            int currentScore = attacker.GetScore(); 
            int trophyGain = 25; 
            
            int newScore = currentScore + trophyGain;
            if (newScore < 0) newScore = 0;

            // Fungsi ini bisa dipanggil setelah lu tambahin kodenya di ClientAvatar.cs (Poin 4)
            attacker.SetScore(newScore);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Battle Score] " + attacker.GetAvatarName() + " selesai PvP! Trophy naik jadi: " + newScore);
            Console.ResetColor();

            // Dibungkus pakai List supaya gak memicu error CS1503
            DatabaseManager.Save(new List<Level> { level });
        }
    }
}
