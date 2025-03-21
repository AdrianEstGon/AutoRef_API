using AutoRef_API.Database;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly AppDataBase _context;

    public RolesController(AppDataBase context)
    {
        _context = context;
    }

    
}
