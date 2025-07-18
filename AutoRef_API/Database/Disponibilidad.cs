namespace AutoRef_API.Database;

using System;
using AutoRef_API.Enum;

using Microsoft.EntityFrameworkCore;

[Index(nameof(UsuarioId), nameof(Fecha), IsUnique = true)]
public class Disponibilidad
{
    public Guid Id { get; private set; }
    public Guid? UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public FranjaDisponibilidad Franja1 { get; set; }
    public FranjaDisponibilidad Franja2 { get; set; }
    public FranjaDisponibilidad Franja3 { get; set; }
    public FranjaDisponibilidad Franja4 { get; set; }
    public string? Comentarios { get; set; }

    public virtual Usuario? Usuario { get; set; }
}



