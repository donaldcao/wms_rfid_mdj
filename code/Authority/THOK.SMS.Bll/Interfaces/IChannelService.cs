using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using System.Data;

namespace THOK.SMS.Bll.Interfaces
{
    public interface IChannelService : IService<Channel>
    {
        object GetDetails(int page, int rows, Channel Channel);

        bool Add(Channel channel, out string strResult);

        bool Edit(Channel channel, out string strResult);

        bool Delete(string channelCode, out string strResult);

        DataTable GetChannel(int page,int rows,string DefaultProductCode,string SortingLineCode,string ChannelType,string GroupNo,string Status);
    }
}
