using csharp_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController(MydbContext db) : ControllerBase
{
    static StudentOut ToOut(Student e) => new(e.Id, e.FullName, e.GroupId, e.InstitutionId, e.Status);

    [HttpGet]
    public async Task<IEnumerable<StudentOut>> GetAll() =>
        (await db.Students.AsNoTracking().ToListAsync()).Select(ToOut);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentOut>> Get(int id) =>
        await db.Students.FindAsync(id) is { } e ? ToOut(e) : NotFound();

    [HttpPost]
    public async Task<ActionResult<StudentOut>> Create(StudentIn dto)
    {
        var e = new Student { FullName = dto.FullName, GroupId = dto.GroupId, InstitutionId = dto.InstitutionId, Status = dto.Status };
        db.Students.Add(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToOut(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentOut>> Update(int id, StudentIn dto)
    {
        var e = await db.Students.FindAsync(id);
        if (e is null) return NotFound();
        e.FullName = dto.FullName; e.GroupId = dto.GroupId; e.InstitutionId = dto.InstitutionId; e.Status = dto.Status;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return ToOut(e);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Students.FindAsync(id);
        if (e is null) return NotFound();
        db.Students.Remove(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { return Conflict("Есть связанные записи"); }
        return NoContent();
    }
}
