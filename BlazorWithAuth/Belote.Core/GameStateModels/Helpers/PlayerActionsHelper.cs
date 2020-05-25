using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belot.Core.GameStateModels.Helpers
{
    public static class PlayerActionsHelper
    {
        public static void DeterminAnnoucments(GameState game)
        {
            //first hand
            game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Clear();

            if (game.TableState.CurrentRoundState.HandNumber <= 0)
            {
                HandleSequentialAnnouncments(game);
                HandleFourOfAKind(game);
                HandleBelot(game);
                HandleConflictingAnnouncements(game);
            }
            else
            {
                HandleBelot(game);
            }
        }

        public static void DeterminePlayeableCards(GameState game)
        {
            var roundState = game.TableState.CurrentRoundState;
            game.Players.ForEach(x => x.PlayerState.Hand.ForEach(y => y.CanPlay = false));
            if (roundState.CurrentHandFirstPlayerIndex == game.TableState.PlayerInTurnIndex)
            {
                game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
            }
            else
            {
                if (roundState.BidSuit == "AllTrumps")
                {
                    var cardsThatCanAnswerSuit = game.CurrentPlayer.PlayerState.Hand.Where(x => x != null && x.Suit == game.TableState.FirstCardInRound.Suit).ToList();

                    if (cardsThatCanAnswerSuit.Count == 0)
                    {
                        game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                        return;
                    }

                    var topCard = game.TableState.CardsOnTable.Where(x => x!= null && x.Suit == game.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();
                    var cardsThatCanRaise = cardsThatCanAnswerSuit.Where(x => x.SortValueAT > topCard.SortValueAT).ToList();

                    if (cardsThatCanRaise.Count == 0)
                    {
                        game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanAnswerSuit.Contains(x));
                        return;
                    }

                    game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanRaise.Contains(x));
                    return;
                }
                else if (roundState.BidSuit == "NoTrumps")
                {
                    var cardsThatCanAnswerSuit = game.CurrentPlayer.PlayerState.Hand.Where(x => x != null && x.Suit == game.TableState.FirstCardInRound.Suit).ToList();

                    if (cardsThatCanAnswerSuit.Count == 0)
                    {
                        game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                        return;
                    }

                    game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanAnswerSuit.Contains(x));

                    return;
                }
                else
                {
                    if (game.TableState.FirstCardInRound.Suit == roundState.BidSuit)
                    {
                        var cardsThatCanAnswerSuit = game.CurrentPlayer.PlayerState.Hand.Where(x => x != null && x.Suit == game.TableState.FirstCardInRound.Suit).ToList();

                        if (cardsThatCanAnswerSuit.Count == 0)
                        {
                            game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                            return;
                        }

                        var topCard = game.TableState.CardsOnTable.Where(x=>x.Suit == game.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();
                        var cardsThatCanRaise = cardsThatCanAnswerSuit.Where(x => x.SortValueAT > topCard.SortValueAT).ToList();

                        if (cardsThatCanRaise.Count == 0)
                        {
                            game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanAnswerSuit.Contains(x));
                            return;
                        }

                        game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanRaise.Contains(x));
                        return;
                    }
                    else
                    {

                        var cardsThatCanAnswerSuit = game.CurrentPlayer.PlayerState.Hand.Where(x => x.Suit == game.TableState.FirstCardInRound.Suit).ToList();

                        //answer if possible
                        if (cardsThatCanAnswerSuit.Count != 0)
                        {
                            game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = cardsThatCanAnswerSuit.Contains(x));
                            return;
                        }

                        var playerTrumps = game.CurrentPlayer.PlayerState.Hand.Where(x => x.Suit == roundState.BidSuit).ToList();

                        //no trumps
                        if (playerTrumps.Count == 0)
                        {
                            game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                            return;
                        }

                        var teamIndex = game.TableState.PlayerInTurnIndex % 2;


                        //team holds hand

                        if (game.TableState.TeamIndexHoldingHand == teamIndex)
                        {
                            game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                            return;
                        }

                        //team doesn't hold hand

                        if (game.TableState.TeamIndexHoldingHand != teamIndex)
                        {
                            var trumps = game.TableState.CardsOnTable.Where(x => x.Suit == game.TableState.CurrentRoundState.BidSuit).OrderByDescending(x => x.SortValueAT).ToList();

                            //trumps on table
                            if (trumps.Count != 0)
                            {
                                var topTrumpInGame = trumps[0];

                                var availableHigherTrumps = game.CurrentPlayer.PlayerState.Hand
                                    .Where(x => x.Suit == game.TableState.CurrentRoundState.BidSuit && x.SortValueAT > topTrumpInGame.SortValueAT)
                                    .ToList();

                                if (availableHigherTrumps.Count == 0)
                                {
                                    game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = true);
                                }
                                else
                                {
                                    game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = availableHigherTrumps.Contains(x));
                                }

                                return;
                            }
                            else
                            {
                                game.CurrentPlayer.PlayerState.Hand.ForEach(x => x.CanPlay = playerTrumps.Contains(x));
                                return;
                            }

                            //no trumps on table

                        }


                    }
                }
            }
        }

        public static void NextPlayerInHand(GameState game, int playerIndex = -1)
        {
            var playerInTurnIndex = playerIndex;
            if (playerIndex == -1)
            {
                playerInTurnIndex = game.GetNextIndex(game.TableState.PlayerInTurnIndex);
            }

            game.Players.ForEach(x => x.PlayerState.ActionRequired = "Wait");

            game.TableState.PlayerInTurnIndex = playerInTurnIndex;
            game.TableState.PlayerInTurnId = game.Players[playerInTurnIndex].Id;
            game.TableState.PlayerInTurnName = game.Players[playerInTurnIndex].Name;
            //this.TableState.CurrentRoundState.CurrentHandFirstPlayerIndex = playerInTurnIndex;

            game.Players[playerInTurnIndex].PlayerState.ActionRequired = "PlayCard";
            //this.Players[this.DealerIndex].PlayerState.ActionRequired = "Waiting";


            DeterminAnnoucments(game);
            DeterminePlayeableCards(game);
            //this.DeterminePlayeableCards();
        }


        private static void HandleConflictingAnnouncements(GameState game)
        {
            var fourOfAKindAnnoucments = game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Where(x => x.Type == "4oK").ToList();
            var seqAnnouncements = game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Where(x => x.Type == "Seq").ToList();

            foreach (var fok in fourOfAKindAnnoucments)
            {
                foreach (var seq in seqAnnouncements)
                {
                    if (seq.Cards.Any(x => x.Value == fok.Cards[0].Value))
                    {
                        seq.ConflictsWith.Add(fok.Id);
                        fok.ConflictsWith.Add(seq.Id);
                    }
                }
            }
        }

        private static void HandleBelot(GameState game)
        { 
            if (game.TableState.CurrentRoundState.BidSuit == "NoTrumps")
            {
                return;
            }

            var cardsInGroupedBySuit = game.CurrentPlayer.PlayerState.Hand.GroupBy(x => x.Suit).ToDictionary(x => x.Key, v => v.ToList());

            foreach (var key in cardsInGroupedBySuit.Keys)
            {
                var t = game.TableState.CurrentRoundState.BidSuit;
                var t1 = game.TableState.FirstCardInRound;
                var isTrump = game.TableState.CurrentRoundState.BidSuit == "AllTrumps";
                var isFirstCard = game.TableState.FirstCardInRound == null || string.IsNullOrEmpty(game.TableState.FirstCardInRound.Value);
                var answersSuite = game.TableState.FirstCardInRound == null || key == game.TableState.FirstCardInRound.Suit;
                var t2 = (isTrump && (isFirstCard || answersSuite));

                if ((game.TableState.CurrentRoundState.BidSuit == "AllTrumps" && (isFirstCard || key == game.TableState.FirstCardInRound.Suit))
                    || (key == game.TableState.CurrentRoundState.BidSuit))
                {
                    var kingAndQueen = cardsInGroupedBySuit[key].Where(x => x.Value == "king" || x.Value == "queen").ToList();
                    List<Card> allowedWith = new List<Card>();

                    if (game.TableState.FirstCardInRound == null || key == game.TableState.FirstCardInRound.Suit)
                    {
                        allowedWith.AddRange(kingAndQueen);
                    }

                    if (kingAndQueen.Count == 2)
                    {
                        var ann = new Announcement()
                        {
                            AnnoucementText = $"Belote {key}",
                            Value = 20,
                            CountingValue = 20,
                            Cards = kingAndQueen,
                            Type = "Belote",
                            CanAnnouceWith = kingAndQueen
                        };

                        game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Add(ann);
                    }
                }

            }
        }

        private static void HandleFourOfAKind(GameState game)
        {
            if (game.TableState.CurrentRoundState.BidSuit == "NoTrumps")
            {
                return;
            }
            var cardsGroupedByValue = game.CurrentPlayer.PlayerState.Hand.GroupBy(x => x.SortValueAT).ToDictionary(x => x.Key, v => v.ToList());

            foreach (var key in cardsGroupedByValue.Keys)
            {
                if (cardsGroupedByValue[key].Count == 4 && key > 1)
                {
                    var ann = new Announcement();
                    ann.AnnoucementText = $"4 {cardsGroupedByValue[key][0].Value}";
                    ann.Cards = cardsGroupedByValue[key];
                    //ann.Rule = "any";
                    ann.Type = "4oK";
                    ann.CanAnnouceWith = game.CurrentPlayer.PlayerState.Hand;

                    if (cardsGroupedByValue[key][0].Value == "nine")
                    {
                        ann.Value = 150;
                        ann.CountingValue = 150;
                    }
                    else if (cardsGroupedByValue[key][0].Value == "jack")
                    {
                        ann.Value = 200;
                        ann.CountingValue = 200;
                    }
                    else if (cardsGroupedByValue[key][0].Value =="seven" || cardsGroupedByValue[key][0].Value == "eight")
                    {
                        ann.Value = 0;
                        ann.CountingValue = 0;
                    }
                    else
                    {
                        ann.Value = 100;
                        ann.CountingValue = 100;
                    }

                    game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Add(ann);

                }
            }
        }

        private static void HandleSequentialAnnouncments(GameState game)
        {
            if (game.TableState.CurrentRoundState.BidSuit == "NoTrumps")
            {
                return;
            }
            var cardsInGroupedBySuit = game.CurrentPlayer.PlayerState.Hand.GroupBy(x => x.Suit).ToDictionary(x => x.Key, v => v.ToList());

            foreach (var key in cardsInGroupedBySuit.Keys)
            {
                var group = cardsInGroupedBySuit[key].OrderBy(x => x.OrderValue).ToList();

                var allChains = FindSequences(key, group);

                foreach (var item in allChains)
                {
                    var text = $"{string.Join(", ", item.Select(x => x.Value + x.Suit))}";

                    var annoucment = new Announcement();
                    annoucment.Cards.AddRange(item);
                    annoucment.AnnoucementText = text;
                    annoucment.CanAnnouceWith = game.CurrentPlayer.PlayerState.Hand;
                    annoucment.Type = "Seq";

                    if (item.Count == 3)
                    {
                        annoucment.Value = 20;
                        annoucment.CountingValue = 20;
                    }
                    else if (item.Count == 4)
                    {
                        annoucment.Value = 50;
                        annoucment.CountingValue = 50;
                    }
                    else if (item.Count >= 5)
                    {
                        annoucment.Value = 100;
                        annoucment.CountingValue = 100;
                    }

                    game.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Add(annoucment);
                }
            }
        }

        private static List<List<Card>> FindSequences(string suit, List<Card> cardFromSuit)
        {
            var currentChain = new List<Card>();
            var allChains = new List<List<Card>>();

            for (int i = 0; i < cardFromSuit.Count; i++)
            {
                //start new chain
                if (currentChain.Count == 0)
                {
                    currentChain.Add(cardFromSuit[i]);
                    continue;
                }

                //not sequential
                if (currentChain[currentChain.Count - 1].OrderValue + 1 != cardFromSuit[i].OrderValue)
                {
                    var chainToAdd = new List<Card>();
                    chainToAdd.AddRange(currentChain);

                    //check if atleast 3
                    if (currentChain.Count >= 3)
                    {

                        allChains.Add(chainToAdd);
                    }

                    currentChain.Clear();
                    currentChain.Add(cardFromSuit[i]);
                    continue;

                }
                else
                {
                    currentChain.Add(cardFromSuit[i]);
                }

                if (i == cardFromSuit.Count - 1)
                {
                    var chainToAdd = new List<Card>();
                    chainToAdd.AddRange(currentChain);

                    //check if atleast 3
                    if (currentChain.Count >= 3)
                    {

                        allChains.Add(chainToAdd);
                    }

                    currentChain.Clear();
                    continue;
                }
            }

            return allChains;
        }
    }
}
