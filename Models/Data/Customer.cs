using System.ComponentModel.DataAnnotations;

namespace SafriSoftv1._3.Models.Data
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCell { get; set; }
        public string CustomerAddress { get; set; }
        public string Street { get; set; }
        public string Surburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int? PostalCode { get; set; }
        public string DateCustomerCreated { get; set; }
        public int NumberOfOrders { get; set; }
        public string Status { get; set; }
        public int OrganisationId { get; set; }
    }
}