using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IProgramActivityDataManager
    {
        ProgramActivityTreeResponse GetTreeByOrcamentoConfig(GetProgramActivityTreeRequest request);
        ResponseBaseDataContract SaveProgramActivity(SaveProgramActivityRequest request);
        ResponseBaseDataContract DeactivateProgramActivity(DeactivateProgramActivityRequest request);
        ResponseBaseDataContract CopyProgramActivityYear(CopyProgramActivityYearRequest request);
    }
}
