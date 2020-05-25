using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belot.Core.GameStateModels.Helpers
{
    public static class ScoreHelper
    {
        public static Dictionary<string, (int, int)> BasePoints = new Dictionary<string, (int, int)>
        {
            ["Clubs"] = (16, 25),
            ["Diamonds"] = (16, 25),
            ["Hearts"] = (16, 25),
            ["Spades"] = (16, 25),
            ["NoTrumps"] = (26, 35),
            ["AllTrumps"] = (26, 35)
        };

        public static void HandlePointFromAnn(GameState game, int winningTeam, int loosingTeam, int annTeam, int otherTeam)
        {
            var roundState = game.TableState.CurrentRoundState;

            if (winningTeam != annTeam || (roundState.IsDouble || roundState.IsQuadrapul))
            {
                var annTeamAnnPoints = roundState.TeamPointsFromAnn[loosingTeam];

                roundState.TeamPointsFromAnn[winningTeam] += annTeamAnnPoints;
                roundState.TeamPointsFromAnn[loosingTeam] = 0;

                roundState.TeamRoundScore[winningTeam] += roundState.TeamPointsFromAnn[winningTeam] / 10;
            }
            else
            {
                roundState.TeamRoundScore[annTeam] += roundState.TeamPointsFromAnn[annTeam] / 10;
                roundState.TeamRoundScore[otherTeam] += roundState.TeamPointsFromAnn[otherTeam] / 10;
            }
        }


        public static void HandlePointsFromCards(GameState game, int winningTeam, int loosingTeam, int annTeam, int otherTeam)
        {
            var roundState = game.TableState.CurrentRoundState;
            var roundUpPoint = 1;

            //bezkos
            if (roundState.BidSuit == "NoTrumps")
            {
                roundState.TeamRoundPoints[annTeam] *= 2;
                roundState.TeamRoundPoints[otherTeam] *= 2;
            }
            //kapo
            if (game.TableState.TeamCardPiles[loosingTeam].Count == 0)
            {
                roundState.IsDouble = false;
                roundState.IsQuadrapul = false;

                roundState.TeamRoundScore[winningTeam] += BasePoints[roundState.BidSuit].Item2;
            }
            //handle vytre or kontra
            else if ((roundState.IsDouble || roundState.IsQuadrapul) || winningTeam != annTeam)
            {
                roundState.TeamRoundScore[winningTeam] += BasePoints[roundState.BidSuit].Item1;
            }
            else
            {
                //handle rounding
                if ((roundState.TeamRoundPoints[loosingTeam] % 10 > 4 && roundState.BidSuit == "AllTrumps")
                    || (roundState.TeamRoundPoints[loosingTeam] % 10 > 5 && roundState.BidSuit == "NoTrumps")
                    || (roundState.TeamRoundPoints[loosingTeam] % 10 > 6 && (roundState.BidSuit != "NoTrumps" && roundState.BidSuit != "AllTrumps")))
                {
                    roundState.TeamRoundScore[loosingTeam] += roundUpPoint;
                    if ((roundState.TeamRoundPoints[loosingTeam] % 10 == 4 && roundState.BidSuit == "AllTrumps")
                    || (roundState.TeamRoundPoints[loosingTeam] % 10 == 5 && roundState.BidSuit == "NoTrumps")
                    || (roundState.TeamRoundPoints[loosingTeam] % 10 == 6 && (roundState.BidSuit != "NoTrumps" && roundState.BidSuit != "AllTrumps")))
                    {
                        roundUpPoint--;
                    }
                }

                if ((roundState.TeamRoundPoints[winningTeam] % 10 > 4 && roundState.BidSuit == "AllTrumps")
                    || (roundState.TeamRoundPoints[winningTeam] % 10 > 5 && roundState.BidSuit == "NoTrumps")
                    || (roundState.TeamRoundPoints[winningTeam] % 10 > 6 && (roundState.BidSuit != "NoTrumps" && roundState.BidSuit != "AllTrumps")))
                {
                    roundState.TeamRoundScore[winningTeam] += roundUpPoint;

                }

                if (roundState.BidSuit == "NoTrumps")
                {
                    roundState.TeamRoundScore[annTeam] += roundState.TeamPointsFromCards[annTeam]*2 / 10;
                    roundState.TeamRoundScore[otherTeam] += roundState.TeamPointsFromCards[otherTeam]*2 / 10;
                }
                else
                {
                    roundState.TeamRoundScore[annTeam] += roundState.TeamPointsFromCards[annTeam] / 10;
                    roundState.TeamRoundScore[otherTeam] += roundState.TeamPointsFromCards[otherTeam] / 10;
                }

            }
        }


        public static void CalculateAnnScore(GameState game)
        {

            var t1 = game.TableState.CurrentRoundState.TeamAnn[0]
                .Where(x => x.Type == "Seq")
                .OrderByDescending(x => x.Value)
                .ThenByDescending(x => x.Cards.OrderByDescending(x => x.OrderValue).First().OrderValue)
                .FirstOrDefault();

            var t2 = game.TableState.CurrentRoundState.TeamAnn[1]
              .Where(x => x.Type == "Seq")
              .OrderByDescending(x => x.Value)
              .ThenByDescending(x => x.Cards.OrderByDescending(x => x.OrderValue).First().OrderValue)
              .FirstOrDefault();

            if (t1 != null && t2 != null)
            {
                if (t1.Value > t2.Value)
                {
                    game.TableState.CurrentRoundState.TeamAnn[1].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                }
                else if (t1.Value < t2.Value)
                {
                    game.TableState.CurrentRoundState.TeamAnn[0].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                }
                else
                {
                    var topCardT1 = t1.Cards.OrderByDescending(x => x.OrderValue).First();
                    var topCardT2 = t2.Cards.OrderByDescending(x => x.OrderValue).First();

                    if (topCardT1.OrderValue > topCardT2.OrderValue)
                    {
                        game.TableState.CurrentRoundState.TeamAnn[1].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                    }
                    else if (topCardT2.OrderValue > topCardT1.OrderValue)
                    {
                        game.TableState.CurrentRoundState.TeamAnn[0].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                    }
                    else
                    {
                        game.TableState.CurrentRoundState.TeamAnn[1].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                        game.TableState.CurrentRoundState.TeamAnn[0].Where(x => x.Type == "Seq").ToList().ForEach(x => x.CountingValue = 0);
                    }
                }
            }

            game.TableState.CurrentRoundState.TeamPointsFromAnn[0] = game.TableState.CurrentRoundState.TeamAnn[0].Sum(x => x.CountingValue);
            game.TableState.CurrentRoundState.TeamPointsFromAnn[1] = game.TableState.CurrentRoundState.TeamAnn[1].Sum(x => x.CountingValue);

            game.TableState.CurrentRoundState.TeamPointsFromDispAnn[0] = game.TableState.CurrentRoundState.TeamAnn[0].Sum(x => x.CountingValue);
            game.TableState.CurrentRoundState.TeamPointsFromDispAnn[1] = game.TableState.CurrentRoundState.TeamAnn[1].Sum(x => x.CountingValue);


        }


        public static void CalculatePoints(GameState game)
        {
            if (game.TableState.CurrentRoundState.BidSuit == "AllTrumps")
            {
                for (int i = 0; i < game.TableState.TeamCardPiles.Count; i++)
                {
                    game.TableState.CurrentRoundState.TeamPointsFromCards[i] = game.TableState.TeamCardPiles[i].Sum(x => x.ATPoints);
                }
            }
            else if (game.TableState.CurrentRoundState.BidSuit == "NoTrumps")
            {
                for (int i = 0; i < game.TableState.TeamCardPiles.Count; i++)
                {
                    game.TableState.CurrentRoundState.TeamPointsFromCards[i] = game.TableState.TeamCardPiles[i].Sum(x => x.NTPoints);
                }
            }
            else
            {
                for (int i = 0; i < game.TableState.TeamCardPiles.Count; i++)
                {
                    game.TableState.CurrentRoundState.TeamPointsFromCards[i] = game.TableState.TeamCardPiles[i]
                                                                                                .Sum(x => x.Suit == game.TableState.CurrentRoundState.BidSuit ? x.ATPoints : x.NTPoints);
                }
            }
        }
    }
}
