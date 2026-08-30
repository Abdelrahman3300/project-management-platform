using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStackProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost] public async Task<ActionResult<UserDto>> Create( CreateUserRequest request) { try { var user = await _userService.CreateAsync(request); return CreatedAtAction( nameof(GetById), new { id = user.Id }, user); } catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } }
}