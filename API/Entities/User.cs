using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class User
    {
        public int ID { get; set; }
        public int RollID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? HouseNumber { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[]? Token { get; set; }
        public DateTime CreatedAtDatetime { get; set; }
        public DateTime? LastLoginDatetime { get; set; }
        public int? UpdatedByID { get; set; }
        public DateTime? UpdatedAtDatetime { get; set; }
    }

}