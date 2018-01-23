using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InventoryManage.Models;
using InventoryManage.Data;

namespace InventoryManage.Controllers
{
    public class HomeController : Controller
    {
        private readonly CenterContext _context;
        private readonly UserManager<Worker> _userManager;
        private readonly SignInManager<Worker> _signInManager;

        public HomeController(CenterContext context, UserManager<Worker> userManager, SignInManager<Worker> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var assetsQuery = from iv in _context.Invoices.Where(i => i.WorkerId == user.Id && i.Status == InvoiceStatus.通过)
                                group iv by iv.EquipmentId into ivg
                              from eqp in _context.Equipments.Where(e => e.EquipmentId == ivg.Key && e.Type == EquipmentType.固定资产)
                         select new AssetViewModel
                         {
                             EquipmentId = eqp.EquipmentId,
                             Category = eqp.Category,
                             Name = eqp.Name,
                             Detail = eqp.Detail,
                             Number = ivg.Sum(x => x.Number),
                             Date = ivg.Max(x => x.Date)
                         };
            var consumablesQuery = from iv in _context.Invoices.Where(i => i.WorkerId == user.Id && i.Status == InvoiceStatus.通过)
                                   group iv by iv.EquipmentId into ivg
                                   from eqp in _context.Equipments.Where(e => e.EquipmentId == ivg.Key && e.Type == EquipmentType.耗材)
                                   select new ConsumableViewModel
                                   {
                                       EquipmentId = eqp.EquipmentId,
                                       Category = eqp.Category,
                                       Name = eqp.Name,
                                       Number = ivg.Sum(x => x.Number),
                                       Date = ivg.Max(x => x.Date)
                                   };
            var invoiceQuery = from iv in _context.Invoices.Where(i => i.WorkerId == user.Id)
                               from eqp in _context.Equipments.Where(e => e.EquipmentId == iv.EquipmentId)
                               select new InvoiceViewModel
                               {
                                   Category = eqp.Category,
                                   Name = eqp.Name,
                                   Number = iv.Number,
                                   Date = iv.Date,
                                   Reason = iv.Reason,
                                   Status = iv.Status,
                                   ApproveReason = iv.ApproveReason
                               };
            var personalIndexViewModel = new PersonalIndexViewModel
            {
                AssetViewModel = await assetsQuery.ToListAsync(),
                ConsumableViewModel = await consumablesQuery.ToListAsync(),
                InvoiceList = await PaginatedList<InvoiceViewModel>.CreateAsync(invoiceQuery, 1, 3)
            };
            return View(personalIndexViewModel);
        }

        public async Task<IActionResult> InvoiceList(int? page, int? pageSize)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var invoiceQuery = from iv in _context.Invoices.Where(i => i.WorkerId == user.Id)
                               from eqp in _context.Equipments.Where(e => e.EquipmentId == iv.EquipmentId)
                               select new InvoiceViewModel
                               {
                                   Category = eqp.Category,
                                   Name = eqp.Name,
                                   Number = iv.Number,
                                   Date = iv.Date,
                                   Reason = iv.Reason,
                                   Status = iv.Status,
                                   ApproveReason = iv.ApproveReason
                               };
            var invoiceList = await PaginatedList<InvoiceViewModel>.CreateAsync(invoiceQuery, page??1, pageSize??10);
            return PartialView("PersonalInvoicePartial", invoiceList);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        // GET: /Home/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return RedirectToAction(nameof(HomeController.Index));
            }
            return View();
        }
        // POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    if (result.IsLockedOut)
                    {
                        return View("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "不合法的登录.");
                        return View(model);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Home/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
