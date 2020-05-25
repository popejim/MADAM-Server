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
        public static List<Classes.Users> getUsers(string ip)
        {
            List<Classes.Users> users =  new List<Classes.Users>();
            try
            {
                Classes.Users tempUser = new Classes.Users();
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + ip, "MADAM", "Test123");
                DirectorySearcher mySearcher = new DirectorySearcher(entry);
                mySearcher.Filter = ("(objectClass=user)");
                Console.WriteLine("Listing of users in the Active Directory");
                Console.WriteLine("============================================"); foreach (SearchResult resEnt in mySearcher.FindAll())
                {
                    Console.WriteLine(resEnt.GetDirectoryEntry().Name.ToString());
                    tempUser.fullName = resEnt.GetDirectoryEntry().Name.ToString();
                    users.Add(tempUser);
                }
                Console.WriteLine("=========== End of Listing =============");
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return users;
            }
        }
    }
}
