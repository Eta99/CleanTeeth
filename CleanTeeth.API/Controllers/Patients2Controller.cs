using CleanTeeth.Application.Features.UniversalCrud.Commands.Create;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Delete;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Update;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetById;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers
{
    /// <summary>
    /// CRUD пациентов через универсальные операции (GetByIdQuery&lt;Patient&gt;, GetAllQuery&lt;Patient&gt;, Create/Update/DeleteCommand&lt;Patient&gt;).
    /// </summary>
    [ApiController]
    [Route("api/patients2")]
    public class Patients2Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public Patients2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Patient>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllQuery<Patient>());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetByIdQuery<Patient> { Id = id });
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Patient>> Create([FromBody] Patient entity)
        {            
            var result = await _mediator.Send(new CreateCommand<Patient>
            {
                Entity = entity,
                RequiredActionName = "Create:Patient"
            });
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Patient entity)
        {
            try
            {
                await _mediator.Send(new UpdateCommand<Patient>
                {
                    Id = id,
                    Entity = entity,
                    RequiredActionName = "Update:Patient"
                });
                return NoContent();
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCommand<Patient> { Id = id, RequiredActionName = "Delete:Patient" });
                return NoContent();
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
