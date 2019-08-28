using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DAL
{
    public class Common
    {
        public string CheckInput(string strInput)
        {
            strInput.Replace("@", "");
            strInput.Replace("^", "");
            strInput.Replace("[", "");
            strInput.Replace("]", "");
            strInput.Replace("'", "");
            strInput.Replace(".", "");
            strInput.Replace("/", "");
            strInput.Replace(".", "");
            strInput.Replace("$", "");
            strInput.Replace("#", "");
            strInput.Replace("|", "");

            return strInput;
        }

        public void WriteLog(string path, string msg)
        {
            if (path.Trim() != "")
            {
                StreamWriter sw;
                string sFullName = string.Format("{0:dd-MM-yyyy}.log", DateTime.Now);
                string sDirName = path + @"\Log\" + string.Format("{0:yyyy}", DateTime.Now) + @"\" + string.Format("{0:MM}", DateTime.Now);

                if (!Directory.Exists(sDirName + "\\")) Directory.CreateDirectory(sDirName + "\\");
                sFullName = sDirName + "\\" + sFullName;
                if (File.Exists(sFullName))
                {
                    sw = System.IO.File.AppendText(sFullName);
                    sw.WriteLine(string.Format("{0:hh:mm:ss tt}", DateTime.Now) + ": " + msg);
                    sw.Flush();
                }
                else
                {
                    sw = new StreamWriter(sFullName);
                    sw.WriteLine(string.Format("{0:hh:mm:ss tt}", DateTime.Now) + ": " + msg);
                }

                if (sw != null) sw.Close();
            }
        }

        public bool CheckVnDate(string strDate, ref System.DateTime dtDate)
        {
            System.Globalization.DateTimeFormatInfo objFormat = new System.Globalization.DateTimeFormatInfo();
            objFormat.ShortDatePattern = "dd-MM-yyyy";
            return System.DateTime.TryParse(strDate, objFormat, System.Globalization.DateTimeStyles.None, out dtDate);
        }

        /// <summary>
        /// Được sử dụng để mã hóa 1 chuỗi text
        /// </summary>
        /// <param name="InputText">Chuỗi cần mã hóa</param>
        /// <returns>Chuỗi sau khi được mã hóa</returns>
        public string HashMD5(string InputText)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            if (string.IsNullOrEmpty(InputText.Trim()))
                return "";
            byte[] arrInput = null;
            arrInput = System.Text.UnicodeEncoding.UTF8.GetBytes(InputText);
            byte[] arrOutput = null;
            arrOutput = MD5.ComputeHash(arrInput);
            return Convert.ToBase64String(arrOutput);
        }


        public string ToMd5(string s)
        {
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] originalBytes = Encoding.Default.GetBytes(s);
            byte[] encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).ToLower().Replace("-", "");
        }

        //Chuyển từ font Unicode UTF-8 sang Unicode không dấu
        public string UnicodeToPlain(string strEncode)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = strEncode.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public string UnicodeToNoneMark(string iInput)
        {
            if (iInput == null)
                return string.Empty;
            if (iInput.Length <= 0)
                return string.Empty;
            iInput = iInput.ToLower();
            System.Text.StringBuilder strOutput = new System.Text.StringBuilder();
            string strMark = "";
            string strChar = null;
            for (int i = 0; i <= iInput.Length - 1; i++)
            {
                strChar = iInput.Substring(i, 1);
                switch (strChar)
                {
                    case "â":
                    case "ê":
                    case "ô":
                    case "đ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        break;
                    case "ấ":
                    case "ế":
                    case "ố":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "s";
                        break;
                    case "ầ":
                    case "ề":
                    case "ồ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "f";
                        break;
                    case "ẩ":
                    case "ể":
                    case "ổ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "r";
                        break;
                    case "ẫ":
                    case "ễ":
                    case "ỗ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "x";
                        break;
                    case "ậ":
                    case "ệ":
                    case "ộ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append(UnicodeToPlain(strChar));
                        strMark = "j";
                        break;
                    case "ă":
                    case "ư":
                    case "ơ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        break;
                    case "ắ":
                    case "ứ":
                    case "ớ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "s";
                        break;
                    case "ằ":
                    case "ừ":
                    case "ờ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "f";
                        break;
                    case "ẳ":
                    case "ử":
                    case "ở":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "r";
                        break;
                    case "ẵ":
                    case "ữ":
                    case "ỡ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "x";
                        break;
                    case "ặ":
                    case "ự":
                    case "ợ":
                        strOutput.Append(UnicodeToPlain(strChar)).Append("w");
                        strMark = "j";
                        break;
                    case "á":
                    case "é":
                    case "í":
                    case "ó":
                    case "ú":
                    case "ý":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "s";
                        break;
                    case "à":
                    case "è":
                    case "ì":
                    case "ò":
                    case "ù":
                    case "ỳ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "f";
                        break;
                    case "ả":
                    case "ẻ":
                    case "ỉ":
                    case "ỏ":
                    case "ủ":
                    case "ỷ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "r";
                        break;
                    case "ã":
                    case "ẽ":
                    case "ĩ":
                    case "õ":
                    case "ũ":
                    case "ỹ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "x";
                        break;
                    case "ạ":
                    case "ẹ":
                    case "ị":
                    case "ọ":
                    case "ụ":
                    case "ỵ":
                        strOutput.Append(UnicodeToPlain(strChar));
                        strMark = "j";
                        break;
                    case "a":
                    case "b":
                    case "c":
                    case "d":
                    case "e":
                    case "f":
                    case "g":
                    case "h":
                    case "i":
                    case "j":
                    case "k":
                    case "l":
                    case "m":
                    case "n":
                    case "o":
                    case "p":
                    case "q":
                    case "r":
                    case "s":
                    case "t":
                    case "u":
                    case "v":
                    case "w":
                    case "x":
                    case "y":
                    case "z":
                        strOutput.Append(strChar);
                        break;
                    default:
                        strOutput.Append(strMark).Append(strChar);
                        strMark = "";
                        break;
                }
            }
            strOutput.Append(strMark);
            return strOutput.ToString();
        }

        //Serialize 1 object để có thể gửi qua WebService
        public byte[] SerializeObject(object iObject)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bf.Serialize(ms, iObject);
            ms.Flush();
            ms.Position = 0;
            byte[] bytBuffer = new byte[ms.Length + 1];
            ms.Read(bytBuffer, 0, Convert.ToInt32(ms.Length));
            ms.Close();
            ms = null;
            bf = null;
            return bytBuffer;
        }

        //Deserialize Object
        public object DeserializeObject(byte[] iInput)
        {
            object ReturnValue = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(iInput);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            ReturnValue = bf.Deserialize(ms);
            bf = null;
            ms.Close();
            ms = null;
            return ReturnValue;
        }

        //Tạo file trong chương trình
        public bool CreateFile(string strFilename, string strContent)
        {
            bool isOK = false;
            string FolderName = "";
            System.IO.StreamWriter sFileCreate = null;

            try
            {
                FolderName = strFilename.Substring(0, strFilename.LastIndexOf("\\"));
                //Kiểm tra sự tồn tại của thư mục
                if (!System.IO.Directory.Exists(FolderName))
                {
                    //Tạo thư mục
                    System.IO.Directory.CreateDirectory(FolderName);
                }
                sFileCreate = new System.IO.StreamWriter(strFilename);
                sFileCreate.WriteLine(strContent);
                sFileCreate.Close();
                isOK = true;
                return isOK;

            }
            catch
            {
                if (sFileCreate != null)
                {
                    sFileCreate.Close();
                    sFileCreate = null;
                }
                return false;
            }
        }

        //Đọc file
        public string ReadEndFile(string strFilename)
        {

            if (!System.IO.File.Exists(strFilename))
                return "File not found 123: " + strFilename;
            System.IO.StreamReader sr = new System.IO.StreamReader(strFilename);
            try
            {
                string strContent = "";
                strContent = sr.ReadToEnd();
                sr.Close();
                sr = null;
                return strContent;
            }
            catch (Exception ex)
            {
                sr.Close();
                return "Error when read file: " + ex.Message;
            }

        }

        //ProcessDate
        public DateTime ConvertStringToDate(string strDate)
        {
            try
            {
                DateTime result;
                IFormatProvider celture = new CultureInfo("fr-FR", true);
                if (strDate.Length < 10)
                {
                    var arrayDate = strDate.Split('/');
                    if (arrayDate[0].Length < 2)
                    {
                        arrayDate[0] = "0" + arrayDate[0];
                    }
                    if (arrayDate[1].Length < 2)
                    {
                        arrayDate[1] = "0" + arrayDate[1];
                    }
                    strDate = arrayDate[0] + "/" + arrayDate[1] + "/" + arrayDate[2];
                }
                result = DateTime.ParseExact(strDate, "dd/MM/yyyy", celture, DateTimeStyles.NoCurrentDateDefault);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public DateTime ConvertStringToDate(string strDate, string format)
        {
            DateTime result;
            try
            {
                IFormatProvider celture = new CultureInfo("fr-FR", true);

                var arrayDate = strDate.Split('-');
                if (arrayDate[0].Length < 2)
                {
                    arrayDate[0] = "0" + arrayDate[0];
                }
                if (arrayDate[1].Length < 2)
                {
                    arrayDate[1] = "0" + arrayDate[1];
                }
                strDate = arrayDate[0] + "-" + arrayDate[1] + "-" + arrayDate[2];
                result = DateTime.ParseExact(strDate, format, celture, DateTimeStyles.NoCurrentDateDefault);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Hàm mã hóa dữ liệu thường dùng 1 Key để mã hóa và giải mã
        public string EncryptString(string strText)
        {
            string strEncrKey = "SuperEditor";
            Byte[] IV = { 12, 13, 14, 15, 16, 17, 18, 19 };
            System.Text.UTF8Encoding byteOriginal = new UTF8Encoding();
            Byte[] InputByteArray = byteOriginal.GetBytes(strText);
            Byte[] bykey = byteOriginal.GetBytes(strEncrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(bykey, IV), CryptoStreamMode.Write);
            cs.Write(InputByteArray, 0, InputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        //Hàm giải mã dữ liệu thường dùng 1 Key để mã hóa và giải mã
        public string DecryptString(string strText)
        {
            string strEncrKey = "SuperEditor";
            Byte[] IV = { 12, 13, 14, 15, 16, 17, 18, 19 };
            Byte[] InputByteArray = new Byte[strText.Length];
            System.Text.UTF8Encoding byteOriginal = new UTF8Encoding();
            Byte[] bykey = byteOriginal.GetBytes(strEncrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            InputByteArray = Convert.FromBase64String(strText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(bykey, IV), CryptoStreamMode.Write);
            cs.Write(InputByteArray, 0, InputByteArray.Length);
            cs.FlushFinalBlock();
            return byteOriginal.GetString(ms.ToArray());
        }

        public bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public string GenLinkFile(string filename)
        {
            filename = UnicodeToPlain(filename);
            filename = Regex.Replace(filename, " ", "-", RegexOptions.Multiline);
            filename = Regex.Replace(filename, "--", "-", RegexOptions.RightToLeft);
            return filename;
        }
        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public string FieldCheck(string sData)
        {
            string functionReturnValue = null;
            if (string.IsNullOrEmpty(sData))
                functionReturnValue = "Null";
            else
                functionReturnValue = "'" + sData.Trim().Replace("'", "''") + "'";
            return functionReturnValue;
        }

        public string FieldUniCk(string sData)
        {
            string functionReturnValue = string.Empty;
            if (string.IsNullOrEmpty(sData))
                functionReturnValue = "Null";
            else
                functionReturnValue = "N'" + sData.Trim().Replace("'", "''") + "'";
            return functionReturnValue;
        }

        public string FieldUniCkEmpty(string sData)
        {
            string functionReturnValue = string.Empty;
            if (string.IsNullOrEmpty(sData))
                functionReturnValue = "''";
            else
                functionReturnValue = "N'" + sData.Trim().Replace("'", "''") + "'";
            return functionReturnValue;
        }

        public string ValueCheck(string sData)
        {
            double dTest = 0;
            if (double.TryParse(sData, out dTest)) return sData.Trim();
            else return "NULL";
        }

        public string DateCheck(DateTime objDate)
        {
            if (objDate == null) return "NULL";
            else
            {
                if (objDate.Year < 2000) return "NULL";
                return "'" + objDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
        }
        public string DateCheck2(DateTime objDate)
        {
            if (objDate == null) return "NULL";
            else
            {
                if (objDate.Year < 1900) return "NULL";
                return "'" + objDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
        }

        //Hàm mã hõa dữ liệu sử dụng Public Key
        public static string RSAEncrypt(string OriginalData, string PublicKey)
        {
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(PublicKey);
                System.Text.UnicodeEncoding byteOriginal = new UnicodeEncoding();
                Byte[] decrypted = byteOriginal.GetBytes(OriginalData);
                Byte[] encrypted = RSA.Encrypt(decrypted, false);
                return System.Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //Hàm giải mã dữ liệu sử dụng Private Key
        public static string RSADecrypt(string EncryptedData, string PrivateKey)
        {
            try
            {
                CspParameters cspParam = new CspParameters();
                cspParam.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(PrivateKey);
                System.Text.UnicodeEncoding byteOriginal = new UnicodeEncoding();
                Byte[] encrypted = System.Convert.FromBase64String(EncryptedData);
                Byte[] decrypted = RSA.Decrypt(encrypted, false);
                return byteOriginal.GetString(decrypted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string FormatDisplayDate(string strDate)
        {
            DateTime objDate;
            if (DateTime.TryParse(strDate, out objDate))
            {
                if (DateTime.Now.Year == objDate.Year)
                {
                    return objDate.ToString("dd MMM, hh:mm tt");

                }
                else
                {
                    return objDate.ToString("dd MMM, yyyy hh:mm tt");
                }

            }
            return strDate;
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string FormatMoney(long Money)
        {
            string strMoney = "<span style=\"color:red\">" + Money.ToString("#,##0") + "</span>";
            if (Money > 0) strMoney = "<span style=\"color:blue\">" + "+" + Money.ToString("#,##0") + "</span>";
            return strMoney;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Money"></param>
        /// <param name="type">1 : thu , 0 : chi</param>
        /// <returns></returns>
        public static string FormatMoney(long Money, int type)
        {
            if (type == 0)
            {
                string strMoney = "<span style=\"color:red\">" + "-" + Money.ToString("#,##0") + "</span>";
                if (Money == 0)
                    strMoney = "<span style=\"color:red\">" + Money.ToString("#,##0") + "</span>";
                return strMoney;
            }
            else
            {

                string strMoney = "<span style=\"color:blue\">" + "+" + Money.ToString("#,##0") + "</span>";
                if (Money == 0)
                    strMoney = "<span style=\"color:blue\">" + Money.ToString("#,##0") + "</span>";
                return strMoney;
            }
        }

        public static string ConvertFullTextSearchString(string strText)
        {
            if (!string.IsNullOrEmpty(strText))
            {
                strText = strText.Trim();
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "\\s+", " AND ");
            }
            else
            {
                strText = "\"\"";
            }
            return strText;
        }

        public static string ConvertMoneyToText(string number)
        {
            string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string doc;
            int i, j, k, n, len, found, ddv, rd;

            len = number.Length;
            number += "ss";
            doc = "";
            found = 0;
            ddv = 0;
            rd = 0;

            i = 0;
            while (i < len)
            {
                //So chu so o hang dang duyet
                n = (len - i + 2) % 3 + 1;

                //Kiem tra so 0
                found = 0;
                for (j = 0; j < n; j++)
                {
                    if (number[i + j] != '0')
                    {
                        found = 1;
                        break;
                    }
                }

                //Duyet n chu so
                if (found == 1)
                {
                    rd = 1;
                    for (j = 0; j < n; j++)
                    {
                        ddv = 1;
                        switch (number[i + j])
                        {
                            case '0':
                                if (n - j == 3) doc += cs[0] + " ";
                                if (n - j == 2)
                                {
                                    if (number[i + j + 1] != '0') doc += "lẻ ";
                                    ddv = 0;
                                }
                                break;
                            case '1':
                                if (n - j == 3) doc += cs[1] + " ";
                                if (n - j == 2)
                                {
                                    doc += "mười ";
                                    ddv = 0;
                                }
                                if (n - j == 1)
                                {
                                    if (i + j == 0) k = 0;
                                    else k = i + j - 1;

                                    if (number[k] != '1' && number[k] != '0')
                                        doc += "mốt ";
                                    else
                                        doc += cs[1] + " ";
                                }
                                break;
                            case '5':
                                if (i + j == len - 1)
                                    doc += "lăm ";
                                else
                                    doc += cs[5] + " ";
                                break;
                            default:
                                doc += cs[(int)number[i + j] - 48] + " ";
                                break;
                        }

                        //Doc don vi nho
                        if (ddv == 1)
                        {
                            doc += dv[n - j - 1] + " ";
                        }
                    }
                }


                //Doc don vi lon
                if (len - i - n > 0)
                {
                    if ((len - i - n) % 9 == 0)
                    {
                        if (rd == 1)
                            for (k = 0; k < (len - i - n) / 9; k++)
                                doc += "tỉ ";
                        rd = 0;
                    }
                    else
                        if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                }

                i += n;
            }

            if (len == 1)
                if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];

            return doc.Substring(0, 1).ToUpper() + doc.Substring(1) + " đồng";
        }

        public static string GetFormatDateByMinute(DateTime date)
        {
            string strTime = string.Empty;
            //string strMinute = string.Empty;
            TimeSpan valueTime = DateTime.Now - date;
            int minute = (int)DateTime.Now.Subtract(date).TotalMinutes;

            if (minute < 60)
            {
                strTime = @"{0} phút trước";
                //strMinute = minute < 10 ? "0" + minute : minute.ToString();
                strTime = string.Format(strTime, minute);
            }
            else if (minute < 1440)
            {
                strTime = @"{0} giờ trước";
                minute = minute / 60;
                //strMinute = minute < 10 ? "0" + minute : minute.ToString();
                strTime = string.Format(strTime, minute);
            }
            else if (minute < 1440 * 30)
            {
                strTime = @"{0} ngày trước";
                minute = minute / 1440;
                //strMinute = minute < 10 ? "0" + minute : minute.ToString();
                strTime = string.Format(strTime, minute);
            }
            else if (minute < 1440 * 30 * 12)
            {
                strTime = @"{0} tháng trước";
                minute = minute / (1440 * 30);
                //strMinute = minute < 10 ? "0" + minute : minute.ToString();
                strTime = string.Format(strTime, minute);
            }
            else
            {
                strTime = @"{0} năm trước";
                minute = minute / (1440 * 30 * 12);
                //strMinute = minute < 10 ? "0" + minute : minute.ToString();
                strTime = string.Format(strTime, minute);
            }
            return strTime;
        }

        public static string HideNumberPhone(string number)
        {
            if (!string.IsNullOrEmpty(number) && number.Length > 3)
            {
                string left = number.Substring(0, 3);
                string right = number.Substring(number.Length - 3, 3);

                return left + "*****" + right;
            }
            return number;

        }

        
        public static string ToHex(byte[] bytes, bool upperCase)
        {
            var result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

        /// <summary>
        /// Làm tròn tiền (hàng triệu)
        /// </summary>
        /// <param name="myNum"></param>
        /// <param name="roundTo"></param>
        /// <returns></returns>
        public static long RoundingTo(long myNum, long roundTo)
        {
            if (roundTo <= 0) return myNum;
            return (myNum + roundTo / 2) / roundTo * roundTo;
        }

        public static string ConvertMinuteToDayHourMinute(int Minute)
        {
            //total hours
            int hours = Minute / 60;
            //total day
            int day = hours / 24;

            int minutes = Minute % 60;

            if (day <= 0)
            {
                return string.Format("{0} giờ , {1} phút", hours, minutes);
            }
            else
            {
                hours = hours % 24;
                //output is 1:10
                var time = string.Format("{0} ngày, {1} giờ, {2} phút", day, hours, minutes);

                return time;
            }
        }
        public static void CaculateIndividualBusiness(long sotienkhachhangdangkyvay, long doanhthutrungbinh3thang, long doanhthuthangthapnhat, int thoihanconlaicuahopdong, long giatritiendatcoc, long giatritienthueconlaikhachhangdadong, long giatritaisan, int hinhthuccutru, int nhomCIC, int loantime, ref long tonghanmucchovay, ref long tonglaivaphithangdau, ref long tongtientattoan)
        {
            decimal T1 = 0.1M; decimal S1 = 0;
            decimal T2 = 0.1M; decimal S2 = 0;
            decimal T3 = 0.15M; decimal S3 = 0;
            decimal T4 = 0.3M; decimal S4 = 0;
            decimal T5 = 0.05M; decimal S5 = 0;
            decimal T6 = 0.3M; decimal S6 = 0;

            // Tinh S1
            S1 = ((thoihanconlaicuahopdong / 6) >= 1) ? 1 : 0;
            // Tinh S2
            S2 = ((giatritiendatcoc + giatritienthueconlaikhachhangdadong) / sotienkhachhangdangkyvay) >= 1 ? 1 : 0;
            // Tinh S3
            S3 = ((giatritaisan / (2 * sotienkhachhangdangkyvay)) >= 1) ? 1 : 0;
            // Tinh S4
            S4 = (((doanhthuthangthapnhat / 2) / sotienkhachhangdangkyvay) >= 1) ? 1 : 0;
            // Tinh S5
            S5 = (hinhthuccutru == 1) ? 1 : 0;
            // Tinh S6
            if (nhomCIC == 1)
                S6 = 1;
            else if (nhomCIC == 2)
                S6 = 0.8M;
            else
                S6 = 0;

            decimal ScoreCreadit = 0;
            ScoreCreadit = T1 * S1 + T2 * S2 + T3 * S3 + T4 * S4 + T5 * S5 + T6 * S6;

            long _hanmuchovay = 0;
            _hanmuchovay = Convert.ToInt64((doanhthutrungbinh3thang * ScoreCreadit) / 2);
            tonghanmucchovay = _hanmuchovay;
            tonghanmucchovay = Common.RoundingTo(_hanmuchovay, 100000);

            long _tienlaivaphithangdau = 0;
            _tienlaivaphithangdau = (tonghanmucchovay * 5000 / 1000000) * 30; //Convert.ToInt64(tonghanmucchovay * (Models.Constants.RateConsultant + Models.Constants.RateService + Models.Constants.Rate) * loantime);
            tonglaivaphithangdau = _tienlaivaphithangdau;

            long _tientattoan = 0;
            _tientattoan = tonghanmucchovay + (tonghanmucchovay * 5000 / 1000000) * 30;// Entity.Common.RoundingTo(tonghanmucchovay, 100000); //Convert.ToInt64(tonghanmucchovay + tonghanmucchovay * (Models.Constants.RateConsultant + Models.Constants.RateService + Models.Constants.Rate) * loantime);
            tongtientattoan = _tientattoan;
        }
    }
    public class QueryString
    {
        public string strTableName;
        public string strSPName = string.Empty;
        public string strConditon;
        System.Collections.Generic.Dictionary<string, string> htData;
        Common objFucntion;

        public QueryString()
        {
            htData = new Dictionary<string, string>();
            objFucntion = new Common();
        }

        public void AddInt(string columnName, int? value)
        {
            if (value.HasValue)
            {
                htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            }
            else
            {
                htData.Add(columnName, "NULL");
            }
        }
        public void AddLong(string columnName, long? value)
        {
            //htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            if (value.HasValue)
            {
                htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            }
            else
            {
                htData.Add(columnName, "NULL");
            }
        }
        public void AddDouble(string columnName, double? value)
        {
            //htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            if (value.HasValue)
            {
                htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            }
            else
            {
                htData.Add(columnName, "NULL");
            }
        }

        public void AddDecimal(string columnName, decimal? value)
        {
            //htData.Add(columnName, objFucntion.ValueCheck(value.ToString())); 
            if (value.HasValue)
            {
                htData.Add(columnName, objFucntion.ValueCheck(value.ToString()));
            }
            else
            {
                htData.Add(columnName, "NULL");
            }
        }

        public void AddString(string columnName, string value)
        {
            htData.Add(columnName, objFucntion.FieldCheck(value));
        }

        public void AddStringUnicode(string columnName, string value)
        {
            htData.Add(columnName, objFucntion.FieldUniCk(value));
        }

        public void AddStringUnicodeEmpty(string columnName, string value)
        {
            htData.Add(columnName, objFucntion.FieldUniCkEmpty(value));
        }
        public void AddDate(string columnName, DateTime? value)
        {
            if (value.HasValue)
            {
                htData.Add(columnName, objFucntion.DateCheck2(value.Value));
            }
            else
            {
                htData.Add(columnName, "NULL");
            }

        }

        public void AddFunction(string columnName, string functionName)
        {
            htData.Add(columnName, functionName);
        }

        public string GenInsertString()
        {
            if (htData.Count == 0) return "";

            string MAIN_SQL;
            string Column_SQL = string.Empty;
            string Value_SQL = string.Empty;
            int i = 0;

            foreach (string key in htData.Keys)
            {
                if (i > 0)
                {
                    Column_SQL += ",";
                    Value_SQL += ",";
                }
                Column_SQL += key;
                Value_SQL += htData[key];
                i++;
            }
            MAIN_SQL = "INSERT INTO {0} ({1}) Values({2})";
            MAIN_SQL = string.Format(MAIN_SQL, this.strTableName, Column_SQL, Value_SQL);
            return MAIN_SQL;
        }

        public string GenUpdateString()
        {
            if (htData.Count == 0) return "";

            string MAIN_SQL = string.Empty;
            string KEY_VALUE = string.Empty;
            int i = 0;

            foreach (string key in htData.Keys)
            {
                if (i > 0)
                {
                    KEY_VALUE += ",";
                }
                KEY_VALUE += key + "=" + htData[key];
                i++;
            }
            if (this.strConditon.Trim().Length > 0)
            {
                MAIN_SQL = "Update {0} Set {1} Where {2}";
                MAIN_SQL = string.Format(MAIN_SQL, this.strTableName, KEY_VALUE, this.strConditon);
            }
            else
            {
                MAIN_SQL = "Update {0} Set {1}";
                MAIN_SQL = string.Format(MAIN_SQL, this.strTableName, KEY_VALUE);
            }

            return MAIN_SQL;
        }

        public string GenSPString()
        {
            if (htData.Count == 0) return "";

            string MAIN_SQL = string.Empty;
            string KEY_VALUE = string.Empty;
            int i = 0;

            foreach (string key in htData.Keys)
            {
                if (i > 0)
                {
                    KEY_VALUE += ",";
                }
                KEY_VALUE += key + "=" + htData[key];
                i++;
            }

            MAIN_SQL = "{0} {1}";
            MAIN_SQL = string.Format(MAIN_SQL, strSPName, KEY_VALUE);

            return MAIN_SQL;
        }

        public void Clear()
        {
            strTableName = "";
            strSPName = "";
            strConditon = "";
            htData.Clear();
        }

        public void Close()
        {
            this.Clear();
            this.htData = null;
            this.objFucntion = null;
        }



    }

}
