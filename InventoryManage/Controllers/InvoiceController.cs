using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using InventoryManage.Models;
using InventoryManage.Data;

namespace InventoryManage.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly CenterContext _context;
        private readonly UserManager<Worker> _userManager;

        public InvoiceController(CenterContext context, UserManager<Worker> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Add(int? equipmentId)
        {
            var invoice = new Invoice();
            if (equipmentId != null)
            {
                var equipment = await _context.Equipments.AsNoTracking().SingleOrDefaultAsync(e => e.EquipmentId == equipmentId);
                if (equipment != null)
                {
                    invoice.EquipmentId = equipment.EquipmentId;
                    invoice.Equipment = equipment;
                }
            }
            ViewBag.CategoryList = await _context.Equipments.GroupBy(e => e.Category).Select(x => x.Key).ToListAsync();
            return View(invoice);
        }
        [HttpPost]
        public async Task<IActionResult> Add([Bind("EquipmentId", "Number", "Reason")] Invoice invoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    invoice.WorkerId = user.Id;
                    invoice.Date = DateTime.Now;

                    _context.Add(invoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error
                ModelState.AddModelError("", "无法保存更改，重试或联系系统管理员.");
            }
            return View(invoice);
        }

        public async Task<JsonResult> GetNames(string category)
        {
            var NameList = await _context.Equipments.Where(e => e.Category == category)
                .GroupBy(e => e.Name).Select(x => x.Key).ToListAsync();
            return Json(NameList);
        }
        public async Task<JsonResult> GetDetailsAndId(string category, string name)
        {
            var DetailList = await _context.Equipments.Where(e => e.Category == category && e.Name == name)
                .Select(x => new { Id = x.EquipmentId, Detail = x.Detail }).ToListAsync();
            return Json(DetailList);
        }

        public IActionResult Approve()
        {
            return View();
        }
    }
}