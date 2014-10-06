namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using THOK.Authority.DbModel;

    internal sealed class Configuration : DbMigrationsConfiguration<THOK.Wms.Repository.AuthorizeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(THOK.Wms.Repository.AuthorizeContext context)
        {
            //Clear(context);

            #region 登录界面数据显示
            //CreateCities(context);
            //CreateServers(context);
            CreateSystems(context); 
            #endregion

            #region 基础权限管理系统
            CreateServer(context);
            CreateSystemAuthority(context);
            CreateSystemLog(context);
            CreateHelpContents(context); 
            #endregion

            CreateOrg(context);
            CreateWarehouse(context);
            CreateProduct(context);

            CreateStockBill(context);

            CreateSearch(context);
            CreateStock(context);          
            CreateProductQuality(context);

            CreateSorting(context);            
            CreateAutomotiveSystems(context);
            CreateSystemInfo(context);
            CreateSystemParameter(context);
            
            CreateSortSystemInfo(context);
        }

        private void Clear(THOK.Wms.Repository.AuthorizeContext context)
        {
           string sql = @"delete [dbo].[auth_role_function]
                          delete [dbo].[auth_role_module]
                          delete [dbo].[auth_user_function]
                          delete [dbo].[auth_user_module]
                          delete [dbo].[auth_function]
                          delete [dbo].[auth_module]";
           context.Database.ExecuteSqlCommand(sql);
        }

        #region 登录界面数据显示
        private void CreateCities(AuthorizeContext context)
        {
            context.Set<City>().AddOrUpdate(
            new City()
            {
                CityID = new Guid("F8344F88-08AD-4F9A-8F45-EAD8BB471105"),
                CityName = "永州市",
                Description = "永州市",
                IsActive = true
            });
            context.SaveChanges();
        }
        private void CreateServers(AuthorizeContext context)
        {
            City city = context.Set<City>().SingleOrDefault(c => c.CityID == new Guid("F8344F88-08AD-4F9A-8F45-EAD8BB471105"));
            context.Set<Server>().AddOrUpdate(
            new Server()
            {
                ServerID = new Guid("F8344F88-08AD-4F9A-8F45-EAD8BB471106"),
                ServerName = "永州服务器",
                Description = "永州服务器",
                Url = "",
                IsActive = true,
                City = city,
                City_CityID = city.CityID
            });
            context.SaveChanges();
        }
        private void CreateSystems(AuthorizeContext context)
        {
            context.Set<SystemInfo>().AddOrUpdate(
            new SystemInfo()
            {
                SystemID = new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"),
                SystemName = "基础权限管理系统",
                Description = "基础权限管理系统",
                Status = true
            },
            new SystemInfo()
            {
                SystemID = new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"),
                SystemName = "自动化仓储管理系统",
                Description = "自动化仓储管理系统",
                Status = true
            },
            new SystemInfo()
            {
                SystemID = new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA9"),
                SystemName = "自动化仓储控制系统",
                Description = "自动化仓储控制系统",
                Status = true
            },
            new SystemInfo()
            {
                SystemID = new Guid("ED0E6430-9DEB-4CDE-8DCF-702D5B528AA8"),
                SystemName = "自动化分拣管理系统",
                Description = "自动化分拣管理系统",
                Status = true
            });
            context.SaveChanges();
        } 
        #endregion


        #region 服务器信息管理
        private void CreateServer(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"));
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("87104EFF-4D12-4ac9-BD62-11F8C0DA1032"),
                ModuleName = "服务器信息管理",
                ShowOrder = 1,
                ModuleURL = "",
                IndicateImage = "icon-Menu_ServerDevice",
                DeskTopImage = "image-Menu_ServerDevice",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("87104EFF-4D12-4ac9-BD62-11F8C0DA1032")
            });
            this.ModuleCity(context, system, 1);
            this.ModuleServer(context, system, 2);
            context.SaveChanges();
        }
        private void ModuleCity(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426"),
                ModuleName = "地市信息",
                ShowOrder = order,
                ModuleURL = "/City/",
                IndicateImage = "icon-Son_Cities",
                DeskTopImage = "image-Son_Cities",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("87104EFF-4D12-4ac9-BD62-11F8C0DA1032")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("C2F089D7-90C6-4ae9-A941-C1454CADA3CA"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            },
            new Function()
            {
                FunctionID = new Guid("6F34032B-6EC9-45fa-90C8-33FADDE2C4BA"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            },
            new Function()
            {
                FunctionID = new Guid("51E449C0-96C9-4f63-8453-360B4A38BF32"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            },
            new Function()
            {
                FunctionID = new Guid("EC68F16E-52E3-46bd-9D90-7E7E4A5ED5E1"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            },
            new Function()
            {
                FunctionID = new Guid("E355C8D8-E28F-4662-930F-67B61DCE4675"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            },
            new Function()
            {
                FunctionID = new Guid("63CD539F-2358-48d0-ABBE-6A09E027661A"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("4CCF72D8-2590-4b86-97B5-DCA5B6F2C426")
            });
        }
        private void ModuleServer(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7"),
                ModuleName = "服务器信息",
                ShowOrder = order,
                ModuleURL = "/Server/",
                IndicateImage = "icon-Son_ServerComputer",
                DeskTopImage = "image-Son_ServerComputer",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("87104EFF-4D12-4ac9-BD62-11F8C0DA1032")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("593116DE-EF9D-4ffa-BBFC-352A70B9545B"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            },
            new Function()
            {
                FunctionID = new Guid("D255D452-767B-4dc3-8FE1-B5661E134E68"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            },
            new Function()
            {
                FunctionID = new Guid("1616070B-9D6B-4e60-9E8A-FF88BD20C2D2"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            },
            new Function()
            {
                FunctionID = new Guid("B056C5A6-8647-4f11-9471-BAE9811627E3"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            },
            new Function()
            {
                FunctionID = new Guid("09CB7C0C-8E5E-4d2b-802A-249BF9D76298"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            },
            new Function()
            {
                FunctionID = new Guid("8A0FFF66-2BE9-44c3-8B5C-C9C3FBF7EFF7"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("55F51C66-AF1F-4893-8710-A5420B899FD7")
            });
        }
        #endregion

        #region 系统权限管理
        private void CreateSystemAuthority(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"));
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5"),
                ModuleName = "系统权限管理",
                ShowOrder = 2,
                ModuleURL = "",
                IndicateImage = "icon-Menu_Jurisdiction",
                DeskTopImage = "image-Menu_Jurisdiction",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5")
            });
            this.ModuleSystem(context, system, 1);
            this.ModuleModule(context, system, 2);
            this.ModuleRole(context, system, 3);
            this.ModuleUser(context, system, 4);
            context.SaveChanges();
        }
        private void ModuleSystem(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052"),
                ModuleName = "系统信息",
                ShowOrder = order,
                ModuleURL = "/System/",
                IndicateImage = "icon-Son_SysJurisdiction",
                DeskTopImage = "image-Son_SysJurisdiction",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("CB027D95-9425-403e-B229-E569BC771887"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            },
            new Function()
            {
                FunctionID = new Guid("1C40D6EA-C8ED-4711-82E4-411836CA6C98"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            },
            new Function()
            {
                FunctionID = new Guid("6642B9DA-BA7F-407f-B206-A34A8FFDEAA5"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            },
            new Function()
            {
                FunctionID = new Guid("07D6EBEA-1FC8-4de3-95B4-2A36537C4553"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            },
            new Function()
            {
                FunctionID = new Guid("5497040B-EACE-4c77-896C-0431B5C226F4"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            },
            new Function()
            {
                FunctionID = new Guid("9703FC37-FA3D-4cbd-8A47-EF18FA57F7D2"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("D06BA0E2-8CD8-4c61-9518-6172286C2052")
            });
        }
        private void ModuleModule(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC"),
                ModuleName = "模块信息",
                ShowOrder = order,
                ModuleURL = "/Module/",
                IndicateImage = "icon-Son_ModuleJurisdiction",
                DeskTopImage = "image-Son_ModuleJurisdiction",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("76FE9BAF-7D5E-4022-BD8F-DBF129251D65"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("1C336946-1047-47bd-B8CD-8A4331C5075B"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("D753CA72-AA60-41b6-8879-59A34967A17C"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("2931BDD1-A043-4c4b-866A-310229C10B8C"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("E3F6C1FE-3F1A-4eb2-9AE7-CB835EFF5DB6"),
                FunctionName = "功能",
                ControlName = "functionadmin",
                IndicateImage = "icon-son_SortWork",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("E3C2261D-B643-4614-8943-C975E9BFFC64"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            },
            new Function()
            {
                FunctionID = new Guid("FD685941-16E6-4965-B848-8411702BD943"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("182120F0-CC8E-43cb-858C-E074972412DC")
            });
        }
        private void ModuleRole(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2"),
                ModuleName = "角色信息",
                ShowOrder = order,
                ModuleURL = "/Role/",
                IndicateImage = "icon-Son_RoleJurisdiction",
                DeskTopImage = "image-Son_RoleJurisdiction",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("1AFA4F03-297A-46a7-A634-59EAEFA7DF99"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("7143E1B5-C545-41f5-A475-BB05B058F6A7"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("726D92A4-8FEC-4836-92FE-D68BB8C8F1D2"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("78317237-70AC-41a4-8768-B17438315EE6"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("B238E721-ECC9-4b71-A82F-17C69EB1C03D"),
                FunctionName = "权限",
                ControlName = "permissionadmin",
                IndicateImage = "icon-Son_RoleJurisdiction",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("EFBF340C-1183-4a59-BABF-A1439696951F"),
                FunctionName = "用户",
                ControlName = "useradmin",
                IndicateImage = "icon-Son_UserJurisdiction",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("71AA92B0-CD04-4042-9E6B-6B63701B90AB"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            },
            new Function()
            {
                FunctionID = new Guid("A59DEFCF-3F17-47c2-8B8C-AEF6CEAF0B7A"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("ADD20759-88F7-43db-A885-4508DB9BEEC2")
            });
        }
        private void ModuleUser(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA"),
                ModuleName = "用户信息",
                ShowOrder = order,
                ModuleURL = "/User/",
                IndicateImage = "icon-Son_UserJurisdiction",
                DeskTopImage = "image-Son_UserJurisdiction",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("0C07E22C-B747-453c-9ED6-D02962D00CE5")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("38EAD282-DA22-4c9a-8963-47E5B94C434C"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("7C176C75-3B0E-4f48-BEAF-54C6E68EBBAC"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("7407C692-E6B8-4731-8729-9F45BA1F42D1"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("838BE0BA-C9F1-409d-9644-6694418EF1BF"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("8CA2DF4E-F3EC-4d79-87F5-74699BFBBDA9"),
                FunctionName = "权限",
                ControlName = "permissionadmin",
                IndicateImage = "icon-Son_RoleJurisdiction",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("6B579D44-DA6C-422a-900A-DD8FB3DD028E"),
                FunctionName = "角色",
                ControlName = "roleadmin",
                IndicateImage = "icon-Son_RoleJurisdiction",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("2ABA26B1-126A-48dc-8337-E69D9D91C1A2"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            },
            new Function()
            {
                FunctionID = new Guid("12DE4CCE-2EF1-4794-A314-FB825B38EE91"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("D7A91161-12CB-4938-872C-E9CBFFFF80DA")
            });
        }
        #endregion

        #region 系统日志管理
        private void CreateSystemLog(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"));
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("54C11B83-884A-4256-87E8-F3CD62004661"),
                ModuleName = "系统日志管理",
                ShowOrder = 3,
                ModuleURL = "",
                IndicateImage = "icon-Menu_SysLog",
                DeskTopImage = "image-Menu_SysLog",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("54C11B83-884A-4256-87E8-F3CD62004661")
            });
            ModuleLoginLog(context, system, 1);
            ModuleSystemEventLog(context, system, 2);
            ModuleExceptionalLog(context, system, 3);
            context.SaveChanges();
        }
        private void ModuleLoginLog(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29"),
                ModuleName = "登录日志",
                ShowOrder = order,
                ModuleURL = "/LoginLog/",
                IndicateImage = "icon-Son_LoginLog",
                DeskTopImage = "image-Son_LoginLog",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("54C11B83-884A-4256-87E8-F3CD62004661")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("5400F22F-D11B-421a-9FBF-3B08D6BB4223"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29")
            },
            new Function()
            {
                FunctionID = new Guid("5400F22F-D11B-421a-9FBF-3B08D7BB4293"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29")
            },
            new Function()
            {
                FunctionID = new Guid("5400F22F-D15B-421a-9FBF-3B03D6BB4223"),
                FunctionName = "清空",
                ControlName = "empty",
                IndicateImage = "icon-cancel",
                Module_ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29")
            },
            new Function()
            {
                FunctionID = new Guid("A979C69D-BEA5-468e-9890-FA057093A0D3"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29")
            },
            new Function()
            {
                FunctionID = new Guid("C223CB09-ADF4-4ad6-8ADB-1DDCCB0E4E5A"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("A031D57E-2352-4a4d-81A5-8E7001915A29")
            });
        }
        private void ModuleSystemEventLog(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5"),
                ModuleName = "业务日志",
                ShowOrder = order,
                ModuleURL = "/SystemEventLog/",
                IndicateImage = "icon-Son_WorkLog",
                DeskTopImage = "image-Son_WorkLog",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("54C11B83-884A-4256-87E8-F3CD62004661")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("FC581C8B-2D60-4a79-A112-4C7BE1C4BFCD"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5")
            },
            new Function()
            {
                FunctionID = new Guid("FC531C8B-2D60-4a79-A112-4C7BE1C2BFCD"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5")
            },
            new Function()
            {
                FunctionID = new Guid("FC581C8B-2D30-4a79-A112-4C7BE3C43FCD"),
                FunctionName = "清空",
                ControlName = "empty",
                IndicateImage = "icon-cancel",
                Module_ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5")
            },
            new Function()
            {
                FunctionID = new Guid("AC83BE1B-66FB-453f-B5CD-D4FC957C2235"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5")
            },
            new Function()
            {
                FunctionID = new Guid("B904FA69-268B-4be7-85DC-D9F30DD3C84F"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("848EFB88-9816-4fd1-BD14-3D41A2876BB5")
            });
        }
        private void ModuleExceptionalLog(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853"),
                ModuleName = "错误日志",
                ShowOrder = order,
                ModuleURL = "/ExceptionalLog/",
                IndicateImage = "icon-Son_WrongLog",
                DeskTopImage = "image-Son_WrongLog",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("54C11B83-884A-4256-87E8-F3CD62004661")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("F6DDF9EC-73F4-43de-803E-4E7077F9097B"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853")
            },
            new Function()
            {
                FunctionID = new Guid("F6DDF9EC-73F4-43de-803E-4E7024F50273"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853")
            },
            new Function()
            {
                FunctionID = new Guid("F6D3F9EC-74F4-43de-803E-4E4027F90275"),
                FunctionName = "清空",
                ControlName = "empty",
                IndicateImage = "icon-cancel",
                Module_ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853")
            },
            new Function()
            {
                FunctionID = new Guid("864BF3F2-1717-400f-B176-C80D11017672"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853")
            },
            new Function()
            {
                FunctionID = new Guid("591A15A6-B8C0-4638-8E3B-28DBDB54C74E"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("8A7D294A-FCD1-44de-A816-3D3ED2E21853")
            });
        }
        #endregion

        #region 帮助文档管理
        private void CreateHelpContents(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"));
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("9280918A-632F-43a9-B611-D7597C858EA8"),
                ModuleName = "帮助文档管理",
                ShowOrder = 4,
                ModuleURL = "",
                IndicateImage = "icon-Menu_Help",
                DeskTopImage = "image-Menu_Help",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("9280918A-632F-43a9-B611-D7597C858EA8")
            });
            this.ModuleHelpContent(context, system, 1);
            this.ModuleHelpEdit(context, system, 2);
            this.ModuleHelp(context, system, 3);
            context.SaveChanges();
        }
        private void ModuleHelpContent(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED"),
                ModuleName = "帮助目录",
                ShowOrder = order,
                ModuleURL = "/HelpContent/",
                IndicateImage = "icon-Son_HelpContents",
                DeskTopImage = "image-Son_HelpContents",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("9280918A-632F-43a9-B611-D7597C858EA8")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("4C0DF8C9-5B71-4e78-AC9A-6E4B6220245D"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            },
            new Function()
            {
                FunctionID = new Guid("C67CB9CE-732D-49c9-8AD3-33E417E39EB7"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            },
            new Function()
            {
                FunctionID = new Guid("973E55CE-A543-4e01-B907-992B67A63B2D"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            },
            new Function()
            {
                FunctionID = new Guid("8D0226E3-9790-4b6d-AB19-6FD9635AC9D3"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            },
            new Function()
            {
                FunctionID = new Guid("1DA521F8-25AD-40ca-B581-36168D3BF9AD"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            },
            new Function()
            {
                FunctionID = new Guid("DEEB9C0E-D9BB-446b-8A93-3448EFC9989E"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("008DD08E-CC34-4f41-98A6-3FAC28F89CED")
            });
        }
        private void ModuleHelpEdit(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3"),
                ModuleName = "帮助维护",
                ShowOrder = order,
                ModuleURL = "/HelpEdit/",
                IndicateImage = "icon-Son_HelpEdit",
                DeskTopImage = "image-Son_HelpEdit",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("9280918A-632F-43a9-B611-D7597C858EA8")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("1C4D26DE-B10A-4bf9-A060-17FEDA640C11"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            },
            new Function()
            {
                FunctionID = new Guid("57D9BE18-9943-405c-AC2B-A6CBD34A6C84"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            },
            new Function()
            {
                FunctionID = new Guid("3594D7A3-E40A-475e-A4A4-53335152ED42"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            },
            new Function()
            {
                FunctionID = new Guid("74CEDC92-2DF4-43a6-9BF0-DC24CAE024AD"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            },
            new Function()
            {
                FunctionID = new Guid("FA842F7D-2F2E-4563-BE14-1FC90066E49F"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            },
            new Function()
            {
                FunctionID = new Guid("D3E30CA1-7244-4ed4-ADF7-7984C3E56395"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("A9FB8AFF-4233-4824-94A6-52D4B37466B3")
            });
        }
        private void ModuleHelp(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("63D5366F-1B84-4d72-9DC0-38BB68F0AAE0"),
                ModuleName = "帮助主页",
                ShowOrder = order,
                ModuleURL = "/Help/",
                IndicateImage = "icon-Son_HelpIndex",
                DeskTopImage = "image-Son_HelpIndex",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("9280918A-632F-43a9-B611-D7597C858EA8")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("F1F6E2D4-5A97-478e-82A5-BE66B1F03F71"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("63D5366F-1B84-4d72-9DC0-38BB68F0AAE0")
            },
            new Function()
            {
                FunctionID = new Guid("A187C965-7866-4945-9926-FA756925F584"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("63D5366F-1B84-4d72-9DC0-38BB68F0AAE0")
            });
        } 
        #endregion


        private void CreateOrg(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                   new Module()
                   {
                       ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101"),
                       ModuleName = "组织结构管理",
                       ShowOrder = 1,
                       ModuleURL = "",
                       IndicateImage = "icon-Menu_Organization",
                       DeskTopImage = "image-Menu_Organization",
                       System = system,
                       System_SystemID = system.SystemID,
                       ParentModule_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101")
                   },
                   new Module()
                   {
                       ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102"),
                       ModuleName = "公司信息",
                       ShowOrder = 1,
                       ModuleURL = "/Company/",
                       IndicateImage = "icon-son_Company",
                       DeskTopImage = "image-son_Company",
                       System = system,
                       System_SystemID = system.SystemID,
                       ParentModule_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101")
                   },
                   new Module()
                   {
                       ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103"),
                       ModuleName = "部门信息",
                       ShowOrder = 2,
                       ModuleURL = "/Department/",
                       IndicateImage = "icon-son_Department",
                       DeskTopImage = "image-son_Department",
                       System = system,
                       System_SystemID = system.SystemID,
                       ParentModule_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101")
                   },
                   new Module()
                   {
                       ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104"),
                       ModuleName = "岗位信息",
                       ShowOrder = 3,
                       ModuleURL = "/Job/",
                       IndicateImage = "icon-son_Job",
                       DeskTopImage = "image-son_Job",
                       System = system,
                       System_SystemID = system.SystemID,
                       ParentModule_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101")
                   },
                   new Module()
                   {
                       ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105"),
                       ModuleName = "员工信息",
                       ShowOrder = 4,
                       ModuleURL = "/Employee/",
                       IndicateImage = "icon-son_Employee",
                       DeskTopImage = "image-son_Employee",
                       System = system,
                       System_SystemID = system.SystemID,
                       ParentModule_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471101")
                   }
               );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("A85AB2B3-5949-4ebf-A55F-7A46DA21EAD0"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("3E296244-5B4F-46c9-A456-FA88463D612E"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("84FC6FD9-4F81-4300-8946-7D250D98DF71"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("D368026A-68B0-4310-9532-681A62BD9670"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("C355CED1-0780-4a2e-9B81-42CC5F714808"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("65F990D8-9AC8-4718-A768-C85D37346F23"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("4957BC1E-FB21-455f-8CC3-BE1383824FC6"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("8B157B15-2827-424c-8099-806696639B1D"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("03E3D962-8442-4cf3-A634-3E0573A74046"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("C6BF5241-4D85-47a0-BAC6-969F3AF0D8E2"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("2767CA63-5260-45d1-9CCC-4AC05AAC50CB"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("4A453138-1DF0-444d-8869-FB9670E85757"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("DF01E03B-F6E2-4e68-AB0C-256F2F3FC7AE"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("8A457D90-3594-4293-AA5D-5E62A6537343"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("E322D367-75DB-43ce-9B7D-709A9131B484"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("179BE697-F7BF-4b54-8599-42FAF6BC7290"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("B31011DA-AA8D-4ea5-8F7E-979CFD31B605"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("54C285D8-AB98-454e-AC7E-F51E55339863"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("9B19A906-A2D3-4089-AF69-40E752F9C0D7"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("EF2FF820-81B7-410a-A3EE-E0809DD152C0"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("080FFAD5-9C58-4ed5-9F2A-24ABEAE7900E"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("C1E95428-3FEC-484c-845E-A4173B9FA924"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("F9F33A01-76BB-4232-87D2-1DC3B6109AC8"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("20DF1E53-5D2F-4147-8097-88F134E794AE"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("A8344F88-08AD-4FDA-8F45-EAD3BB471105")
                }
                );
            context.SaveChanges();
        }

        private void CreateWarehouse(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101"),
                        ModuleName = "仓库信息管理",
                        ShowOrder = 2,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Warehouse",
                        DeskTopImage = "image-Menu_Warehouse",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "仓库信息设置",
                        ShowOrder = 1,
                        ModuleURL = "/Warehouse2/",
                        IndicateImage = "icon-son_Warehouse",
                        DeskTopImage = "image-son_Warehouse",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "储位卷烟预设",
                        ShowOrder = 2,
                        ModuleURL = "/DefaultProductSet/",
                        IndicateImage = "icon-son_Warehouse",
                        DeskTopImage = "image-son_Warehouse",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("0587FC63-9DE4-447F-B15C-7261A6CBB2EA"),
                        ModuleName = "拆盘位预设置",
                        ShowOrder = 3,
                        ModuleURL = "/SplitPalletCell/",
                        IndicateImage = "icon-son_Bill_Type",
                        DeskTopImage = "image-son_Bill_Type",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("A410FB87-3C37-4CC9-8284-9D08EC2C3AD8"),
                        ModuleName = "库存货位预览",
                        ShowOrder = 4,
                        ModuleURL = "/StorageCellPreview/",
                        IndicateImage = "icon-search",
                        DeskTopImage = "image-search",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("57237A92-3213-4188-8240-BEF7A2C221AD"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("A6CA0BC0-215F-44c3-8AA3-0FFF4C2F0495"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("82EE5370-3E65-49b3-87BB-86B75E671A4D"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("D0FC3809-4204-4130-8948-3B0039E62851"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("39D78BC8-CA6B-41f3-907E-A6BE76D87487"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("5468C301-1B83-4311-98B4-98AA9A5CF3E0"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("B8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAAA7A92-3213-4188-8240-BEF7A2C221AD"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAAA0BC0-215F-44c3-8AA3-0FFF4C2F0495"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAAE5370-3E65-49b3-87BB-86B75E671A4D"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAAC3809-4204-4130-8948-3B0039E62851"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAA78BC8-CA6B-41f3-907E-A6BE76D87487"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("AAA8C301-1B83-4311-98B4-98AA9A5CF3E0"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D3344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    //拆盘位预设置
                    FunctionID = new Guid("6D2E6B3C-F0C2-4CE4-9870-991F06B82B6A"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("0587FC63-9DE4-447F-B15C-7261A6CBB2EA")
                },
                new Function()
                {
                    FunctionID = new Guid("358A066F-0656-4125-ABFF-9700EDA376A8"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("0587FC63-9DE4-447F-B15C-7261A6CBB2EA")
                },
                new Function()
                {
                    //库存货位预览
                    FunctionID = new Guid("4C4660E5-7D03-4F4E-BE12-25BB5FB34B4F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("A410FB87-3C37-4CC9-8284-9D08EC2C3AD8")
                }
                );
            context.SaveChanges();
        }

        private void CreateProduct(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101"),
                        ModuleName = "卷烟信息管理",
                        ShowOrder = 3,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Cigarette",
                        DeskTopImage = "image-Menu_Cigarette",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "卷烟信息",
                        ShowOrder = 1,
                        ModuleURL = "/Product/",
                        IndicateImage = "icon-son_CigaretteInfo",
                        DeskTopImage = "image-son_CigaretteInfo",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103"),
                        ModuleName = "厂商信息",
                        ShowOrder = 2,
                        ModuleURL = "/Supplier/",
                        IndicateImage = "icon-son_CigaretteSupplier",
                        DeskTopImage = "image-son_CigaretteSupplier",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104"),
                        ModuleName = "卷烟品牌",
                        ShowOrder = 3,
                        ModuleURL = "/Brand/",
                        IndicateImage = "icon-son_CigaretteBrand",
                        DeskTopImage = "image-son_CigaretteBrand",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105"),
                        ModuleName = "单位系列",
                        ShowOrder = 4,
                        ModuleURL = "/UnitList/",
                        IndicateImage = "icon-son_CigaretteUnitList",
                        DeskTopImage = "image-son_CigaretteUnitList",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106"),
                        ModuleName = "计量单位",
                        ShowOrder = 5,
                        ModuleURL = "/Unit/",
                        IndicateImage = "icon-son_CigaretteUnit",
                        DeskTopImage = "image-son_CigaretteUnit",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                );
            context.SaveChanges();
            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("89E04DB6-DC74-44ec-A6E5-382752824557"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("1374C404-3606-4c8b-BC2E-EB7E2626D4DD"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("561349C3-A2EA-4ee0-BA52-A434D14DA347"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("579D4CA9-6E99-40f0-AD1A-FD9B8916AF8F"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("C3C27660-DE3F-4e77-9C6E-BC37CB2C480A"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("12ACEC75-BEC2-457a-BDCE-0269C9BDED1E"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                     new Function()
                     {
                         FunctionID = new Guid("E3524C69-00BC-4d7d-BD11-73EB62A7D8C1"),
                         FunctionName = "查询",
                         ControlName = "search",
                         IndicateImage = "icon-search",
                         Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                     },
                new Function()
                {
                    FunctionID = new Guid("7DD02B78-6936-4aa6-943D-CF77AC587AD6"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("1FC0760E-8E58-4e5d-9876-0F328BBFC447"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("DAF3A8A6-0092-49b2-9723-E97960A14722"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("1BFE2494-27EC-4b68-826C-AA42EBFA39C9"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("B4ACA461-0A8C-4387-BC20-952F931418BD"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                     new Function()
                     {
                         FunctionID = new Guid("7815DB01-45FE-4a64-A043-B011D992CA56"),
                         FunctionName = "查询",
                         ControlName = "search",
                         IndicateImage = "icon-search",
                         Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                     },
                new Function()
                {
                    FunctionID = new Guid("727D0507-20E3-49f1-A8C3-1FBE1CA71C9A"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("2B4ED7C0-1645-4620-BFA3-C9F999593A76"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("CC924251-F880-4504-B938-D150A9E162C1"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("AAE6C29E-630D-4297-8830-FA2FC0F25F0D"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("3CD649E5-3B7F-4249-B6A7-C5B8FFB0CDE4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                     new Function()
                     {
                         FunctionID = new Guid("8BB188B4-6280-4854-9340-9A3C14FE4E77"),
                         FunctionName = "查询",
                         ControlName = "search",
                         IndicateImage = "icon-search",
                         Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                     },
                new Function()
                {
                    FunctionID = new Guid("C3FF7F83-067B-440f-A627-17932F216796"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("2D691698-BFD1-44fc-BE2A-9FAAF709354F"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("9BC0C657-734F-4904-9511-FF0E5254CE40"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("AB14EE13-BFFA-451b-9522-3552EA98F53D"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("B0CCE73E-E072-4510-8DC4-F21C36BA0DC1"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                     new Function()
                     {
                         FunctionID = new Guid("2406CEAA-5493-438c-A738-148C5966959C"),
                         FunctionName = "查询",
                         ControlName = "search",
                         IndicateImage = "icon-search",
                         Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                     },
                new Function()
                {
                    FunctionID = new Guid("4FBC4C07-6E8B-4fe8-A440-757FC67DDD46"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("82DE0F87-6B85-4d39-BF9B-0B370F91328A"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("CC01F548-84AD-4a57-93FC-3294A9052F56"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("8504A60A-8982-4cc4-BB8B-D7AAE118A037"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("1C2DFE80-92ED-4c1b-98E9-9F8845430222"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("C8344F88-08AD-4FDA-8F45-EAD3BB471106")
                }
                );
            context.SaveChanges();
        }

        #region 仓库单据管理
        private void CreateStockBill(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101"),
                ModuleName = "仓库单据管理",
                ShowOrder = 4,
                ModuleURL = "",
                IndicateImage = "icon-Menu_StockInto",
                DeskTopImage = "image-Menu_StockInto",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            ModuleStockType(context, system, 1);
            ModuleStockIn(context, system, 2);
            ModuleStockOut(context, system, 3);
            ModuleStockMove(context, system, 4);
            ModuleStockCheck(context, system, 5);
            ModuleProfitLoss(context, system, 6);
            context.SaveChanges();
        }
        private void ModuleStockType(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F"),
                ModuleName = "单据类型",
                ShowOrder = order,
                ModuleURL = "/StockBillType/",
                IndicateImage = "icon-son_Bill_Type",
                DeskTopImage = "image-son_Bill_Type",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("FFBB3E38-4455-468E-B736-DAD500E7ADD8"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            },
            new Function()
            {
                FunctionID = new Guid("4308378B-D764-4CFD-AB5D-1426C51B17D6"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            },
            new Function()
            {
                FunctionID = new Guid("E6FF8887-B971-436F-AF7D-495851DB3756"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            },
            new Function()
            {
                FunctionID = new Guid("3944C6AB-C60C-42C3-8A21-587B310DF3EE"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            },
            new Function()
            {
                FunctionID = new Guid("381C48D7-8E17-4B76-80FA-24E735D7A411"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            },
            new Function()
            {
                FunctionID = new Guid("118FFA18-F6DC-4F92-B636-B15EC103F77A"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("FE331262-4344-4280-84E1-889AAC19FD8F")
            });
        }
        private void ModuleStockIn(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103"),
                ModuleName = "入库单",
                ShowOrder = order,
                ModuleURL = "/StockInBill/",
                IndicateImage = "icon-son_StockIntoBill",
                DeskTopImage = "image-son_StockIntoBill",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("6305942A-32EC-40b9-AAB6-178D048387B3"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("FE1E48EF-9086-48df-BD3B-AA3FFCAA4A24"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("51225C33-035C-4214-98F8-F3D8D33C101E"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("A4BE0DFB-B943-4d86-A0DB-2EECFAB85F96"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142C0"),
                FunctionName = "下载",
                ControlName = "download",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142D1"),
                FunctionName = "审核",
                ControlName = "audit",
                IndicateImage = "icon-ok",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142D2"),
                FunctionName = "反审",
                ControlName = "antitrial",
                IndicateImage = "icon-undo",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142C1"),
                FunctionName = "分配",
                ControlName = "allot",
                IndicateImage = "icon-Menu_CheckBill",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("FFF65519-A56F-44e2-856D-24C2868142C1"),
                FunctionName = "结单",
                ControlName = "settle",
                IndicateImage = "icon-Menu_CheckBill",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("7EA785AF-EE61-4b68-9B7C-3E97E325D81C"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("A311C19E-7319-4dfa-A2F0-D79B449F29B7"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("7B5B9063-D425-4911-9345-6433ADF6610A"),
                FunctionName = "迁移",
                ControlName = "migration",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("E0000000-D425-4911-9345-6433ADF6610A"),
                FunctionName = "作业",
                ControlName = "task",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471103")
            });
        }
        private void ModuleStockOut(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103"),
                ModuleName = "出库单",
                ShowOrder = 4,
                ModuleURL = "/StockOutBill/",
                IndicateImage = "icon-son_StockOutBill",
                DeskTopImage = "image-son_StockOutBill",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("660B5C53-48FC-4eee-96A3-72376912A044"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("1598883A-E9A1-4269-B0E8-E1E6C02E6989"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("9FE35ED3-FF3C-4952-948E-2E0939F9684A"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("AF49D51B-E421-4067-AF55-461FB0D63A83"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("EF3789C5-9369-4f93-BFFB-E300C0493698"),
                FunctionName = "下载",
                ControlName = "download",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            }, new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142E1"),
                FunctionName = "审核",
                ControlName = "audit",
                IndicateImage = "icon-ok",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142E2"),
                FunctionName = "反审",
                ControlName = "antitrial",
                IndicateImage = "icon-undo",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("87565519-A56F-44e2-856D-24C2868142C2"),
                FunctionName = "分配",
                ControlName = "allot",
                IndicateImage = "icon-Menu_CheckBill",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("DDD65519-A56F-44e2-856D-24C2868142C1"),
                FunctionName = "结单",
                ControlName = "settle",
                IndicateImage = "icon-Menu_CheckBill",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("3AA9334F-15FB-4849-BF90-67B24A0C8600"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("B30581E3-FC5B-4b49-87CD-AE03E4D80093"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("65F3A9C6-C3C7-4C06-992F-CD19FA4182E0"),
                FunctionName = "迁移",
                ControlName = "migration",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("E0000000-D425-4911-9345-6433ADF66104"),
                FunctionName = "作业",
                ControlName = "task",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("E8344F88-08AD-4FDA-8F45-EAD3BB471103")
            });
        }
        private void ModuleStockMove(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103"),
                ModuleName = "移库单",
                ShowOrder = 6,
                ModuleURL = "/StockMoveBill/",
                IndicateImage = "icon-son_MoveBill",
                DeskTopImage = "image-son_MoveBill",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("EC3C66E1-4C9D-4374-B859-1B57347B1DA0"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("0E76C6E1-D27E-4534-B7B6-E1A53784E4DD"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("185F9355-253E-4052-843A-98BFEB1378CF"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("2AE3AFF1-E2E4-48f5-B66B-EBBFCD74EC6C"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("37565519-A56F-44e2-856D-24C2868142D1"),
                FunctionName = "审核",
                ControlName = "audit",
                IndicateImage = "icon-ok",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("EEE65519-A56F-44e2-856D-24C2868142D1"),
                FunctionName = "反审",
                ControlName = "antitrial",
                IndicateImage = "icon-undo",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("EEE65519-A56F-44e2-856D-24C2868142C1"),
                FunctionName = "结单",
                ControlName = "settle",
                IndicateImage = "icon-Menu_CheckBill",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("8B214732-8B57-4026-A89B-93BC125D2362"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("F2CB4D5B-7BD5-49e6-BFFC-78FDCEF7349E"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("2A115F40-39D7-45D7-9472-01F448D87C21"),
                FunctionName = "迁移",
                ControlName = "migration",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("E0000000-D425-4911-9345-6433ADF66101"),
                FunctionName = "作业",
                ControlName = "task",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("F8344F88-08AD-4FDA-8F45-EAD3BB471103")
            });
        }
        private void ModuleStockCheck(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104"),
                ModuleName = "盘点单",
                ShowOrder = 8,
                ModuleURL = "/CheckBill/",
                IndicateImage = "icon-son_CheckBill",
                DeskTopImage = "image-son_CheckBill",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("B981741F-76D6-4da7-A417-873262A5890E"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("A98EDD95-4C18-4df5-9A82-87E1E42B26C9"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("2A0F2708-150D-43fb-8F41-26C94ED653D4"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("0042DEF4-07FE-4cd6-870E-A0A9C56652E5"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("F7565519-A56F-44e2-856D-24C2868142D1"),
                FunctionName = "审核",
                ControlName = "audit",
                IndicateImage = "icon-ok",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("F7565519-A56F-44e2-856D-24C2868142D2"),
                FunctionName = "反审",
                ControlName = "antitrial",
                IndicateImage = "icon-undo",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("F6E6597A-64A9-4eb2-9C72-9C24C39F5652"),
                FunctionName = "确认",
                ControlName = "confirm",
                IndicateImage = "icon-tip",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("36E6597A-64A9-4eb2-9C72-9C24C39F5652"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("20EB0C11-A9A3-42ab-860C-3BBCC0DE7935"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("E0000000-64A9-4eb2-9C72-9C24C39F5652"),
                FunctionName = "迁移",
                ControlName = "migration",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            },
            new Function()
            {
                FunctionID = new Guid("87CCA685-5A77-47CC-B4A2-000E793DA8C9"),
                FunctionName = "作业",
                ControlName = "task",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("FA344F88-08AD-4FDA-8F45-EAD3BB471104")
            });
            context.SaveChanges();
        }
        private void ModuleProfitLoss(AuthorizeContext context, SystemInfo system, int order)
        {
            context.Set<Module>().AddOrUpdate(
            new Module()
            {
                ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103"),
                ModuleName = "损益单",
                ShowOrder = 10,
                ModuleURL = "/ProfitLossBill/",
                IndicateImage = "icon-son_DifferBill",
                DeskTopImage = "image-son_DifferBill",
                System = system,
                System_SystemID = system.SystemID,
                ParentModule_ModuleID = new Guid("D8344F88-08AD-4FDA-8F45-EAD3BB471101")
            });
            context.Set<Function>().AddOrUpdate(
            new Function()
            {
                FunctionID = new Guid("B509B802-D5B9-46eb-8013-AE7494D1CC90"),
                FunctionName = "查询",
                ControlName = "search",
                IndicateImage = "icon-search",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("BCA32B01-5FD2-4407-87D5-F86E62734722"),
                FunctionName = "新增",
                ControlName = "add",
                IndicateImage = "icon-add",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("0E40BB17-1A3E-49f5-8816-200093AF3150"),
                FunctionName = "编辑",
                ControlName = "edit",
                IndicateImage = "icon-edit",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("BB80FFAE-27AD-4bf3-99B1-CDF420145AAA"),
                FunctionName = "删除",
                ControlName = "delete",
                IndicateImage = "icon-remove",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("D7565519-A56F-44e2-856D-24C2868142D1"),
                FunctionName = "审核",
                ControlName = "audit",
                IndicateImage = "icon-ok",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("C3C1362E-2946-46f4-8939-40ADD4A275DE"),
                FunctionName = "打印",
                ControlName = "print",
                IndicateImage = "icon-print",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("C70881F2-C98B-4b83-BCAD-E151E70E8439"),
                FunctionName = "帮助",
                ControlName = "help",
                IndicateImage = "icon-help",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            },
            new Function()
            {
                FunctionID = new Guid("0758E8EB-715F-4D95-B9DC-E2D88DC3DA8C"),
                FunctionName = "迁移",
                ControlName = "migration",
                IndicateImage = "icon-reload",
                Module_ModuleID = new Guid("FB344F88-08AD-4FDA-8F45-EAD3BB471103")
            });
        } 
        #endregion

        private void CreateSearch(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101"),
                        ModuleName = "历史单据查询",
                        ShowOrder = 5,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Research",
                        DeskTopImage = "image-Menu_Research",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "入库单查询",
                        ShowOrder = 2,
                        ModuleURL = "/StockIntoSearch/",
                        IndicateImage = "icon-son_StockIntoBill",
                        DeskTopImage = "image-son_StockIntoBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471103"),
                        ModuleName = "出库单查询",
                        ShowOrder = 3,
                        ModuleURL = "/StockOutSearch/",
                        IndicateImage = "icon-son_StockOutBill",
                        DeskTopImage = "image-son_StockOutBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471104"),
                        ModuleName = "移库单查询",
                        ShowOrder = 4,
                        ModuleURL = "/StockMoveSearch/",
                        IndicateImage = "icon-son_MoveBill",
                        DeskTopImage = "image-son_MoveBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471105"),
                        ModuleName = "盘点单查询",
                        ShowOrder = 5,
                        ModuleURL = "/StockCheckSearch/",
                        IndicateImage = "icon-son_CheckBill",
                        DeskTopImage = "image-son_CheckBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471106"),
                        ModuleName = "损益单查询",
                        ShowOrder = 6,
                        ModuleURL = "/StockDifferSearch/",
                        IndicateImage = "icon-son_DifferBill",
                        DeskTopImage = "image-son_DifferBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471107"),
                        ModuleName = "分拣单查询",
                        ShowOrder = 7,
                        ModuleURL = "/SortOrderSearch/",
                        IndicateImage = "icon-son_SortOrder",
                        DeskTopImage = "image-son_SortOrder",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("4E85AFA7-91BC-44ee-853B-A185A0246A88"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("7C77134B-A8CE-4ff1-88C7-2DCE25F89B51"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("E810A404-246D-4825-A721-419C582F33E4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("8656F99A-977F-4777-B4FF-61EFDA5DBBBE"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("4C974309-03BE-452b-A8C7-FC44685E0922"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("D214B545-F49E-414c-898F-4831E9F10856"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("C71C71CC-700F-4258-8178-1AF5939FD130"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("FBBECF2C-66EE-447a-A196-4E2919854D85"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("6C1F7938-3777-4de1-8060-6C620F7B56C9"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("9C568B0A-4137-4760-B0A3-43F26F02E610"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("7C9F7A2C-AB08-42bc-982E-905D745EB146"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("08644954-4353-42f6-A966-BDC93D008A58"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("32152D4A-E601-41d3-ADAD-0EE2B8F7EF2E"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("FBD92FE9-A07F-4a15-9EA9-532FFB499747"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("E113E3D4-378C-486f-93E6-5EBA39214CC3"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("C6B0E124-345B-420a-A227-8F94D65C0768"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471107")
                },
                new Function()
                {
                    FunctionID = new Guid("AF2B7AEB-8883-4e09-8153-28CCD42F74D9"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471107")
                },
                new Function()
                {
                    FunctionID = new Guid("FF238EBF-7A7F-490e-ABD2-867FB726EAF1"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FE344F88-08AD-4FDA-8F45-EAD3BB471107")
                }
               );
            context.SaveChanges();
        }

        private void CreateStock(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101"),
                        ModuleName = "库存信息查询",
                        ShowOrder = 6,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_WarehouseM",
                        DeskTopImage = "image-Menu_WarehouseM",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "仓库库存日结",
                        ShowOrder = 1,
                        ModuleURL = "/DailyBalance/",
                        IndicateImage = "icon-son_StockDayBill",
                        DeskTopImage = "image-son_StockDayBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471103"),
                        ModuleName = "当前库存查询",
                        ShowOrder = 2,
                        ModuleURL = "/CurrentStock/",
                        IndicateImage = "icon-son_StockNow",
                        DeskTopImage = "image-son_StockNow",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471104"),
                        ModuleName = "库存分布查询",
                        ShowOrder = 3,
                        ModuleURL = "/Distribution/",
                        IndicateImage = "icon-son_StockPlace",
                        DeskTopImage = "image-son_StockPlace",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471105"),
                        ModuleName = "货位库存查询",
                        ShowOrder = 4,
                        ModuleURL = "/Cargospace/",
                        IndicateImage = "icon-son_StockArea",
                        DeskTopImage = "image-son_StockArea",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471106"),
                        ModuleName = "库存历史总账",
                        ShowOrder = 5,
                        ModuleURL = "/StockLedger/",
                        IndicateImage = "icon-son_StockOldAll",
                        DeskTopImage = "image-son_StockOldAll",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                    ,
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471107"),
                        ModuleName = "库存历史明细",
                        ShowOrder = 6,
                        ModuleURL = "/HistoricalDetail/",
                        IndicateImage = "icon-son_StockOlddetails",
                        DeskTopImage = "image-son_StockOlddetails",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                     ,
                    new Module()
                    {
                        ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471188"),
                        ModuleName = "货位历史明细",
                        ShowOrder = 7,
                        ModuleURL = "/CellHistorical/",
                        IndicateImage = "icon-son_StockOlddetails",
                        DeskTopImage = "image-son_StockOlddetails",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("54D23A0C-DE19-4cb3-9501-B270AE8790A6"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("F4D23A0C-DE19-4cb3-9501-B270AE8790A6"),
                    FunctionName = "日结",
                    ControlName = "balance",
                    IndicateImage = "icon-son_StockDayBill",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("08620A48-281C-4cf9-8D61-D7A232F19557"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("FC13ED35-EC24-42c1-BB26-F1BDEEE8D1ED"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("EFCBEFED-32C9-4031-8FB4-21DFEF3EF9A8"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("EF48B5AA-35EA-4ADC-A229-89C05DD36CC4"),
                    FunctionName = "迁移",
                    ControlName = "migration",
                    IndicateImage = "icon-reload",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("6C52AB11-5E5E-4bce-88E8-AA33CB4994ED"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("51963EC5-0C7B-45e1-995E-65E89E5AD135"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("16FCCB0F-D99C-4e58-AE14-F61B0BAAA6B0"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("C1814498-1069-473d-95D8-DD18CFABEAA2"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("46BD48EF-BBE5-4699-B0E2-1AF6B353CC78"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("7F2A6227-2A40-45c6-8BF6-5271B041089F"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("45B68172-26D0-4d92-9135-4AAFA820E0CE"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("384D20FE-7035-40b8-9808-6A714AE2ED90"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("CFF1F588-4C65-412b-BDD8-A6732DFED24D"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("E28CD5F7-36AD-4df0-BA0F-867FA4E47AC0"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("99ABB6BB-5846-4ad7-9D31-B65BABFAAAE6"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("FF73D025-EDD8-41b2-B393-05DAC0BA75D3"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("0BB4ACC6-3964-4520-B3F3-D29E28A96D29"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471107")
                },
                new Function()
                {
                    FunctionID = new Guid("B6197758-0DEF-43d8-BDCC-E0609D48D80C"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471107")
                },
                new Function()
                {
                    FunctionID = new Guid("6375FB25-510E-4431-BD91-8FA8D75CAC05"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471107")
                },
                new Function()
                {
                    FunctionID = new Guid("0BB4ACC6-3964-4520-B3F3-D29E28A96D88"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471188")
                },
                new Function()
                {
                    FunctionID = new Guid("B6197758-0DEF-43d8-BDCC-E0609D48D888"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471188")
                },
                new Function()
                {
                    FunctionID = new Guid("6375FB25-510E-4431-BD91-8FA8D75CAC88"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FC344F88-08AD-4FDA-8F45-EAD3BB471188")
                }
                );
            context.SaveChanges();
        }

        private void CreateProductQuality(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B"),
                        ModuleName = "产品质量管理",
                        ShowOrder = 7,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_ProductQuality",
                        DeskTopImage = "image-Menu_ProductQuality",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F"),
                        ModuleName = "产品预警信息设置",
                        ShowOrder = 2,
                        ModuleURL = "/ProductWarning/",
                        IndicateImage = "icon-Son_ProductWarning",
                        DeskTopImage = "image-Son_ProductWarning",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("61069EC9-D0F4-478F-A900-BAF784607214"),
                        ModuleName = "产品超储短缺查询",
                        ShowOrder = 3,
                        ModuleURL = "/QuantityLimits/",
                        IndicateImage = "icon-son_StockOutBill",
                        DeskTopImage = "image-son_StockOutBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("DE67D3BC-EB84-47E8-984C-076C2F6AE89E"),
                        ModuleName = "积压产品清单",
                        ShowOrder = 4,
                        ModuleURL = "/ProductTimeOut/",
                        IndicateImage = "icon-son_MoveBill",
                        DeskTopImage = "image-son_MoveBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("05E82C3C-AC7D-49E3-AF8A-7DE741E412CF"),
                        ModuleName = "库存变化分析",
                        ShowOrder = 5,
                        ModuleURL = "/StorageAnalysis/",
                        IndicateImage = "icon-son_CheckBill",
                        DeskTopImage = "image-son_CheckBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("2EE9ADD1-A254-47DE-8FA5-2516AE4FF3D1"),
                        ModuleName = "库区占有率分析",
                        ShowOrder = 6,
                        ModuleURL = "/CellAnalysis/",
                        IndicateImage = "icon-Son_CellAnalysis",
                        DeskTopImage = "image-Son_CellAnalysis",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                    },
                     new Module()
                     {
                         ModuleID = new Guid("B9168A39-D6F8-4ACA-875C-A8B5827CD9B8"),
                         ModuleName = "货位利用分析",
                         ShowOrder = 7,
                         ModuleURL = "/CellInfo/",
                         IndicateImage = "icon-son_DifferBill",
                         DeskTopImage = "image-son_DifferBill",
                         System = system,
                         System_SystemID = system.SystemID,
                         ParentModule_ModuleID = new Guid("D7E448C6-7CBE-4F5A-B8E8-98193619D52B")
                     }
                );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("59E749FF-15BD-4B65-A6F4-6FCFEE5DA594"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                },
                 new Function()
                 {
                     FunctionID = new Guid("F8BF2A92-5BFA-4E17-8764-2564A7625557"),
                     FunctionName = "新增",
                     ControlName = "add",
                     IndicateImage = "icon-add",
                     Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                 },
                  new Function()
                  {
                      FunctionID = new Guid("BC59ACEB-7F00-4C69-B4E1-21D29525E6B9"),
                      FunctionName = "编辑",
                      ControlName = "edit",
                      IndicateImage = "icon-edit",
                      Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                  },
                   new Function()
                   {
                       FunctionID = new Guid("919AAF97-FB5B-42BB-8B6C-ED2612280809"),
                       FunctionName = "删除",
                       ControlName = "delete",
                       IndicateImage = "icon-remove",
                       Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                   },
                new Function()
                {
                    FunctionID = new Guid("11C91342-C394-4705-85B6-F02707AE7DD2"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                },
                new Function()
                {
                    FunctionID = new Guid("F09EA811-5FCE-45E1-926E-9F31EC065B94"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("AB1BE2ED-1522-4722-ADC6-82654656589F")
                },
                new Function()
                {
                    FunctionID = new Guid("F11A0589-4B6D-402C-BBD5-82B1F800B187"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("61069EC9-D0F4-478F-A900-BAF784607214")
                },
                new Function()
                {
                    FunctionID = new Guid("F9A4D9C6-9127-403A-9DB4-D954302F460C"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("61069EC9-D0F4-478F-A900-BAF784607214")
                },
                new Function()
                {
                    FunctionID = new Guid("6D8C7068-DD3B-4280-8046-17D292BD94EE"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("61069EC9-D0F4-478F-A900-BAF784607214")
                },
                new Function()
                {
                    FunctionID = new Guid("5B50252A-5B3E-4041-9B7D-D8377A22360B"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("DE67D3BC-EB84-47E8-984C-076C2F6AE89E")
                },
                new Function()
                {
                    FunctionID = new Guid("735D6D64-88B3-43D7-AA6C-E0D802793883"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("DE67D3BC-EB84-47E8-984C-076C2F6AE89E")
                },
                new Function()
                {
                    FunctionID = new Guid("2A14B0CC-A5F2-49E2-8E81-A2B1E7501473"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("DE67D3BC-EB84-47E8-984C-076C2F6AE89E")
                },
                new Function()
                {
                    FunctionID = new Guid("C6F6FBD7-829B-4163-831A-0688C2F6EB0B"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("05E82C3C-AC7D-49E3-AF8A-7DE741E412CF")
                },
                new Function()
                {
                    FunctionID = new Guid("6B023D92-3ED0-40E2-8393-8ACC924E562F"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("05E82C3C-AC7D-49E3-AF8A-7DE741E412CF")
                },
                new Function()
                {
                    FunctionID = new Guid("F835A1A2-CDA8-47C5-B869-2674316A119A"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("05E82C3C-AC7D-49E3-AF8A-7DE741E412CF")
                },
                new Function()
                {
                    FunctionID = new Guid("4439341B-3224-4222-94F9-4ACDE1593C04"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("2EE9ADD1-A254-47DE-8FA5-2516AE4FF3D1")
                },
                new Function()
                {
                    FunctionID = new Guid("1E1A4438-DCF8-43DF-9D51-6297CAD27208"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("2EE9ADD1-A254-47DE-8FA5-2516AE4FF3D1")
                },
                new Function()
                {
                    FunctionID = new Guid("649A9350-24BA-4A60-9305-BD5654228522"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("2EE9ADD1-A254-47DE-8FA5-2516AE4FF3D1")
                },
                 new Function()
                 {
                     FunctionID = new Guid("451C7DE6-8FED-4AF4-A444-A7A96AF49FEB"),
                     FunctionName = "查询",
                     ControlName = "search",
                     IndicateImage = "icon-search",
                     Module_ModuleID = new Guid("B9168A39-D6F8-4ACA-875C-A8B5827CD9B8")
                 },
                new Function()
                {
                    FunctionID = new Guid("D2565640-6D9C-4DC9-B7E3-895D32374C2E"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("B9168A39-D6F8-4ACA-875C-A8B5827CD9B8")
                }
               );
            context.SaveChanges();
        }
                

        private void CreateSorting(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101"),
                        ModuleName = "分拣调度管理",
                        ShowOrder = 8,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Sort",
                        DeskTopImage = "image-Menu_Sort",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102"),
                        ModuleName = "分拣线信息管理",
                        ShowOrder = 1,
                        ModuleURL = "/SortingLine/",
                        IndicateImage = "icon-son_SortInfo",
                        DeskTopImage = "image-son_SortInfo",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103"),
                        ModuleName = "备货区下限管理",
                        ShowOrder = 2,
                        ModuleURL = "/SortingLowerLimit/",
                        IndicateImage = "icon-son_StockAreaDownline",
                        DeskTopImage = "image-son_StockAreaDownline",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471104"),
                        ModuleName = "分拣订单管理",
                        ShowOrder = 3,
                        ModuleURL = "/SortingOrder/",
                        IndicateImage = "icon-son_SortOrder",
                        DeskTopImage = "image-son_SortOrder",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105"),
                        ModuleName = "分拣线路调度",
                        ShowOrder = 4,
                        ModuleURL = "/SortOrderDispatch/",
                        IndicateImage = "icon-son_SortOrderc",
                        DeskTopImage = "image-son_SortOrderc",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106"),
                        ModuleName = "分拣作业调度",
                        ShowOrder = 5,
                        ModuleURL = "/SortWorkDispatch/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471101")
                    }
                );
            context.SaveChanges();
            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("2DDBDA1F-9495-4659-8975-F6388B642947"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("3E14D9AF-ACDB-4195-9FB6-47503B9F1565"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("E41A1AA9-D5B9-4bac-8A29-808FE129130B"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("FBAABA03-4697-4f83-98ED-C3550F54871F"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("6FF92D17-36ED-4282-8973-83A72535F415"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("19F95169-AB68-4dc3-8A07-7D324010A2A1"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471102")
                },
                new Function()
                {
                    FunctionID = new Guid("24F9F100-FC5C-4222-9982-D446403E72D9"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("7AF036DE-EA3A-4026-AC7F-F215AE888B7B"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("299263A1-BAFE-43b2-9739-69479153ED22"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("1A6D4FF6-BC82-4393-88E3-8521B5936889"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("AC729C78-3E3F-478a-8DAA-6C0347BA3F15"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("A6D9FB57-F3EC-43e0-94F4-1DE5115A68EC"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471103")
                },
                new Function()
                {
                    FunctionID = new Guid("2CF05C43-E5B0-4575-85D3-B730BD6E5104"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("135E3A1B-E86A-4a3a-AB2E-DA02AE9DF5B4"),
                    FunctionName = "下载",
                    ControlName = "download",
                    IndicateImage = "icon-reload",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("8F9C9A2E-8BE2-436f-AF4C-28BA6F9C13DE"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("BD299336-2207-4b17-B232-9AD55FBD4654"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471104")
                },
                new Function()
                {
                    FunctionID = new Guid("D33A7F8F-28BE-41c2-9F28-A2FC01D29E2B"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("880A30A1-63B5-4616-8543-92B661392968"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("5FC70683-E090-423a-9706-7C98AB4205AD"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("DAE3AEFF-9A80-4911-90BD-267BC363FBF3"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("2EFA4ADF-CC5C-48ed-B248-EDF496E18A09"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("839F9C61-6C05-48bf-A82E-E05705F727E7"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471105")
                },
                new Function()
                {
                    FunctionID = new Guid("DDE8894B-D24E-4049-8DD5-B7547E772883"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("30A8A88D-B338-4f2b-ACE2-205EAB732084"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("600DE9BB-F3C3-4856-BF1B-17706AC6DE3C"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("EB333E0F-9973-4bcc-BC87-EF253E0BFB7C"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("CCC65519-A56F-44e2-856D-24C2868142D1"),
                    FunctionName = "审核",
                    ControlName = "audit",
                    IndicateImage = "icon-ok",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("CCC65519-A56F-44e2-856D-24C2868142D2"),
                    FunctionName = "反审",
                    ControlName = "antitrial",
                    IndicateImage = "icon-undo",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("CCC65519-A56F-44e2-856D-24C2868142C1"),
                    FunctionName = "结单",
                    ControlName = "settle",
                    IndicateImage = "icon-Menu_CheckBill",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("F9134624-2316-4f1b-B50A-3680FC7CA439"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("60A88801-6EB4-4921-94C1-EE8D31E7A9D4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                },
                new Function()
                {
                    FunctionID = new Guid("E0000000-D425-4911-9345-6433ADF66102"),
                    FunctionName = "作业",
                    ControlName = "task",
                    IndicateImage = "icon-reload",
                    Module_ModuleID = new Guid("FD344F88-08AD-4FDA-8F45-EAD3BB471106")
                }
               );
            context.SaveChanges();
        }               

        private void CreateAutomotiveSystems(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA9"));
            context.Set<Module>().AddOrUpdate(
                new Module()
                {
                    ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD"),
                    ModuleName = "手动作业管理",
                    ShowOrder = 11,
                    ModuleURL = "",
                    IndicateImage = "icon-son_Bill_Type",
                    DeskTopImage = "image-son_Bill_Type",
                    System = system,
                    System_SystemID = system.SystemID,
                    ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                },
                 new Module()
                 {
                     ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB"),
                     ModuleName = "入库作业",
                     ShowOrder = 1,
                     ModuleURL = "/StockInTask/",
                     IndicateImage = "icon-son_StockIntoBill",
                     DeskTopImage = "image-son_StockIntoBill",
                     System = system,
                     System_SystemID = system.SystemID,
                     ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                 },
                 new Module()
                 {
                     ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05"),
                     ModuleName = "出库作业",
                     ShowOrder = 2,
                     ModuleURL = "/StockOutTask/",
                     IndicateImage = "icon-son_StockOutBill",
                     DeskTopImage = "image-son_StockOutBill",
                     System = system,
                     System_SystemID = system.SystemID,
                     ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                 },
                 new Module()
                 {
                     ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598"),
                     ModuleName = "移库作业",
                     ShowOrder = 3,
                     ModuleURL = "/StockMoveTask/",
                     IndicateImage = "icon-son_MoveBill",
                     DeskTopImage = "image-son_MoveBill",
                     System = system,
                     System_SystemID = system.SystemID,
                     ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                 },
                 new Module()
                 {
                     ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C"),
                     ModuleName = "盘点作业",
                     ShowOrder = 4,
                     ModuleURL = "/StockCheckTask/",
                     IndicateImage = "icon-son_CheckBill",
                     DeskTopImage = "image-son_CheckBill",
                     System = system,
                     System_SystemID = system.SystemID,
                     ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                 },
                 new Module()
                 {
                     ModuleID = new Guid("D4C8834D-11D0-4CAE-A12F-E33709242AA1"),
                     ModuleName = "手持作业",
                     ShowOrder = 5,
                     ModuleURL = "/PdaTask/",
                     IndicateImage = "icon-son_Employee",
                     DeskTopImage = "image-son_Employee",
                     System = system,
                     System_SystemID = system.SystemID,
                     ParentModule_ModuleID = new Guid("EA86ECE1-BFE3-42CD-9071-A7131A9280FD")
                 }
             );
            context.SaveChanges();
            context.Set<Function>().AddOrUpdate(
                // 入库作业按钮
                new Function()
                {
                    FunctionID = new Guid("E7CFD8A5-9B4C-4383-ACAF-76F1B92C4AE6"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("368AEC97-1EA1-4016-A37E-503442EEE3E8"),
                    FunctionName = "申请",
                    ControlName = "apply",
                    IndicateImage = "icon-apply",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("DA331CEC-C9AA-4F14-A146-57F4AB399B2C"),
                    FunctionName = "取消",
                    ControlName = "cancel",
                    IndicateImage = "icon-cancel2",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("DE54C0F9-9A2F-47ED-94A0-B13CD9EC5466"),
                    FunctionName = "完成",
                    ControlName = "finish",
                    IndicateImage = "icon-finish",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("40AEB689-8C5A-4EBE-9D70-D8341E17ECDD"),
                    FunctionName = "批量",
                    ControlName = "batch",
                    IndicateImage = "icon-batch",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("5056638E-69CD-4236-B60B-C4E89D6401E5"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                new Function()
                {
                    FunctionID = new Guid("9A408FCE-3D2F-477F-93D1-4D4FDFD68AFD"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("287D79FA-5217-43DF-873A-8430888B8EDB")
                },
                // 出库作业按钮
                new Function()
                {
                    FunctionID = new Guid("9FFBFEA3-CF40-480B-908C-96845C0AF79A"),
                    FunctionName = "查找",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("9AD26265-304C-4AE2-93C4-7CE55A8D598B"),
                    FunctionName = "申请",
                    ControlName = "apply",
                    IndicateImage = "icon-apply",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("C90B96D0-3EC4-4F50-BFE4-E0A0867693EF"),
                    FunctionName = "取消",
                    ControlName = "cancel",
                    IndicateImage = "icon-cancel2",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("0B73416B-5308-457C-ACC8-1F8012DFD640"),
                    FunctionName = "完成",
                    ControlName = "finish",
                    IndicateImage = "icon-finish",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("B38FD945-875D-4B4B-818A-AF7952B5651D"),
                    FunctionName = "批量",
                    ControlName = "batch",
                    IndicateImage = "icon-batch",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("C9C634BA-C628-4A19-8FB5-698849F24E7D"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                new Function()
                {
                    FunctionID = new Guid("97FE8FAC-1B44-45BB-9259-B3A602353625"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("7331492B-A45F-4727-9656-1E2D1EF2DE05")
                },
                // 移库作业按钮
                new Function()
                {
                    FunctionID = new Guid("7CA56AA2-C264-4CDC-9AA3-644B7292CAA1"),
                    FunctionName = "查找",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("59ED93EA-02A1-43E1-9037-3DE3EFB54917"),
                    FunctionName = "申请",
                    ControlName = "apply",
                    IndicateImage = "icon-apply",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("92F94D43-C09C-49DC-A103-A0AE3AB81013"),
                    FunctionName = "取消",
                    ControlName = "cancel",
                    IndicateImage = "icon-cancel2",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("CFEE5475-70D2-4F01-95F2-8A3BFF704D36"),
                    FunctionName = "完成",
                    ControlName = "finish",
                    IndicateImage = "icon-finish",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("DEE0CE91-B090-42DF-A77E-21493B1A4DAB"),
                    FunctionName = "批量",
                    ControlName = "batch",
                    IndicateImage = "icon-batch",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("8C64423E-6B61-41C0-96C3-3E156DEDD1B5"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                new Function()
                {
                    FunctionID = new Guid("D466CDBF-0F4F-4870-B5A8-06DD1ED61EF1"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("B071F22D-38EC-423B-8B90-145B697F7598")
                },
                // 盘点作业按钮
                new Function()
                {
                    FunctionID = new Guid("20AD5809-2485-4FC8-B338-5F51071ADC61"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                new Function()
                {
                    FunctionID = new Guid("20ADED65-404F-4844-8A65-35F7BD64B2B7"),
                    FunctionName = "申请",
                    ControlName = "apply",
                    IndicateImage = "icon-apply",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                new Function()
                {
                    FunctionID = new Guid("EC0195A0-A4D4-47BE-9B79-46D5410F329E"),
                    FunctionName = "取消",
                    ControlName = "cancel",
                    IndicateImage = "icon-cancel2",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                }
                ,
                new Function()
                {
                    FunctionID = new Guid("6CA10850-4F76-40A5-A579-78E56593DF77"),
                    FunctionName = "完成",
                    ControlName = "finish",
                    IndicateImage = "icon-finish",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                new Function()
                {
                    FunctionID = new Guid("D51D7FBC-7B13-4641-87BE-A56BD84F7BF8"),
                    FunctionName = "批量",
                    ControlName = "batch",
                    IndicateImage = "icon-batch",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                new Function()
                {
                    FunctionID = new Guid("2BEFBF62-6927-4F51-8AD0-7DC8C8C61BAC"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                new Function()
                {
                    FunctionID = new Guid("B5A396DC-CD61-4A2E-BB31-157591481A32"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("737DDF67-4ADA-4F57-A239-8CEAFC0E3C0C")
                },
                //手持作业
                new Function()
                {
                    FunctionID = new Guid("1FC72DA0-6D0D-4E6B-B59D-3F8B424A3588"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D4C8834D-11D0-4CAE-A12F-E33709242AA1")
                },
                new Function()
                {
                    FunctionID = new Guid("85440D29-012A-4C84-A0BB-8AA0460323D3"),
                    FunctionName = "完成",
                    ControlName = "finish",
                    IndicateImage = "icon-finish",
                    Module_ModuleID = new Guid("D4C8834D-11D0-4CAE-A12F-E33709242AA1")
                },
                 new Function()
                {
                    FunctionID = new Guid("22445A35-ACF2-450D-A82D-40F48586B112"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("D4C8834D-11D0-4CAE-A12F-E33709242AA1")
                },
                new Function()
                {
                    FunctionID = new Guid("EF73C643-5CB9-4CA0-B0D4-255AC7776CC2"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D4C8834D-11D0-4CAE-A12F-E33709242AA1")
                }
            );
            context.SaveChanges();
        }

        private void CreateSystemInfo(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6EF0-9DEB-4CDE-8DCF-702D5B666AA9"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D"),
                        ModuleName = "基础信息管理",
                        ShowOrder = 10,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Jurisdiction",
                        DeskTopImage = "image-Menu_Jurisdiction",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F90"),
                        ModuleName = "自动作业管理",
                        ShowOrder = 12,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Jurisdiction",
                        DeskTopImage = "image-Menu_Jurisdiction",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F90")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("C78563F5-4886-4587-9137-E1A30267F6B7"),
                        ModuleName = "设备状态查询",
                        ShowOrder = 13,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Jurisdiction",
                        DeskTopImage = "image-Menu_Jurisdiction",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C78563F5-4886-4587-9137-E1A30267F6B7")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("BC8214C9-6AE4-41E0-B298-4E4920B0A06F"),
                        ModuleName = "运行状态查询",
                        ShowOrder = 30,
                        ModuleURL = "/WcsDeviceState/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C78563F5-4886-4587-9137-E1A30267F6B7")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("888548B8-CABC-4C3B-A810-B5FB7229E1D2"),
                        ModuleName = "故障状态查询",
                        ShowOrder = 31,
                        ModuleURL = "/WcsDeviceFault/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("C78563F5-4886-4587-9137-E1A30267F6B7")
                    },
                    new Module
                    {
                        ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E"),
                        ModuleName = "作业任务管理",
                        ShowOrder = 20,
                        ModuleURL = "/TaskManage/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F90")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC"),
                        ModuleName = "堆垛设备信息",
                        ShowOrder = 1,
                        ModuleURL = "/SRM/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    }
                    ,
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC"),
                        ModuleName = "作业区域信息",
                        ShowOrder = 2,
                        ModuleURL = "/Region/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC"),
                        ModuleName = "作业位置信息",
                        ShowOrder = 3,
                        ModuleURL = "/Position/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC"),
                        ModuleName = "作业路径信息",
                        ShowOrder = 4,
                        ModuleURL = "/Path/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC"),
                        ModuleName = "货位位置信息",
                        ShowOrder = 5,
                        ModuleURL = "/CellPosition/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC"),
                        ModuleName = "卷烟尺寸信息",
                        ShowOrder = 6,
                        ModuleURL = "/ProductSize/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC"),
                        ModuleName = "设备报警信息",
                        ShowOrder = 7,
                        ModuleURL = "/AlarmInfo/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("ED0E6EF5-AD8A-4D50-8DB9-71D36EF77F9D")
                    }

            );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                //堆垛机信息
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E4B-8901-C664C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938024C4-5622-43A1-8402-38BF28ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC03E-9618-4B19-8603-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E504-D227-4058-9E04-D77C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB105C2-8A95-4E4B-8905-C66DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C6-5645-43A1-8406-38BFD8ADEFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003311C7B1DC")
                },
                //区域信息
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A97-4E4B-8907-C664C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5628-43A1-8408-38BF28ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9619-4B19-8609-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D287-4058-9E10-D77C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-4E4B-8911-C67DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5645-43A1-8412-38BFD86DEFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003312C7B1DC")
                },
                //位置信息
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-454B-8913-C664C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8414-38BF28ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-4B19-8615-18633D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E299-D227-4058-9E16-D77C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A91-4E4B-8917-C66DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5645-43A1-8418-38BFD8ADEFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003313C7B1DC")
                },
                //路径信息
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E4B-8919-C634C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8420-38BF28A4EFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-5B19-8621-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D227-4056-9E22-D67C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                 },
                  new Function()
                  {
                      FunctionID = new Guid("C8A9E599-D227-4056-93A2-D67C6D3794BB"),
                      FunctionName = "节点",
                      ControlName = "node",
                      IndicateImage = "icon-node",
                      Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                  },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-4E4B-8923-C67DC078919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5647-43A1-8424-38BFD8A8EFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003314C7B1DC")
                },                
                //货位位置
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E6B-8931-C664C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8432-38B428ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-4B19-8633-186A3D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D227-4058-9E34-DB7C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-4E4B-8935-C65DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5645-43A1-8436-38BFD8A1EFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003316C7B1DC")
                },
                //卷烟尺寸
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E4B-8943-C664C068918F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8444-38BF22ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-4B19-8645-18645D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D257-4058-9E46-D77C8D1794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A93-4E4B-8947-C662C06D919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5645-43A1-8448-38BF28DDEFB4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003318C7B1DC")
                },
                //报警信息
                new Function()
                {
                    FunctionID = new Guid("689492BF-43E8-4ED7-87DE-911F233C7BA4"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("E232585A-E7FA-431F-B9EE-F29826DFBDC4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("7ABADC27-57B7-41DA-8929-5F7F1AC9BCEC"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("90867491-7FEF-4109-BB86-A0E416974324"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("FEAC32C4-5EBB-4AC3-ABB9-2484A3A6A8D8"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("B3A6950C-4EF8-444B-8BCF-79F0BF45CCEF"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6EF0-AC3C-4F58-91A7-003319C7B1DC")
                },
                //任务管理
                new Function()
                {
                    FunctionID = new Guid("42D64D0C-2DA2-4FD4-91AC-EF8D9D798CD0"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("9EB915C3-1934-46CF-9028-2DA72862A4FD"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("EFDF4A22-3BEB-420A-8155-7700F5718B3E"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                },
                 new Function()
                 {
                     FunctionID = new Guid("54D0BD9B-E4B0-4C77-B9CA-95E22AB5813D"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("3ACA760B-2340-423F-9611-34379965E8E1"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                 },
                new Function()
                {
                    FunctionID = new Guid("5DCD76B9-E275-4E69-BCC7-D0E305807FFD"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("F035AF71-87D0-4D18-98EE-3318BF65145C"),
                    FunctionName = "清空",
                    ControlName = "empty",
                    IndicateImage = "icon-cancel",
                    Module_ModuleID = new Guid("145A95C4-C37D-48EE-96F1-6EDEAFE70A7E")
                },
                //运行状态查询
                new Function()
                {
                    FunctionID = new Guid("5A4FBFDA-8679-487E-B120-81086E285503"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("BC8214C9-6AE4-41E0-B298-4E4920B0A06F")
                },
                 new Function()
                 {
                     FunctionID = new Guid("44BF77D6-90DB-4586-81C7-E6196BC4E866"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("BC8214C9-6AE4-41E0-B298-4E4920B0A06F")
                 },
                new Function()
                {
                    FunctionID = new Guid("7E4DB709-AC51-4008-B24D-48A2AF0B2419"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("BC8214C9-6AE4-41E0-B298-4E4920B0A06F")
                },
                //故障状态查询
                new Function()
                {
                    FunctionID = new Guid("DB2D7A07-A52E-450D-87F1-121FAA9EAD41"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("888548B8-CABC-4C3B-A810-B5FB7229E1D2")
                },
                 new Function()
                 {
                     FunctionID = new Guid("CB8AC2AF-7293-4B63-A146-93F75B530A92"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("888548B8-CABC-4C3B-A810-B5FB7229E1D2")
                 },
                new Function()
                {
                    FunctionID = new Guid("5AD5E819-0105-4331-875C-6728BEC7F6A5"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("888548B8-CABC-4C3B-A810-B5FB7229E1D2")
                }
            );
            context.SaveChanges();
        }

        private void CreateSystemParameter(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("E8344F88-08AD-4F9A-8F45-EAD8BB471104"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("EDABFF3B-AD8A-4D50-8DB9-71D36EF77F9D"),
                        ModuleName = "系统参数管理",
                        ShowOrder = 10000,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Jurisdiction",
                        DeskTopImage = "image-Menu_Jurisdiction",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("EDABFF3B-AD8A-4D50-8DB9-71D36EF77F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("3C735153-AC3C-4F58-91A7-003311C7B1DC"),
                        ModuleName = "参数设置",
                        ShowOrder = 1,
                        ModuleURL = "/ParameterSet/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("EDABFF3B-AD8A-4D50-8DB9-71D36EF77F9D")
                    }
            );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E4B-8929-C664C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("3C735153-AC3C-4F58-91A7-003311C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8402-38BF28ADEFB4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("3C735153-AC3C-4F58-91A7-003311C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-4B19-86DD-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("3C735153-AC3C-4F58-91A7-003311C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D227-4058-9E5E-D77C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("3C735153-AC3C-4F58-91A7-003311C7B1DC")
                 }
            );
            context.SaveChanges();
        }


        private void CreateSortSystemInfo(AuthorizeContext context)
        {
            SystemInfo system = context.Set<SystemInfo>().SingleOrDefault(s => s.SystemID == new Guid("ED0E6430-9DEB-4CDE-8DCF-702D5B528AA8"));
            context.Set<Module>().AddOrUpdate(
                    new Module()
                    {
                        ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D"),
                        ModuleName = "基础信息管理",
                        ShowOrder = 1,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module
                    {
                        ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E"),
                        ModuleName = "分拣设备信息",
                        ShowOrder = 1,
                        ModuleURL = "/SortingLine/",
                        IndicateImage = "icon-son_SortInfo",
                        DeskTopImage = "image-son_SortInfo",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC"),
                        ModuleName = "分拣烟道信息",
                        ShowOrder = 2,
                        ModuleURL = "/ChannelInfo/",
                        IndicateImage = "icon-son_Warehouse",
                        DeskTopImage = "image-son_Warehouse",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100"),
                        ModuleName = "拆盘位置信息",
                        ShowOrder = 3,
                        ModuleURL = "/SupplyPosition/",
                        IndicateImage = "icon-son_Employee",
                        DeskTopImage = "image-son_Employee",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E914C7B1DC"),
                        ModuleName = "销售卷烟信息",
                        ShowOrder = 4,
                        ModuleURL = "/ProductInfo/",
                        IndicateImage = "icon-son_CigaretteInfo",
                        DeskTopImage = "image-son_CigaretteInfo",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6430-3EA8-4FA8-91A7-6AE712C7B1DC"),
                        ModuleName = "配送区域信息",
                        ShowOrder = 5,
                        ModuleURL = "/DeliverDist/",
                        IndicateImage = "icon-son_StockOutBill",
                        DeskTopImage = "image-son_StockOutBill",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("235A95C8-C37D-6A27-96F1-6EDEA3370A7E"),
                        ModuleName = "配送线路信息",
                        ShowOrder = 6,
                        ModuleURL = "/DeliverLine/",
                        IndicateImage = "icon-son_CigaretteSupplier",
                        DeskTopImage = "image-son_CigaretteSupplier",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B1DC"),
                        ModuleName = "零售客户信息",
                        ShowOrder = 7,
                        ModuleURL = "/Customer/",
                        IndicateImage = "icon-son_Employee",
                        DeskTopImage = "image-son_Employee",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C"),
                        ModuleName = "设备报警信息",
                        ShowOrder = 8,
                        ModuleURL = "/SmsAlarmInfo/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-71D364301F9D")
                    },
                    new Module()
                    {
                         ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0200"),
                         ModuleName = "分拣调度管理",
                         ShowOrder = 2,
                         ModuleURL = "",
                         IndicateImage = "icon-Menu_Sort",
                         DeskTopImage = "image-Menu_Sort",
                         System = system,
                         System_SystemID = system.SystemID,
                         ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0200")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                        ModuleName = "分拣订单管理",
                        ShowOrder = 1,
                        ModuleURL = "/SortingOrder/",
                        IndicateImage = "icon-son_SortOrder",
                        DeskTopImage = "image-son_SortOrder",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0200")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                        ModuleName = "分拣线路调度",
                        ShowOrder = 2,
                        ModuleURL = "/SortOrderDispatch/",
                        IndicateImage = "icon-son_SortOrderc",
                        DeskTopImage = "image-son_SortOrderc",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0200")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                        ModuleName = "分拣作业调度",
                        ShowOrder = 3,
                        ModuleURL = "/SortBatchDispatch/",
                        IndicateImage = "icon-son_SortWork",
                        DeskTopImage = "image-son_SortWork",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0200")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F9D"),
                        ModuleName = "补货计划管理",
                        ShowOrder = 3,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91"),
                        ModuleName = "拆盘位置库存",
                        ShowOrder = 1,
                        ModuleURL = "/SupplyPositionStorage/",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F92"),
                        ModuleName = "自动补货计划",
                        ShowOrder = 2,
                        ModuleURL = "/SupplyTask/",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F9D")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300"),
                        ModuleName = "综合数据查询",
                        ShowOrder = 4,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_StockInto",
                        DeskTopImage = "image-Menu_StockInto",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0301"),
                        ModuleName = "访销订单查询",
                        ShowOrder = 1,
                        ModuleURL = "/SortOrderSearch/",
                        IndicateImage = "icon-son_SortOrder",
                        DeskTopImage = "image-son_SortOrder",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0304"),
                        ModuleName = "分拣订单查询",
                        ShowOrder = 2,
                        ModuleURL = "/SortOrderAllotSearch/",
                        IndicateImage = "icon-Menu_StockInto",
                        DeskTopImage = "image-Menu_StockInto",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0302"),
                        ModuleName = "备货清单查询",
                        ShowOrder = 3,
                        ModuleURL = "/BatchCigaretteSearch/",
                        IndicateImage = "icon-Menu_StockInto",
                        DeskTopImage = "image-Menu_StockInto",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0303"),
                        ModuleName = "分拣烟道查询",
                        ShowOrder = 4,
                        ModuleURL = "/ChannelAllotSearch/",
                        IndicateImage = "icon-Menu_StockDiffer",
                        DeskTopImage = "image-Menu_StockDiffer",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0403"),
                        ModuleName = "分拣补货查询",
                        ShowOrder = 5,
                        ModuleURL = "/SortSupply/",
                        IndicateImage = "icon-Menu_StockDiffer",
                        DeskTopImage = "image-Menu_StockDiffer",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0305"),
                        ModuleName = "分拣状态查询",
                        ShowOrder = 6,
                        ModuleURL = "/DeliverOrderSearch/",
                        IndicateImage = "icon-son_CigaretteSupplier",
                        DeskTopImage = "image-son_CigaretteSupplier",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0300")
                    },
                     new Module()
                    {
                        ModuleID = new Guid("A705073F-3C39-4AED-BB8F-F131C38CBD80"),
                        ModuleName = "设备状态查询",
                        ShowOrder = 5,
                        ModuleURL = "",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("A705073F-3C39-4AED-BB8F-F131C38CBD80")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("F1A78070-8ACA-4B55-96E1-F0CB9EE55B41"),
                        ModuleName = "运行状态查询",
                        ShowOrder = 1,
                        ModuleURL = "/SmsDeviceState/",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("A705073F-3C39-4AED-BB8F-F131C38CBD80")
                    },
                    new Module()
                    {
                        ModuleID = new Guid("9F401A8E-91B3-4FB1-9492-E6375B44D3A5"),
                        ModuleName = "故障状态查询",
                        ShowOrder = 2,
                        ModuleURL = "/SmsDeviceFault/",
                        IndicateImage = "icon-Menu_Organization",
                        DeskTopImage = "image-Menu_Organization",
                        System = system,
                        System_SystemID = system.SystemID,
                        ParentModule_ModuleID = new Guid("A705073F-3C39-4AED-BB8F-F131C38CBD80")
                    }
            );
            context.SaveChanges();

            context.Set<Function>().AddOrUpdate(
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-23E9-8901-C864C062319F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("938024C4-5622-45E9-8232-38BF28AD34B4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC03E-4533-4B19-8433-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E504-D227-32A7-9E04-D77C8D3794AB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB105C2-8A95-23E9-8ED5-C68DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C6-56E5-45E9-45E9-38BFD8AD43B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("235A95C8-C37D-6F47-95D1-6EDEAFE70A7E")
                },
                //拆盘位置信息
                new Function()
                {
                    FunctionID = new Guid("9F75AE64-95F3-4CBA-B35E-104605C88FE7"),
                    FunctionName = "查找",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                new Function()
                {
                    FunctionID = new Guid("62301936-F727-4CE2-BA96-F33AF7ACEC49"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                new Function()
                {
                    FunctionID = new Guid("68C275DE-C0FD-4CB8-8C66-6F9ECA6E6112"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                new Function()
                {
                    FunctionID = new Guid("4A974323-5899-4468-B5E6-00ED07AC51A5"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                new Function()
                {
                    FunctionID = new Guid("3DEF9558-4A1F-416C-9C41-08EC53B1FB49"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                new Function()
                {
                    FunctionID = new Guid("87F8EA05-76F5-4851-A66D-DB2468A33CE4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B100")
                },
                //拆盘位置库存
                new Function()
                {
                    FunctionID = new Guid("439B8426-EEBE-4D14-A4C6-9AF1952527A0"),
                    FunctionName = "查找",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                new Function()
                {
                    FunctionID = new Guid("B2B97874-E057-4F0D-A4CB-B53E0A53BB3C"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                new Function()
                {
                    FunctionID = new Guid("AF97656E-5E63-4577-B64E-A47E290874AA"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                new Function()
                {
                    FunctionID = new Guid("978F41A2-AC87-4859-BEEE-A15DEA69B4CD"),
                    FunctionName = "删除",
                    ControlName = "delete",
                    IndicateImage = "icon-remove",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                new Function()
                {
                    FunctionID = new Guid("EFED53AB-00C8-4C80-BC8D-A34A8A1FDB08"),
                    FunctionName = "打印",
                    ControlName = "print",
                    IndicateImage = "icon-print",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                new Function()
                {
                    FunctionID = new Guid("F53392B8-08C7-446E-A6D0-F9089F7B2C71"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F91")
                },
                //
                new Function()
                 {
                     FunctionID = new Guid("AE688DC3-2D1D-4271-ACB1-CBB553097FAE"),
                     FunctionName = "查询",
                     ControlName = "search",
                     IndicateImage = "icon-search",
                     Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-5622-45E9-8432-38B428AD43B4"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-4533-4B19-8633-186A3D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D227-12A7-9E34-DB7C8D3794BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-23E9-8935-C65DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-A213-45E9-8436-38BFD8A143B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E916C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("EAB141C1-8A97-23E9-89A7-C664C0682E6F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6430-3EA8-4FA8-91A7-6AE712C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9619-4B19-8609-182A3D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6430-3EA8-4FA8-91A7-6AE712C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-23E9-8211-C67DC068519F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6430-3EA8-4FA8-91A7-6AE712C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-56D5-45E9-8A12-38BFD86D43B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6430-3EA8-4FA8-91A7-6AE712C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C1-8A97-23E9-8957-C661C068919F"),
                     FunctionName = "查询",
                     ControlName = "search",
                     IndicateImage = "icon-search",
                     Module_ModuleID = new Guid("235A95C8-C37D-6A27-96F1-6EDEA3370A7E")
                 },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9619-4B19-8A19-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("235A95C8-C37D-6A27-96F1-6EDEA3370A7E")
                },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-23E9-87E1-C67DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("235A95C8-C37D-6A27-96F1-6EDEA3370A7E")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-5A45-45E9-8492-38BFD86D43B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("235A95C8-C37D-6A27-96F1-6EDEA3370A7E")
                },
                new Function()
                {
                    FunctionID = new Guid("DB8B7FB7-E812-43C4-8BD1-8F6D4C4C5D5A"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-4533-4B19-8615-18633D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A91-23E9-8917-C66DC068919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-5645-45E9-823B-38BFD8AD43B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E913C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-23E9-8919-C634C068919F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E914C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-4533-5B19-8621-18643D1FC308"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E914C7B1DC")
                },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A95-23E9-8923-C67DC078919F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E914C7B1DC")
                 },
                new Function()
                {
                    FunctionID = new Guid("9AF054C4-5647-45E9-8424-38BFD8A843B4"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("ED0E6430-95C8-4F58-91A7-23E914C7B1DC")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201")
                },
                new Function()
                {
                    FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                    FunctionName = "下载",
                    ControlName = "download",
                    IndicateImage = "icon-reload",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201")
                },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201")
                },
                 new Function()
                 {
                     FunctionID = new Guid("05432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201")
                 },
                new Function()
                {
                    FunctionID = new Guid("06432F88-08AD-4FDA-8F45-EAD3B2AE0201"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0201")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                },
                new Function()
                {
                    FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                },
                 new Function()
                 {
                     FunctionID = new Guid("04432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("05432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                 },
                new Function()
                {
                    FunctionID = new Guid("06432F88-08AD-4FDA-8F45-EAD3B2AE0202"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0202")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                },
                new Function()
                {
                    FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                },
                 new Function()
                 {
                     FunctionID = new Guid("04432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                 },
                  new Function()
                  {
                      FunctionID = new Guid("05432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                      FunctionName = "订单优化",
                      ControlName = "optimize",
                      IndicateImage = "icon-Menu_CheckBill",
                      Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                  },
                 //new Function()
                 //{
                 //    FunctionID = new Guid("06432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                 //    FunctionName = "清除优化",
                 //    ControlName = "empty",
                 //    IndicateImage = "icon-cancel",
                 //    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                 //},
                 new Function()
                 {
                     FunctionID = new Guid("07432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                     FunctionName = "数据上传",
                     ControlName = "upload",
                     IndicateImage = "icon-apply",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("08432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                 },
                new Function()
                {
                    FunctionID = new Guid("09432F88-08AD-4FDA-8F45-EAD3B2AE0203"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0203")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0301"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0301")
                },
                 new Function()
                 {
                     FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0301"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0301")
                 },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0301"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0301")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0302"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0302")
                },
                 new Function()
                 {
                     FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0302"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0302")
                 },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0302"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0302")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0303"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0303")
                },
                 new Function()
                 {
                     FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0303"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0303")
                 },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0303"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0303")
                },
                //自动补货计划
                new Function()
                {
                    FunctionID = new Guid("73994BC0-7CAC-436C-B904-E3C05EF1D80E"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F92")
                },
                new Function()
                {
                    FunctionID = new Guid("AA4CD3CC-FECA-41F9-8626-F7F337733253"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F92")
                },
                 new Function()
                 {
                     FunctionID = new Guid("54F07A34-8910-45E0-9E3C-B444884AA399"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F92")
                 },
                new Function()
                {
                    FunctionID = new Guid("4081DBD1-4E93-4172-88D9-DB87A2EE1C99"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("71D36437-AD8A-4D50-8DB9-000064301F92")
                },
                //分拣补货查询
                new Function()
                {
                    FunctionID = new Guid("3E3A0F72-7D5E-4005-A865-D5FA135408FB"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0403")
                },
                 new Function()
                 {
                     FunctionID = new Guid("827EAB75-7FA9-47B1-B19B-1A66A551FA7A"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0403")
                 },
                new Function()
                {
                    FunctionID = new Guid("4081DBD1-4E93-4172-88D9-DB87A2EE1CB0"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0403")
                },

                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0304"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0304")
                },
                 new Function()
                 {
                     FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0304"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0304")
                 },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0304"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0304")
                },
                new Function()
                {
                    FunctionID = new Guid("01432F88-08AD-4FDA-8F45-EAD3B2AE0305"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0305")
                },
                 new Function()
                 {
                     FunctionID = new Guid("02432F88-08AD-4FDA-8F45-EAD3B2AE0305"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0305")
                 },
                new Function()
                {
                    FunctionID = new Guid("03432F88-08AD-4FDA-8F45-EAD3B2AE0305"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("D8432F88-08AD-4FDA-8F45-EAD3B2AE0305")
                },
                //运行状态查询
                new Function()
                {
                    FunctionID = new Guid("E756F081-CA3C-480F-BC7B-09EFD55FDE0E"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("F1A78070-8ACA-4B55-96E1-F0CB9EE55B41")
                },
                 new Function()
                 {
                     FunctionID = new Guid("6E028A8E-92AF-4701-9484-778ECB10B8D9"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("F1A78070-8ACA-4B55-96E1-F0CB9EE55B41")
                 },
                new Function()
                {
                    FunctionID = new Guid("3A181837-B6AD-480A-B1BE-CD1C9C4E0DDF"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("F1A78070-8ACA-4B55-96E1-F0CB9EE55B41")
                },
                //故障状态查询
                new Function()
                {
                    FunctionID = new Guid("5D35936D-71F3-4877-ADBB-A6D40B37BDFB"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("9F401A8E-91B3-4FB1-9492-E6375B44D3A5")
                },
                 new Function()
                 {
                     FunctionID = new Guid("1ED6068B-96CD-4BF7-ACE0-790E70579412"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("9F401A8E-91B3-4FB1-9492-E6375B44D3A5")
                 },
                new Function()
                {
                    FunctionID = new Guid("F322873B-994A-44E6-A79C-82C1AFF583AB"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("9F401A8E-91B3-4FB1-9492-E6375B44D3A5")
                },
                //报警信息
                new Function()
                {
                    FunctionID = new Guid("EAB101C1-8A93-4E4B-8943-C164C068911F"),
                    FunctionName = "查询",
                    ControlName = "search",
                    IndicateImage = "icon-search",
                    Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5622-43A1-8444-28BF22ADEFB2"),
                    FunctionName = "新增",
                    ControlName = "add",
                    IndicateImage = "icon-add",
                    Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                },
                new Function()
                {
                    FunctionID = new Guid("A6EAC93E-9618-4B19-8645-13645D1FC303"),
                    FunctionName = "编辑",
                    ControlName = "edit",
                    IndicateImage = "icon-edit",
                    Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                },
                 new Function()
                 {
                     FunctionID = new Guid("C8A9E599-D257-4058-9E46-D47C8D1744BB"),
                     FunctionName = "删除",
                     ControlName = "delete",
                     IndicateImage = "icon-remove",
                     Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                 },
                 new Function()
                 {
                     FunctionID = new Guid("EAB101C2-8A93-4E4B-8947-C562C06D959F"),
                     FunctionName = "打印",
                     ControlName = "print",
                     IndicateImage = "icon-print",
                     Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                 },
                new Function()
                {
                    FunctionID = new Guid("938034C4-5645-43A1-8448-36BF28DDEF64"),
                    FunctionName = "帮助",
                    ControlName = "help",
                    IndicateImage = "icon-help",
                    Module_ModuleID = new Guid("72D21D89-39C5-4CC1-8778-2193F5EFCC2C")
                }
            );
           context.SaveChanges();
        }
    }
}
