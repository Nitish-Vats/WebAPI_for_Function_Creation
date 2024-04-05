using DAL.Entities;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IConfiguration _configuration;
        ProductRepository _productRepository;
        CategoryRepository _categoryRepository;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            _productRepository = new ProductRepository(_configuration.GetConnectionString("DbConnection"));
            _categoryRepository = new CategoryRepository(_configuration.GetConnectionString("DbConnection"));
        }
        [HttpPost] 
        public async Task<List<Product>> GetAllProducts() 
        {
            var products = await _productRepository.GetProducts();
            return products; 
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            ModelState.Remove("ProductId"); // It is an optional field for creation, so remove it
            if (ModelState.IsValid)
            {
                var result=_productRepository.AddProduct(model);
                if (result == true)
                {
                    return Ok("Product Added Successfully...");
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }
               // Assuming you want to return a 200 OK response
            }
            return BadRequest(ModelState); // Return validation errors if ModelState is not valid
        }


        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Product model)
        {
            if (id != model.ProductId)
            {
                return BadRequest("Invalid product ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = _productRepository.GetProductById(id);

            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            _productRepository.UpdateProduct(model);

            return Ok("Product updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingProduct = _productRepository.GetProductById(id);

            if (existingProduct.ProductId == 0)
            {
                return NotFound("Product not found");
            }

            _productRepository.DeleteProduct(id);

            return Ok("Product deleted successfully");
        }



    }
}
