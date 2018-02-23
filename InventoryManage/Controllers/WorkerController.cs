using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InventoryManage.Models;
using InventoryManage.Data;

namespace InventoryManage.Controllers
{
    public class WorkerController : Controller
    {
        private readonly CenterContext _context;
        private readonly UserManager<Worker> _userManager;
        private readonly RoleManager<CenterRole> _roleManager;

        public WorkerController(CenterContext context, UserManager<Worker> userManager, RoleManager<CenterRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Policy = "CanViewWorker")]
        public async Task<IActionResult> Index()
        {
            var rs = from wk in _context.Workers
                     from ur in _context.UserRoles
                     where ur.UserId == wk.Id
                     from r in _context.Roles
                     where r.Id == ur.RoleId
                     select new { Worker = wk, RoleName = r.Name };
            var workersQuery = from r in rs
                               group r by r.Worker into g
                               select new WorkerWithRole
                               {
                                   Worker = g.Key,
                                   Roles = g.Select(r => r.RoleName).ToList(),
                                   RoleList = _context.Roles.Select(r => new SelectListItem
                                   {
                                       Text = r.Name,
                                       Value = r.Name,
                                   }).ToList()
                               };

            var workerViewModel = new WorkerIndexViewModel
            {
                WorkerAdd = new WorkerWithRole
                {
                    RoleList = _context.Roles.Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Name,
                    }).ToList()
                },
                WorkerList = await PaginatedList<WorkerWithRole>.CreateAsync(workersQuery, 1, 10)
            };
            return View(workerViewModel);
        }

        [HttpGet]
        [Authorize(Policy = "CanViewWorker")]
        public async Task<IActionResult> WorkerList(int? page, string filter, int? pageSize)
        {
            var rs = from wk in _context.Workers
                     from ur in _context.UserRoles
                     where ur.UserId == wk.Id
                     from r in _context.Roles
                     where r.Id == ur.RoleId
                     select new { Worker = wk, RoleName = r.Name };
            var workersQuery = from r in rs
                               group r by r.Worker into g
                               select new WorkerWithRole
                               {
                                   Worker = g.Key,
                                   Roles = g.Select(r => r.RoleName).ToList(),
                                   RoleList = _context.Roles.Select(r => new SelectListItem
                                   {
                                       Text = r.Name,
                                       Value = r.Name,
                                   }).ToList()
                               };
            if (filter != null)
            {
                workersQuery = workersQuery.Where(wk => wk.Worker.Name.Contains(filter) || wk.Worker.UserName.Contains(filter));
            }
            return PartialView("WorkersPartial", await PaginatedList<WorkerWithRole>.CreateAsync(workersQuery, page ?? 1, pageSize ?? 10));
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartmentList()
        {
            var departmentQuery = (from w in _context.Workers select new { w.Department }).Distinct();
            return Json(await departmentQuery.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        [Authorize(Policy = "CanAddWorker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(WorkerWithRole workerWithRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _userManager.CreateAsync(workerWithRole.Worker, workerWithRole.NewPassword ?? "666666");
                if (result.Succeeded)
                {
                    foreach (var role in workerWithRole.Roles)
                    {
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(workerWithRole.Worker, role);
                    }                    
                }
            }            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "CanEditWorker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkerWithRole workerWithRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var worker = await _userManager.FindByIdAsync(workerWithRole.Worker.Id.ToString());
                    if (worker != null)
                    {                       
                        worker.UserName = workerWithRole.Worker.UserName;
                        worker.Name = workerWithRole.Worker.Name;
                        worker.Department = workerWithRole.Worker.Department;
                        worker.IsStaff = workerWithRole.Worker.IsStaff;                           
                        var result = await _userManager.UpdateAsync(worker);
                        if (result.Succeeded)
                        {
                            if (workerWithRole.NewPassword != null) // change password
                            {
                                result = await _userManager.RemovePasswordAsync(worker);
                                if (result.Succeeded)
                                {
                                    await _userManager.AddPasswordAsync(worker, workerWithRole.NewPassword);
                                }
                            }                           

                            // change roles
                            var existRoles = await _userManager.GetRolesAsync(workerWithRole.Worker);
                            var existRolesHS = new HashSet<string>(existRoles);
                            var changedRolesHS = new HashSet<string>(workerWithRole.Roles);
                            foreach (var role in _context.Roles.Select(r => r.Name).ToList())
                            {
                                if (changedRolesHS.Contains(role))
                                {
                                    if (!existRolesHS.Contains(role))
                                    {
                                        await _userManager.AddToRoleAsync(worker, role);
                                    }
                                }
                                else
                                {
                                    if (existRolesHS.Contains(role))
                                    {
                                        await _userManager.RemoveFromRoleAsync(worker, role);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "CanDeleteWorker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var worker = await _userManager.FindByIdAsync(Id.ToString());
                var result = await _userManager.DeleteAsync(worker);
            }
            catch (DbUpdateException /* ex */)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
    }
}