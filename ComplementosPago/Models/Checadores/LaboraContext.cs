//using Encrypt;
//using Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class LaboraContext : DbContext
    {
        //public virtual Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> HasNoKey();
        //public DbSet<LBCH> molochec { get; set; }
        public DbSet<LBPR> nmloperi { get; set; }
        public DbSet<LBEM> nmcoempl { get; set; }
        public DbSet<LBCH> molochec { get; set; }

  //      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //      {
		//	funFprGra fncGral = new funFprGra();
		//	clsEncDnc cdEncDnc = new clsEncDnc();

		//	string dbsusr = cdEncDnc.dncPss(fncGral.getStt("dbsusr"));
		//	string dbspss = cdEncDnc.dncPss(fncGral.getStt("dbspss"));
		//	string dbssrv = cdEncDnc.dncPss(fncGral.getStt("dbssrv"));
		//	string dbslbr = cdEncDnc.dncPss(fncGral.getStt("dbslbr"));
		//	string cnxStt = $@"uid={dbsusr};pwd={dbspss};data source={dbssrv};database={dbslbr};TrustServerCertificate=True;MultipleActiveResultSets=True";
		//	////optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["conLabora"].ConnectionString);
		//	optionsBuilder.UseSqlServer(cnxStt);
		//}

  //      public void ConfigureServices(IServiceCollection services)
  //      {
		//	//////<add name="conLabora" providerName="SQLServer" connectionString="uid=sa;pwd=S01s3RzOzE;data source=SQLSAPB1\SQLSAPBO;database=Labora;TrustServerCertificate=True;MultipleActiveResultSets=True" />
		//	funFprGra fncGral = new funFprGra();
		//	clsEncDnc cdEncDnc = new clsEncDnc();

		//	string dbsusr = cdEncDnc.dncPss(fncGral.getStt("dbsusr"));
		//	string dbspss = cdEncDnc.dncPss(fncGral.getStt("dbspss"));
		//	string dbssrv = cdEncDnc.dncPss(fncGral.getStt("dbssrv"));
		//	string dbslbr = cdEncDnc.dncPss(fncGral.getStt("dbslbr"));
		//	string cnxStt = $@"uid={dbsusr};pwd={dbspss};data source={dbssrv};database={dbslbr};TrustServerCertificate=True;MultipleActiveResultSets=True";
		//	////services.AddDbContext<LaboraContext>(options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["conLabora"].ConnectionString));
		//	services.AddDbContext<LaboraContext>(options => options.UseSqlServer(cnxStt));
		//}

		protected override void OnModelCreating(ModelBuilder builder)
        {
			//var cascadas = builder.Model.GetEntityTypes()
			//	.SelectMany(t => t.Name);

			//builder.Entity<LBPR>().HasNoKey();
			builder.Entity<LBPR>(entity => { entity.HasNoKey(); });
            builder.Entity<LBEM>(entity => { entity.HasNoKey(); });
            builder.Entity<LBCH>(entity => { entity.HasNoKey(); });

            var cascadas = builder.Model.GetEntityTypes()
						 .SelectMany(t => t.GetForeignKeys())
						 .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

			foreach (var fk in cascadas)
			{
				fk.DeleteBehavior = DeleteBehavior.Restrict;
			}

			base.OnModelCreating(builder);
        }


    }
}
