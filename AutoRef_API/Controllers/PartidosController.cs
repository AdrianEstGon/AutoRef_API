using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoRef_API.Database;
using AutoRef_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AutoRef_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly AppDataBase _context;

        public PartidosController(AppDataBase context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Partidos
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPartidos()
        {
            // Obtener la lista de partidos desde la base de datos
            var partidos = await _context.Partidos
                .Include(p => p.Lugar)  // Incluir los detalles del lugar (Polideportivo)
                .Include(p => p.EquipoLocal)  // Incluir detalles del equipo local
                .Include(p => p.EquipoVisitante)  // Incluir detalles del equipo visitante
                .Include(p => p.Categoria)  // Incluir detalles de la categoría
                .Include(p => p.Arbitro1)  // Incluir detalles del primer árbitro
                .Include(p => p.Arbitro2)  // Incluir detalles del segundo árbitro
                .Include(p => p.Anotador)  // Incluir detalles del anotador
                .ToListAsync();

            var partidoList = new List<object>();

            foreach (var partido in partidos)
            {
                partidoList.Add(new
                {
                    partido.Id,
                    EquipoLocal = partido.EquipoLocal?.Nombre,
                    partido.EquipoLocalId,               
                    EquipoVisitante = partido.EquipoVisitante?.Nombre,
                    partido.EquipoVisitanteId,
                    partido.Fecha,
                    partido.Hora,
                    Lugar = partido.Lugar?.Nombre,  // Mostrar el nombre del lugar (Polideportivo)
                    partido.LugarId,
                    Categoria = partido.Categoria?.Nombre,
                    partido.CategoriaId,

                    partido.Jornada,
                    partido.NumeroPartido,
                    Arbitro1 = new
                    {
                        partido.Arbitro1?.Nombre,  
                        partido.Arbitro1?.PrimerApellido,
                        partido.Arbitro1?.SegundoApellido,
                    },
                    Arbitro2 = new
                    {
                        partido.Arbitro2?.Nombre,
                        partido.Arbitro2?.PrimerApellido,
                        partido.Arbitro2?.SegundoApellido,
                    },
                    Anotador = new
                    {
                        partido.Anotador?.Nombre,
                        partido.Anotador?.PrimerApellido,
                        partido.Anotador?.SegundoApellido,
                    }
                });
            }

            return Ok(partidoList);
        }

        // GET: api/Partidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Partido>> GetPartido(Guid id)
        {
            var partido = await _context.Partidos.Include(p => p.Lugar).FirstOrDefaultAsync(p => p.Id == id);

            if (partido == null)
            {
                return NotFound();
            }

            return partido;
        }

        // PUT: api/Partidos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartido(Guid id, Partido partido)
        {
            if (id != partido.Id)
            {
                return BadRequest();
            }

            _context.Entry(partido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartidoExists(id))
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

        [HttpPost("crearPartido")]
        public async Task<IActionResult> CrearPartido([FromBody] PartidoModel partidoModel)
        {
            if (partidoModel == null)
            {
                return BadRequest(new { message = "Los datos del partido no son válidos" });
            }

            // Convertir la fecha recibida (string) en un objeto DateTime
            DateTime fechaPartido = DateTime.Parse(partidoModel.Fecha.ToString("yyyy-MM-dd"));

            // Convertir la hora recibida (string) en un objeto TimeSpan
            TimeSpan horaPartido = TimeSpan.Parse(partidoModel.Hora);

            // Combinar la fecha y la hora en un único DateTime
            DateTime fechaHoraPartido = fechaPartido.Add(horaPartido);

            // Crear el partido con los valores correctos
            var partido = new Partido
            {
                LugarId = partidoModel.LugarId,
                Arbitro1Id = null,  // Asignar null a los árbitros
                Arbitro2Id = null,  // Asignar null a los árbitros
                AnotadorId = null,  // Asignar null al anotador si es necesario
                Fecha = fechaHoraPartido,  // Combinar fecha y hora en DateTime
                Hora = horaPartido, // Solo la hora como TimeSpan
                EquipoLocalId = partidoModel.EquipoLocalId,
                EquipoVisitanteId = partidoModel.EquipoVisitanteId,
                CategoriaId = partidoModel.CategoriaId,
                Jornada = partidoModel.Jornada,
                NumeroPartido = partidoModel.NumeroPartido
            };

            // Guardar el partido en la base de datos
            _context.Partidos.Add(partido);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Partido creado con éxito", partido });
        }

        // DELETE: api/Partidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartido(Guid id)
        {
            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null)
            {
                return NotFound(new { message = "El partido no existe." });
            }

            _context.Partidos.Remove(partido);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Partido eliminado con éxito." });
        }




        private bool PartidoExists(Guid id)
        {
            return _context.Partidos.Any(e => e.Id == id);
        }
    }
}

