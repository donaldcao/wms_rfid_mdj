using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;
using THOK.Wms.Download.Dao;

namespace THOK.Wms.Download.Dao
{
    public class DownRouteDao : BaseDao
    {
        /// <summary>
        /// ��Ӫ�������ͻ���·����Ϣ
        /// </summary>
        public DataTable GetRouteInfo(string routeCodeList)
        {
            SysParameterDao parameterDao = new SysParameterDao();
            string downInterFaceViewName = parameterDao.FindDownInterFaceViewName();
            string sql = "";
            try
            {
                sql = string.Format(@"SELECT * FROM {0} A  LEFT JOIN {1} B ON A.DELIVER_LINE_CODE=B.DELIVER_LINE_CODE",
                    string.Format(downInterFaceViewName, "V_WMS_DIST_BILL"), string.Format(downInterFaceViewName, "V_WMS_DELIVER_LINE"));
                return this.ExecuteQuery(sql).Tables[0];
            }
            catch (Exception)
            {
                sql = string.Format(@"SELECT * FROM {0} A LEFT JOIN {1} B ON A.DELIVER_LINE_CODE=B.DELIVER_LINE_CODE",
                    string.Format(downInterFaceViewName, "V_DWV_ORD_DIST_BILL"), string.Format(downInterFaceViewName, "V_WMS_DELIVER_LINE"));
                return this.ExecuteQuery(sql).Tables[0];
            }
        }
        /// <summary>
        /// �����ͻ���·����Ϣ ����
        /// </summary>
        public DataTable GetRouteInfos(string routeCodeList)
        {
            string sql = string.Format("SELECT * FROM IC.V_WMS_DELIVER_LINE WHERE {0}", routeCodeList);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// �ӷּ������ͻ���·����Ϣ
        /// </summary>
        public DataTable GetSortRouteInfo(string routeCodeList)
        {
            string sql = string.Format(@"SELECT DIST_BILL_ID,DELIVERYMAN_CODE,DELIVERYMAN_NAME,DELIVERLINECODE,DELIVERLINENAME FROM SORTORDER {0}
                                               GROUP BY DIST_BILL_ID,DELIVERYMAN_CODE,DELIVERYMAN_NAME,DELIVERLINECODE,DELIVERLINENAME", routeCodeList);
            return this.ExecuteQuery(sql).Tables[0];
        }

    
        /// <summary>
        /// ��ѯ��·���е���·����
        /// </summary>
        /// <returns></returns>
        public DataTable GetRouteCode()
        {
            string sql = " SELECT deliver_line_code FROM wms_deliver_line";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����·��Ϣ�������ݿ�
        /// </summary>
        /// <param name="ds"></param>
        public void Insert(DataSet ds)
        {
            BatchInsert(ds.Tables["DWV_OUT_DELIVER_LINE"], "wms_deliver_line");
        }


        //ɾ��7��֮ǰ����·�����ȱ��ּ��м��ͷּ������ϸ��,��ҵ���ȱ�
        public void DeleteTable()
        {
            string sql = @" DELETE WMS_SORT_ORDER_DETAIL WHERE ORDER_ID IN(
                            SELECT ORDER_ID FROM  WMS_SORT_ORDER WHERE ORDER_DATE<
                            DATEADD(DAY, -7, CONVERT(VARCHAR(14), GETDATE(), 112)))

                            DELETE WMS_SORT_ORDER WHERE ORDER_DATE<
                            DATEADD(DAY, -7, CONVERT(VARCHAR(14), GETDATE(), 112))

                            DELETE WMS_SORT_ORDER_DISPATCH WHERE ORDER_DATE<
                            DATEADD(DAY, -7, CONVERT(VARCHAR(14), GETDATE(), 112))

                            DELETE WMS_DELIVER_LINE WHERE UPDATE_TIME<
                            DATEADD(DAY, -7, CONVERT(VARCHAR(14), GETDATE(), 112)) 

                            DELETE WMS_SORT_WORK_DISPATCH WHERE ORDER_DATE<
                            DATEADD(DAY, -7, CONVERT(VARCHAR(14), GETDATE(), 112))";
            this.ExecuteNonQuery(sql);
        }

        public string dbTypeName = "";
        public string SalesSystemDao()
        {
            SysParameterDao parameterDao = new SysParameterDao();
            Dictionary<string, string> parameter = parameterDao.FindParameters();

            //�ִ�ҵ�����ݽӿڷ��������ݿ�����
            if (parameter["SalesSystemDBType"] != "")
                dbTypeName = parameter["SalesSystemDBType"];

            return dbTypeName;
        }


        /// <summary>
        /// ��·��
        /// </summary>
        /// <returns></returns>
        public DataTable FindRoute()
        {
            string sql = "";
            dbTypeName = this.SalesSystemDao();
            switch (dbTypeName)
            {

                case "gzqdn-oracle":
                    sql = @"SELECT DELIVER_LINE_CODE AS ROUTECODE, DELIVER_LINE_NAME AS ROUTENAME, 1 AS AREACODE, DELIVER_LINE_ORDER AS SORTID
                             FROM V_WMS_DELIVER_LINE WHERE ISACTIVE = '1'";
                    break;

                case "yzyc-db2":
                    sql = @"SELECT ROUTECODE,ROUTENAME, AREACODE, SORTID " +
                            " FROM OUKANG.OUKANG_RUT";
                    break;

                default:

                    break;
            }
            return ExecuteQuery(sql).Tables[0];
        }

        //������·��
        public void SynchronizeRoute(DataTable routeTable)
        {

            DateTime dt = DateTime.Now;
            foreach (DataRow row in routeTable.Rows)
            {
                string sql = "IF '{0}' IN (SELECT deliver_line_code FROM wms_deliver_line) " +
                                "BEGIN " +
                                    "UPDATE wms_deliver_line SET deliver_line_name = '{1}',dist_code = '{2}' WHERE deliver_line_code = '{0}' " +
                                "END " +
                             "ELSE " +
                                "BEGIN " +
                                    "INSERT wms_deliver_line VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') " +
                                "END";
                sql = string.Format(sql, row["ROUTECODE"], ' ', row["ROUTENAME"], row["AREACODE"], row["SORTID"], ' ', '1', dt, ' ');
                ExecuteNonQuery(sql);
            }
        }


        //ɾ����ʷ��¼����7 ��·�����ȱ��ּ��м��ͷּ������ϸ��,��ҵ���ȱ�
        public void DeleteHistory(string orderDate)
        {
            string sql = @"DELETE wms_sort_work_dispatch where update_time< CONVERT(VARCHAR(14),DATEADD(DAY, -7, CONVERT(VARCHAR(100), GETDATE(), 112)),112)
                           DELETE wms_sort_order_dispatch where update_time< CONVERT(VARCHAR(14),DATEADD(DAY, -7, CONVERT(VARCHAR(100), GETDATE(), 112)),112)
                           DELETE wms_sort_order_detail where order_id in (select order_id from wms_sort_order where update_time< CONVERT(VARCHAR(14),DATEADD(DAY, -7, CONVERT(VARCHAR(100), GETDATE(), 112)),112))
                           DELETE wms_sort_order where update_time< CONVERT(VARCHAR(14),DATEADD(DAY, -7, CONVERT(VARCHAR(100), GETDATE(), 112)),112)
                           DELETE wms_deliver_line where update_time< CONVERT(VARCHAR(14),DATEADD(DAY, -7, CONVERT(VARCHAR(100), GETDATE(), 112)),112)";
            this.ExecuteNonQuery(sql);
        }
    }
}
