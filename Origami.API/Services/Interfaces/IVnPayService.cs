namespace Origami.API.Services.Interfaces;

public interface IVnPayService
{
    string CreatePaymentUrl(int transactionId, decimal amount, string ipAddress);
    bool ValidateSignature(Dictionary<string, string> vnpayData, string vnp_SecureHash);
    Dictionary<string, string> ParseCallbackData(string queryString);
}
