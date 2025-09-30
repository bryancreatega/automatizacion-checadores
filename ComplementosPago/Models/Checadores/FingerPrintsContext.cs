//using Encrypt;
//using Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class FingerPrintsContext : DbContext
    {
        public FingerPrintsContext(DbContextOptions<FingerPrintsContext> options)
        : base(options)
        {
        }

        public DbSet<DST> DSTF { get; set; }
        public DbSet<FPR> FPRS { get; set; }
        public DbSet<OBK> OBAK { get; set; }
        public DbSet<OCD> OCHD { get; set; }//checadas eliminadas
        public DbSet<OCH> OCHE { get; set; }//checadas
        public DbSet<ODL> ODEL { get; set; }
        public DbSet<ODT> ODTS { get; set; }
        public DbSet<OGT> OGET { get; set; }
        public DbSet<LIC> OLCN { get; set; }//licencia
        public DbSet<OPR> OPRS { get; set; }
        public DbSet<OST> OSET { get; set; }
        public DbSet<OTB> OTAB { get; set; }//replicas
        public DbSet<SIF> STFP { get; set; }//relacion de muchos a muchos empleados y lectores
        public DbSet<STF> STFS { get; set; }//empleados
        public DbSet<USF> USFP { get; set; }//usuarios lectores
        public DbSet<USU> USUS { get; set; }
        public DbSet<ZON> ZONS { get; set; }
        public DbSet<BIT> BITA { get; set; }
        public DbSet<ATM> ATMS { get; set; } //Tareas automaticas
        public DbSet<ACT> ACTS { get; set; } //Actualizaciones en tablas
        public DbSet<PAR> PAR { get; set; } //Parametros
        public DbSet<PRO> PRO { get; set; } //Procesos
        public DbSet<ESTPR> ESTPR { get; set; } //Estatus Procesos
        public DbSet<LOAT> LOAT { get; set; } //Log de Automatizaciones
        public DbSet<COAT> COAT { get; set; } //Lista de correos
        public DbSet<CFAT> CFAT { get; set; } //Configuracion del SMTP




        //public FingerPrintsContext()
        //{
        //          //Database.Migrate();
        //	//Database.EnsureCreated();
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    funFprGra fncGral = new funFprGra();
        //    clsEncDnc cdEncDnc = new clsEncDnc();

        //    string dbsusr = cdEncDnc.dncPss(fncGral.getStt("dbsusr")); //Usuario
        //    string dbspss = cdEncDnc.dncPss(fncGral.getStt("dbspss")); //Contraseña
        //    string dbssrv = cdEncDnc.dncPss(fncGral.getStt("dbssrv")); //Ip
        //    string dbsfng = cdEncDnc.dncPss(fncGral.getStt("dbsfng")); //Base de datos
        //    string cnxStt = $@"uid={dbsusr};pwd={dbspss};data source={dbssrv};database={dbsfng};TrustServerCertificate=True;MultipleActiveResultSets=True";
        //    optionsBuilder.UseSqlServer(cnxStt);

        //    //si vas a aplicar migracion: 
        //    //optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["conLectores"].ConnectionString);
        //}

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    funFprGra fncGral = new funFprGra();
        //    clsEncDnc cdEncDnc = new clsEncDnc();

        //    string dbsusr = cdEncDnc.dncPss(fncGral.getStt("dbsusr"));
        //    string dbspss = cdEncDnc.dncPss(fncGral.getStt("dbspss"));
        //    string dbssrv = cdEncDnc.dncPss(fncGral.getStt("dbssrv"));
        //    string dbsfng = cdEncDnc.dncPss(fncGral.getStt("dbsfng"));
        //    string cnxStt = $@"uid={dbsusr};pwd={dbspss};data source={dbssrv};database={dbsfng};TrustServerCertificate=True;MultipleActiveResultSets=True";
        //    //services.AddDbContext<FingerPrintsContext>(options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["conLectores"].ConnectionString));
        //    services.AddDbContext<FingerPrintsContext>(options => options.UseSqlServer(cnxStt));
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<USU>().HasData(new USU
            {
                usu_keyusu = 1,
                usu_namusu = "Crt",
                usu_dscusu = "Createga",
                usu_conusu = "pIw7VdZkOd9cm69R/0YR6A==",//CONTRASEÑA = Createga@2025
                usu_fnpusu = 1,
                usu_bckusu = 1,
                usu_rplusu = 1,
                usu_rmvusu = 1,
                usu_synusu = 1,
                usu_mrkusu = 1,
                usu_rptusu = 1,
                usu_sttusu = 1,
                usu_stausu = 1,
                usu_AdminConf = 1
            });

            builder.Entity<DST>(entity => { entity.HasKey(e => e.dst_keydst); });//entity.HasAlternateKey(e => e.sff_tmpsff); entity.HasAlternateKey(e => e.stf_keystf);
            builder.Entity<STF>(entity => { entity.HasKey(e => e.stf_keystf); entity.HasAlternateKey(e => e.stf_numstf); });
            builder.Entity<OCD>(entity => { entity.HasKey(e => e.ocd_keyocd); entity.HasIndex(e => new { e.ocd_datocd, e.ocd_horocd, e.stf_numstf, e.fpr_keyfpr }).IsUnique(); }); //checadas eliminadas
            builder.Entity<OCH>(entity => { entity.HasKey(e => e.och_keyoch); entity.HasIndex(e => new { e.och_datoch, e.och_horoch, e.stf_numstf, e.fpr_keyfpr }).IsUnique(); }); //checdas//, e.och_hrpoch

            builder.Entity<SIF>(entity => { entity.HasKey(e => e.sif_keysif); });

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
