using csharp_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_api.Controllers;

[ApiController]
[Route("api/exams")]
public class ExamsController(MydbContext db) : ControllerBase
{
    static ExamOut ToOut(Exam e) => new(e.Id, e.Name, e.ExamDate, e.GroupId);

    [HttpGet]
    public async Task<IEnumerable<ExamOut>> GetAll() =>
        (await db.Exams.AsNoTracking().ToListAsync()).Select(ToOut);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExamOut>> Get(int id) =>
        await db.Exams.FindAsync(id) is { } e ? ToOut(e) : NotFound();

    [HttpPost]
    public async Task<ActionResult<ExamOut>> Create(ExamIn dto)
    {
        var e = new Exam { Name = dto.Name, ExamDate = dto.ExamDate, GroupId = dto.GroupId };
        db.Exams.Add(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToOut(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExamOut>> Update(int id, ExamIn dto)
    {
        var e = await db.Exams.FindAsync(id);
        if (e is null) return NotFound();
        e.Name = dto.Name; e.ExamDate = dto.ExamDate; e.GroupId = dto.GroupId;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return ToOut(e);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Exams.FindAsync(id);
        if (e is null) return NotFound();
        db.Exams.Remove(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { return Conflict("Есть связанные записи"); }
        return NoContent();
    }
}
