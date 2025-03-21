namespace AutoRef_API.Models
{
    public class PartidoModel
    {
        public Guid? EquipoLocalId { get; set; }    // Nombre del equipo local
        public Guid? EquipoVisitanteId { get; set; } // Nombre del equipo visitante
        public DateTime Fecha { get; set; }         // Fecha del partido
        public string Hora { get; set; }          // Hora del partido
        public Guid? LugarId { get; set; }           // ID del polideportivo (usando GUID, puede ser tipo int si el id es entero)
        public Guid? CategoriaId { get; set; }       // Categoría del partido                                         
        public int Jornada { get; set; }            // Jornada del partido
        public int NumeroPartido { get; set; }      // Número del partido

    }

    public class UpdatePartidoModel
    {
        public Guid Id { get; set; }
        public Guid? EquipoLocalId { get; set; }    // Nombre del equipo local
        public Guid? EquipoVisitanteId { get; set; } // Nombre del equipo visitante
        public DateTime Fecha { get; set; }         // Fecha del partido
        public string Hora { get; set; }          // Hora del partido
        public Guid? LugarId { get; set; }           // ID del polideportivo (usando GUID, puede ser tipo int si el id es entero)
        public Guid? CategoriaId { get; set; }       // Categoría del partido                                         
        public int Jornada { get; set; }            // Jornada del partido
        public int NumeroPartido { get; set; }      // Número del partido

    }
}
