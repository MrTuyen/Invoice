using Newtonsoft.Json;
using SynctaskInvoicePMSmat.Common;
using SynctaskInvoicePMSmat.DAO;
using SynctaskInvoicePMSmat.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;

namespace SynctaskInvoicePMSmat
{
    public partial class NOVAON_Service : ServiceBase
    {
        #region Variables
        private Timer timer = new Timer();
        private static List<InvoiceBO> listInvoices = null;
        private int setIntervalTime = ConfigHelper.setIntervalTime;
        private string LogPath = ConfigHelper.LogPath;

        #endregion

        public void onDebug()
        {
            OnStart(null);
        }

        public NOVAON_Service()
        {
            InitializeComponent();
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

        protected override void OnStop()
        {
            WriteLog("OnStop", "OnStop");
        }

        private void GetInvoice()
        {
            try
            {
                ServiceResponse result = new ServiceResponse();
                InvoiceDAO invoiceDAO = new InvoiceDAO();
                var listInvoicesData = new List<InvoiceBO>();
                var listInvoices = new List<InvoiceBO>();
                var listInvoicesCacheFile = new List<InvoiceBO>();
                var requestedTime = DateTime.Now;
                if (ConfigHelper.SetDateTime == "Y")
                {
                    requestedTime = Convert.ToDateTime(ConfigHelper.SetDateTimeValue);
                }

                listInvoices = invoiceDAO.GetInvoices(requestedTime);
                var cacheFileData = ReadCacheFile(); // Đọc cache file. Kiểm tra nếu có dữ liệu thì đẩy lấy dữ liệu trong db và đẩy lên serivce
                if (!string.IsNullOrEmpty(cacheFileData))
                    listInvoicesCacheFile = invoiceDAO.GetInvoicesFromCacheFile(cacheFileData);
                listInvoices.AddRange(listInvoicesCacheFile);

                //Lấy ra rồi thì xóa cache
                RemoveContentCacheFile();

                var groupInvoice = listInvoices.GroupBy(x => x.PMSMAT_INVOICEID).Select(y => y.ToList()).ToList();
                //Tạo hóa đơn từ list hóa đơn ở trên listInvoiceData
                foreach (var invoices in groupInvoice)
                {
                    InvoiceBO objInvocie = invoices[0];
                    objInvocie.INVOICECODE = objInvocie.PMSMAT_INVOICEID;

                    //Chi tiết hóa đơn
                    List<InvoiceDetailBO> listObjInvoiceDetail = new List<InvoiceDetailBO>();
                    foreach (var inv in invoices.OrderBy(x => x.STT))
                    {
                        var objInvoiceDetail = new InvoiceDetailBO()
                        {
                            PRODUCTNAME = inv.PRODUCTNAME.Trim(),
                            QUANTITYUNIT = inv.QUANTITYUNIT.Trim(),
                            QUANTITY = inv.QUANTITY,
                            TAXRATE = inv.TAXRATE,
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

                var listMAHD = string.Join(",", listInvoices.Select(x => x.PMSMAT_INVOICEID).Distinct());
                if (listInvoicesData.Count > 0)
                {
                    result = SendData(listInvoicesData);
                }
                if (result.Success)
                {
                    // Update data sau khi gửi dữ liệu thành công => DaXuatHD = 1
                    if (listInvoices != null)
                    {
                        var isSuccess = invoiceDAO.UpdateInvoices(listMAHD);
                        if (isSuccess == -1)
                        {
                            WriteLog("GetInvoice", $"Lỗi cập nhật hóa đơn thành đã xuất. {listMAHD}");
                        }
                    }
                }
                else
                {
                    // Nếu xảy ra vấn đề thì lưu dữ liệu tại file cache. 
                    // Bình thường trở lại thì đọc file cache để đẩy data lên hệ thống
                    SaveToCacheFile(listMAHD);
                }
            }
            catch (Exception objEx)
            {
                WriteLog("GetInvoice", objEx.ToString());
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
                WriteLog("SendData", "Gửi dữ liệu thành công." + "\n" + sr.Code + "-" + sr.Message + "-" + sr.Time + "-" + string.Join(";", listInvoicesData.Select(x => x.PMSMAT_INVOICEID)));
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
    }
}
