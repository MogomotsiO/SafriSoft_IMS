using Microsoft.Owin;
using Owin;
using System.Data.Entity.Migrations;

[assembly: OwinStartupAttribute(typeof(SafriSoftv1._3.Startup))]
namespace SafriSoftv1._3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DbMigrator dbMigrator = new DbMigrator(new SafriSoftv1._3.Migrations.Configuration());
            dbMigrator.Update(null);

            ConfigureAuth(app);
        }
    }
}
