namespace AutoRef_API.Models
{
    public class PartidoModel
    {
        public string EquipoLocal { get; set; }    // Nombre del equipo local
        public string EquipoVisitante { get; set; } // Nombre del equipo visitante
        public DateTime Fecha { get; set; }         // Fecha del partido
        public string Hora { get; set; }          // Hora del partido
        public Guid LugarId { get; set; }           // ID del polideportivo (usando GUID, puede ser tipo int si el id es entero)
        public string Categoria { get; set; }       // Categoría del partido
        public string Competicion { get; set; }     // Competición en la que se juega el partido
      
    }
}
