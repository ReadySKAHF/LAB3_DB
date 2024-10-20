using System;
using System.Collections.Generic;

namespace Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    public string DriverLicenseNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
