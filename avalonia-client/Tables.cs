namespace AvaloniaClient;

public record Field(string Name, string Label, bool IsNumber = false, string Default = "");

public record Table(string Path, string Title, Field[] Fields)
{
    public override string ToString() => Title;
}

public static class Tables
{
    public static readonly Table[] All =
    [
        new("institutions", "Учреждения",
        [
            new("name", "Название"),
        ]),
        new("student-groups", "Группы",
        [
            new("name", "Название"),
            new("course", "Курс", IsNumber: true),
            new("status", "Статус", Default: "active"),
        ]),
        new("students", "Студенты",
        [
            new("fullName", "ФИО"),
            new("groupId", "ID группы", IsNumber: true),
            new("institutionId", "ID учреждения", IsNumber: true),
            new("status", "Статус", Default: "active"),
        ]),
        new("exams", "Экзамены",
        [
            new("name", "Название"),
            new("examDate", "Дата (гггг-мм-дд)"),
            new("groupId", "ID группы", IsNumber: true),
        ]),
        new("exam-results", "Результаты",
        [
            new("studentId", "ID студента", IsNumber: true),
            new("examId", "ID экзамена", IsNumber: true),
            new("grade", "Оценка", IsNumber: true),
            new("status", "Статус", Default: "scheduled"),
        ]),
    ];
}
