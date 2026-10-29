using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ParcInformatiqueContext context) : ControllerBase
{
    [HttpGet] // GET: api/users
    public async Task<ActionResult<List<User>>> GetAllUsers() 
    {
        return Ok(await context.Users.ToListAsync());
    }

    [HttpGet("{login}")]  // GET: api/users/{login}
    public async Task<ActionResult<User>> GetUserByLogin(string login)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(user => user.Login == login);

        return user is null ? NotFound() : Ok(user);
    }
}
