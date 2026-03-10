using CleanTeeth.API.DTOs.Patients;
using CleanTeeth.API.ReferenceCrud;
using CleanTeeth.Application.Features.Patients.Queries.GetPatientDetail;
using CleanTeeth.Application.Features.Patients.Queries.GetPatientsList;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanTeeth.API.Controllers.Ref
{
    /// <summary>
    /// CRUD пациентов через базовый справочный контроллер (ReferenceCrud): общая логика + DTO и маппер.
    /// </summary>
    [ApiController]
    [Route("api/ref/patients")]
    public class PatientsReferenceController : ReferenceCrudControllerBase<Patient, PatientListDTO, PatientDetailDTO, CreatePatientDTO, UpdatePatientDTO>
    {
        public PatientsReferenceController(
            IMediator mediator,
            IReferenceCrudMapper<Patient, PatientListDTO, PatientDetailDTO, CreatePatientDTO, UpdatePatientDTO> mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PatientDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<PatientDetailDTO>> Get(Guid id, CancellationToken cancellationToken = default)
            => GetByIdCore(id, cancellationToken);

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Put(Guid id, [FromBody] UpdatePatientDTO dto, CancellationToken cancellationToken = default)
            => UpdateCore(id, dto, cancellationToken);

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
            => DeleteCore(id, cancellationToken);
    }
}
