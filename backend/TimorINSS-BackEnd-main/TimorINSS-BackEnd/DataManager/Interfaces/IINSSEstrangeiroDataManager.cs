using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IINSSEstrangeiroDataManager
    {
        public ResponseBaseDataContract EditINSSEstrangeiro(INSSEstrangeiroRequest request);

        public ResponseBaseDataContract SaveINSSEstrangeiro(INSSEstrangeiroRequest request);
    }
}