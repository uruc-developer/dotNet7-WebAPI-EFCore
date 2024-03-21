using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthrepository _authRepo;

        public AuthController(IAuthrepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(AddUserDto request){
            var responce = await _authRepo.Register(new Models.User {Username = request.Username}, request.Password );
            if (!responce.Success)
            {
                return BadRequest(responce);
            }
            return Ok(responce);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request){
            var responce = await _authRepo.Login(request.Username, request.Password );
            if (!responce.Success)
            {
                return BadRequest(responce);
            }
            return Ok(responce);
        }

    }
}