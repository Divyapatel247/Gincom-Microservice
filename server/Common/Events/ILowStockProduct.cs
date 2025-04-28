using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Events
{
    public interface ILowStockProduct
    {
        int lowStock { get;  }
    }

    public class LowStockProduct : ILowStockProduct
    {
         public int lowStock { get; set; }
    }
}