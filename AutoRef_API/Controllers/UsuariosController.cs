using AutoRef_API.Database;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

    public UsuariosController(
        UserManager<Usuario> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new Usuario
        {
            UserName = model.Username,
            Email = model.Email,
            //NombreCompleto = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok(new { message = "Usuario registrado con éxito" });
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
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
            role = roles.FirstOrDefault() // Devuelve el rol del usuario
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
//    [Authorize(Roles = "Admin")]
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
                Roles = roles
            });
        }

        return Ok(userList);
    }

    private string GenerateJwtToken(Usuario user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Agregar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Usa una clave secreta de al menos 128 bits
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuClaveSecretaQueEsLoSuficientementeLargaParaCumplirConLosRequisitosDeHS256"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TuIssuer",  // Puedes poner tu dominio o algo relevante
            audience: "TuAudience",  // También tu dominio o algo relevante
            claims: claims,
            expires: DateTime.Now.AddHours(1),  // Duración del token
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
