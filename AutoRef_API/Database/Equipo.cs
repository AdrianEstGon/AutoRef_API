namespace AutoRef_API.Database;


using System;

public class Equipo
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Nombre { get; set; }
    public string Categoria { get; set; }

    public virtual Club Club { get; set; }

}

