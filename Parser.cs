using System;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;

namespace MusiShop
{
    public static class Parser
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            //TypeInfoResolver = new DefaultJsonTypeInfoResolver
            //{
            //    Modifiers =
            //{
            //    ti =>
            //    {
            //        if (ti.Type == typeof(Product))
            //        {
            //            ti.PolymorphismOptions = new JsonPolymorphismOptions
            //            {
            //                TypeDiscriminatorPropertyName = "$type",
            //                IgnoreUnrecognizedTypeDiscriminators = true,
            //                DerivedTypes =
            //                {
            //                    new JsonDerivedType(typeof(Vinyl), "vinyl")
            //                }
            //            };
            //        }
            //    }
            //}
            //}
        };

        public static Product[] ReadProducts(string type)
        {
            string path = type;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path);
            var products = new Product[files.Length];
            for (int i = 0; i < products.Length; i++)
            {
                var lines = File.ReadAllText(files[i]);
                if (lines == null) continue;
                products[i] = JsonSerializer.Deserialize<Product>(lines, options);
            }

            return products;

        }

        public static Product[] ReadProducts()
        {
            var directories = Directory.GetDirectories("./");
            List<string> files = new();
            foreach (var directory in directories)
            {
                files.AddRange(Directory.GetFiles(directory));
            }
            var products = new Product[files.Count];
            for (int i = 0; i < products.Length; i++)
            {
                var lines = File.ReadAllText(files[i]);
                if (lines == null) continue;
                products[i] = JsonSerializer.Deserialize<Product>(lines, options);
            }

            return products;

        }

        public static void WriteProduct(Product product)
        {
            if (!Directory.Exists(product.Type))
                Directory.CreateDirectory(product.Type);
            string file = JsonSerializer.Serialize(product, options);
            File.WriteAllText(product.Type + '/' + product.Name + ".json", file);
        }

        public static void WriteProducts(string type, Product[] products)
        {
            foreach (var product in products)
            {
                string file = JsonSerializer.Serialize(product, options);
                File.WriteAllText(type + '/' + product.Name + ".json", file);
            }
        }

        public static string[] GetTypes()
        {
            return Directory.GetDirectories("./").Select(x => x.Substring(x.LastIndexOf('/') + 1)).ToArray();
        }
    }
}