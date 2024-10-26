using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeManager(IEmployeeRepository employeeRepository) : DomainService
    {
        public virtual async Task<Employee> CreateAsync(
             Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, string email, string mobilePhoneNumber, EnumGender gender)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), EmployeeConst.FirstNameMaxLength, EmployeeConst.FirstNameMinLength);
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), EmployeeConst.LastNameMaxLength, EmployeeConst.LastNameMinLength);
            Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), EmployeeConst.IdentityNumberMaxLength, 0);
            Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), EmployeeConst.MobilePhoneNumberMaxLength);
            Check.NotNull(birthDate, nameof(birthDate));
            Check.NotNull(email, nameof(email));
            Check.Length(email, nameof(email), EmployeeConst.EmailMaxLength, 0);
            Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
            //gender = Enum.IsDefined(typeof(EnumGender), gender) ? gender : throw new ArgumentException($"Geçersiz cinsiyet değeri: {gender}", nameof(gender));


            var employee = new Employee(
                GuidGenerator.Create(), departmentId, firstName, lastName, identityNumber, birthDate, email, mobilePhoneNumber,gender
                );

            return await employeeRepository.InsertAsync(employee);
        }

        public virtual async Task<Employee> UpdateAsync(
            Guid id, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, string email, string mobilePhoneNumber)
        {

            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), EmployeeConst.FirstNameMaxLength, EmployeeConst.FirstNameMinLength);
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), EmployeeConst.LastNameMaxLength, EmployeeConst.LastNameMinLength);
            Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), EmployeeConst.IdentityNumberMaxLength, 0);
            Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), EmployeeConst.MobilePhoneNumberMaxLength);
            Check.NotNull(birthDate, nameof(birthDate));
            Check.NotNull(email, nameof(email));
            Check.Length(email, nameof(email), EmployeeConst.EmailMaxLength, 0);
            Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));

            var employee = await employeeRepository.GetAsync(id);

            employee.DepartmentId = departmentId;
            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.IdentityNumber = identityNumber;
            employee.BirthDate = birthDate;
            employee.Email = email;
            employee.MobilePhoneNumber = mobilePhoneNumber;

            return await employeeRepository.UpdateAsync(employee);
            
        }
    }
}
