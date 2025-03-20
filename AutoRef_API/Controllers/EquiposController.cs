namespace AutoRef_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoRef_API.Database;
    using Microsoft.AspNetCore.Authorization;

    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private readonly AppDataBase _context;

        public EquiposController(AppDataBase context)
        {
            _context = context;
        }

        // GET: api/Equipos
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetEquipos()
        {
            var equipos = await _context.Equipos.ToListAsync();
            var equiposList = new List<object>();

            foreach (var equipo in equipos)
            {
                equiposList.Add(new
                {
                    equipo.Id,
                    equipo.Nombre,
                    equipo.ClubId,
                    equipo.CategoriaId
                });
            }

            return Ok(equiposList);
        }

        // GET: api/Equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(Guid id)
        {
            var equipo = await _context.Equipos.FindAsync(id);

            if (equipo == null)
            {
                return NotFound();
            }

            return equipo;
        }

        // GET: api/Equipos/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetEquipoByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            var equipo = await _context.Equipos
                .Where(e => e.Nombre.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return NotFound($"No se encontró un equipo con el nombre '{name}'.");
            }

            var result = new
            {
                equipo.Id,
                equipo.Nombre,
                equipo.ClubId,
                equipo.CategoriaId
            };

            return Ok(result);
        }

        // GET: api/Equipos/categoria/{categoriaId}
        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetEquiposByCategoria(Guid categoriaId)
        {
            var equipos = await _context.Equipos
                .Where(e => e.CategoriaId == categoriaId)
                .ToListAsync();

            // Devolver lista vacía si no hay equipos en lugar de NotFound
            var equiposList = equipos.Select(e => new
            {
                e.Id,
                e.Nombre,
                e.ClubId,
                e.CategoriaId
            }).ToList();

            return Ok(equiposList); // Siempre devolver una respuesta Ok con una lista vacía si no hay equipos
        }


        // PUT: api/Equipos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(Guid id, Equipo equipo)
        {
            if (id != equipo.Id)
            {
                return BadRequest();
            }

            _context.Entry(equipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipoExists(id))
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

        // POST: api/Equipos
        [HttpPost]
        public async Task<ActionResult<Equipo>> PostEquipo(Equipo equipo)
        {
            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipo", new { id = equipo.Id }, equipo);
        }

        // DELETE: api/Equipos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(Guid id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipoExists(Guid id)
        {
            return _context.Equipos.Any(e => e.Id == id);
        }
    }
}
