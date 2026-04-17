using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MusiShop
{
    public static class Menu
    {
        public static readonly string[] mainMenu =
        {
            "Check Backlog",
            "Add new product",
            "Edit products",
            "Delete products",
            "Validate files",
            "Open command line",
            "Exit program"
        };
        private static readonly List<string> usedCommands = new();
        // private static readonly Action[] mainMenuFuncs =
        // {
        //     CheckBacklog,
        //     AddNewProduct,
        //     EditProducts,
        //     DeleteProducts,
        //     ValidateFiles,
        //     CommandLineInterpreter,
        //     ExitProgram
        // };
        private static readonly string[] currMenu = mainMenu;
        // private static readonly Action[] currMenuFuncs = mainMenuFuncs;
        private static int currItem = 0;
        private static void PrintMenu(string[] menu)
        {
            int index = 0;
            foreach (var item in menu)
            {
                if (item.StartsWith("["))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(item);
                    Console.ForegroundColor = ConsoleColor.White;
                    index++;
                    continue;
                }
                if (index++ == currItem)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(item);
                Console.BackgroundColor = ConsoleColor.Black; // dumb ahh thing ngl
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }
    }
}