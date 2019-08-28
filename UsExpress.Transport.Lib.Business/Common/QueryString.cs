using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsExpress.Transport.Lib.Business.Common
{
    public class QueryString
    {
        public string strTableName;
        public string strSPName = string.Empty;
        public string strConditon;
        System.Collections.Generic.Dictionary<string, string> htData;
        LibCommon objFucntion;

        public QueryString()
        {
            htData = new Dictionary<string, string>();
            objFucntion = new LibCommon();
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
