﻿namespace SafriSoftv1._3.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCell { get; set; }
        public string CustomerAddress { get; set; }
        public string DateCustomerCreated { get; set; }
        public int NumberOfOrders { get; set; }
    }
}