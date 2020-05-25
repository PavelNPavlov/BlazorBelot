using Belot.Core.DeckCore;
using Belot.Core.GameStateModels;
using CardGames.Models.Belot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belot.Core
{
    public class BelotGameManager
    {
        public Dictionary<string, GameState> Games { get; set; }

        public bool CreatingGame { get; set; }

        public BelotGameManager()
        {
            this.Games = new Dictionary<string, GameState>();
        }

        public void CreateGame(string gameName)
        {
            if (this.Games.ContainsKey(gameName))
            {
                return;
            }

            var count = this.Games.Count;
            this.CreatingGame = true;

            var gameState = new GameState();
            gameState.GameId = gameName;

            if (this.Games != null && this.Games.Keys != null && !this.Games.Keys.Any(x => x == gameName))
            {
                this.Games.Add(gameName, gameState);
            }

            this.CreatingGame = false;
        }

        public string AddPlayerToGame(string gameName, string playerName, string id, int seat)
        {
            var clientToDisconect = this.Games[gameName].AddPLayer(playerName, id, seat);

            return clientToDisconect;
        }

        public void Deal(string gameId)
        {
            this.Games[gameId].Deal();

            var stage = this.Games[gameId].TableState.CurrentRoundState.Stage;

            if (stage == "Dealt1")
            {
                this.Games[gameId].TableState.CurrentRoundState.Stage = "WaitToDeal2";
            }

            if (stage == "Dealt2")
            {
                this.Games[gameId].StartBidding();
            }

            if (stage == "Dealt3")
            {
                this.Games[gameId].TableState.CurrentRoundState.Stage = "StartGame";
                this.Games[gameId].StartFirstHand();
            }
        }


        public void Annouce(string gameId, string announcement, string playerId)
        {
            var shouldStartGame = this.Games[gameId].MakeBid(announcement, playerId);

            if (shouldStartGame == 1)
            {
                this.Games[gameId].SecondPropmtDealer();
            }
            //else
            //{
            //    this.Games[gameId].StartNextRound();
            //}
        }

        public int PlayCard(string gameId, CardVm cardVm, List<AnnouncementVm> announcements, string playerId)
        {
            var result = this.Games[gameId].PlayCard(cardVm, announcements, playerId);

            return result;
        }

        public void NextRound(string gameId)
        {
            this.Games[gameId].StartNextRound();
        }

        public void NextGameInSet(string gameId)
        {
            this.Games[gameId].StartNextRound();


            this.Games[gameId].TableState.CurrentGameScore = new List<List<int>>
            {
                new List<int>(){ 0},
                new List<int>(){ 0}
            };
        }
    }
}
