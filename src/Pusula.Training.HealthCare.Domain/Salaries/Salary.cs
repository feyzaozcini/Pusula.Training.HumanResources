using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Salaries
{
    public class Salary : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual decimal BaseAmount { get; set; } //Taban maaş
        [CanBeNull]
        public virtual decimal Bonus { get; set; } // Ek ödemeler
        [CanBeNull]
        public virtual decimal Deduction { get; set; } // Kesintiler
        [NotNull]
        public virtual DateTime EffectiveFrom { get; set; }   // Maaşın geçerli olduğu başlangıç tarihi
        [CanBeNull]
        public virtual DateTime EffectiveTo { get; set; }
        [CanBeNull]
        public virtual decimal TotalAmount { get; set; }

            

        public virtual Guid EmployeeId { get; set; }

        protected Salary()
        {
            BaseAmount= decimal.Zero;
            EffectiveFrom = DateTime.Now;
        }

        public Salary(Guid id, Guid employeeId, decimal baseAmount, decimal bonus, decimal deduction, DateTime effectiveFrom, DateTime effectiveTo, decimal totalAmount)
        {
            Check.NotNull(baseAmount, nameof(baseAmount));
            Check.Range(baseAmount, nameof(baseAmount), SalaryConst.BaseAmountMinLength, SalaryConst.BaseAmountMaxLength);
            Check.Range(bonus, nameof(bonus), SalaryConst.BonusMinLength, SalaryConst.BonusMaxLength);
            Check.Range(deduction, nameof(deduction), SalaryConst.DeductionMinLength, SalaryConst.DeductionMaxLength);
            Check.NotNull(effectiveFrom, nameof(effectiveFrom));
            Check.NotNullOrWhiteSpace(employeeId.ToString(), nameof(employeeId));

            Id = id;
            EmployeeId = employeeId;
            BaseAmount = baseAmount;
            Bonus = bonus;
            Deduction = deduction;
            EffectiveFrom = effectiveFrom;
            EffectiveTo = effectiveTo;
            TotalAmount = totalAmount;
        }
    }
}
