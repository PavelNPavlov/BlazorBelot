using AutoMapper;
using Belot.Core;
using Belot.Core.GameStateModels;
using CardGames.Models.Belot;
using CardGames.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardGames.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly object mangaerLock = new object();
        private readonly BelotGameManager belotGameManager;
        private readonly IMapper mapper;
        public GameHub(BelotGameManager belotGameManager, IMapper mapper)
        {
            this.belotGameManager = belotGameManager;
            this.mapper = mapper;
        }

        public async Task JoinGame(JoinGameReq joinGameReq)
        {
            try
            {
                if (!this.belotGameManager.Games.ContainsKey(joinGameReq.GameName))
                {
                    lock (mangaerLock)
                    {
                        this.belotGameManager.CreateGame(joinGameReq.GameName);
                    }                  

                }

                if (this.belotGameManager.Games[joinGameReq.GameName].GameStarted)
                {
                    var clientToDisconect = this.belotGameManager.AddPlayerToGame(joinGameReq.GameName, joinGameReq.PlayerName, this.Context.ConnectionId, joinGameReq.Seat);
                    await this.Groups.AddToGroupAsync(this.Context.ConnectionId, joinGameReq.GameName);
                    var rejoinResult = new JoinGameRes { Success = true };
                    await this.Clients.Caller.SendAsync(ChannelConstants.JoinGameAnswer, rejoinResult);
                    await this.SendDisconectMessage(clientToDisconect, joinGameReq.GameName);
                    await this.UpdateTabelState(joinGameReq.GameName);
                    await this.UpdateIndividualPlayerStates(joinGameReq.GameName, "NewPlayer");

                    return;
                }


                this.belotGameManager.AddPlayerToGame(joinGameReq.GameName, joinGameReq.PlayerName, this.Context.ConnectionId, joinGameReq.Seat);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, joinGameReq.GameName);
                var result = new JoinGameRes { Success = true };
                await this.Clients.Caller.SendAsync(ChannelConstants.JoinGameAnswer, result);

               
                if (!this.belotGameManager.Games[joinGameReq.GameName].Players.Any(x => string.IsNullOrEmpty(x.Id))
                    && !this.belotGameManager.Games[joinGameReq.GameName].CanStart)
                {
                    this.belotGameManager.Games[joinGameReq.GameName].StartGame();

                   
                    await this.UpdateTabelState(joinGameReq.GameName);

                    this.belotGameManager.Games[joinGameReq.GameName].CanStart = false;
                    await this.UpdateIndividualPlayerStates(joinGameReq.GameName, "GameStart");

                    return;
                    
                }

                await this.UpdateIndividualPlayerStates(joinGameReq.GameName);


            }
            catch (Exception ex)
            {
                var s = 4;
            }

        }


        public async Task Deal(string gameId)
        {
            try
            {
                this.belotGameManager.Deal(gameId);


                await this.UpdateIndividualPlayerStates(gameId, "Deal1");

                this.belotGameManager.Deal(gameId);

                await this.UpdateIndividualPlayerStates(gameId, "Deal2");

                await this.UpdateTabelState(gameId);


            }
            catch (Exception ex)
            {
                var s = 4;
            }

        }

        public async Task Announce(string gameId, string announcement)
        {
            try
            {
                this.belotGameManager.Annouce(gameId, announcement, this.Context.ConnectionId);

                await this.UpdateIndividualPlayerStates(gameId, "Announcement");

                await this.UpdateTabelState(gameId);
            }
            catch (Exception ex)
            {
                var s = 4;
            }
        }

        public async Task PlayCard(string gameId, CardVm card, List<AnnouncementVm> announcementVms)
        {
            try
            {
                var validPlay =  this.belotGameManager.PlayCard(gameId, card, announcementVms, this.Context.ConnectionId);

                if(validPlay == -1)
                {
                    return;
                }

                await this.UpdateTabelState(gameId);
                await this.UpdateIndividualPlayerStates(gameId, "Play");

                if (this.belotGameManager.Games[gameId].ShouldCloseHand)
                {
                    Thread.Sleep(4000);

                    this.belotGameManager.Games[gameId].CloseHand();
                    await this.UpdateTabelState(gameId);
                    await this.UpdateIndividualPlayerStates(gameId, "GameStart");
                }

                if (this.belotGameManager.Games[gameId].TableState.ActionRequied == "ViewScoreRound")
                {
                    Thread.Sleep(10000);
                    this.belotGameManager.NextRound(gameId);

                    await this.UpdateTabelState(gameId);
                    await this.UpdateIndividualPlayerStates(gameId, "GameStart");
                }

                if (this.belotGameManager.Games[gameId].TableState.ActionRequied == "ViewScoreGame")
                {
                    Thread.Sleep(15000);
                    this.belotGameManager.NextGameInSet(gameId);

                    await this.UpdateTabelState(gameId);
                    await this.UpdateIndividualPlayerStates(gameId, "GameStart");
                }
            }
            catch (Exception ex)
            {
                var s = 4;
            }
        }

        
        private async Task SendDisconectMessage(string clientId, string gameId)
        {
            var tbState = new TableStateVm()
            {
                ActionRequied = "Disconnect"
            };

            await this.Clients.Client(clientId).SendAsync("Disconnect");

            await this.Groups.RemoveFromGroupAsync(clientId, gameId);
        }

        private async Task UpdateTabelState(string gameName)
        {
            var gameState = this.belotGameManager.Games[gameName];

            var tableState = this.mapper.Map<TableStateVm>(gameState.TableState);

            await this.Clients.Group(gameName).SendAsync(ChannelConstants.TableUpdate, tableState);
        }

        private async Task UpdateIndividualPlayerStates(string gameName, string eventType = null)
        {
            var gameSate = this.belotGameManager.Games[gameName];

            var gameStateVm = this.mapper.Map<GameStateVm>(gameSate);

            foreach (var item in gameStateVm.PlayerStates.Keys)
            {
                await this.Clients.Client(item).SendAsync(ChannelConstants.PlayerStateChange, gameStateVm.PlayerStates[item], eventType);
            }
        }

        //private async Task UpdateGameTask(string gameName)
        //{
        //    var gameState = this.belotGameManager.Games[gameName];
        //    var joinedPlayer = gameState.Players.Where(x => x.Id != null).ToList();
        //    foreach (var item in joinedPlayer)
        //    {
        //        try
        //        {
        //            var mappedState = this.mapper.Map<GameState, GameStateVm>(gameState,
        //            opt => opt.AfterMap((src, dest) =>
        //            {
        //                dest.PlayerSatate = dest.Players.FirstOrDefault(x => x.Id == item.Id);
        //                foreach (var destItem in dest.Players)
        //                {
        //                    destItem.Hand.Clear();
        //                }
        //            })
        //            );

        //            await this.Clients.Client(item.Id).SendAsync(ChannelNameConstants.GameChange, mappedState);
        //        }
        //        catch (Exception ex)
        //        {
        //            var s = 4;
        //        }

        //    }
        //    //return this.Clients.Group(gameName).SendAsync(ChannelNameConstants.GameChange, this.belotGameManager.Games[gameName]);
        //}
    }
}
