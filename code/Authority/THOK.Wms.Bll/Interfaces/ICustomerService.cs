﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICustomerService : IService<Customer>
    {
        object GetDetails(int page, int rows, string CustomerCode,string CustomerName, string CompanyCode, string SaleRegionCode, string CustomerType, string CityOrCountryside, string LicenseCode, string IsActive);

        bool Add(Customer customer, out string strResult);

        object C_Details(int page, int rows, string QueryString, string Value);

        bool Delete(string CustomerCode);

        bool Save(Customer customer, out string strResult);

        System.Data.DataTable GetCustomerInfo(int page, int rows, string CustomerCode,string CustomerName,string DeliverLineCode);

        object GetDetails(int page, int rows, string customerCode, string customerName, string deliverLineCode);
    }
}
