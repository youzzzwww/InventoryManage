using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManage.Models
{
    public class ApproveInvoiceViewModel
    {
        [Required]
        public int InvoiceId { get; set; }
        [Required]
        [Display(Name = "申请人")]
        public string Proposer { get; set; }
        [Required]
        [Display(Name = "设备类型")]
        public EquipmentType Type { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "设备种类不能超过50个字符")]
        [Display(Name = "种类")]
        public string Category { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "型号不能超过50个字符")]
        [Display(Name = "型号")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "详细描述不超过100个字符")]
        [Display(Name = "详细描述")]
        public string Detail { get; set; }
        [Required]
        [Range(1, 10, ErrorMessage = "数量应该在1-10之间")]
        [Display(Name = "申请数量")]
        public int Number { get; set; }
        [Required]
        [Display(Name = "库存数量")]
        public int RemainNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "日期")]
        public DateTime Date { get; set; }
        [Display(Name = "申请理由")]
        [StringLength(200, ErrorMessage = "理由不能超过200个字符")]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "状态")]
        public InvoiceStatus Status { get; set; }
        [Display(Name = "审批人")]
        public string Approver { get; set; }
        [Display(Name = "审核意见")]
        [StringLength(200, ErrorMessage = "审核意见不能超过200个字符")]
        public string ApproveReason { get; set; }
    }
}
