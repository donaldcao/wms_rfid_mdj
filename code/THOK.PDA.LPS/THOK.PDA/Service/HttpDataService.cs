using System;
using System.Collections.Generic;
using System.Text;
using AS.PDA.Model;
using AS.PDA.Util;
using System.Data;
using Newtonsoft.Json;

namespace AS.PDA.Service
{
    public class HttpDataService
    {
        HttpUtil util = new HttpUtil();

        public bool FinishTask(string methodName, out string message)
        {
            string msg = util.GetDataFromServer(methodName);
            Result r = JsonConvert.DeserializeObject<Result>(msg);
            if (r!=null && !r.IsSuccess)
            {
                message = r.Message;
                return false;
            }
            else
            {
                message = "scan success!";
                return true;
            }
        }
    }
}
