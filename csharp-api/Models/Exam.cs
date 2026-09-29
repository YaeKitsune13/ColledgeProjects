using System;
using System.Collections.Generic;

namespace csharp_api.Models;

public partial class Exam
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly ExamDate { get; set; }

    public int GroupId { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual StudentGroup Group { get; set; } = null!;
}
