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
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryQuotationController : ControllerBase
    {
        IConfiguration _configuration;
        InquiryRepository _inquiryRepository;
        public InquiryQuotationController(IConfiguration configuration)
        {
            _configuration = configuration;
           // _categoryRepository = new CategoryRepository(_configuration.GetConnectionString("DbConnection"));
            _inquiryRepository = new InquiryRepository(_configuration.GetConnectionString("DbConnection1"));
        }

        [HttpPost]
        public IActionResult Create(List<Inquiry> model)
        {
            // It is an optional field for creation, so remove it
            if (ModelState.IsValid)
            {
                var result = _inquiryRepository.AddInquiryItem(model);
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
        [HttpGet]
        public async Task<IActionResult> Costing(string WorkOrder)
        {
            // It is an optional field for creation, so remove it
            if (ModelState.IsValid)
            {
                CostingResult result = await _inquiryRepository.GetCosting(WorkOrder);
                if (result!=null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }
                // Assuming you want to return a 200 OK response
            }
            return BadRequest(ModelState); // Return validation errors if ModelState is not valid
        }




        [HttpPatch]
        public async Task<IActionResult> UpdateInquiryItem(InquiryItemUpdate model)
        {
            var result = await _inquiryRepository.UpdateInquiryItem(model);
            if (result == true)
            {
                return Ok("Inquiry Updated Successfully...");
            }
            else
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
