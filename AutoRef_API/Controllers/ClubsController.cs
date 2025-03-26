using AutoRef_API.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoRef_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController: ControllerBase
    {
        private readonly AppDataBase _context;

        public ClubsController(AppDataBase context)
        {
            _context = context;
        }

        // GET: api/Clubs
        [HttpGet]
        public async Task<IActionResult> GetClubs()
        {
            var clubs = await _context.Clubs.ToListAsync();
            var clubsList = new List<object>();

            foreach (var club in clubs)
            {
                clubsList.Add(new
                {
                    club.Id,
                    club.Nombre
                });
            }

            return Ok(clubsList);
        }

        // GET: api/Clubs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClub(Guid id)
        {
            var club = await _context.Clubs.FindAsync(id);

            if (club == null)
            {
                return NotFound();
            }

            return club;
        }
    
    }
}
