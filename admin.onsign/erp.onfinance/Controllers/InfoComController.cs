using DS.BusinessLogic.hosodoanhnghiep;
using DS.BusinessObject.hosodoanhnghiep;
using DS.Common.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace erp.onfinance.Controllers
{
    public class InfoComController : Controller
    {
        string BaseURL = "http://www.hotel84.com/tam-dao/";

        // GET: InfoCom
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Init(string keyword, int? itemPerPage, int? currentPage)
        {
            for (int i = 1; i < 2; i++)
            {
                new Thread(() => this.GetInfoCom(string.Format("{0}{1}{2}/", this.BaseURL, "page", i)))
                {
                    IsBackground = true
                }.Start();
                Thread.Sleep(10000);
            }

            return Json(new { rs = true });
        }
        public void GetInfoCom(string url)
        {
            string strHtml = this.HttpGet(url).Trim();
            var html = new HtmlDocument();
            html.LoadHtml(strHtml);
            var document = html.DocumentNode;
            try
            {
                var listPromo = document.QuerySelectorAll(".row_hotel_list");
                foreach (HtmlNode item in listPromo)
                {
                    hosodoanhnghiepBO hsdn = new hosodoanhnghiepBO
                    {
                        LINKDETAIL = item.QuerySelector("a").Attributes["href"].Value.Trim(),
                        COMNAME = item.QuerySelector("a").Attributes["title"].Value.Trim(),
                        COMADDRESS = WebUtility.HtmlDecode(item.QuerySelectorAll("span .tbold").Last().NextSibling.InnerText).Trim()
                    };
                    new Thread(() => GetDetailInfoCom(hsdn))
                    {
                        IsBackground = true
                    }.Start();
                    Thread.Sleep(100);
                }
            }
            catch (Exception exp)
            {
            }
        }

        private void GetDetailInfoCom(hosodoanhnghiepBO hsdn)
        {
            string strHtml = this.HttpGet(hsdn.LINKDETAIL).Trim();
            var html = new HtmlDocument();
            html.LoadHtml(strHtml);
            var document = html.DocumentNode;
            try
            {

                hsdn.COMPHONE = document.QuerySelectorAll("a").Where(x => x.Attributes["href"].Value.Contains("el:")).First().InnerText.Trim();

                hosodoanhnghiepBLL bll = new hosodoanhnghiepBLL();
                var result = bll.AddHSDN(hsdn);
                if (bll.ResultMessageBO.IsError)
                {
                    ConfigHelper.Instance.WriteLog(bll.ResultMessageBO.Message, bll.ResultMessageBO.MessageDetail, MethodBase.GetCurrentMethod().Name, "GetUser");
                }
            }
            catch (Exception exp)
            {
            }
        }

        private string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();
            var responseReader = new StreamReader(webStream);
            var response = responseReader.ReadToEnd();
            responseReader.Close();
            return response;
        }

    }
}