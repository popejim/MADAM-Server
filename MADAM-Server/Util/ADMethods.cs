using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace MADAM_Server.Util
{
    static class ADMethods
    {
        public static void getUsers(string ip)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + ip, "MADAM", "Test123");
                DirectorySearcher mySearcher = new DirectorySearcher(entry);
                mySearcher.Filter = ("(objectClass=user)");
                Console.WriteLine("Listing of users in the Active Directory");
                Console.WriteLine("============================================"); foreach (SearchResult resEnt in mySearcher.FindAll())
                {
                    Console.WriteLine(resEnt.GetDirectoryEntry().Name.ToString());
                }
                Console.WriteLine("=========== End of Listing =============");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
