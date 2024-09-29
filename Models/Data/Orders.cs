using System;
using System.ComponentModel.DataAnnotations;

namespace SafriSoftv1._3.Models.Data
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public int NumberOfItems { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderStatus { get; set; }
        public int OrderProgress { get; set; }
        public string DateOrderCreated { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime DateDue { get; set; }
        public string Status { get; set; }
        public decimal? OrderWorth { get; set; }
        public decimal? ShippingCost { get; set; }
        public string UserId { get; set; }
        public int OrganisationId { get; set; }
    }
}