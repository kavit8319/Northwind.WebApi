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
    public class OrdersDetailController : CustomControllerBase
    {
        public OrdersDetailController(NorthwindContext cont, IMapper map) : base(cont, map)
        {
        }
        // GET: api/<OrdersDetailController>
        [HttpGet("/SelectOrdersDetail", Name = "SelectOrdersDetail")]
        public async Task<ActionResult<IEnumerable<OrderDetailReturn>>> SelectOrdersDetail([FromQuery] OrderDetailReturn order, string collName, string ascDesc)
        {
            try
            {
                var result = await context.Procedures.SelectOrdersDetailAsync(order.Id, collName, ascDesc);
                return mapper.Map<List<OrderDetailReturn>>(result); ;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // POST api/<OrdersController>
        [HttpPost(Name = "AddOrderDetail")]
        public async Task<ActionResult<OrderDetailReturn>> AddOrderDetail([FromBody] OrderDetailReturn order)
        {
            try
            {
                var ordIns = mapper.Map<OrderDetail>(order);
                context.OrderDetails.Add(ordIns);
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
        [HttpPut("EditOrderDetail")]
        public async Task<ActionResult<OrderDetailReturn>> EditOrderDetail([FromBody] OrderDetailReturn order)
        {
            var ord = mapper.Map<OrderDetail>(order);
            context.Entry(ord).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                var orderPaging = context.SelectOrderPagindViews.FirstOrDefault(el => el.Id == order.Id);
                if (orderPaging != null)
                    order.TotalSumm = orderPaging.TotalSumm;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApiException(ex);
            }
            return Ok(order);
        }

        [HttpDelete("DeleteOrderDetail/{id:int}/{productId:int}", Name = "DeleteOrderDetail")]
        public async Task<IActionResult> DeleteOrderDetail(int id, int productId)
        {
            try
            {
                if (context.Orders == null)
                    return NotFound();
                var ord = await context.OrderDetails.FirstOrDefaultAsync(el => el.OrderId == id && el.ProductId == productId);
                if (ord == null)
                    return NotFound();

                context.OrderDetails.Remove(ord);
                await context.SaveChangesAsync();
                var ret = mapper.Map<OrderDetailReturn>(ord);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
