using csharp_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_api.Controllers;

[ApiController]
[Route("api/exam-results")]
public class ExamResultsController(MydbContext db) : ControllerBase
{
    static ExamResultOut ToOut(ExamResult e) => new(e.Id, e.StudentId, e.ExamId, e.Grade, e.Status);

    [HttpGet]
    public async Task<IEnumerable<ExamResultOut>> GetAll() =>
        (await db.ExamResults.AsNoTracking().ToListAsync()).Select(ToOut);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExamResultOut>> Get(int id) =>
        await db.ExamResults.FindAsync(id) is { } e ? ToOut(e) : NotFound();

    [HttpPost]
    public async Task<ActionResult<ExamResultOut>> Create(ExamResultIn dto)
    {
        var e = new ExamResult { StudentId = dto.StudentId, ExamId = dto.ExamId, Grade = dto.Grade, Status = dto.Status };
        db.ExamResults.Add(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToOut(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExamResultOut>> Update(int id, ExamResultIn dto)
    {
        var e = await db.ExamResults.FindAsync(id);
        if (e is null) return NotFound();
        e.StudentId = dto.StudentId; e.ExamId = dto.ExamId; e.Grade = dto.Grade; e.Status = dto.Status;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return ToOut(e);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.ExamResults.FindAsync(id);
        if (e is null) return NotFound();
        db.ExamResults.Remove(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { return Conflict("Есть связанные записи"); }
        return NoContent();
    }
}
