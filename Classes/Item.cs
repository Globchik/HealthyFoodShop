using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HealthyFood_Shop.Classes
{
    /// <summary></summary>
    public class Item
    {
        private Int64? id = null;
        public Int64? ID { get => id; }

        private string? name;
        public string? Name
        {
            get { return this.name; }
            set
            {
                if (value is null) this.name = value;
                else if (General.ValidateString(value))
                    this.name = value;
                else throw new ArgumentException("Порожнє Ім'я\n");
            }
        }
        private double price;
        public double Price
        {
            get { return this.price; }
            [MemberNotNull(nameof(price))]
            set
            {
                if ((double?)value is not null || value > 0)
                    this.price = value;
                else throw new ArgumentException("Невалідна ціна\n");
            }
        }

        private int available_items;
        public int AvailableItems
        {
            get => available_items;
            private set { available_items = value; }
        }

        private string? fk_category;
        public string? FK_Category
        {
            get { return this.fk_category; }
            set
            {
                if (value is null) this.fk_category = value;
                else if (General.ValidateString(value) &&
                    SQLite_Interaction.IsValueInTable(value, SQLite_Interaction.DB_Tables.category))
                    this.fk_category = value;
                else throw new ArgumentException("Невалідна категорія\n");
            }
        }
        private string? fk_sub_category;
        public string? FK_SubCategory
        {
            get { return this.fk_sub_category; }
            set
            {
                if (value is null) this.fk_sub_category = value;
                else if (value is null)
                    this.fk_sub_category = null;
                else if (SQLite_Interaction.IsValueInTable(value, SQLite_Interaction.DB_Tables.sub_category))
                    this.fk_sub_category = value;
                else throw new Exception("Невалідна підкатегорія");
            }
        }
        private string? fk_country;
        public string? FK_Country
        {
            get { return this.fk_country; }
            set
            {
                if (value is null) this.fk_country = value;
                else if (General.ValidateString(value) &&
                    SQLite_Interaction.IsValueInTable(value, SQLite_Interaction.DB_Tables.country))
                    this.fk_country = value;
                else throw new ArgumentException("Невалідна країна\n");
            }
        }
        private string? fk_manufacturer;
        public string? FK_Manufacturer
        {
            get { return this.fk_manufacturer; }
            set
            {
                if (value is null) this.fk_manufacturer = value;
                else if (General.ValidateString(value) &&
                    SQLite_Interaction.IsValueInTable(value, SQLite_Interaction.DB_Tables.manufacturer))
                    this.fk_manufacturer = value;
                else throw new ArgumentException("Невалідний виробник\n");
            }
        }
        private string? fk_measurment_unit;
        public string? FK_MeasurmentUnit
        {
            get { return this.fk_measurment_unit; }
            set
            {
                if (value is null) this.fk_measurment_unit = value;
                else if (General.ValidateString(value) &&
                    SQLite_Interaction.IsValueInTable(value, SQLite_Interaction.DB_Tables.measurment_unit))
                    this.fk_measurment_unit = value;
                else throw new ArgumentException("Невалідна одиниця виміру\n");
            }
        }
        private double amount_in_item;
        public double AmountInItem
        {
            get { return this.amount_in_item; }
            [MemberNotNull(nameof(amount_in_item))]
            set
            {
                if ((double?)value is not null && value > 0)
                    this.amount_in_item = value;
                else throw new ArgumentException("Невалідна кількість\n");
            }
        }
        private int? discount;
        public int? Discount
        {
            get { return this.discount; }
            set
            {
                if (value is not null)
                {
                    if (value >= 0 && value < 100)
                        this.discount = value;
                    else throw new ArgumentException("Невалідна знижка\n");
                }
            }
        }
        private string? comment;
        public string? Comment
        {
            get { return this.comment; }
            set
            {
                this.comment = value;
            }
        }

        public string? ShortComment
        {
            get
            {
                if (comment is null) return null;
                if(comment.Length <= 13) return comment;
                return comment.Substring(0, 13) + "......";
            }
        }

        public string? FullAmount
        {
            get { return this.amount_in_item + " " + this.fk_measurment_unit; }
        }

        public double CurrentPrice
        {
            get { return Math.Round(this.price * (this.discount is null ? 1.0 : ((100 - (double)discount) / 100.0)), 2); }
        }

        public string CurrentPriceString
        {
            get { return Math.Round(this.price * (this.discount is null ? 1.0 : ((100 - (double)discount) / 100.0)), 2).ToString("F"); }
        }


        public string? PercentDiscount
        {
            get
            {
                int d = this.discount is null ? 0 : (int)this.discount;
                return d.ToString() + "%";
            }
        }

        public Item(in string? n_name, in double n_price,
            in string? n_category, in string? n_country,
            in string? n_manufacturer, in string? n_mes_unit,
            in double n_amount_in_it, in Int64? n_id = null,
            int? n_discount = null, in string? n_subcategory = null,
            in string? n_comment = null, int n_available_items = 0)
        {
            this.Name = n_name;
            this.Price = n_price;
            this.FK_Category = n_category;
            this.FK_Country = n_country;
            this.FK_Manufacturer = n_manufacturer;
            this.FK_MeasurmentUnit = n_mes_unit;
            this.AmountInItem = n_amount_in_it;
            this.id = n_id;
            this.Discount = n_discount;
            this.FK_SubCategory = n_subcategory;
            this.Comment = n_comment;
            this.AvailableItems = n_available_items;
        }

        public Item()
        {
            name = String.Empty;
            fk_category = String.Empty;
            fk_country = String.Empty;
            fk_manufacturer = String.Empty;
            fk_measurment_unit = String.Empty;
        }

        public bool IsItemValid()
        {
            return Name is not null && (double?)Price is not null && FK_Category is not null &&
                FK_Country is not null && FK_Manufacturer is not null &&
                FK_MeasurmentUnit is not null && (double?)AmountInItem is not null;
        }
    }
}
