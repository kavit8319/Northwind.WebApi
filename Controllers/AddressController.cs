using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.ModelsExt.Errors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : CustomControllerBase
    {
        // GET: api/<AddressController>
        public AddressController(NorthwindContext _context):base(_context,null)
        {
        }
        [HttpGet(template:"GetAllCountry")]
        public async Task<ActionResult<IEnumerable<Country>>> GetAllCountry()
        {
            try
            {
                var result = await context.Procedures.GetAllCountrysAsync();
                return result.Select(el => new Country() { Name = el.Name, Code = el.Code }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }

        }

        //// GET api/<AddressController>/5
        [HttpGet(template: "GetAllCity/{code}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllCity(string code)
        {
            try
            {
                var result = await context.Procedures.GetAllCityByCountryAsync(code);
                return result.Select(el => el.Name).ToList();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // POST api/<AddressController>
        [HttpGet(template: "GetAllPostCode/{code}")]
        public async Task<ActionResult<List<string>>> GetAllPostCode(string code)
        {
            try
            {
                var result = await context.Procedures.GetAllPostCodesByCountryCodeAsync(code);
                return result.Select(el => el.PostCode).ToList();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
