using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using TaskManager.Data.Entities;

namespace TaskManager.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("getById/{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("getByUsername/{username}")]
        public IActionResult GetByUsername(string username)
        {
            var user = _userService.GetByUsername(username);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserModel user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(new User { Username = user.Username, Password = user.Password });
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserModel user)
        {
            var result = await _userService.UpdateAsync(id, new User { Id = id, Username = user.Username, Password = user.Password });
            if (!result.Item1)
                throw new Exception("Something went wrong!!");

            return Ok(result.Item2);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return Ok("Deleted");
        }
    }
}
