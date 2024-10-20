using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Mechanic
{
    public int MechanicId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Qualification { get; set; }

    public int ExperienceYears { get; set; }

    public decimal? Salary { get; set; }

    public virtual ICollection<CarService> CarServices { get; set; } = new List<CarService>();

    public virtual ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
}
