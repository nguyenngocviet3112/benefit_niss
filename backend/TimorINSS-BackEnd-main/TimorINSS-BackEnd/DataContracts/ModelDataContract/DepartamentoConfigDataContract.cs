using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // Distinct from the legacy DepartamentoDataContract/DepartamentoListagem (camelCase, id/nome only,
    // used by the old dropdown endpoints) — this is the new-mode admin/CRUD screen contract, under
    // Hệ thống > Cấu hình phòng ban. Reuses the existing [DEPARTAMENTO] table (11 real rows already
    // seeded, tied to InstitutionId) instead of a new satellite table — this is master data, not a
    // transactional entity, so no additive-table split needed.
    [DataContract]
    public class DepartamentoConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }

        [DataMember]
        public string InstitutionNome { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }
    }
}
