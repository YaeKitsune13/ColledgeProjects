using System;
using System.Collections.Generic;

namespace csharp_api.Models;

public partial class StudentGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Course { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
