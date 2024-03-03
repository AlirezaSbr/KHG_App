using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //api/users/2
        [HttpGet("{id}")]

        public async Task<ActionResult<User>> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        // TODO:
        //api/users/Alireza
        // [HttpGet("{UserName}")]

        // public ActionResult<User> GetUserByUserName(string UserName)
        // {
        //     User? user = _context.Users.FromSqlRaw($"[security].[GetUserByUserName] @UserName={UserName.ToLower()}").AsEnumerable().FirstOrDefault();
        //     return user;
        // }
    }
}