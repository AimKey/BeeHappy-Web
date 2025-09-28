using System;

namespace CommonObjects.DTOs.PaymentDTOs;

public record class CreatePaymentLinkRequest(
    string productName,
    string description,
    int price,
    string returnUrl,
    string cancelUrl
);
