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
        public static void ReplaceProduct(string originalName, string originalType, Product newProduct)
        {
            string path = originalType;
            string fullPath = path + '/' + originalName + ".json";
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            if (!Directory.Exists(newProduct.Type))
                Directory.CreateDirectory(newProduct.Type);
            string file = JsonSerializer.Serialize(newProduct, options);
            File.WriteAllText(newProduct.Type + '/' + newProduct.Name + ".json", file);
        }
        public static void DeleteProduct(Product product)
        {
            string path = product.Type;
            string fullPath = path + '/' + product.Name + ".json";
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        public static bool ValidateProduct(string type, string name)
        {
            string fullPath = type + '/' + name + ".json";
            if (!File.Exists(fullPath))
                return false;
            try
            {
                var text = File.ReadAllText(fullPath);
                var product = JsonSerializer.Deserialize<Product>(text, options);
                return product != null
                    && !string.IsNullOrEmpty(product.Name)
                    && !string.IsNullOrEmpty(product.Type)
                    && product.Price >= 0
                    && product.Quantity >= 0;
            }
            catch
            {
                return false;
            }
        }
        public static List<string> ValidateAll()
        {
            var broken = new List<string>();
            foreach (var dir in Directory.GetDirectories("./"))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    string type = dir.Substring(dir.LastIndexOf('/') + 1);
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (!ValidateProduct(type, name))
                        broken.Add(file);
                }
            }
            return broken;
        }
    }
}
