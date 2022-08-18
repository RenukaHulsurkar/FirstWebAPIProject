using FirstWebAPIProject.Interfaces;
using FirstWebAPIProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstWebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ILogger<ProductsController> _logger;

        private ICacheProvider _cacheProvider;
        private  IMemoryCache _cache;

        public ProductsController(ShopContext context, ILogger<ProductsController> logger, ICacheProvider cacheProvider,IMemoryCache memoryCache)
        {
            _context = context;
            _context.Database.EnsureCreated();
            _logger = logger;
            _cacheProvider = cacheProvider;
            _cache = memoryCache;
        }

        [HttpGet("{id}")] //[HttpGet, Route("/products/{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            //var products = _context.Products.ToList();
            return Ok(await _context.Products.ToArrayAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Products>> PostProduct(Products products)
        {
            //if (!modelstate.isvalid)
            //{
            //    return badrequest();
            //}

            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                  "GetProduct", new { id = products.Id }, products
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Products products)
        {
            if (id != products.Id)
            {
                _logger.LogInformation("FROM PRODUCT ID DOES NOT MATCH");
                return BadRequest();
            }

            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    _logger.LogInformation("FROM CATCH BLOCK");
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            var p1 = await _context.Products.MaxAsync(a => a.Price);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<Products>> DeleteMultipleProduct([FromQuery] int[] ids)
        {
            var products = new List<Products>();
            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("getAllproducts")]
        public IActionResult GetAllproducts()
        {
            //if (! _cache.TryGetValue(CacheKeys.Products, out List<Products> employees))
            //{
            //    employees = _context.Products.ToList();  //GetEmployeesDeatilsFromDB(); // Get the data from database

            //    var cacheEntryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            //        SlidingExpiration = TimeSpan.FromMinutes(2),
            //        Size = 1024,
            //    };  
            //    _cache.Set(CacheKeys.Products, employees, cacheEntryOptions);
            //}
            //return Ok(employees);

            try
            {
                var employees = _cacheProvider.GetCachedResponse().Result;
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return new ContentResult()
                {
                    StatusCode = 500,
                    Content = "{ \n error : " + ex.Message + "}",
                    ContentType = "application/json"
                };
            }
        }
    }
}
