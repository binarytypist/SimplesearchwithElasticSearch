using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using GeorgDuncker.Vessels.Values;

namespace GeorgDuncker.Vessels
{
    public class Vessel : AggregateRoot<Guid>
    {
        [Required]
        public virtual GeneralInformation GeneralInformation { get; set; } = new GeneralInformation();

        [Required]
        public virtual TechnicalDetails TechnicalDetails { get; set; } = new TechnicalDetails();

        [Required]
        public virtual InsuranceDetails InsuranceDetails { get; set; } = new InsuranceDetails();

        public virtual string OwningCompany { get; set; }

        public virtual string BareboatCharterer { get; set; }

        public virtual string Manager { get; set; }

        public virtual string CommercialManager { get; set; }

        public virtual string SubManager { get; set; }

        public virtual string Mortgagee { get; set; }
    }
}
