using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationProject.Data;
using ReservationProject.Mappings;
using ReservationProject.Models;

namespace ReservationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationProejctDbContext _context;

          private readonly IMapper _mapper;
        public ReservationsController(ReservationProejctDbContext context, IMapper mapper)
        {
            _mapper = mapper;

            _context = context;
        }
        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservation()
        {
            return await _context.Reservation.ToListAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // PUT: api/Reservations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reservations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }
        // POST: api/Reservations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("book-a-room/{userId}")]
        public async Task<ActionResult<User>> PostReservation(int userId, RoomDto roomDto)
        {
            try
            {
                Reservation reservation = _mapper.Map<Reservation>(roomDto);
                reservation.Reservationstatus = "Confirmed";
                reservation.UserId = userId;
                Reservation Conflict = await _context.Reservation.FirstOrDefaultAsync(x => roomDto.RoomNumber == x.RoomNumber && x.Reservationdate.Value.Date== roomDto.Reservationdate.Value.Date);
                if (Conflict==null) { 
                    _context.Reservation.Add(reservation);
                    await _context.SaveChangesAsync();
                    //return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
                    return await _context.User.FirstOrDefaultAsync(x=>x.Id == userId);
                }
            }
            catch (Exception)
            {

            }
            return BadRequest("Reservation Conflict");
        }


        // POST: api/Reservations
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpGet("check-room-availbility")]
        public async Task<ActionResult<bool>> CheckReservationConflict(string roomNumber, DateTime reservationdate)
        {
            try
            {   Reservation Conflict = await _context.Reservation.FirstOrDefaultAsync(x => roomNumber == x.RoomNumber && x.Reservationdate.Value.Date == reservationdate.Date);
                return Conflict == null;
            }
            catch (Exception)
            {
            }
            return BadRequest("Reservation Conflict");
        }


        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reservation>> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return reservation;
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
