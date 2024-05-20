using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ECO.DAL
{
    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("data source=172.28.10.28;initial catalog=ECO_DXForPUR;user id=sa;password=umc@123;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }
  //        <connectionStrings>
  //<add name = "ECOConnection" connectionString="data source=172.28.10.28;initial catalog=ECO_DXForPUR;user id=sa;password=umc@123;&#xD;&#xA;       MultipleActiveResultSets=True;App=EntityFramework"
  //                                                        providerName="System.Data.SqlClient" />
  //</connectionStrings>
        public virtual DbSet<WoChanging> WoChangings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WoChanging>()
                .Property(e => e.ORDER_NO)
                .IsUnicode(false);

            modelBuilder.Entity<WoChanging>()
                .Property(e => e.ECO_NO)
                .IsUnicode(false);
        }
    }
}
