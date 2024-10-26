using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryManager(ISalaryRepository salaryRepository) : DomainService
    {
        public virtual async Task<Salary> CreateAsync(
             Guid employeeId, decimal baseAmount, decimal bonus, decimal deduction, DateTime effectiveFrom, DateTime effectiveTo, decimal totalAmount)
        {
            Check.NotNull(baseAmount, nameof(baseAmount));
            Check.Range(baseAmount, nameof(baseAmount), SalaryConst.BaseAmountMinLength, SalaryConst.BaseAmountMaxLength);
            Check.Range(bonus, nameof(bonus), SalaryConst.BonusMinLength, SalaryConst.BonusMaxLength);
            Check.Range(deduction, nameof(deduction), SalaryConst.DeductionMinLength, SalaryConst.DeductionMaxLength);
            Check.NotNull(effectiveFrom, nameof(effectiveFrom));
            Check.NotNullOrWhiteSpace(employeeId.ToString(), nameof(employeeId));

            var salary = new Salary(
                GuidGenerator.Create(), employeeId, baseAmount,bonus, deduction, effectiveFrom, effectiveTo, totalAmount
                );

            return await salaryRepository.InsertAsync(salary);
        }

        public virtual async Task<Salary> UpdateAsync(
            Guid id, Guid employeeId, decimal baseAmount, decimal bonus, decimal deduction, DateTime effectiveFrom, DateTime effectiveTo, decimal totalAmount)
        {

            Check.NotNull(baseAmount, nameof(baseAmount));
            Check.Range(baseAmount, nameof(baseAmount), SalaryConst.BaseAmountMinLength, SalaryConst.BaseAmountMaxLength);
            Check.Range(bonus, nameof(bonus), SalaryConst.BonusMinLength, SalaryConst.BonusMaxLength);
            Check.Range(deduction, nameof(deduction), SalaryConst.DeductionMinLength, SalaryConst.DeductionMaxLength);
            Check.NotNull(effectiveFrom, nameof(effectiveFrom));
            Check.NotNullOrWhiteSpace(employeeId.ToString(), nameof(employeeId));

            var salary = await salaryRepository.GetAsync(id);

            salary.EmployeeId = employeeId;
            salary.BaseAmount = baseAmount;
            salary.Bonus = bonus;
            salary.Deduction = deduction;
            salary.EffectiveFrom = effectiveFrom;
            salary.EffectiveTo = effectiveTo;
            salary.TotalAmount = totalAmount;

            return await salaryRepository.UpdateAsync(salary);

        }
    }
}
