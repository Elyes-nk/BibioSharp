using APILibrary.Core.Extensions;
using Archi.Library.Filter;
using Archi.Library.Helpers;
using Archi.Library.Models;
using Archi.Library.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Archi.Library.Controllers
{
    public abstract class BaseController<TContext, TUriService, TModel> : ControllerBase where TContext : DbContext where TUriService : IUriService where TModel : ModelBase
    {
        protected readonly TContext _context;
        protected readonly TUriService _uriService;


        public BaseController(TContext context, TUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }


        // GET: api/{model}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll(string search, string asc, string desc, string type, string rating, String date, string range)
        {
            //search
            var contents = from m in _context.Set<TModel>() select m;
            if (!string.IsNullOrEmpty(search))
            {
                contents = contents.Where(s => s.Name.Contains(search));
            }

            //tris
            if (!string.IsNullOrEmpty(asc) || !string.IsNullOrEmpty(desc))
            {
                contents = contents.OrderThis(asc, desc);
            }

            //filter 
            if (!string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(rating) || !string.IsNullOrEmpty(date))
            {
                contents = contents.FilterThis(type, rating, date);
            }



            //pagination
            var totalRecords = await contents.CountAsync();
            if (String.IsNullOrEmpty(range))
            {
                range = 1 + "-" + totalRecords;
            }
            var route = Request.Path.Value;
            //split range into page and pagesize
            //TODO function for spliting range
            var tab = range.Split('-');
            var start = int.Parse(tab[0]);
            var end = int.Parse(tab[1]);
            if (start == 0)
            {
                start++;
                end++;
            }
            if (end > totalRecords)
            {
                end = totalRecords;
            }
            var pageSize = (1 + end - start);
            var page = 1 + (start / pageSize);
            var validFilter = new PaginationFilter(page, pageSize);
      
            var pagedData = await contents
                    .Skip((validFilter.Page - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize)
                    .ToListAsync();
            var pagedResponse = PaginationHelper.CreatePagedResponse<TModel>(pagedData, range, validFilter, totalRecords, _uriService, route);
            return Ok(pagedResponse);
        }










        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TModel>> GetContent(int id)
        {
            var content = await _context.Set<TModel>().FindAsync(id);

            if (content == null)
            {
                return NotFound();
            }

            //return content;
            return Ok(new Response<TModel>(content));

        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContent(int id, TModel content)
        {
            if (id != content.ID)
            {
                return BadRequest();
            }

            _context.Entry(content).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContentExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TModel>> PostContent(TModel content)
        {
            _context.Set<TModel>().Add(content);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContent", new { id = content.ID }, content);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Set<TModel>().FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            //customer.Active = false;
            //_context.Entry(customer).State = EntityState.Modified;

            //_context.Entry(customer).State = EntityState.Deleted;
            _context.Set<TModel>().Remove(content);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContentExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.ID == id);
        }
    }
}