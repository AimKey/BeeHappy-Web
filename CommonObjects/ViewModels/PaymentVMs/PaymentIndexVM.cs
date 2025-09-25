namespace CommonObjects.ViewModels.PaymentVMs;

public class PaymentIndexVM
{
    public List<PaymentPlanVM> PaymentPlans { get; set; } = new();
    public string? CurrentUserId { get; set; }
}