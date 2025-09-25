using CommonObjects.DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Http;

namespace Services.HelperServices;

public static class PaymentUtils
{
    public static PayOSResponseObj ParseResponse(IQueryCollection response)
    {
        if (response == null || response.Count == 0)
        {
            throw new ArgumentException("Response is null or empty", nameof(response));
        }
        var payOsResponse = new PayOSResponseObj
        {
            Code = response["code"],
            Id = response["id"],
            Cancel = response["cancel"] == "true",
            Status = response["status"],
            OrderCode = long.TryParse(response["orderCode"], out var orderCode) ? orderCode : -1
        };
        return payOsResponse;
    }
}