using System;
using System.Linq;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

using System.Data;

namespace THOK.Wms.Bll.Service
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        [Dependency]
        public ICustomerRepository CustomerRepository { get; set; }

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }


        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string CustomerCode, string CustomerName, string CompanyCode, string SaleRegionCode, string CustomerType, string CityOrCountryside, string LicenseCode, string IsActive)
        {
            var customerQuery = CustomerRepository.GetQueryable();
            var customer = customerQuery.Where(c => c.CustomerCode.Contains(CustomerCode) &&
                                                          c.CustomerType.Contains(CustomerType) &&
                                                          c.SaleRegionCode.Contains(SaleRegionCode) &&
                                                          c.CityOrCountryside.Contains(CityOrCountryside) &&
                                                          c.CustomerName.Contains(CustomerName) &&
                                                          c.IsActive.Contains(IsActive));
            if (!CompanyCode.Equals(string.Empty))
            {
                customer = customer.Where(c => c.CompanyCode == CompanyCode);
            }
            if (!LicenseCode.Equals(string.Empty))
            {
                customer = customer.Where(c => c.LicenseCode == LicenseCode);
            }
            customer = customer.OrderBy(h => h.CustomerCode);
            int total = customer.Count();
            customer = customer.Skip((page - 1) * rows).Take(rows);

            var temp = customer.ToArray().Select(c => new
            {
                CustomerCode = c.CustomerCode,
                CustomCode = c.CustomCode,
                CustomerName = c.CustomerName,
                CompanyCode = c.CompanyCode,
                SaleRegionCode = c.SaleRegionCode,
                UniformCode = c.UniformCode,
                CustomerType = c.CustomerType,
                SaleScope = c.SaleScope,
                IndustryType = c.IndustryType,
                CityOrCountryside = c.CityOrCountryside,
                DeliverOrder = c.DeliverOrder,
                DeliverLineCode = c.DeliverLineCode,
                Address = c.Address,
                Phone = c.Phone,
                LicenseType = c.LicenseType,
                LicenseCode = c.LicenseCode,
                PrincipalName = c.PrincipalName,
                PrincipalPhone = c.PrincipalPhone,
                PrincipalAddress = c.PrincipalAddress,
                ManagementName = c.ManagementName,
                ManagementPhone = c.ManagementPhone,
                Bank = c.Bank,
                BankAccounts = c.BankAccounts,
                Description = c.Description,
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(Customer customer, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var cust = new Customer();
            if (customer != null)
            {
                try
                {
                    cust.CustomerCode = customer.CustomerCode;
                    cust.CustomCode = customer.CustomCode;
                    cust.CustomerName = customer.CustomerName;
                    cust.CompanyCode = customer.CompanyCode;
                    cust.SaleRegionCode = customer.SaleRegionCode;
                    cust.UniformCode = customer.UniformCode;
                    cust.CustomerType = customer.CustomerType;
                    cust.SaleScope = customer.SaleScope;
                    cust.IndustryType = customer.IndustryType;
                    cust.CityOrCountryside = customer.CityOrCountryside;
                    cust.DeliverLineCode = customer.DeliverLineCode;
                    cust.DeliverOrder = customer.DeliverOrder;
                    cust.Address = customer.Address;
                    cust.Phone = customer.Phone;
                    cust.LicenseType = customer.LicenseType;
                    cust.LicenseCode = customer.LicenseCode;
                    cust.PrincipalName = customer.PrincipalName;
                    cust.PrincipalPhone = customer.PrincipalPhone;
                    cust.PrincipalAddress = customer.PrincipalAddress;
                    cust.ManagementName = customer.ManagementName;
                    cust.ManagementPhone = customer.ManagementPhone;
                    cust.Bank = customer.Bank;
                    cust.BankAccounts = customer.BankAccounts;
                    cust.Description = customer.Description;
                    cust.IsActive = customer.IsActive;
                    cust.UpdateTime = DateTime.Now;

                    CustomerRepository.Add(cust);
                    CustomerRepository.SaveChanges();
                    //上报客户信息
                    //DataSet ds = Insert(cust);
                    //Customer.InsertCustom(ds);
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            return result;
        }

        public object C_Details(int page, int rows, string QueryString, string Value)
        {
            string CustomerCode = "";
            string CustomerName = "";
            if (QueryString == "CustomerCode")
            {
                CustomerCode = Value;
            }
            else
            {
                CustomerName = Value;
            }
            IQueryable<Customer> customerQuery = CustomerRepository.GetQueryable();
            var customer = customerQuery.Where(c => c.CustomerCode.Contains(CustomerCode) && c.CustomerName.Contains(CustomerName))
                .OrderBy(c => c.CompanyCode)
                .Select(c => c);
            if (!CustomerCode.Equals(string.Empty))
            {
                customer = customer.Where(p => p.CustomerCode == CustomerCode);
            }
            int total = customer.Count();
            customer = customer.Skip((page - 1) * rows).Take(rows);

            var temp = customer.ToArray().Select(c => new
            {
                CustomerCode = c.CustomerCode,
                CustomCode = c.CustomCode,
                CustomerName = c.CustomerName,
                CompanyCode = c.CompanyCode,
                IsActive = c.IsActive == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Delete(string CustomerCode)
        {
            var customer = CustomerRepository.GetQueryable()
               .FirstOrDefault(i => i.CustomerCode == CustomerCode);
            if (CustomerCode != null)
            {
                CustomerRepository.Delete(customer);
                CustomerRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(Customer customer, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                var cust = CustomerRepository.GetQueryable()
                    .FirstOrDefault(i => i.CustomerCode == customer.CustomerCode);
                cust.CustomCode = customer.CustomCode;
                cust.CustomerName = customer.CustomerName;
                cust.CompanyCode = customer.CompanyCode;
                cust.SaleRegionCode = customer.SaleRegionCode;
                cust.UniformCode = customer.UniformCode;
                cust.CustomerType = customer.CustomerType;
                cust.SaleScope = customer.SaleScope;
                cust.IndustryType = customer.IndustryType;
                cust.CityOrCountryside = customer.CityOrCountryside;
                cust.DeliverOrder = customer.DeliverOrder;
                cust.DeliverLineCode = customer.DeliverLineCode;
                cust.Address = customer.Address;
                cust.Phone = customer.Phone;
                cust.LicenseType = customer.LicenseType;
                cust.LicenseCode = customer.LicenseCode;
                cust.PrincipalName = customer.PrincipalName;
                cust.PrincipalPhone = customer.PrincipalPhone;
                cust.PrincipalAddress = customer.PrincipalAddress;
                cust.ManagementName = customer.ManagementName;
                cust.ManagementPhone = customer.ManagementPhone;
                cust.Bank = customer.Bank;
                cust.BankAccounts = customer.BankAccounts;
                cust.Description = customer.Description;
                cust.IsActive = customer.IsActive;
                cust.UpdateTime = DateTime.Now;
                CustomerRepository.SaveChanges();
                //上报客户信息
                //DataSet ds = Insert(cust);
                //Customer.InsertCustom(ds);

            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.InnerException;
            }
            return true;
        }

        public DataSet Insert(Customer customer)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["DWV_IORG_CUSTOMER"].NewRow();
            inbrddr["customer_code"] = customer.CustomCode;
            inbrddr["custom_code"] = customer.CustomCode;
            inbrddr["customer_name"] = customer.CustomerName;
            inbrddr["company_code"] = customer.CompanyCode;
            inbrddr["sale_region_code"] = customer.SaleRegionCode;
            inbrddr["uniform_code"] = customer.UniformCode;
            inbrddr["customer_type"] = customer.CustomerType ?? "";
            inbrddr["sale_scope"] = customer.SaleScope;
            inbrddr["industry_type"] = customer.IndustryType;
            inbrddr["city_or_countryside"] = customer.CityOrCountryside;
            inbrddr["deliver_line_code"] = customer.DeliverLineCode;
            inbrddr["deliver_order"] = customer.DeliverOrder;
            inbrddr["address"] = customer.Address;
            inbrddr["phone"] = customer.Phone;
            inbrddr["license_type"] = customer.LicenseType ?? "";
            inbrddr["license_code"] = customer.LicenseCode ?? "";
            inbrddr["principal_name"] = customer.PrincipalName ?? "";
            inbrddr["principal_phone"] = customer.PrincipalPhone ?? "";
            inbrddr["principal_address"] = customer.PrincipalAddress ?? "";
            inbrddr["management_name"] = customer.ManagementName ?? "";
            inbrddr["management_phone"] = customer.ManagementPhone ?? "";
            inbrddr["bank"] = customer.Bank ?? "";
            inbrddr["bank_accounts"] = customer.BankAccounts ?? "";
            inbrddr["description"] = customer.Description ?? "";
            inbrddr["is_active"] = customer.IsActive;
            inbrddr["update_time"] = DateTime.Now;
            ds.Tables["DWV_IORG_CUSTOMER"].Rows.Add(inbrddr);
            return ds;
        }

        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("DWV_IORG_CUSTOMER");
            inbrtable.Columns.Add("customer_code");
            inbrtable.Columns.Add("custom_code");
            inbrtable.Columns.Add("customer_name");
            inbrtable.Columns.Add("company_code");
            inbrtable.Columns.Add("sale_region_code");
            inbrtable.Columns.Add("uniform_code");
            inbrtable.Columns.Add("customer_type");
            inbrtable.Columns.Add("sale_scope");
            inbrtable.Columns.Add("industry_type");
            inbrtable.Columns.Add("city_or_countryside");
            inbrtable.Columns.Add("deliver_line_code");
            inbrtable.Columns.Add("deliver_order");
            inbrtable.Columns.Add("address");
            inbrtable.Columns.Add("phone");
            inbrtable.Columns.Add("license_type");
            inbrtable.Columns.Add("license_code");
            inbrtable.Columns.Add("principal_name");
            inbrtable.Columns.Add("principal_phone");
            inbrtable.Columns.Add("principal_address");
            inbrtable.Columns.Add("management_name");
            inbrtable.Columns.Add("management_phone");
            inbrtable.Columns.Add("bank");//一号工程条形码
            inbrtable.Columns.Add("bank_accounts");
            inbrtable.Columns.Add("description");
            inbrtable.Columns.Add("is_active");
            inbrtable.Columns.Add("update_time");
            return ds;
        }

        public System.Data.DataTable CustomerTable(int page, int rows, string customerCode, string customerName, string deliverLineCode)
        {
            IQueryable<Customer> query = CustomerRepository.GetQueryable();
            IQueryable <DeliverLine> deliverQuery = DeliverLineRepository.GetQueryable();

            IQueryable<Customer> query1 = query;
            if (!string.IsNullOrEmpty(customerName))
            {
                query1 = query.Where(a => a.CustomerName.Contains(customerName));
            }
            IQueryable<Customer> query2 = query1;
            if (!string.IsNullOrEmpty(customerCode))
            {
                query2 = query1.Where(a => a.CustomerCode.Contains(customerCode));
            }
            IQueryable<Customer> query3 = query2;
            if (!string.IsNullOrEmpty(deliverLineCode))
            {
                query3 = query2.Where(a => a.DeliverLineCode == deliverLineCode);
            }
            var v1 = query3.Join(deliverQuery, a => a.DeliverLineCode, b => b.NewDeliverLineCode, (a, b) => new
            {
                a.CustomerCode,
                a.CustomerName,
                a.DeliverLineCode,
                a.DeliverOrder,
                a.Address,
                a.IsActive,
                a.UpdateTime,
                b.DeliverLineName
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("客户编码", typeof(string));
            dt.Columns.Add("客户名称", typeof(string));
            dt.Columns.Add("线路编码", typeof(string));
            dt.Columns.Add("线路名称", typeof(string));
            dt.Columns.Add("送货顺序", typeof(string));
            dt.Columns.Add("送货地址", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var a in v1)
            {
                dt.Rows.Add
                (
                    a.CustomerCode,
                    a.CustomerName,
                    a.DeliverLineCode,
                    a.DeliverOrder,
                    a.Address,
                    a.IsActive,
                    a.UpdateTime,
                    a.DeliverLineName
                );
            }
            return dt;
        }

        //分拣查询客户信息
        public object GetDetails(int page, int rows, string customerCode, string customerName, string deliverLineCode)
        {
            IQueryable<Customer> query = CustomerRepository.GetQueryable();
            IQueryable<DeliverLine> deliverQuery = DeliverLineRepository.GetQueryable();

            IQueryable<Customer> query1 = query;
            if (!string.IsNullOrEmpty(customerName))
            {
                query1 = query.Where(a => a.CustomerName.Contains(customerName));
            }
            IQueryable<Customer> query2 = query1;
            if (!string.IsNullOrEmpty(customerCode))
            {
                query2 = query1.Where(a => a.CustomerCode.Contains(customerCode));
            }
            IQueryable<Customer> query3 = query2;
            if (!string.IsNullOrEmpty(deliverLineCode))
            {
                query3 = query2.Where(a => a.DeliverLineCode == deliverLineCode);
            }
            var v1 = query3.Join(deliverQuery, a => a.DeliverLineCode, b => b.NewDeliverLineCode, (a, b) => new
            {
                a.CustomerCode,
                a.CustomerName,
                a.DeliverLineCode,
                a.DeliverOrder,
                a.Address,
                a.IsActive,
                a.UpdateTime,
                b.DeliverLineName
            })
            .OrderBy(a => a.CustomerCode)
            .Select(a => a);

            int total = v1.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);

            var customerArray = v1.ToArray().Select(c => new
            {
                c.CustomerCode,
                c.CustomerName,
                c.DeliverOrder,
                c.DeliverLineCode,
                c.DeliverLineName,
                c.Address,
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = customerArray.ToArray() };
        }
    }
}
