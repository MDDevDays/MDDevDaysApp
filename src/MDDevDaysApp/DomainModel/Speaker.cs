using System;

namespace MDDevDaysApp.DomainModel
{
    public class Speaker
    {
        private string _fullname;

        public Guid Id { get; set; }
        public string AcademicTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUri { get; set; }
        public string Bio { get; set; }

        public string FullName => _fullname ?? (_fullname = $"{AcademicTitle} {FirstName} {LastName}".Trim());
    }
}
