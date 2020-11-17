using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Product
    {
        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            private set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Methods to work with photos list.
        /// </summary>
        private List<string> photos;
        public List<string> GetPhotos() => photos;
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

        public Product(string name, string descr = "", List<string> photosList = null)
        {
            Name = name;
            Description = descr;
            photos = photosList == null ? new List<string>() : photosList;
        }
    }
}
