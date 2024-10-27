﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryUpdateDto
    {
        [Required]
        public Guid Id { get; set; } = default!;

        [Required]
        public decimal BaseAmount { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deduction { get; set; }
        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime EffectiveTo { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }
    }
}