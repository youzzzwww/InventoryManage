﻿using System;
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
        }     
    
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "入职日期")]
        public DateTime EnrollmentDate { get; set; }
        [Required]
        [Display(Name = "姓名")]
        [StringLength(50, ErrorMessage = "姓名不能超过50个字符")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "是否在职")]
        public bool IsStaff { get; set; }
        [Display(Name = "部门")]
        public String Department { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
    }   
}
