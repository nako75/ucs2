using System;
using System.IO;
using UCS.Core;
using UCS.Logic;

namespace UCS.PacketProcessing.Commands
{
    internal class EndAttackCommand : Command
    {
        public EndAttackCommand(BinaryReader br) : base(br)
        {
        }

        public override void Execute(Level level)
        {
            ClientAvatar attacker = level.GetPlayerAvatar();
            
            int currentScore = attacker.GetScore(); 
            int trophyGain = 25; 
            
            int newScore = currentScore + trophyGain;
            if (newScore < 0) newScore = 0;

            attacker.SetScore(newScore);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Battle Score] " + attacker.GetAvatarName() + " selesai PvP! Trophy naik jadi: " + newScore);
            Console.ResetColor();

            DatabaseManager.Save(level);
        }
    }
}
