using ReservationProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationProject.Mappings
{
    public class RoomDto
    {
        public string RoomNumber { get; set; }
        public DateTime? Reservationdate { get; set; }

    }
}
