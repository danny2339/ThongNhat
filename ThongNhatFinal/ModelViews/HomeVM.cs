using System.Collections.Generic;
using ThongNhatFinal.Models;

namespace ThongNhatFinal.ModelViews
{
    public class HomeVM
    {
        public List<News> News { get; set; }
        public List<ProductHomeVM> Products { get; set; }
    }
}
