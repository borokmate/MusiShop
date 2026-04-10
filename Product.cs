using System;
using System.Collections.Generic;
using System.Net.Quic;
using System.Text;

namespace BRUHWEGOOD
{
    public class Product
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public Product(DateTime date, string name, int price, int quantity, string type)
        {
            Date = date;
            Name = name;
            Price = price;
            Quantity = quantity;
            Type = type;
        }
    }
}
