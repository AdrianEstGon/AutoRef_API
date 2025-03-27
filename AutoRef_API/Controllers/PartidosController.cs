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
                .Include(p => p.Categoria)  // Incluir detalles de la categor�a
                .Include(p => p.Arbitro1)  // Incluir detalles del primer �rbitro
                .Include(p => p.Arbitro2)  // Incluir detalles del segundo �rbitro
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
                    partido.Arbitro1Id,
                    partido.Arbitro2Id,
                    partido.AnotadorId
                   
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
        public async Task<IActionResult> UpdatePartido(Guid id, [FromBody] UpdatePartidoModel partidoModel)
        {
            if (id != partidoModel.Id)
            {
                return BadRequest(new { message = "El ID del partido no coincide." });
            }

            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null)
            {
                return NotFound(new { message = "El partido no existe." });
            }

            // Actualizar propiedades del partido con los datos recibidos
            partido.EquipoLocalId = partidoModel.EquipoLocalId;
            partido.EquipoVisitanteId = partidoModel.EquipoVisitanteId;
            partido.Fecha = partidoModel.Fecha;
            partido.Hora = TimeSpan.Parse(partidoModel.Hora);
            partido.LugarId = partidoModel.LugarId;
            partido.CategoriaId = partidoModel.CategoriaId;
            partido.Jornada = partidoModel.Jornada;
            partido.NumeroPartido = partidoModel.NumeroPartido;
            partido.Arbitro1Id = partidoModel.Arbitro1Id;
            partido.Arbitro2Id = partidoModel.Arbitro2Id;
            partido.AnotadorId = partidoModel.AnotadorId;

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

            return Ok(new { message = "Partido actualizado con �xito." });
        }


        [HttpPost("crearPartido")]
        public async Task<IActionResult> CrearPartido([FromBody] PartidoModel partidoModel)
        {
            if (partidoModel == null)
            {
                return BadRequest(new { message = "Los datos del partido no son v�lidos" });
            }

            // Convertir la fecha recibida (string) en un objeto DateTime
            DateTime fechaPartido = DateTime.Parse(partidoModel.Fecha.ToString("yyyy-MM-dd"));

            // Convertir la hora recibida (string) en un objeto TimeSpan
            TimeSpan horaPartido = TimeSpan.Parse(partidoModel.Hora);

            // Combinar la fecha y la hora en un �nico DateTime
            DateTime fechaHoraPartido = fechaPartido.Add(horaPartido);

            // Crear el partido con los valores correctos
            var partido = new Partido
            {
                LugarId = partidoModel.LugarId,
                Arbitro1Id = null,  // Asignar null a los �rbitros
                Arbitro2Id = null,  // Asignar null a los �rbitros
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

            return Ok(new { message = "Partido creado con �xito", partido });
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

            return Ok(new { message = "Partido eliminado con �xito." });
        }




        private bool PartidoExists(Guid id)
        {
            return _context.Partidos.Any(e => e.Id == id);
        }
    }
}

