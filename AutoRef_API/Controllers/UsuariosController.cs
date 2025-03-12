using AutoRef_API.Database;

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
    private const string GoogleMapsApiKey = "TU_API_KEY_AQUI"; // Reemplaza con tu clave de API

    public UsuariosController(
        UserManager<Usuario> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _httpClient = new HttpClient();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Obtener las coordenadas (geolocalización)
        var coordenadas = await ObtenerCoordenadas(model.Direccion, model.Ciudad, model.Pais);

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
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Asigna "Admin" si EsAdmin es true

            // Agregar el rol al usuario
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

    private async Task<(double Latitud, double Longitud)> ObtenerCoordenadas(string direccion, string ciudad, string pais)
    {
        var direccionCompleta = $"{direccion}, {ciudad}, {pais}";
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(direccionCompleta)}&key={GoogleMapsApiKey}";

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
        var token = GenerateJwtToken(user, roles);

        return Ok(new
        {
            message = "Inicio de sesión exitoso",
            token,
            roles
        });
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
