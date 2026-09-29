using csharp_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_api.Controllers;

[ApiController]
[Route("api/institutions")]
public class InstitutionsController(MydbContext db) : ControllerBase
{
    static InstitutionOut ToOut(Institution e) => new(e.Id, e.Name);

    [HttpGet]
    public async Task<IEnumerable<InstitutionOut>> GetAll() =>
        (await db.Institutions.AsNoTracking().ToListAsync()).Select(ToOut);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InstitutionOut>> Get(int id) =>
        await db.Institutions.FindAsync(id) is { } e ? ToOut(e) : NotFound();

    [HttpPost]
    public async Task<ActionResult<InstitutionOut>> Create(InstitutionIn dto)
    {
        var e = new Institution { Name = dto.Name };
        db.Institutions.Add(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToOut(e));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InstitutionOut>> Update(int id, InstitutionIn dto)
    {
        var e = await db.Institutions.FindAsync(id);
        if (e is null) return NotFound();
        e.Name = dto.Name;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return BadRequest(ex.InnerException?.Message ?? ex.Message); }
        return ToOut(e);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Institutions.FindAsync(id);
        if (e is null) return NotFound();
        db.Institutions.Remove(e);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { return Conflict("Есть связанные записи"); }
        return NoContent();
    }
}
