using DS.BusinessObject.EmailSender;
using DS.DataObject.Email;
using DS.DataObject.Invoice;
using SAB.Library.Data;
using System.Collections.Generic;

namespace DS.BusinessLogic.EmailSender
{
    public class EmailBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public EmailBLL()
        {
        }

        public EmailBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public long AddEmail(EmailBO email)
        {
            EmailDAO emailDAO = new EmailDAO();
            return emailDAO.AddEmail(email);
        }

        public long UpdateEmail(EmailBO email)
        {
            EmailDAO emailDAO = new EmailDAO();
            return emailDAO.UpdateEmail(email);
        }

        public List<EmailBO> GetEmailHistoryByInvoiceId(long invoiceId)
        {
            EmailDAO emailDAO = new EmailDAO();
            return emailDAO.GetEmailHistoryByInvoiceId(invoiceId);
        }

        public List<EmailBO> GetEmailHistoryByComtaxcode(FormSearchEmail form)
        {
            EmailDAO emailDAO = new EmailDAO();
            return emailDAO.GetEmailHistoryByComtaxcode(form);
        }
        #endregion
    }
}
