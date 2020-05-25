using Belot.Core.DeckCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.GameStateModels.Helpers
{
    public static class DealAndShuffleHelper
    {
        public static void ShuffleDeck(GameState game)
        {
            game.TableState.CurrentRoundState.Deck.Shuffle();
        }

        public static void DealCards(GameState game, int numberOfCards)
        {
            var current = game.TableState.DealerIndex;

            for (int i = 0; i < 4; i++)
            {
                current = game.GetNextIndex(current);

                var cardsToDeal = new List<Card>();

                for (int y = 0; y < numberOfCards; y++)
                {
                    cardsToDeal.Add(game.TableState.CurrentRoundState.Deck.SuffledDeck.Dequeue());
                }

                game.Players[current].RecieveCards(cardsToDeal);
            }

            game.TableState.CardsInHand += numberOfCards;
        }

        public static void ResetCardsOnTable(GameState game)
        {
            game.TableState.CardsOnTable.Clear();
            game.TableState.CardsOnTable.Add(new Card());
            game.TableState.CardsOnTable.Add(new Card());
            game.TableState.CardsOnTable.Add(new Card());
            game.TableState.CardsOnTable.Add(new Card());
        }
    }
}
