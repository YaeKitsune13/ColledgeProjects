using System;
using System.Collections.Generic;

namespace csharp_api.Models;

public partial class ExamResult
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int ExamId { get; set; }

    public int? Grade { get; set; }

    public string Status { get; set; } = null!;

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
