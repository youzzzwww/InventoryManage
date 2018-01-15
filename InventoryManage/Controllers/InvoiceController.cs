using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManage.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Approve()
        {
            return View();
        }
    }
}