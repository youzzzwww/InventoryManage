using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManage.Models
{
    public class EquipmentIndexViewModel
    {
        public Equipment EquipmentAdd { get; set; }
        public PaginatedList<Equipment> EquipmentList { get; set; }
    }
}
