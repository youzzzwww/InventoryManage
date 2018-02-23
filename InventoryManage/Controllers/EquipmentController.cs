using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InventoryManage.Models;
using InventoryManage.Data;

namespace InventoryManage.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly CenterContext _context;

        public EquipmentController(CenterContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "CanViewEquipment")]
        public async Task<IActionResult> Store()
        {
            var equipmentsQuery = from eqp in _context.Equipments select eqp;
            var equipmentViewModel = new EquipmentIndexViewModel
            {
                EquipmentAdd = new Equipment(),
                EquipmentList = await PaginatedList<Equipment>.CreateAsync(equipmentsQuery, 1, 10)
            };
            return View(equipmentViewModel);
        }

        [HttpGet]
        [Authorize(Policy = "CanViewEquipment")]
        public async Task<IActionResult> EquipmentList(int? page, string filter, int? pageSize)
        {
            var equipmentsQuery = from eqp in _context.Equipments select eqp;
            if (filter != null)
            {
                equipmentsQuery = equipmentsQuery.Where(eqp => eqp.Name.Contains(filter));
            }
            return PartialView("EquipmentsPartial", await PaginatedList<Equipment>.CreateAsync(equipmentsQuery, page??1, pageSize??10));
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoryList(string type)
        {
            var categoryQuery = (from c in _context.Equipments
                                 where c.Type == (EquipmentType)Enum.Parse(typeof(EquipmentType), type)
                                 orderby c.Category ascending
                                 select new { c.Category }).Distinct();           
            return Json(await categoryQuery.AsNoTracking().ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetNameList(string type, string category)
        {
            var nameQuery = (from c in _context.Equipments
                             where c.Category == category & c.Type == (EquipmentType)Enum.Parse(typeof(EquipmentType), type)
                             orderby c.Name ascending
                             select new { c.Name }).Distinct();
            return Json(await nameQuery.AsNoTracking().ToListAsync());
        }
        [HttpGet]
        public async Task<JsonResult> GetDetailList(string type, string category, string name)
        {
            var detailQuery = (from c in _context.Equipments
                             where c.Category == category & c.Name == name &
                                c.Type == (EquipmentType)Enum.Parse(typeof(EquipmentType), type)
                             orderby c.Detail ascending
                             select new { c.Detail }).Distinct();
            return Json(await detailQuery.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        [Authorize(Policy = "CanEditEquipment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Store");
        }

        [HttpPost]
        [Authorize(Policy = "CanAddEquipment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EquipmentIndexViewModel equipmentIndex)
        {          
            if (ModelState.IsValid)
            {
                var equipment = equipmentIndex.EquipmentAdd;
                try
                {
                    var existEqp = await _context.Equipments.SingleOrDefaultAsync(e => e.Category == equipment.Category & e.Type == equipment.Type
                        & e.Name == equipment.Name & e.Detail == equipment.Detail);
                    if (existEqp == null)
                    {
                        _context.Add(equipment);
                    }
                    else
                    {
                        existEqp.Number += equipment.Number;
                        _context.Update(existEqp);
                    }                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }                
            }
            return RedirectToAction("Store");
        }

        [HttpPost]
        [Authorize(Policy = "CanDeleteEquipment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int EquipmentId)
        {           
            try
            {
                var equipment = await _context.Equipments.SingleOrDefaultAsync(m => m.EquipmentId == EquipmentId);
                _context.Equipments.Remove(equipment);                 
                await _context.SaveChangesAsync();               
            }
            catch (DbUpdateException /* ex */)
            {
                throw;
            }
            return RedirectToAction("Store");
        }
    }
}