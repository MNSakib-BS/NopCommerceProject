using Nop.Core;

namespace Nop.Plugin.Misc.NopstationTeamTest.Domain
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        /*public IFormFile Profile { get; set; }
        public byte[] ProfilePicture { get; set; }*/ // Add this line
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
