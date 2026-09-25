-- Инициализация схемы MySQL (Часть I задания)
-- Выполняется автоматически при ПЕРВОМ запуске контейнера mysql
-- (папка данных должна быть пустой — том mysql_data не создан ранее)

-- ВАЖНО: без этой строки кириллица бьётся при вставке через docker-entrypoint,
-- так как клиент mysql по умолчанию может читать файл не в UTF-8
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS institutions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS student_groups (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    course INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'active'
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS students (
    id INT AUTO_INCREMENT PRIMARY KEY,
    full_name VARCHAR(255) NOT NULL,
    group_id INT NOT NULL,
    institution_id INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'active',
    CONSTRAINT fk_students_group
        FOREIGN KEY (group_id) REFERENCES student_groups(id),
    CONSTRAINT fk_students_institution
        FOREIGN KEY (institution_id) REFERENCES institutions(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS exams (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    exam_date DATE NOT NULL,
    group_id INT NOT NULL,
    CONSTRAINT fk_exams_group
        FOREIGN KEY (group_id) REFERENCES student_groups(id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS exam_results (
    id INT AUTO_INCREMENT PRIMARY KEY,
    student_id INT NOT NULL,
    exam_id INT NOT NULL,
    grade INT,
    status VARCHAR(50) NOT NULL DEFAULT 'scheduled',
    CONSTRAINT fk_results_student
        FOREIGN KEY (student_id) REFERENCES students(id),
    CONSTRAINT fk_results_exam
        FOREIGN KEY (exam_id) REFERENCES exams(id)
) ENGINE=InnoDB;

-- Демо-данные для проверки CRUD
INSERT INTO institutions (name) VALUES
    ('Колледж №1'),
    ('Колледж №2');

INSERT INTO student_groups (name, course, status) VALUES
    ('ИС-21', 2, 'active'),
    ('ПР-19', 3, 'active');

INSERT INTO students (full_name, group_id, institution_id, status) VALUES
    ('Иванов Иван Иванович', 1, 1, 'active'),
    ('Петров Петр Петрович', 2, 1, 'active');

INSERT INTO exams (name, exam_date, group_id) VALUES
    ('Программирование', '2026-01-15', 1),
    ('Базы данных', '2026-01-20', 2);

INSERT INTO exam_results (student_id, exam_id, grade, status) VALUES
    (1, 1, 5, 'passed'),
    (2, 2, 4, 'passed');
