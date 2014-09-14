using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using System.Data;

namespace THOK.SMS.Bll.Service
{
    public class ChannelService : ServiceBase<Channel>, IChannelService
    {
        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        //烟道类型判断
        public string whatChannelType(string channelType)
        {

            string str = "";
            switch (channelType)
            {
                case "1":
                    str = "立式机";
                    break;
                case "2":
                    str = "立式机";
                    break;
                case "3":
                    str = "通道机";
                    break;
                case "4":
                    str = "卧式机";
                    break;
                case "5":
                    str = "混合烟道";
                    break;
            }

            return str;
        }



        /// <summary>
        /// 获取烟道信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">每页行数</param>
        /// <param name="channel">条件</param>
        /// <returns></returns>
        public object GetDetails(int page, int rows, Channel channel)
        {

            var channelQuery = ChannelRepository.GetQueryable();
            var productQuery = ProductRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();

            var channelDetails = channelQuery.Where(a => a.IsActive.Contains(channel.IsActive));
            //烟道组          
            if (channel.GroupNo>0)
            {
                channelDetails = channelDetails.Where(a => a.GroupNo == channel.GroupNo);
            }
            //烟道类型

            if (channel.ChannelType != null&&channel.ChannelType!=string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.ChannelType == channel.ChannelType);
            }
            //分拣线
            if (channel.SortingLineCode != null && channel.SortingLineCode != string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.SortingLineCode == channel.SortingLineCode);
            }
            //卷烟
            if (channel.ProductCode != null&& channel.ProductCode!= string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.ProductCode == channel.ProductCode);
            }
            //烟道
            if (channel.ChannelCode != null && channel.ChannelCode != string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.ChannelCode == channel.ChannelCode);
            }
            if (channel.ChannelName != null && channel.ChannelName != string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.ChannelName == channel.ChannelName);
            }

            //是否留烟
            if (channel.IsAcceptRemainQuantity.ToString() != null && channel.IsAcceptRemainQuantity.ToString() != string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.IsAcceptRemainQuantity.ToString() == channel.IsAcceptRemainQuantity.ToString());
            }

            channelDetails = channelDetails.OrderBy(c => c.SortingLineCode).ThenBy(c => c.GroupNo).ThenBy(c => c.SortAddress);
            int total = channelDetails.Count();
            var channelArray = channelDetails.Skip((page - 1) * rows).Take(rows).ToArray();

            var channelSkip = channelArray.Select(c => new
            {
                c.ChannelCode,
                c.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortingLineCode).Select(s => s.SortingLineName),
                c.ChannelName,
                ChannelType = whatChannelType(c.ChannelType),
                c.LedNo,
                c.ProductCode,
                c.ProductName,
                c.RemainQuantity,
                c.X,
                c.Y,
                c.Width,
                c.Height,
                c.ChannelCapacity,
                c.SupplyAddress,
                c.SortAddress,
                GroupNo = c.GroupNo == 1 ? "A线" : "B线",
                c.OrderNo,
                IsActive = c.IsActive == "1" ? "启用" : "不启用",
                IsAcceptRemainQuantity = c.IsAcceptRemainQuantity.ToString()== "False" ? "不留烟" : "留烟" //false=0 不留烟
            });
            return new { total, rows = channelSkip.ToArray() };
        }

        /// <summary>
        /// 新增烟道信息
        /// </summary>
        /// <param name="channel">烟道实体</param>
        /// <param name="strResult">相关信息</param>
        /// <returns>是否成功</returns>
        public bool Add(Channel channel, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var channelCode = ChannelRepository.GetQueryable().FirstOrDefault(c => c.ChannelCode == channel.ChannelCode);
            if (channelCode == null)
            {
                try
                {
                    channel.UpdateTime = DateTime.Now;
                    ChannelRepository.Add(channel);
                    ChannelRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            else
            {
                strResult = "原因：该编号已存在！";
            }
            return result;
        }

        /// <summary>
        /// 编辑烟道信息
        /// </summary>
        /// <param name="channel">需要编辑的烟道实体</param>
        /// <param name="strResult">相关信息</param>
        /// <returns>是否成功</returns>
        public bool Edit(Channel channel, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Channel newChannel = ChannelRepository.GetQueryable().FirstOrDefault(c => c.ChannelCode == channel.ChannelCode);
            if (newChannel != null)
            {
                try
                {
                
                    newChannel.ChannelType = channel.ChannelType;
                    newChannel.SortingLineCode = channel.SortingLineCode;
                    newChannel.ChannelCapacity = channel.ChannelCapacity;
                    newChannel.ChannelCode = channel.ChannelCode;
                    newChannel.ChannelName = channel.ChannelName;
                    newChannel.ProductCode = channel.ProductCode;
                    newChannel.ProductName = channel.ProductName;
                    newChannel.GroupNo = channel.GroupNo;
                    newChannel.Height = channel.Height;
                    newChannel.LedNo = channel.LedNo;
                    newChannel.OrderNo = channel.OrderNo;
                    newChannel.RemainQuantity = channel.RemainQuantity;
                    newChannel.SortAddress = channel.SortAddress;
                    newChannel.IsActive = channel.IsActive;
                    newChannel.SupplyAddress = channel.SupplyAddress;
                    newChannel.Width = channel.Width;
                    newChannel.X = channel.X;
                    newChannel.Y = channel.Y;
                    newChannel.IsAcceptRemainQuantity = channel.IsAcceptRemainQuantity;
                    ChannelRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            else
            {
                strResult = "原因：未找到当前需要修改的数据！";
            }
            return result;
        }

        /// <summary>
        /// 删除烟道信息
        /// </summary>
        /// <param name="channelCode">烟道代码</param>
        /// <param name="strResult">相关信息</param>
        /// <returns>是否成功</returns>
        public bool Delete(string channelCode, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Channel channel = ChannelRepository.GetQueryable().FirstOrDefault(c => c.ChannelCode == channelCode);
            if (channel != null)
            {
                try
                {
                    ChannelRepository.Delete(channel);
                    ChannelRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            else
            {
                strResult = "原因：未找到当前需要删除的数据！";
            }
            return result;
        }

        public DataTable GetChannel(int page, int rows, string ProductCode, string SortingLineCode, string ChannelType, string GroupNo, string Status)
        {
            var channelQuery = ChannelRepository.GetQueryable();
            var productQuery = ProductRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            var channelDetails = channelQuery.Where(a => a.ProductCode.Contains(ProductCode) && a.SortingLineCode.Contains(SortingLineCode) && a.ChannelType.Contains(ChannelType)&&a.IsActive.Contains(Status)).OrderBy(a => a.ChannelCode).Select(a=>a);
        
            if (GroupNo != "")
            {
                int no;
                Int32.TryParse(GroupNo, out no);
                if (no != 0)
                {
                    channelDetails = channelDetails.Where(a => a.GroupNo == no);
                }
            }
            var channelArray = channelDetails.ToArray().Select(c => new
                {
                    c.ChannelCode,
                    SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortingLineCode).Select(s => s.SortingLineName),
                    c.ChannelName,
                    ChannelType = whatChannelType(c.ChannelType),                 
                    ProductCode = c.ProductCode,
                    ProductName = productQuery.Where(p => p.ProductCode == c.ProductCode).Select(p => p.ProductName),
                    c.RemainQuantity,
                    c.ChannelCapacity,                  
                    GroupNo = c.GroupNo == 1 ? "A线" : "B线",
                    c.OrderNo,
                    c.SortAddress,
                    c.SupplyAddress,
                    IsActive = c.IsActive == "1" ? "启用" : "不启用"
                }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("烟道名称", typeof(string));
            dt.Columns.Add("烟道类型", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));      
            dt.Columns.Add("预设卷烟编码", typeof(string));
            dt.Columns.Add("预设卷烟名称", typeof(string));
            dt.Columns.Add("剩余数量", typeof(int));
            dt.Columns.Add("烟道最大容量", typeof(int));
            dt.Columns.Add("组号", typeof(string));
            dt.Columns.Add("顺序号", typeof(int));
            dt.Columns.Add("分拣地址", typeof(int));
            dt.Columns.Add("补货地址", typeof(int));
            dt.Columns.Add("状态", typeof(string));
            foreach (var item in channelArray)
            {
                dt.Rows.Add
                    (
                        item.ChannelCode,                   
                        item.ChannelName,
                        item.ChannelType,
                        item.SortingLineName.ToArray().Length <= 0 ? "" : item.SortingLineName.ToArray()[0],                    
                        item.ProductCode,
                        item.ProductName.ToArray().Length <= 0 ? "" : item.ProductName.ToArray()[0],
                        item.RemainQuantity,
                        item.ChannelCapacity,
                        item.GroupNo,
                        item.OrderNo,
                        item.SortAddress,
                        item.SupplyAddress,
                        item.IsActive
                    );
            }
            return dt;
        }
    }
}
