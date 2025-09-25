namespace CommonObjects.ViewModels.PaymentVMs;

public class BadgeProgressVM
{
    public string CurrentLevel { get; set; } = string.Empty;
    public string NextLevel { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
}
