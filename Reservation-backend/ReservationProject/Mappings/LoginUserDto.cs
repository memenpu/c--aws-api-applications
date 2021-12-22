using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationProject.Models;

namespace ReservationProject.Mappings
{
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        //public User Uesr { get => new User { Email = Email, Password = Password }; }
    }
}
