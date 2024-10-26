using JetBrains.Annotations;
using System;
using System.Net.Mail;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;


namespace Pusula.Training.HealthCare.Employees
{
    public class Employee : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string FirstName { get; set; }

        [NotNull]
        public virtual string LastName { get; set; }

        [NotNull]
        public virtual string IdentityNumber { get; set; }

        [NotNull]
        public virtual DateTime BirthDate { get; set; }

        [NotNull]
        public virtual string Email { get; set; }

        [NotNull]
        public virtual string MobilePhoneNumber { get; set; }
        
        //Tekrar kontrol et
        [CanBeNull]
        public virtual EnumGender Gender { get; set; }

        
        public virtual Guid DepartmentId { get; set; }

        protected Employee()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            IdentityNumber = string.Empty;
            BirthDate = DateTime.Now;
            Email = string.Empty;
            MobilePhoneNumber = string.Empty;
        }

        public Employee(Guid id, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate,string email,string mobilePhoneNumber, EnumGender gender)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), EmployeeConst.FirstNameMaxLength, EmployeeConst.FirstNameMinLength);
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), EmployeeConst.LastNameMaxLength, EmployeeConst.LastNameMinLength);
            Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), EmployeeConst.IdentityNumberMaxLength, 0);
            Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), EmployeeConst.MobilePhoneNumberMaxLength);
            Check.NotNull(birthDate, nameof(birthDate));
            Check.NotNull(email, nameof(email));
            Check.Length(email, nameof(email), EmployeeConst.EmailMaxLength, 0);
            Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));

            /*if (!Enum.IsDefined(typeof(EnumGender), gender))
            {
                throw new ArgumentException($"Geçersiz cinsiyet: {gender}", nameof(gender));
            }*/

            Id = id;
            DepartmentId = departmentId;
            FirstName = firstName;
            LastName = lastName;
            IdentityNumber = identityNumber;            
            BirthDate = birthDate;
            Email = email;
            MobilePhoneNumber = mobilePhoneNumber;
            Gender = gender;
        }
    }
}
