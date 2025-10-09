using CommonObjects.AppConstants;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CronjobServices;

public class PaymentJob
{
    private readonly IPaymentService _paymentService;

    public PaymentJob(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // Cancel if not paid anymore
    public async Task CancelIfUnpaidAsync(long orderCode)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
            {
                Console.WriteLine($"Không tìm thấy đơn hàng {orderCode}");
                return;
            }

            // if payment pending after ex:15mins => update status -> cancel
            if (!payment.Status.Equals(PaymentConstants.PAYMENT_PENDING))
            {
                Console.WriteLine($"Đơn {orderCode} chưa thanh toán, tiến hành hủy...");
                await _paymentService.CancelPurchaseHistoryForUser(orderCode);
            }
            else
            {
                Console.WriteLine($"Đơn {orderCode} đã thanh toán, không thể hủy.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi xử lý hủy đơn {orderCode}: {ex.Message}");
        }
    }
}
