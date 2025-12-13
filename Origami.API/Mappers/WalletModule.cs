using AutoMapper;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;

namespace Origami.API.Mappers;

public class WalletModule : Profile
{
    public WalletModule()
    {
        CreateMap<Wallet, GetWalletResponse>()
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance ?? 0));
        CreateMap<Transaction, TransactionResponse>();
    }
}

