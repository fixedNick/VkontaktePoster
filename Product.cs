using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Product
    {
        public static readonly List<Product> Products = new List<Product>();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Price { get; private set; }

        /// <summary>
        /// Using photos everywhere inside class.
        /// Using Photos to get read-only collection from other classes
        /// </summary>
        private List<string> photos = new List<string>();
        public IReadOnlyList<string> Photos
        {
            get => photos.AsReadOnly();
            private set => photos = new List<string>(value);
        }
        public void AddPhoto(string photoPath)
        {
            foreach(var ph in photos)
            {
                if (ph.Trim().ToLower().Equals(photoPath.Trim().ToLower()))
                    return;
            }
            photos.Add(photoPath);
        }
        public void RemovePhoto(int index) => photos.RemoveAt(index);
        public void RemovePhoto(string photoPath)
        {
            foreach(var ph in photos)
            {
                if (ph.Trim().ToLower().Equals(photoPath.Trim().ToLower()))
                {
                    photos.Remove(ph);
                    break;
                }
            }
        }

        public Product(string name, int price, string descr = "", List<string> photosList = null)
        {
            Name = name;
            Description = descr;
            Price = price;
            if (photosList != null) 
                photos = photosList;

            Products.Add(this);
        }
    }
}
