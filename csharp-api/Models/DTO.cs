namespace csharp_api.Controllers;

public record InstitutionIn(string Name);
public record GroupIn(string Name, int Course, string Status = "active");
public record StudentIn(string FullName, int GroupId, int InstitutionId, string Status = "active");
public record ExamIn(string Name, DateOnly ExamDate, int GroupId);
public record ExamResultIn(int StudentId, int ExamId, int? Grade, string Status = "scheduled");

public record InstitutionOut(int Id, string Name);
public record GroupOut(int Id, string Name, int Course, string Status);
public record StudentOut(int Id, string FullName, int GroupId, int InstitutionId, string Status);
public record ExamOut(int Id, string Name, DateOnly ExamDate, int GroupId);
public record ExamResultOut(int Id, int StudentId, int ExamId, int? Grade, string Status);
