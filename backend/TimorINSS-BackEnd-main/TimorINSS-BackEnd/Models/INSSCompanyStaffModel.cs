using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class INSSCompanyStaffModel
    {
        

        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ContributeMonth { get; set; }
        public double? ContributeMoney { get; set; }
       
    }
}
