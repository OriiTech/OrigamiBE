using AutoMapper;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class WalletModule : Profile
    {
        public WalletModule()
        {
            CreateMap<Wallet, GetWalletResponse>();
        }
    }
}
