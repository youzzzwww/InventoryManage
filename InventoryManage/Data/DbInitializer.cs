using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using InventoryManage.Models;
using InventoryManage.Authorization;

namespace InventoryManage.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<CenterContext>();
            var userManager = serviceProvider.GetService<UserManager<Worker>>();
            var roleManager = serviceProvider.GetService<RoleManager<CenterRole>>();

            context.Database.EnsureCreated();
            // Look for any workers existed.
            if (context.Workers.Any())
            {
                return;
            }

            // Roles and Claims
            var roles = new Dictionary<string, CenterRole>
            {
                { Constant.Admin, new CenterRole{Name=Constant.Admin} },
                { Constant.EquipmentManager, new CenterRole{Name=Constant.EquipmentManager} },
                { Constant.WorkerManager, new CenterRole{Name=Constant.WorkerManager} },
                { Constant.Approver, new CenterRole{Name=Constant.Approver} },
                { Constant.NormalUser, new CenterRole{Name=Constant.NormalUser} }
            };
            foreach (var r in roles)
            {
                await roleManager.CreateAsync(r.Value);
            }

            var EquipmentClaims = new Dictionary<string, Claim>
            {
                {"View", new Claim(Constant.EquipmentViewClaim, Constant.EquipmentViewClaim) },
                {"Add", new Claim(Constant.EquipmentAddClaim,Constant.EquipmentAddClaim)},
                {"Edit", new Claim(Constant.EquipmentEditClaim, Constant.EquipmentEditClaim)},
                {"Delete", new Claim(Constant.EquipmentDeleteClaim, Constant.EquipmentDeleteClaim)}
            };
            var InvoiceClaims = new Dictionary<string, Claim>
            {
                {"View", new Claim(Constant.InvoiceViewClaim, Constant.InvoiceViewClaim) },
                {"Add", new Claim(Constant.InvoiceAddClaim,Constant.InvoiceAddClaim)},
                {"Edit", new Claim(Constant.InvoiceEditClaim, Constant.InvoiceEditClaim)},
                {"Delete", new Claim(Constant.InvoiceDeleteClaim, Constant.InvoiceDeleteClaim)}
            };
            var WorkerClaims = new Dictionary<string, Claim>
            {
                {"View", new Claim(Constant.WorkerViewClaim, Constant.WorkerViewClaim) },
                {"Add", new Claim(Constant.WorkerAddClaim,Constant.WorkerAddClaim)},
                {"Edit", new Claim(Constant.WorkerEditClaim, Constant.WorkerEditClaim)},
                {"Delete", new Claim(Constant.WorkerDeleteClaim, Constant.WorkerDeleteClaim)}
            };
            foreach (var c in EquipmentClaims)
            {
                await roleManager.AddClaimAsync(roles[Constant.Admin], c.Value);
                await roleManager.AddClaimAsync(roles[Constant.EquipmentManager], c.Value);
            }
            foreach (var c in InvoiceClaims)
            {
                await roleManager.AddClaimAsync(roles[Constant.Admin], c.Value);
                await roleManager.AddClaimAsync(roles[Constant.Approver], c.Value);
            }
            foreach (var c in WorkerClaims)
            {
                await roleManager.AddClaimAsync(roles[Constant.Admin], c.Value);
                await roleManager.AddClaimAsync(roles[Constant.WorkerManager], c.Value);
            }
            await roleManager.AddClaimAsync(roles[Constant.NormalUser], InvoiceClaims["Add"]);


            // User and add to role
            const string default_passwd = "666666";
            var workers = new Worker[]
            {
                new Worker{UserName="xiesongbo",Name="谢松波",EnrollmentDate=DateTime.Parse("2017-06-01"),Department="信息部",IsStaff=true},
                new Worker{UserName="maiweixian",Name="麦伟贤",EnrollmentDate=DateTime.Parse("2007-06-01"),Department="信息部",IsStaff=true},
                new Worker{UserName="chenjinyuan",Name="陈劲源",EnrollmentDate=DateTime.Parse("2007-06-01"),Department="信息部",IsStaff=true},
                new Worker{UserName="xiaosimin",Name="肖思敏",EnrollmentDate=DateTime.Now,Department="办公室",IsStaff=true}
            };
            foreach (Worker w in workers)
            {
                await userManager.CreateAsync(w, default_passwd);
            }
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "xiesongbo"), Constant.Admin);
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "maiweixian"), Constant.Approver);
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "chenjinyuan"), Constant.EquipmentManager);
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "xiaosimin"), Constant.NormalUser);

            // other data initial
            var equipments = new Equipment[]
            {
                new Equipment{Type=EquipmentType.固定资产, Category="台式机", Name="Dell optiplex 7040", Detail="内存:4G;CPU:i7", Number=5},
                new Equipment{Type=EquipmentType.固定资产, Category="台式机", Name="Dell GX520", Detail="内存:2G", Number=2},
                new Equipment{Type=EquipmentType.固定资产, Category="打印机",Name="HP Color LaserJet CP5520", Detail="", Number=0},
                new Equipment{Type=EquipmentType.耗材, Category="硒鼓",Name="HP LaserJet 05A", Detail="", Number=100}
            };
            foreach (Equipment e in equipments)
            {
                context.Equipments.Add(e);
            }
            await context.SaveChangesAsync();

            var invoices = new Invoice[]
            {
                new Invoice{WorkerId=workers.Single(w => w.UserName=="xiesongbo").Id, Date=DateTime.Parse("2017-08-01"), Number =1, Status=InvoiceStatus.通过,
                    Reason="新机测试", EquipmentId=equipments.Single(e => e.Name=="Dell optiplex 7040").EquipmentId,
                    ApproverId=workers.Single(w => w.UserName=="xiesongbo").Id, ApproveDate=DateTime.Parse("2017-08-01")},
                new Invoice{WorkerId=workers.Single(w => w.UserName=="xiesongbo").Id, Date=DateTime.Parse("2017-06-01"), Number =1, Status=InvoiceStatus.通过,
                    Reason="原始机器", EquipmentId=equipments.Single(e => e.Name=="Dell optiplex 7040").EquipmentId,
                    ApproverId=workers.Single(w => w.UserName=="xiesongbo").Id, ApproveDate=DateTime.Parse("2017-06-01")},
                new Invoice{WorkerId=workers.Single(w => w.UserName=="xiesongbo").Id, Date=DateTime.Now, Number =1, Status=InvoiceStatus.拒绝,
                    Reason="原始机器", EquipmentId=equipments.Single(e => e.Name=="Dell optiplex 7040").EquipmentId,
                    ApproverId=workers.Single(w => w.UserName=="xiesongbo").Id, ApproveReason="库存不足", ApproveDate=DateTime.Now},
                new Invoice{WorkerId=workers.Single(w => w.UserName=="xiesongbo").Id, Date=DateTime.Parse("2018-01-01"), Number =1, Status=InvoiceStatus.通过,
                    Reason="", EquipmentId=equipments.Single(e => e.Name=="HP LaserJet 05A").EquipmentId,
                    ApproverId=workers.Single(w => w.UserName=="xiesongbo").Id, ApproveDate=DateTime.Parse("2018-01-01")},
                new Invoice{WorkerId=workers.Single(w => w.UserName=="xiesongbo").Id, Date=DateTime.Now,Number =1, Status=InvoiceStatus.未处理,
                    EquipmentId=equipments.Single(e => e.Name=="HP LaserJet 05A").EquipmentId},
                new Invoice{WorkerId=workers.Single(w => w.UserName=="maiweixian").Id, Date=DateTime.Now,Number =1, Status=InvoiceStatus.未处理,
                    EquipmentId=equipments.Single(e => e.Name=="HP LaserJet 05A").EquipmentId}
            };
            foreach (Invoice r in invoices)
            {
                context.Invoices.Add(r);
            }
            await context.SaveChangesAsync();
        }
    }
}
