using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Archi.API.Model;
using Archi.API.Data;
using Archi.Library.Controllers;

namespace ArchiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController<ArchiDbContext, IUriService, Product>
    {
        public ProductsController(ArchiDbContext c, IUriService u) : base(c, u)
        {
        }
    }
}