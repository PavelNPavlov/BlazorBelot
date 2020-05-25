using BelotBots;
using CardGames.Models.Lobbies;
using CardGames.Shared;
using LobbyManager;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace CardGames.Server.Hubs
{
    public class LobbyHub : Hub
    {

        private readonly LobbyManagerService lobbyManagerService;


        public LobbyHub(LobbyManagerService lobbyManagerService)
        {
            this.lobbyManagerService = lobbyManagerService;

            if (!this.lobbyManagerService.Initialized)
            {
                this.lobbyManagerService.Initialize();
            }
        }


        public async Task GetLobbies(string type)
        {
            await this.UpdateLobbies(type);
        }

        public async Task JoinLobby(LobbyConnectRequest lobbyConnectRequest)
        {
            var lobby = this.lobbyManagerService.Lobbies.FirstOrDefault(x => x.Name == lobbyConnectRequest.LobbyName);
            var type = lobby.Type;
            var conenctResponse = new LobbyConnectResponse();

            var lobbyUser = new LobbyUser()
            {
                Id = this.Context.ConnectionId,
                Name = lobbyConnectRequest.UserName
            };

           

            if (lobby.LobbyUsers.Count < 4)
            {
                
                if(lobby.LobbyUsers.Count == 0)
                {
                    lobby.HostId = lobbyUser.Id;
                }               

                lobby.LobbyUsers.Add(lobbyUser);


                conenctResponse.Succeded = true;
                conenctResponse.LobbyName = lobby.Name;

                await this.Clients.Caller.SendAsync(ChannelConstants.LobbyConnectionAnswer, conenctResponse);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, lobby.Name);

                await this.UpdateLobbies(type);
            }
            else
            {
                conenctResponse.Succeded = false;
                conenctResponse.LobbyName = lobby.Name;

                await this.Clients.Caller.SendAsync(ChannelConstants.LobbyConnectionAnswer, conenctResponse);
            }

        }

        public async Task AddBotToLobby(string lobbyName)
        {
            var lobby = this.lobbyManagerService.Lobbies.FirstOrDefault(x => x.Name == lobbyName);

            if(lobby.LobbyUsers.Count == 4)
            {
                return;
            }

            BotsManager.CreateBotForLobby(lobby.Name);

            await this.UpdateLobbies(lobby.Type);
        }

        public async Task LeaveLobby(LobbyDisconnectRequest req)
        {
            var lobby = this.lobbyManagerService.Lobbies.FirstOrDefault(x => x.Name == req.LobbyName);
            var type = lobby.Type;

            var userToRemove = lobby.LobbyUsers.FirstOrDefault(x => x.Id == this.Context.ConnectionId);

            lobby.LobbyUsers.Remove(userToRemove);

            if (lobby.LobbyUsers.Count > 0)
            {
                lobby.HostId = lobby.LobbyUsers[0].Id;
            }

            var disconnectResponse = new LobbyDisconnectResponse();

            disconnectResponse.Succeded = true;
            disconnectResponse.LobbyName = lobby.Name;

            await this.Clients.Caller.SendAsync(ChannelConstants.LobbyDisconectAnswer, disconnectResponse);
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, lobby.Name);

            await this.UpdateLobbies(type);


        }

        public async Task StartGame(string lobbyName)
        {
            await this.Clients.Group(lobbyName).SendAsync(ChannelConstants.GameStarting);
        }


        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        private Task UpdateLobbies(string type)
        {
            var lobbies = this.lobbyManagerService.Lobbies.Where(x => x.Type == type).ToList();
            return this.Clients.All.SendAsync(ChannelConstants.LobbiesDataChannel, lobbies);
        }
    }
}
