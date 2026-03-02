using CleanTeeth.API.DTOs.Roles;
using CleanTeeth.Application.Features.Roles.Commands.CreateRole;
using CleanTeeth.Application.Features.Roles.Commands.DeleteRole;
using CleanTeeth.Application.Features.Roles.Commands.UpdateRole;
using CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail;
using CleanTeeth.Application.Features.Roles.Queries.GetRolesList;
using CleanTeeth.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleListDTO>>> Get()
        {
            var result = await _mediator.Send(new GetRolesListQuery());
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDetailDTO>> Get(long id)
        {
            var result = await _mediator.Send(new GetRoleDetailQuery { Id = id });
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateRoleDto dto)
        {
            await _mediator.Send(new CreateRoleCommand { Title = dto.Title });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateRoleDto dto)
        {
            await _mediator.Send(new UpdateRoleCommand { Id = id, Title = dto.Title });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _mediator.Send(new DeleteRoleCommand { Id = id });
            return NoContent();
        }
    }
}
