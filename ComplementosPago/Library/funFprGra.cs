using Encrypt;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class funFprGra
    {
        private readonly IConfiguration _configuration;

        public funFprGra(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public class clsGnrc
        {
            public int keyGenr { get; set; }
            public string desGenr { get; set; }
            public DateTime fciGenr { get; set; }
            public DateTime fcfGenr { get; set; }
        }

        public static DataTable graLsdt<T>(IList<T> lstData)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            int noType = 0;
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (!prop.PropertyType.FullName.Contains("FingerPrintsContext.Models"))
                    table.Columns.Add(prop.Name, prop.PropertyType);
                else
                    noType++;
            }
            object[] values = new object[props.Count - noType];
            int j = 0;
            foreach (T item in lstData)
            {
                j = 0;
                for (int i = 0; i < props.Count; i++)//values.Length + noType
                {
                    if (!props[i].PropertyType.FullName.Contains("FingerPrintsContext.Models"))
                        values[j++] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public string graTblt<T>(IList<T> lstData)
        {
            StringBuilder sb = new StringBuilder();
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            string comodin = ",";
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
                string proType = (prop.PropertyType.Name.ToString() == "Decimal" ? "decimal(18,0)" :
                                                   prop.PropertyType.Name.ToString() == "String" ? "nvarchar(max)" :
                                                   prop.PropertyType.Name.ToString() == "Int32" ? "int" :
                                                   prop.PropertyType.Name.ToString() == "DateTime" ? "date" :
                                                   prop.PropertyType.Name.ToString() == "Boolean" ? "bit" :
                                                   prop.PropertyType.Name.ToString() == "TimeSpan" ? "time(7)" :
                                                   prop.PropertyType.Name.ToString() == "Double" ? "float" : "");
                if (!string.IsNullOrEmpty(proType) || !string.IsNullOrWhiteSpace(proType))
                {
                    sb.Append(" " + prop.Name + " " + proType + comodin);
                }
            }
            sb.Replace(",", ")", sb.Length - 1, 1);
            return sb.ToString();
        }

        

        public static bool graInsb(DataTable datTabl, string tblNam)
        {
            bool result = false;
            clsEncDnc cdEncDnc = new clsEncDnc();

   //         string dbsusr = cdEncDnc.dncPss("z/IBNJD+JWwvaGweC+Iguw==");
			//string dbspss = cdEncDnc.dncPss("ZERQWEUAMnbojOSbZi0joQ==");
			//string dbssrv = cdEncDnc.dncPss("zph7DdNl2LZGyyYjgVRzG5zT+V2pCi9tJZU+EYI3Nis=");
			//string dbslbr = cdEncDnc.dncPss("mh6XdzqD3cysleJS+4fzpQ==");
            string dbsusr = "sa";
            string dbspss = "RQmuYcGrTf@9*^";
            string dbssrv = "10.147.17.34";
            string dbslbr = "Labora";
            string cnxStt = $@"uid={dbsusr};pwd={dbspss};data source={dbssrv};database={dbslbr};TrustServerCertificate=True;MultipleActiveResultSets=True";
            //using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["conLabora"].ConnectionString))
            using (var con = new SqlConnection(cnxStt))
            {
                result = true;
                con.Open();
                SqlTransaction tra = con.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tra))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = tblNam; // "dbo.FTP";
                    try
                    {
                        bulkCopy.WriteToServer(datTabl);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        tra.Rollback();
                        con.Close();
                        return result;
                    }
                }
                tra.Commit();
            }
            return result;
        }

        public void graProc(string msg, string file)
        {
            try
            {
                TextWriter tw;
                string nomlog = Environment.CurrentDirectory + @"\" + file + ".txt";
                tw = new StreamWriter(nomlog, true);
                tw.WriteLine(msg);
                tw.Close();
            }
            catch (Exception exf)
            {
                System.Diagnostics.EventLog.WriteEntry("Aplicacion", "Ocurrio el siguiente error " + exf.Message);
            }
        }

        public void graProc(string msg, string file, int keyfpr, int keyzon, int numfpr, int numeve, DateTime datini, DateTime datfin, DateTime datpin)
        {
            TimeSpan tmscnx;
            try
            {
                TextWriter tw;
                string nomlog = Environment.CurrentDirectory + @"\" + file + "_" + datpin.ToString("yyyyMMdd") + ".txt";
                tw = new StreamWriter(nomlog, true);
                tmscnx = datfin - datini;
                if (string.IsNullOrEmpty(msg) || string.IsNullOrWhiteSpace(msg))
                {
                    tw.WriteLine("Lector: " + keyfpr.ToString("000") + "; Zona: " + keyzon.ToString("000") + "; Registros: " + numfpr.ToString("#00,000") +
                                 "; Evento: " + numeve.ToString("00") + "; Tiempo: " + tmscnx.ToString() + "; Hora Inicial:" + datini.ToLongTimeString() +
                                 "; Hora Final:" + datfin.ToLongTimeString());
                }
                else
                {
                    tw.WriteLine("Lector: " + keyfpr.ToString("000") + "; Zona: " + keyzon.ToString("000") + "; Mensaje: " + msg +
                                 "; Evento: " + numeve.ToString("00") + "; Tiempo: " + tmscnx.ToString() + "; Hora Inicial:" + datini.ToLongTimeString() +
                                 "; Hora Final:" + datfin.ToLongTimeString());
                }
                tw.Close();

                //nomlog = Environment.CurrentDirectory + @"\" + file + "_" + keyzon.ToString("000") + ".csv";
                //string[][] outPut = new string[][] {
                //    new string[]{ keyfpr.ToString("000"), keyzon.ToString("000"), numfpr.ToString("#00000"), numeve.ToString("00"), tmscnx.ToString(), datini.ToLongTimeString(), datfin.ToLongTimeString() }
                //};
                //int length = outPut.GetLength(0);
                //StringBuilder sb = new StringBuilder();
                //for (int index = 0; index < length; index++)
                //    sb.AppendLine(string.Join(";", outPut[index]));

                //File.WriteAllText(nomlog, sb.ToString());

            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Aplicacion", "Ocurrio el siguiente error " + ex.Message);
            }
        }

        public List<T> graDtls<T>(DataTable datData)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in datData.Rows)
            {
                T item = graItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T graItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static Task<List<T>> graLsAs<T>(IQueryable<T> query)
        {
            try
            {
                return Task<List<T>>.Run(() =>
              {
                  return query.ToList();
              });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int graWeek(DateTime datWeek)
        {
            DayOfWeek dywWeek = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(datWeek);
            if (dywWeek >= DayOfWeek.Monday && dywWeek <= DayOfWeek.Wednesday)
            {
                datWeek = datWeek.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datWeek, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static List<clsGnrc> graWeek(int datYear, int datMnth)
        {
            List<clsGnrc> datWeks = new List<clsGnrc>();

            int frsWeek = 0;
            int lstWeek = 0;
            DateTime datWeek = new DateTime(datYear, datMnth, 1);
            DateTime datBwkd = new DateTime(datYear, datMnth, 1);

            DayOfWeek dywWeek = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(datWeek);
            if (dywWeek >= DayOfWeek.Monday && dywWeek <= DayOfWeek.Wednesday)
            {
                datWeek = datWeek.AddDays((int)dywWeek - 1);
            }
            else if (dywWeek >= DayOfWeek.Thursday && dywWeek <= DayOfWeek.Saturday)
            {
                datWeek = datWeek.AddDays(-(int)dywWeek + 1);
            }
            frsWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datWeek, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            datBwkd = datWeek;
            frsWeek = frsWeek == 53 ? 1 : frsWeek;

            int mnthDat = 0;
            int yearDat = 0;

            yearDat = datYear;
            mnthDat = datMnth;

            if (mnthDat == 12)
            {
                mnthDat = 1;
                yearDat++;
            }
            else
            {
                mnthDat++;
            }

            datWeek = new DateTime(yearDat, mnthDat, 1);
            datWeek = datWeek.AddDays(-1);
            dywWeek = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(datWeek);
            if (dywWeek >= DayOfWeek.Monday && dywWeek <= DayOfWeek.Wednesday)
            {
                datWeek = datWeek.AddDays(3);
            }
            lstWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(datWeek, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            lstWeek = lstWeek == 53 ? lstWeek - 1 : lstWeek;

            for (; frsWeek <= lstWeek; frsWeek++)
            {
                datWeks.Add(new clsGnrc() { keyGenr = frsWeek, desGenr = "Semana No." + frsWeek.ToString("00"), fciGenr = datBwkd, fcfGenr = datBwkd.AddDays(6) });
                datBwkd = datBwkd.AddDays(7);
            }
            return datWeks;
        }

        public static DateTime graFrst(int datYear, int datWeek, CultureInfo datClti)
        {
            DateTime frsDayy = new DateTime(datYear, 1, 1);
            int dosDays = (int)datClti.DateTimeFormat.FirstDayOfWeek - (int)frsDayy.DayOfWeek;
            DateTime firstWeekDay = frsDayy.AddDays(dosDays);
            //int firstWeek = datClti.Calendar.GetWeekOfYear(frsDayy, datClti.DateTimeFormat.CalendarWeekRule, datClti.DateTimeFormat.FirstDayOfWeek);
            int firstWeek = datClti.Calendar.GetWeekOfYear(frsDayy, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if ((firstWeek <= 1 || firstWeek >= 52) && dosDays >= -3)
            {
                datWeek -= 1;
            }
            //else
            //{
            //    datWeek -= Math.Abs(dosDays) + 1;
            //}
            return firstWeekDay.AddDays(datWeek * 7);
        }

        public static Type graGtls<T>(List<T> lstData)
        {
            return typeof(T);
        }

        public string getStt(string key)
        {
            return _configuration[key] ?? _configuration.GetSection(key)?.Value;
        }

       
    }
}
