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
    }

    public class WorkerIndexViewModel
    {
        public WorkerWithRole WorkerAdd { get; set; }
        public PaginatedList<WorkerWithRole> WorkerList { get; set; }
    }
}
