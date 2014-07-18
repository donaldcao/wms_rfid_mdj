using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;
using DBRabbit;
using THOK.WMS.DownloadWms.Dao;

namespace THOK.Wms.DownloadWms.Dao
{
    public class DownSalesSystemDao : BaseDao
    {
        /// <summary>
        /// 删除7天之前的数据
        /// </summary>
        public void DeleteHistory(string orderDate)
        {
            try
            {
                using (PersistentManager dbPm = new PersistentManager("master_1"))
                {
                    DownRouteDao dao = new DownRouteDao();
                    dao.SetPersistentManager(dbPm);
                    dao.DeleteHistory(orderDate);
                }
            }
            catch (Exception e)
            {
                throw new Exception("删除数据失败！原因：" + e.Message);
            }
        }
    }
}
