using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyncMessaging
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: LyncMessaging.exe <email_address> <message>");
                return;
            }

            LyncManager lm = new LyncManager(args[0], args[1]);

            while (!lm.Done)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
