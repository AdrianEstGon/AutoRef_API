using AutoRef_API.Database;
using AutoRef_API.Services;

using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly HttpClient _httpClient;
    private readonly Cloudinary _cloudinary;
    private const string GoogleMapsApiKey = "TU_API_KEY_AQUI"; // Reemplaza con tu clave de API

    public UsuariosController(
        UserManager<Usuario> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<Usuario> signInManager,
        Cloudinary cloudinary)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _httpClient = new HttpClient();
        _cloudinary = cloudinary;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExistente = await _userManager.FindByEmailAsync(model.Email);
        if (userExistente != null)
        {
            return BadRequest(new { message = "El correo electrónico ya está registrado" });
        }
        // Obtener las coordenadas (geolocalización)
        var coordenadas = await ObtenerCoordenadas(model.Direccion, model.Ciudad, model.Pais);

        // Generar la contraseña
        var contraseñaGenerada = GenerarContraseña(model.Nombre); // Usa la función para generar la contraseña

        // Crear el objeto Usuario
        var user = new Usuario
        {
            UserName = model.Username,
            Email = model.Email,
            Nombre = model.Nombre,
            PrimerApellido = model.PrimerApellido,
            SegundoApellido = model.SegundoApellido,
            FechaNacimiento = model.FechaNacimiento,
            Nivel = model.Nivel,
            ClubVinculado = model.ClubVinculado,
            Licencia = model.Licencia,
            Direccion = model.Direccion,
            Pais = model.Pais,
            Region = model.Region,
            Ciudad = model.Ciudad,
            CodigoPostal = model.CodigoPostal,
            Latitud = coordenadas.Latitud,
            Longitud = coordenadas.Longitud
        };

        // Registrar el usuario
        var result = await _userManager.CreateAsync(user, contraseñaGenerada);

        if (result.Succeeded)
        {
            // Enviar la contraseña por correo
            var mailService = new MailService();  // Crear instancia del servicio de correo
            await mailService.SendEmailAsync(model.Email, "Tu nueva contraseña", $"Hola {model.Nombre},\n\nTu nueva contraseña es: {contraseñaGenerada}\n\nSaludos!");

            // Asigna "Admin" si EsAdmin es true
            if (model.EsAdmin)
            {
                var role = "Admin";
                await _userManager.AddToRoleAsync(user, role);
                return Ok(new { message = "Usuario registrado con éxito", role });
            }

            return Ok(new { message = "Usuario registrado con éxito" });
        }

        return BadRequest(result.Errors);
    }

    // Función para generar una contraseña aleatoria (con símbolos, letras y números)
    private string GenerarContraseña(string nombre)
    {
        var random = new Random();
        var longitud = 12; // Longitud de la contraseña
        var caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>?/";
        var contrasena = new StringBuilder();

        // Agregar al menos una letra mayúscula, una minúscula, un número y un símbolo
        contrasena.Append(nombre.Substring(0, 1).ToUpper()); // Usar la primera letra del nombre en mayúscula
        contrasena.Append('a'); // Garantizar que haya una letra minúscula
        contrasena.Append('1'); // Garantizar que haya un número
        contrasena.Append('!'); // Garantizar que haya un símbolo

        // Generar el resto de la contraseña
        for (int i = contrasena.Length; i < longitud; i++)
        {
            contrasena.Append(caracteres[random.Next(caracteres.Length)]);
        }

        // Barajar la contraseña para que no siempre sea en el mismo orden
        var contrasenaFinal = contrasena.ToString().ToCharArray();
        random.Shuffle(contrasenaFinal);

        return new string(contrasenaFinal);
    }



    private async Task<(double Latitud, double Longitud)> ObtenerCoordenadas(string direccion, string ciudad, string pais)
    {
        var direccionCompleta = $"{direccion}, {ciudad}, {pais}";
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(direccionCompleta)}&key=AIzaSyC24LaFVU6RgtEswKeAvrryUFBg7CBgONQ";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return (0, 0);
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(jsonResponse);

        if (data.status == "OK")
        {
            var location = data.results[0].geometry.location;
            return ((double)location.lat, (double)location.lng);
        }
        return (0, 0);
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        var roles = await _userManager.GetRolesAsync(user);

        // Generar el token JWT
        var token = GenerateJwtToken(user, roles);

        return Ok(new
        {
            message = "Inicio de sesión exitoso",
            token,   // Agrega el token en la respuesta
            role = roles.FirstOrDefault(),// Devuelve el rol del usuario
            id = user.Id
        });
    }

    /// <summary>
    /// Crea un nuevo rol en el sistema.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
    {
        if (!await _roleManager.RoleExistsAsync(model.RoleName))
        {
            var role = new ApplicationRole { Name = model.RoleName };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Ok(new { message = "Rol creado con éxito" });

            return BadRequest(result.Errors);
        }

        return BadRequest(new { message = "El rol ya existe" });
    }

    /// <summary>
    /// Asigna un rol a un usuario.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);

        if (result.Succeeded)
            return Ok(new { message = "Rol asignado con éxito" });

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Obtiene la lista de usuarios con sus roles.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.ToList();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.UserName,
                user.ClubVinculado,
                user.Licencia,
                user.Nivel,
                user.Nombre,
                user.PrimerApellido,
                user.SegundoApellido,
                user.FechaNacimiento,
                user.FotoPerfil,
                user.Longitud,
                user.Latitud,
                user.Email,
                Roles = roles,
                user.CodigoPostal,
                user.Ciudad,
                user.Direccion,
                user.Pais,
                user.Region
            });
        }

        return Ok(userList);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.ClubVinculado,
            user.Licencia,
            user.Nivel,
            user.Nombre,
            user.PrimerApellido,
            user.SegundoApellido,
            user.FechaNacimiento,
            user.FotoPerfil,
            user.Longitud,
            user.Latitud,
            user.Email,
            Roles = roles,
            user.CodigoPostal,
            user.Ciudad,
            user.Direccion,
            user.Pais,
            user.Region
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Obtener los roles asignados al usuario
        var roles = await _userManager.GetRolesAsync(user);

        // Remover cada rol asignado
        foreach (var role in roles)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        // Eliminar el usuario
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "Usuario y roles eliminados con éxito" });
        }

        return BadRequest(new { message = "Error al eliminar el usuario", errors = result.Errors });
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        user.Nombre = model.Nombre;
        user.PrimerApellido = model.PrimerApellido;
        user.SegundoApellido = model.SegundoApellido;
        user.FechaNacimiento = model.FechaNacimiento;
        user.Nivel = model.Nivel;
        user.ClubVinculado = model.ClubVinculado;
        user.Licencia = model.Licencia;
        user.Direccion = model.Direccion;
        user.Pais = model.Pais;
        user.Region = model.Region;
        user.Ciudad = model.Ciudad;
        user.CodigoPostal = model.CodigoPostal;

        // Obtener las coordenadas actualizadas
        var coordenadas = await ObtenerCoordenadas(model.Direccion, model.Ciudad, model.Pais);
        user.Latitud = coordenadas.Latitud;
        user.Longitud = coordenadas.Longitud;

        // Verificar si el usuario ya tiene el rol de administrador
        var esAdminActual = await _userManager.IsInRoleAsync(user, "Admin");

        if (model.EsAdmin && !esAdminActual)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else if (!model.EsAdmin && esAdminActual)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "Usuario actualizado con éxito" });
        }

        return BadRequest(new { message = "Error al actualizar el usuario", errors = result.Errors });
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded)
        {
            return Ok(new { message = "Contraseña actualizada con éxito" });
        }

        // Aquí puedes devolver un mensaje de error específico si la contraseña actual es incorrecta
        if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
        {
            return BadRequest(new { message = "La contraseña actual no es correcta" });
        }

        return BadRequest(new { message = "Error al cambiar la contraseña", errors = result.Errors });
    }

    [Authorize]
    [HttpPut("upload-profile-picture/{userId}")]
    public async Task<IActionResult> UploadProfilePicture(string userId, IFormFile file)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se proporcionó ninguna imagen" });
        }

        // Cargar la imagen a Cloudinary
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream())
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == HttpStatusCode.OK)
        {
            // Obtener la URL pública de la imagen
            var imageUrl = uploadResult.SecureUrl.ToString();

            // Guardar la URL de la imagen en la base de datos
            user.FotoPerfil = imageUrl;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Foto de perfil actualizada con éxito", imageUrl = imageUrl });
            }
        }

        return BadRequest(new { message = "Error al cargar la imagen", details = uploadResult.Error?.Message });
    }


    [Authorize]
    [HttpGet("profile-picture")]
    public async Task<IActionResult> GetProfilePicture()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Verificar si el usuario tiene una URL válida de Cloudinary
        if (string.IsNullOrEmpty(user.FotoPerfil))
        {
            return NotFound(new { message = "No se ha establecido una foto de perfil" });
        }

        // Si la URL de la foto está en Cloudinary, devolverla como una URL de redirección
        return Redirect(user.FotoPerfil); // Redirige a la imagen en Cloudinary
    }








    private string GenerateJwtToken(Usuario user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuClaveSecretaQueEsLoSuficientementeLargaParaCumplirConLosRequisitosDeHS256"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TuIssuer",
            audience: "TuAudience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
