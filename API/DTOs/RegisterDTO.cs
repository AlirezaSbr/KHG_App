using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDTO
    {
        public required int RollID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? HouseNumber { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public int? CreatedByID { get; set; }
    }
}