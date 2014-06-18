namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_ef_to_6_1_1_beta_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.auth_city",
                c => new
                    {
                        city_id = c.Guid(nullable: false),
                        city_name = c.String(nullable: false, maxLength: 50),
                        description = c.String(),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.city_id);
            
            CreateTable(
                "dbo.auth_role_system",
                c => new
                    {
                        role_system_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        role_role_id = c.Guid(nullable: false),
                        city_city_id = c.Guid(nullable: false),
                        system_system_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.role_system_id)
                .ForeignKey("dbo.auth_city", t => t.city_city_id)
                .ForeignKey("dbo.auth_role", t => t.role_role_id)
                .ForeignKey("dbo.auth_system", t => t.system_system_id)
                .Index(t => t.role_role_id)
                .Index(t => t.city_city_id)
                .Index(t => t.system_system_id);
            
            CreateTable(
                "dbo.auth_role",
                c => new
                    {
                        role_id = c.Guid(nullable: false),
                        role_name = c.String(nullable: false, maxLength: 50),
                        is_lock = c.Boolean(nullable: false),
                        memo = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.role_id);
            
            CreateTable(
                "dbo.auth_user_role",
                c => new
                    {
                        user_role_id = c.Guid(nullable: false),
                        role_role_id = c.Guid(nullable: false),
                        user_user_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.user_role_id)
                .ForeignKey("dbo.auth_role", t => t.role_role_id)
                .ForeignKey("dbo.auth_user", t => t.user_user_id)
                .Index(t => t.role_role_id)
                .Index(t => t.user_user_id);
            
            CreateTable(
                "dbo.auth_user",
                c => new
                    {
                        user_id = c.Guid(nullable: false),
                        user_name = c.String(nullable: false, maxLength: 50),
                        pwd = c.String(nullable: false, maxLength: 50),
                        chinese_name = c.String(maxLength: 50),
                        is_lock = c.Boolean(nullable: false),
                        is_admin = c.Boolean(nullable: false),
                        login_pc = c.String(maxLength: 50),
                        memo = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.user_id);
            
            CreateTable(
                "dbo.auth_login_log",
                c => new
                    {
                        log_id = c.Guid(nullable: false),
                        login_pc = c.String(nullable: false, maxLength: 50),
                        login_time = c.String(nullable: false, maxLength: 30),
                        logout_time = c.String(maxLength: 30),
                        user_user_id = c.Guid(nullable: false),
                        system_system_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.log_id)
                .ForeignKey("dbo.auth_system", t => t.system_system_id)
                .ForeignKey("dbo.auth_user", t => t.user_user_id)
                .Index(t => t.user_user_id)
                .Index(t => t.system_system_id);
            
            CreateTable(
                "dbo.auth_system",
                c => new
                    {
                        system_id = c.Guid(nullable: false),
                        system_name = c.String(nullable: false, maxLength: 100),
                        description = c.String(),
                        status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.system_id);
            
            CreateTable(
                "dbo.auth_module",
                c => new
                    {
                        module_id = c.Guid(nullable: false),
                        module_name = c.String(nullable: false, maxLength: 20),
                        show_order = c.Int(nullable: false),
                        module_url = c.String(nullable: false, maxLength: 100),
                        indicate_image = c.String(maxLength: 100),
                        desk_top_image = c.String(maxLength: 100),
                        system_system_id = c.Guid(nullable: false),
                        parent_module_module_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.module_id)
                .ForeignKey("dbo.auth_module", t => t.parent_module_module_id)
                .ForeignKey("dbo.auth_system", t => t.system_system_id)
                .Index(t => t.system_system_id)
                .Index(t => t.parent_module_module_id);
            
            CreateTable(
                "dbo.auth_function",
                c => new
                    {
                        function_id = c.Guid(nullable: false),
                        function_name = c.String(nullable: false, maxLength: 50),
                        control_name = c.String(nullable: false, maxLength: 50),
                        indicate_image = c.String(maxLength: 100),
                        module_module_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.function_id)
                .ForeignKey("dbo.auth_module", t => t.module_module_id)
                .Index(t => t.module_module_id);
            
            CreateTable(
                "dbo.auth_role_function",
                c => new
                    {
                        role_function_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        role_module_role_module_id = c.Guid(nullable: false),
                        function_function_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.role_function_id)
                .ForeignKey("dbo.auth_function", t => t.function_function_id)
                .ForeignKey("dbo.auth_role_module", t => t.role_module_role_module_id)
                .Index(t => t.role_module_role_module_id)
                .Index(t => t.function_function_id);
            
            CreateTable(
                "dbo.auth_role_module",
                c => new
                    {
                        role_module_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        role_system_role_system_id = c.Guid(nullable: false),
                        module_module_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.role_module_id)
                .ForeignKey("dbo.auth_module", t => t.module_module_id)
                .ForeignKey("dbo.auth_role_system", t => t.role_system_role_system_id)
                .Index(t => t.role_system_role_system_id)
                .Index(t => t.module_module_id);
            
            CreateTable(
                "dbo.auth_user_function",
                c => new
                    {
                        user_function_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        user_module_user_module_id = c.Guid(nullable: false),
                        function_function_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.user_function_id)
                .ForeignKey("dbo.auth_function", t => t.function_function_id)
                .ForeignKey("dbo.auth_user_module", t => t.user_module_user_module_id)
                .Index(t => t.user_module_user_module_id)
                .Index(t => t.function_function_id);
            
            CreateTable(
                "dbo.auth_user_module",
                c => new
                    {
                        user_module_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        user_system_user_system_id = c.Guid(nullable: false),
                        module_module_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.user_module_id)
                .ForeignKey("dbo.auth_module", t => t.module_module_id)
                .ForeignKey("dbo.auth_user_system", t => t.user_system_user_system_id)
                .Index(t => t.user_system_user_system_id)
                .Index(t => t.module_module_id);
            
            CreateTable(
                "dbo.auth_user_system",
                c => new
                    {
                        user_system_id = c.Guid(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        user_user_id = c.Guid(nullable: false),
                        city_city_id = c.Guid(nullable: false),
                        system_system_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.user_system_id)
                .ForeignKey("dbo.auth_city", t => t.city_city_id)
                .ForeignKey("dbo.auth_system", t => t.system_system_id)
                .ForeignKey("dbo.auth_user", t => t.user_user_id)
                .Index(t => t.user_user_id)
                .Index(t => t.city_city_id)
                .Index(t => t.system_system_id);
            
            CreateTable(
                "dbo.auth_system_parameter",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        parameter_name = c.String(nullable: false, maxLength: 50),
                        parameter_value = c.String(nullable: false, maxLength: 50),
                        remark = c.String(nullable: false, maxLength: 50),
                        user_name = c.String(nullable: false, maxLength: 20),
                        system_id = c.Guid(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.auth_system", t => t.system_id)
                .Index(t => t.system_id);
            
            CreateTable(
                "dbo.auth_server",
                c => new
                    {
                        server_id = c.Guid(nullable: false),
                        server_name = c.String(nullable: false, maxLength: 50),
                        description = c.String(),
                        url = c.String(),
                        is_active = c.Boolean(nullable: false),
                        city_city_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.server_id)
                .ForeignKey("dbo.auth_city", t => t.city_city_id)
                .Index(t => t.city_city_id);
            
            CreateTable(
                "dbo.auth_system_event_log",
                c => new
                    {
                        event_log_id = c.Guid(nullable: false),
                        event_log_time = c.String(nullable: false, maxLength: 30),
                        event_type = c.String(nullable: false),
                        event_name = c.String(nullable: false),
                        event_description = c.String(nullable: false),
                        from_pc = c.String(nullable: false),
                        operate_user = c.String(nullable: false),
                        target_system = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.event_log_id);
            
            CreateTable(
                "dbo.auth_help_content",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        content_code = c.String(nullable: false, maxLength: 20),
                        content_name = c.String(nullable: false, maxLength: 50),
                        content_text = c.String(),
                        content_path = c.String(nullable: false, maxLength: 100),
                        node_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        father_node_id = c.Guid(nullable: false),
                        module_id = c.Guid(nullable: false),
                        node_order = c.Int(nullable: false),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.auth_help_content", t => t.father_node_id)
                .ForeignKey("dbo.auth_module", t => t.module_id)
                .Index(t => t.father_node_id)
                .Index(t => t.module_id);
            
            CreateTable(
                "dbo.auth_exceptional_log",
                c => new
                    {
                        exceptional_log_id = c.Guid(nullable: false),
                        catch_time = c.String(nullable: false, maxLength: 30),
                        module_name = c.String(nullable: false),
                        function_name = c.String(nullable: false),
                        exceptional_type = c.String(nullable: false),
                        exceptional_description = c.String(nullable: false),
                        state = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.exceptional_log_id);
            
            CreateTable(
                "dbo.wms_company",
                c => new
                    {
                        company_id = c.Guid(nullable: false),
                        company_code = c.String(nullable: false, maxLength: 20),
                        company_name = c.String(nullable: false, maxLength: 100),
                        company_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        description = c.String(),
                        parent_company_id = c.Guid(nullable: false),
                        uniform_code = c.String(maxLength: 20),
                        warehouse_space = c.Decimal(nullable: false, precision: 18, scale: 2),
                        warehouse_count = c.Decimal(nullable: false, precision: 18, scale: 2),
                        warehouse_capacity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        sorting_count = c.Decimal(nullable: false, precision: 18, scale: 2),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.company_id)
                .ForeignKey("dbo.wms_company", t => t.parent_company_id)
                .Index(t => t.parent_company_id);
            
            CreateTable(
                "dbo.wms_department",
                c => new
                    {
                        department_id = c.Guid(nullable: false),
                        department_code = c.String(nullable: false, maxLength: 20),
                        department_name = c.String(nullable: false, maxLength: 100),
                        department_leader_id = c.Guid(),
                        description = c.String(),
                        company_id = c.Guid(nullable: false),
                        parent_department_id = c.Guid(nullable: false),
                        uniform_code = c.String(maxLength: 20),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.department_id)
                .ForeignKey("dbo.wms_company", t => t.company_id)
                .ForeignKey("dbo.wms_employee", t => t.department_leader_id)
                .ForeignKey("dbo.wms_department", t => t.parent_department_id)
                .Index(t => t.department_leader_id)
                .Index(t => t.company_id)
                .Index(t => t.parent_department_id);
            
            CreateTable(
                "dbo.wms_employee",
                c => new
                    {
                        employee_id = c.Guid(nullable: false),
                        employee_code = c.String(nullable: false, maxLength: 20),
                        employee_name = c.String(nullable: false, maxLength: 100),
                        user_name = c.String(maxLength: 50),
                        description = c.String(),
                        department_id = c.Guid(),
                        job_id = c.Guid(nullable: false),
                        sex = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        tel = c.String(maxLength: 50),
                        Status = c.String(nullable: false, maxLength: 4, fixedLength: true),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.employee_id)
                .ForeignKey("dbo.wms_department", t => t.department_id)
                .ForeignKey("dbo.wms_job", t => t.job_id)
                .Index(t => t.department_id)
                .Index(t => t.job_id);
            
            CreateTable(
                "dbo.wms_check_bill_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_product_code = c.String(nullable: false, maxLength: 20),
                        real_unit_code = c.String(nullable: false, maxLength: 20),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_cell", t => t.cell_code)
                .ForeignKey("dbo.wms_check_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_product", t => t.real_product_code)
                .ForeignKey("dbo.wms_unit", t => t.real_unit_code)
                .ForeignKey("dbo.wms_storage", t => t.storage_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.cell_code)
                .Index(t => t.storage_code)
                .Index(t => t.product_code)
                .Index(t => t.unit_code)
                .Index(t => t.real_product_code)
                .Index(t => t.real_unit_code)
                .Index(t => t.operate_person_id);
            
            CreateTable(
                "dbo.wms_cell",
                c => new
                    {
                        cell_code = c.String(nullable: false, maxLength: 20),
                        cell_name = c.String(nullable: false, maxLength: 20),
                        short_name = c.String(nullable: false, maxLength: 10),
                        cell_type = c.String(nullable: false, maxLength: 4),
                        layer = c.Int(nullable: false),
                        col = c.Int(nullable: false),
                        img_x = c.Int(nullable: false),
                        img_y = c.Int(nullable: false),
                        rfid = c.String(maxLength: 100),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        area_code = c.String(nullable: false, maxLength: 20),
                        shelf_code = c.String(nullable: false, maxLength: 20),
                        default_product_code = c.String(maxLength: 20),
                        max_quantity = c.Int(nullable: false),
                        is_single = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        max_pallet_quantity = c.Int(nullable: false),
                        description = c.String(maxLength: 100),
                        lock_tag = c.String(),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        IsMultiBrand = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        first_in_first_out = c.Boolean(nullable: false),
                        storage_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.cell_code)
                .ForeignKey("dbo.wms_area", t => t.area_code)
                .ForeignKey("dbo.wms_product", t => t.default_product_code)
                .ForeignKey("dbo.wms_shelf", t => t.shelf_code)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.area_code)
                .Index(t => t.shelf_code)
                .Index(t => t.default_product_code);
            
            CreateTable(
                "dbo.wms_area",
                c => new
                    {
                        area_code = c.String(nullable: false, maxLength: 20),
                        area_name = c.String(nullable: false, maxLength: 20),
                        short_name = c.String(nullable: false, maxLength: 10),
                        area_type = c.String(nullable: false, maxLength: 2),
                        allot_in_order = c.Int(nullable: false),
                        allot_out_order = c.Int(nullable: false),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.area_code)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.warehouse_code);
            
            CreateTable(
                "dbo.wms_shelf",
                c => new
                    {
                        shelf_code = c.String(nullable: false, maxLength: 20),
                        shelf_name = c.String(nullable: false, maxLength: 20),
                        short_name = c.String(nullable: false, maxLength: 10),
                        shelf_type = c.String(nullable: false, maxLength: 2),
                        cell_rows = c.Int(nullable: false),
                        cell_cols = c.Int(nullable: false),
                        img_x = c.Int(nullable: false),
                        img_y = c.Int(nullable: false),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        area_code = c.String(nullable: false, maxLength: 20),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.shelf_code)
                .ForeignKey("dbo.wms_area", t => t.area_code)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.area_code);
            
            CreateTable(
                "dbo.wms_warehouse",
                c => new
                    {
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        warehouse_name = c.String(nullable: false, maxLength: 20),
                        short_name = c.String(nullable: false, maxLength: 10),
                        warehouse_type = c.String(maxLength: 1),
                        company_code = c.String(maxLength: 20),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.warehouse_code);
            
            CreateTable(
                "dbo.wms_check_bill_master",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.bill_no)
                .ForeignKey("dbo.wms_bill_type", t => t.bill_type_code)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_employee", t => t.verify_person_id)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.bill_type_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.operate_person_id)
                .Index(t => t.verify_person_id);
            
            CreateTable(
                "dbo.wms_bill_type",
                c => new
                    {
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        bill_type_name = c.String(nullable: false, maxLength: 20),
                        bill_class = c.String(nullable: false, maxLength: 4, fixedLength: true),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.bill_type_code);
            
            CreateTable(
                "dbo.wms_in_bill_master",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        lock_tag = c.String(maxLength: 50),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        target_cell_code = c.String(),
                    })
                .PrimaryKey(t => t.bill_no)
                .ForeignKey("dbo.wms_bill_type", t => t.bill_type_code)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_employee", t => t.verify_person_id)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.bill_type_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.operate_person_id)
                .Index(t => t.verify_person_id);
            
            CreateTable(
                "dbo.wms_in_bill_allot",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        in_bill_detail_id = c.Int(nullable: false),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_cell", t => t.cell_code)
                .ForeignKey("dbo.wms_in_bill_detail", t => t.in_bill_detail_id)
                .ForeignKey("dbo.wms_in_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_storage", t => t.storage_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.product_code)
                .Index(t => t.in_bill_detail_id)
                .Index(t => t.cell_code)
                .Index(t => t.storage_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_in_bill_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bill_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_in_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_product", t => t.product_code, cascadeDelete: true)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_product",
                c => new
                    {
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        uniform_code = c.String(nullable: false, maxLength: 20),
                        custom_code = c.String(maxLength: 30),
                        short_code = c.String(maxLength: 10),
                        unit_list_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        supplier_code = c.String(nullable: false, maxLength: 20),
                        brand_code = c.String(nullable: false, maxLength: 20),
                        abc_type_code = c.String(maxLength: 1, fixedLength: true),
                        product_type_code = c.String(maxLength: 4, fixedLength: true),
                        pack_type_code = c.String(maxLength: 4, fixedLength: true),
                        price_level_code = c.String(maxLength: 4),
                        statistic_type = c.String(maxLength: 10),
                        piece_barcode = c.String(maxLength: 13),
                        bar_barcode = c.String(maxLength: 13),
                        package_barcode = c.String(maxLength: 13),
                        one_project_barcode = c.String(maxLength: 30),
                        buy_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        trade_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        retail_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cost_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        is_filter_tip = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_new = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_famous = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_main_product = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_province_main_product = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        belong_region = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_confiscate = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_abnormity = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        is_rounding = c.String(maxLength: 1, fixedLength: true),
                        cell_max_product_quantity = c.Int(nullable: false),
                        point_area_codes = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.product_code)
                .ForeignKey("dbo.wms_brand", t => t.brand_code)
                .ForeignKey("dbo.wms_supplier", t => t.supplier_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .ForeignKey("dbo.wms_unit_list", t => t.unit_list_code)
                .Index(t => t.unit_list_code)
                .Index(t => t.unit_code)
                .Index(t => t.supplier_code)
                .Index(t => t.brand_code);
            
            CreateTable(
                "dbo.wms_brand",
                c => new
                    {
                        brand_code = c.String(nullable: false, maxLength: 20),
                        uniform_code = c.String(maxLength: 20),
                        custom_code = c.String(maxLength: 20),
                        brand_name = c.String(nullable: false, maxLength: 50),
                        supplier_code = c.String(nullable: false, maxLength: 20),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.brand_code)
                .ForeignKey("dbo.wms_supplier", t => t.supplier_code)
                .Index(t => t.supplier_code);
            
            CreateTable(
                "dbo.wms_supplier",
                c => new
                    {
                        supplier_code = c.String(nullable: false, maxLength: 20),
                        uniform_code = c.String(maxLength: 20),
                        custom_code = c.String(maxLength: 20),
                        supplier_name = c.String(nullable: false, maxLength: 50),
                        province_name = c.String(maxLength: 20),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.supplier_code);
            
            CreateTable(
                "dbo.wms_daily_balance",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        settle_date = c.DateTime(nullable: false),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        AreaCode = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        beginning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        entry_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        delivery_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        profit_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        loss_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ending = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_area", t => t.AreaCode)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.AreaCode)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_unit",
                c => new
                    {
                        unit_code = c.String(nullable: false, maxLength: 20),
                        unit_name = c.String(nullable: false, maxLength: 20),
                        count = c.Int(nullable: false),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_move_bill_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        pallet_tag = c.Int(),
                        product_code = c.String(nullable: false, maxLength: 20),
                        out_cell_code = c.String(nullable: false, maxLength: 20),
                        out_storage_code = c.String(nullable: false, maxLength: 50),
                        in_cell_code = c.String(nullable: false, maxLength: 20),
                        in_storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        can_real_operate = c.String(maxLength: 1),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_cell", t => t.in_cell_code)
                .ForeignKey("dbo.wms_storage", t => t.in_storage_code)
                .ForeignKey("dbo.wms_move_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_cell", t => t.out_cell_code)
                .ForeignKey("dbo.wms_storage", t => t.out_storage_code)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.product_code)
                .Index(t => t.out_cell_code)
                .Index(t => t.out_storage_code)
                .Index(t => t.in_cell_code)
                .Index(t => t.in_storage_code)
                .Index(t => t.unit_code)
                .Index(t => t.operate_person_id);
            
            CreateTable(
                "dbo.wms_storage",
                c => new
                    {
                        storage_code = c.String(nullable: false, maxLength: 50),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        storage_time = c.DateTime(nullable: false),
                        rfid = c.String(maxLength: 100),
                        in_frozen_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        out_frozen_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        is_lock = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        lock_tag = c.String(maxLength: 50),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        storage_sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.storage_code)
                .ForeignKey("dbo.wms_cell", t => t.cell_code)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .Index(t => t.cell_code)
                .Index(t => t.product_code);
            
            CreateTable(
                "dbo.wms_out_bill_allot",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        pallet_tag = c.Int(),
                        product_code = c.String(nullable: false, maxLength: 20),
                        out_bill_detail_id = c.Int(nullable: false),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        can_real_operate = c.String(maxLength: 1),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_cell", t => t.cell_code)
                .ForeignKey("dbo.wms_out_bill_detail", t => t.out_bill_detail_id)
                .ForeignKey("dbo.wms_out_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_storage", t => t.storage_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.product_code)
                .Index(t => t.out_bill_detail_id)
                .Index(t => t.cell_code)
                .Index(t => t.storage_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_out_bill_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bill_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_out_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_product", t => t.product_code, cascadeDelete: true)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_out_bill_master",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        origin = c.String(nullable: false, maxLength: 1),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        move_bill_master_bill_no = c.String(maxLength: 20),
                        lock_tag = c.String(maxLength: 50),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        target_cell_code = c.String(),
                    })
                .PrimaryKey(t => t.bill_no)
                .ForeignKey("dbo.wms_bill_type", t => t.bill_type_code)
                .ForeignKey("dbo.wms_move_bill_master", t => t.move_bill_master_bill_no)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_employee", t => t.verify_person_id)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.bill_type_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.operate_person_id)
                .Index(t => t.verify_person_id)
                .Index(t => t.move_bill_master_bill_no);
            
            CreateTable(
                "dbo.wms_move_bill_master",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        origin = c.String(nullable: false, maxLength: 1),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        lock_tag = c.String(maxLength: 50),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.bill_no)
                .ForeignKey("dbo.wms_bill_type", t => t.bill_type_code)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_employee", t => t.verify_person_id)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.bill_type_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.operate_person_id)
                .Index(t => t.verify_person_id);
            
            CreateTable(
                "dbo.wms_profit_loss_bill_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_profit_loss_bill_master", t => t.bill_no)
                .ForeignKey("dbo.wms_storage", t => t.storage_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.bill_no)
                .Index(t => t.storage_code)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_profit_loss_bill_master",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        check_bill_no = c.String(maxLength: 20),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        lock_tag = c.String(maxLength: 50),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.bill_no)
                .ForeignKey("dbo.wms_bill_type", t => t.bill_type_code)
                .ForeignKey("dbo.wms_employee", t => t.operate_person_id)
                .ForeignKey("dbo.wms_employee", t => t.verify_person_id)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.bill_type_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.operate_person_id)
                .Index(t => t.verify_person_id);
            
            CreateTable(
                "dbo.wms_sorting_lowerlimit",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        sort_order = c.Int(),
                        sort_type = c.String(),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_sorting_line", t => t.sorting_line_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.sorting_line_code)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_sorting_line",
                c => new
                    {
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        sorting_line_name = c.String(nullable: false, maxLength: 100),
                        sorting_line_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        out_bill_type_code = c.String(nullable: false, maxLength: 4),
                        move_bill_type_code = c.String(nullable: false, maxLength: 4),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sorting_line_code)
                .ForeignKey("dbo.wms_cell", t => t.cell_code)
                .Index(t => t.cell_code);
            
            CreateTable(
                "dbo.wms_sort_order_dispatch",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        order_date = c.String(nullable: false, maxLength: 14),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        sort_work_dispatch_id = c.Guid(),
                        work_status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_deliver_line", t => t.deliver_line_code)
                .ForeignKey("dbo.wms_sorting_line", t => t.sorting_line_code)
                .ForeignKey("dbo.wms_sort_work_dispatch", t => t.sort_work_dispatch_id)
                .Index(t => t.sorting_line_code)
                .Index(t => t.deliver_line_code)
                .Index(t => t.sort_work_dispatch_id);
            
            CreateTable(
                "dbo.wms_deliver_line",
                c => new
                    {
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        custom_code = c.String(maxLength: 50),
                        deliver_line_name = c.String(nullable: false, maxLength: 100),
                        dist_code = c.String(maxLength: 50),
                        deliver_order = c.Int(nullable: false),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        new_deliver_line_code = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.deliver_line_code);
            
            CreateTable(
                "dbo.wms_sort_order",
                c => new
                    {
                        order_id = c.String(nullable: false, maxLength: 12),
                        company_code = c.String(maxLength: 20),
                        sale_region_code = c.String(nullable: false, maxLength: 50),
                        order_date = c.String(nullable: false, maxLength: 14),
                        order_type = c.String(nullable: false, maxLength: 1),
                        customer_code = c.String(nullable: false, maxLength: 50),
                        customer_name = c.String(nullable: false, maxLength: 100),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        quantity_sum = c.Decimal(nullable: false, precision: 18, scale: 4),
                        amount_sum = c.Decimal(nullable: false, precision: 18, scale: 4),
                        detail_num = c.Decimal(nullable: false, precision: 18, scale: 2),
                        deliver_order = c.Int(nullable: false),
                        DeliverDate = c.String(nullable: false, maxLength: 14, fixedLength: true),
                        description = c.String(nullable: false, maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.order_id)
                .ForeignKey("dbo.wms_deliver_line", t => t.deliver_line_code)
                .Index(t => t.deliver_line_code);
            
            CreateTable(
                "dbo.wms_sort_order_detail",
                c => new
                    {
                        order_detail_id = c.String(nullable: false, maxLength: 20),
                        order_id = c.String(nullable: false, maxLength: 12),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 40, fixedLength: true),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        unit_name = c.String(nullable: false, maxLength: 20),
                        demand_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        unit_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.order_detail_id)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_sort_order", t => t.order_id)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.order_id)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_sort_work_dispatch",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        order_date = c.String(nullable: false, maxLength: 14),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        dispatch_batch = c.String(nullable: false, maxLength: 2),
                        out_bill_no = c.String(nullable: false, maxLength: 20),
                        move_bill_no = c.String(nullable: false, maxLength: 20),
                        dispatch_status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_move_bill_master", t => t.move_bill_no)
                .ForeignKey("dbo.wms_out_bill_master", t => t.out_bill_no)
                .ForeignKey("dbo.wms_sorting_line", t => t.sorting_line_code)
                .Index(t => t.sorting_line_code)
                .Index(t => t.out_bill_no)
                .Index(t => t.move_bill_no);
            
            CreateTable(
                "dbo.wms_unit_list",
                c => new
                    {
                        unit_list_code = c.String(nullable: false, maxLength: 20),
                        uniform_code = c.String(maxLength: 20),
                        unit_list_name = c.String(nullable: false, maxLength: 50),
                        unit_code01 = c.String(nullable: false, maxLength: 20),
                        quantity01 = c.Decimal(nullable: false, precision: 18, scale: 4),
                        unit_code02 = c.String(nullable: false, maxLength: 20),
                        quantity02 = c.Decimal(nullable: false, precision: 18, scale: 4),
                        unit_code03 = c.String(nullable: false, maxLength: 20),
                        quantity03 = c.Decimal(nullable: false, precision: 18, scale: 4),
                        unit_code04 = c.String(nullable: false, maxLength: 20),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.unit_list_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code01)
                .ForeignKey("dbo.wms_unit", t => t.unit_code02)
                .ForeignKey("dbo.wms_unit", t => t.unit_code03)
                .ForeignKey("dbo.wms_unit", t => t.unit_code04)
                .Index(t => t.unit_code01)
                .Index(t => t.unit_code02)
                .Index(t => t.unit_code03)
                .Index(t => t.unit_code04);
            
            CreateTable(
                "dbo.wms_job",
                c => new
                    {
                        job_id = c.Guid(nullable: false),
                        job_code = c.String(nullable: false, maxLength: 20),
                        job_name = c.String(nullable: false, maxLength: 20),
                        description = c.String(),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.job_id);
            
            CreateTable(
                "dbo.wms_deliver_dist",
                c => new
                    {
                        dist_code = c.String(nullable: false, maxLength: 50),
                        custom_code = c.String(maxLength: 50),
                        dist_name = c.String(nullable: false, maxLength: 100),
                        dist_center_code = c.String(maxLength: 20),
                        company_code = c.String(maxLength: 20),
                        uniform_code = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.dist_code);
            
            CreateTable(
                "dbo.wms_customer",
                c => new
                    {
                        customer_code = c.String(nullable: false, maxLength: 50),
                        custom_code = c.String(maxLength: 50),
                        customer_name = c.String(nullable: false, maxLength: 100),
                        company_code = c.String(maxLength: 20),
                        sale_region_code = c.String(nullable: false, maxLength: 50),
                        uniform_code = c.String(nullable: false, maxLength: 50),
                        customer_type = c.String(nullable: false, maxLength: 4, fixedLength: true),
                        sale_scope = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        industry_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        city_or_countryside = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        deliver_order = c.Int(nullable: false),
                        address = c.String(nullable: false, maxLength: 100),
                        phone = c.String(nullable: false, maxLength: 20),
                        license_type = c.String(maxLength: 10),
                        license_code = c.String(maxLength: 20),
                        principal_name = c.String(maxLength: 20),
                        principal_phone = c.String(maxLength: 60),
                        principal_address = c.String(maxLength: 100),
                        management_name = c.String(maxLength: 20),
                        management_phone = c.String(maxLength: 60),
                        bank = c.String(maxLength: 50),
                        bank_accounts = c.String(maxLength: 50),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.customer_code);
            
            CreateTable(
                "dbo.wms_business_systems_daily_balance",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        settle_date = c.DateTime(nullable: false),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        beginning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        entry_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        delivery_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        profit_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        loss_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ending = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_product", t => t.product_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .ForeignKey("dbo.wms_warehouse", t => t.warehouse_code)
                .Index(t => t.warehouse_code)
                .Index(t => t.product_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_product_warning",
                c => new
                    {
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        min_limited = c.Decimal(precision: 18, scale: 4),
                        max_limited = c.Decimal(precision: 18, scale: 4),
                        assembly_time = c.Decimal(precision: 18, scale: 4),
                        memo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.product_code)
                .ForeignKey("dbo.wms_unit", t => t.unit_code)
                .Index(t => t.unit_code);
            
            CreateTable(
                "dbo.wms_tray_info",
                c => new
                    {
                        TaryID = c.Guid(nullable: false),
                        TaryRfid = c.String(nullable: false, maxLength: 100),
                        ProductCode = c.String(nullable: false, maxLength: 20),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.TaryID);
            
            CreateTable(
                "dbo.wms_in_bill_master_history",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        lock_tag = c.String(maxLength: 50),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        target_cell_code = c.String(),
                    })
                .PrimaryKey(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_in_bill_allot_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        in_bill_detail_id = c.Int(nullable: false),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_in_bill_detail_history", t => t.in_bill_detail_id)
                .ForeignKey("dbo.wms_in_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no)
                .Index(t => t.in_bill_detail_id);
            
            CreateTable(
                "dbo.wms_in_bill_detail_history",
                c => new
                    {
                        id = c.Int(nullable: false),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bill_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_in_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_out_bill_master_history",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        origin = c.String(nullable: false, maxLength: 1),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        move_bill_master_bill_no = c.String(),
                        lock_tag = c.String(maxLength: 50),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        target_cell_code = c.String(),
                    })
                .PrimaryKey(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_out_bill_allot_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        pallet_tag = c.Int(),
                        product_code = c.String(nullable: false, maxLength: 20),
                        out_bill_detail_id = c.Int(nullable: false),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        can_real_operate = c.String(maxLength: 1),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_out_bill_detail_history", t => t.out_bill_detail_id)
                .ForeignKey("dbo.wms_out_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no)
                .Index(t => t.out_bill_detail_id);
            
            CreateTable(
                "dbo.wms_out_bill_detail_history",
                c => new
                    {
                        id = c.Int(nullable: false),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        bill_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        allot_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_out_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_move_bill_master_history",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        origin = c.String(nullable: false, maxLength: 1),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        lock_tag = c.String(maxLength: 50),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_move_bill_detail_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        pallet_tag = c.Int(),
                        product_code = c.String(nullable: false, maxLength: 20),
                        out_cell_code = c.String(nullable: false, maxLength: 20),
                        out_storage_code = c.String(nullable: false, maxLength: 50),
                        in_cell_code = c.String(nullable: false, maxLength: 20),
                        in_storage_code = c.String(nullable: false, maxLength: 50),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        can_real_operate = c.String(maxLength: 1),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_move_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_check_bill_master_history",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_check_bill_detail_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        real_product_code = c.String(nullable: false, maxLength: 20),
                        real_unit_code = c.String(nullable: false, maxLength: 20),
                        real_quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        operate_person_id = c.Guid(),
                        _operator = c.String(name: "operator", maxLength: 20),
                        start_time = c.DateTime(),
                        finish_time = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_check_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_profit_loss_bill_master_history",
                c => new
                    {
                        bill_no = c.String(nullable: false, maxLength: 20),
                        bill_date = c.DateTime(nullable: false),
                        bill_type_code = c.String(nullable: false, maxLength: 4),
                        check_bill_no = c.String(maxLength: 20),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        operate_person_id = c.Guid(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        verify_person_id = c.Guid(),
                        verify_date = c.DateTime(),
                        description = c.String(maxLength: 100),
                        is_active = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        update_time = c.DateTime(nullable: false),
                        lock_tag = c.String(maxLength: 50),
                        row_version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_profit_loss_bill_detail_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bill_no = c.String(nullable: false, maxLength: 20),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        storage_code = c.String(nullable: false, maxLength: 50),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wms_profit_loss_bill_master_history", t => t.bill_no)
                .Index(t => t.bill_no);
            
            CreateTable(
                "dbo.wms_daily_balance_history",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        settle_date = c.DateTime(nullable: false),
                        warehouse_code = c.String(nullable: false, maxLength: 20),
                        AreaCode = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        unit_code = c.String(nullable: false, maxLength: 20),
                        beginning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        entry_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        delivery_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        profit_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        loss_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ending = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wms_xml_value",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 50),
                        date_time = c.DateTime(nullable: false),
                        xml_value_text = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_region",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        region_name = c.String(nullable: false, maxLength: 50),
                        description = c.String(),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_path",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        path_name = c.String(nullable: false, maxLength: 50),
                        origin_region_id = c.Int(nullable: false),
                        target_region_id = c.Int(nullable: false),
                        description = c.String(),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wcs_region", t => t.origin_region_id)
                .ForeignKey("dbo.wcs_region", t => t.target_region_id)
                .Index(t => t.origin_region_id)
                .Index(t => t.target_region_id);
            
            CreateTable(
                "dbo.wcs_path_node",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        path_id = c.Int(nullable: false),
                        position_id = c.Int(nullable: false),
                        path_node_order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wcs_path", t => t.path_id)
                .ForeignKey("dbo.wcs_position", t => t.position_id)
                .Index(t => t.path_id)
                .Index(t => t.position_id);
            
            CreateTable(
                "dbo.wcs_position",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        position_name = c.String(nullable: false, maxLength: 50),
                        position_type = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        region_id = c.Int(nullable: false),
                        srmname = c.String(nullable: false, maxLength: 50),
                        travel_pos = c.Int(nullable: false),
                        lift_pos = c.Int(nullable: false),
                        extension = c.Int(nullable: false),
                        description = c.String(),
                        has_goods = c.Boolean(nullable: false),
                        able_stock_out = c.Boolean(nullable: false),
                        able_stock_in_pallet = c.Boolean(nullable: false),
                        tag_address = c.String(nullable: false),
                        current_task_id = c.Int(nullable: false),
                        current_operate_quantity = c.Int(nullable: false),
                        channel_code = c.String(maxLength: 50),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        has_get_request = c.Boolean(nullable: false),
                        has_put_request = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wcs_region", t => t.region_id)
                .Index(t => t.region_id);
            
            CreateTable(
                "dbo.wcs_cell_position",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        cell_code = c.String(nullable: false, maxLength: 20),
                        stock_in_position_id = c.Int(nullable: false),
                        stock_out_position_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.wcs_position", t => t.stock_in_position_id)
                .ForeignKey("dbo.wcs_position", t => t.stock_out_position_id)
                .Index(t => t.stock_in_position_id)
                .Index(t => t.stock_out_position_id);
            
            CreateTable(
                "dbo.wcs_srm",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        srmname = c.String(nullable: false, maxLength: 50),
                        description = c.String(),
                        opcservice_name = c.String(nullable: false, maxLength: 50),
                        get_request = c.String(nullable: false, maxLength: 50),
                        get_allow = c.String(nullable: false, maxLength: 50),
                        get_complete = c.String(nullable: false, maxLength: 50),
                        put_request = c.String(nullable: false, maxLength: 50),
                        put_allow = c.String(nullable: false, maxLength: 50),
                        put_complete = c.String(nullable: false, maxLength: 50),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        cancel_task = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_size",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        size_name = c.String(nullable: false, maxLength: 50),
                        size_no = c.Int(nullable: false),
                        length = c.Int(nullable: false),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_product_size",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_no = c.Int(nullable: false),
                        size_no = c.Int(nullable: false),
                        area_no = c.Int(nullable: false),
                        length = c.Int(nullable: false),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_task",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        task_type = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        task_level = c.Int(nullable: false),
                        path_id = c.Int(nullable: false),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 20),
                        origin_cell_code = c.String(nullable: false, maxLength: 50),
                        target_cell_code = c.String(nullable: false, maxLength: 50),
                        origin_storage_code = c.String(nullable: false, maxLength: 50),
                        target_storage_code = c.String(nullable: false, maxLength: 50),
                        origin_position_id = c.Int(nullable: false),
                        target_position_id = c.Int(nullable: false),
                        current_position_id = c.Int(nullable: false),
                        current_position_state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        tag_state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        quantity = c.Int(nullable: false),
                        task_quantity = c.Int(nullable: false),
                        operate_quantity = c.Int(nullable: false),
                        order_id = c.String(maxLength: 20),
                        order_type = c.String(maxLength: 2, fixedLength: true),
                        allot_id = c.Int(nullable: false),
                        download_state = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        storage_sequence = c.Int(nullable: false),
                        create_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_task_history",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        task_id = c.Int(nullable: false),
                        task_type = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        task_level = c.Int(nullable: false),
                        path_id = c.Int(nullable: false),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 20),
                        origin_cell_code = c.String(nullable: false, maxLength: 50),
                        target_cell_code = c.String(nullable: false, maxLength: 50),
                        origin_storage_code = c.String(nullable: false, maxLength: 50),
                        target_storage_code = c.String(nullable: false, maxLength: 50),
                        origin_position_id = c.Int(nullable: false),
                        target_position_id = c.Int(nullable: false),
                        current_position_id = c.Int(nullable: false),
                        current_position_state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        tag_state = c.String(nullable: false, maxLength: 2, fixedLength: true),
                        quantity = c.Int(nullable: false),
                        task_quantity = c.Int(nullable: false),
                        operate_quantity = c.Int(nullable: false),
                        order_id = c.String(maxLength: 20),
                        order_type = c.String(maxLength: 2, fixedLength: true),
                        allot_id = c.Int(nullable: false),
                        download_state = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        storage_sequence = c.Int(nullable: false),
                        create_time = c.DateTime(nullable: false),
                        clear_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.wcs_alarm_info",
                c => new
                    {
                        alarm_code = c.String(nullable: false, maxLength: 20),
                        description = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.alarm_code);
            
            CreateTable(
                "dbo.inter_bill_master",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        uuid = c.String(maxLength: 64),
                        bill_type = c.String(nullable: false, maxLength: 50),
                        bill_date = c.DateTime(nullable: false),
                        maker_name = c.String(maxLength: 50),
                        operate_date = c.DateTime(),
                        cigarette_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        bill_company_code = c.String(nullable: false, maxLength: 8),
                        supplier_code = c.String(nullable: false, maxLength: 20),
                        supplier_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        state = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.inter_bill_detail",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        master_id = c.Guid(nullable: false),
                        piece_cigar_code = c.String(nullable: false, maxLength: 13),
                        box_cigar_code = c.String(nullable: false, maxLength: 13),
                        bill_quantity = c.Decimal(nullable: false, precision: 16, scale: 4),
                        fixed_quantity = c.Decimal(nullable: false, precision: 16, scale: 4),
                        real_quantity = c.Decimal(nullable: false, precision: 16, scale: 4),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.inter_bill_master", t => t.master_id)
                .Index(t => t.master_id);
            
            CreateTable(
                "dbo.inter_contract",
                c => new
                    {
                        contract_code = c.String(nullable: false, maxLength: 50),
                        master_id = c.Guid(nullable: false),
                        supply_side_code = c.String(maxLength: 100),
                        demand_side_code = c.String(maxLength: 50),
                        contract_date = c.DateTime(nullable: false),
                        start_dade = c.DateTime(nullable: false),
                        end_date = c.DateTime(nullable: false),
                        send_place_code = c.String(),
                        send_address = c.String(maxLength: 100),
                        receive_place_code = c.String(maxLength: 100),
                        receive_address = c.String(maxLength: 100),
                        sale_date = c.String(maxLength: 100),
                        state = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.contract_code)
                .ForeignKey("dbo.inter_bill_master", t => t.master_id)
                .Index(t => t.master_id);
            
            CreateTable(
                "dbo.inter_contract_detail",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        contract_code = c.String(nullable: false, maxLength: 50),
                        brand_code = c.String(nullable: false, maxLength: 13),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        price = c.Decimal(nullable: false, precision: 16, scale: 2),
                        amount = c.Decimal(nullable: false, precision: 16, scale: 2),
                        tax_amount = c.Decimal(nullable: false, precision: 16, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.inter_contract", t => t.contract_code)
                .Index(t => t.contract_code);
            
            CreateTable(
                "dbo.inter_navicert",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        master_id = c.Guid(nullable: false),
                        navicert_code = c.String(nullable: false, maxLength: 8),
                        navicert_date = c.DateTime(nullable: false),
                        truck_plate_no = c.String(maxLength: 50),
                        contract_code = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.inter_bill_master", t => t.master_id)
                .ForeignKey("dbo.inter_contract", t => t.contract_code)
                .Index(t => t.master_id)
                .Index(t => t.contract_code);
            
            CreateTable(
                "dbo.inter_pallet",
                c => new
                    {
                        pallet_id = c.String(nullable: false, maxLength: 128),
                        wms_uuid = c.String(maxLength: 64),
                        uuid = c.String(maxLength: 64),
                        ticket_no = c.String(maxLength: 100),
                        operate_date = c.DateTime(),
                        operate_type = c.String(nullable: false, maxLength: 50),
                        bar_code_type = c.String(maxLength: 2),
                        rfid_ant_code = c.String(maxLength: 20),
                        piece_cigar_code = c.String(maxLength: 13),
                        box_cigar_code = c.String(maxLength: 13),
                        cigarette_name = c.String(maxLength: 50),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        scan_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.pallet_id);
            
            CreateTable(
                "dbo.sms_batch",
                c => new
                    {
                        batch_id = c.Int(nullable: false, identity: true),
                        order_date = c.DateTime(nullable: false),
                        batch_no = c.Int(nullable: false),
                        batch_name = c.String(nullable: false, maxLength: 100),
                        operate_date = c.DateTime(nullable: false),
                        operate_person_id = c.Guid(nullable: false),
                        optimize_schedule = c.Int(nullable: false),
                        verify_person_id = c.Guid(),
                        description = c.String(maxLength: 200),
                        project_batch_no = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.batch_id);
            
            CreateTable(
                "dbo.sms_batch_sort",
                c => new
                    {
                        batch_sort_id = c.Int(nullable: false, identity: true),
                        batch_id = c.Int(nullable: false),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.batch_sort_id)
                .ForeignKey("dbo.sms_batch", t => t.batch_id)
                .Index(t => t.batch_id);
            
            CreateTable(
                "dbo.sms_channel_allot",
                c => new
                    {
                        channel_allot_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                        in_quantity = c.Int(nullable: false),
                        out_quantity = c.Int(nullable: false),
                        real_quantity = c.Int(nullable: false),
                        remain_quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.channel_allot_code)
                .ForeignKey("dbo.sms_batch_sort", t => t.batch_sort_id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .Index(t => t.batch_sort_id)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_channel",
                c => new
                    {
                        channel_code = c.String(nullable: false, maxLength: 20),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        channel_name = c.String(nullable: false, maxLength: 100),
                        channel_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        led_code = c.String(nullable: false, maxLength: 20),
                        default_product_code = c.String(maxLength: 20),
                        remain_quantity = c.Int(nullable: false),
                        middle_quantity = c.Int(nullable: false),
                        max_quantity = c.Int(nullable: false),
                        group_no = c.Int(nullable: false),
                        order_no = c.Int(nullable: false),
                        address = c.Int(nullable: false),
                        cell_code = c.String(maxLength: 20),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.channel_code)
                .ForeignKey("dbo.sms_led", t => t.led_code)
                .Index(t => t.led_code);
            
            CreateTable(
                "dbo.sms_led",
                c => new
                    {
                        led_code = c.String(nullable: false, maxLength: 20),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        led_name = c.String(nullable: false, maxLength: 100),
                        led_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        led_ip = c.String(nullable: false, maxLength: 20),
                        xaxes = c.Int(nullable: false),
                        yaxes = c.Int(nullable: false),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                        led_group_code = c.String(nullable: false, maxLength: 20),
                        order_no = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.led_code)
                .ForeignKey("dbo.sms_led", t => t.led_group_code)
                .Index(t => t.led_group_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_detail",
                c => new
                    {
                        order_detail_code = c.String(nullable: false, maxLength: 50),
                        order_master_code = c.String(nullable: false, maxLength: 50),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.order_detail_code)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .ForeignKey("dbo.sms_sort_order_allot_master", t => t.order_master_code)
                .Index(t => t.order_master_code)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_master",
                c => new
                    {
                        order_master_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        order_id = c.String(nullable: false, maxLength: 20),
                        customer_order = c.Int(nullable: false),
                        customer_deliver_order = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        export_no = c.Int(nullable: false),
                        start_time = c.DateTime(nullable: false),
                        finish_time = c.DateTime(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.order_master_code)
                .ForeignKey("dbo.sms_batch_sort", t => t.batch_sort_id)
                .Index(t => t.batch_sort_id);
            
            CreateTable(
                "dbo.sms_sort_supply",
                c => new
                    {
                        sort_supply_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        supply_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.sort_supply_code)
                .ForeignKey("dbo.sms_batch_sort", t => t.batch_sort_id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .Index(t => t.batch_sort_id)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_deliver_line_allot",
                c => new
                    {
                        deliver_line_allot_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        deliver_quantity = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.deliver_line_allot_code)
                .ForeignKey("dbo.sms_batch_sort", t => t.batch_sort_id)
                .Index(t => t.batch_sort_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sms_deliver_line_allot", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_channel_allot", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_sort_supply", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_sort_supply", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "order_master_code", "dbo.sms_sort_order_allot_master");
            DropForeignKey("dbo.sms_sort_order_allot_master", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_channel", "led_code", "dbo.sms_led");
            DropForeignKey("dbo.sms_led", "led_group_code", "dbo.sms_led");
            DropForeignKey("dbo.sms_channel_allot", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_batch_sort", "batch_id", "dbo.sms_batch");
            DropForeignKey("dbo.inter_navicert", "contract_code", "dbo.inter_contract");
            DropForeignKey("dbo.inter_navicert", "master_id", "dbo.inter_bill_master");
            DropForeignKey("dbo.inter_contract_detail", "contract_code", "dbo.inter_contract");
            DropForeignKey("dbo.inter_contract", "master_id", "dbo.inter_bill_master");
            DropForeignKey("dbo.inter_bill_detail", "master_id", "dbo.inter_bill_master");
            DropForeignKey("dbo.wcs_path", "target_region_id", "dbo.wcs_region");
            DropForeignKey("dbo.wcs_path_node", "position_id", "dbo.wcs_position");
            DropForeignKey("dbo.wcs_cell_position", "stock_out_position_id", "dbo.wcs_position");
            DropForeignKey("dbo.wcs_cell_position", "stock_in_position_id", "dbo.wcs_position");
            DropForeignKey("dbo.wcs_position", "region_id", "dbo.wcs_region");
            DropForeignKey("dbo.wcs_path_node", "path_id", "dbo.wcs_path");
            DropForeignKey("dbo.wcs_path", "origin_region_id", "dbo.wcs_region");
            DropForeignKey("dbo.wms_profit_loss_bill_detail_history", "bill_no", "dbo.wms_profit_loss_bill_master_history");
            DropForeignKey("dbo.wms_check_bill_detail_history", "bill_no", "dbo.wms_check_bill_master_history");
            DropForeignKey("dbo.wms_move_bill_detail_history", "bill_no", "dbo.wms_move_bill_master_history");
            DropForeignKey("dbo.wms_out_bill_allot_history", "bill_no", "dbo.wms_out_bill_master_history");
            DropForeignKey("dbo.wms_out_bill_allot_history", "out_bill_detail_id", "dbo.wms_out_bill_detail_history");
            DropForeignKey("dbo.wms_out_bill_detail_history", "bill_no", "dbo.wms_out_bill_master_history");
            DropForeignKey("dbo.wms_in_bill_allot_history", "bill_no", "dbo.wms_in_bill_master_history");
            DropForeignKey("dbo.wms_in_bill_allot_history", "in_bill_detail_id", "dbo.wms_in_bill_detail_history");
            DropForeignKey("dbo.wms_in_bill_detail_history", "bill_no", "dbo.wms_in_bill_master_history");
            DropForeignKey("dbo.wms_product_warning", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_business_systems_daily_balance", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_business_systems_daily_balance", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_business_systems_daily_balance", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_company", "parent_company_id", "dbo.wms_company");
            DropForeignKey("dbo.wms_department", "parent_department_id", "dbo.wms_department");
            DropForeignKey("dbo.wms_department", "department_leader_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_employee", "job_id", "dbo.wms_job");
            DropForeignKey("dbo.wms_employee", "department_id", "dbo.wms_department");
            DropForeignKey("dbo.wms_check_bill_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_check_bill_detail", "storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_check_bill_detail", "real_unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_check_bill_detail", "real_product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_check_bill_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_check_bill_detail", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_check_bill_detail", "bill_no", "dbo.wms_check_bill_master");
            DropForeignKey("dbo.wms_check_bill_detail", "cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_cell", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_cell", "shelf_code", "dbo.wms_shelf");
            DropForeignKey("dbo.wms_cell", "default_product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_cell", "area_code", "dbo.wms_area");
            DropForeignKey("dbo.wms_area", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_shelf", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_check_bill_master", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_check_bill_master", "verify_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_check_bill_master", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_check_bill_master", "bill_type_code", "dbo.wms_bill_type");
            DropForeignKey("dbo.wms_in_bill_master", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_in_bill_master", "verify_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_in_bill_master", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_in_bill_allot", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_in_bill_allot", "storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_in_bill_allot", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_in_bill_allot", "bill_no", "dbo.wms_in_bill_master");
            DropForeignKey("dbo.wms_in_bill_allot", "in_bill_detail_id", "dbo.wms_in_bill_detail");
            DropForeignKey("dbo.wms_in_bill_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_in_bill_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_product", "unit_list_code", "dbo.wms_unit_list");
            DropForeignKey("dbo.wms_unit_list", "unit_code04", "dbo.wms_unit");
            DropForeignKey("dbo.wms_unit_list", "unit_code03", "dbo.wms_unit");
            DropForeignKey("dbo.wms_unit_list", "unit_code02", "dbo.wms_unit");
            DropForeignKey("dbo.wms_unit_list", "unit_code01", "dbo.wms_unit");
            DropForeignKey("dbo.wms_product", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_product", "supplier_code", "dbo.wms_supplier");
            DropForeignKey("dbo.wms_daily_balance", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_daily_balance", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_sorting_lowerlimit", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_sorting_lowerlimit", "sorting_line_code", "dbo.wms_sorting_line");
            DropForeignKey("dbo.wms_sort_order_dispatch", "sort_work_dispatch_id", "dbo.wms_sort_work_dispatch");
            DropForeignKey("dbo.wms_sort_work_dispatch", "sorting_line_code", "dbo.wms_sorting_line");
            DropForeignKey("dbo.wms_sort_work_dispatch", "out_bill_no", "dbo.wms_out_bill_master");
            DropForeignKey("dbo.wms_sort_work_dispatch", "move_bill_no", "dbo.wms_move_bill_master");
            DropForeignKey("dbo.wms_sort_order_dispatch", "sorting_line_code", "dbo.wms_sorting_line");
            DropForeignKey("dbo.wms_sort_order_dispatch", "deliver_line_code", "dbo.wms_deliver_line");
            DropForeignKey("dbo.wms_sort_order_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_sort_order_detail", "order_id", "dbo.wms_sort_order");
            DropForeignKey("dbo.wms_sort_order_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_sort_order", "deliver_line_code", "dbo.wms_deliver_line");
            DropForeignKey("dbo.wms_sorting_line", "cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_sorting_lowerlimit", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_move_bill_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_move_bill_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_move_bill_detail", "out_storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_move_bill_detail", "out_cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_move_bill_detail", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_move_bill_detail", "bill_no", "dbo.wms_move_bill_master");
            DropForeignKey("dbo.wms_move_bill_detail", "in_storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_profit_loss_bill_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_profit_loss_bill_detail", "storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_profit_loss_bill_detail", "bill_no", "dbo.wms_profit_loss_bill_master");
            DropForeignKey("dbo.wms_profit_loss_bill_master", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_profit_loss_bill_master", "verify_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_profit_loss_bill_master", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_profit_loss_bill_master", "bill_type_code", "dbo.wms_bill_type");
            DropForeignKey("dbo.wms_profit_loss_bill_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_storage", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_out_bill_allot", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_out_bill_allot", "storage_code", "dbo.wms_storage");
            DropForeignKey("dbo.wms_out_bill_allot", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_out_bill_allot", "bill_no", "dbo.wms_out_bill_master");
            DropForeignKey("dbo.wms_out_bill_allot", "out_bill_detail_id", "dbo.wms_out_bill_detail");
            DropForeignKey("dbo.wms_out_bill_detail", "unit_code", "dbo.wms_unit");
            DropForeignKey("dbo.wms_out_bill_detail", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_out_bill_detail", "bill_no", "dbo.wms_out_bill_master");
            DropForeignKey("dbo.wms_out_bill_master", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_out_bill_master", "verify_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_out_bill_master", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_out_bill_master", "move_bill_master_bill_no", "dbo.wms_move_bill_master");
            DropForeignKey("dbo.wms_move_bill_master", "warehouse_code", "dbo.wms_warehouse");
            DropForeignKey("dbo.wms_move_bill_master", "verify_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_move_bill_master", "operate_person_id", "dbo.wms_employee");
            DropForeignKey("dbo.wms_move_bill_master", "bill_type_code", "dbo.wms_bill_type");
            DropForeignKey("dbo.wms_out_bill_master", "bill_type_code", "dbo.wms_bill_type");
            DropForeignKey("dbo.wms_out_bill_allot", "cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_storage", "cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_move_bill_detail", "in_cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_daily_balance", "product_code", "dbo.wms_product");
            DropForeignKey("dbo.wms_daily_balance", "AreaCode", "dbo.wms_area");
            DropForeignKey("dbo.wms_product", "brand_code", "dbo.wms_brand");
            DropForeignKey("dbo.wms_brand", "supplier_code", "dbo.wms_supplier");
            DropForeignKey("dbo.wms_in_bill_detail", "bill_no", "dbo.wms_in_bill_master");
            DropForeignKey("dbo.wms_in_bill_allot", "cell_code", "dbo.wms_cell");
            DropForeignKey("dbo.wms_in_bill_master", "bill_type_code", "dbo.wms_bill_type");
            DropForeignKey("dbo.wms_shelf", "area_code", "dbo.wms_area");
            DropForeignKey("dbo.wms_department", "company_id", "dbo.wms_company");
            DropForeignKey("dbo.auth_help_content", "module_id", "dbo.auth_module");
            DropForeignKey("dbo.auth_help_content", "father_node_id", "dbo.auth_help_content");
            DropForeignKey("dbo.auth_server", "city_city_id", "dbo.auth_city");
            DropForeignKey("dbo.auth_role_system", "system_system_id", "dbo.auth_system");
            DropForeignKey("dbo.auth_role_system", "role_role_id", "dbo.auth_role");
            DropForeignKey("dbo.auth_user_role", "user_user_id", "dbo.auth_user");
            DropForeignKey("dbo.auth_login_log", "user_user_id", "dbo.auth_user");
            DropForeignKey("dbo.auth_login_log", "system_system_id", "dbo.auth_system");
            DropForeignKey("dbo.auth_system_parameter", "system_id", "dbo.auth_system");
            DropForeignKey("dbo.auth_module", "system_system_id", "dbo.auth_system");
            DropForeignKey("dbo.auth_module", "parent_module_module_id", "dbo.auth_module");
            DropForeignKey("dbo.auth_user_function", "user_module_user_module_id", "dbo.auth_user_module");
            DropForeignKey("dbo.auth_user_module", "user_system_user_system_id", "dbo.auth_user_system");
            DropForeignKey("dbo.auth_user_system", "user_user_id", "dbo.auth_user");
            DropForeignKey("dbo.auth_user_system", "system_system_id", "dbo.auth_system");
            DropForeignKey("dbo.auth_user_system", "city_city_id", "dbo.auth_city");
            DropForeignKey("dbo.auth_user_module", "module_module_id", "dbo.auth_module");
            DropForeignKey("dbo.auth_user_function", "function_function_id", "dbo.auth_function");
            DropForeignKey("dbo.auth_role_function", "role_module_role_module_id", "dbo.auth_role_module");
            DropForeignKey("dbo.auth_role_module", "role_system_role_system_id", "dbo.auth_role_system");
            DropForeignKey("dbo.auth_role_module", "module_module_id", "dbo.auth_module");
            DropForeignKey("dbo.auth_role_function", "function_function_id", "dbo.auth_function");
            DropForeignKey("dbo.auth_function", "module_module_id", "dbo.auth_module");
            DropForeignKey("dbo.auth_user_role", "role_role_id", "dbo.auth_role");
            DropForeignKey("dbo.auth_role_system", "city_city_id", "dbo.auth_city");
            DropIndex("dbo.sms_deliver_line_allot", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_sort_supply", new[] { "channel_code" });
            DropIndex("dbo.sms_sort_supply", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_sort_order_allot_master", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "channel_code" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "order_master_code" });
            DropIndex("dbo.sms_led", new[] { "led_group_code" });
            DropIndex("dbo.sms_channel", new[] { "led_code" });
            DropIndex("dbo.sms_channel_allot", new[] { "channel_code" });
            DropIndex("dbo.sms_channel_allot", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_batch_sort", new[] { "batch_id" });
            DropIndex("dbo.inter_navicert", new[] { "contract_code" });
            DropIndex("dbo.inter_navicert", new[] { "master_id" });
            DropIndex("dbo.inter_contract_detail", new[] { "contract_code" });
            DropIndex("dbo.inter_contract", new[] { "master_id" });
            DropIndex("dbo.inter_bill_detail", new[] { "master_id" });
            DropIndex("dbo.wcs_cell_position", new[] { "stock_out_position_id" });
            DropIndex("dbo.wcs_cell_position", new[] { "stock_in_position_id" });
            DropIndex("dbo.wcs_position", new[] { "region_id" });
            DropIndex("dbo.wcs_path_node", new[] { "position_id" });
            DropIndex("dbo.wcs_path_node", new[] { "path_id" });
            DropIndex("dbo.wcs_path", new[] { "target_region_id" });
            DropIndex("dbo.wcs_path", new[] { "origin_region_id" });
            DropIndex("dbo.wms_profit_loss_bill_detail_history", new[] { "bill_no" });
            DropIndex("dbo.wms_check_bill_detail_history", new[] { "bill_no" });
            DropIndex("dbo.wms_move_bill_detail_history", new[] { "bill_no" });
            DropIndex("dbo.wms_out_bill_detail_history", new[] { "bill_no" });
            DropIndex("dbo.wms_out_bill_allot_history", new[] { "out_bill_detail_id" });
            DropIndex("dbo.wms_out_bill_allot_history", new[] { "bill_no" });
            DropIndex("dbo.wms_in_bill_detail_history", new[] { "bill_no" });
            DropIndex("dbo.wms_in_bill_allot_history", new[] { "in_bill_detail_id" });
            DropIndex("dbo.wms_in_bill_allot_history", new[] { "bill_no" });
            DropIndex("dbo.wms_product_warning", new[] { "unit_code" });
            DropIndex("dbo.wms_business_systems_daily_balance", new[] { "unit_code" });
            DropIndex("dbo.wms_business_systems_daily_balance", new[] { "product_code" });
            DropIndex("dbo.wms_business_systems_daily_balance", new[] { "warehouse_code" });
            DropIndex("dbo.wms_unit_list", new[] { "unit_code04" });
            DropIndex("dbo.wms_unit_list", new[] { "unit_code03" });
            DropIndex("dbo.wms_unit_list", new[] { "unit_code02" });
            DropIndex("dbo.wms_unit_list", new[] { "unit_code01" });
            DropIndex("dbo.wms_sort_work_dispatch", new[] { "move_bill_no" });
            DropIndex("dbo.wms_sort_work_dispatch", new[] { "out_bill_no" });
            DropIndex("dbo.wms_sort_work_dispatch", new[] { "sorting_line_code" });
            DropIndex("dbo.wms_sort_order_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_sort_order_detail", new[] { "product_code" });
            DropIndex("dbo.wms_sort_order_detail", new[] { "order_id" });
            DropIndex("dbo.wms_sort_order", new[] { "deliver_line_code" });
            DropIndex("dbo.wms_sort_order_dispatch", new[] { "sort_work_dispatch_id" });
            DropIndex("dbo.wms_sort_order_dispatch", new[] { "deliver_line_code" });
            DropIndex("dbo.wms_sort_order_dispatch", new[] { "sorting_line_code" });
            DropIndex("dbo.wms_sorting_line", new[] { "cell_code" });
            DropIndex("dbo.wms_sorting_lowerlimit", new[] { "unit_code" });
            DropIndex("dbo.wms_sorting_lowerlimit", new[] { "product_code" });
            DropIndex("dbo.wms_sorting_lowerlimit", new[] { "sorting_line_code" });
            DropIndex("dbo.wms_profit_loss_bill_master", new[] { "verify_person_id" });
            DropIndex("dbo.wms_profit_loss_bill_master", new[] { "operate_person_id" });
            DropIndex("dbo.wms_profit_loss_bill_master", new[] { "warehouse_code" });
            DropIndex("dbo.wms_profit_loss_bill_master", new[] { "bill_type_code" });
            DropIndex("dbo.wms_profit_loss_bill_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_profit_loss_bill_detail", new[] { "product_code" });
            DropIndex("dbo.wms_profit_loss_bill_detail", new[] { "storage_code" });
            DropIndex("dbo.wms_profit_loss_bill_detail", new[] { "bill_no" });
            DropIndex("dbo.wms_move_bill_master", new[] { "verify_person_id" });
            DropIndex("dbo.wms_move_bill_master", new[] { "operate_person_id" });
            DropIndex("dbo.wms_move_bill_master", new[] { "warehouse_code" });
            DropIndex("dbo.wms_move_bill_master", new[] { "bill_type_code" });
            DropIndex("dbo.wms_out_bill_master", new[] { "move_bill_master_bill_no" });
            DropIndex("dbo.wms_out_bill_master", new[] { "verify_person_id" });
            DropIndex("dbo.wms_out_bill_master", new[] { "operate_person_id" });
            DropIndex("dbo.wms_out_bill_master", new[] { "warehouse_code" });
            DropIndex("dbo.wms_out_bill_master", new[] { "bill_type_code" });
            DropIndex("dbo.wms_out_bill_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_out_bill_detail", new[] { "product_code" });
            DropIndex("dbo.wms_out_bill_detail", new[] { "bill_no" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "unit_code" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "storage_code" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "cell_code" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "out_bill_detail_id" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "product_code" });
            DropIndex("dbo.wms_out_bill_allot", new[] { "bill_no" });
            DropIndex("dbo.wms_storage", new[] { "product_code" });
            DropIndex("dbo.wms_storage", new[] { "cell_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "operate_person_id" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "in_storage_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "in_cell_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "out_storage_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "out_cell_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "product_code" });
            DropIndex("dbo.wms_move_bill_detail", new[] { "bill_no" });
            DropIndex("dbo.wms_daily_balance", new[] { "unit_code" });
            DropIndex("dbo.wms_daily_balance", new[] { "product_code" });
            DropIndex("dbo.wms_daily_balance", new[] { "AreaCode" });
            DropIndex("dbo.wms_daily_balance", new[] { "warehouse_code" });
            DropIndex("dbo.wms_brand", new[] { "supplier_code" });
            DropIndex("dbo.wms_product", new[] { "brand_code" });
            DropIndex("dbo.wms_product", new[] { "supplier_code" });
            DropIndex("dbo.wms_product", new[] { "unit_code" });
            DropIndex("dbo.wms_product", new[] { "unit_list_code" });
            DropIndex("dbo.wms_in_bill_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_in_bill_detail", new[] { "product_code" });
            DropIndex("dbo.wms_in_bill_detail", new[] { "bill_no" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "unit_code" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "storage_code" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "cell_code" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "in_bill_detail_id" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "product_code" });
            DropIndex("dbo.wms_in_bill_allot", new[] { "bill_no" });
            DropIndex("dbo.wms_in_bill_master", new[] { "verify_person_id" });
            DropIndex("dbo.wms_in_bill_master", new[] { "operate_person_id" });
            DropIndex("dbo.wms_in_bill_master", new[] { "warehouse_code" });
            DropIndex("dbo.wms_in_bill_master", new[] { "bill_type_code" });
            DropIndex("dbo.wms_check_bill_master", new[] { "verify_person_id" });
            DropIndex("dbo.wms_check_bill_master", new[] { "operate_person_id" });
            DropIndex("dbo.wms_check_bill_master", new[] { "warehouse_code" });
            DropIndex("dbo.wms_check_bill_master", new[] { "bill_type_code" });
            DropIndex("dbo.wms_shelf", new[] { "area_code" });
            DropIndex("dbo.wms_shelf", new[] { "warehouse_code" });
            DropIndex("dbo.wms_area", new[] { "warehouse_code" });
            DropIndex("dbo.wms_cell", new[] { "default_product_code" });
            DropIndex("dbo.wms_cell", new[] { "shelf_code" });
            DropIndex("dbo.wms_cell", new[] { "area_code" });
            DropIndex("dbo.wms_cell", new[] { "warehouse_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "operate_person_id" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "real_unit_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "real_product_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "unit_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "product_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "storage_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "cell_code" });
            DropIndex("dbo.wms_check_bill_detail", new[] { "bill_no" });
            DropIndex("dbo.wms_employee", new[] { "job_id" });
            DropIndex("dbo.wms_employee", new[] { "department_id" });
            DropIndex("dbo.wms_department", new[] { "parent_department_id" });
            DropIndex("dbo.wms_department", new[] { "company_id" });
            DropIndex("dbo.wms_department", new[] { "department_leader_id" });
            DropIndex("dbo.wms_company", new[] { "parent_company_id" });
            DropIndex("dbo.auth_help_content", new[] { "module_id" });
            DropIndex("dbo.auth_help_content", new[] { "father_node_id" });
            DropIndex("dbo.auth_server", new[] { "city_city_id" });
            DropIndex("dbo.auth_system_parameter", new[] { "system_id" });
            DropIndex("dbo.auth_user_system", new[] { "system_system_id" });
            DropIndex("dbo.auth_user_system", new[] { "city_city_id" });
            DropIndex("dbo.auth_user_system", new[] { "user_user_id" });
            DropIndex("dbo.auth_user_module", new[] { "module_module_id" });
            DropIndex("dbo.auth_user_module", new[] { "user_system_user_system_id" });
            DropIndex("dbo.auth_user_function", new[] { "function_function_id" });
            DropIndex("dbo.auth_user_function", new[] { "user_module_user_module_id" });
            DropIndex("dbo.auth_role_module", new[] { "module_module_id" });
            DropIndex("dbo.auth_role_module", new[] { "role_system_role_system_id" });
            DropIndex("dbo.auth_role_function", new[] { "function_function_id" });
            DropIndex("dbo.auth_role_function", new[] { "role_module_role_module_id" });
            DropIndex("dbo.auth_function", new[] { "module_module_id" });
            DropIndex("dbo.auth_module", new[] { "parent_module_module_id" });
            DropIndex("dbo.auth_module", new[] { "system_system_id" });
            DropIndex("dbo.auth_login_log", new[] { "system_system_id" });
            DropIndex("dbo.auth_login_log", new[] { "user_user_id" });
            DropIndex("dbo.auth_user_role", new[] { "user_user_id" });
            DropIndex("dbo.auth_user_role", new[] { "role_role_id" });
            DropIndex("dbo.auth_role_system", new[] { "system_system_id" });
            DropIndex("dbo.auth_role_system", new[] { "city_city_id" });
            DropIndex("dbo.auth_role_system", new[] { "role_role_id" });
            DropTable("dbo.sms_deliver_line_allot");
            DropTable("dbo.sms_sort_supply");
            DropTable("dbo.sms_sort_order_allot_master");
            DropTable("dbo.sms_sort_order_allot_detail");
            DropTable("dbo.sms_led");
            DropTable("dbo.sms_channel");
            DropTable("dbo.sms_channel_allot");
            DropTable("dbo.sms_batch_sort");
            DropTable("dbo.sms_batch");
            DropTable("dbo.inter_pallet");
            DropTable("dbo.inter_navicert");
            DropTable("dbo.inter_contract_detail");
            DropTable("dbo.inter_contract");
            DropTable("dbo.inter_bill_detail");
            DropTable("dbo.inter_bill_master");
            DropTable("dbo.wcs_alarm_info");
            DropTable("dbo.wcs_task_history");
            DropTable("dbo.wcs_task");
            DropTable("dbo.wcs_product_size");
            DropTable("dbo.wcs_size");
            DropTable("dbo.wcs_srm");
            DropTable("dbo.wcs_cell_position");
            DropTable("dbo.wcs_position");
            DropTable("dbo.wcs_path_node");
            DropTable("dbo.wcs_path");
            DropTable("dbo.wcs_region");
            DropTable("dbo.wms_xml_value");
            DropTable("dbo.wms_daily_balance_history");
            DropTable("dbo.wms_profit_loss_bill_detail_history");
            DropTable("dbo.wms_profit_loss_bill_master_history");
            DropTable("dbo.wms_check_bill_detail_history");
            DropTable("dbo.wms_check_bill_master_history");
            DropTable("dbo.wms_move_bill_detail_history");
            DropTable("dbo.wms_move_bill_master_history");
            DropTable("dbo.wms_out_bill_detail_history");
            DropTable("dbo.wms_out_bill_allot_history");
            DropTable("dbo.wms_out_bill_master_history");
            DropTable("dbo.wms_in_bill_detail_history");
            DropTable("dbo.wms_in_bill_allot_history");
            DropTable("dbo.wms_in_bill_master_history");
            DropTable("dbo.wms_tray_info");
            DropTable("dbo.wms_product_warning");
            DropTable("dbo.wms_business_systems_daily_balance");
            DropTable("dbo.wms_customer");
            DropTable("dbo.wms_deliver_dist");
            DropTable("dbo.wms_job");
            DropTable("dbo.wms_unit_list");
            DropTable("dbo.wms_sort_work_dispatch");
            DropTable("dbo.wms_sort_order_detail");
            DropTable("dbo.wms_sort_order");
            DropTable("dbo.wms_deliver_line");
            DropTable("dbo.wms_sort_order_dispatch");
            DropTable("dbo.wms_sorting_line");
            DropTable("dbo.wms_sorting_lowerlimit");
            DropTable("dbo.wms_profit_loss_bill_master");
            DropTable("dbo.wms_profit_loss_bill_detail");
            DropTable("dbo.wms_move_bill_master");
            DropTable("dbo.wms_out_bill_master");
            DropTable("dbo.wms_out_bill_detail");
            DropTable("dbo.wms_out_bill_allot");
            DropTable("dbo.wms_storage");
            DropTable("dbo.wms_move_bill_detail");
            DropTable("dbo.wms_unit");
            DropTable("dbo.wms_daily_balance");
            DropTable("dbo.wms_supplier");
            DropTable("dbo.wms_brand");
            DropTable("dbo.wms_product");
            DropTable("dbo.wms_in_bill_detail");
            DropTable("dbo.wms_in_bill_allot");
            DropTable("dbo.wms_in_bill_master");
            DropTable("dbo.wms_bill_type");
            DropTable("dbo.wms_check_bill_master");
            DropTable("dbo.wms_warehouse");
            DropTable("dbo.wms_shelf");
            DropTable("dbo.wms_area");
            DropTable("dbo.wms_cell");
            DropTable("dbo.wms_check_bill_detail");
            DropTable("dbo.wms_employee");
            DropTable("dbo.wms_department");
            DropTable("dbo.wms_company");
            DropTable("dbo.auth_exceptional_log");
            DropTable("dbo.auth_help_content");
            DropTable("dbo.auth_system_event_log");
            DropTable("dbo.auth_server");
            DropTable("dbo.auth_system_parameter");
            DropTable("dbo.auth_user_system");
            DropTable("dbo.auth_user_module");
            DropTable("dbo.auth_user_function");
            DropTable("dbo.auth_role_module");
            DropTable("dbo.auth_role_function");
            DropTable("dbo.auth_function");
            DropTable("dbo.auth_module");
            DropTable("dbo.auth_system");
            DropTable("dbo.auth_login_log");
            DropTable("dbo.auth_user");
            DropTable("dbo.auth_user_role");
            DropTable("dbo.auth_role");
            DropTable("dbo.auth_role_system");
            DropTable("dbo.auth_city");
        }
    }
}
