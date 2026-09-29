using System;
using System.Collections.Generic;

namespace csharp_api.Models;

public partial class Institution
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
