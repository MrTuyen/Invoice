using DS.BusinessObject.PaymentMethod;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.PaymentMethod
{
    public class PaymentMethodDAO:BaseDAO
    {
        #region Constructor

        public PaymentMethodDAO() : base()
        {
        }

        public PaymentMethodDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion
        #region Methods
        //lấy ra danh sách HTTT
        public List<PaymentMethodBO> GetPaymentMethod(string keyword, int pagesize, int offset,string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_payment_method_getall");
                objIData.AddParameter("p_keyword", keyword);
                objIData.AddParameter("p_pagesize", pagesize);
                objIData.AddParameter("p_offset", offset);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<PaymentMethodBO>();
                ConvertToObject(reader, list);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //Tạo mới hoặc cập nhật HTTT
        public bool SavePaymentMethod(PaymentMethodBO payment)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.common_payment_method_add");
                objIData.AddParameter("p_id", payment.ID);
                objIData.AddParameter("p_paymentmethod", payment.PAYMENTMETHOD);
                objIData.AddParameter("p_comtaxcode", payment.COMTAXCODE);
                objIData.AddParameter("p_isactived", payment.ISACTIVED);
                var reader = objIData.ExecNonQuery();
                CommitTransactionIfAny(objIData);

                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //xóa hình thức thanh toán
        
        public string RemovePaymentMehtod(string id)
        {
            IData objData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objData);
                objData.ExecUpdate($"DELETE FROM ds_masterdata.common_payment_method WHERE id IN ({id}) AND comtaxcode != '1' AND isactived = false");
                CommitTransactionIfAny(objData);
                return string.Empty;
            }catch(Exception ex)
            {
                RollBackTransactionIfAny(objData);
                throw ex;
            }
            finally
            {
                this.DisconnectIData(objData);
            }
        }
        #endregion
    }
}
