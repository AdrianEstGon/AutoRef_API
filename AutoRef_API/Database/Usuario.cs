using System;
using Microsoft.SqlServer.Types;  // Necesario para SqlGeography

namespace AutoRef_API.Database;

using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole
{
    // Puedes añadir propiedades personalizadas para roles si lo necesitas
}

public class Usuario : IdentityUser
{
    public string Email { get; set; }
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Nivel { get; set; }
    public string ClubVinculado { get; set; }
    public int Licencia { get; set; }
    public string Direccion { get; set; }
    public string Pais { get; set; }
    public string Region { get; set; }
    public string Ciudad { get; set; }
    public string CodigoPostal { get; set; }
    public double Latitud { get; set; }  // Se obtiene con Google Maps API
    public double Longitud { get; set; }
    // Propiedad para la foto de perfil (almacenada como un array de bytes)
    public byte[] FotoPerfil { get; set; }

    // Propiedad para la ubicación (tipo GEOGRAPHY)
    public SqlGeography Ubicacion
    {
        get
        {
            // Convertimos las coordenadas en un objeto SqlGeography
            return SqlGeography.Point(Latitud, Longitud, 4326); // SRID 4326 es el sistema de coordenadas geográficas WGS 84
        }
    }
    public Usuario() : base()
    {
        Nombre = string.Empty;
        PrimerApellido = string.Empty;
        SegundoApellido = string.Empty;
        Email = string.Empty;
        //Clave = string.Empty;
        ClubVinculado = string.Empty;
        Nivel = string.Empty;
        FotoPerfil = Array.Empty<byte>();
        FechaNacimiento = DateTime.Now;
        Ciudad = string.Empty;
        Pais = string.Empty;
        Region = string.Empty;
        Direccion = string.Empty;
        CodigoPostal = string.Empty;
        Latitud = 0;
        Longitud = 0;
    }
}
