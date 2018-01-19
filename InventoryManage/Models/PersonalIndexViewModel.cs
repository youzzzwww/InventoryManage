using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManage.Models
{
    public class AssetViewModel
    {
        [Required]
        public int EquipmentId { get; set; }
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
        [Display(Name = "数量")]
        public int Number { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "日期")]
        public DateTime Date { get; set; }
    }
    public class ConsumableViewModel
    {
        [Required]
        public int EquipmentId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "设备种类不能超过50个字符")]
        [Display(Name = "设备种类")]
        public string Category { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "名称不能超过50个字符")]
        [Display(Name = "设备名称")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "数量")]
        public int Number { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "日期")]
        public DateTime Date { get; set; }
    }

    public class PersonalIndexViewModel
    {
        public ICollection<AssetViewModel> assetViewModel { get; set; }
        public ICollection<ConsumableViewModel> consumableViewModel { get; set; }
        public PaginatedList<Invoice> invoiceList { get; set; }
    }
}
