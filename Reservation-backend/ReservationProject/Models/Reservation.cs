using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ReservationProject.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public string RoomNumber { get; set; }
        [Required]
        public string Reservationstatus { get; set; }
        [Required]
        public DateTime? Reservationdate { get; set; }
    }
}
