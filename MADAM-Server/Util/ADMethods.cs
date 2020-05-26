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
    class ADMethods
    {
        public static List<Classes.Users> getUsers(string ip)
        {
            //returns a list of Users objects from an active directory server specified by ip
            try
            {
                
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + ip, "MADAM", "Test123");
                DirectorySearcher mySearcher = new DirectorySearcher(entry);
                mySearcher.Filter = ("(objectClass=user)");
                //debug
                Console.WriteLine("Listing of users in the Active Directory");
                Console.WriteLine("============================================");
                //
                List<Classes.Users> userList = new List<Classes.Users>();
                foreach (SearchResult resEnt in mySearcher.FindAll())
                {
                    Classes.Users tempUser = new Classes.Users();
                    DirectoryEntry tempDE = resEnt.GetDirectoryEntry();
                    Console.WriteLine(resEnt.GetDirectoryEntry().Name.ToString());
                    tempUser.fullName = resEnt.GetDirectoryEntry().Name.ToString();
                    userList.Add(tempUser);
                }
                Console.WriteLine("=========== End of Listing =============");
                return userList;
            }
            catch (Exception e)
            {
                List<Classes.Users> userList = new List<Classes.Users>();
                Console.WriteLine(e);
                return userList;
            }
        }

        public static void addUser(string ip, string userName)
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + ip, "MADAM", "Test123");
            entry.Children.Add(userName,entry.SchemaClassName);
            entry.CommitChanges();
        }
    }
}
