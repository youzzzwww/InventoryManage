using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InventoryManage.Models
{
    public enum EquipmentType
    {
        固定资产, 耗材
    }
    public class Equipment
    {
        public int EquipmentId { get; set; }
        [Required]
        [Display(Name = "设备类型")]
        public EquipmentType Type { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "设备种类不能超过50个字符")]
        [Display(Name = "设备种类")]
        public string Category { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "名称不能超过50个字符")]
        [Display(Name = "设备名称")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "详细描述不超过100个字符")]
        [Display(Name = "详细描述")]
        public string Detail { get; set; }
        [Required]
        [Range(0, 10000, ErrorMessage = "数量应该在0-10000之间")]
        [Display(Name = "库存数量")]
        public int Number { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
    }
}
