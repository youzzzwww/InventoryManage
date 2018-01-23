using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using InventoryManage.Models;

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

            var roles = new CenterRole[]
            {
                new CenterRole{Name="Admin"},
                new CenterRole{Name="Manager"},
                new CenterRole{Name="Customer"}
            };
            foreach (var r in roles)
            {
                await roleManager.CreateAsync(r);
            }

            const string default_passwd = "666666";
            var workers = new Worker[]
            {
                new Worker{UserName="xiesongbo",Name="谢松波",EnrollmentDate=DateTime.Parse("2017-06-01"),Department="信息部"},
                new Worker{UserName="maiweixian",Name="麦伟贤",EnrollmentDate=DateTime.Parse("2007-06-01"),Department="信息部"},
                new Worker{UserName="xiaosimin",Name="肖思敏",EnrollmentDate=DateTime.Now,Department="办公室"}
            };
            foreach (Worker w in workers)
            {
                await userManager.CreateAsync(w, default_passwd);
            }
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "xiesongbo"), "Admin");
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "maiweixian"), "Manager");
            await userManager.AddToRoleAsync(workers.Single(w => w.UserName == "xiaosimin"), "Customer");

            var equipments = new Equipment[]
            {
                new Equipment{Type=EquipmentType.固定资产, Category="台式机", Name="Dell optiplex 7040", Detail="内存:4G;CPU:i7", Number=5},              
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
