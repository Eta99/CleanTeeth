using CleanTeeth.Application.Features.UniversalCrud.Commands.Create;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Delete;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Update;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetById;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CleanTeeth.API.Controllers
{
    /// <summary>
    /// Универсальный CRUD: получение и изменение данных по имени сущности.
    /// Тип репозитория определяется по сегменту {entityName}.
    /// </summary>
    [ApiController]
    [Route("api/universal")]
    public class UniversalCrudController : ControllerBase
    {
        private static readonly Dictionary<string, Type> EntityTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["dentaloffices"] = typeof(DentalOffice),
            ["patients"] = typeof(Patient),
            ["dentists"] = typeof(Dentist),
            ["appointments"] = typeof(Appointment),
            ["users"] = typeof(User),
            ["roles"] = typeof(Role),
            ["actions"] = typeof(AppAction),
        };

        private readonly IMediator _mediator;

        public UniversalCrudController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Список зарегистрированных типов сущностей.</summary>
        [HttpGet("entities")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetEntityNames()
        {
            return Ok(EntityTypeMap.Keys.ToList());
        }

        /// <summary>Получить все сущности указанного типа.</summary>
        /// <param name="entityName">Имя сущности (dentaloffices, roles, patients, dentists, appointments, users, actions).</param>
        [HttpGet("{entityName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<object>>> GetAll(string entityName)
        {
            if (!EntityTypeMap.TryGetValue(entityName, out var entityType))
                return BadRequest($"Unknown entity: {entityName}. Allowed: {string.Join(", ", EntityTypeMap.Keys)}");

            var query = new GetAllQuery { EntityType = entityType };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Получить сущность по идентификатору.</summary>
        /// <param name="entityName">Имя сущности.</param>
        /// <param name="id">Id (Guid для dentaloffices, patients, dentists, appointments; long для users, roles, actions).</param>
        [HttpGet("{entityName}/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> GetById(string entityName, string id)
        {
            if (!EntityTypeMap.TryGetValue(entityName, out var entityType))
                return BadRequest($"Unknown entity: {entityName}. Allowed: {string.Join(", ", EntityTypeMap.Keys)}");

            object? parsedId = ParseId(id, entityType);
            if (parsedId == null)
                return BadRequest($"Invalid id format for {entityName}.");

            var query = new GetByIdQuery { EntityType = entityType, Id = parsedId! };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>Создать сущность (тело запроса — JSON сущности).</summary>
        [HttpPost("{entityName}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(string entityName, [FromBody] JsonElement body)
        {
            if (!EntityTypeMap.TryGetValue(entityName, out var entityType))
                return BadRequest($"Unknown entity: {entityName}.");

            object? entity;
            try
            {
                entity = JsonSerializer.Deserialize(body.GetRawText(), entityType);
            }
            catch (JsonException ex)
            {
                return BadRequest(ex.Message);
            }

            if (entity == null)
                return BadRequest("Request body could not be deserialized to entity.");

            var command = new CreateCommand
            {
                EntityType = entityType,
                Entity = entity,
                RequiredActionName = $"Create:{entityType.Name}"
            };
            var created = await _mediator.Send(command);
            return Ok(created);
        }

        /// <summary>Обновить сущность (тело запроса — JSON сущности).</summary>
        [HttpPut("{entityName}/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string entityName, string id, [FromBody] JsonElement body)
        {
            if (!EntityTypeMap.TryGetValue(entityName, out var entityType))
                return BadRequest($"Unknown entity: {entityName}.");

            object? entity;
            try
            {
                entity = JsonSerializer.Deserialize(body.GetRawText(), entityType);
            }
            catch (JsonException ex)
            {
                return BadRequest(ex.Message);
            }

            if (entity == null)
                return BadRequest("Request body could not be deserialized to entity.");

            var command = new UpdateCommand
            {
                EntityType = entityType,
                Entity = entity,
                RequiredActionName = $"Update:{entityType.Name}"
            };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>Удалить сущность по идентификатору.</summary>
        [HttpDelete("{entityName}/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string entityName, string id)
        {
            if (!EntityTypeMap.TryGetValue(entityName, out var entityType))
                return BadRequest($"Unknown entity: {entityName}.");

            object? parsedId = ParseId(id, entityType);
            if (parsedId == null)
                return BadRequest($"Invalid id format for {entityName}.");

            try
            {
                var command = new DeleteCommand
                {
                    EntityType = entityType,
                    Id = parsedId,
                    RequiredActionName = $"Delete:{entityType.Name}"
                };
                await _mediator.Send(command);
            }
            catch (Application.Exceptions.NotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static object? ParseId(string id, Type entityType)
        {
            if (entityType == typeof(DentalOffice) || entityType == typeof(Patient) ||
                entityType == typeof(Dentist) || entityType == typeof(Appointment))
            {
                if (Guid.TryParse(id, out var guid))
                    return guid;
                return null;
            }
            if (entityType == typeof(User) || entityType == typeof(Role) || entityType == typeof(AppAction))
            {
                if (long.TryParse(id, out var longId))
                    return longId;
                return null;
            }
            return null;
        }
    }
}
