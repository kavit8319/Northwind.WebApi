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
    public class OrdersController : CustomControllerBase
    {
        // GET: api/<OrdersController>
        public OrdersController(NorthwindContext _context, IMapper map):base(_context,map)
        {
        }
        [HttpGet("/SelectAllOrders", Name = "SelectAllOrders")]
        public async Task<ActionResult<IEnumerable<OrderReturn>>> SelectAllOrders([FromQuery] OrderReturn order,[FromQuery] AllParamToPagingSel paramToPaging)
        {
            try
            {
                var result = await context.Procedures.SelectOrdersPagingAsync(order.CompanyName, order.ContactName, order.EmplFullName,
                     order.Freight, order.ShipName, order.ShipAddress, order.Country, order.City,
                     order.PostalCode,order.TotalSumm, order.OrderDate, order.RequiredDate, order.ShippedDate,
                     paramToPaging.FilterType, paramToPaging.SortCollName, paramToPaging.AscDesc,
                     paramToPaging.PageNumber, paramToPaging.PageSize);
                return mapper.Map<List<OrderReturn>>(result); ;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // POST api/<OrdersController>
        [HttpPost(Name = "AddOrder")]
        public async Task<ActionResult<OrderReturn>> AddOrder([FromBody] OrderReturn order)
        {
            try
            {
                var ordIns = mapper.Map<Order>(order);
                context.Orders.Add(ordIns);
                await context.SaveChangesAsync();
                order.Id = ordIns.OrderId;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
            return Ok(order);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("EditOrder")]
        public async Task<ActionResult<OrderReturn>> EditOrder([FromBody] OrderReturn order)
        {
            var ord = mapper.Map<Order>(order);
            context.Entry(ord).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApiException(ex);
            }
            return Ok(order);
        }

        [HttpDelete("DeleteOrder/{id:int}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                if (context.Orders == null)
                    return NotFound();
                var ord = await context.Orders.FindAsync(id);
                if (ord == null)
                    return NotFound();

                context.Orders.Remove(ord);
                await context.SaveChangesAsync();
                var ret = mapper.Map<OrderReturn>(ord);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
