using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.Model.Models;
using Northwind.WebApi.ModelsExt.Errors;

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : CustomControllerBase
    {
        public ProductsController(NorthwindContext context, IMapper _map):base(context,_map)
        {
        }

        // GET: api/Products
        [HttpGet("GetSelectProductPaging")]

        public async Task<ActionResult<IEnumerable<ProductReturn>>> SelectProductPaging([FromQuery] ProductReturn product, [FromQuery] AllParamToPagingSel gridParam)
        {
            try
            {
                var products = await context.Procedures.SelectProductPagingAsync(product.ProductName, product.SupplierId, product.CategoryId,
              product.QuantityPerUnit, product.UnitPrice, product.UnitsInStock, product.UnitsOnOrder, product.ReorderLevel, product.Discontinued,product.ExcludeProductIdStr,
              gridParam.FilterType, gridParam.SortCollName, gridParam.AscDesc, gridParam.PageNumber, gridParam.PageSize);
               
                var res = mapper.Map<List<ProductReturn>>(products);
                return new JsonResult(res);
            }
            catch (ApiException ex)
            {
                throw new ApiException(ex);
            }
        }



        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("EditProduct")]
        public async Task<IActionResult> PutProduct([FromBody] ProductReturn product)
        {

            var prod = mapper.Map<Product>(product);
            context.Entry(prod).State=EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApiException(ex);
            }

            return Ok(product);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("AddProduct")]
        public async Task<ActionResult<ProductReturn>> AddProduct([FromBody] ProductReturn product)
        {
            try
            {
                var prod = mapper.Map<Product>(product);
                context.Products.Add(prod);
                await context.SaveChangesAsync();
                product.Id = prod.ProductId;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
            return Ok(product);
        }

        // DELETE: api/Products/5
        [HttpDelete("DeleteProduct/{id:int}",Name ="DeleteProduct")]
        public async Task<ActionResult<ProductReturn>> DeleteProduct(int id)
        {
            try
            {
                if (context.Products == null)
                    return NotFound();
                var product = await context.Products.FindAsync(id);
                if (product == null)
                    return NotFound();

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                var ret=mapper.Map<ProductReturn>(product);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
