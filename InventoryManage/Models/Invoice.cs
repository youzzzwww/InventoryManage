using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManage.Models
{
    public enum InvoiceStatus
    {
        未处理, 通过, 拒绝
    }
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int WorkerId { get; set; }
        public int EquipmentId { get; set; }
        public int? ApproverId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "数量应该在1-10之间")]
        [Display(Name = "申请数量")]
        public int Number { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申请日期")]
        public DateTime Date { get; set; }
        [Display(Name = "申请理由")]
        [StringLength(200, ErrorMessage = "理由不能超过200个字符")]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "订单状态")]
        public InvoiceStatus Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "审核日期")]
        public DateTime ApproveDate { get; set; }
        [Display(Name = "审核意见")]
        [StringLength(200, ErrorMessage = "审核意见不能超过200个字符")]
        public string ApproveReason { get; set; }

        public Worker Worker { get; set; }
        public Equipment Equipment { get; set; }
        public Worker Approver { get; set; }
    }
}
