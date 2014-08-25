using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.Download.Dao
{
    public class DownOutBillDao : BaseDao
    {
        /// <summary>
        /// ��Ӫ��ϵͳ���س��ⵥ������������
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMaster(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER WHERE {0} AND ORDER_TYPE='70'", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }

        #region ��Ӫ��ϵͳ���س��ⵥ�����������ݣ�ĵ�����˳���
        /// <summary>��Ӫ��ϵͳ���س��ⵥ������������ 2013-09-09 21:56:33 ĵ�����˳� JJ</summary>
        public DataTable GetOutBillMaster2(string outBillNoList)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER WHERE {0} ", outBillNoList);
            return this.ExecuteQuery(sql).Tables[0];
        }
        #endregion

        /// <summary>
        /// ��Ӫ��ϵͳ���س��ⵥ������������  ����
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMasters(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER WHERE {0} AND ORDER_TYPE='1003'", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// ��Ӫ��ϵͳ���س��ⵥ��ϸ������
        /// </summary>
        /// <returns></returns>
        public DataTable OutBillDetail(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER_DETAIL WHERE {0}", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ��Ӫ��ϵͳ���س��ⵥ��ϸ������,�ϵ�����
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillDetail(string outBillNo)
        {
            string sql = string.Format(@"SELECT BRAND_CODE,SUM(QUANTITY) AS QUANTITY,PRICE FROM V_WMS_OUT_ORDER_DETAIL 
                                        WHERE {0} GROUP BY BRAND_CODE,PRICE ORDER BY BRAND_CODE", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }

        #region ��Ӫ��ϵͳ���س��ⵥ��ϸ�����ݣ�ĵ�����˳���
        /// <summary>��Ӫ��ϵͳ���س��ⵥ��ϸ������,�ϵ����� 2013-09-10 09:27:45 JJ</summary>
        public DataTable GetOutBillDetail2(string outBillNoList)
        {
            string sql = string.Format("SELECT A.*,B.BRAND_N AS BRANDCODE FROM V_WMS_OUT_ORDER_DETAIL A LEFT JOIN V_WMS_BRAND B ON A.BRAND_CODE=B.BRAND_CODE WHERE {0} ", outBillNoList);
            return this.ExecuteQuery(sql).Tables[0];
        } 
        #endregion

        /// <summary>
        /// �����������ݵ��� WMS_OUT_BILLMASTER
        /// </summary>
        /// <param name="ds"></param>
        public void InsertOutBillMaster(DataSet ds)
        {
            foreach (DataRow row in ds.Tables["WMS_OUT_BILLMASTER"].Rows)
            {
                string sql = "INSERT INTO WMS_OUT_BILL_MASTER(bill_no,bill_date,bill_type_code,warehouse_code,status,is_active,update_time,operate_person_id,origin" +
                   ") VALUES('" + row["bill_no"] + "','" + row["bill_date"] + "','" + row["bill_type_code"] + "'," +
                   "'" + row["warehouse_code"] + "','" + row["status"] + "','" + row["is_active"] + "','" + row["update_time"] + "','" + row["operate_person_id"] + "','" + row["origin"] + "')";
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// ��ѯ��ǰ��½�Ĳ���Ա
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable FindEmployee(string userName)
        {
            string sql = "SELECT * FROM WMS_EMPLOYEE WHERE USER_NAME='" + userName + "'";
            return this.ExecuteQuery(sql).Tables[0];
        } 
        /// <summary>
        /// ������ϸ�����ݵ��� WMS_OUT_BILLDETAIL
        /// </summary>
        /// <param name="ds"></param>
        public void InsertOutBillDetail(DataSet ds)
        {
            if (ds.Tables["WMS_OUT_BILLDETAILA"].Rows.Count > 0)
            {
                BatchInsert(ds.Tables["WMS_OUT_BILLDETAILA"], "wms_out_bill_detail");
            }
        }

        /// <summary>
        /// �������ݵ��м��
        /// </summary>
        /// <param name="ds"></param>
        public void InsertMiddle(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_MIDDLE_OUT_BILLDETAIL"], "WMS_MIDDLE_OUT_BILL");
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        public DataSet FindOutBillNo(string orderDate)
        {
            string sql = string.Format("SELECT TOP 1 BILL_NO FROM WMS_OUT_BILL_MASTER WHERE BILL_NO LIKE '{0}%' ORDER BY BILL_NO DESC", orderDate);
            return this.ExecuteQuery(sql);
        }

        /// <summary>
        /// ����ʱ���ѯ�ֿ�ĳ��ⵥ�ݺ�
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillNo(string orderDate)
        {
            string sql = "SELECT BILL_NO FROM WMS_MIDDLE_OUT_BILL WHERE BILL_DATE='" + orderDate + "'";
            return this.ExecuteQuery(sql).Tables[0];
        }

        #region ����ʱ���ѯ�ֿ�ĳ��ⵥ�ݺţ�ĵ�����˳���
        /// <summary>����ʱ���ѯ�ֿ�ĳ��ⵥ�ݺ� 2013-09-10 08:51:28 JJ</summary>
        public DataTable GetOutBillNo2(string orderDate)
        {
            string sql = "SELECT bill_no FROM wms_out_bill_master WHERE bill_date='" + orderDate + "'";
            return this.ExecuteQuery(sql).Tables[0];
        } 
        #endregion
    }
}