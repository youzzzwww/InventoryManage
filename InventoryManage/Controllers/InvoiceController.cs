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

        [HttpGet]
        public async Task<IActionResult> Approve()
        {
            var invoicesQuery = from iv in _context.Invoices
                                from wk in _context.Workers where wk.Id == iv.WorkerId
                                from ap in _context.Workers.Where(x => x.Id == iv.ApproverId).DefaultIfEmpty()
                                from eqp in _context.Equipments where eqp.EquipmentId == iv.EquipmentId
                                orderby iv.Date descending
                                select new ApproveInvoiceViewModel
                                {
                                    InvoiceId = iv.InvoiceId,
                                    Proposer = wk.Name,
                                    Type = eqp.Type,
                                    Category = eqp.Category,
                                    Name = eqp.Name,
                                    Number = iv.Number,
                                    RemainNumber = eqp.Number,
                                    Date = iv.Date,
                                    Reason = iv.Reason,
                                    Status = iv.Status,
                                    Approver = ap!=null ? ap.Name : "",
                                    ApproveReason = iv.ApproveReason
                                };
            return View(await PaginatedList<ApproveInvoiceViewModel>.CreateAsync(invoicesQuery, 1, 5));
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceList(int? page, string filter, int? pageSize, bool unhandled)
        {
            var invoicesQuery = from iv in _context.Invoices
                                from wk in _context.Workers
                                where wk.Id == iv.WorkerId
                                from ap in _context.Workers.Where(x => x.Id == iv.ApproverId).DefaultIfEmpty()
                                from eqp in _context.Equipments
                                where eqp.EquipmentId == iv.EquipmentId
                                orderby iv.Date descending
                                select new ApproveInvoiceViewModel
                                {
                                    InvoiceId = iv.InvoiceId,
                                    Proposer = wk.Name,
                                    Type = eqp.Type,
                                    Category = eqp.Category,
                                    Name = eqp.Name,
                                    Number = iv.Number,
                                    RemainNumber = eqp.Number,
                                    Date = iv.Date,
                                    Reason = iv.Reason,
                                    Status = iv.Status,
                                    Approver = ap != null ? ap.Name : "",
                                    ApproveReason = iv.ApproveReason
                                };
            if (unhandled)
            {
                invoicesQuery = invoicesQuery.Where(iv => iv.Status == InvoiceStatus.未处理);
            }
            if (filter != null)
            {
                invoicesQuery = invoicesQuery.Where(iv => iv.Name.Contains(filter) || iv.Proposer.Contains(filter));
            }
            return PartialView("InvoicesPartial", await PaginatedList<ApproveInvoiceViewModel>.CreateAsync(invoicesQuery, page ?? 1, pageSize ?? 10));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int InvoiceId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = await _context.Invoices.SingleOrDefaultAsync(i => i.InvoiceId == InvoiceId);
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    if (invoice != null)
                    {
                        invoice.Status = InvoiceStatus.通过;
                        invoice.ApproveDate = DateTime.Now;
                        invoice.ApproverId = user.Id;

                        var equipment = await _context.Equipments.SingleOrDefaultAsync(e => e.EquipmentId == invoice.EquipmentId);
                        if (equipment != null && equipment.Number > invoice.Number)
                        {
                            equipment.Number -= invoice.Number;
                            _context.Update(equipment);
                            _context.Update(invoice);
                        }                        
                        await _context.SaveChangesAsync();
                    }                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Approve");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("InvoiceId", "Type", "Category", "Name", "Detail", "Number", "ApproveReason")]
            ApproveInvoiceViewModel approveInvoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var equipment = await _context.Equipments.SingleOrDefaultAsync(e => e.Type == approveInvoiceViewModel.Type
                        && e.Category == approveInvoiceViewModel.Category && e.Name == approveInvoiceViewModel.Name && e.Detail == approveInvoiceViewModel.Detail);
                    if (equipment != null)
                    {
                        var invoice = await _context.Invoices.SingleOrDefaultAsync(i => i.InvoiceId == approveInvoiceViewModel.InvoiceId);
                        var user = await _userManager.GetUserAsync(HttpContext.User);
                        invoice.EquipmentId = equipment.EquipmentId;
                        invoice.Number = approveInvoiceViewModel.Number;
                        invoice.Status = InvoiceStatus.通过;
                        invoice.ApproveReason = approveInvoiceViewModel.ApproveReason;
                        invoice.ApproveDate = DateTime.Now;
                        invoice.ApproverId = user.Id;

                        if (equipment.Number > invoice.Number)
                        {
                            equipment.Number -= invoice.Number;
                            _context.Update(equipment);
                            _context.Update(invoice);
                        }
                        await _context.SaveChangesAsync();
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Approve");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuse(int InvoiceId, string ApproveReason)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = await _context.Invoices.SingleOrDefaultAsync(i => i.InvoiceId == InvoiceId);
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    invoice.Status = InvoiceStatus.拒绝;
                    invoice.ApproveReason = ApproveReason;
                    invoice.ApproveDate = DateTime.Now;
                    invoice.ApproverId = user.Id;

                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Approve");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int InvoiceId)
        {
            try
            {
                var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceId == InvoiceId);
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                throw;
            }
            return RedirectToAction("Approve");
        }
    }
}