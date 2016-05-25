using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading;
using System.Threading.Tasks;

namespace hm3v2
{

    public struct CurrentTypes
    {
        public static Dictionary<int, String> dictionary;

        public static int addType(String type)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, string>();

            if (!dictionary.ContainsValue(type))
            {
                dictionary.Add(dictionary.Count, type);
                return dictionary.Count - 1;
            }

            return dictionary.FirstOrDefault(x => x.Value.Equals(type)).Key;
        }

        public static String getType(int i) => (dictionary != null && dictionary.ContainsKey(i)) ? dictionary[i] : "Unnamed";
    }

    public struct Money
    {
        public decimal value;
        public int currencyType;

        public Money(decimal value, int currencyType)
        {
            this.value = value;
            this.currencyType = (CurrentTypes.getType(currencyType) != null) ? currencyType : -1;
        }

        public Money(decimal value, String currencyType)
        {
            this.value = value;
            this.currencyType = CurrentTypes.addType(currencyType);
        }

        public Money(Money money)
        {
            this.value = money.value;
            this.currencyType = money.currencyType;
        }

        public static Money operator +(Money a, Money b)
            => (a.currencyType == b.currencyType && a.currencyType != -1) ? new Money(a.value + b.value, a.currencyType) : new Money(0, -1);

        public static Money operator -(Money a, Money b)
            => (a.currencyType == b.currencyType && a.currencyType != -1) ? new Money(a.value - b.value, a.currencyType) : new Money(0, -1);

        public static Money operator *(Money a, decimal i)
            => (a.currencyType != -1) ? new Money(a.value * i, a.currencyType) : new Money(0, -1);

        public static Money operator /(Money a, decimal i)
            => (i != 0 && a.currencyType != -1) ? new Money(a.value / i, a.currencyType) : new Money(0, -1);

        public override int GetHashCode() => (value.GetHashCode() * 397) ^ currencyType.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Money money = (Money)obj;

            return (value == money.value && currencyType == money.currencyType);
        }

        public override string ToString() => (currencyType != -1) ? value + " " + CurrentTypes.getType(currencyType) : "Unadditional values";
    }

    public class Payment
    {

        public Dictionary<int, decimal> payments;

        public Payment(Money money)
        {
            payments = new Dictionary<int, decimal>();
            if (money.currencyType != -1)
                payments.Add(money.currencyType, money.value);
        }

        public Payment(Payment payment)
        {
            payments = new Dictionary<int, decimal>();
            foreach (KeyValuePair<int, decimal> pair in payment.payments)
            {
                payments.Add(pair.Key, pair.Value);
            }
        }

        public override int GetHashCode()
        {
            int hash = 1;

            if (payments != null)
            {
                foreach (KeyValuePair<int, decimal> pair in payments)
                {
                    hash = (hash * 397) ^ pair.GetHashCode();
                }
            }
            return hash;
        }

        public override string ToString()
        {
            String s = "";

            foreach (KeyValuePair<int, decimal> pair in payments)
            {
                if (pair.Value != 0)
                    s += pair.Value + " " + CurrentTypes.getType(pair.Key) + "\n";
            }

            return s;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Payment payment = (Payment)obj;

            if (payments == null && payment.payments == null)
                return true;

            if ((payments == null ^ payment.payments == null) || payments.Count != payment.payments.Count)
                return false;

            Boolean equal = true;

            try
            {
                foreach (KeyValuePair<int, decimal> pair in payments)
                {
                    if (payment.payments[pair.Key] != pair.Value)
                    {
                        equal = false;
                        break;
                    }
                }
                return equal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static Payment operator +(Payment p1, Payment p2)
        {
            Payment sum = new Payment(p1);
            foreach (KeyValuePair<int, decimal> pair in p2.payments)
            {
                if (pair.Key != -1 && sum.payments.ContainsKey(pair.Key))
                {
                    sum.payments[pair.Key] += pair.Value;
                }
                else
                {
                    sum.payments.Add(pair.Key, pair.Value);
                }
            }

            return sum;
        }

        public static Payment operator -(Payment p1, Payment p2)
        {
            Payment sum = new Payment(p1);
            foreach (KeyValuePair<int, decimal> pair in p2.payments)
            {
                if (pair.Key != -1 && sum.payments.ContainsKey(pair.Key))
                {
                    sum.payments[pair.Key] -= pair.Value;
                }
                else
                {
                    sum.payments.Add(pair.Key, -pair.Value);
                }
            }

            return sum;
        }

        public static Payment operator /(Payment p1, int i)
        {
            if (i == 0) return new Payment(new Money(0, -1));
            Payment sum = new Payment(p1);
            foreach (KeyValuePair<int, decimal> pair in sum.payments)
            {
                if (pair.Key != -1) sum.payments[pair.Key] /= i;
            }

            return sum;
        }

        public static Payment operator *(Payment p1, int i)
        {
            Payment sum = new Payment(p1);
            foreach (KeyValuePair<int, decimal> pair in sum.payments)
            {
                if (pair.Key != -1) sum.payments[pair.Key] /= i;
            }

            return sum;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Money m1 = new Money(15, "EUR");
            Money m2 = new Money(20, "EUR");
            Money m3 = m1 + m2;
            Console.WriteLine(m3);
            m3 = m1 - m2;
            Console.WriteLine(m3);
            m3 = m1 * 2 - m2 / 4;
            Console.WriteLine(m3);
            Console.WriteLine(m2.Equals(m1));
            m2 = new Money(20, "RUB");
            Console.WriteLine(m2.Equals(m1));
            m3 = m1 + m2;
            Console.WriteLine(m3);


            Payment p1 = new Payment(m1);
            Payment p2 = new Payment(m2);
            Payment p3 = p1 + p2;
            Console.WriteLine(p3);
            p3 = p3 - p2;
            Console.WriteLine(p3);
            p2 = new Payment(new Money(15, "EUR"));
            Console.WriteLine(p2.Equals(p1));
        }


    }
}