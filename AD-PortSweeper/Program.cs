using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;

namespace LocalAdminChecker
{
    class Program
    {

        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Please enter a value for the port number to check.");
                Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " [port_num]");
                Console.ForegroundColor = ConsoleColor.White;
                System.Environment.Exit(0);
            }
            int port = Convert.ToInt32(arguments[1]);

            Console.WriteLine("=======================================================");
            Console.WriteLine("--->Port Sweeper<---");
            Console.WriteLine("=======================================================");
            Console.WriteLine("Dumping a list of computers from AD...");
            List<string> CompNames = new List<string>();
            string mydom = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            DirectoryEntry ent = new DirectoryEntry("LDAP://" + mydom);
            DirectorySearcher searcher = new DirectorySearcher(ent);
            searcher.Filter = ("(objectClass=computer)");
            searcher.SizeLimit = int.MaxValue;
            searcher.PageSize = int.MaxValue;

            foreach (SearchResult result in searcher.FindAll())
            {
                string ComputerName = result.GetDirectoryEntry().Name;
                if (ComputerName.StartsWith("CN="))
                {
                    ComputerName = ComputerName.Remove(0, "CN=".Length);
                }
                CompNames.Add(ComputerName);
            }
            searcher.Dispose();
            ent.Dispose();

            Console.WriteLine("=======================================================");
            Console.WriteLine("Performing port sweep...");
            Console.WriteLine("=======================================================");

            Queue myqueue = new Queue(CompNames);
            List<Thread> threads = new List<Thread>();

            for (int i = 1; i <= myqueue.Count; i++)
            {
                Thread newThread = new Thread(() => Threader(myqueue, port));
                newThread.IsBackground = true;
                newThread.Start();
                threads.Add(newThread);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }


        }


        static void Threader(Queue myqueue, int port)
        {
            while (myqueue.Count != 0)
            {
                var worker = myqueue.Dequeue();
                string worker2 = Convert.ToString(worker);
                Connector(worker2, port);
            }
        }

        static void Connector(String hostname, int port)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                try
                {
                    socket.Connect(hostname, port);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] Port " + port + " open on " + hostname);
                    Console.ForegroundColor = ConsoleColor.White;
                    socket.Close();
                }
                catch
                {
                    //silently handle when the port is closed
                }


            }
        }


    }



}

