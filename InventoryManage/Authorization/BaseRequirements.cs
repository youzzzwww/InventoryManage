using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InventoryManage.Authorization
{
    public class BaseRequirements
    {
        public static OperationAuthorizationRequirement ViewRequirement =
            new OperationAuthorizationRequirement {Name = "View"};
    }

    public class Constant
    {
        public readonly static string EquipmentViewClaim = "查看设备";
        public readonly static string EquipmentEditClaim = "修改设备";
        public readonly static string EquipmentDeleteClaim = "删除设备";
        public readonly static string EquipmentAddClaim = "添加设备";

        public readonly static string InvoiceViewClaim = "查看申请单";
        public readonly static string InvoiceEditClaim = "修改申请单";
        public readonly static string InvoiceDeleteClaim = "删除申请单";
        public readonly static string InvoiceAddClaim = "添加申请单";

        public readonly static string WorkerViewClaim = "查看人员";
        public readonly static string WorkerEditClaim = "修改人员";
        public readonly static string WorkerDeleteClaim = "删除人员";
        public readonly static string WorkerAddClaim = "添加人员";

        public readonly static string Admin = "系统管理员";
        public readonly static string EquipmentManager = "库存管理员";
        public readonly static string WorkerManager = "用户管理员";
        public readonly static string Approver = "审批人员";
        public readonly static string NormalUser = "职工";
    }
}
