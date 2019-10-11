using ADUC2.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ADUC2.Services
{
    public interface IAccountService
    {
        void Create(Account account);
        void Delete(string accountName);
        void Update(Account account);
        Account GetAccount(string accountName);
    }

    public class AccountService : IAccountService
    {
        private readonly string domain;
        private readonly string container;

        public AccountService(string domain, string container)
        {
            this.domain = domain;
            this.container = container;
        }

        public void Create(Account account)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, container))
                using (UserPrincipal up = new UserPrincipal(pc, account.AccountName, account.Password, account.Enabled))
                {
                    up.GivenName = account.FirstName;
                    up.UserPrincipalName = $"{account.AccountName}@{domain}";
                    up.Surname = account.LastName;
                    up.AccountExpirationDate = account.Expires;
                    up.DisplayName = account.AccountName;
                    up.Save();

                    if (account.PasswordExpired)
                        up.ExpirePasswordNow();
                }
                
            }
            catch (PrincipalExistsException)
            {
                throw new AppException("Kontoen findes i forvejen.");
            }
            catch (PasswordException)
            {
                throw new AppException("Passwordet opfylder ikke domænets krav til kompleksitet.");
            }
            catch (Exception)
            {
                throw new AppException("Der skete en fejl ved oprettelsen af kontoen.");
            }
        }

        public Account GetAccount(string accountName)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, container))
            using (UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, accountName))
            {
                if (up == null)
                    return null;

                return new Account
                {
                    AccountName = up.SamAccountName,
                    Enabled = up.Enabled ?? true,
                    Expires = up.AccountExpirationDate,
                    FirstName = up.GivenName,
                    LastName = up.Surname,
                    LastLogon = up.LastLogon,
                    Locked = up.IsAccountLockedOut(),
                    OU = up.DistinguishedName.Substring(up.DistinguishedName.IndexOf(',') + 1)
                };
            }
        }

        public void Update(Account account)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, container))
            using (UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, account.AccountName))
            {
                if (up == null)
                    throw new AppException("Kontoen findes ikke.");

                up.Enabled = account.Enabled;
                up.AccountExpirationDate = account.Expires;
                up.GivenName = account.FirstName;
                up.Surname = account.LastName;
                up.Save();

                if (up.IsAccountLockedOut() && !account.Locked)
                    up.UnlockAccount();
            }
        }

        public void Delete(string accountName)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, container))
                using (UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, accountName))
                    if (up != null)
                        up.Delete();
            }
            catch (Exception)
            {

            }
        }
    }
}