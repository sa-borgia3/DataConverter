﻿using System;

namespace DataConverter
{
    class Program
    {
        static void Main(string[] args)
        {


            Spreadsheet spreadsheet = new Spreadsheet("/Users/salvatoreborgia/Downloads/test2.xlsx", "xlsx");

            spreadsheet.ReadData();

            string json = spreadsheet.ConvertToJson();

            Console.WriteLine(json);

        }

    }
}
