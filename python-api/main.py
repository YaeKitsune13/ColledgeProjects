import os
from contextlib import contextmanager
from datetime import date

from fastapi import APIRouter, FastAPI, HTTPException
from firebird.driver import DatabaseError, connect
from pydantic import BaseModel

app = FastAPI()


@contextmanager
def db():
    con = connect(
        f"{os.getenv('DB_HOST')}/{os.getenv('DB_PORT', '3050')}:{os.getenv('DB_NAME')}",
        user=os.getenv("DB_USER"),
        password=os.getenv("DB_PASSWORD"),
        charset="UTF8",
    )
    try:
        yield con
    finally:
        con.close()


def rows(cur):
    names = [d[0].lower() for d in cur.description]
    return [dict(zip(names, r)) for r in cur.fetchall()]


# ---- модели (без id) ----
class Institution(BaseModel):
    name: str

class Group(BaseModel):
    name: str
    course: int
    status: str = "active"

class Student(BaseModel):
    full_name: str
    group_id: int
    institution_id: int
    status: str = "active"

class Exam(BaseModel):
    name: str
    exam_date: date
    group_id: int

class ExamResult(BaseModel):
    student_id: int
    exam_id: int
    grade: int | None = None
    status: str = "scheduled"


def crud(path: str, table: str, model: type[BaseModel]):
    cols = list(model.model_fields)
    col_list = ", ".join(cols)
    router = APIRouter(prefix=f"/{path}", tags=[path])

    @router.get("")
    def list_all():
        with db() as con:
            cur = con.cursor()
            cur.execute(f"SELECT id, {col_list} FROM {table} ORDER BY id")
            return rows(cur)

    @router.get("/{id}")
    def get_one(id: int):
        with db() as con:
            cur = con.cursor()
            cur.execute(f"SELECT id, {col_list} FROM {table} WHERE id = ?", (id,))
            r = rows(cur)
            if not r:
                raise HTTPException(404, "Not found")
            return r[0]

    @router.post("", status_code=201)
    def create(item: model):
        with db() as con:
            cur = con.cursor()
            try:
                cur.execute(
                    f"INSERT INTO {table} ({col_list}) "
                    f"VALUES ({', '.join('?' * len(cols))}) RETURNING id",
                    tuple(getattr(item, c) for c in cols),
                )
                new_id = cur.fetchone()[0]
                con.commit()
            except DatabaseError as e:
                con.rollback()
                raise HTTPException(400, str(e))
        return {"id": new_id, **item.model_dump()}

    @router.put("/{id}")
    def update(id: int, item: model):
        with db() as con:
            cur = con.cursor()
            try:
                cur.execute(
                    f"UPDATE {table} SET {', '.join(c + ' = ?' for c in cols)} WHERE id = ?",
                    (*(getattr(item, c) for c in cols), id),
                )
                if cur.rowcount == 0:
                    raise HTTPException(404, "Not found")
                con.commit()
            except DatabaseError as e:
                con.rollback()
                raise HTTPException(400, str(e))
        return {"id": id, **item.model_dump()}

    @router.delete("/{id}", status_code=204)
    def delete(id: int):
        with db() as con:
            cur = con.cursor()
            try:
                cur.execute(f"DELETE FROM {table} WHERE id = ?", (id,))
                if cur.rowcount == 0:
                    raise HTTPException(404, "Not found")
                con.commit()
            except DatabaseError:
                con.rollback()
                raise HTTPException(409, "Есть связанные записи")

    app.include_router(router)


crud("institutions", "institutions", Institution)
crud("student-groups", "student_groups", Group)
crud("students", "students", Student)
crud("exams", "exams", Exam)
crud("exam-results", "exam_results", ExamResult)


@app.get("/")
def read_root():
    return {"status": "ok", "service": "python-api"}


@app.get("/health")
def health_check():
    try:
        with db() as con:
            cur = con.cursor()
            cur.execute("SELECT 1 FROM rdb$database")
            cur.fetchone()
        return {"db": "ok"}
    except Exception as e:
        raise HTTPException(503, f"db error: {e}")
