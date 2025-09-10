using RepositoryPattern.Models;
using RepositoryPattern.Repositories;
using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Models.Data;
using RepositoryPattern.Models;
using RepositoryPattern.Repositories;

namespace RepositoryPattern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _context;

        public UsersController(IUserRepository userRepo, AppDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
        }
        //this is getall
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userRepo.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepo.GetUserWithExpensesAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            await _userRepo.AddAsync(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = user.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _userRepo.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
