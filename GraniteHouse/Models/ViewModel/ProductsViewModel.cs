using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModel
{
    public class ProductsViewModel
    {
        public Products products { get; set; }
        public IEnumerable<ProductTypes> productTypes { get; set; }
        public IEnumerable<SpecialTags> specialTags { get; set; }
    }
}
