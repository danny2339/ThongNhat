using System.Collections.Generic;
using ThongNhatFinal.Models;

namespace ThongNhatFinal.ModelViews
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set; }
    }
}
