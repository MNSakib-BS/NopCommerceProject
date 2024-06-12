using Nop.Core;

namespace Nop.Plugin.Widgets.OurTeam.Domain
{
    public class Employees : BaseEntity
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public bool IsMVP { get; set; }
        public bool IsNopCommerceCertified { get; set; }
        public int EmployeeStatusId { get; set; }
        public int PictureId { get; set; }
        public EmployeeStatus EmployeeStatus { 
            get=> (EmployeeStatus)EmployeeStatusId; 
            set=> EmployeeStatusId=(int)value; 
        }
    }
    
}
