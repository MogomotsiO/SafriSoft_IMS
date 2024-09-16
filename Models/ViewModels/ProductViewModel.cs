using SafriSoftv1._3.Models.SystemModels;
using System.Collections.Generic;

namespace SafriSoftv1._3.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductReference { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string ProductImage { get; set; }
        public double Cost { get; set; }
        public double SellingPrice { get; set; }
        public int ItemsSold { get; set; }
        public int ItemsAvailable { get; set; }
        public int TotalItems { get; set; }
        public double TotalItemsCost { get; set; }
        public double ExpectedProfit { get; set; }
        public double CurrProfit { get; set; }
        public int InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public ProductType ProductType { get; set; }
        public string ProductTypeStr { get; set; }
        public int[] Products { get; set; }
        public List<RequirementViewModel> Requirements { get; set; } = new List<RequirementViewModel>();

    }

    public class RequirementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public int QtyAvailable { get; set; }
    }

    public class AddQuantityViewModel
    {
        public int ProductId { get; set; }

        public int Qty { get; set; }

        public List<RequirementViewModel> Requirements { get; set; } = new List<RequirementViewModel>();
    }
}