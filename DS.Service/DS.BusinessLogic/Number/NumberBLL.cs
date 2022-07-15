using DS.BusinessObject;
using DS.BusinessObject.Account;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Number;
using DS.DataObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Number
{
    public class NumberBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public NumberBLL()
        {
        }

        public NumberBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods


        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        /// <returns></returns>

        public List<InvoiceNumberBO> GetNumberWaiting(FormSearchNumber formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNumberWaiting(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách dải hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceNumberBO>();
            }
        }

        /// <summary>
        /// Get waiting number to sign in waiting invoice
        /// </summary>
        /// <param name="waitingNumberId"></param>
        /// <returns></returns>
        public InvoiceNumberBO GetWaitingNumberByWaitingNumberId(long waitingNumberId)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetWaitingNumberByWaitingNumberId(waitingNumberId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy số dải hóa đơn chờ.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        //// <summary>
        ///// Get waiting number to sign in waiting invoice
        ///// </summary>
        ///// <param name="waitingNumberId"></param>
        ///// <returns></returns>
        //public Int64 GetWaitingNumberByWaitingNumberId(long waitingNumberId)
        //{
        //    try
        //    {
        //        NumberDAO numberDAO = new NumberDAO();
        //        return numberDAO.GetWaitingNumberByWaitingNumberId(waitingNumberId);
        //    }
        //    catch (Exception objEx)
        //    {
        //        this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy số dải hóa đơn chờ.");
        //        objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
        //        return 0;
        //    }
        //}


        /// <summary>
        /// Thêm hóa đơn cách dải
        /// </summary>
        /// <returns></returns>
        public bool AddNumberWaiting(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.AddNumberWaiting(invoiceNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm dải chờ hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        /// <summary>
        /// Cập nhật hóa đơn cách dải
        /// </summary>
        /// <returns></returns>
        public bool UpdateNumberWaiting(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.UpdateNumberWaiting(invoiceNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số bắt đầu, số đến dải chờ hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        /// <summary>
        /// Cập nhật số hiện tại của dãy cách dải
        /// </summary>
        /// <returns></returns>
        public bool UpdateCurrentWaittingNumber(long numberId, long nextNumber)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.UpdateCurrentWaittingNumber(numberId, nextNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số hiện tại dải chờ hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        /// <summary>
        /// Thêm thông báo phát hành
        /// </summary>
        /// <returns></returns>
        public bool AddReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.AddReleaseNotice(invoiceNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public bool SaveReleaseNotice(InvoiceNumberBO invoiceNumber)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.SaveReleaseNotice(invoiceNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public bool DeleteReleaseNotice(long id)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.DeleteReleaseNotice(id);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                throw objEx;
            }
        }

        public List<InvoiceNumberBO> AddNumberWaitingcheck(InvoiceNumberBO formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.AddNumberWaitingcheck(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi kiểm tra danh sách dải hóa đơn");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceNumberBO>();
            }
        }

        #region Thống kê, báo cáo

        public List<InvoiceBO> GetUsingInvoice(InvoiceNumberBO formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetUsingInvoice(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lọc báo cáo tình hình sử dụng hóa đơn.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// Bảng kê hóa đơn giá trị gia tăng
        /// </summary>
        /// <param name="formSearch"></param>
        /// <returns></returns>
        public List<OutputInvoice> GetOutputInvoice(InvoiceNumberBO formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetOutputInvoice(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lọc bảng kê hóa đơn đầu ra.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<OutputInvoice>();
            }
        }

        public List<InvoiceBO> GetOutputInvoiceExcel(InvoiceNumberBO formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetOutputInvoiceExcel(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lọc bảng kê hóa đơn đầu ra.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceBO>();
            }
        }

        /// <summary>
        /// Bảng kê hóa đơn bán hàng
        /// </summary>
        /// <param name="formSearch"></param>
        /// <returns></returns>
        public List<OutputInvoice> GetOutputSellInvoice(InvoiceNumberBO formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetOutputSellInvoice(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lọc bảng kê hóa đơn đầu ra.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<OutputInvoice>();
            }
        }

        /// <summary>
        /// Get number list
        /// </summary>
        /// <param name="formSearch"></param>
        /// <returns></returns>
        public List<InvoiceNumberBO> GetNumber(FormSearchNumber formSearch)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNumber(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<InvoiceNumberBO>();
            }
        }

        #endregion

        public InvoiceNumberBO GetNumberByInvoiceId(long invoiceId)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNumberByInvoiceId(invoiceId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new InvoiceNumberBO();
            }
        }

        public Int64 GetNumberByInvoiceId2(long invoiceId)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNumberByInvoiceId2(invoiceId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public List<NextNumber> GetNextNumberByInvoiceIdUSB(long invoiceId, long quantity)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNextNumberByInvoiceIdUSB(invoiceId, quantity);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy số tiếp theo.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public Int64 GetNextNumberByInvoiceId(string comtaxcode, string formcode, string symbolcode)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNextNumberByInvoiceId(comtaxcode, formcode, symbolcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông báo phát hành");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return -1;
            }
        }

        public bool UpdateCurrentNumber(long invoiceId, long nextNumber)
        {
            try
            {
                NumberDAO invoiceDAO = new NumberDAO();
                return invoiceDAO.UpdatCurrentNumber(invoiceId, nextNumber);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số hóa đơn hiện tại");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public string SetSigningStatus(long invoiceId, bool signingStatus)
        {
            try
            {
                NumberDAO numberBLL = new NumberDAO();
                return numberBLL.SetSigningStatus(invoiceId, signingStatus);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật số hóa đơn hiện tại");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public InvoiceNumberBO GetNumberRecord(long invoiceId)
        {
            try
            {
                NumberDAO numberDAO = new NumberDAO();
                return numberDAO.GetNumberRecord(invoiceId);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi GetNumberRecord");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        #endregion
    }
}
