using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoRef_API.Database;
using Microsoft.AspNetCore.Authorization;

namespace AutoRef_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PolideportivosController : ControllerBase
    {
        private readonly AppDataBase _context;

        public PolideportivosController(AppDataBase context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPolideportivos()
        {
            var polideportivos = await _context.Polideportivos.ToListAsync();
            var polideportivosList = new List<object>();

            foreach (var polideportivo in polideportivos)
            {
                polideportivosList.Add(new
                {
                    polideportivo.Id,
                    polideportivo.Nombre,
                    polideportivo.Latitud,
                    polideportivo.Longitud
                });
            }

            return Ok(polideportivosList);
        }


        // GET: api/Polideportivos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Polideportivo>> GetPolideportivo(Guid id)
        {
            var polideportivo = await _context.Polideportivos.FindAsync(id);

            if (polideportivo == null)
            {
                return NotFound();
            }

            return polideportivo;
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPolideportivoByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            // Realizamos la búsqueda insensible a mayúsculas
            var polideportivo = await _context.Polideportivos
                .Where(p => p.Nombre.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (polideportivo == null)
            {
                return NotFound($"No se encontró un polideportivo con el nombre '{name}'.");
            }

            // Devolvemos el polideportivo con los campos relevantes
            var result = new
            {
                polideportivo.Id,
                polideportivo.Nombre,
                polideportivo.Latitud,
                polideportivo.Longitud
            };

            return Ok(result);
        }



        // PUT: api/Polideportivos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPolideportivo(Guid id, Polideportivo polideportivo)
        {
            if (id != polideportivo.Id)
            {
                return BadRequest();
            }

            _context.Entry(polideportivo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PolideportivoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Polideportivos
        [HttpPost]
        public async Task<ActionResult<Polideportivo>> PostPolideportivo(Polideportivo polideportivo)
        {
            _context.Polideportivos.Add(polideportivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPolideportivo", new { id = polideportivo.Id }, polideportivo);
        }

        // DELETE: api/Polideportivos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolideportivo(Guid id)
        {
            var polideportivo = await _context.Polideportivos.FindAsync(id);
            if (polideportivo == null)
            {
                return NotFound();
            }

            _context.Polideportivos.Remove(polideportivo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PolideportivoExists(Guid id)
        {
            return _context.Polideportivos.Any(e => e.Id == id);
        }
    }
}

