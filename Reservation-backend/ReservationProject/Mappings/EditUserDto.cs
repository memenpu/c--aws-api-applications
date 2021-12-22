using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationProject.Models;

namespace ReservationProject.Mappings
{
    public class EditUserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public User Uesr { get => new User { Email = Email, Password = Password, Firstname=Firstname,Lastname=Lastname }; }
    }
}
