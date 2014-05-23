using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;


using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class LedService : ServiceBase<Led>, ILedService
    {
        [Dependency]
        public ILedRepository LedRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }


        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string Status, string LedName, string LedGroupCode, string LedType, string LedCode)
        {
            IQueryable<Led> ledquery = LedRepository.GetQueryable();
            IQueryable<Channel> channelquery = ChannelRepository.GetQueryable();

            IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();

            var led = ledquery.OrderBy(c => c.LedCode == LedCode).Select(a => new
            {
                a.Height,
                a.LedCode,
                a.LedGroupCode,
                a.LedIp,
                a.LedName,
                a.LedType,
                a.OrderNo,
                a.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == a.SortingLineCode).Select(s => s.SortingLineName),
                a.Status,
                a.Width,
                a.XAxes,
                a.YAxes

            });
            int ss = led.Count();
            if (LedCode != null && LedCode != string.Empty)
            {
                led = led.Where(a => a.LedGroupCode == LedCode);

            }
            if (LedType != null && LedType != string.Empty)
            {
                led = led.Where(a => a.LedType == LedType);
            }
            if (LedName != null && LedName != string.Empty)
            {
                led = led.Where(a => a.LedName.Contains(LedName));

            }
            if (LedGroupCode != null && LedGroupCode != string.Empty)
            {
                led = led.Where(a => a.LedGroupCode == LedGroupCode);

            }
            if (Status != null && Status != string.Empty)
            {
                led = led.Where(a => a.Status == Status);

            }
            var channel = led.OrderByDescending(a => a.LedCode).ToArray()
                .Select(a => new
                {

                    a.Height,
                    a.LedCode,
                    a.LedGroupCode,
                    a.LedIp,
                    a.LedName,
                    LedType = a.LedType == "1" ? "整屏" : "分屏",
                    SortingLineCode = a.SortingLineCode,
                    SortingLineName = a.SortingLineName,
                    Status = a.Status == "1" ? "可用" : "不可用",
                    OrderNo = a.OrderNo,
                    a.Width,
                    a.XAxes,
                    a.YAxes
                });


            int total = channel.Count();
            var batchsRow = channel.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = channel.ToArray() };
        }


        public bool Add(Led ledInfo, string LedType, string LedGroupCode, string SortingLineCode, out string strResult)
        {

            string groupCode, SortingCode;
          //无父节点
            if (ledInfo.LedGroupCode == "" && ledInfo.LedGroupCode == string.Empty)
            {
                groupCode = null;              
            }
            else
            {
                groupCode = ledInfo.LedGroupCode;           
            }
            //无分屏
            if (SortingLineCode == "" && SortingLineCode == string.Empty)
            {

                SortingCode = SortingLineCode; 
            }
            else
            {
                SortingCode = ledInfo.SortingLineCode;

            }

            strResult = string.Empty;
            bool result = false;
            var al = LedRepository.GetQueryable();
            if (al != null)
            {

                Led leds = new Led();
                try
                {
                    leds.LedCode = ledInfo.LedCode;
                    leds.Height = ledInfo.Height;
                    leds.LedGroupCode = groupCode;
                    leds.LedIp = "127.0.0.1"; //ledInfo.LedIp;   结构限制 注意及时修改
                    leds.LedName = ledInfo.LedName;

                    leds.LedType = ledInfo.LedType;//ledInfo.LedType;
                                                        
                    leds.OrderNo = ledInfo.OrderNo;
                    leds.Status = ledInfo.Status;
                    leds.Width = ledInfo.Width;
                    leds.XAxes = ledInfo.XAxes;
                    leds.YAxes = ledInfo.YAxes;
                    leds.SortingLineCode = SortingCode;

                    LedRepository.Add(leds);
                    LedRepository.SaveChanges();

                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "原因:" + e.Message;
                }
            }
            else
            {
                strResult = "原因:数据已存在";
            }
            return result;
        }

        public bool Save(Led ledInfo,string LedType,string LedGroupCode,string SortingLineCode, out string strResult)
        {

            string groupCode, SortingCode;
            if (ledInfo.LedGroupCode == "" && ledInfo.LedGroupCode == string.Empty)
            {

                groupCode = null;
            }
            else
            {

                groupCode = ledInfo.LedGroupCode;
            }

            //无分屏
            if (SortingLineCode == "" && SortingLineCode == string.Empty)
            {

                SortingCode = SortingLineCode;
            }
            else
            {
                SortingCode = ledInfo.SortingLineCode;

            }

            strResult = string.Empty;
            bool result = false;
            var leds = LedRepository.GetQueryable().FirstOrDefault(a => a.LedCode == ledInfo.LedCode);
            if (leds != null)
            {
                leds.LedCode = ledInfo.LedCode;
                leds.Height = ledInfo.Height;
                leds.LedGroupCode = LedGroupCode;
                leds.LedIp = "127.0.0.1"; //ledInfo.LedIp;   结构限制 注意及时修改
                leds.LedName = ledInfo.LedName;
                leds.LedType = LedType;//== "整屏" ? "1" : "2";
                leds.OrderNo = ledInfo.OrderNo;
                leds.Status = ledInfo.Status;// == "可用" ? "1" : "0";
                leds.Width = ledInfo.Width;
                leds.XAxes = ledInfo.XAxes;
                leds.YAxes = ledInfo.YAxes;
                leds.SortingLineCode = SortingLineCode;

                LedRepository.SaveChanges();
                result = true;
            }
            else
            {

                strResult = "原因:找不到相应数据";
            }
            return result;
        }


        public bool Delete(string LedCode, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            var ledInfo = LedRepository.GetQueryable().FirstOrDefault(a => a.LedCode.Contains(LedCode));

            if (ledInfo != null)
            {

                var leddetail = LedRepository.GetQueryable().Where(a => a.LedGroupCode == LedCode);
                if (leddetail != null)
                {
                    foreach (var tsub in leddetail)
                    {
                        LedRepository.Delete(tsub);
                    }
                    result = true;
                }
                LedRepository.Delete(ledInfo);
                LedRepository.SaveChanges();
                result = true;
            }
            else
            {
                strResult = "原因:没有找到相应数据";

            }
            return result;
        }



        public object GetLedGroupCode(int page, int rows, string QueryString, string Value)
        {
            string LedName = "";
            string LedIp = "";
            string LedType = "1";
            if (QueryString == "LedName")
            {
                LedName = Value;
            }
            else
            {
                LedIp = Value;
            }
            IQueryable<Led> HelpContentQuery = LedRepository.GetQueryable();
            var LedContent = HelpContentQuery.Where(c => c.LedName.Contains(LedName) && c.LedIp.Contains(LedIp));

            //if (!LedIp.Equals(string.Empty))
            //{
            //    LedContent = LedContent.Where(p => p.LedIp == LedIp);
            //}

            if (!LedType.Equals(string.Empty))
            {
                LedContent = LedContent.Where(p => p.LedType == LedType);
            }

            var leds = LedContent.OrderByDescending(a => a.LedCode).AsEnumerable().Select(a => new
            {

                a.LedCode,
                a.LedName,
                a.LedIp,
                LedType = a.LedType == "1" ? "整屏" : "分屏",
                Status = a.Status == "1" ? "可用" : "不可用"
            });


            int total = LedContent.Count();
            leds = leds.Skip((page - 1) * rows).Take(rows);


            return new { total, rows = leds.ToArray() };
        }


        public System.Data.DataTable GetLed(int page, int rows, string ledcode)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (ledcode != "" && ledcode != null)
            {
                IQueryable<Led> ledQuery = LedRepository.GetQueryable();
                var led = ledQuery.Where(i => i.LedCode.Contains(ledcode)).OrderBy(i => i.LedCode).Select(i => i);
                var temp = led.ToArray().AsEnumerable().Select(i => new
                {
                    i.LedCode,
                    i.LedName,
                    i.LedType,
                    i.LedIp,
                    i.SortingLineCode,
                    i.LedGroupCode,
                    i.XAxes,
                    i.YAxes,
                    i.Width,
                    i.Height,
                    i.OrderNo,
                    i.Status
                });
                dt.Columns.Add("LED编号", typeof(string));
                dt.Columns.Add("LED名称", typeof(string));
                dt.Columns.Add("LED类型", typeof(string));
                dt.Columns.Add("LED地址", typeof(string));
                dt.Columns.Add("分拣线名称", typeof(Int32));
                dt.Columns.Add("X坐标", typeof(Int32));
                dt.Columns.Add("Y坐标", typeof(Int32));
                dt.Columns.Add("LED宽", typeof(Int32));
                dt.Columns.Add("LED高", typeof(Int32));
                dt.Columns.Add("父级编号", typeof(string));
                dt.Columns.Add("顺序号", typeof(Int32));
                dt.Columns.Add("状态", typeof(string));
                foreach (var item in temp)
                {
                    dt.Rows.Add
                        (
                           item.Height,
                           item.LedCode,
                           item.LedGroupCode,
                           item.LedIp,
                           item.LedName,
                           item.LedType,
                           item.OrderNo,
                           item.SortingLineCode,
                           item.Status,
                           item.Width,
                           item.XAxes,
                           item.YAxes
                        );
                }
                //if (temp.Count() > 0)
                //{
                //    dt.Rows.Add(
                //        null, null, null, "总数：",
                //        temp.Sum(m => m.BillQuantity),
                //        temp.Sum(m => m.AllotQuantity),
                //        temp.Sum(m => m.RealQuantity),
                //        null);
                //}
            }
            return dt;
        }

    }
}
