using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Data;
using Northwind.WebApi.Model.Models;
using Northwind.WebApi.ModelsExt.Errors;
using Northwind.WebApi.ModelsExt.ModelAuthentic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierController : CustomControllerBase
    {
        public SupplierController(NorthwindContext _context, IMapper _mapper):base(_context,_mapper)
        {
        }
        // GET: api/<SupplierController>
        [HttpGet("GetSuppliersPaging")]
        public async Task<ActionResult<List<SupplierReturn>>> GetSuppliersPaging([FromQuery] SupplierReturn supplier, string? sortCollName, string? ascDesc, int pageNumber, int pageSize)
        {
            try
            {
                var result = await context.Procedures.SelectSuppliersPagingAsync(supplier.CompanyName, supplier.ContactName,
                   supplier.ContactTitle, supplier.Address, supplier.City, supplier.Region, supplier.PostalCode,
                    supplier.Country, supplier.Phone, supplier.HomePage, sortCollName, ascDesc, pageNumber, pageSize);

                var resultReturn = mapper.Map<List<SelectSuppliersPagingResult>, List<SupplierReturn>>(result);
                return resultReturn;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }

        }


        // POST api/<SupplierController>
        [HttpPost(Name = "AddSupplier")]
        public async Task<ActionResult<SupplierReturn>> AddSupplier(SupplierReturn supplier)
        {
            try
            {
                var supAdd = mapper.Map<SupplierReturn, Supplier>(supplier);
                context.Suppliers.Add(supAdd);
                await context.SaveChangesAsync();
                supplier.Id = supAdd.SupplierId;
                return mapper.Map<Supplier, SupplierReturn>(supAdd);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // PUT api/<SupplierController>/5
        [HttpPut(Name = "ModifSupplier")]
        public async Task<IActionResult> ModifiSupplier([FromBody] SupplierReturn supplier)
        {
            try
            {
                var supUpd = mapper.Map<Supplier>(supplier);
                context.Entry(supUpd).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
            return Ok(supplier);
        }

        // DELETE api/<SupplierController>/5
        [HttpDelete("DeleteSupplier", Name = "DeleteSupplier")]
        public async Task<ActionResult<int>> DeleteSupplier(int id, bool comfermRem)
        {
            try
            {
                var supDel = await context.Suppliers.FindAsync(id);
                if (supDel == null)
                {
                    return NotFound();
                }
                OutputParameter<int> outputParameter = new OutputParameter<int>();
                var res =await context.Procedures.RemoveSupplierByIdAsync(id, comfermRem,outputParameter);
                return new ActionResult<int>(res.First().res);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
