using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DaviesIdeas.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using DaviesIdeas.Data;
using Microsoft.EntityFrameworkCore;

namespace DaviesIdeas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public const string SessionKeyName = "_Name";
      
        //public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly DataContext _dataContext;
        public AuthController(IConfiguration configuration, IUserService userService, DataContext dataContext)
        {
            _configuration = configuration;
            _userService = userService;
            _dataContext = dataContext;
        }
        [HttpGet, Authorize(Roles = "Admin, User")]
        public ActionResult<object> GetMe()
        {
            try
            {
                var userName = _userService.GetMyName();
                var role = _userService.GetMyRole();
                var email = _userService.GetMyEmail();
                var myID = _userService.GetMyId();

                return Ok(new { userName, email, role, myID });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
            
        }
        [HttpPost("register")]

        public async Task<ActionResult<User>> Register(RegisterUser registerReq)
        {
            try
            {
                //Check if username is registered
                var registeredUser = (from c in _dataContext.Users
                                      where c.Username.Equals(registerReq.Username)
                                      select c).SingleOrDefault();

                if (registeredUser != null)
                {
                    return BadRequest("Username taken.");
                }
                //Hashes password.
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerReq.Password);
                User user = new User
                {
                    Username = registerReq.Username,
                    PasswordHash = passwordHash,
                    Email = registerReq.Email,
                    Role = registerReq.Role
                };

                _dataContext.Add(user);
                await _dataContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpPost("login")]

        public ActionResult<User> Login(LoginUser loginReq)
        {
            try
            {
                var validUser = (from c in _dataContext.Users
                                 where c.Username.Equals(loginReq.Username)
                                 select c).SingleOrDefault();
                //Search user
                if (validUser == null)
                {
                    return BadRequest("User not found.");
                }

                if (!BCrypt.Net.BCrypt.Verify(loginReq.Password, validUser.PasswordHash))
                {
                    return BadRequest("Wrong Password.");
                }

                HttpContext.Session.SetString(SessionVariables.SessionKeyUsername, validUser.Username);
                HttpContext.Session.SetInt32(SessionVariables.SessionKeyId, validUser.Id);


                string token = CreateToken(validUser);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        private string CreateToken(User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Hash, user.Id.ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value!));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: creds
                    );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;
            } catch (Exception e)
            {
                return null;
            }
        }
    }
}
