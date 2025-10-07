using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class ItemBatch
    {
        public long? Id { get; set; }
        private DateTime? exp_date;
        public long? FK_Item { get; set; }
        public string? ExpiryDate
        {
            get 
            { 
                if(exp_date is null)
                    return null;
                else
                    return exp_date.Value.ToString("yyyy-MM-dd"); 
            }

            set
            {
                if (value is null)
                    exp_date = null;
                else
                    exp_date = DateTime.ParseExact(value, "yyyy-MM-dd", null);

            }
        }

        public DateTime? GetDate() { return this.exp_date; }
        public void SetDate(DateTime? dt) { this.exp_date = dt; }

        private int amount;
        public int Amount {
            get => amount;
            set
            {
                if (value >= 0)
                    amount = value;
                else throw new Exception("Кількість меньше нуля");
            } 
        }
        private int booked;
        public int Booked
        {
            get => booked;
            set
            {
                if (value >= 0)
                    booked = value;
                else throw new Exception("Кількість меньше нуля");
            }
        }

        public ItemBatch(long? n_id, string? n_expirydate, int n_amount, int n_booked, long? fk_it)
        {
            Id = n_id;
            ExpiryDate = n_expirydate;
            Amount = n_amount;
            Booked = n_booked;
            FK_Item = fk_it;
        }
    }
}
