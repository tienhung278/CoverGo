using SubscriptionManagement;

var subscription = new Subscription
{
    Start = new DateTime(2017, 3, 3),
    PricePerPeriod = 10
};

var billingEnd = new DateTime(2017, 5, 3);

var discounts = new List<Discount>
{
    new Discount
    {
        Start = new DateTime(2017, 3, 3),
        End = new DateTime(2017, 3, 17),
        Percentage = 0.5M
    },
    new Discount
    {
        Start = new DateTime(2017, 3, 10),
        End = new DateTime(2017, 4, 10),
        Percentage = 0.2M
    }
};

var invoiceService = new InvoiceService();
var invoiceLines = invoiceService.BillSubscriptionWithDiscounts(subscription, billingEnd, discounts);
foreach (var invoiceLine in invoiceLines)
{
    Console.WriteLine(
        $"{invoiceLine.Start:D}, to {invoiceLine.End:D}: {invoiceLine.PricePerPeriod:C2}, {invoiceLine.Duration} days, Total: {invoiceLine.Total:C2}");
}