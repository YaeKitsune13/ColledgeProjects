// Базовый адрес API (nginx проксирует /api/ -> python-api:8000)
export const API_BASE = "/api";

// Описание ресурсов: путь -> название вкладки и поля [имя, подпись, тип, значение по умолчанию]
export const RESOURCES = {
  institutions: {
    title: "Учреждения",
    fields: [["name", "Название", "text"]],
  },
  "student-groups": {
    title: "Группы",
    fields: [
      ["name", "Название", "text"],
      ["course", "Курс", "number"],
      ["status", "Статус", "text", "active"],
    ],
  },
  students: {
    title: "Студенты",
    fields: [
      ["full_name", "ФИО", "text"],
      ["group_id", "ID группы", "number"],
      ["institution_id", "ID учреждения", "number"],
      ["status", "Статус", "text", "active"],
    ],
  },
  exams: {
    title: "Экзамены",
    fields: [
      ["name", "Название", "text"],
      ["exam_date", "Дата", "date"],
      ["group_id", "ID группы", "number"],
    ],
  },
  "exam-results": {
    title: "Результаты",
    fields: [
      ["student_id", "ID студента", "number"],
      ["exam_id", "ID экзамена", "number"],
      ["grade", "Оценка", "number"],
      ["status", "Статус", "text", "scheduled"],
    ],
  },
};
