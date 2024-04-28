using SafriSoftv1._3.Models.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SafriSoftv1._3.Models
{
    public class SafriSoftDbContext : DbContext
    {
        public SafriSoftDbContext() : base("name=SafriSoftDbContext")
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<InboxMessages> InboxMessages { get; set; }
        public virtual DbSet<InboxReplies> InboxReplies { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderAudit> OrderAudit { get; set; }
        public virtual DbSet<TrialBalanceAccount> TrialBalanceAccounts { get; set; }
        public virtual DbSet<GeneralLedger> GeneralLedgers { get; set; }
        public virtual DbSet<TrialBalanceGeneralLedgerMapping> TrialBalanceGeneralLedgerMappings { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public static SafriSoftDbContext Create()
        {
            return new SafriSoftDbContext();
        }
    }
}