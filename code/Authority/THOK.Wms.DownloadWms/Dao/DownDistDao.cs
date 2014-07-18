using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;
using THOK.Wms.DownloadWms.Dao;

namespace THOK.WMS.DownloadWms.Dao
{
    public class DownDistDao : BaseDao
    {
       #region 下载配送中心信息
       
       /// <summary>
       /// 下载配送中心信息
       /// </summary>
       /// <returns></returns>
       public DataTable GetDistInfo()
       {
           string sql = " SELECT * FROM V_WMS_DIST_CTR";
           return this.ExecuteQuery(sql).Tables[0];
       }

       /// <summary>
       /// 添加配送信息到数据库
       /// </summary>
       /// <param name="distTable"></param>
       public void Insert(DataTable distTable)
       {
           this.BatchInsert(distTable, "DWV_OUT_DIST_CTR");
       }

       #endregion

       #region 查询数字仓储配送中心信息

       /// <summary>
       /// 清除配送中心信息
       /// </summary>
       public void Delete()
       {
           string sql = "DELETE DWV_OUT_DIST_CTR";
           this.ExecuteNonQuery(sql);
       }

       /// <summary>
       /// 获取配送中心编码
       /// </summary>
       /// <returns></returns>
       public string GetCompany()
       {
           string sql = "SELECT COM_CODE FROM BI_COMPANY";
           return this.ExecuteScalar(sql).ToString();
       }

       #endregion




       #region   纯分拣数据下载

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
       /// 根据条件下载区域表
       /// </summary>
       /// <returns></returns>
       public DataTable FindArea()
       {
           string sql = "";
           dbTypeName = this.SalesSystemDao();
           switch (dbTypeName)
           {

               case "gzqdn-oracle":
                   sql = @"SELECT DISTINCT 1 AS AREACODE,'全部线路' AS AREANAME,0 AS SORTID
                            FROM V_WMS_DELIVER_LINE WHERE ISACTIVE = '1'";
                   break;

               case "yzyc-db2": //永州烟草
                   sql = string.Format(@"SELECT AREACODE, AREANAME,0 AS SORTID " +
                           " FROM OUKANG.OUKANG_REGION");
                   break;


               default://默认

                   break;
           }
           return this.ExecuteQuery(sql).Tables[0];
       }
       #endregion

       #region   插入数据
       //区域
       public void SynchronizeArea(DataTable areaTable)
       {
           DateTime dt = new DateTime();
           foreach (DataRow row in areaTable.Rows)
           {
               string sql = "IF '{0}' IN (SELECT dist_code FROM wms_deliver_dist) " +
                               "BEGIN " +
                                   "UPDATE wms_deliver_dist SET dist_name = '{1}' WHERE dist_code = '{0}' " +
                               "END " +
                            "ELSE " +
                               "BEGIN " +
                                   "INSERT wms_deliver_dist VALUES ('{0}','{1}','{2},'{3}','{4},'{5}','{6},'{7}','{8},'{9}') " +
                               "END";
               sql = string.Format(sql, row["AREACODE"], ' ', row["AREANAME"], ' ', ' ', ' ', ' ', '1', dt, row["SORTID"]);
               ExecuteNonQuery(sql);
           }
       }

       #endregion

   }
}
