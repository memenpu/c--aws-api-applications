using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class UsersController : ControllerBase
    {
        private readonly ReservationProejctDbContext _context;
        private readonly IMapper _mapper;
        public UsersController(ReservationProejctDbContext context, IMapper mapper)
        {
            _mapper = mapper;
        
        _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/3
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(x=>x.Email==email.ToLower());
            
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("update-user")]
        public async Task<ActionResult<User>> PutUser(User editUser)
        {
            try
            {

                _context.Entry(editUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return editUser;
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
        }
        
        // PUT: api/Users/
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == loginUserDto.Email && x.Password == loginUserDto.Password);
                
                if (user == null) return NotFound();
                else return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Email or Password is wrong!"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new System.Web.Http.HttpResponseException(response);
            }
        }

        /// <summary>
        /// Sign Up a User
        /// </summary>
        // POST: api/Users/sign-up
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("sign-up")]
        public async Task<ActionResult<User>> PostUser(EditUserDto signUpUserDto)
        {
            var user = _mapper.Map<User>(signUpUserDto);
            user.Email = user.Email.ToLower();
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.Email }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{email}")]
        public async Task<ActionResult<User>> DeleteUser(string email)
        {
            var user = await GetUser(email);
            if (user == null)
            {
                return NotFound();
            }
            _context.User.Remove(user.Value);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
