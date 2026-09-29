using csharp_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_api.Controllers;

[ApiController]
[Route("api/student-groups")]
public class StudentGroupsController(MydbContext db) : ControllerBase
{
    static GroupOut ToOut(StudentGroup e) => new(e.Id, e.Name, e.Course, e.Status);

    [HttpGet]
    public async Task<IEnumerable<GroupOut>> GetAll() =>
        (await db.StudentGroups.AsNoTracking().ToListAsync()).Select(ToOut);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GroupOut>> Get(int id) =>
        await db.StudentGroups.FindAsync(id) is { } e ? ToOut(e) : NotFound();

    [HttpPost]
    public async Task<ActionResult<GroupOut>> Create(GroupIn dto)
    {
        var e = new StudentGroup { Name = dto.Name, Course = dto.Course, Status = dto.Status };
        db.StudentGroups.Add(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToOut(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GroupOut>> Update(int id, GroupIn dto)
    {
        var e = await db.StudentGroups.FindAsync(id);
        if (e is null) return NotFound();
        e.Name = dto.Name; e.Course = dto.Course; e.Status = dto.Status;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return ToOut(e);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.StudentGroups.FindAsync(id);
        if (e is null) return NotFound();
        db.StudentGroups.Remove(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { return Conflict("Есть связанные записи"); }
        return NoContent();
    }
}
