using Nest;
using System;
using System.Linq;

namespace GeorgDuncker.Vessels
{
    public class VesselMappingDocument
    {
        public VesselMappingDocument(Vessel VesselIndex)
        {
            Id = VesselIndex.Id;
            Vessels = new[]
            {
            VesselIndex.GeneralInformation.Name,
            VesselIndex.Manager,
            VesselIndex.OwningCompany,
            VesselIndex.GeneralInformation.IMO,
            VesselIndex.TechnicalDetails.DWT.ToString(),
            VesselIndex.GeneralInformation.GT.ToString(),
            VesselIndex.GeneralInformation.Type.Label.ToString(),
            }

            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

            IMO = VesselIndex.GeneralInformation.IMO;
            Name = VesselIndex.GeneralInformation.Name;
            OwningCompany = VesselIndex.OwningCompany;
            Manager = VesselIndex.Manager;
            GT = VesselIndex.GeneralInformation.GT.ToString();
            DWT = VesselIndex.TechnicalDetails.DWT.ToString();
            Type = VesselIndex.GeneralInformation.Type;
            
        }

        public Guid Id { get; set; }
       
        public string[] Vessels { get; set; }
        
        public string IMO { get; set; }
                
        public string Name { get; set; }

        [Keyword]
        public string DWT { get; set; }
        
        [Keyword]
        public string GT { get; set; }
      
        public VesselType Type { get; set; }
        
        [Keyword]
        public string Manager { get; set; }
        [Keyword]
        public string OwningCompany { get; set; }
    }
}

