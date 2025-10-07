using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class ItemToOrder
    {
        public long? ID { get; set; } = null;
        public long Batch_ID { get; set; }
        public string Name { get; set; }
        public int start_amount;
        public int Amount { get; set; }
        private double price_with_discount;
        public double PriceWithDiscount 
        { get => Math.Round(price_with_discount, 2);
            set {price_with_discount = value;} }
        public string FullPriceStr
        { get => (Math.Round(price_with_discount, 2)*Amount).ToString("F");}
        public string BatchExpiryDate { get; set; }
        public ItemToOrder()
        {
            start_amount = 0;
            Name = String.Empty;
            Amount = 0;
            BatchExpiryDate = String.Empty;
        }
        public ItemToOrder(in long id, in string name, in double price_w_discount, int am,
            in string exp_date, in long batch_id)
        {
            ID = id;
            Name = name;
            Amount = am;
            start_amount = am;
            BatchExpiryDate = exp_date;
            price_with_discount = price_w_discount;
            Batch_ID = batch_id;
        }
    }
}
