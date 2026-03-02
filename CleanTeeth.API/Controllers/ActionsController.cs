using CleanTeeth.API.DTOs.Actions;
using CleanTeeth.Application.Features.AppActions.Commands.CreateAction;
using CleanTeeth.Application.Features.AppActions.Commands.DeleteAction;
using CleanTeeth.Application.Features.AppActions.Commands.UpdateAction;
using CleanTeeth.Application.Features.AppActions.Queries.GetActionDetail;
using CleanTeeth.Application.Features.AppActions.Queries.GetActionsList;
using CleanTeeth.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers
{
    [ApiController]
    [Route("api/actions")]
    public class ActionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActionListDTO>>> Get()
        {
            var result = await _mediator.Send(new GetActionsListQuery());
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActionDetailDTO>> Get(long id)
        {
            var result = await _mediator.Send(new GetActionDetailQuery { Id = id });
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateActionDto dto)
        {
            await _mediator.Send(new CreateActionCommand { TypeId = dto.TypeId, Name = dto.Name, Title = dto.Title });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateActionDto dto)
        {
            await _mediator.Send(new UpdateActionCommand { Id = id, TypeId = dto.TypeId, Name = dto.Name, Title = dto.Title });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _mediator.Send(new DeleteActionCommand { Id = id });
            return NoContent();
        }
    }
}
