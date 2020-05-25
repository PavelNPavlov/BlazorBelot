using AutoMapper;
using Belot.Core.DeckCore;
using Belot.Core.GameStateModels;
using CardGames.Models.Belot;

namespace CardGames.Server.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            this.CreateMap<GameState, GameStateVm>();
            this.CreateMap<Player, PlayerVm>();
            this.CreateMap<PlayerState, PlayerStateVm>();
            this.CreateMap<TableState, TableStateVm>();
            this.CreateMap<Card, CardVm>();
            this.CreateMap<RoundState, RoundStateVm>();
            this.CreateMap<Announcement, AnnouncementVm>();
            this.CreateMap<AnnouncementVm, Announcement>();
        }
    }
}
