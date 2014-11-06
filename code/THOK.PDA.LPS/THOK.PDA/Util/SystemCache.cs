using System;
using System.Collections.Generic;
using System.Text;
using AS.PDA.View;
using System.Data;

namespace AS.PDA.Util
{
    public static class SystemCache
    {
        private static MainForm mainFrom = null;
        public static MainForm MainFrom
        {
            get { return SystemCache.mainFrom; }
            set { SystemCache.mainFrom = value; }
        }

        private static string connetionType = "";
        public static string ConnetionType
        {
            get
            {
                if (connetionType == "")
                {
                    SystemCache.connetionType = new ConfigUtil().GetConfig("ConnetionType")["Type"];
                }
                return SystemCache.connetionType;
            }
        }

        private static string httpConnectionStr = "";
        public static string HttpConnectionStr
        {
            get
            {
                SystemCache.httpConnectionStr = new ConfigUtil().GetConfig("HttpConnectionStr")["HttpConnStr"];
                return SystemCache.httpConnectionStr;
            }
        }
    }
}
