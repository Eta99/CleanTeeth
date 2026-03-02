using CleanTeeth.API.DTOs.Users;
using CleanTeeth.Application.Features.Users.Commands.CreateUser;
using CleanTeeth.Application.Features.Users.Commands.DeleteUser;
using CleanTeeth.Application.Features.Users.Commands.UpdateUser;
using CleanTeeth.Application.Features.Users.Queries.GetUserDetail;
using CleanTeeth.Application.Features.Users.Queries.GetUsersList;
using CleanTeeth.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserListDTO>>> Get()
        {
            var result = await _mediator.Send(new GetUsersListQuery());
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailDTO>> Get(long id)
        {
            var result = await _mediator.Send(new GetUserDetailQuery { Id = id });
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDto dto)
        {
            await _mediator.Send(new CreateUserCommand { Login = dto.Login });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateUserDto dto)
        {
            await _mediator.Send(new UpdateUserCommand { Id = id, Login = dto.Login });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return NoContent();
        }
    }
}
