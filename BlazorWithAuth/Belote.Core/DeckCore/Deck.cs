using Belot.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belot.Core.DeckCore
{
    public class Deck
    {
        private static Random rng = new Random();

        public List<Card> BaseDeck { get; set; }

        public Queue<Card> SuffledDeck { get; set; }

        public List<List<Card>> TestBaseDeck { get; set; }

        public Deck()
        {
            var cards = new string[] { "seven", "eight", "nine", "ten", "jack", "queen", "king", "ace" };
            var cardsAT = new int[] { 1, 2, 7, 5, 8, 3, 4, 6 };
            var cardsATP = new int[] { 0, 0, 14, 10, 20, 3, 4, 11 };
            var cardsNT = new int[] { 1, 2, 3, 7, 4, 5, 6, 8 };
            var cardsNTP = new int[] { 0, 0, 0, 10, 2, 3, 4, 11 };
            var suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };

            this.BaseDeck = new List<Card>();
            this.SuffledDeck = new Queue<Card>();
            this.TestBaseDeck = new List<List<Card>>();

            for (int i = 0; i < cards.Length; i++)
            {
                for (int y = 0; y < suits.Length; y++)
                {
                    this.BaseDeck.Add(new Card() 
                    { 
                        Value = cards[i], 
                        Suit = suits[y], 
                        SortValueNT = cardsNT[i], 
                        NTPoints = cardsNTP[i],
                        SortSuit = y, 
                        SortValueAT = cardsAT[i],
                        ATPoints = cardsATP[i],
                        OrderValue = i
                    });
                }
            }

            for (int i = 0; i < suits.Length; i++)
            {
                this.TestBaseDeck.Add(new List<Card>());
                for (int y = 0; y < cards.Length; y++)
                {
                    this.TestBaseDeck[i].Add(new Card() 
                    { 
                        Value = cards[y], 
                        Suit = suits[i], 
                        SortValueNT = cardsNT[y],
                        NTPoints = cardsNTP[y],
                        SortSuit = i, 
                        SortValueAT = cardsAT[y], 
                        ATPoints = cardsATP[y],
                        OrderValue = y 
                    });
                }
            }
        }

        public void Shuffle()
        {
            var suffledCards = new List<Card>();
            suffledCards.AddRange(this.BaseDeck);
            suffledCards.Shuffle(rng);

            this.SuffledDeck.Clear();

            foreach (var item in suffledCards)
            {
                this.SuffledDeck.Enqueue(item);
            }

            var s = 5;
        }

        public void ShuffleTest()
        {
            var suffledCards = new List<Card>();

            suffledCards.Add(TestBaseDeck[2][4]);
            suffledCards.Add(TestBaseDeck[2][7]);
            suffledCards.Add(TestBaseDeck[2][1]);

            suffledCards.Add(TestBaseDeck[3][7]);
            suffledCards.Add(TestBaseDeck[3][3]);
            suffledCards.Add(TestBaseDeck[3][4]);

            suffledCards.Add(TestBaseDeck[2][6]);
            suffledCards.Add(TestBaseDeck[2][5]);
            suffledCards.Add(TestBaseDeck[3][6]);

            suffledCards.Add(TestBaseDeck[2][3]);
            suffledCards.Add(TestBaseDeck[2][0]);
            suffledCards.Add(TestBaseDeck[3][5]);

            suffledCards.Add(TestBaseDeck[0][2]);
            suffledCards.Add(TestBaseDeck[0][3]);
            suffledCards.Add(TestBaseDeck[2][2]);

            suffledCards.Add(TestBaseDeck[1][7]);
            suffledCards.Add(TestBaseDeck[3][1]);
            suffledCards.Add(TestBaseDeck[3][0]);


            suffledCards.Add(TestBaseDeck[1][2]);
            suffledCards.Add(TestBaseDeck[3][2]);
            suffledCards.Add(TestBaseDeck[0][4]);

            suffledCards.Add(TestBaseDeck[1][5]);
            suffledCards.Add(TestBaseDeck[1][1]);
            suffledCards.Add(TestBaseDeck[1][6]);

            suffledCards.Add(TestBaseDeck[0][6]);
            suffledCards.Add(TestBaseDeck[0][5]);
            suffledCards.Add(TestBaseDeck[1][3]);

            suffledCards.Add(TestBaseDeck[1][4]);
            suffledCards.Add(TestBaseDeck[1][0]);
            suffledCards.Add(TestBaseDeck[0][0]);

            suffledCards.Add(TestBaseDeck[0][1]);
            suffledCards.Add(TestBaseDeck[0][7]);


            this.SuffledDeck.Clear();

            foreach (var item in suffledCards)
            {
                this.SuffledDeck.Enqueue(item);
            }

            var s = 5;
        }

        public void ShuffleTest2()
        {
            var suffledCards = new List<Card>();

            suffledCards.Add(TestBaseDeck[0][2]);
            suffledCards.Add(TestBaseDeck[1][2]);
            suffledCards.Add(TestBaseDeck[2][2]);

            suffledCards.Add(TestBaseDeck[3][7]);
            suffledCards.Add(TestBaseDeck[3][3]);
            suffledCards.Add(TestBaseDeck[3][4]);

            suffledCards.Add(TestBaseDeck[2][6]);
            suffledCards.Add(TestBaseDeck[2][5]);
            suffledCards.Add(TestBaseDeck[3][6]);

            suffledCards.Add(TestBaseDeck[2][3]);
            suffledCards.Add(TestBaseDeck[2][0]);
            suffledCards.Add(TestBaseDeck[3][5]);

            suffledCards.Add(TestBaseDeck[3][2]);
            suffledCards.Add(TestBaseDeck[0][1]);
            suffledCards.Add(TestBaseDeck[2][2]);

            suffledCards.Add(TestBaseDeck[1][7]);
            suffledCards.Add(TestBaseDeck[3][1]);
            suffledCards.Add(TestBaseDeck[3][0]);


            suffledCards.Add(TestBaseDeck[1][2]);
            suffledCards.Add(TestBaseDeck[3][2]);
            suffledCards.Add(TestBaseDeck[0][0]);

            suffledCards.Add(TestBaseDeck[2][5]);
            suffledCards.Add(TestBaseDeck[2][6]);
            suffledCards.Add(TestBaseDeck[1][6]);

            suffledCards.Add(TestBaseDeck[0][6]);
            suffledCards.Add(TestBaseDeck[0][5]);
            suffledCards.Add(TestBaseDeck[1][3]);

            suffledCards.Add(TestBaseDeck[1][4]);
            suffledCards.Add(TestBaseDeck[1][0]);
            suffledCards.Add(TestBaseDeck[0][0]);

            suffledCards.Add(TestBaseDeck[0][1]);
            suffledCards.Add(TestBaseDeck[0][7]);


            this.SuffledDeck.Clear();

            foreach (var item in suffledCards)
            {
                this.SuffledDeck.Enqueue(item);
            }

            var s = 5;
        }


        public void ShuffleTest3()
        {
            var suffledCards = new List<Card>();

            suffledCards.Add(TestBaseDeck[2][4]);
            suffledCards.Add(TestBaseDeck[2][7]);
            suffledCards.Add(TestBaseDeck[2][1]);

            suffledCards.Add(TestBaseDeck[3][7]);
            suffledCards.Add(TestBaseDeck[3][3]);
            suffledCards.Add(TestBaseDeck[3][4]);

            suffledCards.Add(TestBaseDeck[0][2]);
            suffledCards.Add(TestBaseDeck[0][3]);
            suffledCards.Add(TestBaseDeck[3][6]);

            suffledCards.Add(TestBaseDeck[2][3]);
            suffledCards.Add(TestBaseDeck[2][0]);
            suffledCards.Add(TestBaseDeck[3][5]);

            suffledCards.Add(TestBaseDeck[2][6]);
            suffledCards.Add(TestBaseDeck[2][5]);
            suffledCards.Add(TestBaseDeck[2][2]);

            suffledCards.Add(TestBaseDeck[1][7]);
            suffledCards.Add(TestBaseDeck[3][1]);
            suffledCards.Add(TestBaseDeck[3][0]);


            suffledCards.Add(TestBaseDeck[1][2]);
            suffledCards.Add(TestBaseDeck[3][2]);
            suffledCards.Add(TestBaseDeck[0][4]);

            suffledCards.Add(TestBaseDeck[1][5]);
            suffledCards.Add(TestBaseDeck[1][1]);
            suffledCards.Add(TestBaseDeck[1][6]);

            suffledCards.Add(TestBaseDeck[0][6]);
            suffledCards.Add(TestBaseDeck[0][5]);
            suffledCards.Add(TestBaseDeck[1][3]);

            suffledCards.Add(TestBaseDeck[1][4]);
            suffledCards.Add(TestBaseDeck[1][0]);
            suffledCards.Add(TestBaseDeck[0][0]);

            suffledCards.Add(TestBaseDeck[0][1]);
            suffledCards.Add(TestBaseDeck[0][7]);


            this.SuffledDeck.Clear();

            foreach (var item in suffledCards)
            {
                this.SuffledDeck.Enqueue(item);
            }

            var s = 5;
        }
    }





}
