﻿namespace SubscriptionManagement;

public class InvoiceLine
{
    public int Index { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public decimal PricePerPeriod { get; set; }
    public decimal Duration { get; set; }
    public decimal Total { get; set; }
}