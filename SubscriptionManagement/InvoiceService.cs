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

        var index = 0;
        discounts = discounts.OrderBy(d => d.Start).ToList();
        for (int i = 0; i < discounts.Count; i++)
        {
            if (discounts[i].Start > currentStart)
            {
                // Add the period before the discount starts
                var invLine = new InvoiceLine
                {
                    Index = ++index,
                    Start = currentStart,
                    End = discounts[i].Start,
                    PricePerPeriod = invoiceLine.PricePerPeriod,
                    Duration = (decimal)(discounts[i].Start - currentStart).TotalDays,
                    Total = invoiceLine.PricePerPeriod * (decimal)(discounts[i].Start - currentStart).TotalDays
                };
                invoiceLines.Add(invLine);

                if (invoiceLines.Count > 1 && invLine.Start == discounts[i - 1].End)
                {
                    invLine.Duration = (decimal)(invLine.End - invLine.Start).TotalDays - 0.5M;
                    invLine.Total = invLine.PricePerPeriod * invLine.Duration;

                    var prevInvLine = invoiceLines.First(inv => inv.Index == invLine.Index - 1);
                    prevInvLine.Duration = (decimal)(prevInvLine.End - prevInvLine.Start).TotalDays + 0.5M;
                    prevInvLine.Total = prevInvLine.PricePerPeriod * prevInvLine.Duration;
                }

                currentStart = discounts[i].Start;
            }

            // Apply the discount
            DateTime discountEnd = discounts[i].End > billingEndDate ? billingEndDate : discounts[i].End;
            invoiceLines.Add(new InvoiceLine
            {
                Index = ++index,
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
            var invLine = new InvoiceLine
            {
                Index = ++index,
                Start = currentStart,
                End = billingEndDate,
                PricePerPeriod = invoiceLine.PricePerPeriod,
                Duration = (decimal)(billingEndDate - currentStart).TotalDays,
                Total = invoiceLine.PricePerPeriod * (decimal)(billingEndDate - currentStart).TotalDays
            };
            invoiceLines.Add(invLine);
            
            var prevInvLine = invoiceLines.First(inv => inv.Index == invLine.Index - 1);
            if (invoiceLines.Count > 1)
            {
                invLine.Duration = (decimal)(invLine.End - invLine.Start).TotalDays - 0.5M;
                invLine.Total = invLine.PricePerPeriod * invLine.Duration;

                prevInvLine.Duration = (decimal)(prevInvLine.End - prevInvLine.Start).TotalDays + 0.5M;
                prevInvLine.Total = prevInvLine.PricePerPeriod * prevInvLine.Duration;
            }
        }

        return invoiceLines;
    }
}