using System;
using System.Collections.Generic;

namespace csharp_api.Models;

public partial class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public int GroupId { get; set; }

    public int InstitutionId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual StudentGroup Group { get; set; } = null!;

    public virtual Institution Institution { get; set; } = null!;
}
