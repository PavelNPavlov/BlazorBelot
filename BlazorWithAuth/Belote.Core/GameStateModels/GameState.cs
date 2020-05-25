using Belot.Core.DeckCore;
using Belot.Core.GameStateModels.Helpers;
using Belot.Core.Loggers;
using CardGames.Models.Belot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belot.Core.GameStateModels
{
    public class GameState
    {
        public const string AllTrumps = "AllTrumps";
        public const string NoTrumps = "NoTrumps";
        public const string Clubs = "Clubs";
        public const string Diamonds = "Diamonds";
        public const string Hearts = "Hearts";
        public const string Spades = "Spades";

        public static List<string> NormalAnnouncemnets = new List<string>
        {
            "Clubs",
            "Diamonds",
            "Hearts",
            "Spades",
            "NoTrumps",
            "AllTrumps",
        };

        public static Dictionary<string, (int, int)> BasePoints = new Dictionary<string, (int, int)>
        {
            [Clubs] = (16, 25),
            [Diamonds] = (16, 25),
            [Hearts] = (16, 25),
            [Spades] = (16, 25),
            [NoTrumps] = (26, 35),
            [AllTrumps] = (26, 35)
        };

        private static Random rng = new Random();

        public List<Player> Players { get; set; }

        public TableState TableState { get; set; }

        public Dictionary<string, PlayerState> PlayerStates { get; set; }

        public bool CanStart { get; set; }

        public string GameId { get; set; }

        public int PlayersInGame { get; set; }

        public bool ShouldCloseHand { get; set; }

        public bool GameStarted { get; set; }

        public Player CurrentPlayer
        {
            get { return this.Players[this.TableState.PlayerInTurnIndex]; }
        }

        public GameState()
        {
            this.Players = new List<Player>()
            {
                new Player(),
                new Player(),
                new Player(),
                new Player()
            };

            this.PlayersInGame = 0;
            this.PlayerStates = new Dictionary<string, PlayerState>();
            //this.Team1Scores = new List<int>();
            //this.Team2Scores = new List<int>();
        }

        public string AddPLayer(string playerName, string id, int seat)
        {

            if (string.IsNullOrEmpty(this.Players[seat].Id))
            {
                this.PlayersInGame++;

            }

            this.Players[seat].Id = id;
            this.Players[seat].Name = playerName;

            var currentState = this.Players[seat].PlayerState;

            this.PlayerStates.Add(id, this.Players[seat].PlayerState);

            if (this.PlayerStates.Count > 4)
            {
                var keyToRemove = this.PlayerStates.FirstOrDefault(x => x.Value == currentState && x.Key != id);

                if(this.TableState.PlayerInTurnId == keyToRemove.Key)
                {
                    this.TableState.PlayerInTurnId = id;
                }

                if (!string.IsNullOrEmpty(keyToRemove.Key))
                {
                    this.PlayerStates.Remove(keyToRemove.Key);
                }

                return keyToRemove.Key;
            }

            return null;

        }

        public void StartFirstHand()
        {
            var playerInTurnIndex = this.GetNextIndex(this.TableState.DealerIndex);

            this.TableState.PlayerInTurnIndex = playerInTurnIndex;
            this.TableState.PlayerInTurnId = this.Players[playerInTurnIndex].Id;
            this.TableState.PlayerInTurnName = this.Players[playerInTurnIndex].Name;
            this.TableState.CurrentRoundState.CurrentHandFirstPlayerIndex = playerInTurnIndex;

            this.Players[playerInTurnIndex].PlayerState.ActionRequired = "PlayCard";
            this.Players[this.TableState.DealerIndex].PlayerState.ActionRequired = "Wait";

            PlayerActionsHelper.DeterminAnnoucments(this);
            PlayerActionsHelper.DeterminePlayeableCards(this);
            //this.DeterminePlayeableCards();
        }

        public int PlayCard(CardVm cardVm, List<AnnouncementVm> announcementVms, string playerId)
        {
            if(playerId != this.TableState.PlayerInTurnId)
            {
                return -1;
            }
            var card = this.CurrentPlayer.PlayerState.Hand.FirstOrDefault(x => x.Suit == cardVm.Suit && x.Value == cardVm.Value);

            this.TableState.CardsOnTable[this.TableState.PlayerInTurnIndex] = card;

            this.CurrentPlayer.PlayerState.Hand.Remove(card);           

            this.TableState.CurrentRoundState.CardsPlayedInHand++;

            var teamIndex = this.TableState.PlayerInTurnIndex % 2;
            var ids = announcementVms.Select(x => x.Id).ToList();

            var anns = this.CurrentPlayer.PlayerState.PossiblePlayAnnoucements.Where(x => ids.Contains(x.Id));
            
            var annsText = new StringBuilder();

            foreach (var item in anns)
            {
                if (item.Type == "Seq")
                {
                    if (item.Value == 20)
                    {
                        annsText.AppendLine("Tierce");
                    }
                    else if (item.Value == 50)
                    {
                        annsText.AppendLine("Quarte");
                    }
                    else if (item.Value == 100)
                    {
                        annsText.AppendLine("Quinte");
                    }
                }
                else if (item.Type == "4oK")
                {
                    annsText.AppendLine("Carré");
                }
                else
                {
                    annsText.AppendLine("Belote");
                }
            }

            this.TableState.PlayerAnnThisHand[this.TableState.PlayerInTurnIndex] = annsText.ToString();
            var belots = anns.Where(x => x.Type == "Belote").Select(x => x.Id).ToList();

            this.TableState.CurrentRoundState.TeamAnn[teamIndex].AddRange(anns);

          
            if (this.TableState.CurrentRoundState.CardsPlayedInHand == 1)
            {
                this.TableState.TeamIndexHoldingHand = this.TableState.PlayerInTurnIndex % 2;
                this.TableState.TopCardOwner = this.TableState.PlayerInTurnIndex;
            }
            else
            {
                if (this.TableState.CurrentRoundState.BidSuit == "AllTrumps")
                {
                    var topCard = this.TableState.CardsOnTable.Where(x => x!= null && x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();
                    this.TableState.TopCardOwner = this.TableState.CardsOnTable.IndexOf(topCard);

                    this.TableState.TeamIndexHoldingHand = this.TableState.TopCardOwner % 2;
                }
                else if (this.TableState.CurrentRoundState.BidSuit == "NoTrumps")
                {
                    var topCard = this.TableState.CardsOnTable.Where(x => x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueNT).FirstOrDefault();
                    this.TableState.TopCardOwner = this.TableState.CardsOnTable.IndexOf(topCard);

                    this.TableState.TeamIndexHoldingHand = this.TableState.TopCardOwner % 2;
                }
                else
                {
                    //this.TableState.TopCardOwner = -1;

                    var topTrump = this.TableState.CardsOnTable
                        .Where(x => x != null)
                        .Where(x => x.Suit == this.TableState.CurrentRoundState.BidSuit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();

                    if (topTrump != null)
                    {
                        this.TableState.TopCardOwner = this.TableState.CardsOnTable.IndexOf(topTrump);
                    }
                    else
                    {
                        var topCard = this.TableState.CardsOnTable
                            .Where(x => x != null)
                            .Where(x => x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueNT).FirstOrDefault();

                        this.TableState.TopCardOwner = this.TableState.CardsOnTable.IndexOf(topCard);
                    }

                    this.TableState.TeamIndexHoldingHand = this.TableState.TopCardOwner % 2;
                }
            }

            if (this.TableState.CurrentRoundState.CardsPlayedInHand < 4)
            {
                PlayerActionsHelper.NextPlayerInHand(this);
            }
            else
            {
                this.ShouldCloseHand = true;
                this.Players.ForEach(x => x.PlayerState.Hand.ForEach(y => y.CanPlay = false));
            }

            return 1;
        }

        public void CloseHand()
        {
            this.TableState.CurrentRoundState.HandNumber++;
            this.TableState.PlayerAnnThisHand.ForEach(x => x = "");
           
            var winPlayerIndex = -1;

            if (this.TableState.CurrentRoundState.BidSuit == AllTrumps)
            {
                var topCard = this.TableState.CardsOnTable.Where(x => x!=null && x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();
                winPlayerIndex = this.TableState.CardsOnTable.IndexOf(topCard);

                var winTeam = winPlayerIndex % 2;

                this.TableState.TeamCardPiles[winTeam].AddRange(this.TableState.CardsOnTable);

                DealAndShuffleHelper.ResetCardsOnTable(this);
                this.TableState.CurrentRoundState.CurrentHandFirstPlayerIndex = winPlayerIndex;

            }
            else if (this.TableState.CurrentRoundState.BidSuit == NoTrumps)
            {
                var topCard = this.TableState.CardsOnTable.Where(x => x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueNT).FirstOrDefault();
                winPlayerIndex = this.TableState.CardsOnTable.IndexOf(topCard);

                var winTeam = winPlayerIndex % 2;

                this.TableState.TeamCardPiles[winTeam].AddRange(this.TableState.CardsOnTable);


                DealAndShuffleHelper.ResetCardsOnTable(this);
                this.TableState.CurrentRoundState.CurrentHandFirstPlayerIndex = winPlayerIndex;


            }
            else
            {
                winPlayerIndex = -1;
                var topTrump = this.TableState.CardsOnTable.Where(x => x!=null && x.Suit == this.TableState.CurrentRoundState.BidSuit).OrderByDescending(x => x.SortValueAT).FirstOrDefault();

                if (topTrump != null)
                {
                    winPlayerIndex = this.TableState.CardsOnTable.IndexOf(topTrump);
                }
                else
                {
                    var topCard = this.TableState.CardsOnTable.Where(x => x.Suit == this.TableState.FirstCardInRound.Suit).OrderByDescending(x => x.SortValueNT).FirstOrDefault();
                    winPlayerIndex = this.TableState.CardsOnTable.IndexOf(topCard);
                }

                var winTeam = winPlayerIndex % 2;

                this.TableState.TeamCardPiles[winTeam].AddRange(this.TableState.CardsOnTable);

                DealAndShuffleHelper.ResetCardsOnTable(this);
                this.TableState.CurrentRoundState.CurrentHandFirstPlayerIndex = winPlayerIndex;
            }

            ScoreHelper.CalculatePoints(this);

            this.Players.ForEach(x => x.PlayerState.Hand.ForEach(x => x.CanPlay = false));

            if (this.TableState.CurrentRoundState.HandNumber == 8)
            {
                this.CloseRound();
            }
            else
            {
                PlayerActionsHelper.NextPlayerInHand(this, winPlayerIndex);
                //this.NextPlayerInHand(winPlayerIndex);
            }

            this.ShouldCloseHand = false;
            this.TableState.TopCardOwner = -1;
            this.TableState.CurrentRoundState.CardsPlayedInHand = 0;

        }

        public void CloseRound()
        {
            ScoreHelper.CalculateAnnScore(this);
            //this.CalculateAnnScore();

            var roundState = this.TableState.CurrentRoundState;
            var last10 = new List<int> { 0, 0 };

            var teamHoldingLastHand = this.TableState.TopCardOwner % 2;

            last10[teamHoldingLastHand] = 10;

            roundState.TeamPointsFromCards[0] += last10[0];
            roundState.TeamPointsFromCards[1] += last10[1];

            roundState.TeamRoundPoints[0] = roundState.TeamPointsFromCards[0] + roundState.TeamPointsFromAnn[0];
            roundState.TeamRoundPoints[1] = roundState.TeamPointsFromCards[1] + roundState.TeamPointsFromAnn[1];

            var annTeam = this.TableState.CurrentRoundState.BidderIndex % 2;
            var otherTeam = (annTeam + 1) % 2;
            var winningTeam = -1;
            var loosingTeam = -1;
            var pointsHanging = false;
            var roundUpPoint = 1;
            if (roundState.TeamRoundPoints[0] > roundState.TeamRoundPoints[1]) { winningTeam = 0; }
            else if (roundState.TeamRoundPoints[1] > roundState.TeamRoundPoints[0]) { winningTeam = 1; }
            else
            {
                winningTeam = annTeam;
                pointsHanging = true;
            }

            if (winningTeam == -1)
            {
                roundState.IsDouble = false;
                roundState.IsQuadrapul = false;
            }

            loosingTeam = (winningTeam + 1) % 2;

            //handle PointsFromnCards

            ScoreHelper.HandlePointsFromCards(this, winningTeam, loosingTeam, annTeam, otherTeam);

            //handle points from Ann

            ScoreHelper.HandlePointFromAnn(this, winningTeam, loosingTeam, annTeam, otherTeam);

            //handle multipling

            if (roundState.IsDouble)
            {
                roundState.TeamRoundScore[winningTeam] *= 2;
            }

            if (roundState.IsQuadrapul)
            {
                roundState.TeamRoundScore[winningTeam] *= 4;
            }


            //handle hanging
            if (winningTeam == -1)
            {
                this.TableState.HangingPoints += roundState.TeamRoundScore[annTeam];
                roundState.TeamRoundScore[annTeam] = 0;
            }
            else
            {
                roundState.TeamRoundScore[winningTeam] += this.TableState.HangingPoints;
                this.TableState.HangingPoints = 0;
            }

            ScoreLogger.LogRoundScoring(this);

            this.TableState.ActionRequied = "ViewScoreRound";
            this.Players.ForEach(x => x.PlayerState.ActionRequired = "ViewScoreRound");

            this.TableState.CurrentGameScore[0].Add(roundState.TeamRoundScore[0]);
            this.TableState.CurrentGameScore[1].Add(roundState.TeamRoundScore[1]);

            if (this.TableState.CurrentGameScore[0].Sum() > 151
                && this.TableState.CurrentGameScore[0].Sum() > this.TableState.CurrentGameScore[1].Sum()
                && this.TableState.CurrentRoundState.TeamPointsFromCards[0] > 0)
            {
                this.TableState.OveralScore[0]++;

                this.TableState.ActionRequied = "ViewScoreGame";
                this.Players.ForEach(x => x.PlayerState.ActionRequired = "ViewScoreGame");

            }

            if (this.TableState.CurrentGameScore[1].Sum() > 151
               && this.TableState.CurrentGameScore[1].Sum() > this.TableState.CurrentGameScore[0].Sum()
               && this.TableState.CurrentRoundState.TeamPointsFromCards[1] > 0)
            {
                this.TableState.ActionRequied = "ViewScoreGame";
                this.Players.ForEach(x => x.PlayerState.ActionRequired = "ViewScoreGame");
                this.TableState.OveralScore[1]++;
            }

        }

        public void StartNextRound()
        {
            var newDealerIndex = this.GetNextIndex(this.TableState.DealerIndex);
            this.TableState.DealerIndex = newDealerIndex;
            this.TableState.PlayerAnnThisHand.ForEach(x => x = string.Empty);
            this.Players.ForEach(x =>
            {
                x.PlayerState.PossibleBids = new List<string>();
                x.PlayerState.PossiblePlayAnnoucements = new List<Announcement>();
                x.PlayerState.Hand.Clear();
                x.PlayerState.InTurn = false;
                x.PlayerState.ActionRequired = "Wait";
            });

            //this.Players.ForEach(x => x.PlayerState.ActionRequired = "Wait");
            this.TableState.PlayerInTurnId = this.Players[this.TableState.DealerIndex].Id;
            this.TableState.PlayerInTurnName = this.Players[this.TableState.DealerIndex].Name;
            this.TableState.PlayerInTurnIndex = this.TableState.DealerIndex;
            this.TableState.ActionRequied = "ConfirmDeal";

            this.Players[this.TableState.DealerIndex].PlayerState.InTurn = true;
            this.Players[this.TableState.DealerIndex].PlayerState.ActionRequired = this.TableState.ActionRequied;

            this.TableState.TeamCardPiles[0].Clear();
            this.TableState.TeamCardPiles[1].Clear();





            this.TableState.CurrentRoundState = new RoundState()
            {
                Stage = "WaitToDeal1",
                BidderIndex = -1,
                HangingPoints = this.TableState.HangingPoints
            };

            DealAndShuffleHelper.ShuffleDeck(this);
        }

        public void StartNextSet()
        {
            var winningTeam = this.TableState.CurrentGameScore[0].Sum() > this.TableState.CurrentGameScore[1].Sum() ? 0 : 1;
            var newDealerIndex = rng.Next(99) < 50 ? winningTeam : (winningTeam + 2);

            this.TableState.DealerIndex = newDealerIndex;

            this.Players.ForEach(x => x.PlayerState.ActionRequired = "Wait");
            this.TableState.PlayerInTurnId = this.Players[this.TableState.DealerIndex].Id;
            this.TableState.PlayerInTurnName = this.Players[this.TableState.DealerIndex].Name;
            this.TableState.PlayerInTurnIndex = this.TableState.DealerIndex;
            this.TableState.ActionRequied = "ConfirmDeal";
            this.TableState.CurrentGameScore[0].Clear();
            this.TableState.CurrentGameScore[1].Clear();

            this.Players[this.TableState.DealerIndex].PlayerState.InTurn = true;
            this.Players[this.TableState.DealerIndex].PlayerState.ActionRequired = this.TableState.ActionRequied;

            this.TableState.TeamCardPiles[0].Clear();
            this.TableState.TeamCardPiles[1].Clear();

            this.Players.ForEach(x =>
            {
                x.PlayerState.PossibleBids = new List<string>();
                x.PlayerState.PossiblePlayAnnoucements = new List<Announcement>();
                x.PlayerState.Hand.Clear();
                x.PlayerState.InTurn = false;
            });

            this.TableState.CurrentRoundState = new RoundState()
            {
                Stage = "WaitToDeal1",
                HangingPoints = 0,
                BidderIndex = -1,
            };

            DealAndShuffleHelper.ShuffleDeck(this);
        }

        public void StartGame()
        {
            this.CanStart = true;
            this.TableState = new TableState
            {
                CardsInHand = 0,
                PlayerInTurnId = this.Players[0].Id,
                PlayerInTurnName = this.Players[0].Name,
                PlayerInTurnIndex = 0,
                DealerId = this.Players[0].Id,
                ActionRequied = "ConfirmDeal",
                PlayerNames = this.Players.Select(x => x.Name).ToList(),
                GameId = this.GameId
            };

            this.Players[0].PlayerState.InTurn = true;
            this.Players[0].PlayerState.ActionRequired = this.TableState.ActionRequied;


            this.GameStarted = true;

            this.TableState.CurrentRoundState = new RoundState()
            {
                Stage = "WaitToDeal1",
                HangingPoints = 0,
                BidderIndex = -1,
            };


        }
       
        public void Deal()
        {
            if (this.TableState.CurrentRoundState.Stage == "WaitToDeal1")
            {
                DealAndShuffleHelper.ShuffleDeck(this);
                DealAndShuffleHelper.DealCards(this, 3);
                this.TableState.CurrentRoundState.Stage = "Dealt1";
            }

            if (this.TableState.CurrentRoundState.Stage == "WaitToDeal2")
            {
                DealAndShuffleHelper.DealCards(this, 2);
                this.TableState.CurrentRoundState.Stage = "Dealt2";
            }


            if (this.TableState.CurrentRoundState.Stage == "WaitToDeal3")
            {
                DealAndShuffleHelper.DealCards(this, 3);
                this.TableState.CurrentRoundState.Stage = "Dealt3";
            }
        }

        public void StartBidding()
        {
            this.TableState.CurrentRoundState.Stage = "Bidding";

            BidHelper.NextPersonTurnBidding(this);
        }

        public int MakeBid(string announcement, string playerId) 
        {
            var result = BidHelper.MakeBid(this, announcement, playerId);

            return result;
        }

        public void SecondPropmtDealer()
        {
            this.TableState.PlayerInTurnIndex = this.TableState.DealerIndex;
            this.TableState.PlayerInTurnName = this.Players[this.TableState.DealerIndex].Name;
            this.TableState.PlayerInTurnId = this.Players[this.TableState.DealerIndex].Id;

            for (int i = 0; i < this.Players.Count; i++)
            {
                if (i == this.TableState.DealerIndex)
                {
                    this.Players[i].PlayerState.ActionRequired = "ConfirmDeal";
                    this.TableState.CurrentRoundState.Stage = "WaitToDeal3";
                }
                else
                {
                    this.Players[i].PlayerState.ActionRequired = "Wait";
                }
            }
        }

        public int GetNextIndex(int index)
        {
            return (index + 3) % 4;
        }
    }
}
