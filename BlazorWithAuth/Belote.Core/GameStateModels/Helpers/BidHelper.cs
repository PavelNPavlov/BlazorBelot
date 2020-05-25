using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels.Helpers
{
    public static class BidHelper
    {
        public static List<string> Bids = new List<string>
        {
            "Clubs",
            "Diamonds",
            "Hearts",
            "Spades",
            "NoTrumps",
            "AllTrumps",
        };

        public static bool NextPersonTurnBidding(GameState game)
        {
            var oldIndex = game.TableState.PlayerInTurnIndex;
            var nextPlayerIndex = game.GetNextIndex(game.TableState.PlayerInTurnIndex);

            game.TableState.PlayerInTurnIndex = nextPlayerIndex;
            game.TableState.PlayerInTurnName = game.Players[nextPlayerIndex].Name;
            game.TableState.PlayerInTurnId = game.Players[nextPlayerIndex].Id;

            game.TableState.ActionRequied = "Bidding";

            game.Players.ForEach(x =>
            {
                x.PlayerState.ActionRequired = "Wait";

                if(x.PlayerState.PossibleBids == null)
                {
                    x.PlayerState.PossibleBids = new List<string>();
                }
                x.PlayerState.PossibleBids.Clear();
            });
            game.Players[nextPlayerIndex].PlayerState.ActionRequired = "Bidding";
            //game.Players[oldIndex].PlayerState.ActionRequired = "Wait";
            //game.Players[oldIndex].PlayerState.PossibleBids.Clear();

            var ann = GetAllowedBids(game, nextPlayerIndex);

            game.Players[nextPlayerIndex].PlayerState.PossibleBids = ann;

            if (ann == null)
            {
                if (game.TableState.CurrentRoundState.BidSuit == "Pass")
                {
                    game.StartNextRound();
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public static int MakeBid(GameState game, string announcement, string playerId)
        {
            if(playerId != game.TableState.PlayerInTurnId)
            {
                return -1;
            }

            var roundState = game.TableState.CurrentRoundState;
            var currentPlayerIndex = game.TableState.PlayerInTurnIndex;

            game.TableState.PlayerAnnThisHand[game.TableState.PlayerInTurnIndex] = announcement;

            if (roundState.BidderIndex == -1)
            {
                roundState.BidderIndex = currentPlayerIndex;
                roundState.BidSuit = announcement;
            }
            else if (announcement == "Kontra")
            {
                roundState.BidderIndex = currentPlayerIndex;
                roundState.IsDouble = true;
            }
            else if (announcement == "ReKontra")
            {
                roundState.BidderIndex = currentPlayerIndex;
                roundState.IsQuadrapul = true;
                roundState.IsDouble = false;
            }
            else if (announcement != "Pass")
            {
                roundState.BidderIndex = currentPlayerIndex;
                roundState.BidSuit = announcement;
                roundState.IsQuadrapul = false;
                roundState.IsDouble = false;
            }

            return NextPersonTurnBidding(game) == true? 1:0;
        }

        private static List<string> GetAllowedBids(GameState game, int playerIndex)
        {
            var roundState = game.TableState.CurrentRoundState;

            var result = new List<string>();

            if (playerIndex == roundState.BidderIndex)
            {
                return null;
            }

            if (roundState.BidSuit == "Pass")
            {
                result.AddRange(Bids);
                result.Insert(0, "Pass");

                return result;
            }

            var cAnoucerTeam = playerIndex % 2;
            var hAnoucerTeam = roundState.BidderIndex % 2;
            var annoucedIndex = Bids.IndexOf(roundState.BidSuit);

            var availableOptions = Bids.Count - annoucedIndex;

            if (availableOptions == 0)
            {
                result.Insert(0, "Pass");
                return result;
            }

            for (int i = annoucedIndex + 1; i < Bids.Count; i++)
            {
                result.Add(Bids[i]);
            }

            result.Insert(0, "Pass");

            if (cAnoucerTeam == hAnoucerTeam)
            {
                return result;
            }
            else
            {
                if (!roundState.IsDouble && !roundState.IsQuadrapul)
                {
                    result.Add("Kontra");
                }
                else if (roundState.IsDouble && !roundState.IsQuadrapul)
                {
                    result.Add("ReKontra");
                }

                return result;
            }

        }
    }
}
