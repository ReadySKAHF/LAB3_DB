using System;
using System.Collections.Generic;

namespace Models;

public partial class RepairOrder
{
    public int OrderId { get; set; }

    public int? CarId { get; set; }

    public int? MechanicId { get; set; }

    public DateOnly OrderDate { get; set; }

    public int? StatusId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ICollection<CarService> CarServices { get; set; } = new List<CarService>();

    public virtual Mechanic? Mechanic { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual CarStatus? Status { get; set; }
}
