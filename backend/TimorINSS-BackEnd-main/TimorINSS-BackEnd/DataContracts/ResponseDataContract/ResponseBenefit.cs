using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ResponseBenefit : ResponseBaseDataContract
    {
        [DataMember]
        public List<SelectBenefit> selects { get; set; }
    }

    [DataContract]
    public class SelectBenefit
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string HoTen { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int DiemTichLuy { get; set; }

        [DataMember]
        public DateTime NgaySinh { get; set; }


    }
}