using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mechanica.Models.ViewModel
{
    public class ProductsViewModel
    {
        public Products Products { get; set; }

        [DisplayName("Typ Produktu")]
        public IEnumerable<ProductTypes> ProductTypes { get; set; }
    }
}
