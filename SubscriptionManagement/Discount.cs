namespace SubscriptionManagement;

public class Discount
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public decimal Percentage { get; set; }
    public bool IsStartOverlapped { get; set; }
    public bool IsEndOverlapped { get; set; }
}