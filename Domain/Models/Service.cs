using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class CachedDataService
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<CarService> CarServices { get; set; } = new List<CarService>();
}
