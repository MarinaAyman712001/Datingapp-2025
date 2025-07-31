using API.Data;
using API.entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MembersController : BaseApiController
    {
        private readonly AppDbContext _context;

        public MembersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
    [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            return user;
        }
    }
}
