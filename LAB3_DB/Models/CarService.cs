using System;
using System.Collections.Generic;

namespace Models;

public partial class CarService
{
    public int CarServiceId { get; set; }

    public int? OrderId { get; set; }

    public int? ServiceId { get; set; }

    public int? MechanicId { get; set; }

    public DateOnly WorkDate { get; set; }

    public virtual Mechanic? Mechanic { get; set; }

    public virtual RepairOrder? Order { get; set; }

    public virtual CachedDataService? Service { get; set; }
}
