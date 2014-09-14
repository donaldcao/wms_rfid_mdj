﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.Download.Dao
{
    class SysParameterDao : BaseDao
    {
        public Dictionary<string, string> FindParameters()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            DataTable table = ExecuteQuery("SELECT * FROM AUTH_SYSTEM_PARAMETER WHERE PARAMETER_NAME='SalesSystemDBType'").Tables[0];
            foreach (DataRow row in table.Rows)
            {
                d.Add(row["PARAMETER_NAME"].ToString(), row["PARAMETER_VALUE"].ToString());
            }
            return d;
        }

        public string FindDownInterFaceViewName()
        {
            DataTable table = ExecuteQuery("select parameter_value from auth_system_parameter where parameter_name='DownInterFaceViewName'").Tables[0];
            if (table.Rows.Count <= 0)
                return "{0}";
            return table.Rows[0]["parameter_value"].ToString();
        }
    }
}
