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
    /// CRUD ролей через универсальные операции (GetByIdQuery&lt;Role&gt;, GetAllQuery&lt;Role&gt;, Create/Update/DeleteCommand&lt;Role&gt;).
    /// </summary>
    [ApiController]
    [Route("api/roles2")]
    public class Roles2Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public Roles2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Role>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllQuery<Role>());
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Role>> GetById(long id)
        {
            var result = await _mediator.Send(new GetByIdQuery<Role> { Id = id });
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Role>> Create([FromBody] Role entity)
        {
            var result = await _mediator.Send(new CreateCommand<Role>
            {
                Entity = entity,
                RequiredActionName = "Create:Role"
            });
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] Role entity)
        {
            try
            {
                await _mediator.Send(new UpdateCommand<Role>
                {
                    Id = id,
                    Entity = entity,
                    RequiredActionName = "Update:Role"
                });
                return NoContent();
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _mediator.Send(new DeleteCommand<Role> { Id = id, RequiredActionName = "Delete:Role" });
                return NoContent();
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
