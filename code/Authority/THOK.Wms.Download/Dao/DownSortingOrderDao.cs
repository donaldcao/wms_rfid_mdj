using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.Wms.Download.Dao;

namespace THOK.Wms.Download.Dao
{
    public class DownSortingOrderDao : BaseDao
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
        /// 根据条件下载分拣订单主表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortingOrder(string orderid)
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {
                case "mdjyc-db2"://牡丹江烟草db2
                    sql = string.Format(@"SELECT a.*,b.DIST_BILL_ID,b.DELIVERYMAN_CODE,b.DELIVERYMAN_NAME FROM V_WMS_SORT_ORDER A
                                        LEFT JOIN V_WMS_DIST_BILL B ON A.DIST_BILL_ID=B.DIST_BILL_ID WHERE {0} AND A.QUANTITY_SUM>0", orderid);
                    break;
                case "gxyc-db2"://广西烟草db2
                    sql = string.Format(@"SELECT a.*,b.DIST_BILL_ID,b.DELIVERYMAN_CODE,b.DELIVERYMAN_NAME,SUBSTR(A.ORDER_ID,3,8) AS ORDERID FROM V_WMS_SORT_ORDER A
                                        LEFT JOIN V_WMS_DIST_BILL B ON A.DIST_BILL_ID=B.DIST_BILL_ID WHERE {0} AND A.QUANTITY_SUM>0", orderid);
                    break;
                case "gzyc-oracle"://贵州烟草oracle
                    sql = string.Format(@"SELECT a.*,b.DIST_BILL_ID,b.DELIVERYMAN_CODE,b.DELIVERYMAN_NAME,ORDER_ID AS ORDERID FROM V_WMS_SORT_ORDER A
                                        LEFT JOIN V_WMS_DIST_BILL B ON A.DIST_BILL_ID=B.DIST_BILL_ID WHERE {0} AND A.QUANTITY_SUM>0", orderid);
                    break;
                default://默认广西烟草
                    sql = string.Format(@"SELECT a.*,b.DIST_BILL_ID,b.DELIVERYMAN_CODE,b.DELIVERYMAN_NAME,SUBSTR(A.ORDER_ID,3,8) AS ORDERID FROM V_WMS_SORT_ORDER A
                                        LEFT JOIN V_WMS_DIST_BILL B ON A.DIST_BILL_ID=B.DIST_BILL_ID WHERE {0} AND A.QUANTITY_SUM>0", orderid);
                    break;
            }

            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 根据条件下载分拣订单明细表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortingOrderDetail(string orderid)
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {
                case "mdjyc-db2"://牡丹江烟草db2
                    sql = string.Format(@"SELECT A.* ,B.BRAND_N AS BRANDCODE FROM V_WMS_SORT_ORDER_DETAIL A
                                        LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE
                                        LEFT JOIN V_WMS_SORT_ORDER C ON A.ORDER_ID=C.ORDER_ID WHERE {0} ", orderid);
                    break;
                case "gxyc-db2"://广西烟草db2
                    sql = string.Format(@"SELECT A.* ,SUBSTR(A.ORDER_ID,3,8) AS ORDERID,B.BRAND_N AS BRANDCODE FROM V_WMS_SORT_ORDER_DETAIL A
                                        LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE
                                        LEFT JOIN V_WMS_SORT_ORDER C ON A.ORDER_ID=C.ORDER_ID WHERE {0} ", orderid);
                    break;
                case "gzyc-oracle"://贵州烟草oracle
                    sql = string.Format(@"SELECT A.* ,A.ORDER_ID AS ORDERID,B.BRAND_CODE AS BRANDCODE FROM V_WMS_SORT_ORDER_DETAIL A
                                        LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE
                                        LEFT JOIN V_WMS_SORT_ORDER C ON A.ORDER_ID=C.ORDER_ID WHERE {0} ", orderid);
                    break;
                default://默认广西烟草
                    sql = string.Format(@"SELECT A.* ,SUBSTR(A.ORDER_ID,3,8) AS ORDERID,B.BRAND_N AS BRANDCODE FROM V_WMS_SORT_ORDER_DETAIL A
                                        LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE
                                        LEFT JOIN V_WMS_SORT_ORDER C ON A.ORDER_ID=C.ORDER_ID WHERE {0} ", orderid);
                    break;
            }
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 根据条件下载分拣订单主表信息 创联
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortingOrders(string orderid)
        {
            string sql = string.Format("SELECT * FROM IC.V_WMS_SORT_ORDER WHERE {0} AND QUANTITY_SUM>0", orderid);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 根据条件下载分拣订单明细表信息 创联
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortingOrderDetails(string orderid)
        {
            string sql = string.Format("SELECT order_id as order_detail_id,order_id,brand_code,brand_name,brand_unit_code,brand_unit_name,qty_demand,quantity,price,amount,qty_unit FROM IC.V_WMS_SORT_ORDER_DETAIL WHERE {0} ", orderid);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 添加主表数据到表 DWV_OUT_ORDER
        /// </summary>
        /// <param name="ds"></param>
        public void InsertSortingOrder(DataSet ds)
        {
            BatchInsert(ds.Tables["DWV_OUT_ORDER"], "WMS_SORT_ORDER");
        }

        /// <summary>
        /// 添加明细表数据到表 DWV_OUT_ORDER_DETAIL
        /// </summary>
        /// <param name="ds"></param>
        public void InsertSortingOrderDetail(DataSet ds)
        {
            BatchInsert(ds.Tables["DWV_OUT_ORDER_DETAIL"], "WMS_SORT_ORDER_DETAIL");
        }

        /// <summary>
        /// 查询3天之内的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderId(string orderDate)
        {
            string sql = " SELECT ORDER_ID FROM WMS_SORT_ORDER WHERE ORDER_DATE='" + orderDate + "'";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 查询计量单位信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetUnitProduct()
        {
            string sql = "SELECT * FROM WMS_UNIT_LIST";
            return this.ExecuteQuery(sql).Tables[0];
        }



        /// <summary>
        /// 主单
        /// </summary>
        /// <returns></returns>
        public DataTable FindOrder()
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {

                case "gzqdn-oracle":
                    sql = @"SELECT '{0}', {1}, ORDER_ID AS ORDERID,ORG_CODE AS ORGCODE,1 AS AREACODE,
                             DELIVER_LINE_CODE AS ROUTECODE,CUST_CODE AS CUSTOMERCODE,DELIVER_ORDER AS SORTID ,DETAIL_NUM AS DETAILNUM,'0' AS IS_IMPORT 
                             FROM V_WMS_SORT_ORDER
                             WHERE ORDER_DATE = '{2}' AND DELIVER_LINE_CODE NOT IN ({3}) AND ISACTIVE ='1' ";
                    break;

                case "yzyc-db2": //永州烟草
                    sql = @"SELECT '{0}',{1}, A.ORDERID AS ORDERID,'01'AS ORGCODE,A.AREACODE AS AREACODE,A.RUTCODE AS ROUTECODE,
                             A.CUSTOMERCODE AS CUSTOMERCODE,B.SORTID AS SORTID,'0' AS DETAILNUM,'0' AS IS_IMPORT
                            FROM OUKANG.OUKANG_CO A 
                            LEFT JOIN OUKANG_CUST B ON A.CUSTOMERCODE = B.CUSTOMERCODE 
                             WHERE A.ORDERDATE = '{2}' AND B.ROUTECODE NOT IN ({3}) 
                             GROUP BY ORDERID,A.CUSTOMERCODE, A.RUTCODE ,AREACODE,B.SORTID";
                    break;

                default:

                    break;
            }
            return ExecuteQuery(sql).Tables[0];
        }


        /// <summary>
        /// 细单
        /// </summary>
        /// <returns></returns>
        public DataTable FindOrderDetail()
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {
                case "gzqdn-oracle":
                    sql = @"SELECT A.ORDER_DETAIL_ID AS ORDERDETAILID,A.ORDER_ID AS ORDERID,LTRIM(RTRIM(A.BRAND_CODE)) AS CIGARETTECODE, 
                             LTRIM(RTRIM(A.BRAND_NAME)) AS CIGARETTENAME,'条' AS UTINNAME,A.QUANTITY AS QUANTITY,0,0,'{0}',{1},
                             QTY_DEMAND AS QTYDEMAND,PRICE AS PRICE,AMOUNT AS AMOUNT,'0' AS IS_IMPORT,A.QUANTITY AS ORDER_QUANTITY 
                             FROM V_WMS_SORT_ORDER_DETAIL A 
                             LEFT JOIN V_WMS_SORT_ORDER B ON A.ORDER_ID = B.ORDER_ID
                             WHERE B.ORDER_DATE = '{2}' AND B.DELIVER_LINE_CODE NOT IN ({3}) AND A.QUANTITY > 0 ";
                    break;

                case "yzyc-db2": //永州烟草
                    sql = @"SELECT 0 AS ORDERDETAILID, A.ORDERID AS ORDERID, A.CIGARETTECODE AS CIGARETTECODE, B.CIGARETTENAME AS CIGARETTENAME,'条' AS UTINNAME,A.QUANTITY,0,0,'{0}',{1},
                         0 AS QTYDEMAND,0 AS PRICE,0 AS AMOUNT,0 AS IS_IMPORT,A.QUANTITY AS ORDER_QUANTITY 
                         FROM OUKANG.OUKANG_CO A
                         LEFT JOIN OUKANG.OUKANG_ITEM B ON A.CIGARETTECODE = B.CIGARETTECODE
                        LEFT JOIN OUKANG_CUST C ON A.CUSTOMERCODE = C.CUSTOMERCODE 
                        WHERE ORDERDATE = '{2}' AND C.ROUTECODE NOT IN ({3})";
                    break;

                default:

                    break;
            }
            return ExecuteQuery(sql).Tables[0];
        }



        //插入本地订单主表
        public void SynchronizeMaster(DataTable dtData)
        {
            DateTime dt = DateTime.Now;
            foreach (DataRow row in dtData.Rows)
            {
                string sql = "BEGIN " +
                                    "INSERT wms_sort_order VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}'）" +
                             "END";
                sql = string.Format(sql, row["ORDERID"], ' ', ' ', row["ORDERDATE"], ' ', row["CUSTOMERCODE"], row["CUSTOMERNAME"], ' ', ' ', row["DETAILNUM"], row["SORTID"], dt, ' ', '1', dt, row["ROUTECODE"], ' ', '1');
                ExecuteNonQuery(sql);
            }
        }
        //插入本地订单细表
        public void SynchronizeDetail(DataTable detailTable)
        {
            foreach (DataRow row in detailTable.Rows)
            {
                string sql = "BEGIN " +
                                    "INSERT wms_sort_order_detail VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}') " +
                                "END";
                sql = string.Format(sql, row["ORDERDETAILID"], row["ORDERID"], row["CIGARETTECODE"], row["CIGARETTENAME"], ' ', row["UTINNAME"], row["QTYDEMAND"], row["QUANTITY"], row["PRICE"], row["AMOUNT"], row["QUANTITY"], row["ORDER_QUANTITY"]);
                ExecuteNonQuery(sql);
            }
        }
    }
}
