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
    [ApiExceptionFilter]
    [Authorize]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class CategoriesController : CustomControllerBase
    {

        public const int sizeFile = 5715200;
        public CategoriesController(NorthwindContext context, IMapper map) : base(context, map)
        {
        }

        [HttpGet("GetCategorysPaging")]
        [RequestFormLimits(MultipartBodyLengthLimit = sizeFile)]//max size file 5.71 MB
        [RequestSizeLimit(sizeFile)]
        public async Task<ActionResult<IEnumerable<CategoryView>>> GetCategorysOnPage(string? categoryName, string? categoryDesccription, string? sortCollName, string? ascDesc, int pageNumber, int pageSize)
        {
            try
            {
                var res = await context.Procedures.SelectCategoryPagingAsync(categoryName, categoryDesccription, sortCollName, ascDesc, pageNumber, pageSize);
                var resret = mapper.Map<List<CategoryView>>(res);
                return resret;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut(Name = "ModifiCategory")]
        [RequestFormLimits(MultipartBodyLengthLimit = sizeFile)]
        [RequestSizeLimit(sizeFile)]
        public async Task<IActionResult> PutCategory(CategoryView category)
        {
            try
            {
                var catInsert = mapper.Map<Category>(category);
                context.Entry(catInsert).State = EntityState.Modified;

                await context.SaveChangesAsync();
                return Ok(category);

            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }

        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(Name = "AddCategory")]
        [RequestFormLimits(MultipartBodyLengthLimit = sizeFile)]
        [RequestSizeLimit(sizeFile)]
        public async Task<ActionResult<CategoryView>> AddCategory(CategoryView category)
        {
            var catIns = mapper.Map<Category>(category);
            context.Categories.Add(catIns);
            await context.SaveChangesAsync();
            category.Id = catIns.CategoryId;
            return Ok(category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("DeleteCategory")]
        public async Task<ActionResult<int>> DeleteCategory(int id, bool comfermRem)
        {
            try
            {
                var category = await context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                OutputParameter<int> outputParameter = new OutputParameter<int>();
                var result = await context.Procedures.RemoveCategoryByIdAsync(id, comfermRem, outputParameter);
                return new ActionResult<int>(result >= 1 ? result : outputParameter.Value);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}
