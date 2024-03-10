using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.UserName)) return BadRequest("Username is taken!");
            using var hmac = new HMACSHA512();
            var user = new User
            {
                RollID = (int)registerDTO.RollID,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                HouseNumber = registerDTO.HouseNumber,
                UserName = registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key,
                CreatedAtDatetime = DateTime.Now
            };
            await _context.Users.AddAsync(user);
            return new UserDTO
            {
                ID = user.ID,
                RollID = user.RollID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                HouseNumber = user.HouseNumber,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName.ToLower());
            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(key: user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDTO
            {
                ID = user.ID,
                RollID = user.RollID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                HouseNumber = user.HouseNumber,
                UserName = user.UserName
            }
            ;
        }


        private async Task<bool> UserExist(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }

    }
}