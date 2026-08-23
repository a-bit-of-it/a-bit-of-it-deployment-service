using Api.Application;
using Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class CustomersController(ICustomerRepository customerRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await customerRepository.GetCustomers();
        
        return Ok(customers);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await customerRepository.GetCustomer(id);
        
        if (customer == null)
            throw new NotFoundException($"Customer not found. Id = {id}"); // TODO temp
     
        return Ok(customer);
    }
}