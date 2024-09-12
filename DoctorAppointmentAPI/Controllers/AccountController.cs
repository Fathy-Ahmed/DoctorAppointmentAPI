using Entities.DTO;
using Entities.Models;
using Entities.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities;

namespace DoctorAppointmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAuthenticationServices _authServices;
        private readonly IConfiguration config;

        public AccountController
            (UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IConfiguration config, IAuthenticationServices authServices)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._authServices = authServices;
            this.config = config;
        }

        //---------------------------------------------------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authServices.RegisterAsync(registerDTO);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        //---------------------------------------------------------------------------------------------
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            TokenRequstModel tokenRequstModel = new()
            {
                Email = loginDTO.Email,
                Password=loginDTO.Password,
            };
            var result= await _authServices.GetTokenAsync(tokenRequstModel);
            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);

        }

        //---------------------------------------------------------------------------------------------


    }

}
