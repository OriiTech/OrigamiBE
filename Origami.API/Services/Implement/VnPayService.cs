using Microsoft.AspNetCore.Http;
using Origami.API.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Origami.API.Services.Implement;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VnPayService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string CreatePaymentUrl(int transactionId, decimal amount, string ipAddress)
    {
        var vnp_TmnCode = _configuration["VnPay:TmnCode"] ?? "99ZIDBU8";
        var vnp_HashSecret = _configuration["VnPay:HashSecret"] ?? "3R8B658PZZPP77QFY8FDCY4XNOE14FNP";
        var vnp_Url = _configuration["VnPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        var vnp_ReturnUrl = _configuration["VnPay:ReturnUrl"] ?? "https://origamibe-api.onrender.com/api/v1/wallet/vnpay-callback";

        var vnp_Params = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((long)(amount * 100)).ToString() }, // Convert to cents
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", transactionId.ToString() },
            { "vnp_OrderInfo", $"Nap tien vao vi - Transaction {transactionId}" },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", vnp_ReturnUrl },
            { "vnp_IpAddr", ipAddress },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
        };

        // Sort params by key
        var sortedParams = vnp_Params.OrderBy(x => x.Key).ToList();

        // Create query string
        var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        // Create secure hash
        var signData = queryString;
        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);

        // Add hash to params
        queryString += $"&vnp_SecureHash={vnp_SecureHash}";

        return $"{vnp_Url}?{queryString}";
    }

    public bool ValidateSignature(Dictionary<string, string> vnpayData, string vnp_SecureHash)
    {
        var vnp_HashSecret = _configuration["VnPay:HashSecret"] ?? "3R8B658PZZPP77QFY8FDCY4XNOE14FNP";

        // Remove vnp_SecureHash from data
        var dataToCheck = vnpayData
            .Where(x => x.Key != "vnp_SecureHash" && x.Key != "vnp_SecureHashType")
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        var signData = string.Join("&", dataToCheck.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
        var checkSum = HmacSHA512(vnp_HashSecret, signData);

        return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    public Dictionary<string, string> ParseCallbackData(string queryString)
    {
        var result = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(queryString))
            return result;

        var pairs = queryString.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                result[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]);
            }
        }

        return result;
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);

        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}
