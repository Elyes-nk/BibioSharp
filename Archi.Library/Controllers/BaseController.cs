using Archi.Library.Models;
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
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : DbContext where TModel : ModelBase
    {
        protected readonly TContext _context;

        public BaseController(TContext context)
        {
            _context = context;
        }



        // GET: api/{model}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll(String search, String range)
        {
            var contents = from m in _context.Set<TModel>()
                         select m;

            if (!String.IsNullOrEmpty(search))
            {
                contents = contents.Where(s => s.Name.Contains(search));
            }
            if (!String.IsNullOrEmpty(range))
            {

            }
            return await contents.ToListAsync();

            // var query = _context.Customers.AsQueryable<Customer>();
            //query = query.Take(18);


            //// LAMBDA: x => x.[PropertyName]
            //var parameter = Expression.Parameter(typeof(TModel), "x");
            //Expression property = Expression.Property(parameter, "Name");
            //var lambda = Expression.Lambda(property, parameter);

            //// REFLECTION: source.OrderBy(x => x.Property)
            //var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            //var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
            //var result = orderByGeneric.Invoke(null, new object[] { contents, lambda });

            //return await ((IOrderedQueryable<Customer>)result).ToListAsync();
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

            return content;
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