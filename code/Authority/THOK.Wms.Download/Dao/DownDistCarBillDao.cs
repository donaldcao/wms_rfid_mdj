using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.Download.Dao
{
    public class DownDistCarBillDao : BaseDao
    {
        public DataTable GetOrderInfo(string orderDate)
        {
            string sql = string.Format("SELECT DIST_BILL_ID FROM V_WMS_SORT_ORDER WHERE ORDER_DATE='{0}'  GROUP BY DIST_BILL_ID ", orderDate);
            return this.ExecuteQuery(sql).Tables[0];
        }

        public DataTable GetDistCarBillInfo(string distCarCode)
        {
            SysParameterDao parameterDao = new SysParameterDao();
            string downInterFaceViewName = parameterDao.FindDownInterFaceViewName();
            string sql = "";
            try
            {
                sql = string.Format("SELECT * FROM {1} WHERE {0}", distCarCode, string.Format(downInterFaceViewName, "V_WMS_DIST_BILL"));
                return this.ExecuteQuery(sql).Tables[0];
            }
            catch (Exception)
            {
                sql = string.Format("SELECT * FROM {1} WHERE {0}", distCarCode, string.Format(downInterFaceViewName, "V_DWV_ORD_DIST_BILL"));
                return this.ExecuteQuery(sql).Tables[0];
            }
        }

        public DataTable GetDistStationCode()
        {
            string sql = " SELECT DIST_BILL_ID FROM WMS_ORD_DIST_BILL";
            return this.ExecuteQuery(sql).Tables[0];
        }

        public void Delete()
        {
            string sql = "DELETE WMS_ORD_DIST_BILL";
            this.ExecuteNonQuery(sql);
        }

        public void Insert(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_ORD_DIST_BILL"], "WMS_ORD_DIST_BILL");
        }
    }
}
