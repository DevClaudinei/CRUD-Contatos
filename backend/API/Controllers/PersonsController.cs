using System;
using System.Threading.Tasks;
using DomainModels.Models;
using DomainServices.Exceptions;
using DomainServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var persons = _personService.GetAll();

            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Persons>> GetById(long id)
        {
            var personFound = await _personService.GetById(id);

            if (personFound is null)
                return NotFound();

            return personFound;
        }

        [HttpPost]
        public async Task<ActionResult<Persons>> Create(Persons person)
        {
            var personId = await _personService.Create(person);

            return Created("Id: ", personId);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, Persons person)
        {
            try
            {
                _personService.Update(id, person);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                _personService.Delete(id);
                return NoContent();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}