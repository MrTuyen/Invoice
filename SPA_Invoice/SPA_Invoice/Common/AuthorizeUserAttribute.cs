using DS.BusinessLogic.Customer;
using DS.BusinessObject.Account;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA_Invoice.Common
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public int Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserBLL userBLL = new UserBLL();
            // Nếu là admin thì mặc định full quyền
            if (AccountBO.Current.CurrentUser().ISADMIN)
            {
                return true;
            }
            var currentUser = AccountBO.Current.CurrentUser();
            string privilegeLevels = string.Join(",", userBLL.GetRoleDetailByUserId(currentUser.EMAIL, currentUser.COMTAXCODE).Select(x => x.ROLEDETAILID)); // Lấy quyền của user hiện tại
            if (privilegeLevels.Contains(this.Role.ToString()))
            {
                return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var resultJson = new { rs = false, msg = "Bạn không có quyền thực hiện chức năng này." };
            JsonResult unauthorizedResult = new JsonResult()
            {
                Data = resultJson,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.Result = unauthorizedResult;
        }

        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    filterContext.Result = new RedirectToRouteResult
        //    (
        //        new RouteValueDictionary
        //        (
        //            new
        //            {
        //                controller = "Home/Home"
        //            }
        //        )
        //    );
        //}
    }
}