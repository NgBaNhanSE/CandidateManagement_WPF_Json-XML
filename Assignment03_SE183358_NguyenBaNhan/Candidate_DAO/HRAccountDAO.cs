
using System;
using System.Collections;
using System.IO;
using System.Text.Json;
using System.Linq;
using Candidate_BusinessObjects;
using System.Data;

namespace Candidate_DAO
{
    public class HRAccountDAO
    {
        private readonly string _dataPath;
        private static HRAccountDAO instance;
        public HRAccountDAO()
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData");
        }
        public static HRAccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HRAccountDAO();
                }
                return instance;
            }
        }
        public ArrayList GetHraccounts()
        {
            ArrayList hrAccounts = new ArrayList();
            string filePath = Path.Combine(_dataPath, "HRAccount.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot find file: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            var accounts = JsonSerializer.Deserialize<HraccountList>(jsonString);

            if (accounts?.Users != null)
            {
                foreach (var account in accounts.Users)
                {
                    hrAccounts.Add(account);
                }
            }

            return hrAccounts;
        }

        public bool AddHrAccount(Hraccount account)
        {
            try
            {
                var accounts = GetHraccounts();
                accounts.Add(account);
                return SaveHRAccounts(accounts);
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateHrAccount(Hraccount account)
        {
            try
            {
                var accounts = GetHraccounts();
                bool found = false;

                for (int i = 0; i < accounts.Count; i++)
                {
                    var existing = accounts[i] as Hraccount;
                    if (existing.Email == account.Email)
                    {
                        accounts[i] = account;
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
                return SaveHRAccounts(accounts);
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteHrAccount(string email)
        {
            try
            {
                var accounts = GetHraccounts();
                bool found = false;

                for (int i = accounts.Count - 1; i >= 0; i--)
                {
                    var account = accounts[i] as Hraccount;
                    if (account.Email == email)
                    {
                        accounts.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found) return false;

                SaveHRAccounts(accounts);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool SaveHRAccounts(ArrayList accounts)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "HRAccount.json");
                var hrAccountList = new HraccountList
                {
                    Users = accounts.Cast<Hraccount>().ToList()
                };
                string jsonString = JsonSerializer.Serialize(hrAccountList);
                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Hraccount GetHraccountByEmail(string email)
        {
            try
            {
                var accounts = GetHraccounts();
                foreach (Hraccount account in accounts)
                {
                    if (account.Email.Equals(email))
                    {
                        return account;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public ArrayList GetHraccountByNameOrRole(string? Name, string? Role)
        {

            int? _role = string.IsNullOrEmpty(Role) ? (int?)null : int.Parse(Role);
            ArrayList result = new ArrayList();
            var HrAccountList = this.GetHraccounts();

            foreach (Hraccount hraccount in HrAccountList)
            {

                if ((string.IsNullOrEmpty(Name) || hraccount.FullName.Contains(Name)) &&
                     (!_role.HasValue || hraccount.MemberRole.Equals(_role.Value)))
                {
                    result.Add(hraccount);
                }
            }
            return result;
        }
    }
}
