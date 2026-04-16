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
    }
}