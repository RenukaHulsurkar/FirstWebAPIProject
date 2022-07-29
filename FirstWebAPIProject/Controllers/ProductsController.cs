using FirstWebAPIProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ProductsController(ShopContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _context.Database.EnsureCreated();
            _logger = logger;
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
    }
}
