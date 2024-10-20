using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? OrderId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string Employee { get; set; } = null!;

    public virtual RepairOrder? Order { get; set; }
}
