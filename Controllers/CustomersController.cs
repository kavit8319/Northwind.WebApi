using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.Model.Models;
using Northwind.WebApi.ModelsExt.Errors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : CustomControllerBase
    {
        public CustomersController(NorthwindContext context, IMapper mapper):base(context,mapper)
        {
        }
        // GET: api/<CustomersController>
        [HttpGet("SelectCustomersPaging")]
        public async Task<ActionResult<IEnumerable<CustomerReturn>>> SelectCustomersPaging([FromQuery]CustomerReturn customer, [FromQuery]AllParamToPagingSel paging)
        {
            try
            {
                var result = await context.Procedures.SelectCustomerPagingAsync(customer.CompanyName,
                    customer.ContactName, customer.CustomerTitleId, customer.Address, customer.City,
                    customer.PostalCode, customer.Country, customer.Phone,
                    customer.IsVip, paging.SortCollName, paging.AscDesc, paging.PageNumber, paging.PageSize);
                var res = mapper.Map<List<CustomerReturn>>(result);
                return new JsonResult(res);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
        // POST api/<CustomersController>
        [HttpPost("AddCustomer")]
        public async Task<ActionResult<CustomerReturn>> AddCustomer([FromBody]CustomerReturn customer)
        {
            try
            {
                var custIns = mapper.Map<Customer>(customer);
                context.Customers.Add((Customer?)custIns);
                await context.SaveChangesAsync();
                customer.Id = custIns.CustomerId;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
            return Ok(customer);
        }

        // PUT api/<CustomersController>/5
        [HttpPut("EditCustomer")]
        public async Task<IActionResult> EditCustomer([FromBody]CustomerReturn customer)
        {

            var cust = mapper.Map<Customer>(customer);
            context.Entry(cust).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApiException(ex);
            }
            return Ok(customer);
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("DeleteCustomers/{id:int}",Name = "DeleteCustomers")]
        public async Task<IActionResult> DeleteCustomers(int id)
        {
            try
            {
                if (context.Customers == null)
                    return NotFound();
                var cust = await context.Customers.FindAsync(id);
                if (cust == null)
                    return NotFound();

                context.Customers.Remove(cust);
                await context.SaveChangesAsync();
                var ret = mapper.Map<CustomerReturn>(cust);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
