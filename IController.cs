using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    interface IController<T>
    {
        public void UpdateSingleItem(T item);
    }
}
