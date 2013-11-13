using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SEPBankingApp.Models;
using System.Data.Entity;

namespace SEPBankingApp.Controllers
{
    [HandleError]
    [Authorize(Roles = "GeneralAdmin")]
    public class AdminManagementController : Controller
    {
        public AdminManagementController()
        {
            db = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }
        
        public AdminManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationDbContext db { get; private set; }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<IdentityRole> RoleManager { get; private set; }

        //
        // GET: /AdminManagement/
        public ActionResult Index()
        {
            return View();
        }

        #region ROLES

        //
        // GET: /AdminManagement/Roles
        public ActionResult Roles()
        {
            return View(RoleManager.Roles);
        }

        //
        // GET: /AdminManagement/CreateRole
        public ActionResult CreateRole(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /AdminManagement/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole([Bind(Include="Name")] RoleManagement roleModel)
        {
            if(ModelState.IsValid)
            {
                IdentityResult ir;

                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                if(RoleManager.RoleExists(roleModel.Name))
                {
                    return View();
                }

                ir = rm.Create(new IdentityRole(roleModel.Name));

                return RedirectToAction("Roles", "AdminManagement");
            }

            return View();
        }

        //
        // GET: /AdminManagement/DeleteRole/5
        public ActionResult DeleteRole(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = RoleManager.FindById(id);
            if(role == null)
            {
                return RedirectToAction("Index");
                //return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /AdminManagement/DeleteRole/5
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if(ModelState.IsValid)
            {
                if(id == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return RedirectToAction("Index");
                }

                var role = RoleManager.FindById(id);
                var result = RoleManager.Delete(role);

                if(!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("Roles");
            }

            return View();
        } 

        #endregion

        #region USERS

        //
        // GET: /AdminManagement/Users
        public ActionResult Users()
        {
            return View(UserManager.Users.ToList());
        }

        //
        // GET: /AdminManagement/Users/Details/5
        public ActionResult UserDetails(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindById(id);

            var getbankAccount = from tb in db.BankAccounts
                              select tb;

            IQueryable<BankAccount> BankAccounts = getbankAccount.Where(s => s.UserId.Equals(id));

            var userRoles = UserManager.GetRoles(user.Id);

            UserDetailsViewModels userDetails = new UserDetailsViewModels();

            userDetails.AppUser = user;
            userDetails.BankAccount = BankAccounts;
            userDetails.Roles = userRoles;

            return View(userDetails);
        }

        //
        // GET: /AdminManagement/Users/CreateUser
        public ActionResult CreateUser()
        {
            ViewBag.RoleId = new SelectList(RoleManager.Roles.ToList(), "Id", "Name");
            return View();
        }

        //
        // POST: /AdminManagement/Users/Create
        [HttpPost]
        public ActionResult CreateUser(RegisterViewModel userViewModel, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();

                user.UserName = userViewModel.UserName;

                var adminResult = UserManager.Create(user, userViewModel.Password);

                if (adminResult.Succeeded)
                {
                    if (!String.IsNullOrEmpty(RoleId))
                    {
                        var role = RoleManager.FindById(RoleId);
                        var result = UserManager.AddToRole(user.Id, role.Name);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());

                            ViewBag.RoleId = new SelectList(RoleManager.Roles.ToList(), "Id", "Name");

                            return View();
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError("", adminResult.Errors.First().ToString());

                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");

                    return View();
                }
            }
            
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            return View();
        }

        //
        // GET: /AdminManagement/Users/Edit/1
        public ActionResult EditUser(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");

            var user = UserManager.FindById(id);
            if(user == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        //
        // POST: /AdminManagement/Users/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "UserName,Id")] ApplicationUser formuser, string id, string RoleId)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");

            var user = UserManager.FindById(id);
            user.UserName = formuser.UserName;

            if(ModelState.IsValid)
            {
                UserManager.Update(user);

                var rolesForUser = UserManager.GetRoles(id);
                if(rolesForUser.Count() > 0)
                {
                    foreach(var item in rolesForUser)
                    {
                        var result = UserManager.RemoveFromRole(id, item);
                    }
                }

                if(!String.IsNullOrEmpty(RoleId))
                {
                    var role = RoleManager.FindById(RoleId);

                    var result = UserManager.AddToRole(id, role.Name);
                    if(!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                        return View();
                    }
                }
                return RedirectToAction("Users");
            }

            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            return View();
        }

        // GET: /AdminManagement/EditUserBankAccount/5
        public ActionResult EditUserBankAccount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankaccount = db.BankAccounts.Find(id);
            if (bankaccount == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }
            return View(bankaccount);
        }

        // POST: /AdminManagement/EditUserBankAccount/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserBankAccount([Bind(Include = "AccountNumber,UserId,AccountType,CurrentBalance,BlockAccess")] BankAccount bankaccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankaccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankaccount);
        }

        // GET: /AdminManagement/DeleteUser/5
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                //return RedirectToAction("Index");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
                //return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return View();
                }

                var user = db.Users.Find(id);

                //var logins = user.Logins;
                //foreach(var login in logins)
                //{
                //    db.UserLogins.Remove(login);
                //}

                var rolesForUser = UserManager.GetRoles(user.Id);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser)
                    {
                        var result = UserManager.RemoveFromRole(user.Id, item);
                    }
                }

                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                return View();
        }

        #endregion
    }
}