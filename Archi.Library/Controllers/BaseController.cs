using APILibrary.Core.Attributes;
using APILibrary.Core.Extensions;
using Archi.Library.Filter;
using Archi.Library.Helpers;
using Archi.Library.Models;
using Archi.Library.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
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


        // GET: api/Products
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll(string range, string asc, string desc, string type, string rating, string date)
        {
            var contents = _context.Set<TModel>().AsQueryable();
            //var contents = from m in _context.Set<TModel>() select m;


            //triss
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
            if(totalRecords > 0) { 
                if (String.IsNullOrEmpty(range))
                {
                    range = 1 + "-" + totalRecords;
                }
                var route = Request.Path.Value;
                //TODO function for spliting range
                var tab = range.Split('-');
                var start = int.Parse(tab[0]);
                var end = int.Parse(tab[1]);
                var validRange = new RangeFilter(start, end, totalRecords);
                var pageSize = (1 + validRange.End - validRange.Start);
                var page = 1 + (validRange.Start / pageSize);
                var validFilter = new PaginationFilter(page, pageSize);

                var pagedData = await contents
                        .Skip((validFilter.Page - 1) * validFilter.PageSize)
                        .Take(validFilter.PageSize)
                        .ToListAsync();
                var pagedResponse = PaginationHelper.CreatePagedResponse<TModel>(pagedData, range, validFilter, totalRecords, _uriService, route, asc, desc, type, rating, date);

                return Ok(pagedResponse);
            }
            else
            {
                return new List<TModel>();
            }
        }


    // GET: api/Products/Search
    [Route("search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<dynamic>>> SearchAsync([FromQuery] string name, string type, string rating, string date)
        {
            var contents = _context.Set<TModel>().AsQueryable();

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(rating) || !string.IsNullOrEmpty(date))
            {
                contents = contents.SearchThis(name, type, rating, date);
            }
            return Ok(ToJsonList(await contents.ToListAsync()));
        }


        // GET: api/Products/5
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


        // PUT: api/Products/5
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


        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TModel>> PostContent(TModel content)
        {
            _context.Set<TModel>().Add(content);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContent", new { id = content.ID }, content);
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            var content = await _context.Set<TModel>().FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            _context.Set<TModel>().Remove(content);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContentExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.ID == id);
        }
      

        protected IEnumerable<dynamic> ToJsonList(IEnumerable<dynamic> tab)
        {
            var tabNew = tab.Select((x) =>
            {
                return ToJson(x);
            });
            return tabNew;
        }

        protected dynamic ToJson(dynamic item)
        {
            var expo = new ExpandoObject() as IDictionary<string, object>;
            var collectionType = typeof(TModel);
            IDictionary<string, object> dico = item as IDictionary<string, object>;
            if (dico != null)
            {
                foreach (var propDyn in dico)
                {
                    var propInTModel = collectionType.GetProperty(propDyn.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    var isPresentAttribute = propInTModel.CustomAttributes.Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute) expo.Add(propDyn.Key, propDyn.Value);
                }
            }
            else
            {
                foreach (var prop in collectionType.GetProperties())
                {
                    var isPresentAttribute = prop.CustomAttributes.Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute) expo.Add(prop.Name, prop.GetValue(item));
                }
            }
            return expo;
        }


    }
}