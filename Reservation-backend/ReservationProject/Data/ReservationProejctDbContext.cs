using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationProject.Models;

namespace ReservationProject.Data
{
    public class ReservationProejctDbContext : DbContext
    {
        public ReservationProejctDbContext (DbContextOptions<ReservationProejctDbContext> options)
            : base(options)
        {
        
        }

        public DbSet<ReservationProject.Models.User> User { get; set; }

        public DbSet<ReservationProject.Models.Reservation> Reservation { get; set; }

    }
}
