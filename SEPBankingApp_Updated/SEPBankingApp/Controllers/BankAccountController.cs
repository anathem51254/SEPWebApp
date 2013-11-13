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
using PagedList;

namespace SEPBankingApp.Controllers
{
    [HandleError]
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
            BankAccount bankaccount = db.BankAccounts.Find(id);
            if (bankaccount == null)
            {
                //return HttpNotFound(); 
                return RedirectToAction("Index");
            }

            if(bankaccount.BlockAccess)
            {
                return RedirectToAction("Index");
            }

            return View(bankaccount);
        }

        /// Bug in this section of code due to CurrentBankAccount not setting properly
        // GET: /BankAccount/TransactionHistory/5
        public ActionResult TransactionHistory(int? id, int? page)
        {
            string userid = User.Identity.GetUserId();

            var getBankAccountDetails = from tb in db.BankAccounts
                                     where tb.AccountNumber == id && tb.UserId == userid
                                     select tb;

            if(getBankAccountDetails.Count() < 1)
            {
                return RedirectToAction("Index");
            }

            if (page == null)
                page = 1;

            if (id == null)
            {
                var BankAccountDetails = from tb in db.BankAccounts
                                         where tb.AccountNumber == CurrentBankAccount
                                         select tb;

                if(BankAccountDetails.Count() > 0)
                {
                    if (BankAccountDetails.First().BlockAccess)
                    {
                        return RedirectToAction("Index");
                    }
                }

                var transHistory = from tb in db.TransactionHistorys
                                   where tb.AccountNumber == CurrentBankAccount
                                   select tb;

                if (transHistory == null)
                {
                    return View();
                    //return HttpNotFound();
                }

                IOrderedQueryable<TransactionHistory> OrderedTransHistory = transHistory.OrderBy(s => s.AccountNumber);

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(OrderedTransHistory.ToPagedList(pageNumber, pageSize));
            }
            else if (id != null && CurrentBankAccount == 0)
            {
                var BankAccountDetails = from tb in db.BankAccounts
                                         where tb.AccountNumber == id
                                         select tb;

                if (BankAccountDetails.Count() > 0)
                {
                    if (BankAccountDetails.First().BlockAccess)
                    {
                        ModelState.AddModelError("", "Access to this Account is Currently Blocked!");
                        return RedirectToAction("Index");
                    }
                }

                var transHistory = from tb in db.TransactionHistorys
                                   where tb.AccountNumber == id
                                   select tb;

                if (transHistory == null)
                {
                    return View();
                    //return HttpNotFound();
                }

                CurrentBankAccount = (int)id;

                IOrderedQueryable<TransactionHistory> OrderedTransHistory = transHistory.OrderBy(s => s.AccountNumber);

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(OrderedTransHistory.ToPagedList(pageNumber, pageSize));
            }
            else //if (id == null && CurrentBankAccount == 0)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
        }

        // GET: /BankAccount/MakeTransaction
        public ActionResult MakeTransaction()
        {
            var BankAccountDetails = from tb in db.BankAccounts
                                     select tb;

            string userid = User.Identity.GetUserId();
            BankAccountDetails = BankAccountDetails.Where(s => s.UserId.Equals(userid)).Where(s => s.BlockAccess.Equals(false));

            ViewBag.BankAccountNumbersList = new SelectList(BankAccountDetails, "AccountNumber", "AccountNumber");

            TransactionHistory model = new TransactionHistory();

            model.TransactionDateTime = DateTime.Today.Date;

            return View(model);
        }

        // POST: /BankAccount/MakeTransaction
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeTransaction([Bind(Include="AccountNumber,DestinationAccountNumber,TransactionDateTime,TransactionAmount,Description")] TransactionHistory transHistory)
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

                if (AccToWithdraw == null || AccToDeposit == null || AccToWithdraw.BlockAccess == true || AccToWithdraw.CurrentBalance <= transHistory.TransactionAmount || transHistory.TransactionAmount <= 0)
                {
                    if(AccToWithdraw.BlockAccess == true)
                    {
                        return View(HandleExceptionMakeTransaction(transHistory, "Access to this Account is Blocked!"));
                    }

                    if (AccToWithdraw.CurrentBalance <= transHistory.TransactionAmount || transHistory.TransactionAmount <= 0)
                    {
                        return View(HandleExceptionMakeTransaction(transHistory, "There is not enough money in this Account!"));
                    }

                    return View(HandleExceptionMakeTransaction(transHistory, "Invalid Account is Selected!"));
                }

                ProcessMakeTransaction(transHistory, AccToDeposit, AccToWithdraw);


                db.TransactionHistorys.Add(transHistory);
                db.SaveChanges();
                return RedirectToAction("TransactionHistory", new { id = transHistory.AccountNumber });
            }

            var GetBankAccountDetails = from tb in db.BankAccounts
                                        select tb;

            string Getuserid = User.Identity.GetUserId();
            GetBankAccountDetails = GetBankAccountDetails.Where(s => s.UserId.Equals(Getuserid)).Where(s => s.BlockAccess.Equals(false));

            ViewBag.BankAccountNumbersList = new SelectList(GetBankAccountDetails, "AccountNumber", "AccountNumber");

            return View(transHistory);
        }

        // GET: /BankAccount/Create
        public ActionResult Create()
        {
            List<string> BankAccountTypes = new List<string>();

            BankAccountTypes.Add("Saver");
            BankAccountTypes.Add("Access");

            ViewBag.BankAccountTypeList = new SelectList(BankAccountTypes);

            return View();
        }

        // POST: /BankAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="AccountNumber,UserId,AccountType,CurrentBalance")] BankAccount bankaccount)
        {
            if (ModelState.IsValid)
            {
                bankaccount.CurrentBalance = 100;
                bankaccount.BlockAccess = false;

                bankaccount.UserId = User.Identity.GetUserId();

                db.BankAccounts.Add(bankaccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankaccount);
        }

        //// GET: /BankAccount/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BankAccount bankaccount = db.BankAccounts.Find(id);
        //    if (bankaccount == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bankaccount);
        //}

        //// POST: /BankAccount/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="AccountNumber,UserId,AccountType,CurrentBalance")] BankAccount bankaccount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(bankaccount).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(bankaccount);
        //}




        //
        // GET: /BankAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankaccount = db.BankAccounts.Find(id);
            if (bankaccount == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }
            return View(bankaccount);
        }

        // POST: /BankAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankaccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankaccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region TRANSACTION HELPERS

        protected TransactionHistory HandleExceptionMakeTransaction(TransactionHistory transHistory, string exceptionMsg)
        {
            ModelState.AddModelError("", exceptionMsg);

            var BankAccountDetails = from tb in db.BankAccounts
                                     select tb;

            string userid = User.Identity.GetUserId();
            BankAccountDetails = BankAccountDetails.Where(s => s.UserId.Equals(userid)).Where(s => s.BlockAccess.Equals(false));

            ViewBag.BankAccountNumbersList = new SelectList(BankAccountDetails, "AccountNumber", "AccountNumber");

            return transHistory;
        }

        protected void ProcessMakeTransaction(TransactionHistory transHistory, BankAccount AccToDeposit, BankAccount AccToWithdraw)
        {
            TransactionHistory AccHistory = new Models.TransactionHistory();

            AccHistory.AccountNumber = transHistory.DestinationAccountNumber;
            AccHistory.DestinationAccountNumber = transHistory.AccountNumber;
            AccHistory.TransactionAmount = transHistory.TransactionAmount;
            //AccHistory.TransactionDateTime = transHistory.TransactionDateTime;
            AccHistory.TransactionDateTime = DateTime.Today.Date;
            AccHistory.PreBalance = AccToDeposit.CurrentBalance;
            AccHistory.Description = transHistory.Description;

            transHistory.PreBalance = AccToWithdraw.CurrentBalance;
            Debug.WriteLine("Withdraw Acc Pre Balance: " + AccToWithdraw.CurrentBalance.ToString());

            Debug.WriteLine("Deposit Acc Pre Balance: " + AccToDeposit.CurrentBalance.ToString());

            AccToWithdraw.CurrentBalance -= transHistory.TransactionAmount;
            Debug.WriteLine("Withdraw Acc Post Balance: " + AccToWithdraw.CurrentBalance.ToString());

            AccToDeposit.CurrentBalance += transHistory.TransactionAmount;
            Debug.WriteLine("Deposit Acc Post Balance: " + AccToDeposit.CurrentBalance.ToString());

            AccHistory.PostBalance = AccToDeposit.CurrentBalance;

            transHistory.PostBalance = AccToWithdraw.CurrentBalance;

            transHistory.TransactionDateTime = DateTime.Today.Date;

            db.Entry(AccToWithdraw).State = EntityState.Modified;
            db.SaveChanges();

            db.Entry(AccToDeposit).State = EntityState.Modified;
            db.SaveChanges();

            db.TransactionHistorys.Add(AccHistory);
            db.SaveChanges();

            //db.TransactionHistorys.Add(transHistory);
            //db.SaveChanges();
        }

        #endregion
    }
}
