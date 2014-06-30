﻿using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
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

            IQueryable<Channel> channelQuery = ChannelRepository.GetQueryable();
            IQueryable<Product> productQuery = ProductRepository.GetQueryable();
            IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();

            var channelDetails = channelQuery.Where(a => a.Status.Contains(channel.Status));
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
            if (channel.DefaultProductCode != null&& channel.DefaultProductCode!= string.Empty)
            {
                channelDetails = channelDetails.Where(a => a.DefaultProductCode == channel.DefaultProductCode);
            }
            int total = channelDetails.Count();
            var channelArray = channelDetails.OrderBy(a => a.ChannelCode).ToArray().Skip((page - 1) * rows).Take(rows)
                .Select(c => new
                {
                    c.ChannelCode,
                    c.SortingLineCode,
                    SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortingLineCode).Select(s => s.SortingLineName),
                    c.ChannelName,                 
                    ChannelType = whatChannelType(c.ChannelType),
                    c.LedNo,
                    c.DefaultProductCode,
                    DefaultProductName = productQuery.Where(p => p.ProductCode == c.DefaultProductCode).Select(p => p.ProductName),
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
                    Status = c.Status == "01" ? "可用":"不可用"
                });
            return new { total, rows = channelArray.ToArray() };
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
                    newChannel.DefaultProductCode = channel.DefaultProductCode;
                    newChannel.DefaultProductName = channel.DefaultProductName;
                    newChannel.GroupNo = channel.GroupNo;
                    newChannel.Height = channel.Height;
                    newChannel.LedNo = channel.LedNo;
                    newChannel.OrderNo = channel.OrderNo;
                    newChannel.RemainQuantity = channel.RemainQuantity;
                    newChannel.SortAddress = channel.SortAddress;
                    newChannel.Status = channel.Status;
                    newChannel.SupplyAddress = channel.SupplyAddress;
                    newChannel.Width = channel.Width;
                    newChannel.X = channel.X;
                    newChannel.Y = channel.Y;
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

        public DataTable GetChannel(string channelName, string channelType, string status, string groupNo)
        {
            //IQueryable<Channel> channelQuery = ChannelRepository.GetQueryable();
            //IQueryable<Product> productQuery = ProductRepository.GetQueryable();
            //IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();
            //var channelDetails = channelQuery.Where(a => a.ChannelName.Contains(channelName)
            //    && a.ChannelType.Contains(channelType) && a.Status.Contains(status));
            //if (groupNo != "")
            //{
            //    int no;
            //    Int32.TryParse(groupNo, out no);
            //    if (no != 0)
            //    {
            //        channelDetails = channelDetails.Where(a => a.GroupNo == no);
            //    }
            //}
            //var channelArray = channelDetails.OrderBy(c=>c.ChannelCode).Select(c => new
            //    {
            //        c.ChannelCode,
            //        SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortingLineCode).Select(s => s.SortingLineName),
            //        c.ChannelName,
            //        ChannelType = c.ChannelType == "1" ? "叠垛机" : c.ChannelType == "2" ? "立式机" : c.ChannelType == "3" ? "通道机" : c.ChannelType == "4" ? "卧式机" : "混合烟道",
            //        LedName = ledQuery.Where(s => s.LedCode == c.LedCode).Select(s => s.LedName),
            //        c.DefaultProductCode,
            //        DefaultProductName = productQuery.Where(p => p.ProductCode == c.DefaultProductCode).Select(p => p.ProductName),
            //        c.RemainQuantity,
            //        c.MiddleQuantity,
            //        c.MaxQuantity,
            //        GroupNo = c.GroupNo == 1 ? "A线" : "B线",
            //        c.OrderNo,
            //        Status = c.Status == "1" ? "可用" : "不可用"
            //    }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("烟道名称", typeof(string));
            dt.Columns.Add("烟道类型", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("Led屏名称", typeof(string));
            dt.Columns.Add("预设卷烟编码", typeof(string));
            dt.Columns.Add("预设卷烟名称", typeof(string));
            dt.Columns.Add("提前量", typeof(string));
            dt.Columns.Add("补货中间量", typeof(string));
            dt.Columns.Add("最大缓存量", typeof(string));
            dt.Columns.Add("组号", typeof(string));
            dt.Columns.Add("顺序号", typeof(string));
            dt.Columns.Add("状态", typeof(string));
            //foreach (var item in channelArray)
            //{
            //    dt.Rows.Add
            //        (
            //            item.ChannelCode,
            //            item.ChannelName,
            //            item.ChannelType,
            //            item.SortingLineName.ToArray().Length <= 0 ? "" : item.SortingLineName.ToArray()[0],
            //            item.LedName.ToArray().Length <= 0 ? "" : item.LedName.ToArray()[0],
            //            item.DefaultProductCode,
            //            item.DefaultProductName.ToArray().Length <= 0 ? "" : item.DefaultProductName.ToArray()[0],
            //            item.RemainQuantity,
            //            item.MiddleQuantity,
            //            item.MaxQuantity,
            //            item.GroupNo,
            //            item.OrderNo,
            //            item.Status
            //        );

            //}
            return dt;
        }
    }
}
