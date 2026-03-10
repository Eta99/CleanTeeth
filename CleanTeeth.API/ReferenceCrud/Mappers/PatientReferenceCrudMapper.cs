using CleanTeeth.API.DTOs.Patients;
using CleanTeeth.Application.Features.Patients.Queries.GetPatientDetail;
using CleanTeeth.Application.Features.Patients.Queries.GetPatientsList;
using CleanTeeth.Domain.Entities;
using CleanTeeth.Domain.ValueObjects;

namespace CleanTeeth.API.ReferenceCrud.Mappers
{
    public class PatientReferenceCrudMapper : IReferenceCrudMapper<Patient, PatientListDTO, PatientDetailDTO, CreatePatientDTO, UpdatePatientDTO>
    {
        public PatientListDTO ToListDto(Patient entity)
        {
            return new PatientListDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email.Value
            };
        }

        public PatientDetailDTO ToDetailDto(Patient entity)
        {
            return new PatientDetailDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email.Value
            };
        }

        public Patient ToEntity(CreatePatientDTO dto)
        {
            return new Patient(dto.Name, new Email(dto.Email));
        }

        public void ApplyUpdate(Patient entity, UpdatePatientDTO dto)
        {
            entity.UpdateName(dto.Name);
            entity.UpdateEmail(new Email(dto.Email));
        }

        public object GetId(Patient entity)
        {
            return entity.Id;
        }
    }
}
