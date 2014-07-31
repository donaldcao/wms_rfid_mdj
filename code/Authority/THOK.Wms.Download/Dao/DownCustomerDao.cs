using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;
using THOK.Wms.Download.Dao;

namespace THOK.Wms.Download.Dao
{
    public class DownCustomerDao : BaseDao
    {

        /// <summary>
        /// 根据客户编码下载库户信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerInfo(string customerCode)
        {
            string sql = string.Format("SELECT * FROM V_WMS_CUSTOMER WHERE {0}", customerCode);
            return this.ExecuteQuery(sql).Tables[0];
        }


        /// <summary>
        /// 查询客户编码
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerCode()
        {
            string sql = "SELECT customer_code FROM wms_customer";
            return this.ExecuteQuery(sql).Tables[0];
        }


        /// <summary>
        /// 删除客户表
        /// </summary>
        /// <returns></returns>
        public void DeteleCustomer()
        {
            string sql = "DELETE WMS_CUSTOMER";
            this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="customerDt"></param>
        public void Insert(DataSet customerDs)
        {
            this.BatchInsert(customerDs.Tables["DWV_IORG_CUSTOMER"], "wms_customer");
        }

      
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
        /// 客户
        /// </summary>
        /// <returns></returns>
        public DataTable FindCustomer(DateTime orderDate)
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {

                case "gzqdn-oracle":
                    sql = @"SELECT CUST_CODE AS CUSTOMERCODE,LTRIM(RTRIM(PRINCIPAL_NAME)) AS CUSTOMERNAME,
                             A.DELIVER_LINE_CODE AS ROUTECODE,1 AS AREACODE,LICENSE_CODE AS LICENSENO,
                             DELIVER_ORDER AS SORTID,DIST_PHONE AS TELNO,DIST_ADDRESS AS ADDRESS,N_CUST_CODE
                             FROM V_WMS_CUSTOMER A
                             LEFT JOIN V_WMS_DELIVER_LINE B ON A.DELIVER_LINE_CODE = B.DELIVER_LINE_CODE
                             WHERE A.ISACTIVE ='1'";
                    break;

                case "yzyc-db2": //永州烟草
                    sql = string.Format(@"SELECT A.CUSTOMERCODE, A.CUSTOMERNAME, A.ROUTECODE, " +
                            " B.AREACODE AS AREACODE,A.LICENSENO,A.SORTID, A.TELNO," +
                            " A.ADDRESS,A.CUSTOMERCODE AS N_CUST_CODE  FROM OUKANG.OUKANG_CUST A " +
                            " LEFT JOIN OUKANG.OUKANG_RUT B ON A.ROUTECODE = B.ROUTECODE");
                    break;

                default:

                    break;
            }
            return ExecuteQuery(sql).Tables[0];
        }
      

        //客户
        public void SynchronizeCustomer(DataTable customerTable)
        {
            DateTime dt = DateTime.Now;
            foreach (DataRow row in customerTable.Rows)
            {
                string sql = "IF '{0}' IN (SELECT customer_code FROM wms_customer) " +
                                " BEGIN " +
                                    " UPDATE wms_customer SET customer_name = '{1}',deliver_line_code = '{2}',license_code = '{3}',deliver_order = '{4}', " +
                                    " phone = '{5}' ,address = '{6}',custom_code = '{7}'" +
                                    " WHERE customer_code = '{0}' " +
                                " END " +
                             "ELSE " +
                                " BEGIN " +
                                    " INSERT wms_customer VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}') " +
                                " END";
                sql = string.Format(sql, row["CUSTOMERCODE"], row["N_CUST_CODE"], row["CUSTOMERNAME"], ' ', ' ', ' ', ' ', ' ', ' ', ' ', row["ROUTECODE"], row["SORTID"], row["ADDRESS"], row["TELNO"], ' ', row["LICENSENO"], ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '1', dt);
                ExecuteNonQuery(sql);
            }
        }

      
    }
}
