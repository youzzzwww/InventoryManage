using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManage.Models
{
    public class WorkerWithRole
    {
        public Worker Worker { get; set; }
        [Display(Name = "角色")]
        public List<String> Roles { get; set; }
        public List<SelectListItem> RoleList { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}至少要有{2}字符.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "确认密码应同新密码一致")]
        public string ConfirmPassword { get; set; }
    }

    public class WorkerIndexViewModel
    {
        public WorkerWithRole WorkerAdd { get; set; }
        public PaginatedList<WorkerWithRole> WorkerList { get; set; }
    }
}
