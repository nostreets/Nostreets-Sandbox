using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nostreets_Services.Classes.Domain.Users;
using NostreetsExtensions.DataControl.Classes;

namespace Nostreets_Services.Classes.Domain.Product
{
    public enum ProductStage
    {

        Development,
        Launch,
        Sustainment,
        Growth,
        AfterGrowth

    }

    [Flags]
    public enum CompanyResource
    {
        CompanyLogo,
        CompanyNumber,
        CompanyWebsite,
        LabelDesigns,
        LimitedLiabilityInsurance,
        Other
    }

    [Flags]
    public enum CompanyServices
    {
        Consulting,
        Components,
        Development,
        Manufacturing,
        ProductFormulation,
        Shipping,
        Packaging,
        PackageLabeling,
        ProductLabeling,
        WarehouseLabor,
        Other
    }


    public class ProductDevelopment : DBObject
    {
        public ProductDevelopment() { }
        public ProductDevelopment(DelevopProductRequest request)
        {

        }


        public CompanyServices[] ServiceNeededForCompany { get; set; }


        public string FormulaDescription { get; set; }
        public string FormulaProvided { get; set; }
        public string PrototypeOrBenchmark { get; set; }
        public string AddtionalInformation { get; set; }


        public bool IsThereSamples { get; set; }

        public ProductInfo Product { get; set; }

        public Company Company { get; set; }

        public Container Container { get; set; }

        public TargetDemographic Demographic { get; set; }

    }

    public class TargetDemographic : DBObject
    {
        public string ProductsWillBeSold { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }

    }

    public class ProductInfo : DBObject
    {
        public string ProductName { get; set; }

        public string ProductType { get; set; }
        public string ProductAppearance { get; set; }
        public string ProductColor { get; set; }
        public string ProductConsistency { get; set; }

        public string ProductAttributes { get; set; }


        public int RetailPrice { get; set; }
    }

    public class Company : DBObject
    {
        public StreetAddress Address { get; set; }

        public CompanyResource[] CompanyCurrentObtainedResources { get; set; }

        public string CompanyName { get; set; }

        public string CompanyWebsite { get; set; }

        public List<Employee> Contacts { get; set; }

    }

    public class Container : DBObject
    {
        public string ContainerClosure { get; set; }
        public string ContainerSize { get; set; }
        public string ContainerType { get; set; }
    }

    public class DelevopProductRequest
    {
        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string AddtionalInformation { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string CompanyCurrentObtainedResources { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string CompanyWebsite { get; set; }
        public string ContainerClosure { get; set; }
        public string ContainerSize { get; set; }
        public string ContainerType { get; set; }

        [Required]
        public string DescribeProductAttributes { get; set; }

        [Required]
        public string DoYouHaveSamples { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }


        public string DescribeYourProductOutline { get; set; }

        public string DescribeYourBenchmarks { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string ProductAppearance { get; set; }
        public string ProductColor { get; set; }
        public string ProductConsistency { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductsWillBeSold { get; set; }

        [Required]
        public string ProductType { get; set; }


        [Required]
        public string Race { get; set; }

        public int RetailPrice { get; set; }

        [Required]
        public string ServiceNeededForCompany { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public double Timestamp { get; set; }

        [Required]
        public int ZipCode { get; set; }
    }
}