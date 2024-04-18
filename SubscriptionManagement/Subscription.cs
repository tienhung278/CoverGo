namespace SubscriptionManagement;

public class Subscription
{
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public decimal PricePerPeriod { get; set; }
    
    public InvoiceLine BillSubscription(DateTime billingEndDate)
    {
        // Calculate the duration of the billing period in days
        decimal duration = (decimal)(billingEndDate - Start).TotalDays;

        // If the has ended and the end date is before the billing end date,
        // adjust the duration
        if (End.HasValue && End.Value < billingEndDate)
        {
            duration = (decimal)(End.Value - Start).TotalDays;
        }

        // Calculate the total price for the billing interval
        decimal total = PricePerPeriod * duration;

        // Create the invoice line
        var invoiceLine = new InvoiceLine
        {
            Start = Start,
            End = billingEndDate,
            PricePerPeriod = PricePerPeriod,
            Duration = duration,
            Total = total
        };

        return invoiceLine;
    }
}