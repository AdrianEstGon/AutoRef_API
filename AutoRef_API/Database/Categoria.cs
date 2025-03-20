namespace AutoRef_API.Database
{
    public class Categoria
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool PrimerArbitro { get; set; }
        public bool SegundoArbitro { get; set; }
        public bool Anotador { get; set; }
        public int MinArbitros { get; set; }

    }
}
