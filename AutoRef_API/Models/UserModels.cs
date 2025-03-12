public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
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
    public bool EsAdmin { get; set; } // Para indicar si el usuario debe tener rol Admin
}


public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RoleModel
{
    public string RoleName { get; set; }
}

public class AssignRoleModel
{
    public string Username { get; set; }
    public string RoleName { get; set; }
}
