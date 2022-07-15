using Plugin.Signer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Plugin.Server
{
    public enum FUNCTION_ID
    {
        checkPlugin = 0,
        getCertInfo = 1,
        signPDF = 2,
        chooseFile = 3,
        signXML = 4,
        signPdfAndXml = 5
    }

    public class DataProcessing
    {
        private static String SIGNING_TYPE = "type";
        private static String SIGNING_INPUT = "input";
        private static String SIGNING_INDEX = "index";
        //kt559: transaction ID để kiểm tra khi nhận kết quả => check không ký 2 lần
        //private static String TRANSACION_ID = "transacionid";

        private static String SIGNING_SIGREASON = "sigReason";
        private static String SIGNING_SIGLOCATION = "sigLocation";
        private static String SIGNING_SIGCONTACT = "sigContact";
        private static String SIGNING_LAYER2TEXT = "layer2text";
        private static String SIGNING_VISIBLEMODE = "visibleMode";
        private static String SIGNING_RENDERMODE = "rendermode";
        private static String SIGNING_PAGENO = "pageNo";
        private static String SIGNING_IMG = "img";
        //tọa độ vị trí của vùng chữ ký (chữ nhật). cách tính: bên ngoài tính
        private static String SIGNING_LLX = "llX";
        private static String SIGNING_LLY = "llY";
        private static String SIGNING_URX = "urX";
        private static String SIGNING_URY = "urY";

        private static String IMAGE_WIDTH = "imageWidth";
        private static String IMAGE_HEIGHT = "imageHeight";


        private static String SIGNING_SIGFIELD = "sigFieldName";
        //============== một số lựa chọn ký + hash: SHA1RSA hay SHA256RSA ??
        private static String SIGNING_HASHALG = "hashAlg";
        //=============== thông tin time server
        private static String SIGNING_TSA = "withTSA";
        private static String SIGNING_TSAURL = "tsaUrl";
        private static String SIGNING_TSALOGIN = "tsaLogin";
        private static String SIGNING_TSAPASS = "tsaPass";

        //============ thông tin số ký hiệu, ngày tháng khi ký ==================
        private static string SIGNING_SOKYHIEU = "sokyhieu";
        private static string SIGNING_NGAY = "ngay";
        private static string SIGNING_THANG = "thang";

        //============ thông tin chế độ certification level
        private static string SIGNING_CERTIFICATION = "certlevel";

        //TODO: hàm tạo json trả về
        public static string CreateSigningResult(int code, int index, string erDesc, string type = "", string dataSigned = "")
        {
            String respone = String.Format("\"code\":{0}, \"index\":{1}, \"data\":\"{2}\", \"type\":\"{3}\", \"error\":\"{4}\""
                , code, index, dataSigned, type, erDesc);
            return "{" + respone + "}";
        }

        public static string CreateSigningResult(string transacionID, int code, int index, string erDesc, string type = "", string dataSigned = "", string subject = "")
        {
            //String respone = String.Format("\"code\":{0}, \"index\":{1}, \"data\":\"{2}\", \"type\":\"{3}\", \"error\":\"{4},\" \"transactionid\":{5}"
            String respone = String.Format("\"code\":{0}, \"index\":{1}, \"data\":\"{2}\", \"type\":\"{3}\", \"error\":\"{4}\", \"transactionid\":\"{5}\",\"subject\":\"{6}\""
                , code, index, dataSigned, type, erDesc, transacionID, subject);
            return "{" + respone + "}";
        }
        private static string GetSigningResult(int ret, int index, string type, byte[] output, string CurTransactionId, string subject)
        {
            String result = "";
            switch (ret)
            {
                case (int)SIGNING_RESULT.Success:
                    result = CreateSigningResult(CurTransactionId, ret, index, "", type, Convert.ToBase64String(output), subject);
                    break;
                case (int)SIGNING_RESULT.BadKey:
                    result = CreateSigningResult(ret, index, "Not found certificate", type);
                    break;
                case (int)SIGNING_RESULT.BadInput:
                    result = CreateSigningResult(ret, index, "Bad input", type);
                    break;
                case (int)SIGNING_RESULT.NotFoundPrivateKey:
                    result = CreateSigningResult(ret, index, "Private key not exists", type);
                    break;
                case (int)SIGNING_RESULT.SigningFailed:
                    result = CreateSigningResult(ret, index, "Signing failed", type);
                    break;
                case (int)SIGNING_RESULT.Unknow:
                    result = CreateSigningResult(ret, index, "Exception unknow", type);
                    break;
                case (int)SIGNING_RESULT.SigValidateFailed:
                    result = CreateSigningResult(ret, index, "Signature validate failed", type);
                    break;
                case (int)SIGNING_RESULT.NotSupport:
                    result = CreateSigningResult(ret, index, "File not support", type);
                    break;
                default:
                    result = CreateSigningResult(ret, index, "Exception unknow", type);
                    break;
            }
            return result;
        }
        public static List<JObject> JArrayToArray(JArray jArr)
        {
            List<JObject> list = new List<JObject>();
            using (IEnumerator<JToken> enumerator = jArr.Children().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    JObject jObject = (JObject)enumerator.Current;
                    list.Add(jObject);
                }
            }
            return list;
        }

        public static string SignPDF(List<JObject> listData)
        {
            PdfSigner pdfSigner = new PdfSigner();
            List<string> listStr = new List<string>();
            foreach (var objData in listData)
            {
                try
                {
                    var type = objData[SIGNING_TYPE].Value<string>();
                    var input = objData[SIGNING_INPUT].Value<string>(); // base64
                    var index = objData[SIGNING_INDEX].Value<int>(); // 
                    //var CurTransactionId = objData[TRANSACION_ID].Value<string>();

                    signatureConfig objConf = new signatureConfig();

                    if (objData[SIGNING_SIGREASON] != null) objConf.sigReason = objData[SIGNING_SIGREASON].Value<string>();
                    else objConf.sigReason = "";

                    if (objData[SIGNING_SIGLOCATION] != null) objConf.sigLocation = objData[SIGNING_SIGLOCATION].Value<string>();
                    if (objData[SIGNING_SIGCONTACT] != null) objConf.sigContact = objData[SIGNING_SIGCONTACT].Value<string>();
                    if (objData[SIGNING_LAYER2TEXT] != null) objConf.layer2text = objData[SIGNING_LAYER2TEXT].Value<string>();
                    if (objData[SIGNING_SIGFIELD] != null) objConf.sigFieldName = objData[SIGNING_SIGFIELD].Value<string>();
                    if (objData[SIGNING_VISIBLEMODE] != null) objConf.visibleMode = objData[SIGNING_VISIBLEMODE].Value<int>();
                    else objConf.visibleMode = 0;
                    if (objData[SIGNING_PAGENO] != null) objConf.pageNo = objData[SIGNING_PAGENO].Value<int>();
                    if (objData[SIGNING_RENDERMODE] != null) objConf.rendermode = objData[SIGNING_RENDERMODE].Value<int>();
                    if (objData[SIGNING_LLX] != null) objConf.llX = objData[SIGNING_LLX].Value<float>();
                    if (objData[SIGNING_LLY] != null) objConf.llY = objData[SIGNING_LLY].Value<float>();
                    if (objData[SIGNING_URX] != null) objConf.urX = objData[SIGNING_URX].Value<float>();
                    if (objData[SIGNING_URY] != null) objConf.urY = objData[SIGNING_URY].Value<float>();

                    if (objData[IMAGE_WIDTH] != null) objConf.ImageWidth = objData[IMAGE_WIDTH].Value<int>();
                    if (objData[IMAGE_HEIGHT] != null) objConf.ImageHeight = objData[IMAGE_HEIGHT].Value<int>();

                    if (objData[SIGNING_IMG] != null && objData[SIGNING_IMG].ToString().Length > 0)
                        objConf.img = Convert.FromBase64String(objData[SIGNING_IMG].Value<string>());
                    //objConf.img = Convert.FromBase64String("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");
                    //objConf.withTSA ;  objConf.tsaUrl; objConf.tsaLogin ; objConf.tsaPass
                    if (objData[SIGNING_HASHALG] != null) objConf.hashAlg = objData[SIGNING_HASHALG].Value<string>(); else objConf.hashAlg = "";

                    if (objData[SIGNING_TSA] != null) objConf.withTSA = objData[SIGNING_TSA].Value<bool>();
                    if (objData[SIGNING_TSAURL] != null) objConf.tsaUrl = objData[SIGNING_TSAURL].Value<string>();
                    if (objData[SIGNING_TSALOGIN] != null) objConf.tsaLogin = objData[SIGNING_TSALOGIN].Value<string>();
                    if (objData[SIGNING_TSAPASS] != null) objConf.tsaPass = objData[SIGNING_TSAPASS].Value<string>();

                    if (objData[SIGNING_CERTIFICATION] != null) objConf.CertificationLevel = objData[SIGNING_CERTIFICATION].Value<int>();
                    byte[] output = null;
                    string subject = string.Empty;
                    int ret = (int)SIGNING_RESULT.Unknow;
                    if (type.ToLower().Equals("pdf"))
                    {
                        ret = pdfSigner.Sign(Convert.FromBase64String(input), objConf, out output, out subject);
                    }
                    else
                    {
                        ret = (int)SIGNING_RESULT.NotSupport;
                    }
                    var respone = GetSigningResult(ret, index, type, output, "", subject);
                    listStr.Add(respone);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
            string json = JsonConvert.SerializeObject(listStr);
            return json;
        }
        public static string SignXML(List<JObject> listData, string uri)
        {
            XmlSigner xmlSigner = new XmlSigner();
            List<string> listStr = new List<string>();
            foreach (var objData in listData)
            {
                try
                {
                    var type = objData[SIGNING_TYPE].Value<string>();
                    var input = objData[SIGNING_INPUT].Value<string>(); // base64
                    var index = objData[SIGNING_INDEX].Value<int>(); // 
                    byte[] output = null;
                    string subject = string.Empty;
                    int ret = (int)SIGNING_RESULT.Unknow;
                    if (type.ToLower().Equals("xml"))
                    {
                        ret = xmlSigner.SignXML(Convert.FromBase64String(input), uri, out output, out subject);
                    }
                    else
                    {
                        ret = (int)SIGNING_RESULT.NotSupport;
                    }
                    var respone = GetSigningResult(ret, index, type, output, "", subject);
                    listStr.Add(respone);
                }
                catch (Exception ex)
                {

                }
            }
            string json = JsonConvert.SerializeObject(listStr);
            return json;
        }

        public static string SignPDFANDXML(List<JObject> listData)
        {
            PdfSigner pdfSigner = new PdfSigner();
            XmlSigner xmlSigner = new XmlSigner();
            List<string> listStr = new List<string>();
            foreach (var objData in listData)
            {
                try
                {
                    var type = objData[SIGNING_TYPE].Value<string>();
                    var input = objData[SIGNING_INPUT].Value<string>(); // base64
                    var index = objData[SIGNING_INDEX].Value<int>(); // 
                    //var CurTransactionId = objData[TRANSACION_ID].Value<string>();
                    byte[] output = null;
                    string subject = string.Empty;
                    int ret = (int)SIGNING_RESULT.Unknow;
                    if (type.ToLower().Equals("pdf"))
                    {
                        signatureConfig objConf = new signatureConfig();

                        if (objData[SIGNING_SIGREASON] != null) objConf.sigReason = objData[SIGNING_SIGREASON].Value<string>();
                        else objConf.sigReason = "";

                        if (objData[SIGNING_SIGLOCATION] != null) objConf.sigLocation = objData[SIGNING_SIGLOCATION].Value<string>();
                        if (objData[SIGNING_SIGCONTACT] != null) objConf.sigContact = objData[SIGNING_SIGCONTACT].Value<string>();
                        if (objData[SIGNING_LAYER2TEXT] != null) objConf.layer2text = objData[SIGNING_LAYER2TEXT].Value<string>();
                        if (objData[SIGNING_SIGFIELD] != null) objConf.sigFieldName = objData[SIGNING_SIGFIELD].Value<string>();
                        if (objData[SIGNING_VISIBLEMODE] != null) objConf.visibleMode = objData[SIGNING_VISIBLEMODE].Value<int>();
                        else objConf.visibleMode = 0;
                        if (objData[SIGNING_PAGENO] != null) objConf.pageNo = objData[SIGNING_PAGENO].Value<int>();
                        if (objData[SIGNING_RENDERMODE] != null) objConf.rendermode = objData[SIGNING_RENDERMODE].Value<int>();
                        if (objData[SIGNING_LLX] != null) objConf.llX = objData[SIGNING_LLX].Value<float>();
                        if (objData[SIGNING_LLY] != null) objConf.llY = objData[SIGNING_LLY].Value<float>();
                        if (objData[SIGNING_URX] != null) objConf.urX = objData[SIGNING_URX].Value<float>();
                        if (objData[SIGNING_URY] != null) objConf.urY = objData[SIGNING_URY].Value<float>();

                        if (objData[IMAGE_WIDTH] != null) objConf.ImageWidth = objData[IMAGE_WIDTH].Value<int>();
                        if (objData[IMAGE_HEIGHT] != null) objConf.ImageHeight = objData[IMAGE_HEIGHT].Value<int>();

                        if (objData[SIGNING_IMG] != null && objData[SIGNING_IMG].ToString().Length > 0)
                            objConf.img = Convert.FromBase64String(objData[SIGNING_IMG].Value<string>());
                        //objConf.withTSA ;  objConf.tsaUrl; objConf.tsaLogin ; objConf.tsaPass
                        if (objData[SIGNING_HASHALG] != null) objConf.hashAlg = objData[SIGNING_HASHALG].Value<string>(); else objConf.hashAlg = "";

                        if (objData[SIGNING_TSA] != null) objConf.withTSA = objData[SIGNING_TSA].Value<bool>();
                        if (objData[SIGNING_TSAURL] != null) objConf.tsaUrl = objData[SIGNING_TSAURL].Value<string>();
                        if (objData[SIGNING_TSALOGIN] != null) objConf.tsaLogin = objData[SIGNING_TSALOGIN].Value<string>();
                        if (objData[SIGNING_TSAPASS] != null) objConf.tsaPass = objData[SIGNING_TSAPASS].Value<string>();

                        if (objData[SIGNING_CERTIFICATION] != null) objConf.CertificationLevel = objData[SIGNING_CERTIFICATION].Value<int>();
                        ret = pdfSigner.Sign(Convert.FromBase64String(input), objConf, out output, out subject);
                    }
                    else if (type.ToLower().Equals("xml"))
                    {
                        string FUNC_URI = "uri";
                        string uri = (objData[FUNC_URI] != null) ? objData[FUNC_URI].Value<string>() : "";
                        ret = xmlSigner.SignXML(Convert.FromBase64String(input), uri, out output, out subject);
                    }
                    else
                    {
                        ret = (int)SIGNING_RESULT.NotSupport;
                    }

                    var respone = GetSigningResult(ret, index, type, output, "", subject);
                    listStr.Add(respone);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
            string json = JsonConvert.SerializeObject(listStr);
            return json;
        }
    }
}
