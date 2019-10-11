using ADUC2.Models;
using ADUC2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADUC2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService service;

        public AccountController() : this(null) { }
        public AccountController(IAccountService service = null)
        {
            this.service = service ??
                new AccountService("TRR-INET.local", System.Web.HttpContext.Current.User.IsInRole("Domain Admins") ? "DC = TRR-INET,DC = LOCAL" : "OU = Brugere,DC = TRR-INET,DC = LOCAL");
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Details/5
        public ActionResult Details(string manr)
        {
            Account account = service.GetAccount(manr);

            if (account == null)
                return View("Error", new { message = "Kunne ikke finde kontoen" });
            else
                return View(account);
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Account account = new Account
                {
                    AccountName = collection["AccountName"]
                };

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult ValidateAccountName(string accountName)
        {
            return accountName == "370929" ? Json("MANR er allerede oprettet!", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePassword(string password, string accountName)
        {
            if (accountName.Length > 0 && password.ToLower().Contains(accountName.ToLower()))
                return Json("Password må ikke indeholde brugernavn!", JsonRequestBehavior.AllowGet);

            if (password.Length < 7)
                return Json("Password skal være minimum 7 tegn langt!", JsonRequestBehavior.AllowGet);

            int passwordContains = 0;

            //Check for uppercase letters
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]"))
                passwordContains++;

            //Check for lowercase letters
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]"))
                passwordContains++;

            //Check for digits
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                passwordContains++;

            //Check for special characters
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[^\d\w\n\r\t\v]"))
                passwordContains++;

            if (passwordContains < 3)
                return Json("Password minimum indeholde 3 af følgende type tegn: Store bogstaver, små bogstaver, tal, specialtegn", JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRandomPassword()
        {
            string password = new Helpers.PasswordGenerator(10, true, true, true, true).GeneratePassword();

            return Json(password, JsonRequestBehavior.AllowGet);
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
