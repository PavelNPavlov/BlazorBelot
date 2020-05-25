using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Belot.Core.GameStateModels;
using Belot.Core.Loggers.LoggerModels;
using Newtonsoft.Json;

namespace Belot.Core.Loggers
{
    public static class ScoreLogger
    {
        private const string LogPath = @"D:\BelotProject\BelotBlazor2\BelotBlazor2\Server\ScoreLogs.txt";
        public static void LogRoundScoring(GameState game)
        {
            var team1CardPile = game.TableState.TeamCardPiles[0].Select(x=> x.Value + " " + x.Suit).ToList();

            var team2CardPile = game.TableState.TeamCardPiles[1].Select(x => x.Value + " " + x.Suit).ToList();

            var team1PointsFromCards = game.TableState.CurrentRoundState.TeamPointsFromCards[0];
            var team2PointsFromCards = game.TableState.CurrentRoundState.TeamPointsFromCards[1];

            var team1PointsFromAnn = game.TableState.CurrentRoundState.TeamPointsFromCards[0];
            var team2PointsFromAnn = game.TableState.CurrentRoundState.TeamPointsFromCards[1];

            var team1Score = game.TableState.CurrentRoundState.TeamRoundScore[0];
            var team2Score = game.TableState.CurrentRoundState.TeamRoundScore[1];

            var logEntry = new ScoreLoggerModel()
            {
                CardsT1 = team1CardPile,
                Team1CP = team1PointsFromCards,
                Team1AP = team1PointsFromAnn,
                Team1S = team1Score,
                CardsT2 = team2CardPile,
                Team2CP = team2PointsFromCards,
                Team2AP = team2PointsFromAnn,
                Team2S = team2Score,
                AnnoucedSuite = game.TableState.CurrentRoundState.BidSuit,
                AnnoucerIndex = game.TableState.CurrentRoundState.BidderIndex
            };

            var text = JsonConvert.SerializeObject(logEntry, Formatting.Indented);

            File.AppendAllText(LogPath, text);
        }
    }
}
