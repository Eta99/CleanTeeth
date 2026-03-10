using CleanTeeth.API.DTOs.Roles;
using CleanTeeth.API.ReferenceCrud;
using CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail;
using CleanTeeth.Application.Features.Roles.Queries.GetRolesList;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers.Ref
{
    /// <summary>
    /// CRUD ролей через базовый справочный контроллер (ReferenceCrud): общая логика + DTO и маппер.
    /// </summary>
    [ApiController]
    [Route("api/ref/roles")]
    public class RolesReferenceController : ReferenceCrudControllerBase<Role, RoleListDTO, RoleDetailDTO, CreateRoleDto, UpdateRoleDto>
    {
        public RolesReferenceController(
            IMediator mediator,
            IReferenceCrudMapper<Role, RoleListDTO, RoleDetailDTO, CreateRoleDto, UpdateRoleDto> mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(RoleDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<RoleDetailDTO>> Get(long id, CancellationToken cancellationToken = default)
            => GetByIdCore(id, cancellationToken);

        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Put(long id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken = default)
            => UpdateCore(id, dto, cancellationToken);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
            => DeleteCore(id, cancellationToken);
    }
}
