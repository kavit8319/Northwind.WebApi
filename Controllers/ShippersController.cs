using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippersController : CustomControllerBase
    {
        public ShippersController(NorthwindContext context,IMapper map):base(context,map)
        {
        }

        // GET: api/Shippers
        [HttpGet("/SelectAllShippers",Name = "SelectAllShippers")]
        public async Task<ActionResult<IEnumerable<ShipperReturn>>> SelectAllShippers()
        {
            var res= await context.Shippers.ToListAsync();
            return mapper.Map<List<ShipperReturn>>(res);
        }

        // PUT: api/Shippers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("/EditShipper",Name = "EditShipper")]
        public async Task<IActionResult> EditShipper([FromBody] ShipperReturn shipper)
        {
            var ship = mapper.Map<Shipper>(shipper);

            context.Entry(ship).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipperExists(ship.ShipperId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(shipper);
        }

        // POST: api/Shippers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/AddShipper",Name = "AddShipper")]
        public async Task<ActionResult<ShipperReturn>> AddShipper([FromBody]ShipperReturn shipper)
        {
            if (context.Shippers == null)
            {
                return Problem("Entity set 'NorthwindContext.Shippers'  is null.");
            }

            var newShipper = mapper.Map<Shipper>(shipper);
            context.Shippers.Add(newShipper);
            
            await context.SaveChangesAsync();
            shipper.Id = newShipper.ShipperId;
            return Ok(shipper);
        }

        // DELETE: api/Shippers/5
        [HttpDelete("/DeleteShipper/{id:int}",Name = "DeleteShipper")]
        public async Task<IActionResult> DeleteShipper(int id)
        {
            if (context.Shippers == null)
            {
                return NotFound();
            }
            var shipper = await context.Shippers.FindAsync(id);
            if (shipper == null)
            {
                return NotFound();
            }

            context.Shippers.Remove(shipper);
            await context.SaveChangesAsync();

            return Ok(shipper);
        }

        private bool ShipperExists(int id)
        {
            return (context.Shippers?.Any(e => e.ShipperId == id)).GetValueOrDefault();
        }
    }
}
