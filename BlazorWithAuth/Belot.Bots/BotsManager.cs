using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BelotBots
{
    public static class BotsManager
    {
        public static int BotCount = 0;

        public static List<Task> RunningBots = new List<Task>();

        public static void CreateBotForLobby(string lobbyName)
        {
            BotCount++;

            var task = Task.Factory.StartNew(async () =>
            {
                var bot = new BelotBot(lobbyName, BotCount);
                
                await bot.InitializeLobbyBot(); 

                while (bot.Active) 
                {
                    
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
