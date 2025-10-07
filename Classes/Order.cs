using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class Order
    {
        public long? Id { get; set; }
        public long? empl_id { get; set; }
        public string FK_Emplyee_PIB { get; set; }
        public string FK_Status{ get; set; }

        private DateTime order_time;
        public string OrderTime
        {
            get
            {
                  return order_time.ToString("yyyy-MM-dd HH:mm:ss");
            }

            set
            {
                order_time = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public Order(in long? id, in string empl_pib, in string ord_time, in string ord_status, long? employee_id=null)
        {
            this.Id = id;
            this.OrderTime = ord_time;
            this.FK_Emplyee_PIB = empl_pib;
            this.FK_Status = ord_status;
            this.empl_id = employee_id;
        }

        public Order()
        {
            Id = null;
            this.order_time = DateTime.Now;
            this.FK_Emplyee_PIB = String.Empty;
            this.FK_Status = String.Empty;
            this.empl_id = null;
        }

        public string GetCheckName()
        {
            return this.order_time.ToString("yyyy-MM-dd HH-mm-ss") + " n" + this.Id.ToString();
        }

        public string ReadCheckFromFile()
        {
            try
            {
                using (StreamReader readtext =
                    new(General.checks_path + this.GetCheckName() + ".txt"))
                {
                    return readtext.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool WriteCheckToFile()
        {
            string check = SQLite_Interaction.FormReciept(this);
            try
            {
                using (StreamWriter writetext =
                    new StreamWriter(General.checks_path + this.GetCheckName() + ".txt"))
                {
                    writetext.Write(check);
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

    }
}
