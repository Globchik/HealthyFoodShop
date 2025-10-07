using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace HealthyFood_Shop
{
    public static class SQLite_Statistics
    {
        public static double GetDayShare(in DateTime share_date, out int check_amount, in long? user_id)
        {
            if (!user_id.HasValue)
                throw new Exception("Немає користувача.");
            using (var connection = new SqliteConnection($"Data Source={SQLite_Interaction.DB_Path};Mode=ReadOnly;"))
            {
                double sum = 0;
                check_amount = 0;
                connection.Open();
                string command_str = "select printf('%g', " +
                    "sum(pr_h.price * (100.0 - pr_h.discount) / 100.0 * itoo.amount)), " +
                    "count(DISTINCT ord.id) from order_table as ord left join " +
                    "items_to_order as itoo on ord.id=itoo.fk_order left join " +
                    "price_history as pr_h on itoo.fk_price_history=pr_h.id " +
                    "where date(ord.order_time) = '" +
                    share_date.ToString("yyyy-MM-dd") + "' " +
                    "and ord.fk_employee=@EMPL_ID " +
                    "group by date(ord.order_time);";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                command.Parameters.AddWithValue("@EMPL_ID", user_id);
                using (var reader = command.ExecuteReader())
                {
                    
                    if(reader.Read())
                    {
                        sum = double.Parse((string)reader[0], new CultureInfo("en"));
                        check_amount = General.DBToInt32(reader[1]);
                    }
                }
                return sum;
            }
        }


        public static void GetSoldSumByDates(out Dictionary<DateTime, double> date_sum)
        {
            date_sum = new();
            using (var connection = new SqliteConnection($"Data Source={SQLite_Interaction.DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_str = "select printf('%g', " +
                    "sum(pr_h.price * (100.0 - pr_h.discount) / 100.0 * itoo.amount)), " +
                    "date(ord.order_time) from order_table as ord left join " +
                    "items_to_order as itoo on ord.id=itoo.fk_order left join " +
                    "price_history as pr_h on itoo.fk_price_history=pr_h.id " +
                    "group by date(ord.order_time);";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        date_sum[DateTime.ParseExact((string)reader[1], "yyyy-MM-dd", null)] = double.Parse((string)reader[0], new CultureInfo("en"));
                }
            }
        }


        public static void GetPopularItems(out Dictionary<string, double> name_amount)
        {
            name_amount = new();
            using (var connection = new SqliteConnection($"Data Source={SQLite_Interaction.DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_str = "SELECT it.name as 'name', " +
                    "sum(ifnull(itoo.amount,0)) as 'amount' " +
                    "from item as it left join item_batch as it_b " +
                    "on it.id=it_b.fk_item left join items_to_order as itoo " +
                    "on itoo.fk_batch=it_b.id " +
                    "GROUP by it.id ORDER BY sum(ifnull(itoo.amount,0)) DESC, " +
                    "it.name ASC limit 10;";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        name_amount[General.DBToString(reader[0])] = General.DBToInt32(reader[1]);
                }
            }
        }


        public static void GetStockAvailableTable(out List<string[]> stock_table)
        {
            stock_table = new();
            using (var connection = new SqliteConnection($"Data Source={SQLite_Interaction.DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_str = "SELECT it.name as 'name', " +
                    "it_b.expiry_date as 'expiry_date', it_b.amount as 'amount' " +
                    "from item as it left join item_batch as it_b " +
                    "on it.id=it_b.fk_item where it_b.amount!=0 " +
                    "and (it_b.expiry_date is null or " +
                    "date(it_b.expiry_date)>date('now', 'localtime')) " +
                    "ORDER BY it.name ASC, it_b.expiry_date ASC;";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                using (var reader = command.ExecuteReader())
                {
                    string[] row;
                    while (reader.Read())
                    {
                        row = new string[3];
                        row[0] = General.DBToString(reader[0]);
                        row[1] = General.DBToString(reader[1]);
                        row[2] = General.DBToString(reader[2]);
                        stock_table.Add(row);
                    }
                        
                }
            }
        }


        public static void GetStockExpiredTable(out List<string[]> stock_table)
        {
            stock_table = new();
            using (var connection = new SqliteConnection($"Data Source={SQLite_Interaction.DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_str = "SELECT it.name as 'name', " +
                    "it_b.expiry_date as 'expiry_date', it_b.amount as 'amount' " +
                    "from item as it left join item_batch as it_b on " +
                    "it.id=it_b.fk_item where it_b.amount!=0 and " +
                    "(it_b.expiry_date is not null and " +
                    "date(it_b.expiry_date)<date('now', 'localtime')) " +
                    "ORDER BY it.name ASC, it_b.expiry_date ASC;";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                using (var reader = command.ExecuteReader())
                {
                    string[] row;
                    while (reader.Read())
                    {
                        row = new string[3];
                        row[0] = General.DBToString(reader[0]);
                        row[1] = General.DBToString(reader[1]);
                        row[2] = General.DBToString(reader[2]);
                        stock_table.Add(row);
                    }

                }
            }
        }



    }
}
