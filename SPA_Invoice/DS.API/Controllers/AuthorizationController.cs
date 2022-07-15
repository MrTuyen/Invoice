using DS.API.Dto.Order;
using DS.BusinessLogic.API;
using DS.BusinessObject.API;
using DS.Common.Helpers;
using System;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Linq;

namespace DS.API.Controllers
{
    [RoutePrefix("api/oauth")]
    public class AuthorizationController : BaseApiController
    {
        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public TokenResponse GetToken([FromBody]PartnerRequest requestDto)
        {
            try
            {
                if (requestDto == null)
                {
                    return new TokenResponse { token = null, tokenType = null, success = false, message = "params is not null. body: null" };
                }

                if (string.IsNullOrEmpty(requestDto.partnerId))
                {
                    return new TokenResponse { token = null, tokenType = null, success = false, message = $"partnerId is not null.  partnerId: null" };
                }

                if (!CheckUser(requestDto.partnerId))
                {
                    return new TokenResponse { token = null, tokenType = null, success = false, message = $"{requestDto.partnerId}: partner does not exits." };
                }

                return new TokenResponse { token = JwtManager.GenerateToken(requestDto.partnerId), tokenType = "bearer", success = true, message = "OK" };
            }
            catch
            {
                return new TokenResponse { token = null, tokenType = null, success = false, message = "error system." };
            }
        }

        private bool CheckUser(string partnerId)
        {
            try
            {
                PartnerOAuthBO partnerOAuthBO = new PartnerOAuthBO
                {
                    PARTNERID = partnerId.ToLower().Trim(),
                    ISACTIVED = true
                };
                PartnerOAuthBLL partnerOAuthBLL = new PartnerOAuthBLL();
                var partner = partnerOAuthBLL.GetPartner(partnerOAuthBO);
                if (partnerOAuthBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(partnerOAuthBLL.ResultMessageBO.Message, partnerOAuthBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return false;
                }
                if (partner.Count > 0)
                {
                    //partnerObject = partner.FirstOrDefault();

                    //var session = HttpContext.Current.Session;
                    //if (session == null)
                    //{
                    //    if (session[ConfigHelper.Partner] == null)
                    //    {
                    //        session[ConfigHelper.Partner] = partnerObject;
                    //    }
                    //}
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}