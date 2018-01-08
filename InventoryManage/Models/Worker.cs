using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace InventoryManage.Models
{
    public class Worker : IdentityUser<int>
    {
        public Worker()
        {
            EnrollmentDate = DateTime.Now;
            Disabled = false;
        }     
    
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "入职日期")]
        public DateTime EnrollmentDate { get; set; }
        [Display(Name = "姓名")]
        [StringLength(50, ErrorMessage = "姓名不能超过50个字符")]
        public override string UserName { get; set; }
        public bool Disabled { get; set; }

        [Display(Name = "部门")]
        public String Department { get; set; }
    }   
}
