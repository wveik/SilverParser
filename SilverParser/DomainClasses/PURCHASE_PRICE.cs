using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SilverParser.DomainClasses {
    public class PURCHASE_PRICE {

        public static PURCHASE_PRICE GET_PURCHASE_PRICE(SQLiteDataReader reader) {
            return new PURCHASE_PRICE() {
                PP_KEY = int.Parse(reader["PP_KEY"].ToString())
                ,
                PP_BUY_DATE = reader["PP_BUY_DATE"].ToString()
                ,
                PP_PRICE_BUY = double.Parse(reader["PP_PRICE_BUY"].ToString())
            };
        }

        public int PP_KEY { get; set; }
        public double PP_PRICE_BUY { get; set; }
        public string PP_BUY_DATE { get; set; }
    }
}
