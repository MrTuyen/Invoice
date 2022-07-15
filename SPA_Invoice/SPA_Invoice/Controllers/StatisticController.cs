using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Presentation;
using OfficeOpenXml.Style;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using DS.Common.Helpers;
using static DS.BusinessObject.Invoice.UsingInvoiceXML;
using DS.BusinessLogic.Number;
using System.Reflection;
using DS.BusinessObject;
using System.Linq;
using System.Collections.Generic;
using DS.BusinessLogic.EmailSender;
using DS.BusinessObject.EmailSender;
using DS.BusinessLogic.Invoice;
using DS.BusinessObject.Invoice;
using static DS.Common.Enums.EnumHelper;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace SPA_Invoice.Controllers
{
    public class StatisticController : BaseController
    {
        // GET: Statistic
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult MailHistory()
        {
            return PartialView();
        }

        public ActionResult ActionHistory()
        {
            return PartialView();
        }

        public ActionResult UsingInvoice()
        {
            return PartialView();
        }

        public ActionResult OutputInvoice()
        {
            return PartialView();
        }

        byte[] GetFile(string s)
        {
            using (FileStream fs = System.IO.File.OpenRead(s))
            {
                //System.IO.FileStream fs = System.IO.File.OpenRead(s);
                byte[] data = new byte[fs.Length];
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new IOException(s);
                return data;
            }
        }

        /// <summary>
        /// Báo cáo tình hình sử dụng hóa đơn
        /// </summary>
        /// <param name="reportUsingInvoice"></param>
        /// <returns></returns>
        public ActionResult GetUsingInvoice(FormSearchUsingInvoice reportUsingInvoice)
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                NumberBLL numberBLL = new NumberBLL();
                var month = DateTime.Now.Month;
                var quarter = 1;
                var year = DateTime.Now.Year;
                var preMonth = DateTime.Now.Month;
                var preQuarter = 1;
                var preYear = DateTime.Now.Year - 1;
                DateTime startDatePreviousTerm = new DateTime(preYear, preMonth, 1);
                DateTime endDatePreviousTerm = startDatePreviousTerm.AddMonths(1).AddDays(-1);
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                // Kiểm tra dữ liệu đầu vào để lấy thời gian kỳ trước và kỳ này
                var M_Q_Name = reportUsingInvoice.MONTHQUARTER.Substring(0, 1);
                var M_Q_Index = int.Parse(reportUsingInvoice.MONTHQUARTER.Substring(1, reportUsingInvoice.MONTHQUARTER.Length - 1));
                if (reportUsingInvoice != null)
                {
                    year = reportUsingInvoice.YEAR;
                    // Lấy giá trị tháng/quý
                    // Báo cáo theo tháng
                    if (M_Q_Name == "M")
                    {
                        month = M_Q_Index;
                        preMonth = month - 1;
                        if (month == 1)
                        {
                            preMonth = 12;
                            preYear = year - 1;
                        }
                        quarter = 0;
                        preQuarter = 0;
                    }
                    // Báo cáo theo quý
                    if (M_Q_Name == "Q")
                    {
                        quarter = M_Q_Index;
                        preQuarter = quarter - 1;
                        if (quarter == 1)
                        {
                            preQuarter = 4;
                            preYear = year - 1;
                        }
                        month = 0;
                        preMonth = 0;
                    }
                }

                // Lấy thời gian kỳ này
                if (month != 0)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else
                {
                    var firstMonthInQuarter = 1;
                    switch (quarter)
                    {
                        case 2: { firstMonthInQuarter = 4; break; }
                        case 3: { firstMonthInQuarter = 7; break; }
                        case 4: { firstMonthInQuarter = 10; break; }
                    }
                    startDate = new DateTime(year, firstMonthInQuarter, 1);
                    endDate = startDate.AddMonths(3).AddDays(-1);
                }
                // Lấy thời gian kỳ trước
                if (preMonth != 0)
                {
                    startDatePreviousTerm = new DateTime(preYear, preMonth, 1);
                    endDatePreviousTerm = startDatePreviousTerm.AddMonths(1).AddDays(-1);
                }
                else
                {
                    var firstPreMonthInQuarter = 1;
                    switch (preQuarter)
                    {
                        case 2: { firstPreMonthInQuarter = 4; break; }
                        case 3: { firstPreMonthInQuarter = 7; break; }
                        case 4: { firstPreMonthInQuarter = 10; break; }
                    }
                    startDatePreviousTerm = new DateTime(preYear, firstPreMonthInQuarter, 1);
                    endDatePreviousTerm = startDatePreviousTerm.AddMonths(3).AddDays(-1);
                }

                // HÓA ĐƠN KỲ TRƯỚC
                // Lấy danh sách hóa đơn kỳ trước
                InvoiceNumberBO formSearchPreTerm = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDatePreviousTerm.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDatePreviousTerm.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                var resultPreTerm = numberBLL.GetUsingInvoice(formSearchPreTerm);

                // Danh sách mẫu số ký hiệu kỳ trước
                var listUsingInvoicePreTerm = new List<UsingInvoiceBO>();
                var listFormSymbolPreTerm = new Dictionary<string, string>();
                foreach (var item in resultPreTerm)
                {
                    if (!listFormSymbolPreTerm.ContainsKey(item.FORMCODE + item.SYMBOLCODE))
                    {
                        listFormSymbolPreTerm.Add(item.FORMCODE + item.SYMBOLCODE, item.FORMCODE);
                        UsingInvoiceBO usingInvoiceBO = new UsingInvoiceBO()
                        {
                            FORMCODE = item.FORMCODE,
                            SYMBOLCODE = item.SYMBOLCODE
                        };
                        listUsingInvoicePreTerm.Add(usingInvoiceBO);
                    }
                }
                foreach (var item in listUsingInvoicePreTerm)
                {
                    var numberObj = numberBLL.GetNumber(new FormSearchNumber() { COMTAXCODE = objUser.COMTAXCODE, FORMCODE = item.FORMCODE, SYMBOLCODE = item.SYMBOLCODE });
                    var minNumber = resultPreTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).Min(x => x.NUMBER);
                    var maxNumber = resultPreTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).Max(x => x.NUMBER);
                    item.FROMNUMBER = minNumber;
                    item.TONUMBER = maxNumber;
                    //item.ROOT_TONUMBER = numberObj.Where(x => x.FROMNUMBER < maxNumber && x.TONUMBER > maxNumber).FirstOrDefault().TONUMBER;
                    //item.ROOT_FROMNUMBER = numberObj.Where(x => x.FROMNUMBER < maxNumber && x.TONUMBER > maxNumber).FirstOrDefault().FROMNUMBER;

                    item.ROOT_TONUMBER = numberObj.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).FirstOrDefault().TONUMBER;
                    item.ROOT_FROMNUMBER = numberObj.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).FirstOrDefault().FROMNUMBER;
                }

                // HÓA ĐƠN KỲ NÀY
                // Lấy danh sách hóa đơn kỳ này
                InvoiceNumberBO formSearchThisTerm = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                var resultThisTerm = numberBLL.GetUsingInvoice(formSearchThisTerm);

                // Danh sách mẫu số ký hiệu kỳ này
                var listFormSymbolThisTerm = new Dictionary<string, string>();
                var listUsingInvoiceThisTerm = new List<UsingInvoiceBO>();
                foreach (var item in resultThisTerm)
                {
                    if (!listFormSymbolThisTerm.ContainsKey(item.FORMCODE + item.SYMBOLCODE))
                    {
                        listFormSymbolThisTerm.Add(item.FORMCODE + item.SYMBOLCODE, item.FORMCODE);
                        UsingInvoiceBO usingInvoiceBO = new UsingInvoiceBO()
                        {
                            FORMCODE = item.FORMCODE,
                            SYMBOLCODE = item.SYMBOLCODE
                        };
                        listUsingInvoiceThisTerm.Add(usingInvoiceBO);
                    }
                }

                // Danh sách thông báo phát hành có thời gian phát hành lớn hơn hoặc bằng thời gian kỳ này => Để hiển thị ra danh sách hóa đơn kể cả chưa phát hành hóa đơn nào.
                var tempListReleaseNotices = numberBLL.GetNumber(new FormSearchNumber() { COMTAXCODE = objUser.COMTAXCODE });
                var listReleaseNotices = tempListReleaseNotices.Where(x => x.FROMTIME < formSearchThisTerm.TOTIME).ToList();

                foreach (var item in listUsingInvoiceThisTerm)
                {
                    var numberObj = numberBLL.GetNumber(new FormSearchNumber() { COMTAXCODE = objUser.COMTAXCODE, FORMCODE = item.FORMCODE, SYMBOLCODE = item.SYMBOLCODE }).FirstOrDefault();
                    var isCanceled = false;
                    if (numberObj.STATUS == 3)
                        isCanceled = true;
                    // Xóa những thông báo phát hành có phát hành hóa đơn trong kỳ
                    var itemToRemove = listReleaseNotices.Where(x => x.ID == numberObj.ID).FirstOrDefault();
                    listReleaseNotices.Remove(itemToRemove);

                    var minNumber = resultThisTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).Min(x => x.NUMBER);
                    var maxNumber = resultThisTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE).Max(x => x.NUMBER);

                    //var rootFromNumber = numberObj.Where(x => x.FROMNUMBER <= maxNumber && x.TONUMBER >= maxNumber).FirstOrDefault().FROMNUMBER;
                    //var rootToNumber = numberObj.Where(x => x.FROMNUMBER <= maxNumber && x.TONUMBER >= maxNumber).FirstOrDefault().TONUMBER;
                    //item.ROOT_TONUMBER = rootToNumber;
                    //item.ROOT_FROMNUMBER = rootFromNumber;
                    var rootFromNumber = numberObj.FROMNUMBER;
                    var rootToNumber = numberObj.TONUMBER;

                    item.SUM_PRE_TERM = (rootToNumber - rootFromNumber) + 1;
                    item.ROOT_FROMNUMBER = rootFromNumber;
                    item.ROOT_TONUMBER = rootToNumber;

                    if (numberObj.FROMTIME >= formSearchThisTerm.FROMTIME && numberObj.FROMTIME <= formSearchThisTerm.TOTIME)
                    {
                        item.BUY_INTERM_FROMNUMBER = rootFromNumber;
                        item.BUY_INTERM_TONUMBER = rootToNumber;
                    }
                    else
                    {
                        item.PRE_TERM_RETAIN_FROMNUMBER = rootFromNumber;
                        item.PRE_TERM_RETAIN_TONUMBER = rootToNumber;
                    }

                    item.ALL_USED_FROMNUMBER = minNumber;
                    item.ALL_USED_TONUMBER = isCanceled ? rootToNumber : maxNumber;
                    var realUsedNumber = isCanceled ? rootToNumber - minNumber + 1 : minNumber == maxNumber ? 1 : (maxNumber - minNumber) + 1;
                    item.ALL_USED_SUM = realUsedNumber;

                    var deletedNumber = resultThisTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE && x.NUMBER >= rootFromNumber && x.NUMBER <= rootToNumber && x.INVOICETYPE == 3).ToList();
                    item.DELETED_USED_DETAIL_LIST = string.Join(";", deletedNumber.Select(x => x.NUMBER.ToString("D7")));
                    item.DELETED_USED_SUM = deletedNumber.Count();
                    item.REAL_USED_SUM = isCanceled ? ((maxNumber - minNumber) + 1) - deletedNumber.Count() : realUsedNumber - deletedNumber.Count();
                    item.LAST_RETAIN_FROMNUMBER = isCanceled ? "" : (maxNumber == rootToNumber ? "" : (maxNumber + 1).ToString("D7"));
                    item.LAST_RETAIN_TONUMBER = isCanceled ? "" : (maxNumber == rootToNumber ? "" : rootToNumber.ToString("D7"));
                    item.LAST_RETAIN_SUM = isCanceled ? 0 : (maxNumber == rootToNumber ? 0 : ((rootToNumber - maxNumber)));

                    item.CANCELED_USED_SUM = isCanceled ? rootToNumber - maxNumber : 0;
                    item.CANCELED_USED_DETAIL_LIST = isCanceled ? $"{(maxNumber + 1).ToString("D7")}-{rootToNumber.ToString("D7")}" : "";
                }

                // Những thông báo còn lại không có hóa đơn phát hành trong kỳ
                foreach (var releaseNoticeItem in listReleaseNotices)
                {
                    if (releaseNoticeItem.STATUS == 2 || releaseNoticeItem.STATUS == 3) // Hết số hoặc hủy thì không lấy nữa
                        continue;

                    var numberObj = numberBLL.GetNumber(new FormSearchNumber() { COMTAXCODE = objUser.COMTAXCODE, FORMCODE = releaseNoticeItem.FORMCODE, SYMBOLCODE = releaseNoticeItem.SYMBOLCODE }).FirstOrDefault();
                    // Lấy số hóa đơn lớn nhất đã phát hành của dải số
                    var maxInvoiceNumber = invoiceBLL.GetMaxNumberInvoice(new FormSearchInvoice() { COMTAXCODE = objUser.COMTAXCODE, FORMCODE = numberObj.FORMCODE, SYMBOLCODE = numberObj.SYMBOLCODE });

                    UsingInvoiceBO item = new UsingInvoiceBO();
                    item.FORMCODE = numberObj.FORMCODE;
                    item.SYMBOLCODE = numberObj.SYMBOLCODE;
                    var minNumber = 0;
                    var maxNumber = 0;

                    var rootFromNumber = maxInvoiceNumber != null ? (maxInvoiceNumber.SIGNEDTIME <= formSearchThisTerm.FROMTIME ? maxInvoiceNumber.NUMBER : 0) : 0;
                    var rootToNumber = numberObj.TONUMBER;

                    item.ROOT_FROMNUMBER = numberObj.FROMNUMBER;
                    item.ROOT_TONUMBER = numberObj.TONUMBER;

                    item.SUM_PRE_TERM = (rootToNumber - rootFromNumber);
                    if (numberObj.FROMTIME >= formSearchThisTerm.FROMTIME && numberObj.FROMTIME <= formSearchThisTerm.TOTIME)
                    {
                        item.BUY_INTERM_FROMNUMBER = rootFromNumber + 1;
                        item.BUY_INTERM_TONUMBER = rootToNumber;
                    }
                    else
                    {
                        item.PRE_TERM_RETAIN_FROMNUMBER = rootFromNumber + 1;
                        item.PRE_TERM_RETAIN_TONUMBER = rootToNumber;
                    }

                    item.ALL_USED_FROMNUMBER = minNumber;
                    item.ALL_USED_TONUMBER = maxNumber;
                    var realUsedNumber = 0;
                    item.ALL_USED_SUM = realUsedNumber;

                    var deletedNumber = resultThisTerm.Where(x => x.FORMCODE == item.FORMCODE && x.SYMBOLCODE == item.SYMBOLCODE && x.NUMBER >= rootFromNumber && x.NUMBER <= rootToNumber && x.INVOICETYPE == 3).ToList();
                    item.DELETED_USED_DETAIL_LIST = string.Join(";", deletedNumber.Select(x => x.NUMBER.ToString("D7")));
                    item.DELETED_USED_SUM = deletedNumber.Count();
                    item.REAL_USED_SUM = realUsedNumber - deletedNumber.Count();
                    item.LAST_RETAIN_FROMNUMBER = (rootFromNumber + 1).ToString("D7");
                    item.LAST_RETAIN_TONUMBER = rootToNumber.ToString("D7");
                    item.LAST_RETAIN_SUM = (rootToNumber - rootFromNumber);

                    listUsingInvoiceThisTerm.Add(item);
                }


                // Map dữ liệu lại với nhau theo formcode, symbolcode, fromnumber, tonumber tìm ra số dư đầu kì nếu mẫu số ký hiệu số từ và số đến trùng với nhau
                foreach (var item in listUsingInvoicePreTerm)
                {
                    foreach (var item_1 in listUsingInvoiceThisTerm)
                    {
                        if (item_1.FORMCODE == item.FORMCODE && item_1.SYMBOLCODE == item.SYMBOLCODE && item_1.ROOT_FROMNUMBER == item.ROOT_FROMNUMBER && item_1.ROOT_TONUMBER == item.ROOT_TONUMBER)
                        {
                            item_1.PRE_TERM_RETAIN_FROMNUMBER = item.TONUMBER + 1;
                            item_1.PRE_TERM_RETAIN_TONUMBER = item.ROOT_TONUMBER;
                            item_1.SUM_PRE_TERM = (item.ROOT_TONUMBER - item.TONUMBER);
                        }
                    }
                }

                // Lấy thông tin kỳ báo cáo để hiển thị ra view
                var index = int.Parse(reportUsingInvoice.MONTHQUARTER.Substring(1, reportUsingInvoice.MONTHQUARTER.Length - 1)).ToString("D2");
                var reportTime = new { FIRSTTIME = startDate.ToString("dd/MM/yyyy"), LASTTIME = endDate.ToString("dd/MM/yyyy"), TERM = (reportUsingInvoice.MONTHQUARTER.Contains("M") ? $"Tháng {index}" : $"Quý {index}") + $" năm {year}" };

                // Lưu session để tải file XML
                Session["LISTDATA"] = listUsingInvoiceThisTerm;
                Session["STARTDATE"] = startDate;
                Session["ENDDATE"] = endDate;
                Session["TERMTYPE"] = M_Q_Name;
                Session["TERM"] = M_Q_Index + "/" + year;

                if (numberBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(numberBLL.ResultMessageBO.Message, numberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUsingInvoice");
                    return Json(new { rs = false, msg = numberBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, listUsingInvoiceThisTerm, reportTime });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy báo cáo tình hình sử dụng hóa đơn", ex, MethodBase.GetCurrentMethod().Name, "GetUsingInvoice");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadUsingInvoiceReportXML()
        {
            try
            {
                var data = Session["LISTDATA"];
                var startDate = (DateTime)Session["STARTDATE"];
                var endDate = (DateTime)Session["ENDDATE"];
                var termtype = Session["TERMTYPE"];
                var term = Session["TERM"];

                // Print xml
                HSoThueDTu hstdt = new HSoThueDTu();
                HSoThueDTuHSoKhaiThue hskt = new HSoThueDTuHSoKhaiThue();
                HSoThueDTuHSoKhaiThueTTinChung ttc = new HSoThueDTuHSoKhaiThueTTinChung()
                {
                    TTinDVu = new HSoThueDTuHSoKhaiThueTTinChungTTinDVu()
                    {
                        maDVu = "HTKK",
                        tenDVu = "HỖ TRỢ KÊ KHAI THUẾ",
                        pbanDVu = "4.2.3",
                        ttinNhaCCapDVu = "ONFINANCE"
                    },
                    TTinTKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue()
                    {
                        TKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue()
                        {
                            maTKhai = 102,
                            tenTKhai = "Báo cáo tình hình sử dụng hóa đơn (BC26/AC)",
                            moTaBMau = "(Ban hành kèm theo Thông tư số 39/2014/TT-BTC ngày 31/3/2014 của Bộ Tài chính)",
                            pbanTKhaiXML = "2.0.8",
                            loaiTKhai = "C",
                            soLan = 0,
                            KyKKhaiThue = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue()
                            {
                                kieuKy = termtype.ToString(),
                                kyKKhai = term.ToString(),
                                kyKKhaiTuNgay = startDate.ToString("dd/MM/yyyy"),
                                kyKKhaiDenNgay = endDate.ToString("dd/MM/yyyy"),
                                kyKKhaiTuThang = "",
                                kyKKhaiDenThang = ""
                            },
                            maCQTNoiNop = objUser.TAXDEPARTEMENTCODE,
                            tenCQTNoiNop = objUser.TAXDEPARTEMENT,
                            ngayLapTKhai = DateTime.Now,
                            GiaHan = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan()
                            {
                                maLyDoGiaHan = "",
                                lyDoGiaHan = ""
                            },
                            nguoiKy = "",
                            ngayKy = DateTime.Now,
                            nganhNgheKD = "",
                        },
                        NNT = new HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT()
                        {
                            mst = objUser.COMTAXCODE,
                            tenNNT = objUser.COMNAME,
                            dchiNNT = objUser.COMADDRESS,
                            phuongXa = "",
                            maHuyenNNT = "",
                            tenHuyenNNT = "",
                            maTinhNNT = "",
                            tenTinhNNT = "",
                            dthoaiNNT = "",
                            faxNNT = "",
                            emailNNT = ""
                        }
                    }
                };

                List<ChiTiet> lstItemDetail = new List<ChiTiet>();
                var i = 1;
                long total_preterm = 0;
                long total_used = 0;
                long total_lastterm = 0;
                string invoiceName = "";
                switch (objUser.USINGINVOICETYPE)
                {
                    case (int)AccountObjectType.HOADONBANHANG:
                    case (int)AccountObjectType.HOADONTRUONGHOC:
                        {
                            invoiceName = "Hóa đơn bán hàng";
                        }
                        break;
                    case (int)AccountObjectType.PHIEUXUATKHO:
                        {
                            invoiceName = "Phiếu xuất kho kiêm vận chuyển nội bộ";
                        }
                        break;
                    default:
                        invoiceName = "Hóa đơn giá trị gia tăng";
                        break;
                }
                foreach (var item in (List<UsingInvoiceBO>)data)
                {
                    var itemDetail = new ChiTiet()
                    {
                        id = "ID_" + i.ToString(),
                        maHoaDon = "01GTKT",
                        tenHDon = invoiceName,
                        kHieuMauHDon = item.FORMCODE,
                        kHieuHDon = item.SYMBOLCODE,
                        soTonMuaTrKy_tongSo = item.SUM_PRE_TERM,
                        soTonDauKy_tuSo = item.PRE_TERM_RETAIN_FROMNUMBER == 0 ? "" : item.PRE_TERM_RETAIN_FROMNUMBER.ToString("D7"),
                        soTonDauKy_denSo = item.PRE_TERM_RETAIN_TONUMBER == 0 ? "" : item.PRE_TERM_RETAIN_TONUMBER.ToString("D7"),
                        muaTrongKy_tuSo = item.BUY_INTERM_FROMNUMBER == 0 ? "" : item.BUY_INTERM_FROMNUMBER.ToString("D7"),
                        muaTrongKy_denSo = item.BUY_INTERM_TONUMBER == 0 ? "" : item.BUY_INTERM_TONUMBER.ToString("D7"),
                        tongSoSuDung_tuSo = item.ALL_USED_FROMNUMBER.ToString("D7"),
                        tongSoSuDung_denSo = item.ALL_USED_TONUMBER.ToString("D7"),
                        tongSoSuDung_cong = item.ALL_USED_SUM,
                        soDaSDung = item.REAL_USED_SUM,
                        xoaBo_soLuong = item.DELETED_USED_SUM,
                        xoaBo_so = item.DELETED_USED_DETAIL_LIST,
                        mat_soLuong = 0,
                        mat_so = "",
                        huy_soLuong = item.CANCELED_USED_SUM,
                        huy_so = item.CANCELED_USED_DETAIL_LIST,
                        tonCuoiKy_tuSo = item.LAST_RETAIN_FROMNUMBER,
                        tonCuoiKy_denSo = item.LAST_RETAIN_TONUMBER,
                        tonCuoiKy_soLuong = item.LAST_RETAIN_SUM,
                    };
                    i++;
                    lstItemDetail.Add(itemDetail);
                    total_preterm += item.SUM_PRE_TERM;
                    total_used += item.ALL_USED_SUM;
                    total_lastterm += item.LAST_RETAIN_SUM;
                }

                HSoThueDTuHSoKhaiThueCTieuTKhaiChinh ctkc = new HSoThueDTuHSoKhaiThueCTieuTKhaiChinh()
                {
                    kyBCaoCuoi = 0,
                    chuyenDiaDiem = 0,
                    ngayDauKyBC = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    ngayCuoiKyBC = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    HoaDon = lstItemDetail,
                    tongCongSoTonDKy = total_preterm,
                    tongCongSDung = total_used,
                    tongCongSoTonCKy = total_lastterm,
                    nguoiLapBieu = objUser.COMLEGALNAME,
                    nguoiDaiDien = objUser.COMLEGALNAME,
                    ngayBCao = DateTime.Now
                };
                hskt.id = "_NODE_TO_SIGN";
                hskt.TTinChung = ttc;
                hskt.CTieuTKhaiChinh = ctkc;
                hstdt.HSoKhaiThue = hskt;

                string filePath = HttpContext.Server.MapPath("~/" + "Baocaotinhhinhsudunghoadon.xml");
                string fileName = $"ONFINANCE_Bao_Cao_Tinh_Hinh_Su_Dung_Hoa_Don_{objUser.COMTAXCODE}_{term}.xml";
                // Ghi dữ liệu vào file
                XMLHelper.SerializationXml(hstdt, filePath);
                // Download file
                return Downloadfile(filePath, fileName);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download báo cáo tình hình sử dụng hóa đơn.", ex, MethodBase.GetCurrentMethod().Name, "DownloadUsingInvoiceReportXML");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Session.Remove("LISTDATA");
                Session.Remove("STARTDATE");
                Session.Remove("ENDDATE");
                Session.Remove("TERMTYPE");
                Session.Remove("TERM");
            }
        }

        public ActionResult Downloadfile(string link, string fileName)
        {
            try
            {
                //Loại bỏ domain
                //link = link.Replace(ConfigHelper.UriInvoiceFolder, "");

                //Tách lấy tên file
                string[] path = link.Split('/');

                //Đưa về đường dẫn vật lý
                link = ConfigHelper.PhysicalInvoiceFolder + link.Replace('/', '\\');

                //Đọc file
                byte[] fileBytes = GetFile(link);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception objEx)
            {
                ConfigHelper.Instance.WriteLog("Lỗi tải file.", objEx, MethodBase.GetCurrentMethod().Name, "Downloadfile");
                return null;
            }
        }

        /// <summary>
        /// Bảng kê hóa đơn đầu ra
        /// </summary>
        /// <param name="reportUsingInvoice"></param>
        /// <returns></returns>
        public ActionResult GetOutputInvoice(FormSearchUsingInvoice reportUsingInvoice)
        {
            try
            {
                var month = DateTime.Now.Month;
                var quarter = 1;
                var year = DateTime.Now.Year;
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                if (reportUsingInvoice != null)
                {
                    if (reportUsingInvoice.MONTHQUARTER.Contains("M"))
                    {
                        month = int.Parse(reportUsingInvoice.MONTHQUARTER.Substring(1, reportUsingInvoice.MONTHQUARTER.Length - 1));
                        quarter = 0;
                    }
                    if (reportUsingInvoice.MONTHQUARTER.Contains("Q"))
                    {
                        quarter = int.Parse(reportUsingInvoice.MONTHQUARTER.Substring(1, reportUsingInvoice.MONTHQUARTER.Length - 1));
                        month = 0;
                    }
                    year = reportUsingInvoice.YEAR;
                }

                if (month != 0)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else
                {
                    var firstMonthInQuarter = 1;
                    switch (quarter)
                    {
                        case 2: { firstMonthInQuarter = 4; break; }
                        case 3: { firstMonthInQuarter = 7; break; }
                        case 4: { firstMonthInQuarter = 10; break; }
                    }
                    startDate = new DateTime(year, firstMonthInQuarter, 1);
                    endDate = startDate.AddMonths(3).AddDays(-1);
                }

                Session["ReportUsingInvoice"] = reportUsingInvoice;
                Session["OutputStartDate"] = startDate;
                Session["OutputEndDate"] = endDate;

                InvoiceNumberBO formSearch = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.GetOutputInvoice(formSearch);
                if (objUser.USINGINVOICETYPE == (int)AccountObjectType.HOADONBANHANG)
                {
                    result = numberBLL.GetOutputSellInvoice(formSearch);
                }

                if (numberBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(numberBLL.ResultMessageBO.Message, numberBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
                    return Json(new { rs = false, msg = numberBLL.ResultMessageBO.Message });
                }
                var index = int.Parse(reportUsingInvoice.MONTHQUARTER.Substring(1, reportUsingInvoice.MONTHQUARTER.Length - 1)).ToString("D2");
                var reportTime = new { FIRSTTIME = startDate.ToString("dd/MM/yyyy"), LASTTIME = endDate.ToString("dd/MM/yyyy"), TERM = (reportUsingInvoice.MONTHQUARTER.Contains("M") ? $"Tháng {index}" : $"Quý {index}") + $" năm {year}" };
                return Json(new { rs = true, result, reportTime });
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy thông tin người dùng", ex, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
                return Json(new { rs = false, msg = $"Lỗi {ex}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Bảng kê hóa đơn đầu ra có chi tiết hàng hóa
        /// </summary>
        public void DownloadOutputInvoiceExcel()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                DateTime startDate = (DateTime)Session["OutputStartDate"];
                DateTime endDate = (DateTime)Session["OutputEndDate"];
                InvoiceNumberBO formSearch = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.GetOutputInvoiceExcel(formSearch);

                ExcelPackage ex = new ExcelPackage();
                ExcelWorksheet ws = ex.Workbook.Worksheets.Add("Bang_ke_hoa_don_dau_ra");
                //ws.Column(3).Width = 50;

                // Section 1
                ws.Cells["E2:J2"].Merge = true;
                ws.Cells["E2"].Value = "BẢNG KÊ HOÁ ĐƠN, CHỨNG TỪ HÀNG HOÁ, DỊCH VỤ BÁN RA";
                ws.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E2"].Style.Font.Bold = true;
                ws.Cells["E2"].Style.Font.Size = 11;

                ws.Cells["E3:J3"].Merge = true;
                ws.Cells["E3"].Value = "(Kèm theo tờ khai thuế GTGT theo mẫu số 01/GTGT)";
                ws.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E3"].Style.Font.Italic = true;
                ws.Cells["E3"].Style.Font.Size = 11;

                ws.Cells["E4:J4"].Merge = true;
                ws.Cells["E4"].Value = $"Kỳ tính thuế: Từ ngày {startDate.ToString("dd/MM/yyyy")} đến ngày {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E4"].Style.Font.Size = 11;


                // Section 2
                ws.Cells["B5"].Value = "Tên người nộp thuế: " + objUser.COMNAME;
                ws.Cells["B5"].Style.Font.Bold = true;
                ws.Cells["B6"].Value = "Mã số thuế: " + objUser.COMTAXCODE;
                ws.Cells["B7"].Value = "Tên đại lý thuế(nếu có):";
                ws.Cells["B8"].Value = "Mã số thuế:";
                ws.Cells["B5:B8"].Style.Font.Size = 11;

                // Section 3
                ws.Cells["K2:M2"].Merge = true;
                ws.Cells["K2"].Value = "Mẫu số: 01- 1/GTGT";

                ws.Cells["K3:M3"].Merge = true;
                ws.Cells["K3"].Value = "(Ban hành kèm theo Thông tư số";

                ws.Cells["K4:M4"].Merge = true;
                ws.Cells["K4"].Value = "119/2014/TT-BTC ngày 25/08/2014";

                ws.Cells["K5:M5"].Merge = true;
                ws.Cells["K5"].Value = "của Bộ Tài chính)";

                ws.Cells["K2:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K2:M5"].Style.Font.Size = 11;
                ws.Cells["K2:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                ws.Cells["K10:M10"].Merge = true;
                ws.Cells["K10"].Value = "Đơn vị tiền: đồng Việt Nam";
                ws.Cells["K10"].Style.Font.Size = 11;

                // Section 4 
                ws.Cells["B12"].Value = "STT";
                ws.Cells["B12:B13"].Merge = true;

                ws.Cells["C12"].Value = "Hóa đơn, chứng từ bán";
                ws.Cells["C12:G12"].Merge = true;

                ws.Cells["C13"].Value = "Mã hóa đơn";

                ws.Cells["D13"].Value = "Ký hiệu mẫu hóa đơn";

                ws.Cells["E13"].Value = "Ký hiệu hoá đơn";

                ws.Cells["F13"].Value = "Số hóa đơn";

                ws.Cells["G13"].Value = "Ngày, tháng, năm phát hành";

                ws.Cells["H12"].Value = "Tên người mua";
                ws.Cells["H12:H13"].Merge = true;

                ws.Cells["I12"].Value = "Mã số thuế người mua";
                ws.Cells["I12:I13"].Merge = true;

                ws.Cells["J12"].Value = "Mặt hàng";
                ws.Cells["J12:J13"].Merge = true;

                ws.Cells["K12"].Value = "Doanh số bán chưa có thuế";
                ws.Cells["K12:K13"].Merge = true;

                ws.Cells["L12"].Value = "Thuế GTGT";
                ws.Cells["L12:L13"].Merge = true;

                ws.Cells["M12"].Value = "Ghi chú";
                ws.Cells["M12:M13"].Merge = true;

                ws.Cells["B12:M13"].Style.Font.Size = 11;
                ws.Cells["B12:M13"].Style.Font.Bold = true;
                ws.Cells["B12:M13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["B14"].Value = "[1]";
                ws.Cells["C14"].Value = "[2]";
                ws.Cells["D14"].Value = "[3]";
                ws.Cells["E14"].Value = "[4]";
                ws.Cells["F14"].Value = "[5]";
                ws.Cells["G14"].Value = "[6]";
                ws.Cells["H14"].Value = "[7]";
                ws.Cells["I14"].Value = "[8]";
                ws.Cells["J14"].Value = "[9]";
                ws.Cells["K14"].Value = "[10]";
                ws.Cells["L14"].Value = "[11]";
                ws.Cells["M14"].Value = "[12]";
                ws.Cells["B14:M14"].Style.Font.Italic = true;
                ws.Cells["B14:M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Section 5
                decimal totalAmount = 0;
                decimal totalTaxAmount = 0;
                var index = 16; // danh sách sản phẩm từ dòng thứ 16
                if (objUser.USINGINVOICETYPE != (int)AccountObjectType.HOADONBANHANG)
                {
                    var idx = 1;
                    ws.Cells["B15"].Value = "1. Hàng hoá, dịch vụ không chịu thuế GTGT:";
                    //Lấy danh sách sản phẩm của từng hóa đơn
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == -1)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = 0;
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 6
                    ws.Cells["B" + index].Value = "2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                            if (temProduct.TAXRATE == 0)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = 0;
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }

                    idx = 1;
                    // Section 7
                    ws.Cells["B" + index].Value = "3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 5)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }

                    idx = 1;
                    // Section 8
                    ws.Cells["B" + index].Value = "4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 10)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                }
                else
                {
                    var idx = 1;
                    ws.Cells["B15"].Value = "1. Hàng hoá, dịch vụ không chịu thuế GTGT hoặc hàng hóa dịch vụ chịu thuế suất 0%:";
                    //Lấy danh sách sản phẩm của từng hóa đơn
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 0)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = 0;
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 6
                    ws.Cells["B" + index].Value = "2. Phân phối, cung cấp hàng hóa áp dụng thuế suất 1%";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                            if (temProduct.TAXRATE == 1)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 7
                    ws.Cells["B" + index].Value = "3. Dịch vụ, xây dựng không bao thầu nguyên vật liệu áp dụng thuế suất 5%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 5)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 8
                    ws.Cells["B" + index].Value = "4. Sản xuất, vận tải, dịch vụ có gắn với hàng hóa, xây dựng có bao thầu nguyên vật liệu áp dụng thuế suất 3%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 3)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    // Section 8.1
                    ws.Cells["B" + index].Value = "5. Hoạt động kinh doanh khác áp dụng thuế suất 2%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 2)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                }

                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Section 9
                ws.Cells["B" + ++index].Value = "Tổng doanh thu hàng hoá, dịch vụ bán ra chịu thuế GTGT (*): " + totalAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra (**): " + totalTaxAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tôi cam đoan số liệu khai trên là đúng và chịu trách nhiệm trước pháp luật về những số liệu đã khai./... ";
                index = index + 3;
                // Section 10
                ws.Cells["B" + ++index].Value = "NHÂN VIÊN ĐẠI LÝ THUẾ";
                ws.Cells["K" + index].Value = $" , ngày {DateTime.Now.Day.ToString("D2")} tháng {DateTime.Now.Month.ToString("D2")} năm {DateTime.Now.Year}";
                ws.Cells["B" + ++index].Value = "Họ và tên: ";
                ws.Cells["K" + index].Value = "NGƯỜI NỘP THUẾ hoặc";
                ws.Cells["B" + ++index].Value = "Chứng chỉ hành nghề số:";
                ws.Cells["K" + index].Value = "ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ";
                ws.Cells["B" + ++index].Value = "Ghi chú:";
                ws.Cells["K" + index].Value = "Ký, ghi rõ họ tên, chức vụ và đóng dấu (nếu có)";
                ws.Cells["B" + ++index].Value = "(*) Tổng doanh thu hàng hóa, dịch vụ bán ra chịu thuế GTGT là tổng cộng số liệu tại cột 6 của dòng tổng của các chỉ tiêu 2, 3, 4.";
                ws.Cells["B" + index].Value = "(**) Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra là tổng cộng số liệu tại cột 7 của dòng tổng của các chỉ tiêu 2, 3, 4.";

                Download(ex);
            }
            catch (Exception except)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", except, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
            finally
            {
                Session.Remove("ReportUsingInvoice");
                Session.Remove("OutputStartDate");
                Session.Remove("OutputEndDate");
            }
        }

        /// <summary>
        /// Bảng kế hóa đơn đầu ra không có chi tiết hàng hóa
        /// </summary>
        public void DownloadOutputInvoiceExcelWithoutProduct()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                DateTime startDate = (DateTime)Session["OutputStartDate"];
                DateTime endDate = (DateTime)Session["OutputEndDate"];
                InvoiceNumberBO formSearch = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.GetOutputInvoiceExcel(formSearch);

                ExcelPackage ex = new ExcelPackage();
                ExcelWorksheet ws = ex.Workbook.Worksheets.Add("Bang_ke_hoa_don_dau_ra");
                //ws.Column(3).Width = 50;

                // Section 1
                ws.Cells["E2:J2"].Merge = true;
                ws.Cells["E2"].Value = "BẢNG KÊ HOÁ ĐƠN, CHỨNG TỪ HÀNG HOÁ, DỊCH VỤ BÁN RA";
                ws.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E2"].Style.Font.Bold = true;
                ws.Cells["E2"].Style.Font.Size = 11;

                ws.Cells["E3:J3"].Merge = true;
                ws.Cells["E3"].Value = "(Kèm theo tờ khai thuế GTGT theo mẫu số 01/GTGT)";
                ws.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E3"].Style.Font.Italic = true;
                ws.Cells["E3"].Style.Font.Size = 11;

                ws.Cells["E4:J4"].Merge = true;
                ws.Cells["E4"].Value = $"Kỳ tính thuế: Từ ngày {startDate.ToString("dd/MM/yyyy")} đến ngày {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E4"].Style.Font.Size = 11;


                // Section 2
                ws.Cells["B5"].Value = "Tên người nộp thuế: " + objUser.COMNAME;
                ws.Cells["B5"].Style.Font.Bold = true;
                ws.Cells["B6"].Value = "Mã số thuế: " + objUser.COMTAXCODE;
                ws.Cells["B7"].Value = "Tên đại lý thuế(nếu có):";
                ws.Cells["B8"].Value = "Mã số thuế:";
                ws.Cells["B5:B8"].Style.Font.Size = 11;

                // Section 3
                ws.Cells["K2:M2"].Merge = true;
                ws.Cells["K2"].Value = "Mẫu số: 01- 1/GTGT";

                ws.Cells["K3:M3"].Merge = true;
                ws.Cells["K3"].Value = "(Ban hành kèm theo Thông tư số";

                ws.Cells["K4:M4"].Merge = true;
                ws.Cells["K4"].Value = "119/2014/TT-BTC ngày 25/08/2014";

                ws.Cells["K5:M5"].Merge = true;
                ws.Cells["K5"].Value = "của Bộ Tài chính)";

                ws.Cells["K2:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K2:M5"].Style.Font.Size = 11;
                ws.Cells["K2:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                ws.Cells["K10:M10"].Merge = true;
                ws.Cells["K10"].Value = "Đơn vị tiền: đồng Việt Nam";
                ws.Cells["K10"].Style.Font.Size = 11;

                // Section 4 
                ws.Cells["B12"].Value = "STT";
                ws.Cells["B12:B13"].Merge = true;

                ws.Cells["C12"].Value = "Hóa đơn, chứng từ bán";
                ws.Cells["C12:G12"].Merge = true;

                ws.Cells["C13"].Value = "Mã hóa đơn";

                ws.Cells["D13"].Value = "Ký hiệu mẫu hóa đơn";

                ws.Cells["E13"].Value = "Ký hiệu hoá đơn";

                ws.Cells["F13"].Value = "Số hóa đơn";

                ws.Cells["G13"].Value = "Ngày, tháng, năm phát hành";

                ws.Cells["H12"].Value = "Tên người mua";
                ws.Cells["H12:H13"].Merge = true;

                ws.Cells["I12"].Value = "Mã số thuế người mua";
                ws.Cells["I12:I13"].Merge = true;

                ws.Cells["J12"].Value = "Mặt hàng";
                ws.Cells["J12:J13"].Merge = true;

                ws.Cells["K12"].Value = "Doanh số bán chưa có thuế";
                ws.Cells["K12:K13"].Merge = true;

                ws.Cells["L12"].Value = "Thuế GTGT";
                ws.Cells["L12:L13"].Merge = true;

                ws.Cells["M12"].Value = "Ghi chú";
                ws.Cells["M12:M13"].Merge = true;

                ws.Cells["B12:M13"].Style.Font.Size = 11;
                ws.Cells["B12:M13"].Style.Font.Bold = true;
                ws.Cells["B12:M13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:M13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["B14"].Value = "[1]";
                ws.Cells["C14"].Value = "[2]";
                ws.Cells["D14"].Value = "[3]";
                ws.Cells["E14"].Value = "[4]";
                ws.Cells["F14"].Value = "[5]";
                ws.Cells["G14"].Value = "[6]";
                ws.Cells["H14"].Value = "[7]";
                ws.Cells["I14"].Value = "[8]";
                ws.Cells["J14"].Value = "[9]";
                ws.Cells["K14"].Value = "[10]";
                ws.Cells["L14"].Value = "[11]";
                ws.Cells["M14"].Value = "[12]";
                ws.Cells["B14:M14"].Style.Font.Italic = true;
                ws.Cells["B14:M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                ws.Column(13).Style.WrapText = true;
                ws.Column(13).Width = 25;
                // Section 5
                var listResultTotalTaxEqualZero = result.Where(x => x.TAXMONEY == 0).ToList();
                var listResultTotalTaxGreaterThanZero = result.Where(x => x.TAXMONEY > 0).ToList();

                decimal totalAmount = 0;
                decimal totalTaxAmount = 0;
                var index = 16; // danh sách sản phẩm từ dòng thứ 16
                if (objUser.USINGINVOICETYPE != (int)AccountObjectType.HOADONBANHANG)
                {
                    var listNo = new ConcurrentBag<InvoiceBO>();
                    var listZero = new ConcurrentBag<InvoiceBO>();
                    var listFive = new ConcurrentBag<InvoiceBO>();
                    var listTen = new ConcurrentBag<InvoiceBO>();
                    var tasks = new ConcurrentBag<Task>();

                    foreach (InvoiceBO tempInvoice in listResultTotalTaxEqualZero)
                    {
                        var t = new Task(() =>
                        {
                            var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                            switch (lstProduct[0].TAXRATE)
                            {
                                case (int)TAX_TYPE.NO:
                                    listNo.Add(tempInvoice); break;
                                case (int)TAX_TYPE.ZERO:
                                    listZero.Add(tempInvoice); break;
                                default:
                                    break;
                            }
                        });
                        tasks.Add(t);
                        t.ConfigureAwait(false);
                        t.Start();
                    }

                    foreach (InvoiceBO tempInvoice in listResultTotalTaxGreaterThanZero)
                    {
                        var invoiceTaxRate = Math.Round((tempInvoice.TAXMONEY / tempInvoice.TOTALMONEY) * 100);
                        var t = new Task(() =>
                        {
                            switch (invoiceTaxRate)
                            {
                                case (int)TAX_TYPE.FIVE:
                                    listFive.Add(tempInvoice); break;
                                case (int)TAX_TYPE.TEN:
                                    listTen.Add(tempInvoice); break;
                                default:
                                    break;
                            }
                        });
                        tasks.Add(t);
                        t.ConfigureAwait(false);
                        t.Start();
                    }

                    Task.WaitAll(tasks.ToArray());

                    var idx = 1;
                    ws.Cells["B15"].Value = "1. Hàng hoá, dịch vụ không chịu thuế GTGT:";
                    //Lấy danh sách sản phẩm của từng hóa đơn
                    foreach (InvoiceBO tempInvoice in listNo.OrderBy(x => x.NUMBER))
                    {
                        ws.Cells[string.Format("B{0}", index)].Value = idx;
                        ws.Cells[string.Format("C{0}", index)].Value = "";
                        ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                        ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                        ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                        ws.Cells[string.Format("J{0}", index)].Value = "";
                        ws.Cells[string.Format("K{0}", index)].Value = Math.Round(Math.Abs(tempInvoice.TOTALMONEY) * tempInvoice.EXCHANGERATE);
                        ws.Cells[string.Format("L{0}", index)].Value = 0;
                        ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "") + "\n" + tempInvoice.CUSPAYMENTMETHOD;
                        index++;
                        idx++;
                    }
                    idx = 1;
                    // Section 6
                    ws.Cells["B" + index].Value = "2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in listZero.OrderBy(x => x.NUMBER))
                    {
                        var totalMoney = Math.Round(Math.Abs(tempInvoice.TOTALMONEY) * tempInvoice.EXCHANGERATE);

                        ws.Cells[string.Format("B{0}", index)].Value = idx;
                        ws.Cells[string.Format("C{0}", index)].Value = "";
                        ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                        ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                        ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                        ws.Cells[string.Format("J{0}", index)].Value = "";
                        ws.Cells[string.Format("K{0}", index)].Value = totalMoney;
                        ws.Cells[string.Format("L{0}", index)].Value = 0;
                        ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "") + "\n" + tempInvoice.CUSPAYMENTMETHOD;

                        totalAmount += totalMoney;
                        index++;
                        idx++;
                    }

                    idx = 1;
                    // Section 7
                    ws.Cells["B" + index].Value = "3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in listFive.OrderBy(x => x.NUMBER))
                    {
                        var totalMoney = Math.Round(Math.Abs(tempInvoice.TOTALMONEY) * tempInvoice.EXCHANGERATE);
                        var totalTax = Math.Round(Math.Abs(tempInvoice.TAXMONEY) * tempInvoice.EXCHANGERATE);

                        ws.Cells[string.Format("B{0}", index)].Value = idx;
                        ws.Cells[string.Format("C{0}", index)].Value = "";
                        ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                        ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                        ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                        ws.Cells[string.Format("J{0}", index)].Value = "";
                        ws.Cells[string.Format("K{0}", index)].Value = totalMoney;
                        ws.Cells[string.Format("L{0}", index)].Value = totalTax;
                        ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "") + "\n" + tempInvoice.CUSPAYMENTMETHOD;

                        totalAmount += totalMoney;
                        totalTaxAmount += totalTax;
                        index++;
                        idx++;
                    }

                    idx = 1;
                    // Section 8
                    ws.Cells["B" + index].Value = "4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in listTen.OrderBy(x => x.NUMBER))
                    {
                        var totalMoney = Math.Round(Math.Abs(tempInvoice.TOTALMONEY) * tempInvoice.EXCHANGERATE);
                        var totalTax = Math.Round(Math.Abs(tempInvoice.TAXMONEY) * tempInvoice.EXCHANGERATE);

                        ws.Cells[string.Format("B{0}", index)].Value = idx;
                        ws.Cells[string.Format("C{0}", index)].Value = "";
                        ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                        ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                        ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                        ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                        ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                        ws.Cells[string.Format("J{0}", index)].Value = "";
                        ws.Cells[string.Format("K{0}", index)].Value = Math.Round(Math.Abs(tempInvoice.TOTALMONEY) * tempInvoice.EXCHANGERATE);
                        ws.Cells[string.Format("L{0}", index)].Value = Math.Round(Math.Abs(tempInvoice.TAXMONEY) * tempInvoice.EXCHANGERATE);
                        ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "") + "\n" + tempInvoice.CUSPAYMENTMETHOD;

                        totalAmount += totalMoney;
                        totalTaxAmount += totalTax;
                        index++;
                        idx++;
                    }
                }
                else
                {
                    var idx = 1;
                    ws.Cells["B15"].Value = "1. Hàng hoá, dịch vụ không chịu thuế GTGT hoặc hàng hóa dịch vụ chịu thuế suất 0%:";
                    //Lấy danh sách sản phẩm của từng hóa đơn
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 0)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = 0;
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 6
                    ws.Cells["B" + index].Value = "2. Phân phối, cung cấp hàng hóa áp dụng thuế suất 1%";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                            if (temProduct.TAXRATE == 1)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 7
                    ws.Cells["B" + index].Value = "3. Dịch vụ, xây dựng không bao thầu nguyên vật liệu áp dụng thuế suất 5%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 5)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    idx = 1;
                    // Section 8
                    ws.Cells["B" + index].Value = "4. Sản xuất, vận tải, dịch vụ có gắn với hàng hóa, xây dựng có bao thầu nguyên vật liệu áp dụng thuế suất 3%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 3)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                    // Section 8.1
                    ws.Cells["B" + index].Value = "5. Hoạt động kinh doanh khác áp dụng thuế suất 2%:";
                    index = index + 1;
                    foreach (InvoiceBO tempInvoice in result)
                    {
                        var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                        for (int j = 0; j < lstProduct.Count; j++)
                        {
                            var temProduct = lstProduct[j];
                            var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                            if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                            {
                                amount = temProduct.TOTALMONEY;
                            }
                            var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;

                            if (temProduct.TAXRATE == 2)
                            {
                                ws.Cells[string.Format("B{0}", index)].Value = idx;
                                ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                                ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                                ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                                ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                                ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                                ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                                ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                                ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                                ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                                ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                                ws.Cells[string.Format("M{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                                totalAmount += amount;
                                totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                                index++;
                                idx++;
                            }
                        }
                    }
                }

                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:M{0}", index)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Section 9
                ws.Cells["B" + ++index].Value = "Tổng doanh thu hàng hoá, dịch vụ bán ra chịu thuế GTGT (*): " + totalAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra (**): " + totalTaxAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tôi cam đoan số liệu khai trên là đúng và chịu trách nhiệm trước pháp luật về những số liệu đã khai./... ";
                index = index + 3;
                // Section 10
                ws.Cells["B" + ++index].Value = "NHÂN VIÊN ĐẠI LÝ THUẾ";
                ws.Cells["K" + index].Value = $" , ngày {DateTime.Now.Day.ToString("D2")} tháng {DateTime.Now.Month.ToString("D2")} năm {DateTime.Now.Year}";
                ws.Cells["B" + ++index].Value = "Họ và tên: ";
                ws.Cells["K" + index].Value = "NGƯỜI NỘP THUẾ hoặc";
                ws.Cells["B" + ++index].Value = "Chứng chỉ hành nghề số:";
                ws.Cells["K" + index].Value = "ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ";
                ws.Cells["B" + ++index].Value = "Ghi chú:";
                ws.Cells["K" + index].Value = "Ký, ghi rõ họ tên, chức vụ và đóng dấu (nếu có)";
                ws.Cells["B" + ++index].Value = "(*) Tổng doanh thu hàng hóa, dịch vụ bán ra chịu thuế GTGT là tổng cộng số liệu tại cột 6 của dòng tổng của các chỉ tiêu 2, 3, 4.";
                ws.Cells["B" + index].Value = "(**) Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra là tổng cộng số liệu tại cột 7 của dòng tổng của các chỉ tiêu 2, 3, 4.";

                Download(ex);
            }
            catch (Exception except)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", except, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
            finally
            {
                Session.Remove("ReportUsingInvoice");
                Session.Remove("OutputStartDate");
                Session.Remove("OutputEndDate");
            }
        }

        public void DownloadOutputWaterInvoiceExcel()
        {
            try
            {
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                DateTime startDate = (DateTime)Session["OutputStartDate"];
                DateTime endDate = (DateTime)Session["OutputEndDate"];
                InvoiceNumberBO formSearch = new InvoiceNumberBO
                {
                    COMTAXCODE = objUser.COMTAXCODE,
                    FROMTIME = DateTime.ParseExact(startDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TOTIME = DateTime.ParseExact(endDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                };
                NumberBLL numberBLL = new NumberBLL();
                var result = numberBLL.GetOutputInvoiceExcel(formSearch).Where(x => x.USINGINVOICETYPE == (int)DS.Common.Enums.EnumHelper.AccountObjectType.HOADONTIENNUOC);

                ExcelPackage ex = new ExcelPackage();
                ExcelWorksheet ws = ex.Workbook.Worksheets.Add("Bang_ke_hoa_don_dau_ra");
                //ws.Column(3).Width = 50;

                // Section 1
                ws.Cells["E2:J2"].Merge = true;
                ws.Cells["E2"].Value = "BẢNG KÊ HOÁ ĐƠN, CHỨNG TỪ HÀNG HOÁ, DỊCH VỤ BÁN RA";
                ws.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E2"].Style.Font.Bold = true;
                ws.Cells["E2"].Style.Font.Size = 11;

                ws.Cells["E3:J3"].Merge = true;
                ws.Cells["E3"].Value = "(Kèm theo tờ khai thuế GTGT theo mẫu số 01/GTGT)";
                ws.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E3"].Style.Font.Italic = true;
                ws.Cells["E3"].Style.Font.Size = 11;

                ws.Cells["E4:J4"].Merge = true;
                ws.Cells["E4"].Value = $"Kỳ tính thuế: Từ ngày {startDate.ToString("dd/MM/yyyy")} đến ngày {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E4"].Style.Font.Size = 11;


                // Section 2
                ws.Cells["B5"].Value = "Tên người nộp thuế: " + objUser.COMNAME;
                ws.Cells["B5"].Style.Font.Bold = true;
                ws.Cells["B6"].Value = "Mã số thuế: " + objUser.COMTAXCODE;
                ws.Cells["B7"].Value = "Tên đại lý thuế(nếu có):";
                ws.Cells["B8"].Value = "Mã số thuế:";
                ws.Cells["B5:B8"].Style.Font.Size = 11;

                // Section 3
                ws.Cells["K2:M2"].Merge = true;
                ws.Cells["K2"].Value = "Mẫu số: 01- 1/GTGT";

                ws.Cells["K3:M3"].Merge = true;
                ws.Cells["K3"].Value = "(Ban hành kèm theo Thông tư số";

                ws.Cells["K4:M4"].Merge = true;
                ws.Cells["K4"].Value = "119/2014/TT-BTC ngày 25/08/2014";

                ws.Cells["K5:M5"].Merge = true;
                ws.Cells["K5"].Value = "của Bộ Tài chính)";

                ws.Cells["K2:M5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K2:M5"].Style.Font.Size = 11;
                ws.Cells["K2:M5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                ws.Cells["K10:M10"].Merge = true;
                ws.Cells["K10"].Value = "Đơn vị tiền: đồng Việt Nam";
                ws.Cells["K10"].Style.Font.Size = 11;

                // Section 4 
                ws.Cells["B12"].Value = "STT";
                ws.Cells["B12:B13"].Merge = true;

                ws.Cells["C12"].Value = "Hóa đơn, chứng từ bán";
                ws.Cells["C12:G12"].Merge = true;

                ws.Cells["C13"].Value = "Mã hóa đơn";

                ws.Cells["D13"].Value = "Ký hiệu mẫu hóa đơn";

                ws.Cells["E13"].Value = "Ký hiệu hoá đơn";

                ws.Cells["F13"].Value = "Số hóa đơn";

                ws.Cells["G13"].Value = "Ngày, tháng, năm phát hành";

                ws.Cells["H12"].Value = "Tên người mua";
                ws.Cells["H12:H13"].Merge = true;

                ws.Cells["I12"].Value = "Mã số thuế người mua";
                ws.Cells["I12:I13"].Merge = true;

                ws.Cells["J12"].Value = "Mặt hàng";
                ws.Cells["J12:J13"].Merge = true;

                ws.Cells["K12"].Value = "Doanh số bán chưa có thuế";
                ws.Cells["K12:K13"].Merge = true;

                ws.Cells["L12"].Value = "Thuế GTGT";
                ws.Cells["L12:L13"].Merge = true;

                ws.Cells["M12"].Value = "Phí BVMT (%)";
                ws.Cells["M12:M13"].Merge = true;

                ws.Cells["N12"].Value = "Tổng tiền phí BVMT";
                ws.Cells["N12:N13"].Merge = true;

                ws.Cells["O12"].Value = "Ghi chú";
                ws.Cells["O12:O13"].Merge = true;

                ws.Cells["B12:O13"].Style.Font.Size = 11;
                ws.Cells["B12:O13"].Style.Font.Bold = true;
                ws.Cells["B12:O13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:O13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:O13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:O13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B12:O13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["B14"].Value = "[1]";
                ws.Cells["C14"].Value = "[2]";
                ws.Cells["D14"].Value = "[3]";
                ws.Cells["E14"].Value = "[4]";
                ws.Cells["F14"].Value = "[5]";
                ws.Cells["G14"].Value = "[6]";
                ws.Cells["H14"].Value = "[7]";
                ws.Cells["I14"].Value = "[8]";
                ws.Cells["J14"].Value = "[9]";
                ws.Cells["K14"].Value = "[10]";
                ws.Cells["L14"].Value = "[11]";
                ws.Cells["M14"].Value = "[12]";
                ws.Cells["N14"].Value = "[13]";
                ws.Cells["O14"].Value = "[14]";
                ws.Cells["B14:O14"].Style.Font.Italic = true;
                ws.Cells["B14:O14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Section 5
                decimal totalAmount = 0;
                decimal totalTaxAmount = 0;
                decimal totalWaterTaxAmount = 0;
                var index = 16; // danh sách sản phẩm từ dòng thứ 16
                var idx = 1;
                ws.Cells["B15"].Value = "1. Hàng hoá, dịch vụ không chịu thuế GTGT:";
                //Lấy danh sách sản phẩm của từng hóa đơn
                foreach (InvoiceBO tempInvoice in result)
                {
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                        if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                        {
                            amount = temProduct.TOTALMONEY;
                        }
                        var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                        var waterTaxAmount = (temProduct.TAXRATEWATER * Math.Abs(amount)) / 100;
                        if (temProduct.TAXRATE == -1)
                        {
                            ws.Cells[string.Format("B{0}", index)].Value = idx;
                            ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                            ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                            ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                            ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                            ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                            ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                            ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                            ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                            ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                            ws.Cells[string.Format("L{0}", index)].Value = 0;
                            ws.Cells[string.Format("M{0}", index)].Value = temProduct.TAXRATEWATER;
                            ws.Cells[string.Format("N{0}", index)].Value = waterTaxAmount;
                            ws.Cells[string.Format("O{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");

                            totalWaterTaxAmount += waterTaxAmount;
                            index++;
                            idx++;
                        }
                    }
                }
                idx = 1;
                // Section 6
                ws.Cells["B" + index].Value = "2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                index = index + 1;
                foreach (InvoiceBO tempInvoice in result)
                {
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                        if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                        {
                            amount = temProduct.TOTALMONEY;
                        }
                        var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                        var waterTaxAmount = (temProduct.TAXRATEWATER * Math.Abs(amount)) / 100;
                        if (temProduct.TAXRATE == 0)
                        {
                            ws.Cells[string.Format("B{0}", index)].Value = idx;
                            ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                            ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                            ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                            ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                            ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                            ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                            ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                            ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                            ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                            ws.Cells[string.Format("L{0}", index)].Value = 0;
                            ws.Cells[string.Format("M{0}", index)].Value = temProduct.TAXRATEWATER;
                            ws.Cells[string.Format("N{0}", index)].Value = waterTaxAmount;
                            ws.Cells[string.Format("O{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                            totalAmount += amount;
                            totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                            totalWaterTaxAmount += waterTaxAmount;
                            index++;
                            idx++;
                        }
                    }
                }

                idx = 1;
                // Section 7
                ws.Cells["B" + index].Value = "3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                index = index + 1;
                foreach (InvoiceBO tempInvoice in result)
                {
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                        if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                        {
                            amount = temProduct.TOTALMONEY;
                        }
                        var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                        var waterTaxAmount = (temProduct.TAXRATEWATER * Math.Abs(amount)) / 100;
                        if (temProduct.TAXRATE == 5)
                        {
                            ws.Cells[string.Format("B{0}", index)].Value = idx;
                            ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                            ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                            ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                            ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                            ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                            ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                            ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                            ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                            ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                            ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                            ws.Cells[string.Format("M{0}", index)].Value = temProduct.TAXRATEWATER;
                            ws.Cells[string.Format("N{0}", index)].Value = waterTaxAmount;
                            ws.Cells[string.Format("O{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                            totalAmount += amount;
                            totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                            totalWaterTaxAmount += waterTaxAmount;
                            index++;
                            idx++;
                        }
                    }
                }

                idx = 1;
                // Section 8
                ws.Cells["B" + index].Value = "4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                index = index + 1;
                foreach (InvoiceBO tempInvoice in result)
                {
                    var lstProduct = invoiceBLL.GetInvoiceDetail(tempInvoice.ID);
                    for (int j = 0; j < lstProduct.Count; j++)
                    {
                        var temProduct = lstProduct[j];
                        var amount = tempInvoice.INVOICETYPE == (int)INVOICE_TYPE.CANCEL ? 0 : (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? temProduct.RETAILPRICE * temProduct.QUANTITY * (-1) : temProduct.RETAILPRICE * temProduct.QUANTITY);
                        if (temProduct.RETAILPRICE == 0 && temProduct.QUANTITY == 0)
                        {
                            amount = temProduct.TOTALMONEY;
                        }
                        var taxRate = temProduct.TAXRATE == (int)TAX_TYPE.NO ? 0 : temProduct.TAXRATE;
                        var waterTaxAmount = (temProduct.TAXRATEWATER * Math.Abs(amount)) / 100;
                        if (temProduct.TAXRATE == 10)
                        {
                            ws.Cells[string.Format("B{0}", index)].Value = idx;
                            ws.Cells[string.Format("C{0}", index)].Value = "01GTKT";
                            ws.Cells[string.Format("D{0}", index)].Value = tempInvoice.FORMCODE;
                            ws.Cells[string.Format("E{0}", index)].Value = tempInvoice.SYMBOLCODE;
                            ws.Cells[string.Format("F{0}", index)].Value = tempInvoice.NUMBER.ToString("D7");
                            ws.Cells[string.Format("G{0}", index)].Value = tempInvoice.SIGNEDTIME.ToString("dd/MM/yyyy");
                            ws.Cells[string.Format("H{0}", index)].Value = tempInvoice.CUSNAME;
                            ws.Cells[string.Format("I{0}", index)].Value = tempInvoice.CUSTAXCODE;
                            ws.Cells[string.Format("J{0}", index)].Value = temProduct.PRODUCTNAME;
                            ws.Cells[string.Format("K{0}", index)].Value = Math.Abs(amount);
                            ws.Cells[string.Format("L{0}", index)].Value = (Math.Abs(amount) * (decimal)(taxRate) / 100);
                            ws.Cells[string.Format("M{0}", index)].Value = temProduct.TAXRATEWATER;
                            ws.Cells[string.Format("N{0}", index)].Value = waterTaxAmount;
                            ws.Cells[string.Format("O{0}", index)].Value = tempInvoice.INVOICETYPENAME + (tempInvoice.INVOICEMETHOD == (int)MODIFIED_INVOICE_METHOD_TYPE.DECREASE ? " giảm" : "");
                            totalAmount += amount;
                            totalTaxAmount += (amount * (decimal)(taxRate) / 100);
                            totalWaterTaxAmount += waterTaxAmount;
                            index++;
                            idx++;
                        }
                    }
                }

                ws.Cells[string.Format("B14:O{0}", index)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:O{0}", index)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:O{0}", index)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B14:O{0}", index)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Section 9
                ws.Cells["B" + ++index].Value = "Tổng doanh thu hàng hoá, dịch vụ bán ra chịu thuế GTGT (*): " + totalAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra (**): " + totalTaxAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tổng số phí bảo vệ môi trường: " + totalWaterTaxAmount.ToString("#,##0.00");
                ws.Cells["B" + ++index].Value = "Tôi cam đoan số liệu khai trên là đúng và chịu trách nhiệm trước pháp luật về những số liệu đã khai./... ";
                index = index + 3;
                // Section 10
                ws.Cells["B" + ++index].Value = "NHÂN VIÊN ĐẠI LÝ THUẾ";
                ws.Cells["K" + index].Value = $" , ngày {DateTime.Now.Day.ToString("D2")} tháng {DateTime.Now.Month.ToString("D2")} năm {DateTime.Now.Year}";
                ws.Cells["B" + ++index].Value = "Họ và tên: ";
                ws.Cells["K" + index].Value = "NGƯỜI NỘP THUẾ hoặc";
                ws.Cells["B" + ++index].Value = "Chứng chỉ hành nghề số:";
                ws.Cells["K" + index].Value = "ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ";
                ws.Cells["B" + ++index].Value = "Ghi chú:";
                ws.Cells["K" + index].Value = "Ký, ghi rõ họ tên, chức vụ và đóng dấu (nếu có)";
                ws.Cells["B" + ++index].Value = "(*) Tổng doanh thu hàng hóa, dịch vụ bán ra chịu thuế GTGT là tổng cộng số liệu tại cột 6 của dòng tổng của các chỉ tiêu 2, 3, 4.";
                ws.Cells["B" + index].Value = "(**) Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra là tổng cộng số liệu tại cột 7 của dòng tổng của các chỉ tiêu 2, 3, 4.";

                Download(ex);
            }
            catch (Exception except)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", except, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
            finally
            {
                Session.Remove("ReportUsingInvoice");
                Session.Remove("OutputStartDate");
                Session.Remove("OutputEndDate");
            }
        }

        public void Download(ExcelPackage ex)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=Bang_Ke_Hoa_Don_Dau_Ra.xlsx");
            Response.BinaryWrite(ex.GetAsByteArray());
            Response.End();
        }

        /// <summary>
        /// Danh sách email theo mã số thuế doanh nghiệp
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemPerPage"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult GetEmailHistoryByComtaxcode(FormSearchEmail form, int itemPerPage, int currentPage)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-365).Date;
                DateTime toDate = DateTime.Now.Date;
                if (form.TIME != null)
                {
                    string[] d = form.TIME.Split(';');
                    fromDate = DateTime.ParseExact(d[0], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                    toDate = DateTime.ParseExact(d[1], "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
                }
                form.FROMDATE = string.IsNullOrEmpty(fromDate.ToString("dd/MM/yyyy")) ? new DateTime(2000, 1, 1) : DateTime.ParseExact(fromDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.TODATE = string.IsNullOrEmpty(toDate.ToString("dd/MM/yyyy")) ? DateTime.Now : DateTime.ParseExact(toDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                form.ITEMPERPAGE = itemPerPage;
                form.CURRENTPAGE = currentPage;
                form.COMTAXCODE = objUser.COMTAXCODE;
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;

                EmailBLL emailBLL = new EmailBLL();
                var result = emailBLL.GetEmailHistoryByComtaxcode(form);

                long TotalPages = 0;
                var TotalRow = result.Count == 0 ? 0 : result[0].TOTALROW;
                if (TotalRow % form.ITEMPERPAGE == 0)
                    TotalPages = TotalRow == 0 ? 1 : TotalRow / form.ITEMPERPAGE.Value;
                else
                    TotalPages = TotalRow / form.ITEMPERPAGE.Value + 1;

                if (emailBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(emailBLL.ResultMessageBO.Message, emailBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoiceNumerWaiting");
                    return Json(new { rs = false, msg = emailBLL.ResultMessageBO.Message });
                }
                return Json(new { rs = true, TotalPages, TotalRow, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi không lấy được danh sách lịch sử email.", ex, MethodBase.GetCurrentMethod().Name, "GetEmailHistoryByInvoiceId");
                return Json(new { rs = false, msg = $"Lỗi không lấy được danh sách lịch sử email." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Chưa chạy
        public void DownloadInvoiceStatisticExcel()
        {
            try
            {
                ExcelPackage ex = new ExcelPackage();
                ExcelWorksheet ws = ex.Workbook.Worksheets.Add("BC26_AC");
                ws.Cells.AutoFitColumns();
                ws.Cells.Style.Font.Name = "Arial";
                ws.Cells.Style.Font.Size = 8;


                for (int j = 1; j <= 15; j++)
                {
                    ws.Row(j).Hidden = true;
                }

                // set width for column
                ws.Column(2).Width = 5;
                ws.Column(3).Width = 7;
                ws.Column(4).Width = 50;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 15;
                ws.Column(7).Width = 15;
                ws.Column(8).Width = 15;
                ws.Column(9).Width = 15;
                ws.Column(10).Width = 15;
                ws.Column(11).Width = 15;
                ws.Column(12).Width = 20;
                ws.Column(13).Width = 20;
                ws.Column(14).Width = 20;

                // Row 1
                ws.Cells["E16:L16"].Merge = true;
                ws.Cells["E16"].Value = "BÁO CÁO TÌNH HÌNH SỬ DỤNG HÓA ĐƠN (BC26/AC)";
                ws.Cells["E16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E16"].Style.Font.Bold = true;
                ws.Cells["E16"].Style.Font.Size = 10;

                // Row 3 4 5 6 Column B C D E F
                ws.Cells["B18:B20"].Merge = true;
                ws.Cells["B18"].Value = "STT";
                ws.Cells["B18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C18:C20"].Merge = true;
                ws.Cells["C18"].Value = "Mã loại hóa đơn";
                ws.Cells["C18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D18:D20"].Merge = true;
                ws.Cells["D18"].Value = "Tên loại hóa đơn";
                ws.Cells["D18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["E18:E20"].Merge = true;
                ws.Cells["E18"].Value = "Ký mẫu hiệu hóa đơn";
                ws.Cells["E18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F18:F20"].Merge = true;
                ws.Cells["F18"].Value = "Ký hiệu hóa đơn";
                ws.Cells["F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Row 3 4 5 6 Column G H I J
                ws.Cells["G18:J18"].Merge = true;
                ws.Cells["G18"].Value = "Số tồn đầu kỳ, mua/phát hành trong kỳ";
                ws.Cells["G18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G19:H19"].Merge = true;
                ws.Cells["G19"].Value = "Số tồn đầu kỳ";
                ws.Cells["G19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I19:J19"].Merge = true;
                ws.Cells["I19"].Value = "Số mua/ phát hành trong kỳ";
                ws.Cells["I19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G20"].Value = "Từ số";
                ws.Cells["G20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["H20"].Value = "Đến số";
                ws.Cells["H20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I20"].Value = "Từ số";
                ws.Cells["I20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J20"].Value = "Đến số";
                ws.Cells["J20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Row 14 15 16 17 - Column K L M N
                ws.Cells["K18:N18"].Merge = true;
                ws.Cells["K18"].Value = "Số sử dụng, xoá bỏ, mất, huỷ trong kỳ";
                ws.Cells["K18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["K19:K20"].Merge = true;
                ws.Cells["K19"].Value = "Số lượng đã sử dụng";
                ws.Cells["K19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["L19"].Merge = true;
                ws.Cells["L19"].Value = "Xóa bỏ";
                ws.Cells["L19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["L19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["M19"].Merge = true;
                ws.Cells["M19"].Value = "Mất";
                ws.Cells["M19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["M19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["N19"].Merge = true;
                ws.Cells["N19"].Value = "Hủy";
                ws.Cells["N19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["N19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["L20"].Value = "Số";
                ws.Cells["L20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["L20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["M20"].Value = "Số";
                ws.Cells["M20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["M20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["N20"].Value = "Số";
                ws.Cells["N20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["N20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Row 21
                ws.Cells["B21"].Value = "1";
                ws.Cells["B21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C21"].Value = "";
                ws.Cells["C21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D21"].Value = "2";
                ws.Cells["D21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["E21"].Value = "3";
                ws.Cells["E21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F21"].Value = "4";
                ws.Cells["F21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G21"].Value = "6";
                ws.Cells["G21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["H21"].Value = "7";
                ws.Cells["H21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I21"].Value = "8";
                ws.Cells["I21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J21"].Value = "9";
                ws.Cells["J21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["K21"].Value = "14";
                ws.Cells["K21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["L21"].Value = "15";
                ws.Cells["L21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["L21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["M21"].Value = "17";
                ws.Cells["M21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["M21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["N21"].Value = "19";
                ws.Cells["N21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["N21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Print data table
                int rowStart = 22;
                int i = 1;

                while (rowStart < 20)
                {
                    ws.Cells[string.Format("B{0}", rowStart)].Value = "01GTKT";
                    ws.Cells[string.Format("C{0}", rowStart)].Value = "HÓA ĐƠN GIÁ TRỊ GIA TĂNG	";
                    ws.Cells[string.Format("D{0}", rowStart)].Value = "01GTKT00001";
                    ws.Cells[string.Format("E{0}", rowStart)].Value = "LK/19E	";
                    ws.Cells[string.Format("F{0}", rowStart)].Value = "1000000";
                    ws.Cells[string.Format("G{0}", rowStart)].Value = "1000000";
                    ws.Cells[string.Format("H{0}", rowStart)].Value = "0000001";
                    ws.Cells[string.Format("I{0}", rowStart)].Value = "0100000";
                    ws.Cells[string.Format("J{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("K{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("L{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("M{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("N{0}", rowStart)].Value = "";

                    ws.Cells[string.Format("B{0}:N{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowStart++;
                    i++;
                }

                int lastRow = rowStart;
                // set border for table
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("B18:N{0}", lastRow)].Style.WrapText = true;
                for (int j = 16; j < lastRow; j++)
                {
                    ws.Row(j).Height = 20;
                }
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#9ae88f");
                ws.Cells["B18:N21"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["B18:N21"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                ws.Cells["B18:N21"].Style.Font.Bold = true;


                ExcelWorksheet ws1 = ex.Workbook.Worksheets.Add("Data");
                ws1.Cells.AutoFitColumns();
                ws1.Cells.Style.Font.Name = "Arial";
                ws1.Cells.Style.Font.Size = 8;

                // set width for column
                ws1.Column(2).Width = 20;
                ws1.Column(3).Width = 100;

                // row 1 b c
                ws1.Cells["B1:C1"].Merge = true;
                ws1.Cells["B1"].Value = "Phụ lục 01";
                ws1.Cells["B1:C1"].Style.Font.Size = 14;

                // row 3 b c
                ws1.Cells["B3"].Value = "Mã hóa đơn";

                ws1.Cells["C3"].Value = "Tên hóa đơn";
                ws1.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws1.Cells["B3:C3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws1.Cells["B3:C3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws1.Cells["B3:C3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws1.Cells["B3:C3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // row 4
                ws1.Cells["B4"].Value = "01GTKT";
                ws1.Cells["C4"].Value = "Hóa đơn giá trị gia tăng";
                // row 5
                ws1.Cells["B5"].Value = "01GTKT";
                ws1.Cells["C5"].Value = "Hóa đơn bán hàng";
                // row 6
                ws1.Cells["B6"].Value = "01GTKT";
                ws1.Cells["C6"].Value = "Hóa đơn xuất khẩu";

                for (int j = 1; j <= 19; j++)
                {
                    ws1.Row(j).Height = 20;
                }
                ws1.Cells["B3:C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws1.Cells["B3:C3"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                ws1.Cells["B3:C3"].Style.Font.Bold = true;



                ExcelWorksheet ws2 = ex.Workbook.Worksheets.Add("HDSD");

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xls");
                Response.BinaryWrite(ex.GetAsByteArray());
                Response.End();
            }
            catch (Exception except)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", except, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
        }

        public void DownloadExcel()
        {
            try
            {
                ExcelPackage ex = new ExcelPackage();
                ExcelWorksheet ws = ex.Workbook.Worksheets.Add("UsingInvoice");

                //ws.Column(3).Width = 50;

                // Row 2
                ws.Cells["H2:N2"].Merge = true;
                ws.Cells["H2"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                ws.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H2"].Style.Font.Bold = true;
                ws.Cells["H2"].Style.Font.Size = 12;

                ws.Cells["O2:U2"].Merge = true;
                ws.Cells["O2"].Value = "Mẫu số: 01-1/GTGT";
                ws.Cells["O2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O2"].Style.Font.Bold = true;
                ws.Cells["O2"].Style.Font.Size = 10;

                // Row 3
                ws.Cells["H3:N3"].Merge = true;
                ws.Cells["H3"].Value = "Độc lập - Tự do - Hạnh phúc";
                ws.Cells["H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H3"].Style.Font.Bold = true;
                ws.Cells["H3"].Style.Font.Size = 12;

                ws.Cells["O3:U3"].Merge = true;
                ws.Cells["O3"].Value = "(Ban hành kềm theo Thông tư số 26/2015/TT-BTC";
                ws.Cells["O3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O3"].Style.Font.Size = 10;

                // Row 4
                ws.Cells["H4:N4"].Merge = true;
                ws.Cells["H4"].Value = "- - - - - - - - - - - - - - -";
                ws.Cells["H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H4"].Style.Font.Bold = true;
                ws.Cells["H4"].Style.Font.Size = 12;

                ws.Cells["O4:U4"].Merge = true;
                ws.Cells["O4"].Value = "ngày 27/02/2015 của bộ Tài chính)";
                ws.Cells["O4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O4"].Style.Font.Size = 10;

                // Row 5
                ws.Cells["H5:N5"].Merge = true;
                ws.Cells["H5"].Value = "BÁO CÁO TÌNH HÌNH SỬ DỤNG HÓA ĐƠN (BC26/AC)";
                ws.Cells["H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H5"].Style.Font.Bold = true;
                ws.Cells["H5"].Style.Font.Size = 12;

                // Row 6
                ws.Cells["H6:N6"].Merge = true;
                ws.Cells["H6"].Value = "[01] Kỳ tính thuế quý 2 năm 2019";
                ws.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H6"].Style.Font.Bold = true;
                ws.Cells["H6"].Style.Font.Size = 12;

                // Row 8
                ws.Cells["A8:G8"].Merge = true;
                ws.Cells["A8"].Value = "1: Tên tổ chức: Công ty công nghệ và truyền thông Nova";
                ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A8"].Style.Font.Bold = true;
                ws.Cells["A8"].Style.Font.Size = 10;

                // Row 9
                ws.Cells["A9:G9"].Merge = true;
                ws.Cells["A9"].Value = "2: Mã số thuế: 0968643354322";
                ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A9"].Style.Font.Bold = true;
                ws.Cells["A9"].Style.Font.Size = 10;

                // Row 10
                ws.Cells["A10:G10"].Merge = true;
                ws.Cells["A10"].Value = "3: Địa chỉ: Tầng 5, ngõ 82, Dịch Vọng Hậu, Cầu Giấy";
                ws.Cells["A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A10"].Style.Font.Bold = true;
                ws.Cells["A10"].Style.Font.Size = 10;

                // Row 11
                ws.Cells["A11:G11"].Merge = true;
                ws.Cells["A11"].Value = "Kỳ báo cáo cuối cùng";
                ws.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A11"].Style.Font.Size = 10;

                // Row 12
                ws.Cells["A12:G12"].Merge = true;
                ws.Cells["A12"].Value = "Ngày đầu kỳ báo cáo: 01/04/2019";
                ws.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A12"].Style.Font.Size = 10;

                // Row 14 15 16 17 - Column A B C D E
                ws.Cells["A14:A17"].Merge = true;
                ws.Cells["A14"].Value = "STT";
                ws.Cells["A14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A14"].Style.Font.Size = 12;

                ws.Cells["B14:B17"].Merge = true;
                ws.Cells["B14"].Value = "Mã hóa đơn";
                ws.Cells["B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["B14"].Style.Font.Size = 12;

                ws.Cells["C14:C17"].Merge = true;
                ws.Cells["C14"].Value = "Tên hóa đơn";
                ws.Cells["C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["C14"].Style.Font.Size = 12;

                ws.Cells["D14:D17"].Merge = true;
                ws.Cells["D14"].Value = "Mẫu hóa đơn";
                ws.Cells["D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D14"].Style.Font.Size = 12;

                ws.Cells["E14:E17"].Merge = true;
                ws.Cells["E14"].Value = "Ký hiệu";
                ws.Cells["E14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["E14"].Style.Font.Size = 12;

                // Row 14 15 16 17 - Column F G H I J
                ws.Cells["F14:J14"].Merge = true;
                ws.Cells["F14"].Value = "Số tiền đã ký, phát hành trong kỳ";
                ws.Cells["F14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F15:F17"].Merge = true;
                ws.Cells["F15"].Value = "Tống số";
                ws.Cells["F15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G15:H16"].Merge = true;
                ws.Cells["G15"].Value = "Tồn đầu kỳ";
                ws.Cells["G15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I15:J16"].Merge = true;
                ws.Cells["I15"].Value = "Phát hành trong kỳ";
                ws.Cells["I15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G17"].Value = "Từ số";
                ws.Cells["G17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["G17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["H17"].Value = "Đến số";
                ws.Cells["H17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I17"].Value = "Từ số";
                ws.Cells["I17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J17"].Value = "Đến số";
                ws.Cells["J17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                // Row 14 15 16 17 - Column K L M N O P Q R 
                ws.Cells["K14:R14"].Merge = true;
                ws.Cells["K14"].Value = "Số sử dụng, hủy, xóa bỏ trong kỳ";
                ws.Cells["K14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["K15:M16"].Merge = true;
                ws.Cells["K15"].Value = "Tổng số sử dụng, xóa, hủy trong kỳ";
                ws.Cells["K15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["K17"].Value = "Từ số";
                ws.Cells["K17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["L17"].Value = "Đến số";
                ws.Cells["L17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["L17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["M17"].Value = "Cộng";
                ws.Cells["M17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["M17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["N16:N17"].Merge = true;
                ws.Cells["N16"].Value = "Số lượng đã sử dụng";
                ws.Cells["M17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["M17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["O16:P16"].Merge = true;
                ws.Cells["O16"].Value = "Xóa bỏ";
                ws.Cells["O16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["O17"].Value = "Số lượng";
                ws.Cells["O17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["P17"].Value = "Số";
                ws.Cells["P17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["P17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["Q16:R16"].Merge = true;
                ws.Cells["Q16"].Value = "Hủy";
                ws.Cells["Q16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["Q16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["Q17"].Value = "Số lượng";
                ws.Cells["Q17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["Q17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["R17"].Value = "Số";
                ws.Cells["R17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["R17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                ws.Cells["N15:R15"].Merge = true;
                ws.Cells["N15"].Value = "Trong đó";
                ws.Cells["N15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["N15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Row 14 15 16 17 - Column S T U
                ws.Cells["S14:U16"].Merge = true;
                ws.Cells["S14"].Value = "Tên cuối kỳ";
                ws.Cells["S14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["S14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["S17"].Value = "Từ số";
                ws.Cells["S17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["S17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["T17"].Value = "Đến số";
                ws.Cells["T17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["T17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["U17"].Value = "Số lượng";
                ws.Cells["U17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["U17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Print data table
                int rowStart = 18;
                int i = 1;

                while (rowStart < 20)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = i;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = "01GTKT";
                    ws.Cells[string.Format("C{0}", rowStart)].Value = "HÓA ĐƠN GIÁ TRỊ GIA TĂNG	";
                    ws.Cells[string.Format("D{0}", rowStart)].Value = "01GTKT00001";
                    ws.Cells[string.Format("E{0}", rowStart)].Value = "LK/19E	";
                    ws.Cells[string.Format("F{0}", rowStart)].Value = "1000000";
                    ws.Cells[string.Format("G{0}", rowStart)].Value = "1000000";
                    ws.Cells[string.Format("H{0}", rowStart)].Value = "0000001";
                    ws.Cells[string.Format("I{0}", rowStart)].Value = "0100000";
                    ws.Cells[string.Format("J{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("K{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("N{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("M{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("L{0}", rowStart)].Value = "0";
                    ws.Cells[string.Format("O{0}", rowStart)].Value = "0";
                    ws.Cells[string.Format("P{0}", rowStart)].Value = "0";
                    ws.Cells[string.Format("Q{0}", rowStart)].Value = "";
                    ws.Cells[string.Format("R{0}", rowStart)].Value = "0";
                    ws.Cells[string.Format("S{0}", rowStart)].Value = "0100000";
                    ws.Cells[string.Format("T{0}", rowStart)].Value = "0100000";
                    ws.Cells[string.Format("U{0}", rowStart)].Value = "0100000";

                    ws.Cells[string.Format("A{0}:U{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowStart++;
                    i++;
                }

                int lastRow = rowStart + 1;
                // Row after row start
                ws.Cells[string.Format("A{0}:E{0}", lastRow)].Merge = true;
                ws.Cells[string.Format("A{0}:E{0}", lastRow)].Value = "Tổng";
                ws.Cells[string.Format("A{0}:E{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("A{0}:E{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("A{0}:E{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("F{0}", lastRow)].Value = "20000000";
                ws.Cells[string.Format("F{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("F{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("F{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("G{0}", lastRow)].Value = "20000000";
                ws.Cells[string.Format("G{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("G{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("G{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("H{0}", lastRow)].Value = "20000000";
                ws.Cells[string.Format("H{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("H{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("H{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("I{0}", lastRow)].Value = "20000000";
                ws.Cells[string.Format("I{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("I{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("I{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("j{0}", lastRow)].Value = "";
                ws.Cells[string.Format("j{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("j{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("j{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("k{0}", lastRow)].Value = "";
                ws.Cells[string.Format("k{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("k{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("k{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("l{0}", lastRow)].Value = "";
                ws.Cells[string.Format("l{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("l{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("l{0}", lastRow)].Style.Font.Size = 12;
                ws.Cells[string.Format("m{0}", lastRow)].Value = "";
                ws.Cells[string.Format("m{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("m{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("m{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("n{0}", lastRow)].Value = "";
                ws.Cells[string.Format("n{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("n{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("n{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("o{0}", lastRow)].Value = "";
                ws.Cells[string.Format("o{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("o{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("o{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("p{0}", lastRow)].Value = "";
                ws.Cells[string.Format("p{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("p{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("p{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("q{0}", lastRow)].Value = "";
                ws.Cells[string.Format("q{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("q{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("q{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("r{0}", lastRow)].Value = "";
                ws.Cells[string.Format("r{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("r{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("r{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("s{0}", lastRow)].Value = "";
                ws.Cells[string.Format("s{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("s{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("s{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("t{0}", lastRow)].Value = "";
                ws.Cells[string.Format("t{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("t{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("t{0}", lastRow)].Style.Font.Size = 12;

                ws.Cells[string.Format("u{0}", lastRow)].Value = "";
                ws.Cells[string.Format("u{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("u{0}", lastRow)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("u{0}", lastRow)].Style.Font.Size = 12;

                // set border for table
                ws.Cells[string.Format("A14:U{0}", lastRow)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A14:U{0}", lastRow)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A14:U{0}", lastRow)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A14:U{0}", lastRow)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A14:U{0}", lastRow)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                // footer 

                int footer1 = lastRow + 2;
                int footer2 = lastRow + 3;
                int footer3 = lastRow + 4;

                ws.Cells[string.Format("a{0}:l{0}", footer1)].Merge = true;
                ws.Cells[string.Format("a{0}", footer1)].Value = "Cam kết báo cáo tình hình sử dụng hóa đơn trên đây là đúng sự thật, nếu sai đơn vị chịu hoàn toàn trách nhiệm trước pháp luật";
                ws.Cells[string.Format("a{0}", footer1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer1)].Style.Font.Bold = true;
                ws.Cells[string.Format("a{0}", footer1)].Style.Font.Size = 10;

                ws.Cells[string.Format("a{0}:l{0}", footer2)].Merge = true;
                ws.Cells[string.Format("a{0}", footer2)].Value = "Người lập biểu";
                ws.Cells[string.Format("a{0}", footer2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer2)].Style.Font.Bold = true;
                ws.Cells[string.Format("a{0}", footer2)].Style.Font.Size = 10;

                ws.Cells[string.Format("a{0}:l{0}", footer3)].Merge = true;
                ws.Cells[string.Format("a{0}", footer3)].Value = "(Ký, ghi rõ họ, tên)";
                ws.Cells[string.Format("a{0}", footer3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer3)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("a{0}", footer3)].Style.Font.Size = 10;

                ws.Cells[string.Format("n{0}:u{0}", footer1)].Merge = true;
                ws.Cells[string.Format("n{0}", footer1)].Value = "Ngày 22 tháng 07 năm 2019";
                ws.Cells[string.Format("n{0}", footer1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer1)].Style.Font.Size = 10;

                ws.Cells[string.Format("n{0}:u{0}", footer2)].Merge = true;
                ws.Cells[string.Format("n{0}", footer2)].Value = "ĐẠI DIỆN THEO PHÁP LUẬT";
                ws.Cells[string.Format("n{0}", footer2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer2)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer2)].Style.Font.Bold = true;
                ws.Cells[string.Format("n{0}", footer2)].Style.Font.Size = 10;

                ws.Cells[string.Format("n{0}:u{0}", footer3)].Merge = true;
                ws.Cells[string.Format("n{0}", footer3)].Value = "(Ký, ghi rõ họ, tên )";
                ws.Cells[string.Format("n{0}", footer3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer3)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[string.Format("n{0}", footer3)].Style.Font.Size = 10;

                ExcelWorksheet ws1 = ex.Workbook.Worksheets.Add("Data");
                ExcelWorksheet ws2 = ex.Workbook.Worksheets.Add("HDSD");

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xls");
                Response.BinaryWrite(ex.GetAsByteArray());
                Response.End();
            }
            catch (Exception except)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", except, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
        }

        public void GenerateInvoice(string TransactionID)
        {
            try
            {
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                MemoryStream PDFData = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, PDFData);

                var titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);

                // Custom font
                string ARIALUNI_TFF = Path.Combine(@"D:\X7\SPA_Invoice\SPA_Invoice\fonts\FontUnicode.ttf");
                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, true);
                Font bodyFont = new Font(bf, 10, Font.NORMAL);

                var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                // Type or member is obsolete
                BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");
                // Type or member is obsolete

                Rectangle pageSize = writer.PageSize;
                // Open the Document for writing
                pdfDoc.Open();
                //Add elements to the document here

                // Create the header table 
                PdfPTable headertable = new PdfPTable(3)
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100
                };
                headertable.SetWidths(new float[] { 100f, 320f, 100f });
                headertable.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPTable Invoicetable = new PdfPTable(3)
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100
                };

                Invoicetable.SetWidths(new float[] { 400f, 800f, 400f });  // then set the column's __relative__ widths
                Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                {
                    PdfPCell middlecell = new PdfPCell
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    Invoicetable.AddCell(middlecell);
                }

                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Độc lập - Tự do - Hạnh phúc", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("BÁO CÁO TÌNH HÌNH SỬ DỤNG HÓA ĐƠN (BC26/AC)", bodyFont))
                    {
                        PaddingTop = 20f,
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell3);
                    Invoicetable.AddCell(nested);
                }

                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Mẫu số: 01-1/GTGT", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("(Ban hành kềm theo Thông tư số 26/2015/TT-BTC" + DateTime.Now.ToShortDateString(), bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("ngày 27/02/2015 của bộ Tài chính)", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };

                    nested.AddCell(nextPostCell3);
                    nested.AddCell("");
                    PdfPCell nesthousing = new PdfPCell(nested)
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    Invoicetable.AddCell(nesthousing);
                }

                PdfPTable Invoicetable1 = new PdfPTable(1)
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100
                };

                Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("1: Tên tổ chức: Công ty công nghệ và truyền thông Nova:", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("2: Mã số thuế: 0968643354322", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("3: Địa chỉ: Tầng 5, ngõ 82, Dịch Vọng Hậu, Cầu Giấy", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    nested.AddCell(nextPostCell3);

                    nested.AddCell("");
                    PdfPCell nesthousing = new PdfPCell(nested)
                    {
                        Border = Rectangle.NO_BORDER
                    };
                    Invoicetable1.AddCell(nesthousing);
                }

                pdfDoc.Add(headertable);
                Invoicetable.PaddingTop = 10f;
                Invoicetable1.PaddingTop = 20f;

                pdfDoc.Add(Invoicetable);
                pdfDoc.Add(Invoicetable1);

                // Create table

                PdfPTable table = new PdfPTable(21)
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100,
                    SpacingAfter = 40
                };
                table.SetWidths(new float[] { 5, 10, 15, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });  // then set the column's __relative__ widths
                table.DefaultCell.Border = Rectangle.BOX;
                // row 1
                PdfPCell cell = new PdfPCell(new Phrase("STT", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Mã hóa đơn", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Tên hóa đơn", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Mẫu hóa đơn", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Ký hiệu", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số tiền đã ký, phát hành trong kỳ", bodyFont))
                {
                    Colspan = 5,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số sử dụng, hủy, xóa bỏ trong kỳ", bodyFont))
                {
                    Colspan = 8,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Tên cuối kỳ", bodyFont))
                {
                    Colspan = 3,
                    Rowspan = 3,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                // row 2
                cell = new PdfPCell(new Phrase("Tổng số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 3,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Tồn đầu kỳ", bodyFont))
                {
                    Colspan = 2,
                    Rowspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Phát hành trong kỳ", bodyFont))
                {
                    Colspan = 2,
                    Rowspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Tổng số sử dụng, xóa, hủy trong kỳ", bodyFont))
                {
                    Colspan = 3,
                    Rowspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Trong đó", bodyFont))
                {
                    Colspan = 5,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                // row 3
                cell = new PdfPCell(new Phrase("Số lượng đã sử dụng", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Xóa bỏ", bodyFont))
                {
                    Colspan = 2,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Hủy", bodyFont))
                {
                    Colspan = 2,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                // row 4

                cell = new PdfPCell(new Phrase("Từ số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Đến số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Từ số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Đến số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Từ số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Đến số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Cộng", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số lượng", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số lượng", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Từ số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Đến số", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số lượng", bodyFont))
                {
                    Colspan = 1,
                    Rowspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);


                table.SpacingBefore = 15f;

                int i = 1;
                while (i < 10)
                {
                    // col 1
                    PdfPCell item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 2
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 3
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 4
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 5
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 6
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 7
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 8
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 9
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 10
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 11
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 12
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 13
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 14
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 15
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 16
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 17
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 18
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 19
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 20
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    // col 21
                    item = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(item);

                    i++;
                }

                PdfPCell rowFooter = new PdfPCell(new Phrase("Tổng", bodyFont))
                {
                    Colspan = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                rowFooter = new PdfPCell(new Phrase("2000000", bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 8
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 9
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 10
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 11
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 12
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 13
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 14
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 15
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 16
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 17
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 18
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 19
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 20
                rowFooter = new PdfPCell(new Phrase(i.ToString(), bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                // col 21
                rowFooter = new PdfPCell(new Phrase("200000", bodyFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(rowFooter);

                pdfDoc.Add(table);

                // footer
                PdfPTable footer = new PdfPTable(2)
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100
                };
                footer.SetWidths(new float[] { 800f, 800f });
                footer.DefaultCell.Border = Rectangle.NO_BORDER;

                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Cam kết báo cáo tình hình sử dụng hóa đơn trên đây là đúng sự thật, nếu sai đơn vị chịu hoàn toàn trách nhiệm trước pháp luật", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("Người lập biểu", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(Ký, ghi rõ họ, tên)", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell3);
                    footer.AddCell(nested);
                }
                {
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("Ngày 22 tháng 07 năm 2019", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell1);

                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("ĐẠI DIỆN THEO PHÁP LUẬT", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell2);

                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("(Ký, ghi rõ họ, tên)", bodyFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    nested.AddCell(nextPostCell3);
                    footer.AddCell(nested);
                }
                pdfDoc.Add(footer);

                PdfContentByte cb = new PdfContentByte(writer);
                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageSize.GetLeft(40), 30);
                cb.ShowText("Ký điện tử bởi: CÔNG TY CỔ PHẦN TẬP ĐOÀN TRUYỀN THÔNG VÀ CÔNG NGHỆ NOVAON");
                cb.EndText();

                //Move the pointer and draw line to separate footer section from rest of page
                cb.MoveTo(40, pdfDoc.PageSize.GetBottom(50));
                cb.LineTo(pdfDoc.PageSize.Width - 40, pdfDoc.PageSize.GetBottom(50));
                cb.Stroke();

                pdfDoc.Close();
                DownloadPDF(PDFData);
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi download bảng kế tình hình sử dụng hóa đơn.", ex, MethodBase.GetCurrentMethod().Name, "DownloadXML");
            }
        }

        protected void DownloadPDF(MemoryStream ms)
        {
            // Clear response content & headers
            Response.ContentType = "pdf/application";
            Response.AddHeader("content-disposition", "attachment;filename=DownloadPdf.pdf");
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        }
        #endregion
    }
}