using SilverParser.DomainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;


namespace SilverParser {
    public partial class Main : Form {
        
        private List<PURCHASE_PRICE> _main_price_list = new List<PURCHASE_PRICE>();
        private string DATA_BASE = "C:/sqlite/silver_parser.db";
        private string WEB_SITE = "http://news.yandex.ru/quotes/1506.html";

        public Main() {
            InitializeComponent();
            
            getTodayList();

            using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0}; Version=3;", DATA_BASE))) {
                try {
                    conn.Open();

                    string sql_command = 
                                @"SELECT PP_KEY, PP_PRICE_BUY, PP_BUY_DATE 
                                FROM PURCHASE_PRICE 
                                ORDER BY PP_KEY DESC";

                    SQLiteCommand cmd = new SQLiteCommand(sql_command, conn);

                    try {
                        SQLiteDataReader r = cmd.ExecuteReader();

                        while (r.Read()) {
                            _main_price_list.Add(PURCHASE_PRICE.GET_PURCHASE_PRICE(r));
                        }
                        r.Close();
                        gridMainList.DataSource = _main_price_list;
                    } catch (SQLiteException ex) {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (SQLiteException ex) {
                    MessageBox.Show("Ошибка соединения с базой silver_parser.db!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e) {
            ADD_PURCHASE_PRICE form = new ADD_PURCHASE_PRICE();
            form.ShowDialog();
        }

        private void buttonStartParser_Click(object sender, EventArgs e) {
            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled) {
                timer1_Tick(null, null);
                buttonStartParser.Text = "ВЫКЛЮЧИТЬ робота!!!";
            } else {
                buttonStartParser.Text = "Включить робота";
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            WebRequest wrGETURL = WebRequest.Create(WEB_SITE);
            Stream objStream = wrGETURL.GetResponse().GetResponseStream();

            if (objStream == null) return;

            StreamReader objReader = new StreamReader(objStream);
            string HTML = objReader.ReadToEnd();



            // Регулярка
            string pattern = "<tr class=\"b-quote__head\"><td class=\"b-quote__date\">Дата</td><td class=\"b-quote__value\">Курс</td><td class=\"b-quote__change\">Изменение</td></tr><tr class=\"b-quote__day b-quote__last-day\"><td class=\"b-quote__date\">\\d\\d.\\d\\d</td><td class=\"b-quote__value\"><span class=\"b-quote__sgn\"></span>\\d*,\\d*</td><td class=\"b-quote__change\">";
            RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline;
            Regex regex = new Regex(pattern, options);
            Match match = regex.Match(HTML.ToString());
            string result = "";

            while (match.Success) {
                result += match;
                match = match.NextMatch();
            }

            if (string.IsNullOrEmpty(result)) return;

            result = result.Replace("<tr class=\"b-quote__head\"><td class=\"b-quote__date\">Дата</td><td class=\"b-quote__value\">Курс</td><td class=\"b-quote__change\">Изменение</td></tr><tr class=\"b-quote__day b-quote__last-day\"><td class=\"b-quote__date\">", "");
            result = result.Replace("</td><td class=\"b-quote__change\">", "");
            result = result.Replace(
                String.Format(
                "{0}</td><td class=\"b-quote__value\"><span class=\"b-quote__sgn\"></span>", DateTime.Now.ToShortDateString().Substring(0, 5)),
                "");

            double _price = double.Parse(result);
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=C:/sqlite/silver_parser.db; Version=3;")) {
                try {
                    conn.Open();

                    string sql_command = @"INSERT INTO TODAY_PRICE 
                    (
                        TP_PRICE
                        ,TP_BUY
                    )
                    VALUES
                    (
                        @TP_PRICE
                        ,@TP_BUY
                    )";

                    SQLiteCommand cmd = new SQLiteCommand(sql_command, conn);
                    cmd.Parameters.Add("@TP_PRICE", DbType.Double).Value = _price;
                    cmd.Parameters.Add("@TP_BUY", DbType.String).Value = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                    try {
                        cmd.ExecuteNonQuery();
                    } catch (SQLiteException ex) {
                        MessageBox.Show("Не удалось завести =(!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (_main_price_list[0].PP_PRICE_BUY <= _price) {
                MessageBox.Show("Срочно продавай СЕРЕБРО!!!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            getTodayList();

            notifyIcon1.BalloonTipText = string.Format("Программа отработала. Стоимость {0} $", _price);
            notifyIcon1.ShowBalloonTip(5000);
        }

        private void getTodayList() {
            using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0}; Version=3;", DATA_BASE))) {
                try {
                    conn.Open();

                    string sql_command =
                                @"SELECT TP_KEY, TP_PRICE, TP_BUY 
                                FROM TODAY_PRICE 
                                ORDER BY TP_KEY DESC";

                    SQLiteCommand cmd = new SQLiteCommand(sql_command, conn);

                    List<TODAY_PRICE> todo_price = new List<TODAY_PRICE>();

                    try {
                        SQLiteDataReader r = cmd.ExecuteReader();

                        while (r.Read()) {
                            todo_price.Add(TODAY_PRICE.GET_TODAY_PRICE(r));
                        }
                        r.Close();
                        gridMainList.DataSource = _main_price_list;
                    } catch (SQLiteException ex) {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    gridTodoPrices.DataSource = todo_price;
                } catch (SQLiteException ex) {
                    MessageBox.Show("Ошибка соединения с базой silver_parser.db!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Main_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) {
                Hide();
                
                notifyIcon1.BalloonTipText = "Программа была спрятана";
                
                notifyIcon1.ShowBalloonTip(5000);
            }
        }
    }
}
