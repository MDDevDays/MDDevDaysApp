using System;

namespace MDDevDaysApp.DomainModel
{
    public class Speaker
    {
        public Guid Id { get; set; }
        public string AcademicTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUri { get; set; }
        public string Bio { get; set; }

        public string FullName => $"{AcademicTitle} {FirstName} {LastName}".Trim();
    }
}
