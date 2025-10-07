using System.Collections.Generic;
using System;
using Microsoft.Data.Sqlite;
using HealthyFood_Shop.Classes;
using System.Globalization;
using System.Linq;
using ScottPlot.Plottables;
using OpenTK.Windowing.Common.Input;

namespace HealthyFood_Shop
{
    public static class SQLite_Interaction
    {
        public enum DB_Tables
        {
            action_log,
            category,
            country,
            employee,
            item,
            item_batch,
            items_to_order,
            manufacturer,
            measurment_unit,
            order_table,
            order_status,
            position,
            price_history,
            sub_category
        }

        public static string? DB_Table_ToString(in DB_Tables table)
        {
            return table switch
            {
                DB_Tables.action_log => "Лог",
                DB_Tables.category => "Категорії",
                DB_Tables.country => "Країни",
                DB_Tables.employee => "Робітники",
                DB_Tables.item => "Товари",
                DB_Tables.item_batch => "Партії",
                DB_Tables.items_to_order => "Товари в замовленні",
                DB_Tables.manufacturer => "Виробники",
                DB_Tables.measurment_unit => "Одиниця виміру",
                DB_Tables.order_table => "Замовлення",
                DB_Tables.order_status => "Статуси замовлень",
                DB_Tables.position => "Посади",
                DB_Tables.price_history => "Ціни",
                DB_Tables.sub_category => "Підкатегорії",
                _ => Enum.GetName(table),
            };
        }
        public static string DB_Path { get; private set; } = "my_db/Current.db";
        //Логування

        public static void LogAction(in string desc)
        {
            if(CurrentUser.current_user.Id.HasValue)
                using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("insert into action_log" +
                        "(\"action_description\", \"fk_employee\", \"date_time\") values " +
                        "(@DESCR, @EMPL_ID, datetime('now', 'localtime'))", connection);
                    command.Parameters.AddWithValue("@DESCR", desc);
                    command.Parameters.AddWithValue("@EMPL_ID", CurrentUser.current_user.Id);
                    command.ExecuteNonQuery();
                }
        }

        public static void FillLogActionList(out List<LoggedAction> log_list, in long empl_id)
        {
            log_list = new();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = 
                    "SELECT date_time, action_description FROM action_log where fk_employee=@EMPL_ID order by date_time;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@EMPL_ID", empl_id);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        log_list.Add(
                            new LoggedAction(
                                (string)reader["date_time"],
                                (string)reader["action_description"]
                            ));
                    }
                }
            }
        }

        



        //Загальні функції

        public static long? GetIDByName(in string n, in DB_Tables table_n)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"select id, name from {Enum.GetName(table_n)} where name=@NAME", connection);
                command.Parameters.AddWithValue("@NAME", n);
                return (long?)command.ExecuteScalar();
            }
        }

        public static string? GetValueByID(in DB_Tables table_n, in long id, in string value_name)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = 
                    new SqliteCommand($"select {value_name} from {Enum.GetName(table_n)} where id=@ID",
                    connection);
                command.Parameters.AddWithValue("@ID", id);
                return (string?)command.ExecuteScalar();
            }
        }


        public static void GetValue_ID_Dictionary(out Dictionary<string, Int64> value_id, in DB_Tables table_name)
        {
            value_id = new Dictionary<string, Int64>();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"select * from {Enum.GetName(table_name)}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        value_id[(string)reader[1]] = (Int64)reader[0];
                }
            }
        }

        public static bool DeleteFromDB(in long d_id, in DB_Tables table_n)
        {
            bool is_sucsess;
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"DELETE FROM {Enum.GetName(table_n)} where id = @ID; ", connection);
                command.Parameters.AddWithValue("@ID", d_id);
                is_sucsess = command.ExecuteNonQuery() == 1;
                
                
            }
            if(table_n != DB_Tables.items_to_order)
                LogAction($"В таблиці \"{DB_Table_ToString(table_n)}\" видаляється значення." + (is_sucsess ? "Успіх!" : "ПОМИЛКА!"));
            return is_sucsess;
        }


        public static bool IsValueInTable(in string? search_val, DB_Tables table_name)
        {
            if (search_val is null)
                return false;
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"select * from {Enum.GetName(table_name)}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((string)reader[1] == search_val)
                            return true;
                    }
                }
            }
            return false;
        }

        //Чи є search_val в стовбці col_name таблиці table_name
        public static bool IsValueInTable(in string search_val, DB_Tables table_name,
            in string col_name)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = 
                    new SqliteCommand($"select * from {Enum.GetName(table_name)}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((string)reader[col_name] == search_val)
                            return true;
                    }
                }
            }
            return false;
        }

        //Функції щодо редагування назв

        public static void InsertName(in DB_Tables table_name, in string new_name)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"INSERT INTO {Enum.GetName(table_name)}(name) VALUES(@NEWN)", connection);
                command.Parameters.AddWithValue("@NEWN", new_name);
                command.ExecuteNonQuery();
            }
            LogAction($"В таблиці \"{DB_Table_ToString(table_name)}\" нове значення - {new_name}");
        }

        public static void UpdateName(in DB_Tables table_name,in long pid, in string new_name)
        {
            string? old_name = GetValueByID(table_name, pid, "name");
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand($"UPDATE {Enum.GetName(table_name)} SET name=@NEWN WHERE id=@ID", connection);
                command.Parameters.AddWithValue("@NEWN", new_name);
                command.Parameters.AddWithValue("@ID", pid);
                command.ExecuteNonQuery();
            }
            LogAction($"В таблиці \"{DB_Table_ToString(table_name)}\" змінили значення з {old_name} на {new_name}");
        }

        //Tовари

        public static long? GetItemIDByBatch(in long? batch_id)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("select it.id from item as it " +
                    "left join item_batch as batch on it.id=batch.fk_item where batch.id=@BATCH_ID", connection);
                command.Parameters.AddWithValue("@BATCH_ID", batch_id);
                return (long?)command.ExecuteScalar();
            }
        }

       

       


        //returns item id
        public static Int64? InsertItemToDB(in Item it)
        {
            long? new_it_id;
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string_id = " select seq from sqlite_sequence where name='item'";
                var command_stringb = new System.Text.StringBuilder(
                    "INSERT into item(name, fk_category, fk_sub_category, fk_country, fk_manufacturer," +
                    " fk_measurment_unit, amount_in_item");
                command_stringb.Append(", comment");
                command_stringb.Append(") VALUES (@NAME," +
                    " (select \"id\" from category where name = @CATEGORY)," +
                    " (select \"id\" from sub_category where name = @SUBCATEGORY)," +
                    " (select \"id\" from country where name = @COUNTRY)," +
                    " (select \"id\" from manufacturer where name = @MANUFACTURER)," +
                    " (select \"id\" from measurment_unit where name = @MEASURMENT_UNIT), @AMOUNT_IN_ITEM");
                command_stringb.Append(", @COMMENT");
                command_stringb.Append(")");
                SqliteCommand command = new SqliteCommand(command_stringb.ToString(), connection);
                SqliteCommand command_id = new SqliteCommand(command_string_id, connection);
                command.Parameters.AddWithValue("@NAME", it.Name);
                command.Parameters.AddWithValue("@CATEGORY", it.FK_Category);
                command.Parameters.AddWithValue("@SUBCATEGORY", it.FK_SubCategory is null ? DBNull.Value : it.FK_SubCategory);
                command.Parameters.AddWithValue("@COUNTRY", it.FK_Country);
                command.Parameters.AddWithValue("@MANUFACTURER", it.FK_Manufacturer);
                command.Parameters.AddWithValue("@MEASURMENT_UNIT", it.FK_MeasurmentUnit);
                command.Parameters.AddWithValue("@AMOUNT_IN_ITEM", it.AmountInItem);
                command.Parameters.AddWithValue("@COMMENT", it.Comment is null || it.Comment == String.Empty? 
                    DBNull.Value : it.Comment);
                command.ExecuteNonQuery();
                
                new_it_id = (long?)command_id.ExecuteScalar();
            }
            LogAction($"Створено новий товар \"{it.Name}\"");
            ChangeItemPrice(new_it_id, it.Price, it.Discount.GetValueOrDefault(0));

            return new_it_id;
        }

        public static void ChangeItemPrice(in long? it_id, double new_price, int new_disc)
        {
            if(!it_id.HasValue)
                throw new Exception("Невалідний товар");
            if (new_price < 0 || new_disc >= 100 || new_disc <0)
                throw new Exception("Невалідна ціна");
            new_price = Math.Round(new_price, 2);

            string? it_name;
            double old_price;
            int old_discount;

            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("select it.name, printf(\"%g\",pr_h.price), pr_h.discount " +
                    "from item as it left join price_history pr_h on it.fk_price_history=pr_h.id " +
                    "where it.id=@IT_ID;", connection);
                command.Parameters.AddWithValue("@IT_ID", it_id);
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    it_name = (string?)reader[0];
                    old_price = double.Parse((string)reader[1], new CultureInfo("en"));
                    old_discount = reader[2] is System.DBNull ? 0 : Convert.ToInt32((Int64)reader[2]);
                }
            }
            if (old_discount == new_disc && old_price == new_price)
                return;
            
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "INSERT INTO \"price_history\"(\"price\", \"discount\", \"date_time\") " +
                    "VALUES (@PRICE, @DISC, datetime('now', 'localtime'));";
                string command_string_id = " select seq from sqlite_sequence where name='price_history'";
                SqliteCommand command_id = new SqliteCommand(command_string_id, connection);
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@PRICE", new_price);
                command.Parameters.AddWithValue("@DISC", new_disc);
                command.ExecuteNonQuery();
                long? pr_h_id = (long?)command_id.ExecuteScalar();
                if(!pr_h_id.HasValue)
                    throw new Exception("Немає доступу до нової ціни");
                command_string = "UPDATE item set fk_price_history = @PR_H where item.id = @IT_ID;";
                command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@PR_H", pr_h_id);
                command.Parameters.AddWithValue("@IT_ID", it_id);
                command.ExecuteNonQuery();
            }
            if(old_price == new_price && new_disc == 0)
                LogAction($"Прибрана знижка на товар \"{it_name}\"");
            else if(old_price == new_price)
                LogAction($"Встановлена знижка {new_disc}% на товар \"{it_name}\"");
            else
                LogAction($"Встановлено ціну {new_price} та знижку {new_disc}% на товар \"{it_name}\"");
        }

        public static void UpdateItem(in Item it)
        {
            if (it is null || !it.IsItemValid())
                throw new Exception("Усі необхідні поля товару не заповнено");

            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                var command_stringb = new System.Text.StringBuilder("UPDATE item SET" +
                    " name = @NAME, fk_category=(select \"id\" from category where name = @CATEGORY)," +
                    " fk_sub_category = (select \"id\" from sub_category where name = @SUBCATEGORY)," +
                    " fk_country=(select \"id\" from country where name = @COUNTRY)," +
                    " fk_manufacturer=(select \"id\" from manufacturer where name = @MANUFACTURER)," +
                    " fk_measurment_unit=(select \"id\" from measurment_unit where name = @MEASURMENT_UNIT)," +
                    " amount_in_item=@AMINIT");
                command_stringb.Append(", comment=@COMMENT");
                command_stringb.Append(" where id=@ID;");
                SqliteCommand command = new SqliteCommand(command_stringb.ToString(), connection);
                command.Parameters.AddWithValue("@ID", it.ID);
                command.Parameters.AddWithValue("@NAME", it.Name);
                command.Parameters.AddWithValue("@CATEGORY", it.FK_Category);
                command.Parameters.AddWithValue("@SUBCATEGORY", it.FK_SubCategory is null ? DBNull.Value : it.FK_SubCategory);
                command.Parameters.AddWithValue("@COUNTRY", it.FK_Country);
                command.Parameters.AddWithValue("@MANUFACTURER", it.FK_Manufacturer);
                command.Parameters.AddWithValue("@MEASURMENT_UNIT", it.FK_MeasurmentUnit);
                command.Parameters.AddWithValue("@AMINIT", it.AmountInItem);
                command.Parameters.AddWithValue("@COMMENT", it.Comment is null? DBNull.Value : it.Comment);
                command.ExecuteNonQuery();

                LogAction($"Змінено товар \"{it.Name}\"");
                ChangeItemPrice(it.ID, it.Price, it.Discount.GetValueOrDefault(0));
            }
        }

        public static void GetItemFromDB(in Int64 it_id, out Item it)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "select it.id as \"id\", it.name as \"name\"," +
                    " printf(\"%g\",pr_h.price) as \"price\", pr_h.discount as \"discount\"," +
                    " sum(it_b.amount) as \"available_items\", c.name as \"country\"," +
                    " manu.name as \"manufacturer\", cat.name as \"category\"," +
                    " subcat.name as \"sub_category\", printf(\"%g\"," +
                    " it.amount_in_item) as \"amount_in_item\", mu.name as \"measurment_unit\"," +
                    " it.comment as \"comment\"" +
                    " from item as it left join category as cat on it.fk_category = cat.id" +
                    " left join sub_category as subcat on it.fk_sub_category = subcat.id" +
                    " left join country as c on it.fk_country=c.id" +
                    " left join manufacturer as manu on it.fk_manufacturer=manu.id" +
                    " left join measurment_unit as mu on it.fk_measurment_unit=mu.id" +
                    " left join item_batch as it_b on it_b.fk_item=it.id" +
                    " left join price_history as pr_h on it.fk_price_history=pr_h.id" +
                    " WHERE it.id = @ID GROUP by it.id;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ID", it_id);
                using (var reader = command.ExecuteReader())
                {
                    CultureInfo en = new CultureInfo("en-US");
                    reader.Read();
                    it = new Item(
                            reader["name"] is DBNull? null: (string)reader["name"],
                            double.Parse((string)reader["price"], en),
                            reader["category"] is DBNull? null: (string)reader["category"],
                            reader["country"] is DBNull? null: (string)reader["country"],
                            reader["manufacturer"] is DBNull? null: (string)reader["manufacturer"],
                            reader["measurment_unit"] is DBNull? null: (string)reader["measurment_unit"],
                            double.Parse((string)reader["amount_in_item"], en),
                            (Int64)reader["id"],
                            reader["discount"] is System.DBNull ? null : Convert.ToInt32((Int64)reader["discount"]),
                            reader["sub_category"] is System.DBNull ? null : (string)reader["sub_category"],
                            reader["comment"] is System.DBNull ? null : (string)reader["comment"],
                            reader["available_items"] is System.DBNull ? 0 : Convert.ToInt32((Int64)reader["available_items"]));
                }
            }
        }

        public static void FillItemList(out List<Item> items_list, in string f_name = "",
            double? min_price = null, double? max_price = null, in string f_country = "",
            in string f_manufacturer = "", in string f_category = "", in string f_subcategory = "")
        {
            if (min_price is null)
                min_price = 0.0;
            if (max_price is null)
                max_price = double.MaxValue;
            items_list = new List<Item>();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "select it.id as \"id\", it.name as \"name\", " +
                    "printf(\"%g\",pr_h.price) as \"price\", pr_h.discount as \"discount\", " +
                    "sum(it_b.amount) as \"available_items\", c.name as \"country\", " +
                    "manu.name as \"manufacturer\", cat.name as \"category\", " +
                    "subcat.name as \"sub_category\", printf(\"%g\"," +
                    "it.amount_in_item) as \"amount_in_item\", mu.name as \"measurment_unit\", " +
                    "it.comment as \"comment\" from item as it " +
                    "left join category as cat on it.fk_category = cat.id " +
                    "left join sub_category as subcat on it.fk_sub_category = subcat.id " +
                    "left join country as c on it.fk_country=c.id " +
                    "left join manufacturer as manu on it.fk_manufacturer=manu.id " +
                    "left join measurment_unit as mu on it.fk_measurment_unit=mu.id " +
                    "left join item_batch as it_b on it_b.fk_item=it.id " +
                    "left join price_history as pr_h on it.fk_price_history=pr_h.id " +
                    "WHERE LOWER(IIF(it.name is NULL, \"\", it.name)) like Lower(\"%\" || @F_NAME || \"%\") " +
                    "AND (pr_h.price*((100.0-pr_h.discount)/100)) >= @PRICE_MIN " +
                    "AND (pr_h.price*((100.0-pr_h.discount)/100)) <= @PRICE_MAX " +
                    "AND lower(IIF(c.name is NULL, \"\", c.name)) like lower(\"%\" || @F_COUNTRY || \"%\") " +
                    "AND lower(IIF(manu.name is NULL, \"\", manu.name)) like lower(\"%\" || @F_MANUFACTURER || \"%\") " +
                    "AND lower(IIF(cat.name is NULL, \"\", cat.name)) like lower(\"%\" || @F_CATEGORY || \"%\") " +
                    "AND lower(IIF(subcat.name is NULL, \"\", subcat.name)) like lower(\"%\" || @F_SUBCATEGORY || \"%\") " +
                    "GROUP by it.id;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@F_NAME", f_name);
                command.Parameters.AddWithValue("@PRICE_MIN", min_price);
                command.Parameters.AddWithValue("@PRICE_MAX", max_price);
                command.Parameters.AddWithValue("@F_COUNTRY", f_country);
                command.Parameters.AddWithValue("@F_MANUFACTURER", f_manufacturer);
                command.Parameters.AddWithValue("@F_CATEGORY", f_category);
                command.Parameters.AddWithValue("@F_SUBCATEGORY", f_subcategory);
                using (var reader = command.ExecuteReader())
                {
                    CultureInfo en = new CultureInfo("en-US");
                    while (reader.Read())
                    {
                        items_list.Add(
                            new Item(
                            reader["name"] is DBNull? null: (string)reader["name"],
                            double.Parse((string)reader["price"],en),
                            reader["category"] is DBNull? null: (string)reader["category"],
                            reader["country"] is DBNull? null: (string)reader["country"],
                            reader["manufacturer"] is DBNull? null: (string)reader["manufacturer"],
                            reader["measurment_unit"] is DBNull? null: (string)reader["measurment_unit"],
                            double.Parse((string)reader["amount_in_item"],en), (Int64)reader["id"], 
                            reader["discount"] is System.DBNull? null: Convert.ToInt32((Int64)reader["discount"]),
                            reader["sub_category"] is System.DBNull ? null : (string)reader["sub_category"],
                            reader["comment"] is System.DBNull ? null : (string)reader["comment"],
                            reader["available_items"] is System.DBNull ? 0 : Convert.ToInt32((Int64)reader["available_items"])
                            )
                            );
                    }
                }
            }
        }

        //Користувачі

        public static bool InsertUser(in string login, in string password_hash, in string PIB,
            in string phone, in string position, in bool isadmin)
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
                {
                    connection.Open();
                    string command_string = "INSERT into employee(login, password_hash," +
                        " PIB,fk_position, phone_number, is_admin) values (@LOGIN, @PASSWORD_HASH," +
                        "@PIB, (select \"id\" from position where name = @POSITION), @PHONE, @ISADMIN)";
                    SqliteCommand command = new SqliteCommand(command_string, connection);
                    command.Parameters.AddWithValue("@LOGIN", login);
                    command.Parameters.AddWithValue("@PASSWORD_HASH", password_hash);
                    command.Parameters.AddWithValue("@PIB", PIB);
                    command.Parameters.AddWithValue("@POSITION", position);
                    command.Parameters.AddWithValue("@PHONE", phone);
                    command.Parameters.AddWithValue("@ISADMIN", isadmin ? 1 : 0);
                    command.ExecuteNonQuery();
                }
                
            }
            catch (Exception) { return false; }
            LogAction($"Створено користувача \"{PIB}\"");
            return true;
        }

        public static bool UpdateUser(in long Id, in string login, in string password_hash, in string PIB, 
            in string phone, in string position, in bool isadmin)
        {

            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "update employee set login=@LOGIN, " +
                    "password_hash=@PASSWORD_HASH, " +
                    "PIB=@PIB, " +
                    "fk_position=(select \"id\" from position where name = @POSITION), " +
                    "phone_number=@PHONE, " +
                    "is_admin=@ISADMIN where id = @EMPL_ID";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@LOGIN", login);
                command.Parameters.AddWithValue("@PASSWORD_HASH", password_hash);
                command.Parameters.AddWithValue("@PIB", PIB);
                command.Parameters.AddWithValue("@POSITION", position);
                command.Parameters.AddWithValue("@PHONE", phone);
                command.Parameters.AddWithValue("@ISADMIN", isadmin ? 1 : 0);
                command.Parameters.AddWithValue("@EMPL_ID", Id);
                command.ExecuteNonQuery();
            }
            LogAction($"Оновлено дані користувача \"{PIB}\"");
            return true;
        }

        public static bool LogIntoUser(in string login, in string password, out User user)
        {
            user = new();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "SELECT e.id as \"id\", e.login as \"login\"," +
                    " e.password_hash as \"password_hash\", e.PIB as \"PIB\"," +
                    " e.phone_number as \"phone_number\", p.name as \"position\"," +
                    " e.is_admin as \"is_admin\"" +
                    " FROM employee as e join position p on e.fk_position=p.id" +
                    " where e.login=@LOGIN AND e.password_hash=@PASSWORD_HASH;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@LOGIN", login);
                command.Parameters.AddWithValue("@PASSWORD_HASH", General.GetHash(password));
                Int64? user_id = (Int64?)command.ExecuteScalar();
                if (user_id is not null)
                {
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        user = new User((Int64)user_id, login, password, (string)reader["PIB"],
                            (string)reader["phone_number"], (string)reader["position"],
                            (Int64)reader["is_admin"] == 1 ? true : false);
                    }
                    return true;
                }
                else return false;
            }
        }


        public static void FillUserList(out List<User> user_list)
        {
            user_list = new();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "select empl.login as \"login\", empl.password_hash as \"pass\", " +
                    "empl.id as \"id\", " +
                    "empl.pib as \"PIB\", empl.phone_number as \"phone_number\", " +
                    "pos.name as \"position\", empl.is_admin as \"is_admin\" from employee as empl " +
                    "left join position as pos on pos.id=empl.fk_position;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user_list.Add(
                            new User(
                                (long)reader["id"],
                                (string)reader["login"],
                                (string)reader["pass"],
                                (string)reader["PIB"],
                                (string)reader["phone_number"],
                                (string)reader["position"],
                            (long)reader["is_admin"] == 1
                            ));
                    }
                }
            }
        }

        //Партії

        public static void GetBatchList(in long? it_id, out List<ItemBatch> list_batch)
        {
            list_batch = new();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "select id, expiry_date, amount, booked from item_batch where fk_item = @ID;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ID", it_id);
                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        list_batch.Add(new ItemBatch((long)reader["id"],
                            reader["expiry_date"] is not System.DBNull ? (string)reader["expiry_date"] : null,
                            reader["amount"] is System.DBNull ? 0 : Convert.ToInt32((Int64)reader["amount"]),
                            reader["booked"] is System.DBNull ? 0 : Convert.ToInt32((Int64)reader["booked"]),
                            it_id));
                    }
                }
            }
        }


        public static void InsertBatchToDB(in ItemBatch btch)
        {
            if (!btch.FK_Item.HasValue)
                throw new Exception("Партія без товару!");
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "INSERT INTO item_batch( fk_item, expiry_date, amount, booked) " +
                    "VALUES(@ID_ITEM, @EXP_DATE, @AMOUNT, @BOOKED)";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ID_ITEM", btch.FK_Item);
                command.Parameters.AddWithValue("@EXP_DATE", btch.ExpiryDate is null? DBNull.Value : btch.ExpiryDate);
                command.Parameters.AddWithValue("@AMOUNT", btch.Amount);
                command.Parameters.AddWithValue("@BOOKED", btch.Booked);
                command.ExecuteNonQuery();
            }
            LogAction($"Створено партію для товару \"{GetValueByID(DB_Tables.item, btch.FK_Item.Value, "name")}\"");
        }

        public static void UpdateBatch(in ItemBatch btch)
        {
            if (!btch.FK_Item.HasValue)
                throw new Exception("Партія без товару!");
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "UPDATE item_batch SET fk_item=@FK_ITEM, expiry_date=@EXP_DATE, " +
                    "amount=@AMOUNT, booked=@BOOKED where id=@ID;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ID", btch.Id);
                command.Parameters.AddWithValue("@FK_ITEM", btch.FK_Item);
                command.Parameters.AddWithValue("@EXP_DATE", btch.ExpiryDate is null ? DBNull.Value : btch.ExpiryDate);
                command.Parameters.AddWithValue("@AMOUNT", btch.Amount);
                command.Parameters.AddWithValue("@BOOKED", btch.Booked);
                command.ExecuteNonQuery();
            }
            LogAction($"Змінено вміст партії товару \"{GetValueByID(DB_Tables.item, btch.FK_Item.Value, "name")}\"");
        }


        //Замовлення

        public static long? CreateNewOrder(in Classes.Order my_order)
        {
            long? order_id;
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "INSERT INTO order_table(order_time, fk_employee, fk_status) " +
                    "VALUES(@ORD_TIME, @EMPLOYEE_ID, (SELECT id from order_status where name=@STATUS))";
                string command_string_id = " select seq from sqlite_sequence where name='order_table'";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                SqliteCommand command_id = new SqliteCommand(command_string_id, connection);
                command.Parameters.AddWithValue("@EMPLOYEE_ID", my_order.empl_id);
                command.Parameters.AddWithValue("@STATUS", my_order.FK_Status);
                command.Parameters.AddWithValue("@ORD_TIME", my_order.OrderTime);
                command.ExecuteNonQuery();
                order_id = (long?)command_id.ExecuteScalar();
            }
            LogAction($"Створено замовлення");
            return order_id;
        }

        public static string FormReciept(in Order my_ord)
        {
            if (!my_ord.Id.HasValue)
                throw new Exception("Замовлення не існує");

            string separator = "\n-------------------------------\n";

            List<ItemToOrder> recipe_i_to_o_list;
            FillItemToOrderList(out recipe_i_to_o_list, my_ord.Id.Value);
            for(int i = 0; i < recipe_i_to_o_list.Count;++i)
            {
                for (int j = i+1; j < recipe_i_to_o_list.Count; ++j)
                {
                    if (recipe_i_to_o_list[i].Name != recipe_i_to_o_list[j].Name)
                        continue;
                    recipe_i_to_o_list[i].Amount += recipe_i_to_o_list[j].Amount;
                    recipe_i_to_o_list.Remove(recipe_i_to_o_list[j]);
                    --j;
                }
            }
            System.Text.StringBuilder rec = new($"{CurrentSettings.ShopName}\n" +
                $"{CurrentSettings.ShopOwner}{separator}{CurrentSettings.ShopLocation}" +
                $"\nКасир: {my_ord.FK_Emplyee_PIB}{separator}");
            double ch_sum = 0;
            foreach (var item_to_order in recipe_i_to_o_list)
            {
                if (item_to_order.Amount == 0) continue;
                rec.Append($"{item_to_order.Name}\n   {item_to_order.PriceWithDiscount.ToString("F")} x {item_to_order.Amount}" +
                    $" =\n{Math.Round(item_to_order.PriceWithDiscount * item_to_order.Amount, 2).ToString("F")}\n");
                ch_sum += Math.Round(item_to_order.PriceWithDiscount * item_to_order.Amount, 2);
            }
            rec.Append($"\nCУМА  -  {ch_sum.ToString("F")}");
            rec.Append($"\n{separator}{my_ord.OrderTime}");
            return rec.ToString();
        }

        public static void FinishOrder(in Classes.Order my_order, in List<Classes.ItemToOrder> itoo_list)
        {
            if (!my_order.Id.HasValue)
                throw new Exception("No order IDs");
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "UPDATE order_table SET fk_status = " +
                    "(SELECT id from order_status where name=\"Завершено\") WHERE id=@ORDER_ID;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ORDER_ID", my_order.Id);
                command.ExecuteNonQuery();

                command_string = "UPDATE item_batch SET booked=booked - @BOOKED_CH WHERE id=@ID";
                command = new SqliteCommand(command_string, connection);
                foreach (var i in itoo_list)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@BOOKED_CH", i.Amount);
                    command.Parameters.AddWithValue("@ID", i.Batch_ID);
                    command.ExecuteNonQuery();
                }
            }
            LogAction($"Замовлення продано");
        }

        public static void FillItemToOrderList(out List<ItemToOrder> item_to_order_list, in long? ord_id)
        {
            item_to_order_list = new();
            if (!ord_id.HasValue)
                throw new Exception("No order ID");
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "SELECT i_to_o.id as \"id\", it.name as \"name\", printf(\"%g\"," +
                    "(pr_h.price-(pr_h.price/100.0*pr_h.discount))) as \"price\", sum(i_to_o.amount) as \"amount\", " +
                    "batch.expiry_date as \"expiry_date\", batch.id as \"batch_id\"  FROM item as it " +
                    "LEFT join item_batch as batch on it.id = batch.fk_item " +
                    "left join items_to_order as i_to_o on i_to_o.fk_batch=batch.id " +
                    "left join price_history as pr_h on i_to_o.fk_price_history=pr_h.id " +
                    "left join order_table as ord on ord.id=i_to_o.fk_order where ord.id = @ORDER_ID GROUP BY i_to_o.fk_batch;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ORDER_ID", ord_id);
                using (var reader = command.ExecuteReader())
                {
                    CultureInfo en = new CultureInfo("en-US");
                    while (reader.Read())
                    {
                        item_to_order_list.Add(
                            new ItemToOrder((Int64)reader["id"], reader["name"] is DBNull? "": (string)reader["name"],
                            double.Parse((string)reader["price"], en),
                            Convert.ToInt32((Int64)reader["amount"]),
                            reader["expiry_date"] is DBNull? "" : (string)reader["expiry_date"], (Int64)reader["batch_id"])
                            );
                    }
                }
            }
        }

        public static void FillOrderList(out List<Order> item_to_order_list)
        {
            item_to_order_list = new();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_string = "SELECT ord.id as \"id\", ord.order_time as \"ord_time\", " +
                    "empl.PIB as \"PIB\", os.name as \"status\", empl.id as \"emplid\" " +
                    "FROM order_table as ord left join employee as empl on ord.fk_employee=empl.id " +
                    "left join order_status as os on os.id=ord.fk_status";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                using (var reader = command.ExecuteReader())
                {
                    CultureInfo en = new CultureInfo("en-US");
                    while (reader.Read())
                    {
                        Order tmp = new((long)reader["id"],
                            (string)reader["PIB"], 
                            (string)reader["ord_time"],
                            (string)reader["status"], (long)reader["emplid"]);
                        item_to_order_list.Add(tmp);
                    }
                }
            }
        }

        public static void InsertITOToDB(in ItemToOrder itoo, in long order_id)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();

                string command_string = "select it.fk_price_history from item_batch as it_b " +
                    "left join item as it on it.id = it_b.fk_item where it_b.id = @BATCH_ID;";
                SqliteCommand command = new(command_string, connection);
                command.Parameters.AddWithValue("@BATCH_ID", itoo.Batch_ID);
                long? price_history_id = (long?)command.ExecuteScalar();

                command_string = "insert into items_to_order(fk_order,fk_batch,amount, fk_price_history)" +
                    " VALUES (@ORDER_ID, @BATCH_ID ,@AMOUNT, @PRICE_H)";
                command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@ORDER_ID", order_id);
                command.Parameters.AddWithValue("@BATCH_ID", itoo.Batch_ID);
                command.Parameters.AddWithValue("@AMOUNT", itoo.Amount);
                command.Parameters.AddWithValue("@PRICE_H", price_history_id);
                command.ExecuteNonQuery();

                command_string = "UPDATE item_batch SET amount=amount - @BOOKED_CH, booked=booked + @BOOKED_CH WHERE id=@ID;";
                command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@BOOKED_CH", itoo.Amount);
                command.Parameters.AddWithValue("@ID", itoo.Batch_ID);
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateITO(in ItemToOrder itoo)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "UPDATE items_to_order SET fk_batch=@FK_BATCH, amount=@AMOUNT WHERE id=@ID;";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@FK_BATCH", itoo.Batch_ID);
                command.Parameters.AddWithValue("@AMOUNT", itoo.Amount);
                command.Parameters.AddWithValue("@ID", itoo.ID);
                command.ExecuteNonQuery();

                command_string = "UPDATE item_batch SET amount=amount - @BOOKED_CH, booked=booked + @BOOKED_CH WHERE id=@ID";
                command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@BOOKED_CH", itoo.Amount - itoo.start_amount);
                command.Parameters.AddWithValue("@ID", itoo.Batch_ID);
                command.ExecuteNonQuery();
            }
        }

        public static void UnbookItem(in ItemToOrder itoo)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_string = "UPDATE item_batch SET amount=amount + @BOOKED_CH, booked=booked - @BOOKED_CH WHERE id=@ID";
                SqliteCommand command = new SqliteCommand(command_string, connection);
                command.Parameters.AddWithValue("@BOOKED_CH", itoo.Amount);
                command.Parameters.AddWithValue("@ID", itoo.Batch_ID);
                command.ExecuteNonQuery();
                if(itoo.ID.HasValue)
                    DeleteFromDB(itoo.ID.Value, DB_Tables.items_to_order);
            }
        }

        //Категорії

        public static void GetSubcategory_ID_Dictionary(out Dictionary<string, Int64> value_id, in string cat)
        {
            value_id = new Dictionary<string, Int64>();
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadOnly;"))
            {
                connection.Open();
                string command_str = "select * from sub_category where fk_category =  (select \"id\" from category where name = @CATEGORY)";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                command.Parameters.AddWithValue("@CATEGORY", cat);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        value_id[(string)reader[1]] = (Int64)reader[0];
                }
            }
        }

        public static void InsertSubcategory(in string cat_name, in string new_subcat)
        {
            using (var connection = new SqliteConnection($"Data Source={DB_Path};Mode=ReadWrite;"))
            {
                connection.Open();
                string command_str = "INSERT into sub_category(name,fk_category) " +
               "values (\"@NEW_SUB\", (select \"id\" from category where " +
               "name = \"@CAT\"))";
                SqliteCommand command = new SqliteCommand(command_str, connection);
                command.Parameters.AddWithValue("@NEW_SUB", new_subcat);
                command.Parameters.AddWithValue("@CAT", cat_name);
                command.ExecuteNonQuery();
            }
           

        }


    }
}
