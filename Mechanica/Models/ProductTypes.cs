using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mechanica.Models
{
    public class ProductTypes
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwę")]
        [DisplayName("Typ Produktu")]
        public string Name { get; set; }
    }
}
