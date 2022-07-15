using DS.BusinessObject.Account;
using DS.BusinessObject.Invoice;
using DS.BusinessObject.Product;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Product
{
    public class ProductBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public ProductBLL()
        {
        }

        public ProductBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods


        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        /// <returns></returns>
       
        public List<ProductBO> GetProduct(FormSearchProduct formSearch)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                return productDAO.GetProduct(formSearch);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách sản phẩm");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<ProductBO>();
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        /// <returns></returns>
        public bool AddProduct(ProductBO product)
        {
            try
            {
                if (product.PRODUCTNAME.StartsWith("Điều chỉnh"))
                    return true;
                ProductDAO productDAO = new ProductDAO();
                return productDAO.AddProduct(product);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm sản phẩm");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool UpdateProduct(ProductBO product)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                return productDAO.UpdateProduct(product);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật sản phẩm");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Tải lên danh sách sản phảm Excel
        /// </summary>
        /// <returns></returns>
        public bool ImportProduct(List<ProductBO> products)
        {
            //IData objIData;
            //if (objDataAccess == null)
            //{
            //    objIData = Data.CreateData(ConfigHelper.Instance.GetConnectionStringDS(), false);
            //    objDataAccess = objIData;
            //}
            //else
            //    objIData = objDataAccess;
            try
            {
                ProductDAO productDAO = new ProductDAO(/*objIData*/);
                foreach (var product in products)
                {
                    productDAO.AddProduct(product);
                }
                return true;
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi thêm sản phẩm từ Excel");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        /// <summary>
        /// Cập nhật loại sản phẩm là Hàng hóa hay Dịch vụ
        /// </summary>
        /// <returns></returns>
        public bool UpdateChangeType(string productIds, int productType)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                return productDAO.UpdateChangeType(productIds, productType);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật loại sản phẩm");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
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

                ProductDAO productDAO = new ProductDAO();
                return productDAO.CheckProductDuplicateByTaxCode(comTaxCode, productName);
            }
            catch (Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi kiểm tra MST của doanh nghiệp.");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                msg = $"Không lấy được thông tin MST của doanh nghiệp.";
            }
            return msg;
        }

        public List<ProductBO> ES_ProductByKeyword(string keywords, string comtaxcode, int pagesize, int pageindex)
        {
            try
            {
                keywords = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(keywords));
                //var str = ElaHelper.GenTerm("CHAI");
                string kwfieldEN = "nOTE";
                string[] arrKeywords = keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string rsl_en = kwfieldEN + ":((*" + string.Join("*) AND (*", arrKeywords) + "*))";
                //string rsl_en = kwfieldEN + ":((*" + string.Join("*) AND (*", arrKeywords) + "*)) AND cOMTAXCODE:" + comtaxcode + "";
                //Ưu tiên tìm sản phẩm
                var result = ElasticIndexer.Current.IndexClient.Search<ProductBO>(s => s
                    .Index("product").Type("index")
                    .From(pageindex * pagesize)
                    .Size(pagesize)
                    .Query(q => q.QueryString(qs => qs.Query(rsl_en)))
                );
                return result.Documents.ToList();
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi gọi Elasticsearch product");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<ProductBO> ES_ReceiveByKeyword(string keywords, string comtaxcode, int pagesize, int pageindex)
        {
            try
            {
                keywords = ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(keywords));
                string rsl_en = $"nOTE:{keywords} AND cOMTAXCODE:({comtaxcode})";

                var result = ElasticIndexer.Current.IndexClient.Search<ProductBO>(s => s
                    .Index("product").Type("index")
                    .From(pageindex * pagesize)
                    .Size(pagesize)
                    .Query(q => q.QueryString(qs => qs.Query(rsl_en)))
                );

                //var result = ElasticIndexer.Current.IndexClient.Search<ProductBO>(s => s
                //    .Index("product")
                //    .From(pageindex * pagesize)
                //    .Size(pagesize)
                //    .Query(q =>
                //        q.QueryString(qs =>
                //            qs.Query(rsl_en)
                //            .Type("index")
                //        )
                //    )
                //);

                return result.Documents.ToList(); ;
            }
            catch (Exception objEx)
            {
                var msg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi gọi Elasticsearch customer");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(msg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }

        public List<ProductBO> GetProductListByMeterCode(string meterCode, string custaxcode, string comtaxcode)
        {
            try
            {
                ProductDAO oDL = new ProductDAO();
                return oDL.GetProductListByMeterCode(meterCode, custaxcode, comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<ProductBO>();
            }
        }

        public List<ProductBO> GetListProductByComtaxCode(string custaxcode)
        {
            try
            {
                ProductDAO oDL = new ProductDAO();
                return oDL.GetListProductByComtaxCode(custaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<ProductBO>();
            }
        }

        /// <summary>
        /// Lấy danh sách biểu giá điện
        /// </summary>
        /// <param name="comtaxcode"></param>
        /// <returns></returns>
        public List<ProductBO> GetListProductPriceByComtaxCode(string comtaxcode)
        {
            try
            {
                ProductDAO oDL = new ProductDAO();
                return oDL.GetListProductPriceByComtaxCode(comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy thông tin bộ chỉ số công tơ điện");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return new List<ProductBO>();
            }
        }
        public string DeleteProduct(string productids)
        {
            string msg = string.Empty;
            try
            {
                ProductDAO productDAO = new ProductDAO();
                msg = productDAO.DeleteProduct(productids);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return objEx.Message;
            }
            return msg;
        }
        //kiểm tra sản phẩm có tồn tại trong invoice detail hay k
        public int CheckPro(string proname)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                return productDAO.CkeckProduct(proname);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return 0;
            }
        }
        //kiểm tra mã SKU có nằm trong bảng meter không (với HĐ điện nước)
        public int CheckMeter(string procode)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();
                return productDAO.CkeckMeter(procode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách khách hàng");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return 0;
            }
        }
        #endregion
    }
}
