using System;
using System.Collections.Generic;

namespace Models;

public partial class Car
{
    public int CarId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Brand { get; set; }

    public int? Power { get; set; }

    public string? Color { get; set; }

    public int? YearOfProduction { get; set; }

    public string? ChassisNumber { get; set; }

    public string? EngineNumber { get; set; }

    public DateOnly? DateReceived { get; set; }

    public int? OwnerId { get; set; }

    public int? StatusId { get; set; }

    public virtual Owner? Owner { get; set; }

    public virtual ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
    public override string ToString()
    {
        return CarId + ". " + LicensePlate + " " + Brand + " " + Power + " " + Color + " " + YearOfProduction + " " + ChassisNumber + " " + EngineNumber + " " + DateReceived;
    }
}
