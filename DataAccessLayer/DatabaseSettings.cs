using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class DatabaseSettings
    {
        // هنخزن هنا الـ Connection String عشان نقرأه من أي مكان في الـ DAL
        public static string ConnectionString { get; set; }
    }
}
