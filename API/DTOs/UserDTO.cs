using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserDTO
    {
        public int ID { get; set; }
        public int RollID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? HouseNumber { get; set; }
        public string UserName { get; set; }
        public int? CreatedByID { get; set; }
    }
}