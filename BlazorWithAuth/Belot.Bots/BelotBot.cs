using CardGames.Models.Belot;
using CardGames.Models.Lobbies;
using CardGames.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BelotBots
{
    public class BelotBot
    {
        private const string ServerAddress = "https://localhost:5001";

        private HubConnection gameHubConnection;
        private GameStateVm gameSateVm;
        private PlayerStateVm playerState;
        private TableStateVm tableStateVm;
        private CardVm cardToBePlayed;

        private HubConnection hubConnection;
        private List<LobbyData> lobbies = new List<LobbyData>();
        private string joinedLobbyType;
        private string isHost;
        private LobbyData JoinedLobbyData
        {
            get { return this.lobbies.FirstOrDefault(x => x.Name == JoinedLobby); }
        }

        public bool IsInLobby { get; set; }

        public string JoinedLobby { get; set; }

        public string LobbyToJoin { get; set; }

        public string BotName { get; set; }

        public bool Active { get; set; }

        public string GameName { get; set; }

        public string UserId { get; set; }

        public string Seat { get; set; }

        public BelotBot(string lobbyName, int botIndex)
        {
            this.LobbyToJoin = lobbyName;

            this.BotName = "Bot" + botIndex.ToString();

            this.Active = true;
        }



        public async Task InitializeLobbyBot()
        {
            hubConnection = new HubConnectionBuilder()
               .WithUrl(ServerAddress + "/lobbies")
               .Build();

            hubConnection.On<List<LobbyData>>(ChannelConstants.LobbiesDataChannel, (lobbies) =>
            {
                this.lobbies.Clear();
                this.lobbies.AddRange(lobbies);
            });

            hubConnection.On<LobbyConnectResponse>(ChannelConstants.LobbyConnectionAnswer, (res) =>
            {
                IsInLobby = res.Succeded;

                if (res.Succeded)
                {
                    JoinedLobby = res.LobbyName;
                }
                else
                {
                    joinedLobbyType = string.Empty;
                }
            });

            hubConnection.On<LobbyDisconnectResponse>(ChannelConstants.LobbyDisconectAnswer, (res) =>
            {
                IsInLobby = !res.Succeded;

                if (res.Succeded)
                {
                    JoinedLobby = string.Empty;
                    joinedLobbyType = string.Empty;
                }

            });

            hubConnection.On(ChannelConstants.GameStarting, async () =>
            {
                var user = this.JoinedLobbyData.LobbyUsers.FirstOrDefault(x => x.Id == hubConnection.ConnectionId);
                var seat = this.JoinedLobbyData.LobbyUsers.IndexOf(user);
                //var url = $"/belot/{joinedLobby}/{userInput}/{seat}";
                this.UserId = BotName;
                this.Seat = seat.ToString();
                this.GameName = this.JoinedLobby;

                await InitializeGameHub();
                await hubConnection.DisposeAsync();
               
            });

            await hubConnection.StartAsync();
            
            await Join();

        }

        public async Task InitializeGameHub()
        {
            gameHubConnection = new HubConnectionBuilder()
                .WithUrl(ServerAddress + "/belotHub")
                .Build();

            gameHubConnection.On<PlayerStateVm, string>(ChannelConstants.PlayerStateChange, async (ps, eventName) =>
            {
                this.playerState = ps;
                this.cardToBePlayed = null;
                Console.WriteLine("PlayerStateChange");
                Console.WriteLine(eventName);

                if(eventName == null)
                {
                }
                //Console.WriteLine(ps.ActionRequired);
                else if (eventName == "GameStart")
                {
                    //Console.WriteLine("GameStart State Update");
                }
                else if (eventName.Contains("Deal"))
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(ps));
                }
                else if (eventName.Contains("Play"))
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(ps));
                }
                else if (eventName.Contains("ViewScore"))
                {
                    //Console.WriteLine("ViewScore");
                    //Console.WriteLine(JsonConvert.SerializeObject(ps));
                }

                if(playerState != null)
                {
                    if(playerState.ActionRequired == "Wait")
                    {
                        return;
                    }
                    else if(playerState.ActionRequired == "Bidding")
                    {
                        Thread.Sleep(1000);
                        await this.Announce(playerState.PossibleBids[0]);
                    }
                    else if (playerState.ActionRequired == "ConfirmDeal")
                    {
                        Thread.Sleep(1000);
                        await this.Deal();
                    }
                    else if (playerState.ActionRequired == "PlayCard")
                    {
                        Thread.Sleep(1000);
                        await this.PlayCard();
                    }
                }

            });

            gameHubConnection.On<JoinGameRes>(ChannelConstants.JoinGameAnswer, (res) =>
            {
                Console.WriteLine("JoinRespons");
                Console.WriteLine(res.Success);
            });

            gameHubConnection.On<TableStateVm>(ChannelConstants.TableUpdate, (tu) =>
            {
                Console.WriteLine("TableUpdate");
                this.tableStateVm = tu;

                if (tu.ActionRequied == "ViewScore")
                {
                    Console.WriteLine("ViewScore - Table Update");
                    Console.WriteLine(JsonConvert.SerializeObject(tu));
                }

            });


            await gameHubConnection.StartAsync();

            var joinReq = new JoinGameReq()
            {
                PlayerName = this.UserId,
                GameName = this.GameName,
                Seat = int.Parse(this.Seat)
            };

            Thread.Sleep(int.Parse(this.Seat) * 500);
            await gameHubConnection.SendAsync("JoinGame", joinReq);
        }

        public Task Deal()
        {
            return gameHubConnection.SendAsync("Deal", this.tableStateVm.GameId);
        }

        public Task GetLobbies()
        {
            return hubConnection.SendAsync("GetLobbies", "Belot");
        }

        public Task Join(string type = "Belot")
        {
            joinedLobbyType = type;
            var lobbyJoinRequest = new LobbyConnectRequest() { UserName = this.BotName, LobbyName = this.LobbyToJoin };
            return hubConnection.SendAsync("JoinLobby", lobbyJoinRequest);
        }

        public Task Announce(string announce)
        {
            return gameHubConnection.SendAsync("Announce", this.tableStateVm.GameId, announce);
        }

        Task PlayCard()
        {
            var card = this.playerState.Hand.Where(x => x.CanPlay).OrderByDescending(x => x.Value).FirstOrDefault();
            var annList = new List<AnnouncementVm>();
            //null check
            var ann = this.playerState
                .PossiblePlayAnnoucements
                .Where(x=>x.CanAnnouceWith.Any(y=>y.Suit == card.Suit && y.Value == card.Value))
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

            if(ann!= null)
            {
                annList.Add(ann);
            }

            if(card == null)
            {
                Thread.Sleep(200);
                return Task.Delay(0);
            }

            return gameHubConnection.SendAsync("PlayCard", this.tableStateVm.GameId, card, annList);
        }
    }
}
