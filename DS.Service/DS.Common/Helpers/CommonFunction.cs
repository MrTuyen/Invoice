using Newtonsoft.Json;
//using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace DS.Common.Helpers
{
    public class CommonFunction
    {
        /// <summary>
        /// Set value to property in object
        /// </summary>
        /// <param name="inputObject">đối tượng</param>
        /// <param name="propertyName">tên thuộc tính</param>
        /// <param name="propertyVal">giá trị</param>
        public static void SetPropertyValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        /// <summary>
        /// Kiểm tra Nullable type object
        /// </summary>
        /// <param name="type">kiểu object cần kiểm tra</param>
        /// <returns></returns>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Convert a object to Integer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int NullSafeInteger(object input, int defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    int outValue;
                    bool success = int.TryParse(input.ToString(), out outValue);
                    if (success)
                        return outValue;
                }
            }
            return defaultValue;
        }

        public static long NullSafeLonger(object input, long defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    long outValue;
                    bool success = long.TryParse(input.ToString(), out outValue);
                    if (success)
                        return outValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Convert a object to Decimal
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal NullSafeDecimal(object input, decimal defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    decimal outValue;
                    bool success = decimal.TryParse(input.ToString(), out outValue);
                    if (success)
                        return outValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Convert a object to Float
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float NullSafeFloat(object input, float defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    float outValue;
                    bool success = float.TryParse(input.ToString(), out outValue);
                    if (success)
                        return outValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Convert a object to Double
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double NullSafeDouble(object input, double defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    double outValue;
                    bool success = double.TryParse(input.ToString(), out outValue);
                    if (success)
                        return outValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// convert an object to String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string NullSafeString(object input, string defaultValue)
        {
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.ToString()))
                {
                    return input.ToString();
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// convert an object to Boolean
        /// </summary>
        /// <param name="input"></param>
        /// <param name="returnIfEmpty"></param>
        /// <returns></returns>
        public static bool NullSafeBoolean(object input, bool returnIfEmpty)
        {
            bool returnValue;
            if (input == DBNull.Value || input == null || input.ToString() == String.Empty)
                returnValue = returnIfEmpty;
            else
            {
                bool flag;
                if (Boolean.TryParse(input.ToString(), out flag))
                    returnValue = flag;
                else
                    returnValue = returnIfEmpty;
            }
            return returnValue;
        }

        /// <summary>
        /// convert an object to DateTime
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime NullSafeDateTime(object input, DateTime defaultValue)
        {
            DateTime DTConversion = DateTime.MinValue;
            try
            {
                if (input != null)
                {
                    DTConversion = (DateTime)input;
                }
                else
                {
                    if (defaultValue != null)
                    {
                        DTConversion = defaultValue;
                    }
                }
            }
            catch (Exception)
            {
            }
            return DTConversion;
        }

        /// <summary>
        /// Đọc dữ liệu trong file
        /// truongnv 20200224
        /// </summary>
        /// <param name="fileName">tên file: vd: key.text hoặc key.json</param>
        /// <returns></returns>
        public static string ReadDataFile(string fileName)
        {
            string data = string.Empty;
            try
            {
                //get the Json filepath  
                string file = HttpContext.Current.Server.MapPath($"~/Config/{fileName}");
                //deserialize JSON from file  
                data = System.IO.File.ReadAllText(file);
            }
            catch { data = string.Empty; }
            return data;
        }

        /// <summary>
        /// Kiểm tra Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string ValidateEmail(string email)
        {
            string msg = string.Empty;
            try
            {
                Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                bool isValid = regex.IsMatch(email.Trim());
                if (!isValid)
                {
                    msg = $"Email không đúng định dạng.";
                }
            }
            catch (Exception ex)
            {
                msg = "Lỗi kiểm tra Email.";
                throw ex;
            }
            return msg;
        }

        /// <summary>
        /// Ký số xml dùng api Cyber
        /// truongnv 20200225
        /// </summary>
        /// <param name="xmlBase64">chuỗi xml dạng base64</param>
        public static async Task<XmlSignedData> SignInvoice(string xmlBase64, string apiUrl, string apiId, string secret)
        {
            XmlSignedData dataSigned = new XmlSignedData();
            try
            {
                //Gọi sang Cyber ký
                using (var client = HttpClientFactoryCustom.CreateHttpClient(apiUrl, apiId, secret))
                {
                    var options = new
                    {
                        base64xml = xmlBase64,
                        hashalg = "SHA1"
                    };
                    var stringPayload = JsonConvert.SerializeObject(options);
                    var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(apiUrl, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var oResult = await result.Content.ReadAsStringAsync();
                        dataSigned = JsonConvert.DeserializeObject<XmlSignedData>(oResult);
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog($"Lỗi ký HSM", ex, "SignInvoice", "signXmlHSMApiMultiple=>SignInvoice");
                dataSigned.status = 0;
            }
            return dataSigned;
        }

        /// <summary>
        /// Convert base64 ra file
        /// truongnv 20200226
        /// </summary>
        /// <param name="base64Content"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string Base64StringToFile(string base64Content, string filePath)
        {
            string msg = string.Empty;
            try
            {
                byte[] content = Convert.FromBase64String(base64Content);
                File.WriteAllBytes(filePath, content);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        /// <summary>
        /// Lấy thông tin đường dẫn file
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="comTaxCode"></param>
        /// <param name="invoiceTypeName"></param>
        /// <returns></returns>
        public static string GetFilePath(string invoiceId, string comTaxCode, string invoiceTypeName)
        {
            string fileName = string.Empty;
            try
            {
                var root = ConfigurationManager.AppSettings["InputInvoiceFolder"];
                var branchComTaxCode = "/" + (string.IsNullOrEmpty(comTaxCode) ? "COMTAXCODE/" : comTaxCode + "/");
                var branchInvoiceID = string.IsNullOrEmpty(invoiceId) ? "INVOICEID/" : invoiceId + "/";
                var branchInvoiceType = string.IsNullOrEmpty(invoiceTypeName) ? "INVOICETYPENAME/" : ElaHelper.FormatKeywordSearch(ElaHelper.FilterVietkey(invoiceTypeName)).Replace(" ", "-");
                var branchDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                var path = string.Format($"{root}{branchComTaxCode}{branchInvoiceID}{branchInvoiceType}");

                DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + path));
                if (!dir.Exists)
                {
                    dir.Create();
                }

                string filePath = dir + "\\" + fileName;
                fileName = filePath.Replace(HttpContext.Current.Server.MapPath("~/" + root), "").Replace('\\', '/');

                return fileName;
            }
            catch
            {
                fileName = string.Empty;
            }
            return fileName;
        }

        public static string GenerateCode()
        {
            string[] str = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M" };
            DateTime now = DateTime.Now;
            string timestring = "" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond;
            long num = Convert.ToInt64(timestring);
            long d = 0;
            string decode = "";
            do
            {
                d = num % str.Length;
                num = (num / str.Length);

                decode += str[d];
            } while (num > 0);

            return decode;
        }

        public static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static bool IsWithin(int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static string FormatMoney(decimal value, int digit, string separator, string groupSeparator)
        {
            // Gets a NumberFormatInfo associated with the en-US culture.
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalSeparator = separator;
            // Displays the same value with four decimal digits.
            nfi.NumberDecimalDigits = digit;
            nfi.NumberGroupSeparator = groupSeparator;
            return value.ToString("N", nfi);
        }

        /// <summary>
        /// Lấy địa chỉ IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.Instance.WriteLog("No network adapters with an IPv4 address in the system!", ex, MethodBase.GetCurrentMethod().Name, "GetLocalIPAddress");
            }
            return string.Empty;
        }

        public static string TransformXMLToHTML(string xmlFile, string xsltFile)
        {
            var transform = new XslCompiledTransform();
            using (var reader = XmlReader.Create(File.OpenRead(xsltFile)))
            {
                transform.Load(reader);
            }

            var results = new StringWriter();
            using (var reader = XmlReader.Create(File.OpenRead(xmlFile)))
            {
                transform.Transform(reader, null, results);
            }

            return results.ToString();
        }

        /// <summary>
        /// Finds web and email addresses in a string and surrounds then with the appropriate HTML anchor tags 
        /// </summary>
        /// <param name="s"></param>
        /// <returns>String</returns>
        public static string WithActiveLinks(string s)
        {
            //Finds URLs with no protocol
            var urlregex = new Regex(@"\b\({0,1}(?<url>(www|ftp)\.[^ ,""\s<)]*)\b",
              RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //Finds URLs with a protocol
            var httpurlregex = new Regex(@"\b\({0,1}(?<url>[^>](http://www\.|http://|https://|ftp://)[^,""\s<)]*)\b",
              RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //Finds email addresses
            var emailregex = new Regex(@"\b(?<mail>[a-zA-Z_0-9.-]+\@[a-zA-Z_0-9.-]+\.\w+)\b",
              RegexOptions.IgnoreCase | RegexOptions.Compiled);
            s = urlregex.Replace(s, " <a href=\"http://${url}\" target=\"_blank\">${url}</a>");
            s = httpurlregex.Replace(s, " <a href=\"${url}\" target=\"_blank\">${url}</a>");
            s = emailregex.Replace(s, "<a href=\"mailto:${mail}\">${mail}</a>");
            return s;
        }

        /// <summary>
        /// Wraps matched strings in HTML span elements styled with a background-color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keywords">Comma-separated list of strings to be highlighted</param>
        /// <param name="cssClass">The Css color to apply</param>
        /// <param name="fullMatch">false for returning all matches, true for whole word matches only</param>
        /// <returns>string</returns>
        public static string HighlightKeyWords(string text, string keywords, string cssClass, bool fullMatch)
        {
            if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
                return text;
            var words = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!fullMatch)
                return words.Select(word => word.Trim()).Aggregate(text,
                             (current, pattern) =>
                             Regex.Replace(current,
                                             pattern,
                                               string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                               cssClass,
                                               "$0"),
                                               RegexOptions.IgnoreCase));
            return words.Select(word => "\\b" + word.Trim() + "\\b")
                        .Aggregate(text, (current, pattern) =>
                                   Regex.Replace(current,
                                   pattern,
                                     string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                     cssClass,
                                     "$0"),
                                     RegexOptions.IgnoreCase));

        }

    }
    public class CommonFunction<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputList">Inumerable<T></param>
        /// <param name="ouputNull">T</param>
        /// <param name="ouputNotNull">T</param>
        /// <returns>T</returns>
        public static T CheckListNull(IEnumerable<T> inputList, T ouputNull, int index)
        {
            return (inputList == null || inputList.Count() == 0) ? ouputNull : inputList.ToList()[index];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputList">Inumerable<string></string></param>
        /// <param name="ouputNull">int</param>
        /// <param name="ouputNotNull">int</param>
        /// <returns>int</returns>
        public static int CheckListNull(IEnumerable<string> inputList, int ouputNull, int index)
        {
            return (inputList == null || inputList.Count() == 0) ? ouputNull : int.Parse(inputList.ToList()[index].Trim());
        }
        public static decimal CheckListNull(IEnumerable<string> inputList, decimal ouputNull, int index)
        {
            return (inputList == null || inputList.Count() == 0) ? ouputNull : decimal.Parse(inputList.ToList()[index].Trim());
        }
    }
}

public class XmlSignedData
{
    public string base64xmlSigned { get; set; }
    public int status { get; set; }
    public string description { get; set; }
}
