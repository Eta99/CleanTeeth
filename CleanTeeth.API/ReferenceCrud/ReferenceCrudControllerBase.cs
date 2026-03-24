using CleanTeeth.Application.Features.UniversalCrud.Commands.Create;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Delete;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Update;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetById;
using CleanTeeth.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.ReferenceCrud
{
    /// <summary>
    /// Базовый контроллер CRUD для справочников: одна логика (получить / создать / обновить / удалить), разные DTO и маппинг.
    /// Наследник задаёт маршрут и переопределяет Get/Put/Delete с нужным типом id (long или Guid).
    /// </summary>
    public abstract class ReferenceCrudControllerBase<TEntity, TListDto, TDetailDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : class
    {
        private readonly IMediator _mediator;
        private readonly IReferenceCrudMapper<TEntity, TListDto, TDetailDto, TCreateDto, TUpdateDto> _mapper;

        protected ReferenceCrudControllerBase(
            IMediator mediator,
            IReferenceCrudMapper<TEntity, TListDto, TDetailDto, TCreateDto, TUpdateDto> mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TListDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _mediator.Send(new GetAllQuery<TEntity>());
            var dtos = entities.Select(_mapper.ToListDto).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Вызывается из наследника, например: [HttpGet("{id:long}")] public Task&lt;ActionResult&lt;TDetailDto&gt;&gt; Get(long id) => GetByIdCore(id);
        /// </summary>
        protected async Task<ActionResult<TDetailDto>> GetByIdCore(object id, CancellationToken cancellationToken = default)
        {
            var entity = await _mediator.Send(new GetByIdQuery<TEntity> { Id = id });
            if (entity == null)
                return NotFound();
            return _mapper.ToDetailDto(entity);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TDetailDto>> Create([FromBody] TCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.ToEntity(dto);
            var created = await _mediator.Send(new CreateCommand<TEntity>
            {
                Entity = entity,
                RequiredActionName = $"Create:{typeof(TEntity).Name}"
            });
            var detailDto = _mapper.ToDetailDto(created);
            var resourceId = _mapper.GetId(created);
            return CreatedAtAction("Get", new { id = resourceId }, detailDto);
        }

        /// <summary>
        /// Вызывается из наследника, например: [HttpPut("{id:long}")] public Task&lt;IActionResult&gt; Put(long id, [FromBody] TUpdateDto dto) => UpdateCore(id, dto);
        /// </summary>
        protected async Task<IActionResult> UpdateCore(object id, TUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _mediator.Send(new GetByIdQuery<TEntity> { Id = id });
            if (entity == null)
                return NotFound();
            _mapper.ApplyUpdate(entity, dto);
            await _mediator.Send(new UpdateCommand<TEntity>
            {
                Id = id,
                Entity = entity,
                RequiredActionName = $"Update:{typeof(TEntity).Name}"
            });
            return NoContent();
        }

        /// <summary>
        /// Вызывается из наследника, например: [HttpDelete("{id:long}")] public Task&lt;IActionResult&gt; Delete(long id) => DeleteCore(id);
        /// </summary>
        protected async Task<IActionResult> DeleteCore(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.Send(new DeleteCommand<TEntity> { Id = id, RequiredActionName = $"Delete:{typeof(TEntity).Name}" });
                return NoContent();
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
