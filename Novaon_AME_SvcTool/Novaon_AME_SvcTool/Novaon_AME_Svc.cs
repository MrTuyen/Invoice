using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using Novaon_AME_SvcTool.Common;
using Novaon_AME_SvcTool.DAO;
using System.Globalization;
using System.Threading;

namespace Novaon_AME_SvcTool
{
    partial class Novaon_AME_Svc : ServiceBase
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private int setIntervalTime = ConfigHelper.setIntervalTime;
        private string LogPath = ConfigHelper.LogPath;
        private string CodPathDMKH = ConfigHelper.CodPath + "dmkh.dbf";
        private string CodPathDMVT = ConfigHelper.CodPath + "dmvt.dbf";

        public Novaon_AME_Svc()
        {
            InitializeComponent();
        }

        public void onDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                timer.Elapsed += new ElapsedEventHandler(WorkerProcess);
                timer.Start();
            }
            catch (Exception objMainEx)
            {
                WriteLog("OnStart", objMainEx.ToString());
            }
        }

        private void WorkerProcess(object sender, EventArgs e)
        {
            timer.Interval = setIntervalTime * 1000 * 60;  // Khoảng thời gian giữa các tiến trình
            timer.Stop(); // Stop timer => thực hiện công việc => Start trở lại
            GetInvoice();
            timer.Start();
        }

        private void GetInvoice()
        {
            try
            {
                ServiceResponse result = new ServiceResponse();
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                var listInvoicesData = new List<InvoiceBO>();
                var listInvoices_PH81 = new List<InvoiceBO>();
                var listInvoices_PH21 = new List<InvoiceBO>();
                var listInvoices_PH86 = new List<InvoiceBO>();
                var listInvoicesCacheFile = new List<InvoiceBO>();
                var listInvoices = new List<InvoiceBO>();
                var requestedTime = DateTime.Now;
                if (ConfigHelper.SetDateTime == "Y")
                {
                    requestedTime = Convert.ToDateTime(ConfigHelper.SetDateTimeValue.Replace('^', ' ').Replace('{', ' ').Replace('}', ' '));
                }

                var requestedTimeNow = requestedTime.ToString("yyyy-MM-dd");
                var requestedTimeFormat = requestedTime.ToString("yyyy-MM-dd 23:59:59");
                var requestedTimeFormat1 = requestedTime.ToString("yyyy-MM-dd 00:00:00");

                var tempRequestedTimeNow = "{^" + requestedTimeNow + "}";
                var tempRequestedTime = "{^" + requestedTimeFormat + "}";
                var tempRequestedTime1 = "{^" + requestedTimeFormat1 + "}";

                //var tempRequestedTimeNow = "{^2020-11-05}";
                //var tempRequestedTime = "{^2020-11-05 23:59:59}";
                //var tempRequestedTime1 = "{^2020-11-05 00:00:00}";

                string sqlGetInvoies_PH81 = String.Format(@"SELECT PH81.Stt_rec AS AME_INVOICEID,
                CTOT(TRANSFORM(PH81.Date0) + ' ' + TRANSFORM(PH81.Time0)) AS AME_DATETIME0,
                CTOT(TRANSFORM(PH81.Date) + ' ' + PH81.Time) AS AME_DATETIME,
                PH81.So_ct AS INVOICECODE,
                PH81.Ong_ba AS CUSBUYER,
                DMKH.Ten_kh AS CUSNAME,
                PH81.Ma_so_thue AS CUSTAXCODE,
                PH81.Dia_chi AS CUSADDRESS,
                PH81.Ma_kh AS CUSTOMERCODE,
                PH81.Ht_tt AS CUSPAYMENTMETHOD,
                PH81.Tk_nh AS CUSACCOUNTNUMBER,
                PH81.Ten_nh AS CUSBANKNAME,
                DMKH.E_mail AS CUSEMAIL,
                PH81.Ngay_ct AS INITTIME,
                PH81.Ma_thue AS MATHUE,
                PH81.Thue_suat AS TAXRATE1,
                DMVT.Ten_vt AS PRODUCTNAME,
                CT81.Stt_rec0 AS STT,
                CT81.So_luong AS QUANTITY,
                DMVT.Dvt AS QUANTITYUNIT,
                CT81.Gia2 AS RETAILPRICE,
                CT81.Thue AS TOTALTAX,
                CT81.Tien2 AS TOTALMONEY,
                (CT81.Tien2 + CT81.Thue) AS TOTALPAYMENT 
                FROM PH81 
                JOIN CT81 ON PH81.Stt_rec = CT81.Stt_rec 
                JOIN ('{0}') DMVT ON DMVT.Ma_vt = CT81.Ma_vt
                JOIN ('{1}') DMKH ON DMKH.Ma_kh = PH81.Ma_kh
                WHERE ( CTOT(TRANSFORM(PH81.Date) + ' ' + PH81.Time) BETWEEN {2} and {3}) OR (PH81.Ngay_ct = {4})", CodPathDMVT, CodPathDMKH, tempRequestedTime1, tempRequestedTime, tempRequestedTimeNow);
                listInvoices_PH81 = invoiceDAO.GetInvoices(sqlGetInvoies_PH81);
                listInvoices.AddRange(listInvoices_PH81);

                string sqlGetInvoies_PH21 = String.Format(@"SELECT PH21.Stt_rec AS AME_INVOICEID,
                CTOT(TRANSFORM(PH21.Date0) + ' ' + PH21.Time0) AS AME_DATETIME0,
                CTOT(TRANSFORM(PH21.Date) + ' ' + PH21.Time) AS AME_DATETIME,
                PH21.So_ct AS INVOICECODE,
                PH21.Ong_ba AS CUSBUYER,
                DMKH.Ten_kh AS CUSNAME,
                PH21.Ma_so_thue AS CUSTAXCODE,
                PH21.Dia_chi AS CUSADDRESS,
                PH21.Ma_kh AS CUSTOMERCODE,
                PH21.Ht_tt AS CUSPAYMENTMETHOD,
                PH21.Tk_nh AS CUSACCOUNTNUMBER,
                PH21.Ten_nh AS CUSBANKNAME,
                PH21.Ngay_ct AS INITTIME,
                DMKH.E_mail AS CUSEMAIL,
                CT21.Ma_thue_i AS MATHUE,
                CT21.Thue_suati AS TAXRATE1,
                0 AS QUANTITY,
                CT21.Dien_giaii AS PRODUCTNAME,
                'Khác' AS QUANTITYUNIT,
                0 AS RETAILPRICE,
                CT21.Stt_rec0 AS STT,
                CT21.Thue AS TOTALTAX,
                CT21.Tien2 AS TOTALMONEY,
                (CT21.Tien2 + CT21.Thue) AS TOTALPAYMENT 
                FROM PH21 
                JOIN CT21 ON PH21.Stt_rec = CT21.Stt_rec 
                JOIN ('{0}') DMKH ON DMKH.Ma_kh = PH21.Ma_kh
                WHERE (CTOT(TRANSFORM(PH21.Date) + ' ' + PH21.Time) BETWEEN {1} and {2}) OR (PH21.Ngay_ct = {3})", CodPathDMKH, tempRequestedTime1, tempRequestedTime, tempRequestedTimeNow);
                listInvoices_PH21 = invoiceDAO.GetInvoices(sqlGetInvoies_PH21);
                listInvoices.AddRange(listInvoices_PH21);

                string sqlGetInvoies_PH86 = String.Format(@"SELECT PH86.Stt_rec AS AME_INVOICEID,
                CTOT(TRANSFORM(PH86.Date0) + ' ' + PH86.Time0) AS AME_DATETIME0,
                CTOT(TRANSFORM(PH86.Date) + ' ' + PH86.Time) AS AME_DATETIME,
                PH86.So_ct AS INVOICECODE,
                PH86.Ong_ba AS CUSBUYER,
                DMKH.Ten_kh AS CUSNAME,
                PH86.Ma_so_thue AS CUSTAXCODE,
                PH86.Dia_chi AS CUSADDRESS,
                PH86.Ma_kh AS CUSTOMERCODE,
                PH86.Ngay_ct AS INITTIME,
                DMKH.E_mail AS CUSEMAIL,
                PH86.Ma_thue AS MATHUE,
                PH86.Thue_suat AS TAXRATE1,
                DMVT.Ten_vt AS PRODUCTNAME,
                CT86.Stt_rec0 AS STT,
                CT86.So_luong AS QUANTITY,
                DMVT.Dvt AS QUANTITYUNIT,
                CT86.Gia AS RETAILPRICE,
                CT86.Thue AS TOTALTAX,
                CT86.Tien AS TOTALMONEY,
                (CT86.Tien + CT86.Thue) AS TOTALPAYMENT
                FROM PH86 
                JOIN CT86 ON PH86.Stt_rec = CT86.Stt_rec 
                JOIN ('{0}') DMVT ON DMVT.Ma_vt = CT86.Ma_vt 
                JOIN ('{1}') DMKH ON DMKH.Ma_kh = PH86.Ma_kh
                WHERE (CTOT(TRANSFORM(PH86.Date) + ' ' + PH86.Time) BETWEEN {2} and {3}) OR (PH86.Ngay_ct = {4})", CodPathDMVT, CodPathDMKH, tempRequestedTime1, tempRequestedTime, tempRequestedTimeNow);
                listInvoices_PH86 = invoiceDAO.GetInvoices(sqlGetInvoies_PH86);
                listInvoices.AddRange(listInvoices_PH86);

                //Get invoices from cache file
                var cacheFileData = ReadCacheFile(); // Read cache file. Check if exist data then get invoices from database and push into service
                if (!string.IsNullOrEmpty(cacheFileData))
                {
                    //string sqlGetInvoicesFromCacheFile_PH81 = String.Format(@"SELECT PH81.Stt_rec AS AME_INVOICEID,
                    //CTOT(TRANSFORM(PH81.Date0) + ' ' + PH81.Time0) AS AME_DATETIME0,
                    //CTOT(TRANSFORM(PH81.Date) + ' ' + PH81.Time) AS AME_DATETIME,
                    //PH81.So_ct AS INVOICECODE,
                    //PH81.Ong_ba AS CUSBUYER,
                    //DMKH.Ten_kh AS CUSNAME,
                    //PH81.Ma_so_thue AS CUSTAXCODE,
                    //PH81.Dia_chi AS CUSADDRESS,
                    //PH81.Ma_kh AS CUSTOMERCODE,
                    //PH81.Ht_tt AS CUSPAYMENTMETHOD,
                    //PH81.Tk_nh AS CUSACCOUNTNUMBER,
                    //PH81.Ten_nh AS CUSBANKNAME,
                    //DMKH.E_mail AS CUSEMAIL,
                    //PH81.Ngay_ct AS INITTIME,
                    //PH81.Ma_thue AS MATHUE,
                    //PH81.Thue_suat AS TAXRATE1,
                    //DMVT.Ten_vt AS PRODUCTNAME,
                    //CT81.So_luong AS QUANTITY,
                    //DMVT.Dvt AS QUANTITYUNIT,
                    //CT81.Gia2 AS RETAILPRICE,
                    //CT81.Thue AS TOTALTAX,
                    //CT81.Tien2 AS TOTALMONEY,
                    //(CT81.Tien2 + CT81.Thue) AS TOTALPAYMENT 
                    //FROM PH81 
                    //JOIN CT81 ON PH81.Stt_rec = CT81.Stt_rec 
                    //JOIN ('{0}') DMVT ON DMVT.Ma_vt = CT81.Ma_vt
                    //JOIN ('{1}') DMKH ON DMKH.Ma_kh = PH81.Ma_kh
                    //WHERE PH81.Stt_rec $ '" + cacheFileData + "'", CodPathDMVT, CodPathDMKH);
                    //listInvoicesCacheFile.AddRange(invoiceDAO.GetInvoices(sqlGetInvoicesFromCacheFile_PH81));

                    var listIDs = cacheFileData.Split(',');
                    var tmpList = new List<string>();
                    foreach (var item in listIDs)
                    {
                        var i = $"'{item}'";
                        tmpList.Add(i);
                    }
                    var ids = string.Join(",", tmpList);

                    string sqlGetInvoicesFromCacheFile_PH81 = String.Format(@"SELECT PH81.Stt_rec AS AME_INVOICEID,
                    CTOT(TRANSFORM(PH81.Date0) + ' ' + PH81.Time0) AS AME_DATETIME0,
                    CTOT(TRANSFORM(PH81.Date) + ' ' + PH81.Time) AS AME_DATETIME,
                    PH81.So_ct AS INVOICECODE,
                    PH81.Ong_ba AS CUSBUYER,
                    DMKH.Ten_kh AS CUSNAME,
                    PH81.Ma_so_thue AS CUSTAXCODE,
                    PH81.Dia_chi AS CUSADDRESS,
                    PH81.Ma_kh AS CUSTOMERCODE,
                    PH81.Ht_tt AS CUSPAYMENTMETHOD,
                    PH81.Tk_nh AS CUSACCOUNTNUMBER,
                    PH81.Ten_nh AS CUSBANKNAME,
                    DMKH.E_mail AS CUSEMAIL,
                    PH81.Ngay_ct AS INITTIME,
                    PH81.Ma_thue AS MATHUE,
                    PH81.Thue_suat AS TAXRATE1,
                    DMVT.Ten_vt AS PRODUCTNAME,
                    CT81.So_luong AS QUANTITY,
                    DMVT.Dvt AS QUANTITYUNIT,
                    CT81.Gia2 AS RETAILPRICE,
                    CT81.Thue AS TOTALTAX,
                    CT81.Tien2 AS TOTALMONEY,
                    (CT81.Tien2 + CT81.Thue) AS TOTALPAYMENT 
                    FROM PH81 
                    JOIN CT81 ON PH81.Stt_rec = CT81.Stt_rec 
                    JOIN ('{0}') DMVT ON DMVT.Ma_vt = CT81.Ma_vt
                    JOIN ('{1}') DMKH ON DMKH.Ma_kh = PH81.Ma_kh
                    WHERE PH81.Stt_rec IN (" + ids + ")", CodPathDMVT, CodPathDMKH);
                    listInvoicesCacheFile.AddRange(invoiceDAO.GetInvoices(sqlGetInvoicesFromCacheFile_PH81));

                    string sqlGetInvoicesFromCacheFile_PH21 = String.Format(@"SELECT PH21.Stt_rec AS AME_INVOICEID,
                    CTOT(TRANSFORM(PH21.Date0) + ' ' + PH21.Time0) AS AME_DATETIME0,
                    CTOT(TRANSFORM(PH21.Date) + ' ' + PH21.Time) AS AME_DATETIME,
                    PH21.So_ct AS INVOICECODE,
                    PH21.Ong_ba AS CUSBUYER,
                    DMKH.Ten_kh AS CUSNAME,
                    PH21.Ma_so_thue AS CUSTAXCODE,
                    PH21.Dia_chi AS CUSADDRESS,
                    PH21.Ma_kh AS CUSTOMERCODE,
                    PH21.Ht_tt AS CUSPAYMENTMETHOD,
                    PH21.Tk_nh AS CUSACCOUNTNUMBER,
                    PH21.Ten_nh AS CUSBANKNAME,
                    PH21.Ngay_ct AS INITTIME,
                    DMKH.E_mail AS CUSEMAIL,
                    CT21.Ma_thue_i AS MATHUE,
                    CT21.Thue_suati AS TAXRATE1,
                    0 AS QUANTITY,
                    CT21.Dien_giaii AS PRODUCTNAME,
                    'Khác' AS QUANTITYUNIT,
                    0 AS RETAILPRICE,
                    CT21.Thue AS TOTALTAX,
                    CT21.Tien2 AS TOTALMONEY,
                    (CT21.Tien2 + CT21.Thue) AS TOTALPAYMENT 
                    FROM PH21 
                    JOIN CT21 ON PH21.Stt_rec = CT21.Stt_rec 
                    JOIN ('{0}') DMKH ON DMKH.Ma_kh = PH21.Ma_kh 
                    WHERE PH21.Stt_rec IN (" + ids + ")", CodPathDMKH);
                    listInvoicesCacheFile.AddRange(invoiceDAO.GetInvoices(sqlGetInvoicesFromCacheFile_PH21));

                    string sqlGetInvoicesFromCacheFile_PH86 = String.Format(@"SELECT PH86.Stt_rec AS AME_INVOICEID,
                    CTOT(TRANSFORM(PH86.Date0) + ' ' + PH86.Time0) AS AME_DATETIME0,
                    CTOT(TRANSFORM(PH86.Date) + ' ' + PH86.Time) AS AME_DATETIME,
                    PH86.So_ct AS INVOICECODE,
                    PH86.Ong_ba AS CUSBUYER,
                    DMKH.Ten_kh AS CUSNAME,
                    PH86.Ma_so_thue AS CUSTAXCODE,
                    PH86.Dia_chi AS CUSADDRESS,
                    PH86.Ma_kh AS CUSTOMERCODE,
                    PH86.Ngay_ct AS INITTIME,
                    DMKH.E_mail AS CUSEMAIL,
                    PH86.Ma_thue AS MATHUE,
                    PH86.Thue_suat AS TAXRATE1,
                    DMVT.Ten_vt AS PRODUCTNAME,
                    CT86.So_luong AS QUANTITY,
                    DMVT.Dvt AS QUANTITYUNIT,
                    CT86.Gia AS RETAILPRICE,
                    CT86.Thue AS TOTALTAX,
                    CT86.Tien AS TOTALMONEY,
                    (CT86.Tien + CT86.Thue) AS TOTALPAYMENT
                    FROM PH86 
                    JOIN CT86 ON PH86.Stt_rec = CT86.Stt_rec 
                    JOIN ('{0}') DMVT ON DMVT.Ma_vt = CT86.Ma_vt 
                    JOIN ('{1}') DMKH ON DMKH.Ma_kh = PH86.Ma_kh
                    WHERE PH86.Stt_rec IN (" + ids + ")", CodPathDMVT, CodPathDMKH);
                    listInvoicesCacheFile.AddRange(invoiceDAO.GetInvoices(sqlGetInvoicesFromCacheFile_PH86));
                }
                if (listInvoicesCacheFile.Count > 0)
                {
                    WriteLog("Log Data", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " InvoiceIDs: " + string.Join(",", listInvoicesCacheFile.Select(x => x.AME_INVOICEID).Distinct()));
                }
                listInvoices.AddRange(listInvoicesCacheFile);

                //Lấy ra rồi thì xóa cache
                RemoveContentCacheFile();
                var groupInvoice = listInvoices.GroupBy(x => x.AME_INVOICEID).Select(y => y.ToList()).ToList();
                //Tạo hóa đơn từ list hóa đơn ở trên listInvoiceData
                foreach (var invoices in groupInvoice)
                {
                    InvoiceBO objInvocie = invoices[0];
                    objInvocie.CUSTAXCODE = objInvocie.CUSTAXCODE == null ? "" : objInvocie.CUSTAXCODE.Trim();
                    objInvocie.CUSEMAIL = objInvocie.CUSEMAIL == null ? "" : objInvocie.CUSEMAIL.Trim();
                    objInvocie.CUSADDRESS = objInvocie.CUSADDRESS == null ? "" : objInvocie.CUSADDRESS.Trim();
                    objInvocie.CUSNAME = objInvocie.CUSNAME == null ? "" : objInvocie.CUSNAME.Trim();
                    objInvocie.CUSBUYER = objInvocie.CUSBUYER == null ? "" : objInvocie.CUSBUYER.Trim();
                    objInvocie.NOTE = objInvocie.NOTE == null ? "" : objInvocie.NOTE.Trim();
                    objInvocie.CUSBANKNAME = objInvocie.CUSBANKNAME == null ? "" : objInvocie.CUSBANKNAME.Trim();
                    objInvocie.INVOICECODE = objInvocie.AME_INVOICEID;

                    //Chi tiết hóa đơn
                    var decimalCharacter = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];
                    List<InvoiceDetailBO> listObjInvoiceDetail = new List<InvoiceDetailBO>();
                    foreach (var inv in invoices.OrderBy(x => x.STT))
                    {
                        var objInvoiceDetail = new InvoiceDetailBO()
                        {
                            PRODUCTNAME = inv.PRODUCTNAME.Trim(),
                            QUANTITYUNIT = inv.QUANTITYUNIT.Trim(),
                            QUANTITY = inv.QUANTITY,
                            TAXRATE = inv.MATHUE.Trim() != "KT" ? int.Parse(inv.TAXRATE1.ToString().Split(decimalCharacter)[0]) : -1,
                            RETAILPRICE = inv.RETAILPRICE,
                            TOTALMONEY = inv.TOTALMONEY,
                            TOTALTAX = inv.TOTALTAX,
                            TOTALPAYMENT = inv.TOTALPAYMENT
                        };
                        listObjInvoiceDetail.Add(objInvoiceDetail);
                    }
                    objInvocie.LISTPRODUCT = listObjInvoiceDetail.DistinctBy(x => x.PRODUCTNAME).ToList();
                    listInvoicesData.Add(objInvocie);
                }

                var listMAHD = string.Join(",", listInvoices.Select(x => x.AME_INVOICEID).Distinct());
                if (listInvoicesData.Count > 0)
                {
                    result = SendData(listInvoicesData);
                }
                if (!result.Success)
                {
                    SaveToCacheFile(listMAHD);
                }
            }
            catch (Exception ex)
            {
                ServiceResponse sr = new ServiceResponse()
                {
                    Code = "2",
                    Message = "Exception get invoice",
                    Success = false,
                    Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                };
                WriteLog("SendData", ex.ToString() + "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time);
            }
        }

        public ServiceResponse SendData(List<InvoiceBO> listInvoicesData)
        {
            try
            {
                string token = string.Empty;
                var isGetTokenSuccess = GetToken(ref token);
                if (!isGetTokenSuccess.Success)
                {
                    WriteLog("SendData", "GetToken không thành công." + "\n" + isGetTokenSuccess.Code + "-" + isGetTokenSuccess.Message + "-" + isGetTokenSuccess.Time);
                }

                ServiceResponse sr = new ServiceResponse();
                string url = ConfigHelper.InvoiceApiUrl;
                string inputJson = new JavaScriptSerializer().Serialize(listInvoicesData);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentLength = inputJson.Length;
                httpWebRequest.AllowWriteStreamBuffering = false;
                httpWebRequest.SendChunked = true;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(inputJson);
                }
                var finalResult = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResult = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                sr = JsonConvert.DeserializeObject<ServiceResponse>(finalResult);
                WriteLog("SendData", "Gửi dữ liệu thành công." + "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time + "-" + string.Join(";", listInvoicesData.Select(x => x.AME_INVOICEID)));
                return sr;
            }
            catch (Exception ex)
            {
                ServiceResponse sr = new ServiceResponse()
                {
                    Code = "2",
                    Message = "Exception gửi data API",
                    Success = false,
                    Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                };
                WriteLog("SendData", ex.ToString() + "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time);
                return sr;
            }
        }

        private ServiceResponse GetToken(ref string token)
        {
            try
            {
                ServiceResponse sr = new ServiceResponse();
                string url = ConfigHelper.NOVAON_GetTokenUrl;
                var data = new TokenModel()
                {
                    taxCode = ConfigHelper.OrgComtaxcode,
                    appId = ConfigHelper.NOVAON_AppId
                };
                string inputJson = new JavaScriptSerializer().Serialize(data);

                HttpResponseMessage result;
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(inputJson.ToString(), Encoding.UTF8, "application/json");
                    result = httpClient.PostAsync(url, content).Result;
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    sr = new ServiceResponse()
                    {
                        Code = ((int)HttpStatusCode.NotFound).ToString(),
                        Message = "Lỗi lấy token. Không đúng url. Api ngừng hoạt động.",
                        Success = false,
                        Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                    };
                    WriteLog("GetToken", "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time);
                }
                else
                {
                    var tokenResponse = JsonConvert.DeserializeObject<data>(result.Content.ReadAsStringAsync().Result);
                    if (tokenResponse == null)
                    {
                        sr = new ServiceResponse()
                        {
                            Code = ((int)HttpStatusCode.BadRequest).ToString(),
                            Message = "TokenResponse null",
                            Success = false,
                            Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                        };
                        WriteLog("GetToken", "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time);
                    }
                    else
                    {
                        token = tokenResponse.token.RawData;
                        sr = new ServiceResponse()
                        {
                            Code = ((int)HttpStatusCode.OK).ToString(),
                            Success = true,
                            Message = token
                        };
                    }
                }
                return sr;
            }
            catch (Exception ex)
            {
                ServiceResponse sr = new ServiceResponse()
                {
                    Code = ((int)HttpStatusCode.InternalServerError).ToString(),
                    Message = "Exception lấy token.",
                    Success = false,
                    Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                };
                WriteLog("GetToken", ex.ToString() + "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time);
                return sr;
            }
        }

        private void WriteLog(string functionName, string strMessage)
        {
            try
            {
                string strErrorLogFile = LogPath + "log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"; // File ghi log lỗi
                System.IO.File.AppendAllText(strErrorLogFile, string.Format("{0}{1} {2}: {3}", Environment.NewLine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), functionName, strMessage));
            }
            catch
            {

            }
        }

        private void SaveToCacheFile(string listInvoiceIds)
        {
            try
            {
                var isFileEmpty = ReadCacheFile();
                var cacheFilePath = ConfigHelper.NOVAON_CacheFilePath;
                using (StreamWriter sw = new StreamWriter(cacheFilePath, true))
                {
                    if (string.IsNullOrEmpty(isFileEmpty))
                    {
                        sw.Write(listInvoiceIds);
                    }
                    else
                    {
                        sw.Write("," + listInvoiceIds);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("SaveToCacheFile", "Lỗi lưu cache file. \n" + ex.ToString());
            }
        }

        private string ReadCacheFile()
        {
            try
            {
                var line = "";
                var cacheFilePath = ConfigHelper.NOVAON_CacheFilePath;
                using (StreamReader sr = new StreamReader(cacheFilePath))
                {
                    line = sr.ReadLine();
                }
                return line;
            }
            catch (Exception ex)
            {
                WriteLog("ReadCacheFile", "Lỗi đọc cache file. \n" + ex.ToString());
                return "";
            }
        }

        private void RemoveContentCacheFile()
        {
            try
            {
                var cacheFilePath = ConfigHelper.NOVAON_CacheFilePath;
                using (StreamWriter sw = new StreamWriter(cacheFilePath, false))
                {
                    sw.Write("");
                }
            }
            catch (Exception ex)
            {
                WriteLog("RemoveContentCacheFile", "Lỗi xóa nội dung cache file. \n" + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            WriteLog("OnStop", "OnStop");
        }

    }
}
