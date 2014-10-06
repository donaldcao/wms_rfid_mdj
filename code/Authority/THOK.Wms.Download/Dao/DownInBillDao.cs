using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.Wms.Download.Dao;

namespace THOK.Wms.Download.Dao
{
    public class DownInBillDao : BaseDao
    {
        public string dbTypeName = "";
        public string SalesSystemDao()
        {
            SysParameterDao parameterDao = new SysParameterDao();
            Dictionary<string, string> parameter = parameterDao.FindParameters();

            //仓储业务数据接口服务器数据库类型
            if (parameter["SalesSystemDBType"] != "")
                dbTypeName = parameter["SalesSystemDBType"];

            return dbTypeName;
        }

        /// <summary>
        /// 查询营销系统入库单据主表
        /// </summary>
        /// <returns></returns>
        public DataTable GetInBillMaster(string inBillNoList)
        {
            SysParameterDao parameterDao = new SysParameterDao();
            string downInterFaceViewName = parameterDao.FindDownInterFaceViewName();
            string sql = string.Format("SELECT * FROM {0} WHERE {1} ", string.Format(downInterFaceViewName, "V_WMS_IN_ORDER"), inBillNoList);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 查询营销系统入库单据主表 创联
        /// </summary>
        /// <returns></returns>
        public DataTable GetInBillMasters(string inBillNoList)
        {
            string sql = string.Format("SELECT * FROM V_WMS_IN_ORDER WHERE {0} AND QUANTITY_SUM>0", inBillNoList);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 查询营销系统入库明细表
        /// </summary>
        /// <returns></returns>
        public DataTable GetInBillDetail(string inBillNoList)
        {
            SysParameterDao parameterDao = new SysParameterDao();
            string downInterFaceViewName = parameterDao.FindDownInterFaceViewName();
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {
                case "mdjyc-db2"://牡丹江烟草db2
                    sql = string.Format("SELECT A.*,B.BRAND_N AS BRANDCODE FROM V_WMS_IN_ORDER_DETAIL A LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE WHERE {0} ", inBillNoList);
                    break;
                case "gxyc-db2"://广西烟草db2
                    sql = string.Format("SELECT A.*,B.BRAND_N AS BRANDCODE FROM V_WMS_IN_ORDER_DETAIL A LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE WHERE {0} ", inBillNoList);
                    break;
                case "gzyc-oracle"://贵州烟草oracle
                    sql = string.Format("SELECT A.*,BRAND_CODE AS BRANDCODE FROM {1} A WHERE {0} ", inBillNoList, string.Format(downInterFaceViewName, "V_WMS_IN_ORDER_DETAIL"));
                    break;
                default://默认广西烟草
                    sql = string.Format("SELECT A.*,B.BRAND_N AS BRANDCODE FROM V_WMS_IN_ORDER_DETAIL A LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE WHERE {0} ", inBillNoList);
                    break;
            }

            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 查询主表7天内单据
        /// </summary>
        /// <returns></returns>
        public DataTable GetBillNo()
        {
            string sql = "SELECT bill_no FROM wms_in_bill_master WHERE bill_date>=DATEADD(DAY, -4, CONVERT(VARCHAR(14), GETDATE(), 112)) ORDER BY bill_date";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 插入主表数据
        /// </summary>
        /// <param name="ds"></param>
        public void InsertInBillMaster(DataSet ds)
        {
            foreach (DataRow row in ds.Tables["WMS_IN_BILLMASTER"].Rows)
            {
                string sql = "INSERT INTO wms_in_bill_master(bill_no,bill_date,bill_type_code,warehouse_code,status,is_active,update_time,operate_person_id" +
                   ") VALUES('" + row["bill_no"] + "','" + row["bill_date"] + "','" + row["bill_type_code"] + "'," +
                   "'" + row["warehouse_code"] + "','" + row["status"] + "','" + row["is_active"] + "','" + row["update_time"] + "','" + row["operate_person_id"] + "')";
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 插入明细表数据
        /// </summary>
        /// <param name="ds"></param>
        public void InsertInBillDetail(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_IN_BILLDETAIL"], "wms_in_bill_detail");
        }

        /// <summary>
        /// 查询当前登陆的操作员
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable FindEmployee(string userName)
        {
            string sql = "SELECT * FROM WMS_EMPLOYEE WHERE USER_NAME='" + userName + "'";
            return this.ExecuteQuery(sql).Tables[0];
        }
    }
}
