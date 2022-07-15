using DS.BusinessObject.Account;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.DataObject.Product
{
    public class ProductDAO : BaseDAO
    {
        #region Constructor

        public ProductDAO() : base()
        {
        }

        public ProductDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Methods

        //Lấy danh sách sản phẩm và dịch vụ
        public List<ProductBO> GetProduct(FormSearchProduct formSearch)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_get");
                objIData.AddParameter("p_comtaxcode", formSearch.COMTAXCODE);
                objIData.AddParameter("p_keyword", formSearch.KEYWORD);
                objIData.AddParameter("p_producttype", formSearch.PRODUCTTYPE);
                objIData.AddParameter("p_category", formSearch.CATEGORY);
                objIData.AddParameter("p_pagesize", formSearch.ITEMPERPAGE);
                objIData.AddParameter("p_offset", formSearch.OFFSET);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ProductBO>();
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

        //Tạo mới hoặc cập nhật sản phẩm
        public bool AddProduct(ProductBO product)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_add");
                objIData.AddParameter("p_comtaxcode", product.COMTAXCODE);
                objIData.AddParameter("p_producttype", product.PRODUCTTYPE);
                objIData.AddParameter("p_productname", product.PRODUCTNAME);
                objIData.AddParameter("p_sku", product.SKU);
                objIData.AddParameter("p_category", product.CATEGORY == null ? "Khác" : product.CATEGORY);
                objIData.AddParameter("p_description", product.DESCRIPTION);
                objIData.AddParameter("p_image", product.IMAGE);
                objIData.AddParameter("p_price", product.PRICE);
                objIData.AddParameter("p_quantityunit", product.QUANTITYUNIT == null ? "Khác" : product.QUANTITYUNIT);
                objIData.AddParameter("p_taxrate", null);
                objIData.AddParameter("p_fromlevel", product.FROMLEVEL);
                objIData.AddParameter("p_level", product.TOLEVEL);
                objIData.AddParameter("p_groupid", product.GROUPID);
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

        public bool UpdateProduct(ProductBO product)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_update");
                objIData.AddParameter("p_id", product.ID);
                objIData.AddParameter("p_comtaxcode", product.COMTAXCODE);
                objIData.AddParameter("p_producttype", product.PRODUCTTYPE);
                objIData.AddParameter("p_productname", product.PRODUCTNAME);
                objIData.AddParameter("p_sku", product.SKU);
                objIData.AddParameter("p_category", product.CATEGORY);
                objIData.AddParameter("p_description", product.DESCRIPTION);
                objIData.AddParameter("p_image", product.IMAGE);
                objIData.AddParameter("p_price", product.PRICE);
                objIData.AddParameter("p_quantityunit", product.QUANTITYUNIT);
                objIData.AddParameter("p_fromlevel", product.FROMLEVEL);
                objIData.AddParameter("p_level", product.TOLEVEL);
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

        public bool UpdateChangeType(string productIds, int productType)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_update_change_type");
                objIData.AddParameter("p_ids", productIds);
                objIData.AddParameter("p_producttype", productType);
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

        /// <summary>
        /// Kiểm tra hàng hóa đã tồn tại hay chưa
        /// truongnv 20200225
        /// </summary>
        /// <param name="comTaxCode">MST</param>
        /// <param name="productName">Tên hàng hóa</param>
        /// <returns></returns>
        public string CheckProductDuplicateByTaxCode(string comTaxCode, string productName)
        {
            string msg = string.Empty;
            try
            {
                IData objIData = this.CreateIData();
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_check_duplicate");
                objIData.AddParameter("p_comtaxcode", comTaxCode);
                objIData.AddParameter("p_productname", productName);
                var id = objIData.ExecStoreToString();
                CommitTransactionIfAny(objIData);
                if (!string.IsNullOrWhiteSpace(id))
                    msg = $"Hàng hóa {productName} đã tồn tại. Vui lòng kiểm tra lại.";
            }
            catch (Exception ex)
            {
                msg = $"Lỗi kiểm tra hàng hóa.";
                throw ex;
            }
            return msg;
        }

        public List<ProductBO> GetProductListByMeterCode(string meterCode, string custaxcode, string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_get_by_metercode");
                objIData.AddParameter("p_metercode", meterCode);
                objIData.AddParameter("p_custaxcode", custaxcode);
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ProductBO>();
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

        public List<ProductBO> GetListProductByComtaxCode(string custaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_get_by_comtaxcode");
                objIData.AddParameter("p_comtaxcode", custaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ProductBO>();
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
        public List<ProductBO> GetListProductPriceByComtaxCode(string comtaxcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                objIData.CreateNewStoredProcedure("ds_masterdata.pm_product_price_get_by_comtaxcode");
                objIData.AddParameter("p_comtaxcode", comtaxcode);
                var reader = objIData.ExecStoreToDataReader();
                var list = new List<ProductBO>();
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

        public string DeleteProduct(string productIds)
        {
            IData objIData = this.CreateIData();

            string msg = string.Empty;
            try
            {
                BeginTransactionIfAny(objIData);
                //objIData.CreateNewStoredProcedure("ds_masterdata.pm_invoice_delete_invocie");
                //objIData.AddParameter("p_ids", invoiceIds);
                objIData.ExecUpdate($"UPDATE ds_masterdata.pm_product SET isdeleted = true WHERE pm_product.id IN({productIds})");
                CommitTransactionIfAny(objIData);
                return string.Empty;
            }
            catch (Exception objEx)
            {
                msg = objEx.Message;
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }
        //kiểm tra sản phẩm có tồn tại trong invoice detail hay k
        #endregion
        public int CkeckProduct(string proname)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToString($"SELECT COUNT(*) FROM ds_masterdata.pm_invoice_detail WHERE productname IN ({proname}) AND isdeleted=FALSE");
                CommitTransactionIfAny(objIData);
                return Convert.ToInt32(reader);
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
        //Nếu là hóa đơn tiền điện tiền nước.
        //Kiểm tra xem mã sku có tồn tại trong bảng meter không
        public int CkeckMeter(string productcode)
        {
            IData objIData = this.CreateIData();
            try
            {
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToString($"SELECT COUNT(*) FROM ds_masterdata.pm_product WHERE sku IN (SELECT productcode FROM ds_masterdata.common_meter WHERE productcode IN ({productcode}) AND isactive = TRUE)");
                CommitTransactionIfAny(objIData);
                return Convert.ToInt32(reader);
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
    }
}