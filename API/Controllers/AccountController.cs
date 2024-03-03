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
        public ActionResult<UserTokenDTO> Register(RegisterDTO registerDTO)
        {
            using var hmac = new HMACSHA512();
            bool isUserExist = UserExist(registerDTO.UserName);

            if (isUserExist) return BadRequest("Username is taken.");
            else
            {
                byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                byte[] salt = hmac.Key;
                string userNameLower = registerDTO.UserName.ToLower();

                string passwordHex = BitConverter.ToString(passwordHash).Replace("-", "");
                string saltHex = BitConverter.ToString(salt).Replace("-", "");

                var rollIdParam = new SqlParameter("@RollID", registerDTO.RollID);
                var firstNameParam = new SqlParameter("@FirstName", registerDTO.FirstName);
                var lastNameParam = new SqlParameter("@LastName", registerDTO.LastName);
                var houseNumberParam = new SqlParameter("@HouseNumber", registerDTO.HouseNumber);
                var userNameParam = new SqlParameter("@UserName", userNameLower);
                var passwordParam = new SqlParameter("@Password", passwordHex);
                var saltParam = new SqlParameter("@Salt", saltHex);
                var createdByIdParam = new SqlParameter("@CreatedByID", registerDTO.CreatedByID);

                // Calling the stored procedure with parameters
                _context.Users.FromSqlInterpolated($@"
            EXECUTE [security].[CreateUser] 
            {rollIdParam}, {firstNameParam}, {lastNameParam}, {houseNumberParam}, 
            {userNameParam}, {passwordParam}, {saltParam}, {createdByIdParam}");

                return BadRequest(1);
                User? user = GetUserByUserName(registerDTO.UserName);

                return new UserTokenDTO
                {
                    ID = user.ID,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }
        }
        [HttpPost("login")]
        public ActionResult<UserTokenDTO> Login(LoginDTO loginDTO)
        {
            bool isUserExist = UserExist(loginDTO.UserName);
            if (!isUserExist) return BadRequest("Username or Password is incorrect.");
            else
            {

                User user = GetUserByUserID(GetUserByUserName(loginDTO.UserName).ID);
                using var hmac = new HMACSHA512(key: Encoding.UTF8.GetBytes(user.Salt));
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password)).ToString();
                if (computeHash != user.Password)
                {
                    return BadRequest("Username or Password is incorrect.");
                }
                else
                {
                    _context.Users.FromSqlRaw($"[security].[UserLogin] @UserID ={user.ID}");
                    return new UserTokenDTO
                    {
                        ID = user.ID,
                        UserName = user.UserName,
                        Token = _tokenService.CreateToken(user)
                    };
                }
            }
        }

        private User GetUserByUserID(int id)
        {
            return _context.Users.FromSqlRaw($"[security].[GetUserByUserID] @UserID = {id}").FirstOrDefault();
        }

        private User GetUserByUserName(string userName)
        {
            User? user = _context.Users.FromSqlRaw($"[security].[GetUserByUserName] @UserName={userName.ToLower()}").AsEnumerable().FirstOrDefault();
            return user;
        }

        private bool UserExist(string userName)
        {
            User? user = GetUserByUserName(userName);

            if (user == null) return false;
            else return true;
        }
    }
}