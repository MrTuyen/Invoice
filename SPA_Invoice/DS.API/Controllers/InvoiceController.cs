using DS.API.Dto.Order;
using DS.API.DTO.Invoice;
using DS.API.Filter;
using DS.BusinessLogic.Invoice;
using DS.BusinessObject.Invoice;
using DS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Routing;

namespace DS.API.Controllers
{
    [RoutePrefix("api/onfinance")]
    [JwtAuthentication]
    public class InvoiceController : BaseApiController
    {
        [HttpPost]
        [Route("getInvoice")]
        [JwtAuthentication]
        public InvoiceResponse GetInvoice([FromBody]InvoiceRequest invoiceRequest)
        {
            try
            {
                if (invoiceRequest == null)
                {
                    return new InvoiceResponse { code = "201", success = false,time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = $"Bad request. body: null" };
                }

                if (invoiceRequest.partnerid != "pp")
                {
                    return new InvoiceResponse { code = "203", success = false, time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = $"partnerCode: {invoiceRequest.partnerid} does not exist. partnerid: null" };
                }

                if (invoiceRequest.invoice == new FormSearchInvoice())
                {
                    return new InvoiceResponse { code = "204", success = false, time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = $"invoice is not null. invoice: null" };
                }

                List<InvoiceBO> invoices = this.GetInvoice(invoiceRequest.invoice);

                if (invoices.Count == 0)
                {
                    return new InvoiceResponse { code = "205", success = true, time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = $"data is null." };
                }

                return new InvoiceResponse { code = "200", success = true, time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = $"{invoices.Count} results founded", data = invoices };
            }
            catch (Exception ex)
            {
                return new InvoiceResponse { code = "500", success = false, time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), message = "Error system! Please try again." };
            }
        }



        private List<InvoiceBO> GetInvoice(FormSearchInvoice form)
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
                form.OFFSET = (form.CURRENTPAGE - 1) * form.ITEMPERPAGE;
                InvoiceBLL invoiceBLL = new InvoiceBLL();

                List<InvoiceBO> result = invoiceBLL.GetInvoice(form);


                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.CANCELEDLINK))
                        item.SIGNLINK = item.CANCELEDLINK;
                    else if (!string.IsNullOrEmpty(item.SIGNEDLINK))
                        item.SIGNLINK = item.SIGNEDLINK;
                }

                if (invoiceBLL.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(invoiceBLL.ResultMessageBO.Message, invoiceBLL.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                    return new List<InvoiceBO>();
                }
                return result;
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Lỗi lấy danh sách hóa", ex, MethodBase.GetCurrentMethod().Name, "GetInvoice");
                return new List<InvoiceBO>();
            }
        }

    }
}