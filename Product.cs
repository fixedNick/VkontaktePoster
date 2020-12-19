using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Product
    {
        public readonly int ProductID = -1;
        public static readonly List<Product> Products = new List<Product>();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Price { get; private set; }

        /// <summary>
        /// Using photos everywhere inside class.
        /// Using Photos to get read-only collection from other classes
        /// </summary>
        public List<string> Photos = new List<string>();
        
        public void AddPhoto(string photoPath)
        {
            foreach(var ph in Photos)
            {
                if (ph.Trim().ToLower().Equals(photoPath.Trim().ToLower()))
                    return;
            }
            Photos.Add(photoPath);
        }
        public void RemovePhoto(int index) => Photos.RemoveAt(index);
        public void RemovePhoto(string photoPath)
        {
            foreach(var ph in Photos)
            {
                if (ph.Trim().ToLower().Equals(photoPath.Trim().ToLower()))
                {
                    Photos.Remove(ph);
                    break;
                }
            }
        }

        [JsonConstructor]
        public Product(int productId, string name, string description, int price, List<string> photos) {
            ProductID = productId;
            Name = name;
            Description = description;
            Price = price;
            if(photos != null)
                Photos = photos;
        }

        public Product(Product prod) => Products.Add(prod);
        public Product(string name, int price, string descr = "", List<string> photosList = null)
        {
            if (int.TryParse(Config.GetConfigProperty("NextProductID"), out ProductID) == false) throw new Exception("Не удалось получить значение следующего ID продукта из конфига");
            Config.SetConfigProperty("NextProductID", (ProductID + 1).ToString());

            Name = name;
            Description = descr;
            Price = price;
            if (photosList != null)
                Photos = photosList;

            Products.Add(this);
        }

        public static void DeleteProduct(int id)
        {
            foreach(var prod in Products)
            {
                if (prod.ProductID.Equals(id))
                {
                    VKAccount.RemoveProductFromAccounts(id);
                    Products.Remove(prod);
                    IOController.DeleteFile(prod);
                    break;
                }
            }
        }
    }
}
