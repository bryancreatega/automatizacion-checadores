using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;

using ModelContext.Models;

namespace Library
{
    public class libFprZkx
    {
        CZKEMClass czkFprd;

        public string errorGral = string.Empty;

        //Conectar lector
        //ZKtecko
        public bool zktConx(string dirIpad, int numMacn)
        {
            int errCodg = 0;
            bool cnxFprd = false;
            string error = string.Empty;
            try
            {
                if (czkFprd == null)
                {
                    czkFprd = new CZKEMClass();
                }
                else
                {
                    czkFprd = null;
                    czkFprd = new CZKEMClass();
                }
                cnxFprd = czkFprd.Connect_Net(dirIpad, 4370);
                if (cnxFprd)
                {
                    czkFprd.RegEvent(numMacn, 65535);
                }
                else
                {
                    cnxFprd = false;
                    czkFprd.GetLastError(ref errCodg);
                    error = "Codigo de Error Conexión: " + errCodg.ToString();
                }
            }
            catch (AccessViolationException ax)
            {
                error = $"Codigo de Error Acceso: {ax}";
                cnxFprd = false;
            }
            catch (Exception ex)
            {
                error = $"Codigo de Error Excepción: {ex}";
                cnxFprd = false;
            }
            errorGral = error;

            return cnxFprd;
        }

        //Desconectar lector
        public bool zktDisc()
        {
            bool cnxFprd = false;
            try
            {
                czkFprd.Disconnect();
                cnxFprd = true;
            }
            catch (Exception ex)
            {
                cnxFprd = false;
            }
            return cnxFprd;
        }

        //Obtener información del lector
        public int zktInfo(int numMacn, bool cnxMacn, out string fpr_macfpr, out string fpr_frmfpr, out string fpr_cdgfpr, out string fpr_plffpr, out string fpr_srnfpr, out string fpr_sdkfpr, out string fpr_thrfpr, out int fpr_fpafpr,
                           out int fpr_fcafpr, out int fpr_usrfpr, out int fpr_admfpr, out int fpr_pwdfpr, out int fpr_oplfpr, out int fpr_attfpr, out int fpr_facfpr, out int fpr_fpnfpr)
        {
            int result = 0;

            #region información del lector de huellas
            fpr_macfpr = "";
            fpr_frmfpr = "";
            fpr_cdgfpr = "";
            fpr_plffpr = "";
            fpr_srnfpr = "";
            fpr_sdkfpr = "";
            fpr_thrfpr = "";
            fpr_fpafpr = 0;//algoritmo de huella
            fpr_fcafpr = 0;//algoritmo de rostro
            #endregion
            #region capacidad del lector de huellas
            fpr_usrfpr = 0;
            fpr_admfpr = 0;
            fpr_pwdfpr = 0;
            fpr_oplfpr = 0;
            fpr_attfpr = 0;
            fpr_facfpr = 0;
            fpr_fpnfpr = 0;
            #endregion

            string strTemp = "";

            if (cnxMacn)
            {
                czkFprd.EnableDevice(numMacn, false);
                #region información del lector de huellas
                czkFprd.GetDeviceMAC(numMacn, ref fpr_macfpr);
                czkFprd.GetFirmwareVersion(numMacn, ref fpr_frmfpr);
                czkFprd.GetProductCode(numMacn, out fpr_cdgfpr);
                czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                czkFprd.GetSerialNumber(numMacn, out fpr_srnfpr);
                czkFprd.GetSDKVersion(ref fpr_sdkfpr);
                czkFprd.GetDeviceStrInfo(numMacn, 1, out fpr_thrfpr);

                czkFprd.GetSysOption(numMacn, "~ZKFPVersion", out strTemp);
                fpr_fpafpr = Convert.ToInt32(strTemp);
                czkFprd.GetSysOption(numMacn, "ZKFaceVersion", out strTemp);
                fpr_fcafpr = Convert.ToInt32((string.IsNullOrEmpty(strTemp) || string.IsNullOrWhiteSpace(strTemp)) ? "0" : strTemp);
                #endregion
                #region capacidad del lector de huellas
                czkFprd.GetDeviceStatus(numMacn, 2, ref fpr_usrfpr);
                czkFprd.GetDeviceStatus(numMacn, 1, ref fpr_admfpr);
                czkFprd.GetDeviceStatus(numMacn, 4, ref fpr_pwdfpr);
                czkFprd.GetDeviceStatus(numMacn, 5, ref fpr_oplfpr);
                czkFprd.GetDeviceStatus(numMacn, 6, ref fpr_attfpr);
                czkFprd.GetDeviceStatus(numMacn, 21, ref fpr_facfpr);
                czkFprd.GetDeviceStatus(numMacn, 3, ref fpr_fpnfpr);
                #endregion
                czkFprd.EnableDevice(numMacn, true);
                result = 1;
            }
            else
            {
                errorGral = "Error Información";
                result = 0;
            }

            return result;
        }

        //Obtener Fecha y Hora del Lector
        public string zktGett(int numMacn, bool cnxMacn)
        {
            string result = string.Empty;
            #region variables para la captura de la hora
            int get_añoget = 0;
            int get_mesget = 0;
            int get_diaget = 0;
            int get_horget = 0;
            int get_minget = 0;
            int get_segget = 0;
            #endregion

            try
            {
                if (cnxMacn)
                {
                    if (czkFprd.GetDeviceTime(numMacn, ref get_añoget, ref get_mesget, ref get_diaget, ref get_horget, ref get_minget, ref get_segget))
                    {
                        result = get_añoget.ToString() + "-" + get_mesget.ToString() + "-" + get_diaget.ToString() + " " + get_horget.ToString() + ":" + get_minget.ToString() + ":" + get_segget.ToString();
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            zktDisc();
            return result;
        }

        //Mandar Fecha y Hora del Lector
        public string zktSett(int numMacn, bool cnxMacn, int set_typset, DateTime set_datset, TimeSpan set_hraset, int set_incset)
        {
            bool resultB = false;
            string result = string.Empty;
            #region variables para la captura de la hora
            int set_añoset = 0;
            int set_messet = 0;
            int set_diaset = 0;
            int set_horset = 0;
            int set_minset = 0;
            int set_segset = 0;
            #endregion

            if (set_typset == 1)
            {
                #region envio de la fecha y hora del la computadora
                try
                {
                    if (cnxMacn)
                    {
                        if (czkFprd.SetDeviceTime(numMacn))
                        {
                            resultB = true;
                            result = zktGett(numMacn, true);
                        }
                        else
                        {
                            resultB = false;
                            result = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultB = false;
                }
                #endregion
            }
            else if (set_typset == 2)
            {
                #region variables de asignacion de fecha
                set_añoset = set_datset.Year;
                set_messet = set_datset.Month;
                set_diaset = set_datset.Day;
                set_horset = set_hraset.Hours;
                set_minset = set_hraset.Minutes;
                set_segset = set_hraset.Seconds;
                #endregion
                #region envio de la fecha y hora manualmente
                try
                {
                    if (cnxMacn)
                    {
                        if (czkFprd.SetDeviceTime2(numMacn, set_añoset, set_messet, set_diaset, set_horset, set_minset, set_segset))
                        {
                            resultB = true;
                            result = zktGett(numMacn, true);
                        }
                        else
                        {
                            resultB = false;
                            result = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultB = false;
                }
                #endregion
            }
            else if (set_typset == 3)
            {
                #region aumento o disminucion de la hora tomando en cuanta la hora del reloj
                try
                {
                    if (cnxMacn)
                    {
                        if (czkFprd.GetDeviceTime(numMacn, ref set_añoset, ref set_messet, ref set_diaset, ref set_horset, ref set_minset, ref set_segset))
                        {
                            set_horset += set_incset;
                            DateTime setdatset = new DateTime(set_añoset, set_messet, set_diaset);
                            if (setdatset.Date != set_datset.Date)
                            {
                                if (czkFprd.SetDeviceTime(numMacn))
                                {
                                    czkFprd.GetDeviceTime(numMacn, ref set_añoset, ref set_messet, ref set_diaset, ref set_horset, ref set_minset, ref set_segset);
                                    set_horset += set_incset;
                                }
                            }
                            if (czkFprd.SetDeviceTime2(numMacn, set_añoset, set_messet, set_diaset, set_horset, set_minset, set_segset))
                            {
                                resultB = true;
                                result = zktGett(numMacn, true);
                            }
                            else
                            {
                                resultB = false;
                                result = string.Empty;
                            }
                        }
                        else
                        {
                            resultB = false;
                            result = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultB = false;
                }
                #endregion
            }
            //zktDisc();
            return result;
        }

        //Eliminar todos los datos de los empleados
        public bool zktClea(int numMacn, bool cnxMacn)
        {
            bool result = false;
            try
            {
                if (cnxMacn)
                {
                    czkFprd.EnableDevice(numMacn, false);
                    if (czkFprd.ClearData(numMacn, 5))
                    {
                        czkFprd.RefreshData(numMacn);
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                        result = true;
                    }
                    else
                    {
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                        result = false;
                    }
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = false;
            }

            return result;
        }

        //Eliminar huellas de empleados
        public List<ODL> zktDele(int numMacn, bool cnxMacn, int odt_typodt, List<ODL> lstOdl)
        {
            List<ODL> result = new List<ODL>();

            string fpr_plffpr = "";
            string strTemp = string.Empty;
            int fpr_fpafpr = 0;

            try
            {
                if (cnxMacn)
                {
                    czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                    czkFprd.GetSysOption(numMacn, "~ZKFPVersion", out strTemp);
                    fpr_fpafpr = Convert.ToInt32(strTemp);
                    string[] fpr_tipplf = fpr_plffpr.Split('_');

                    czkFprd.EnableDevice(numMacn, false);
                    #region borrado completo de todas las huellas con las funciones estandar
                    if (fpr_tipplf[0].Contains("ZMM") || fpr_plffpr.Contains("ZLM") || fpr_plffpr.Contains("JZ") || fpr_plffpr.Contains("ZEM"))
                    {
                        var grpOdls = (from tbl1 in lstOdl
                                       group tbl1 by new { tbl1.stf_numstf } into tmp
                                       select new { stf_numstf = tmp.Key.stf_numstf }).ToList();

                        foreach (var odl in grpOdls)
                        {
                            if (!string.IsNullOrEmpty(odl.stf_numstf) || !string.IsNullOrWhiteSpace(odl.stf_numstf))
                            {
                                if (czkFprd.SSR_DelUserTmpExt(numMacn, odl.stf_numstf, 13))
                                {
                                    result.AddRange(lstOdl.FindAll(x => x.stf_numstf == odl.stf_numstf).Select(y => { y.odl_staodl = 1; y.odl_msgodl = "Huella Eliminada"; return y; }).ToList());
                                    if (czkFprd.SSR_DeleteEnrollData(numMacn, odl.stf_numstf, 12))
                                    {
                                        result.AddRange(lstOdl.FindAll(x => x.stf_numstf == odl.stf_numstf).Select(y => { y.odl_staodl = 1; y.odl_msgodl = y.odl_msgodl + " y Usuario"; return y; }).ToList());
                                    }
                                    else
                                    {
                                        result.AddRange(lstOdl.FindAll(x => x.stf_numstf == odl.stf_numstf).Select(y => { y.odl_staodl = 2; y.odl_msgodl = "No se pudo eliminar el usuario"; return y; }).ToList());
                                    }
                                }
                                else
                                {
                                    result.AddRange(lstOdl.FindAll(x => x.stf_numstf == odl.stf_numstf).Select(y => { y.odl_staodl = 2; y.odl_msgodl = "No se pudo eliminar la huella"; return y; }).ToList());
                                }
                            }
                        }
                    }
                    #endregion
                    #region borrado completo de las huellas con funciones pull
                    else
                    {
                        string datRpl = "";
                        string setOptn = "";
                        bool rtrPrc = false;
                        string athLst = string.Empty;
                        string usrLst = string.Empty;
                        string tmpLst = string.Empty;

                        //data = $"Pin={ent.emp_numemp.ToString()}\tAuthorizeTimezoneId=1\tAuthorizeDoorId=1";
                        var lstAthr = lstOdl.Where(x => x.stf_numstf != "").GroupBy(x => new { emp_numemp = x.stf_numstf, pds_psspds = x.odl_pssodl }).Select(x => new { emp = $"Pin={x.Key.emp_numemp}" }).ToList();
                        athLst = string.Join("\r", lstAthr.Select(x => x.emp));

                        var lstUsrs = lstOdl.Where(x => x.stf_numstf != "").GroupBy(x => new { emp_numemp = x.stf_numstf, pds_psspds = x.odl_pssodl }).Select(x => new { emp = $"Pin={x.Key.emp_numemp}" }).ToList();
                        usrLst = string.Join("\r", lstUsrs.Select(x => x.emp));

                        //var lstTmpl = lstOtb.Select(x => new { emp = $"Size={x.pds_lenpds.ToString()}\tPin={x.stf_numstf.ToString()}\tFingerID={x.pds_finpds.ToString()}\tValid=1\tTemplate={x.pds_tmppds}\tResverd=0\tEndTag=0" }).ToList();
                        //var lstTmpl = lstOdl.Where(x => x.stf_numstf != "").Select(x => new { emp = $"Pin={x.stf_numstf}\tFingerID={x.pds_finpds.ToString()}" }).ToList();
                        var lstTmpl = lstOdl.Where(x => x.stf_numstf != "").GroupBy(x => new { emp_numemp = x.stf_numstf, pds_psspds = x.odl_pssodl }).Select(x => new { emp = $"Pin={x.Key.emp_numemp}" }).ToList();
                        tmpLst = string.Join("\r", lstTmpl.Select(x => x.emp));


                        string datTbl = fpr_fpafpr == 10 ? "templatev10" : "fptemplate09";
                        //string datTbl = "fptemplate09";
                        datRpl = tmpLst;
                        rtrPrc = czkFprd.SSR_DeleteDeviceData(numMacn, datTbl, datRpl, setOptn);
                        if (rtrPrc)
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "Huella Eliminada"; x.odl_staodl = 1; return x; }).ToList();
                            lstOdl = lst;
                        }
                        else
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "No se pudo eliminar la huella"; x.odl_staodl = 2; return x; }).ToList();
                            lstOdl = lst;
                        }

                        datRpl = usrLst;
                        rtrPrc = czkFprd.SSR_DeleteDeviceData(numMacn, "user", datRpl, setOptn);
                        if (rtrPrc)
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "Huella Eliminada"; x.odl_staodl = 1; return x; }).ToList();
                            lstOdl = lst;
                        }
                        else
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "No se pudo eliminar la huella"; x.odl_staodl = 2; return x; }).ToList();
                            lstOdl = lst;
                        }

                        datRpl = athLst;
                        rtrPrc = czkFprd.SSR_DeleteDeviceData(numMacn, "userauthorize", datRpl, setOptn);
                        if (rtrPrc)
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "No se pudo eliminar la huella"; x.odl_staodl = 2; return x; }).ToList();
                            lstOdl = lst;
                        }
                        else
                        {
                            var lst = lstOdl.Select(x => { x.odl_msgodl = "No se pudo eliminar la huella"; x.odl_staodl = 2; return x; }).ToList();
                            lstOdl = lst;
                        }

                        result = lstOdl;
                    }
                    #endregion

                    czkFprd.RefreshData(numMacn);
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    //result = lstOdl;
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = lstOdl;
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = lstOdl;
            }
            return result;
        }

        //Desacargar huellas de empleados
        public List<OBK> zktDown(int numMacn, bool cnxMacn, int fpr_keyfpr, int fpr_numfpr, int fpr_typfpr, int odt_keyodt)//bool
        {
            OBK obk;
            List<OBK> result = new List<OBK>();
            TimeSpan opr_tmsopr;
            DateTime opr_iniopr;
            DateTime opr_finopr;

            string strTemp = "";
            int fpr_fpafpr = 0;//algoritmo de huella
            int fpr_fcafpr = 0;//algoritmo de rostro

            try
            {
                if (cnxMacn)
                {
                    #region variables para el personal
                    int obk_priobk = 0;
                    int obk_eroobk = 0;
                    bool obk_enaobk = false;
                    bool obk_finger = false;
                    string obk_enrobk = string.Empty;
                    string obk_namobk = string.Empty;
                    string obk_pssobk = string.Empty;
                    string obk_crdobk = string.Empty;
                    #endregion
                    #region variables para las huellas
                    int obk_lenobk = 0;
                    int obk_flaobk = 1;
                    string obk_tmpobk = "";
                    #endregion

                    czkFprd.EnableDevice(numMacn, false);
                    try
                    {
                        if (czkFprd.BeginBatchUpdate(numMacn, obk_flaobk))
                        {
                            //czkFprd.BASE64 = 1;
                            if (czkFprd.ReadAllUserID(numMacn))
                            {
                                //czkFprd.BASE64 = 0;
                                obk_finger = czkFprd.ReadAllTemplate(numMacn);
                                opr_iniopr = DateTime.Now;
                                int opr_numftp = 0;
                                int obk_linobk = 0;
                                string fpr_plffpr = string.Empty;
                                czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                                string[] fpr_tipplf = fpr_plffpr.Split('_');
                                fpr_typfpr = fpr_tipplf.Count() > 1 ? 1 : 2;

                                czkFprd.GetSysOption(numMacn, "~ZKFPVersion", out strTemp);
                                fpr_fpafpr = Convert.ToInt32(strTemp);
                                czkFprd.GetSysOption(numMacn, "ZKFaceVersion", out strTemp);
                                fpr_fcafpr = Convert.ToInt32((string.IsNullOrEmpty(strTemp) || string.IsNullOrWhiteSpace(strTemp)) ? "0" : strTemp);

                                #region respaldo de huellas algortimo 9
                                if (fpr_typfpr == 1)
                                {
                                    while (czkFprd.SSR_GetAllUserInfo(numMacn, out obk_enrobk, out obk_namobk, out obk_pssobk, out obk_priobk, out obk_enaobk))
                                    {
                                        czkFprd.GetStrCardNumber(out obk_crdobk);
                                        opr_numftp = 0;
                                        if (obk_finger)
                                        {
                                            for (int i = 0; i <= 9; i++)
                                            {
                                                if (czkFprd.GetUserTmpExStr(numMacn, obk_enrobk, i, out obk_flaobk, out obk_tmpobk, out obk_lenobk))
                                                {
                                                    #region agregando huella a la base de datos
                                                    obk = new OBK();
                                                    obk.obk_keyobk = 0;
                                                    obk.obk_linobk = obk_linobk;
                                                    obk.obk_namobk = obk_namobk;
                                                    obk.obk_pssobk = obk_pssobk;
                                                    obk.obk_crdobk = obk_crdobk;
                                                    obk.obk_priobk = obk_priobk;
                                                    obk.obk_enaobk = obk_enaobk ? 1 : 0;
                                                    obk.obk_finobk = i;
                                                    obk.obk_tmpobk = obk_tmpobk;
                                                    obk.obk_lenobk = obk_lenobk;
                                                    obk.obk_datobk = DateTime.Now;
                                                    obk.obk_horobk = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                                    obk.obk_typobk = fpr_fpafpr == 0 ? fpr_fcafpr == 0 ? fpr_fpafpr : fpr_fcafpr : fpr_fpafpr;  //10; //tipo de algoritmo //9;
                                                    obk.obk_staobk = 1;
                                                    obk.obk_msgobk = "Huella respaldada";
                                                    obk.odt_keyodt = odt_keyodt;
                                                    obk.fpr_keyfpr = fpr_keyfpr;
                                                    obk.fpr_numfpr = fpr_numfpr;
                                                    obk.stf_keystf = 0;
                                                    obk.stf_numstf = obk_enrobk;
                                                    #endregion
                                                    result.Add(obk);
                                                    opr_numftp++;
                                                    obk_linobk++;
                                                }
                                            }
                                        }
                                        //empleado sin huella solo con la contraseña
                                        if (opr_numftp == 0)
                                        {
                                            #region agregando huella a la base de datos
                                            obk = new OBK();
                                            obk.obk_keyobk = 0;
                                            obk.obk_linobk = obk_linobk;
                                            obk.obk_namobk = obk_namobk;
                                            obk.obk_pssobk = obk_pssobk;
                                            obk.obk_crdobk = obk_crdobk;
                                            obk.obk_priobk = obk_priobk;
                                            obk.obk_enaobk = obk_enaobk ? 1 : 0;
                                            obk.obk_finobk = 0;
                                            obk.obk_tmpobk = string.Empty;
                                            obk.obk_lenobk = 0;
                                            obk.obk_datobk = DateTime.Now;
                                            obk.obk_horobk = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                            obk.obk_typobk = fpr_fpafpr == 0 ? fpr_fcafpr == 0 ? fpr_fpafpr : fpr_fcafpr : fpr_fpafpr;  //9;
                                            obk.obk_staobk = 1;
                                            obk.obk_msgobk = "Huella respaldada";
                                            obk.odt_keyodt = odt_keyodt;
                                            obk.fpr_keyfpr = fpr_keyfpr;
                                            obk.fpr_numfpr = fpr_numfpr;
                                            obk.stf_keystf = 0;
                                            obk.stf_numstf = obk_enrobk;
                                            #endregion
                                            result.Add(obk);
                                            obk_linobk++;
                                        }
                                        opr_finopr = DateTime.Now;
                                        opr_tmsopr = (TimeSpan)(opr_finopr - opr_iniopr);
                                        if (opr_tmsopr.TotalMinutes > 3.05)
                                        {
                                            czkFprd.EnableDevice(numMacn, true);
                                            zktDisc();
                                            result = new List<OBK>();
                                            break;
                                        }
                                    }
                                    czkFprd.EnableDevice(numMacn, true);
                                    zktDisc();
                                }
                                #endregion
                                #region respaldo de huellas algortimo 10
                                else
                                {//while (czkFP.GetAllUserInfo(numMac, ref enrFtn, ref namFtp, ref pssFtp, ref priFtp, ref enaFtp))
                                    while (czkFprd.GetAllUserInfo(numMacn, ref obk_eroobk, ref obk_namobk, ref obk_pssobk, ref obk_priobk, ref obk_enaobk))
                                    {
                                        opr_numftp = 0;
                                        for (int i = 0; i <= 9; i++)
                                        {
                                            if (czkFprd.GetUserTmpExStr(numMacn, obk_eroobk.ToString(), i, out obk_flaobk, out obk_tmpobk, out obk_lenobk))
                                            {
                                                #region agregando huella a la base de datos
                                                obk = new OBK();
                                                obk.obk_keyobk = 0;
                                                obk.obk_linobk = obk_linobk;
                                                obk.obk_namobk = obk_namobk;
                                                obk.obk_pssobk = obk_pssobk;
                                                obk.obk_crdobk = obk_crdobk;
                                                obk.obk_priobk = obk_priobk;
                                                obk.obk_enaobk = obk_enaobk ? 1 : 0;
                                                obk.obk_finobk = i;
                                                obk.obk_tmpobk = obk_tmpobk;
                                                obk.obk_lenobk = obk_lenobk;
                                                obk.obk_datobk = DateTime.Now;
                                                obk.obk_horobk = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                                obk.obk_typobk = fpr_fpafpr == 0 ? fpr_fcafpr == 0 ? fpr_fpafpr : fpr_fcafpr : fpr_fpafpr;  //10; //tipo de algoritmo
                                                obk.obk_staobk = 1;
                                                obk.obk_msgobk = "Huella respaldada";
                                                obk.odt_keyodt = odt_keyodt;
                                                obk.fpr_keyfpr = fpr_keyfpr;
                                                obk.fpr_numfpr = fpr_numfpr;
                                                obk.stf_keystf = 0;
                                                obk.stf_numstf = obk_eroobk.ToString();
                                                #endregion
                                                result.Add(obk);
                                                opr_numftp++;
                                                obk_linobk++;
                                            }
                                        }
                                        //empleado si huella solo con la contraseña
                                        if (opr_numftp == 0)
                                        {
                                            #region agregando huella a la base de datos
                                            obk = new OBK();
                                            obk.obk_keyobk = 0;
                                            obk.obk_linobk = obk_linobk;
                                            obk.obk_namobk = obk_namobk;
                                            obk.obk_pssobk = obk_pssobk;
                                            obk.obk_crdobk = obk_crdobk;
                                            obk.obk_priobk = obk_priobk;
                                            obk.obk_enaobk = obk_enaobk ? 1 : 0;
                                            obk.obk_finobk = obk_lenobk;
                                            obk.obk_tmpobk = string.Empty;
                                            obk.obk_lenobk = 0;
                                            obk.obk_datobk = DateTime.Now;
                                            obk.obk_horobk = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                            obk.obk_typobk = fpr_fpafpr == 0 ? fpr_fcafpr == 0 ? fpr_fpafpr : fpr_fcafpr : fpr_fpafpr;  //10; //tipo de algoritmo
                                            obk.obk_staobk = 1;
                                            obk.obk_msgobk = "Huella respaldada";
                                            obk.odt_keyodt = odt_keyodt;
                                            obk.fpr_keyfpr = fpr_keyfpr;
                                            obk.fpr_numfpr = fpr_numfpr;
                                            obk.stf_keystf = 0;
                                            obk.stf_numstf = obk_eroobk.ToString();
                                            #endregion
                                            result.Add(obk);
                                            obk_linobk++;
                                        }
                                        opr_finopr = DateTime.Now;
                                        opr_tmsopr = (TimeSpan)(opr_finopr - opr_iniopr);
                                        if (opr_tmsopr.TotalMinutes > 3.05)
                                        {
                                            czkFprd.EnableDevice(numMacn, true);
                                            zktDisc();
                                            //lstObk = new List<OBK>();
                                            break;
                                        }
                                    }
                                    czkFprd.EnableDevice(numMacn, true);
                                    zktDisc();
                                }
                                #endregion
                                //result = true;
                            }
                            else//funcion solo cuando se utiliza el pull
                            {
                                string getFltr = "";
                                string setOptn = "";
                                string dvcTbln = "templatev10";
                                string strTbls = "Size\tUID\tPin\tFingerID\tValid\tTemplate\tResverd\tEndTag";
                                string bffrOut = "";
                                int bffSize = 10 * 1024 * 1024;
                                bool rtnExt = czkFprd.SSR_GetDeviceData(numMacn, out bffrOut, bffSize, dvcTbln, strTbls, getFltr, setOptn);

                                if (rtnExt)
                                {
                                    string[] bffArry = bffrOut.Split('\n');
                                    int pds_linpds = 0;
                                    foreach (var arr in bffArry)
                                    {
                                        string[] rngArr = arr.Split(',');
                                        int rngNum;
                                        if (int.TryParse(rngArr[0], out rngNum))
                                        {
                                            obk = new OBK();
                                            obk.obk_keyobk = 0;
                                            obk.obk_linobk = pds_linpds;
                                            obk.obk_namobk = "";
                                            obk.obk_pssobk = "";
                                            obk.obk_crdobk = "";
                                            obk.obk_priobk = 0;
                                            obk.obk_enaobk = 1;
                                            obk.obk_finobk = Convert.ToInt32(rngArr[3]);
                                            obk.obk_tmpobk = rngArr[5];
                                            obk.obk_lenobk = Convert.ToInt32(rngArr[0]);
                                            obk.obk_datobk = DateTime.Now;
                                            obk.obk_horobk = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                            obk.obk_typobk = 10;
                                            obk.obk_staobk = 1;
                                            obk.obk_msgobk = "Huella respaldada";
                                            obk.odt_keyodt = odt_keyodt;
                                            obk.fpr_keyfpr = fpr_keyfpr;
                                            obk.fpr_numfpr = fpr_numfpr;
                                            obk.stf_keystf = 0;
                                            obk.stf_numstf = rngArr[2];
                                            result.Add(obk);
                                        }
                                        pds_linpds++;
                                    }

                                    try
                                    {
                                        setOptn = "";
                                        dvcTbln = "user";
                                        strTbls = "CardNo\tPin\tName\tPassword\tGroup\tStartTime\tEndTime";
                                        bffrOut = "";
                                        bffSize = 10 * 1024 * 1024;
                                        rtnExt = czkFprd.SSR_GetDeviceData(numMacn, out bffrOut, bffSize, dvcTbln, strTbls, getFltr, setOptn);

                                        if (rtnExt)
                                        {
                                            foreach (var arr in bffArry)
                                            {
                                                string[] rngArr = arr.Split(',');
                                                int rngNum;
                                                if (int.TryParse(rngArr[0], out rngNum))
                                                {
                                                    foreach (var mod in result.FindAll(x => x.stf_numstf == rngArr[1]).ToList())
                                                    {
                                                        mod.obk_namobk = rngArr[2];
                                                        mod.obk_pssobk = rngArr[3];
                                                        mod.obk_crdobk = rngArr[0];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                czkFprd.EnableDevice(numMacn, true);
                                zktDisc();
                            }
                        }
                        else
                        {
                            czkFprd.EnableDevice(numMacn, true);
                            zktDisc();
                            result = new List<OBK>();
                        }
                    }
                    catch (Exception ex)
                    {
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                        result = new List<OBK>();
                    }
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = new List<OBK>();
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = new List<OBK>();
            }
            return result; //result;
        }

        //Extraer los marcajes de los empleados
        public List<OCH> zktChec(int numMacn, bool cnxMacn, int fpr_keyfpr, int fpr_numfpr, int fpr_typfpr, int och_typoch, int odt_keyodt, bool segundos)
        {
            List<OCH> result = new List<OCH>();
            string fpr_plffpr = "";
            string strTemp = string.Empty;
            int fpr_fpafpr = 0;

            #region variables de registros de los marcajes
            int och_valoch = 0;// saber si hay movimientos en el lector 
            int och_veroch = 0;
            int och_outoch = 0;
            int och_añooch = 0;
            int och_mesoch = 0;
            int och_diaoch = 0;
            int och_hraoch = 0;
            int och_minoch = 0;
            int och_segoch = 0;
            int och_codoch = 0;
            int och_numstf = 0;
            int och_refoch = 0;

            string stf_numstf = string.Empty;
            #endregion
            #region variables del proces
            DateTime och_dtpoch;
            TimeSpan och_hrpoch;
            #endregion
            try
            {
                if (cnxMacn)
                {
                    czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                    czkFprd.GetSysOption(numMacn, "~ZKFPVersion", out strTemp);
                    fpr_fpafpr = Convert.ToInt32(strTemp);

                    czkFprd.GetDeviceStatus(numMacn, 6, ref och_valoch);
                    #region validación si existen movimientos a extraer
                    if (och_valoch > 0)
                    {
                        if (!czkFprd.ReadGeneralLogData(numMacn))
                        {
                            int errCode = 0;
                            czkFprd.GetLastError(ref errCode);
                            Console.WriteLine("Error al leer checadas. Código: " + errCode);
                        }

                        if (czkFprd.ReadGeneralLogData(numMacn))
                        {
                            fpr_plffpr = string.Empty;
                            czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                            string[] fpr_tipplf = fpr_plffpr.Split('_');
                            fpr_typfpr = fpr_tipplf.Count() > 1 ? 1 : 2;
                            #region lectura de marcajes algoritmo 9
                            if (fpr_typfpr == 1)
                            {
                                #region extracción de marcajes
                                while (czkFprd.SSR_GetGeneralLogData(numMacn, out stf_numstf, out och_veroch, out och_outoch, out och_añooch, out och_mesoch, out och_diaoch, out och_hraoch, out och_minoch, out och_segoch, ref och_codoch))
                                {
                                    if (int.TryParse(stf_numstf, out och_numstf))
                                    {
                                        och_dtpoch = DateTime.Now;
                                        och_hrpoch = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                        #region llenado de marcajes
                                        OCH och = new OCH();
                                        och.och_keyoch = 0;
                                        och.stf_numstf = stf_numstf;
                                        och.och_datoch = Convert.ToDateTime(och_añooch + "/" + och_mesoch.ToString("00") + "/" + och_diaoch.ToString("00"));
                                        if (segundos)
                                        {
                                            och.och_horoch = new TimeSpan(och_hraoch, och_minoch, och_segoch);
                                        }
                                        else
                                        {
                                            och.och_horoch = new TimeSpan(och_hraoch, och_minoch, 0);
                                        }
                                        och.och_dtpoch = och_dtpoch;
                                        och.och_hrpoch = och_hrpoch;
                                        och.och_stzoch = och_outoch.ToString();
                                        och.och_staoch = 0;
                                        och.och_fstoch = 0;
                                        och.och_typoch = och_typoch;//OnDemand o Automatico
                                        och.fpr_keyfpr = fpr_keyfpr;
                                        och.fpr_numfpr = fpr_numfpr;
                                        och.odt_keyodt = odt_keyodt;
                                        result.Add(och);
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            #endregion
                            #region lectura de marcajes algoritmo 10
                            else
                            {
                                #region extracción de marcajes
                                while (czkFprd.GetGeneralExtLogData(numMacn, ref och_numstf, ref och_veroch, ref och_outoch, ref och_añooch, ref och_mesoch, ref och_diaoch, ref och_hraoch, ref och_minoch, ref och_segoch, ref och_codoch, ref och_refoch))
                                {
                                    och_dtpoch = DateTime.Now;
                                    och_hrpoch = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                    #region llenado de marcajes
                                    OCH och = new OCH();
                                    och.och_keyoch = 0;
                                    och.stf_numstf = och_numstf.ToString();
                                    och.och_datoch = Convert.ToDateTime(och_añooch + "/" + och_mesoch.ToString("00") + "/" + och_diaoch.ToString("00"));
                                    if (segundos)
                                    {
                                        och.och_horoch = new TimeSpan(och_hraoch, och_minoch, och_segoch);
                                    }
                                    else
                                    {
                                        och.och_horoch = new TimeSpan(och_hraoch, och_minoch, 0);
                                    }
                                    och.och_dtpoch = och_dtpoch;
                                    och.och_hrpoch = och_hrpoch;
                                    och.och_stzoch = och_outoch.ToString();
                                    och.och_staoch = 0;
                                    och.och_fstoch = 0;
                                    och.och_typoch = och_typoch;//OnDemand o Automatico
                                    och.fpr_keyfpr = fpr_keyfpr;
                                    och.fpr_numfpr = fpr_numfpr;
                                    och.odt_keyodt = odt_keyodt;
                                    result.Add(och);
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else//funcion cuando se utiliza el pull
                        {
                            string getFltr = "";
                            string setOptn = "";
                            string dvcTbln = "transaction";
                            string strTbls = "Cardno\tPin\tVerified\tDoorID\tEventType\tInOutState\tTime_second";
                            string bffrOut = "";
                            int bffSize = 10 * 1024 * 1024;
                            bool rtnExt = czkFprd.SSR_GetDeviceData(numMacn, out bffrOut, bffSize, dvcTbln, strTbls, getFltr, setOptn);

                            try
                            {
                                if (rtnExt)
                                {
                                    string[] bffArry = bffrOut.Split('\n');
                                    foreach (var arr in bffArry)
                                    {
                                        string[] rngArr = arr.Split(',');
                                        int rngNum;
                                        if (int.TryParse(rngArr[0], out rngNum))
                                        {
                                            int datTim = Convert.ToInt32(rngArr[6]);
                                            int timScd = datTim;
                                            int datScn = timScd % 60; timScd /= 60;
                                            int datMin = timScd % 60; timScd /= 60;
                                            int datHor = timScd % 24; timScd /= 24;
                                            int datDay = timScd % 31 + 1; timScd /= 31;
                                            int datMnt = timScd % 12 + 1; timScd /= 12;
                                            int datYer = timScd + 2000;
                                            OCH och = new OCH();
                                            och.och_keyoch = 0;
                                            och.stf_numstf = rngArr[1];
                                            och.och_datoch = new DateTime(datYer, datMnt, datDay);
                                            och.och_horoch = new TimeSpan(datHor, datMin, 0);
                                            och.och_dtpoch = DateTime.Now;
                                            och.och_hrpoch = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                            och.och_stzoch = rngArr[1];
                                            och.och_staoch = 0;
                                            och.och_fstoch = 0;
                                            och.och_typoch = 0;//OnDemand o Automatico
                                            och.fpr_keyfpr = fpr_keyfpr;
                                            och.fpr_numfpr = fpr_numfpr;
                                            och.odt_keyodt = odt_keyodt;
                                            result.Add(och);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                    }
                    #endregion
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
            }
            return result;
        }

        //Replicar huellas de empleados
        public List<OTB> zktRepl(int numMacn, bool cnxMacn, int fpr_typfpr, List<OTB> lstOtb, int odt_typodt)//, int odt_typodt, List<OBK> lstObk
        {
            List<OTB> result = new List<OTB>();
            string fpr_plffpr = "";
            string strTemp = string.Empty;
            string fpr_sdkfpr = string.Empty;
            string fpr_thrfpr = string.Empty;
            int fpr_fpafpr = 0;

            try
            {
                if (cnxMacn)
                {
                    czkFprd.GetPlatform(numMacn, ref fpr_plffpr);
                    czkFprd.GetSysOption(numMacn, "~ZKFPVersion", out strTemp);
                    czkFprd.GetSDKVersion(ref fpr_sdkfpr);
                    czkFprd.GetDeviceStrInfo(numMacn, 1, out fpr_thrfpr);

                    fpr_fpafpr = Convert.ToInt32(strTemp);
                    string[] fpr_tipplf = fpr_plffpr.Split('_');
                    fpr_typfpr = fpr_tipplf.Count() > 1 ? 1 : 2;
                    #region variables para las huellas
                    int otb_flaotb = 1;
                    #endregion
                    czkFprd.EnableDevice(numMacn, false);
                    #region replica de huellas con funciones estandar
                    if (fpr_plffpr.Contains("ZMM") || fpr_plffpr.Contains("ZLM") || fpr_plffpr.Contains("JZ") || fpr_plffpr.Contains("ZEM"))//activar si la version utiliza las funciones pull//fpr_tipplf
                    {//activar si la version utiliza las funciones pull
                        if (czkFprd.BeginBatchUpdate(numMacn, otb_flaotb))
                        {
                            lstOtb = lstOtb.OrderBy(x => x.stf_numstf).ToList();
                            foreach (OTB otb in lstOtb)
                            {
                                if (czkFprd.SSR_SetUserInfo(numMacn, otb.stf_numstf, otb.otb_namotb, otb.otb_pssotb, otb.otb_priotb, otb.otb_enaotb == 1 ? true : false))
                                {
                                    if (!string.IsNullOrEmpty(otb.otb_tmpotb) || !string.IsNullOrWhiteSpace(otb.otb_tmpotb))
                                    {
                                        if (!string.IsNullOrEmpty(otb.otb_pssotb) || !string.IsNullOrWhiteSpace(otb.otb_pssotb))
                                        {
                                            czkFprd.SetStrCardNumber(otb.otb_crdotb);
                                            czkFprd.SSR_SetUserInfo(numMacn, otb.stf_numstf, otb.otb_namotb, otb.otb_pssotb, 0, true);
                                            czkFprd.SetUserTmpExStr(numMacn, otb.stf_numstf, otb.otb_finotb, otb_flaotb, otb.otb_tmpotb);
                                        }
                                        else
                                            czkFprd.SetUserTmpExStr(numMacn, otb.stf_numstf, otb.otb_finotb, otb_flaotb, otb.otb_tmpotb);
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(otb.otb_pssotb) || !string.IsNullOrWhiteSpace(otb.otb_pssotb))
                                        {
                                            czkFprd.SetStrCardNumber(otb.otb_crdotb);
                                            czkFprd.SSR_SetUserInfo(numMacn, otb.stf_numstf, otb.otb_namotb, otb.otb_pssotb, 0, true);
                                        }
                                    }
                                    otb.otb_staotb = 1;
                                    otb.otb_msgotb = "Huella Replicada";
                                }
                                else
                                {
                                    otb.otb_staotb = 2;
                                    otb.otb_msgotb = "No se pudo replicar la huella";
                                }
                                result.Add(otb);
                            }
                        }
                    }//activar si la version utiliza las funciones pull
                    #endregion
                    //activar si la version utiliza las funciones pull
                    #region replica de huellas con funciones pull
                    else
                    {
                        if (czkFprd.BeginBatchUpdate(numMacn, otb_flaotb))
                        {
                            string datRpl = "";
                            string setOptn = "";
                            bool rtrPrc = false;
                            string athLst = string.Empty;
                            string usrLst = string.Empty;
                            string tmpLst = string.Empty;

                            var lstAthr = lstOtb.GroupBy(x => x.stf_numstf).Select(x => new { emp = "Pin=" + x.Key }).ToList();
                            athLst = string.Join("\tAuthorizeTimezoneId=1\tAuthorizeDoorId=1\r", lstAthr.Select(x => x.emp)) + "\tAuthorizeTimezoneId=1\tAuthorizeDoorId=1";

                            var lstUsrs = lstOtb.GroupBy(x => new { emp_numemp = x.stf_numstf, pds_crdpds = x.otb_crdotb, pds_psspds = x.otb_pssotb }).
                                Select(x => new { emp = $"Pin={x.Key.emp_numemp}\tCardNo={x.Key.pds_crdpds}\tPassword={x.Key.pds_psspds}" }).ToList();
                            usrLst = string.Join("\r", lstUsrs.Select(x => x.emp));

                            var lstTmpl = lstOtb.Select(x => new { emp = $"Size={x.otb_lenotb.ToString()}\tPin={x.stf_numstf.ToString()}\tFingerID={x.otb_finotb.ToString()}\tValid=1\tTemplate={x.otb_tmpotb}\tResverd=0\tEndTag=0" }).ToList();
                            tmpLst = string.Join("\r", lstTmpl.Select(x => x.emp));

                            datRpl = athLst;

                            rtrPrc = czkFprd.SSR_SetDeviceData(numMacn, "userauthorize", datRpl, setOptn);
                            if (rtrPrc)
                            {
                                datRpl = usrLst;
                                rtrPrc = czkFprd.SSR_SetDeviceData(numMacn, "user", datRpl, setOptn);
                                if (rtrPrc)
                                {
                                    datRpl = tmpLst;
                                    rtrPrc = czkFprd.SSR_SetDeviceData(numMacn, "templatev10", datRpl, setOptn);
                                    if (rtrPrc)
                                    {
                                        var lst = lstOtb.Select(x => { x.otb_msgotb = "Huella Replicada"; x.otb_staotb = 1; return x; }).ToList();
                                        lstOtb = lst;
                                    }
                                    else
                                    {
                                        var lst = lstOtb.Select(x => { x.otb_msgotb = "No se pudo replicar la huella"; x.otb_staotb = 2; return x; }).ToList();
                                        lstOtb = lst;
                                    }
                                }
                                else
                                {
                                    var lst = lstOtb.Select(x => { x.otb_msgotb = "No se pudo replicar la huella"; x.otb_staotb = 2; return x; }).ToList();
                                    lstOtb = lst;
                                }
                            }
                            else
                            {
                                var lst = lstOtb.Select(x => { x.otb_msgotb = "No se pudo replicar la huella"; x.otb_staotb = 2; return x; }).ToList();
                                lstOtb = lst;
                            }
                            result = lstOtb;
                        }
                    }
                    #endregion
                    czkFprd.BatchUpdate(numMacn);
                    czkFprd.RefreshData(numMacn);
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = lstOtb;
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = lstOtb;
            }
            return result;
        }

        //Lector de tarjetas rfid
        public string zkCard(int numMacn, bool cnxMacn, string stf_numstf, string stf_nckstf, string stf_crdstf, string stf_pwdstf)
        {
            string result = string.Empty;
            try
            {
                if (cnxMacn)
                {
                    //
                    int iPIN2Width = 0;
                    int iIsABCPinEnable = 0;
                    int iT9FunOn = 0;
                    string strTemp = "";
                    czkFprd.GetSysOption(numMacn, "~PIN2Width", out strTemp);
                    iPIN2Width = Convert.ToInt32(strTemp);
                    czkFprd.GetSysOption(numMacn, "~IsABCPinEnable", out strTemp);
                    iIsABCPinEnable = Convert.ToInt32(strTemp);
                    czkFprd.GetSysOption(numMacn, "~T9FunOn", out strTemp);
                    iT9FunOn = Convert.ToInt32(strTemp);
                    //
                    if (stf_numstf.Length > iPIN2Width)
                    {
                        return "-1022";
                    }

                    if (iIsABCPinEnable == 0 || iT9FunOn == 0)
                    {
                        if (stf_numstf.ToString().Substring(0, 1) == "0")
                        {
                            return "-1023";
                        }

                        foreach (char tempchar in stf_numstf.ToCharArray())
                        {
                            if (!(char.IsDigit(tempchar)))
                            {
                                return "-1024";
                            }
                        }
                    }
                    //
                    czkFprd.EnableDevice(1, true);
                    czkFprd.SetStrCardNumber(stf_crdstf);
                    czkFprd.SSR_SetUserInfo(1, stf_numstf, stf_nckstf, stf_pwdstf, 0, true);
                    czkFprd.RefreshData(1);
                    czkFprd.EnableDevice(1, true);
                    zktDisc();
                    result = "Ok";
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                zktDisc();
                result = string.Empty;
            }
            return result;
        }

        //Reiniciar el lector
        public bool zktRset(int numMacn, bool cnxMacn)
        {
            bool result = false;
            try
            {
                if (cnxMacn)
                {
                    try
                    {
                        result = czkFprd.RestartDevice(numMacn);
                        result = result ? result : true;
                    }
                    catch (Exception ex)
                    {
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                        result = false;
                    }
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = false;
            }
            return result;
        }

        //Cambio de algoritmo del lector
        public bool zktChan(int numMacn, bool cnxMacn, int fpr_fpafpr)
        {
            bool result = false;
            try
            {
                if (cnxMacn)
                {
                    try
                    {
                        czkFprd.EnableDevice(numMacn, false);
                        result = czkFprd.SetSysOption(numMacn, "~ZKFPVersion", fpr_fpafpr.ToString());
                        czkFprd.EnableDevice(numMacn, true);
                        result = result ? result : true;
                    }
                    catch (Exception ex)
                    {
                        czkFprd.EnableDevice(numMacn, true);
                        zktDisc();
                        result = false;
                    }
                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                czkFprd.EnableDevice(numMacn, true);
                zktDisc();
                result = false;
            }
            return false;
        }

        //Borrado de checadas en el lecor
        public bool zktDatt(int numMacn, bool cnxMacn, DateTime fpr_dtbfpr, DateTime fpr_dtffpr) //yyyy-MM-dd HH:mm:ss, formato de fechas
        {
            bool result = false;
            try
            {
                if (cnxMacn)
                {
                    czkFprd.EnableDevice(numMacn, false);
                    //.DeleteAttlogByTime(numMacn, fpr_dtffpr)//hasta la fecha
                    var fpr_dtbfpr1 = fpr_dtbfpr.ToString("yyyy-MM-dd HH:mm:ss");
                    var fpr_dtffpr1 = fpr_dtffpr.ToString("yyyy-MM-dd HH:mm:ss");

                    if (czkFprd.DeleteAttlogBetweenTheDate(numMacn, fpr_dtbfpr1, fpr_dtffpr1))//.DeleteAttlogBetweenTheDate(numMacn, fpr_dtbfpr, fpr_dtffpr)// rango de fechas
                    {
                        czkFprd.RefreshData(numMacn);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();

                }
                else
                {
                    czkFprd.EnableDevice(numMacn, true);
                    zktDisc();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                zktDisc();
                result = false;
            }
            return result;
        }
    }
}
