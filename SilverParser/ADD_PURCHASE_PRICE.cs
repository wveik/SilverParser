using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SilverParser {
    public partial class ADD_PURCHASE_PRICE : Form {
        public ADD_PURCHASE_PRICE() {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(_PP_PRICE_BUY.Text)) {
                MessageBox.Show("Введите стоимость покупки!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=C:/sqlite/silver_parser.db; Version=3;")) {
                try {
                    conn.Open();

                    string sql_command = @"INSERT INTO PURCHASE_PRICE 
                    (
                        PP_PRICE_BUY
                        ,PP_BUY_DATE
                    )
                    VALUES
                    (
                        @PP_PRICE_BUY
                        ,@PP_BUY_DATE
                    )";

                    SQLiteCommand cmd = new SQLiteCommand(sql_command, conn);
                    cmd.Parameters.Add("@PP_PRICE_BUY", DbType.Double).Value = double.Parse(_PP_PRICE_BUY.Text);
                    cmd.Parameters.Add("@PP_BUY_DATE", DbType.String).Value = _PP_BUY_DATE.Value.ToShortDateString();
                    
                    try {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("ВСЕ ПРОШЛО УДАЧНО!", "Good", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    } catch (SQLiteException ex) {
                        MessageBox.Show("Не удалось завести =(!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
