using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FabrykaINBD.Data;
using FabrykaINBD.Models;

namespace FabrykaINBD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HalaApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HalaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HalaApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hala>>> GetHale()
        {
            return await _context.Hale.ToListAsync();
        }

        // GET: api/HalaApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hala>> GetHala(int id)
        {
            var hala = await _context.Hale.FindAsync(id);

            if (hala == null)
            {
                return NotFound();
            }

            return hala;
        }

        // PUT: api/HalaApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHala(int id, Hala hala)
        {
            if (id != hala.Id)
            {
                return BadRequest();
            }

            _context.Entry(hala).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HalaExists(id))
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

        // POST: api/HalaApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hala>> PostHala(Hala hala)
        {
            _context.Hale.Add(hala);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHala", new { id = hala.Id }, hala);
        }

        // DELETE: api/HalaApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHala(int id)
        {
            var hala = await _context.Hale.FindAsync(id);
            if (hala == null)
            {
                return NotFound();
            }

            _context.Hale.Remove(hala);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HalaExists(int id)
        {
            return _context.Hale.Any(e => e.Id == id);
        }
    }
}
