using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mechanica.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Nazwa"), StringLength(50, ErrorMessage = "Długość nazwy nie powinna przekraczać {1} liter.")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Cena")]
        public double Price { get; set; }

        [Required]
        [DisplayName("Ilość")]
        public int Quantity { get; set; }

        [DisplayName("Dostępność")]
        public bool Available { get; set; }

        [DisplayName("Obrazek")]
        public string Image { get; set; }

        [DisplayName("Waga")]
        public string Weight { get; set; }

        [DisplayName("Pojemość")]
        public string Capacity { get; set; }

        public int ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]
        [DisplayName("Typ Produktu")]
        public virtual ProductTypes ProductTypes { get; set; }
    }
}
