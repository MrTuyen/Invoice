using System;

namespace DS.Common.Helpers
{
    public static class ReadNumberToCurrencyWords
    {
        public static string ConvertToWords(decimal total)
        {
            try
            {
                string rs = "";
                total = Math.Round(total, MidpointRounding.AwayFromZero);
                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "nghìn", "", "", "triệu", "", "", "tỷ", "", "", "nghìn", "", "", "triệu" };
                string nstr = total.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            rs += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                rs += " " + rch[n[i]];// đọc số 
                                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
                    rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (rs[rs.Length - 1] != ' ')
                    rs += " đồng";
                else
                    rs += "đồng";

                if (rs.Length > 2)
                {
                    string rs1 = rs.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    rs = rs.Substring(2);
                    rs = rs1 + rs;
                }
                return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",","");
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToWordsWithPostfix(decimal total, string currency)
        {
            decimal originTotal = total;
            string postfixMoney = "";
            string rs = "";
            string percentStr = string.Empty;
            try
            {
                var valueString = "";
                if (currency == "USD")
                {
                    valueString = total.ToString("N3");

                }
                else
                {
                    total.ToString("N2");
                }
                //var valueString = total.ToString();
                var decimalString = valueString.Substring(valueString.LastIndexOf('.') + 1);
                var c = total.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string v2 = string.Empty;
                decimal vVal = 0;
                if (currency == "USD" && c.Length > 1)
                {
                    vVal = CommonFunction.NullSafeDecimal(decimalString, 0);
                    percentStr = vVal > 0 ? NumberToWordsPercent(vVal, currency) : string.Empty;
                }

                vVal = CommonFunction.NullSafeDecimal(c[0].ToString(), 0);
                total = vVal;
                switch (currency)
                {
                    case "USD": { postfixMoney = "Đô La Mỹ"; } break;
                    case "SGD": { postfixMoney = "Đô la Singapore"; } break;
                    case "EUR": { postfixMoney = "Đồng Euro"; } break;
                    case "JPY": { postfixMoney = "Yên Nhật"; } break;
                    case "CAD": { postfixMoney = "Đô la Canada"; } break;
                    case "CNY": { postfixMoney = "Nhân dân tệ Trung Quốc"; } break;
                    case "AUD": { postfixMoney = "Đô la Úc"; } break;
                    case "THB": { postfixMoney = "Bạt Thái"; } break;
                    case "HKD": { postfixMoney = "Đô la Hồng Kông"; } break;
                    case "NZD": { postfixMoney = "Đô la Newzealand"; } break;
                    case "CHF": { postfixMoney = "Phơ răng Thụy Sỹ"; } break;
                    case "GBP": { postfixMoney = "Bảng Anh"; } break;
                    default:
                        { postfixMoney = "đồng"; }
                        break;
                }
                //total = Math.Round(total, MidpointRounding.AwayFromZero);
                if (currency != "VND")
                {
                    total = Math.Round(total, MidpointRounding.AwayFromZero); // Làm tròn số ban đầu thay vì số tách ra từ dấu phẩy động
                }
                else
                {
                    total = Math.Round(originTotal, MidpointRounding.AwayFromZero); // Làm tròn số ban đầu thay vì số tách ra từ dấu phẩy động
                }
                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "nghìn", "", "", "triệu", "", "", "tỷ", "", "", "nghìn", "", "", "triệu" };
                string nstr = total.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            rs += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                rs += " " + rch[n[i]];// đọc số 
                                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
                    rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (rs[rs.Length - 1] != ' ')
                    rs += " " + postfixMoney;
                else
                    rs += postfixMoney;

                if (rs.Length > 2)
                {
                    string rs1 = rs.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    rs = rs.Substring(2);
                    rs = rs1 + rs;
                }
                return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",", "") + (percentStr.Length > 0 ? $" và {char.ToLower(percentStr[0]) + percentStr.Substring(1)}." : string.Empty);
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToWordsWithPostfixNumberPlace(decimal total, string currency, int numberPlace)
        {
            decimal originTotal = total;
            string postfixMoney = "";
            string rs = "";
            string percentStr = string.Empty;
            try
            {
                var valueString = "";
                if (currency == "USD")
                    valueString = total.ToString("N" + numberPlace.ToString());
                else
                    valueString = total.ToString("N2");

                var decimalString = valueString.Substring(valueString.LastIndexOf('.') + 1);
                var c = total.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string v2 = string.Empty;
                decimal vVal = 0;
                if (currency == "USD" && c.Length > 1)
                {
                    //vVal = CommonFunction.NullSafeDecimal(decimalString, 0);
                    //percentStr = vVal > 0 ? NumberToWordsPercent(vVal, currency) : string.Empty;

                    //vVal = CommonFunction.NullSafeDecimal(decimalString, 0);
                    //percentStr = vVal > 0 ? ReadNumber(vVal) : string.Empty;

                    vVal = CommonFunction.NullSafeDecimal(decimalString, 0);
                    percentStr = vVal > 0 ? ReadNumber(decimalString) : string.Empty;
                }

                vVal = CommonFunction.NullSafeDecimal(c[0].ToString(), 0);
                total = vVal;
                switch (currency)
                {
                    case "USD": { postfixMoney = "Đôla Mỹ"; } break;
                    case "SGD": { postfixMoney = "Đôla Singapore"; } break;
                    case "EUR": { postfixMoney = "Đồng Euro"; } break;
                    case "JPY": { postfixMoney = "Yên Nhật"; } break;
                    case "CAD": { postfixMoney = "Đôla Canada"; } break;
                    case "CNY": { postfixMoney = "Nhân dân tệ Trung Quốc"; } break;
                    case "AUD": { postfixMoney = "Đôla Úc"; } break;
                    case "THB": { postfixMoney = "Bạt Thái"; } break;
                    case "HKD": { postfixMoney = "Đôla Hồng Kông"; } break;
                    case "NZD": { postfixMoney = "Đôla Newzealand"; } break;
                    case "CHF": { postfixMoney = "Phơ răng Thụy Sỹ"; } break;
                    case "GBP": { postfixMoney = "Bảng Anh"; } break;
                    default:
                        { postfixMoney = "đồng"; }
                        break;
                }
                //total = Math.Round(total, MidpointRounding.AwayFromZero);
                if (currency != "VND")
                {
                    total = Math.Round(total, MidpointRounding.AwayFromZero); // Làm tròn số ban đầu thay vì số tách ra từ dấu phẩy động
                }
                else
                {
                    total = Math.Round(originTotal, MidpointRounding.AwayFromZero); // Làm tròn số ban đầu thay vì số tách ra từ dấu phẩy động
                }
                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "nghìn", "", "", "triệu", "", "", "tỷ", "", "", "nghìn", "", "", "triệu" };
                string nstr = total.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            rs += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                rs += " " + rch[n[i]];// đọc số 
                                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
                    rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (rs[rs.Length - 1] != ' ')
                {
                    if (currency != "VND") ;
                    else
                        rs += " " + postfixMoney;
                }
                else
                {
                    if (currency != "VND") ;
                    else
                        rs += postfixMoney;
                }

                if (rs.Length > 2)
                {
                    string rs1 = rs.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    rs = rs.Substring(2);
                    rs = rs1 + rs;
                }
                string result = rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",", "") + (percentStr.Length > 0 ? $" và {char.ToLower(percentStr[0]) + percentStr.Substring(1)}." : string.Empty);
                if (currency != "VND")
                {
                    result = rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",", "") + " " + (percentStr.Length > 0 ? $" {percentStr}" : string.Empty) + postfixMoney;
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string NumberToWordsPercent(decimal total2, string currency2)
        {
            string percentText = string.Empty;
            string result = string.Empty;
            try
            {
                switch (currency2)
                {
                    case "USD": { percentText = "cents"; } break;
                }

                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "nghìn", "", "", "triệu", "", "", "tỷ", "", "", "nghìn", "", "", "triệu" };
                string nstr = total2.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                result += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            result += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            result += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            result += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            result += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                result += " " + rch[n[i]];// đọc số 
                                result += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    result += (result == "" ? " " : ", ") + ch[n[i]];// đọc số
                    result += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (result[result.Length - 1] != ' ')
                    result += " " + percentText;
                else
                    result += percentText;

                if (result.Length > 2)
                {
                    string rs1 = result.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    result = result.Substring(2);
                    result = rs1 + result;
                }
                return result.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",", "");
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ReadNumber(decimal number)
        {
            string result = "phẩy ";
            string numberString = number.ToString();
            foreach (char item in numberString)
            {
                switch (item)
                {
                    case '0': result += "không "; break;
                    case '1': result += "một "; break;
                    case '2': result += "hai "; break;
                    case '3': result += "ba "; break;
                    case '4': result += "bốn "; break;
                    case '5': result += "năm "; break;
                    case '6': result += "sáu "; break;
                    case '7': result += "bảy "; break;
                    case '8': result += "tám "; break;
                    case '9': result += "chín "; break;
                    default:
                        break;
                }
            }

            //switch (currency)
            //{
            //    case "USD": result += "Đôla Mỹ"; break;
            //    default:
            //        break;
            //}
            return result;
        }

        public static string ReadNumber(string number)
        {
            string result = "phẩy ";
            string numberString = number.ToString();
            foreach (char item in numberString)
            {
                switch (item)
                {
                    case '0': result += "không "; break;
                    case '1': result += "một "; break;
                    case '2': result += "hai "; break;
                    case '3': result += "ba "; break;
                    case '4': result += "bốn "; break;
                    case '5': result += "năm "; break;
                    case '6': result += "sáu "; break;
                    case '7': result += "bảy "; break;
                    case '8': result += "tám "; break;
                    case '9': result += "chín "; break;
                    default:
                        break;
                }
            }

            //switch (currency)
            //{
            //    case "USD": result += "Đôla Mỹ"; break;
            //    default:
            //        break;
            //}
            return result;
        }
    }
}
