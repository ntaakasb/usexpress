using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace DAL
{
    public class CommonDao
    {
        public static DataTable ConvertListStringToTable(List<string> listCols)
        {
            DataTable dt = new DataTable();
            if (listCols.Any())
            {
                foreach (var col in listCols)
                {
                    dt.Columns.Add(col);
                }
            }
            return dt;
        }

         
    }
}
