using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace HealthyFood_Shop
{
    public static class General
    {
        public static string checks_path { get; private set; } = Directory.GetCurrentDirectory() + @"\all_checks\";
        //Перевіряє, чи строка порожня.
        public static bool ValidateString([NotNullWhen(true)] in string? str)
        {
            if (str is not null)
            {
                if (str.Trim().Length == 0)
                    return false;
            }
            else return false;
            return true;
        }

        public static bool ValidatePriceString([NotNullWhen(true)] in string? str)
        {
            if (String.IsNullOrEmpty(str))
                return false;
            string my_str = str.Replace(',', '.');

            if (!Regex.IsMatch(my_str, @"^\p{Nd}+(\.\p{Nd}\p{Nd}?)?$"))
                return false;

            CultureInfo en = new CultureInfo("en-US");
            double tmp;
            if (!double.TryParse(my_str, NumberStyles.AllowDecimalPoint, en, out tmp))
                return false;

            if (tmp == 0.0)
                return false;

            return true;
        }

        public static string GetHash(in string inp)
        {
            var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(inp)));
        }


        public static void GetObjectFromContextMenu<T>(object sender, out T obj)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            obj = (T)item.Item;
        }

        public static int DBToInt32(in object DB_Value)
        {
            return Convert.ToInt32((Int64)DB_Value);
        }

        public static string DBToString(in object DB_Value)
        {
            if (DB_Value is DBNull || DB_Value is null)
                return String.Empty;

            if(DB_Value is string)
                return (string)DB_Value;
            if (DB_Value is long)
                return ((long)DB_Value).ToString();

            return String.Empty;
        }

        public static void CreateZReport(out string rep, in string user_pib, in DateTime time,
            in double sum, in int checks)
        {
            int sep_len = 40;
            string separator = "\n" + new string('-', sep_len) + "\n";

            StringBuilder strb_report = new(CenterString(CurrentSettings.ShopName, sep_len, ' '));
            strb_report.Append($"\n{CurrentSettings.ShopOwner}\n{CurrentSettings.ShopLocation}{separator}");
            strb_report.AppendLine(CenterString("Z-ЗВІТ", sep_len));
            strb_report.AppendLine($"\nПродаж{separator}Кількість чеків  -  {checks}");
            strb_report.AppendLine($"Загальна сума    -  {sum}");
            strb_report.AppendLine($"ПДВ А = 20%      -  {(sum/6.0).ToString("F")}");
            strb_report.Append($"{separator}\n");
            strb_report.AppendLine($"Касир  -  {user_pib}");
            strb_report.Append($"Час    -  {time.ToString("yyyy-MM-dd HH:mm:ss")}");
            rep = strb_report.ToString();
        }

        public static string CenterString(in string str, in int total_len, in char sep_char = ' ')
        {
            int pad_len = (total_len - str.Length/2) / 2;
            if(pad_len < 0)
                return str;

            string pads = new string(sep_char, pad_len);
            return pads + str + pads;
        }

        public static void ChangeCurrentPage<new_page_type>
            (ref Grid panel_for_page, in string page_name, ref Label l_page_n)
            where new_page_type : System.Windows.UIElement
        {
            panel_for_page.Children.Clear();
            panel_for_page.Children.Add(Activator.CreateInstance<new_page_type>());
            l_page_n.Content = page_name;
        }
        public static void ChangeCurrentPage<new_page_type>
            (ref Grid panel_for_page)
            where new_page_type : System.Windows.UIElement
        {
            panel_for_page.Children.Clear();
            panel_for_page.Children.Add(Activator.CreateInstance<new_page_type>());
        }


    }
}
