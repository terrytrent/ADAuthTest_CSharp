using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.DirectoryServices.ActiveDirectory;
using System.Collections.Generic;
using DnDns.Enums;
using DnDns.Query;

namespace ADAuthTest_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string ldapServer = getLdapServer();
            List<string> authorizedUsers = new List<string>();
            authorizedUsers.Add("zzzttrent");
            authorizedUsers.Add("ttrent"); 

            Dictionary<string, string> ldapErrors = new Dictionary<string, string>();
            ldapErrors.Add("52e", "Username or Password incorrect");
            ldapErrors.Add("530", "User Account not permitted to logon during this time");
            ldapErrors.Add("531", "User Account not permitted to logon at this computer");
            ldapErrors.Add("532", "User Account password expired");
            ldapErrors.Add("533", "User Account disabled");
            ldapErrors.Add("701", "User Account expired");
            ldapErrors.Add("773", "User Account password must be reset");
            ldapErrors.Add("775", "User Account Locked");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = "";

            ConsoleKeyInfo info = Console.ReadKey(true);

            while (info.Key != ConsoleKey.Enter)
            {
                string asterisks = generateAsterisks();
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write(asterisks);
                    password += info.KeyChar;
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    int cursorPosition = Console.CursorLeft;
                    if(cursorPosition != 16)
                    {
                        Console.Write("\b\0\b");
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring
                        (0, password.Length - 1);
                    }
                    info = Console.ReadKey(true);
                }
            }
            
            if(authorizedUsers.Contains(username))
            {
                try
                {
                    LdapConnection connection = new LdapConnection(ldapServer);
                    NetworkCredential credential = new NetworkCredential(username, password);
                    connection.Credential = credential;
                    connection.Bind();
                    Console.WriteLine("\nlogged in");
                }
                catch (LdapException lexc)
                {
                    string error = lexc.ServerErrorMessage;
                    string errorcode = error.Substring(77, 3);
                    string errorMessage = ldapErrors[errorcode];
                    Console.WriteLine($"\n{errorMessage}");
                }
            }
            else
            {
                Console.WriteLine("Sorry, you are not authorized to log in.");
            }
                        
        }

        static string generateAsterisks()
        {
            Random randomNumbers = new Random();
            int numberOfAsterisks = randomNumbers.Next(1, 3);

            return new String('*',numberOfAsterisks);
        }

        static string getLdapServer()
        {
            Domain localDomain = Domain.GetCurrentDomain();

            DnsQueryRequest request = new DnsQueryRequest();
            DnsQueryResponse Response = request.Resolve($"_ldap.{localDomain.ToString()}", NsType.A, NsClass.ANY, System.Net.Sockets.ProtocolType.Udp);

            string answer = Response.AuthoritiveNameServers[0].Answer;

            int indexOfDomain = answer.IndexOf($".{localDomain.ToString()}");
            int lengthToSkip = 20;
            int lengthOfLdapServer = indexOfDomain - lengthToSkip;
            string nameOfLdapServer = $"{answer.Substring(lengthToSkip, lengthOfLdapServer)}.{localDomain.ToString()}";
            return nameOfLdapServer;
        }
    }
}
