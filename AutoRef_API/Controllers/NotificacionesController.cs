﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoRef_API.Database;
using Microsoft.AspNetCore.Authorization;
using AutoRef_API.Models;
using AutoRef_API.Services;
using AutoRef_API.Migrations;

namespace AutoRef_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly AppDataBase _context;

        public NotificacionesController(AppDataBase context)
        {
            _context = context;
        }

        // GET: api/Notificaciones
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotificaciones()
        {
            var notificaciones = await _context.Notificaciones
                .Include(n => n.Usuario)
                .ToListAsync();

            var result = notificaciones.Select(n => new
            {
                n.Id,
                n.UsuarioId,
                n.Mensaje,
                n.Fecha,
                n.Leida
            });

            return Ok(result);
        }

        // GET: api/Notificaciones/usuario/{usuarioId}
        [Authorize]
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetNotificacionesPorUsuario(Guid usuarioId)
        {
            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();

            var result = notificaciones.Select(n => new
            {
                n.Id,
                n.UsuarioId,
                n.Mensaje,
                n.Fecha,
                n.Leida
            });

            return Ok(result);
        }

        // GET: api/Notificaciones/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Notificacion>> GetNotificacion(Guid id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);

            if (notificacion == null)
            {
                return NotFound();
            }

            return notificacion;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Notificacion>> PostNotificacion([FromBody] NotificacionModel notificacionModel)
        {
            if (notificacionModel == null || string.IsNullOrWhiteSpace(notificacionModel.Mensaje))
            {
                return BadRequest(new { message = "Los datos de la notificación no son válidos." });
            }

            var notificacion = new Notificacion
            {
                UsuarioId = notificacionModel.UsuarioId,
                Mensaje = notificacionModel.Mensaje,
                Fecha = notificacionModel.Fecha,
                Leida = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            Usuario user = await _context.Users.FindAsync(notificacionModel.UsuarioId);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }
            try
            {
                var mailService = new MailService();
                await mailService.SendEmailAsync(user.Email, "Nueva designación", $"Hola {user.Nombre},\n\n{notificacion.Mensaje}\n\n¡Saludos!");
            }
            catch (Exception ex)
            {
                // Log del error para que puedas investigar
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return StatusCode(500, new { message = "Error al enviar el correo." });
            }


            return Ok(new { message = "Notificacion creada con éxito" });
        }

        // PUT: api/Notificaciones/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotificacion(Guid id, UpdateNotificacionModel notificacion)
        {
            // Buscar la notificación existente en la base de datos por su Id
            var existingNotificacion = await _context.Notificaciones.FindAsync(id);

            if (existingNotificacion == null)
            {
                // Si no se encuentra, devolver un NotFound
                return NotFound();
            }

            // Actualizar todos los campos con los valores proporcionados
            existingNotificacion.Mensaje = notificacion.Mensaje;
            existingNotificacion.Fecha = notificacion.Fecha;
            existingNotificacion.Leida = notificacion.Leida;
            existingNotificacion.UsuarioId = notificacion.UsuarioId;

            try
            {
                // Marcar la entidad como modificada y guardar los cambios en la base de datos
                _context.Entry(existingNotificacion).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si hay problemas de concurrencia (por ejemplo, la notificación ya fue modificada por otro proceso)
                if (!NotificacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar 204 No Content indicando que la actualización fue exitosa
            return NoContent();
        }



        // DELETE: api/Notificaciones/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificacion(Guid id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);

            if (notificacion == null)
            {
                // Ya fue eliminada
                return Ok(new { message = "La notificación ya fue eliminada." });
            }

            try
            {
                _context.Notificaciones.Remove(notificacion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Algo la eliminó entre FindAsync y SaveChangesAsync, tratamos como éxito
                return Ok(new { message = "La notificación ya no existía al intentar eliminarla." });
            }

            return Ok(new { message = "Notificación eliminada correctamente." });
        }


        private bool NotificacionExists(Guid id)
        {
            return _context.Notificaciones.Any(n => n.Id == id);
        }
    }
}
