using System;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ITaskDataManager
    {
        //public ResponseBaseDataContract UpdateContaCorrente(DateTime dataVencimento);

        //public ResponseBaseDataContract UpdateGuiaPagamento();

        public ResponseBaseDataContract UpdateDeclaracao();

        public ResponseBaseDataContract UpdateTrabalhadorInterno();

        public void DeleteOldImports();
    }
}