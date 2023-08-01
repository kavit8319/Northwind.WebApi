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
    public class EmployeesController : CustomControllerBase
    {
        public const int sizeFile = 5715200;
        public EmployeesController(NorthwindContext context, IMapper _map):base(context,_map)
        {
        }
        // GET: api/<EmployeesController>
        [HttpGet("GetEmployeesPaging")]
        [RequestFormLimits(MultipartBodyLengthLimit = sizeFile)]//max size file 5.71 MB
        [RequestSizeLimit(sizeFile)]
        public async Task<ActionResult<IEnumerable<EmployeeReturn>>> SelectEmployeesPaging([FromQuery] EmployeeReturn employee, [FromQuery] AllParamToPagingSel gridParam)
        {
            try
            {
                var employees = await context.Procedures.SelectEmployeesPagingAsync(employee.FirstName, employee.LastName, employee.TitleOfCourtesy,employee.TitleId, employee.BirthDate, employee.HireDate,
                    employee.Address, employee.City, employee.Country,employee.ReportsTo, employee.PostalCode, employee.Notes, gridParam.FilterType, gridParam.SortCollName, gridParam.AscDesc, gridParam.PageNumber, gridParam.PageSize);
                var res = mapper.Map<List<EmployeeReturn>>(employees);
                return new JsonResult(res);
            }
            catch (ApiException ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("SelectEmployeesAll")]
        public async Task<ActionResult<IEnumerable<EmployeeAllReturn>>> SelectEmployeesAll()
        {
            try
            {
                var empl = await context.Procedures.SelectEmployeesAllAsync();
                return new JsonResult(mapper.Map<List<EmployeeAllReturn>>(empl));
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("GetTitles")]
        public async Task<ActionResult<IEnumerable<TitleReturn>>> GetTitles(string typeTitle)
        {
            try
            {
                var title = await context.Procedures.GetTitlesAsync(typeTitle);
                var result = mapper.Map<List<TitleReturn>>(title);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("EmployeerExist")]
        public async Task<bool> EmployeerExist(string firstName, string lastName)
        {
            try
            {
                var res = await context.Procedures.GetEmployeeExistAsync(firstName, lastName);
                var exist = res[0].Exist;
                return exist != null && exist.Value;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("GetReportTo")]
        public async Task<ActionResult<IEnumerable<ReportToResult>>> GetReportTo(int? EmployeeId)
        {
            try
            {
                var result = await context.Procedures.GetEmployeesForReportsToAsync(EmployeeId);
                return mapper.Map<List<ReportToResult>>(result);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // POST api/<EmployeesController>
        [HttpPost("AddEmployee")]
        public async Task<ActionResult<EmployeeReturn>> AddEmployee([FromBody] EmployeeReturn employee)
        {
            try
            {
                var empl = mapper.Map<Employee>(employee);
                context.Employees.Add(empl);
                
                await context.SaveChangesAsync();
                employee.Id = empl.EmployeeId;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
            return Ok(employee);
        }

        // PUT api/<EmployeesController>/5
        [HttpPut("EditEmployee")]
        public async Task<IActionResult> PutEmployee([FromBody] EmployeeReturn product)
        {

            var prod = mapper.Map<Employee>(product);
            context.Entry(prod).State = EntityState.Modified;
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

        // DELETE api/<EmployeesController>/5
        [HttpDelete("DeleteEmployee/{id:int}", Name = "DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                if (context.Employees == null)
                    return NotFound();
                var employee = await context.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound();

                await  context.Procedures.RemoveEmployeesByIdAsync(employee.EmployeeId);
                var ret = mapper.Map<EmployeeReturn>(employee);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
