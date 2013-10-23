using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SEPBankingApp.Models;
using System.Diagnostics;

namespace SEPBankingApp.Controllers
{
    [Authorize]
    public class BankAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int CurrentBankAccount = 0;

        // GET: /BankAccount/
        public ActionResult Index()
        {
            var BankAccountDetails = from tb in db.BankAccounts
                                     select tb;

            string userid = User.Identity.GetUserId();
            BankAccountDetails = BankAccountDetails.Where(s => s.UserId.Equals(userid));

            CurrentBankAccount = 0;

            return View(BankAccountDetails);
        }

        // GET: /BankAccount/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankaccountmodel = db.BankAccounts.Find(id);
            if (bankaccountmodel == null)
            {
                return HttpNotFound();
            }
            return View(bankaccountmodel);
        }
        
        // GET: /BankAccount/TransactionHistory/5
        public ActionResult TransactionHistory(int? id)
        {
            if (id == null)
            {
                var transHistory = from tb in db.TransactionHistorys
                                   where tb.AccountNumber == CurrentBankAccount
                                   select tb;

                if (transHistory == null)
                {
                    return View();
                    //return HttpNotFound();
                }

                return View(transHistory);
            }
            else if (id != null && CurrentBankAccount == 0)
            {
                var transHistory = from tb in db.TransactionHistorys
                                   where tb.AccountNumber == id
                                   select tb;

                if (transHistory == null)
                {
                    return View();
                    //return HttpNotFound();
                }

                CurrentBankAccount = (int)id;

                return View(transHistory);
            }
            else //if (id == null && CurrentBankAccount == 0)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return HttpNotFound();
            }
        }

        // GET: /BankAccount/MakeTransaction
        public ActionResult MakeTransaction()
        {
            var BankAccountDetails = from tb in db.BankAccounts
                                     select tb;

            string userid = User.Identity.GetUserId();
            BankAccountDetails = BankAccountDetails.Where(s => s.UserId.Equals(userid));

            ViewBag.BankAccountNumbersList = new SelectList(BankAccountDetails, "AccountNumber", "AccountNumber");

            return View();
        }

        // POST: /BankAccount/MakeTransaction
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeTransaction(TransactionHistory transHistory)
        {
            if (ModelState.IsValid)
            {
                var AccountsToWithdraw = from tb in db.BankAccounts
                                    where tb.AccountNumber == transHistory.AccountNumber
                                    select tb;

                var AccountsToDeposit = from tb in db.BankAccounts
                                   where tb.AccountNumber == transHistory.DestinationAccountNumber
                                   select tb;

                BankAccount AccToWithdraw = AccountsToWithdraw.FirstOrDefault();
                BankAccount AccToDeposit = AccountsToDeposit.FirstOrDefault();

                transHistory.PreBalance = AccToWithdraw.CurrentBalance;
                Debug.WriteLine("Withdraw Acc Pre Balance: " + AccToWithdraw.CurrentBalance.ToString());

                Debug.WriteLine("Deposit Acc Pre Balance: " + AccToDeposit.CurrentBalance.ToString());

                AccToWithdraw.CurrentBalance -= transHistory.TransactionAmount;
                Debug.WriteLine("Withdraw Acc Post Balance: " + AccToWithdraw.CurrentBalance.ToString());

                AccToDeposit.CurrentBalance += transHistory.TransactionAmount;
                Debug.WriteLine("Deposit Acc Post Balance: " + AccToDeposit.CurrentBalance.ToString());

                db.Entry(AccToWithdraw).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(AccToDeposit).State = EntityState.Modified;
                db.SaveChanges();

                transHistory.PostBalance = AccToWithdraw.CurrentBalance;

                db.TransactionHistorys.Add(transHistory);
                db.SaveChanges();
                return RedirectToAction("TransactionHistory", new { id = transHistory.AccountNumber });
            }

            return View(transHistory);
        }

        // GET: /BankAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /BankAccount/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BankAccount bankaccountmodel)
        {
            if (ModelState.IsValid)
            {
                bankaccountmodel.UserId = User.Identity.GetUserId();
                db.BankAccounts.Add(bankaccountmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            } 

            return View(bankaccountmodel);
        }

        // GET: /BankAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankaccountmodel = db.BankAccounts.Find(id);
            if (bankaccountmodel == null)
            {
                return HttpNotFound();
            }
            return View(bankaccountmodel);
        }

        // POST: /BankAccount/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankAccount bankaccountmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankaccountmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankaccountmodel);
        }

        // GET: /BankAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankaccountmodel = db.BankAccounts.Find(id);
            if (bankaccountmodel == null)
            {
                return HttpNotFound();
            }
            return View(bankaccountmodel);
        }

        // POST: /BankAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankaccountmodel = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankaccountmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
