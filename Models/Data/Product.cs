﻿using SafriSoftv1._3.Models.SystemModels;
using System.ComponentModel.DataAnnotations;

namespace SafriSoftv1._3.Models.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductReference { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string ProductImage { get; set; }
        public double Cost { get; set; }
        public double SellingPrice { get; set; }
        public int? ItemsSold { get; set; }
        public int ItemsAvailable { get; set; }
        public string Status { get; set; }
        public ProductType ProductType { get; set; }
        public int InventoryAccountId { get; set; }
        public int OrganisationId { get; set; }
    }
}