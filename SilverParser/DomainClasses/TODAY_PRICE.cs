using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SilverParser.DomainClasses {
    public class TODAY_PRICE {
        public static TODAY_PRICE GET_TODAY_PRICE(SQLiteDataReader reader) {
            return new TODAY_PRICE() {
                TP_KEY = int.Parse(reader["TP_KEY"].ToString())
                ,
                TP_BUY = reader["TP_BUY"].ToString()
                ,
                TP_PRICE = double.Parse(reader["TP_PRICE"].ToString())
            };
        }

        public int TP_KEY { get; set; }
        public double TP_PRICE { get; set; }
        public string TP_BUY { get; set; }
    }
}
