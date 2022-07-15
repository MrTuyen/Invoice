using DS.BusinessObject.Account;
using DS.BusinessObject.Category;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using DS.DataObject.Account;
using DS.DataObject.Category;
using DS.DataObject.Product;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessLogic.Category
{
    public class CategoryBLL : BaseBLL
    {
        #region Fields
        protected IData objDataAccess = null;

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public CategoryBLL()
        {
        }

        public CategoryBLL(IData objIData)
        {
            objDataAccess = objIData;
        }
        #endregion

        #region Methods
        //lấy ra danh sách dnah mục
        public List<CategoryBO> GetAllCategory(string keyword, int pagesize, int offest,string Comtaxcode)
        {
            try
            {
                CategoryDAO objCategoryDAO = new CategoryDAO();
                return objCategoryDAO.GetAllCategory(keyword,pagesize,offest,Comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi lấy danh sách danh mục");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return null;
            }
        }
        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        /// <returns></returns>
        public bool SaveProduct(CategoryBO category)
        {
            try
            {
                CategoryDAO productDAO = new CategoryDAO();
                return productDAO.SaveCategory(category);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Tên danh mục bị trùng vui lòng thử lại với tên khác");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }

        public bool SaveQuantityUnit(string category,long ID,string comtaxcode)
        {
            try
            {
                CategoryDAO productDAO = new CategoryDAO();
                return productDAO.SaveQuantityUnit(category,ID,comtaxcode);
            }
            catch (Exception objEx)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(objEx, "Lỗi cập nhật đơn vị tính");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, objEx, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return false;
            }
        }
        //xóa dịch vụ
        public string RemoveCategory(string id)
        {
            try
            {
                CategoryDAO categoryDAO = new CategoryDAO();
                return categoryDAO.RemoveCategory(id);
            }
            catch(Exception ex)
            {
                this.ErrorMsg = MethodHelper.Instance.GetErrorMessage(ex, "Lỗi xóa dịch vụ");
                objResultMessageBO = ConfigHelper.Instance.WriteLog(this.ErrorMsg, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), this.NameSpace);
                return this.ErrorMsg;
            }
        }
        #endregion
    }
}
