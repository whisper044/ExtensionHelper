﻿using System;
using ExtensionHelper;

namespace ExtensionHelperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            long x = 3843748;
            Console.WriteLine(x.ToFileSize());

            DateTime birthdate = new DateTime(1960, 6, 13);
            Console.WriteLine(birthdate.CalculateAge());
        }
    }
}
