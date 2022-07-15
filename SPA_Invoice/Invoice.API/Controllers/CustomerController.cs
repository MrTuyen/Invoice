using DS.BusinessLogic.Company;
using DS.BusinessObject.Invoice;
using Invoice.API.DTO.Company;
using Invoice.API.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Invoice.API.Controllers
{
    [RoutePrefix("api/onfinance")]
    public class CustomerController : BaseApiController
    {
        /// <summary>
        /// Get company's info by comtaxcode
        /// { "comtaxcode": "0336692463" }
        /// </summary>
        /// <param name="inv"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthentication]
        [Route("home/v21/companies/getinfo")]
        public HttpResponseMessage GetCompanyInfo([FromBody] InvoiceBO inv)
        {
            try
            {
                if (inv != null)
                {
                    CompanyBLL companyBLL = new CompanyBLL();
                    string comTaxCode = inv.COMTAXCODE;
                    var objCompany = companyBLL.GetInfoEnterpriseByTaxCode(comTaxCode);
                    if (objCompany == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, msg = "Không tìm thấy thông tin doanh nghiệp." });
                    }
                    var result = new CompanyResponse()
                    {
                        COMTAXCODE = objCompany.COMTAXCODE,
                        COMNAME = objCompany.COMNAME,
                        COMADDRESS = objCompany.COMADDRESS,
                        COMEMAIL = objCompany.COMADDRESS,
                        COMPHONENUMBER = objCompany.COMPHONENUMBER,
                        COMLEGALNAME = objCompany.COMLEGALNAME,
                        COMACCOUNTNUMBER = objCompany.COMACCOUNTNUMBER,
                        COMBANKNAME = objCompany.COMBANKNAME
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = true, code = 200, msg = "Tìm thấy thông tin công ty.", data = result });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 404, msg = "Không tìm thấy mã số thuế doanh nghiệp truyền vào." });
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { rs = false, code = 500, msg = "Internal Error." });
            }
        }
    }
}