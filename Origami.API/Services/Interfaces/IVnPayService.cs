using Origami.BusinessTier.Payload.Wallet;
using System.Collections.Generic;

namespace Origami.API.Services.Interfaces;

public interface IVnPayService
{
    Task<TopUpResponse> TopUpWallet(TopUpRequest request);
    Task<bool> ProcessVnpayCallback(Dictionary<string, string> vnpayData);
}

