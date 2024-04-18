namespace SubscriptionManagement;

public class InvoiceService
{
    public List<Discount> Discounts { get; set; }

    public List<InvoiceLine> BillSubscriptionWithDiscounts(Subscription subscription, DateTime billingEndDate,
        List<Discount> discounts)
    {
        // Generate the invoice line without discounts
        InvoiceLine invoiceLine = subscription.BillSubscription(billingEndDate);

        // Apply the discounts
        List<InvoiceLine> invoiceLines = new List<InvoiceLine>();
        DateTime currentStart = invoiceLine.Start;

        discounts = discounts.OrderBy(d => d.Start).ToList();
        for (int i = 0; i < discounts.Count; i++)
        {
            if (discounts[i].Start > currentStart)
            {
                // Add the period before the discount starts
                invoiceLines.Add(new InvoiceLine
                {
                    Start = currentStart,
                    End = discounts[i].Start,
                    PricePerPeriod = invoiceLine.PricePerPeriod,
                    Duration = (decimal)(discounts[i].Start - currentStart).TotalDays,
                    Total = invoiceLine.PricePerPeriod * (decimal)(discounts[i].Start - currentStart).TotalDays
                });

                currentStart = discounts[i].Start;
            }

            // Apply the discount
            DateTime discountEnd = discounts[i].End > billingEndDate ? billingEndDate : discounts[i].End;
            invoiceLines.Add(new InvoiceLine
            {
                Start = currentStart,
                End = discountEnd,
                PricePerPeriod = invoiceLine.PricePerPeriod * (1 - discounts[i].Percentage),
                Duration = (decimal)(discountEnd - currentStart).TotalDays,
                Total = invoiceLine.PricePerPeriod * (1 - discounts[i].Percentage) *
                        (decimal)(discountEnd - currentStart).TotalDays
            });

            currentStart = discountEnd;
        }

        if (currentStart < billingEndDate)
        {
            // Add the period after the last discount ends
            invoiceLines.Add(new InvoiceLine
            {
                Start = currentStart,
                End = billingEndDate,
                PricePerPeriod = invoiceLine.PricePerPeriod,
                Duration = (decimal)(billingEndDate - currentStart).TotalDays,
                Total = invoiceLine.PricePerPeriod * (decimal)(billingEndDate - currentStart).TotalDays
            });
        }

        return invoiceLines;
    }
}