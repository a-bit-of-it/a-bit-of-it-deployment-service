using Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class CustomersController(CustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await customerService.GetAll();
        
        return Ok(customers);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await customerService.Get(id);
     
        return Ok(customer);
    }
}