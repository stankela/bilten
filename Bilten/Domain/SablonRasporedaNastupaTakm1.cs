using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace Bilten.Domain
{
    public class SablonRasporedaNastupaTakm1 : DomainObject
    {
        private RasporedNastupa rasporedNastupa;
        public virtual RasporedNastupa RasporedNastupa
        {
            get { return rasporedNastupa; }
            set { rasporedNastupa = value; }
        }

        private byte grupa;
        public virtual byte Grupa
        {
            get { return grupa; }
            set { grupa = value; }
        }

        private ISet<SablonRasporedaNastupaTakm1Item> items =
            new HashedSet<SablonRasporedaNastupaTakm1Item>();
        public virtual ISet<SablonRasporedaNastupaTakm1Item> Items
        {
            get { return items; }
            set { items = value; }
        }

        public virtual void addItem(SablonRasporedaNastupaTakm1Item item)
        {
            if (existsEkipa(item.Ekipa))
                return;

            item.RedBroj = (byte)(getMaxRedBroj(item.Sprava) + 1);
            Items.Add(item);
        }

        public virtual void removeItem(SablonRasporedaNastupaTakm1Item item)
        {
            if (Items.Remove(item))
            {
                renumber(item.Sprava);
            }
        }

        private void renumber(Sprava sprava)
        {
            List<SablonRasporedaNastupaTakm1Item> items = getItems(sprava);
            items.Sort(
            delegate(SablonRasporedaNastupaTakm1Item i1, 
                SablonRasporedaNastupaTakm1Item i2)
            {
                return i1.RedBroj.CompareTo(i2.RedBroj);
            });
            for (int i = 0; i < items.Count; i++)
            {
                items[i].RedBroj = (byte)(i + 1);
            }
        }

        private List<SablonRasporedaNastupaTakm1Item> getItems(Sprava sprava)
        {
            List<SablonRasporedaNastupaTakm1Item> result = 
                new List<SablonRasporedaNastupaTakm1Item>();
            foreach (SablonRasporedaNastupaTakm1Item item in Items)
            {
                if (item.Sprava == sprava)
                    result.Add(item);
            }
            return result;
        }

        public virtual bool existsEkipa(Ekipa ekipa)
        {
            foreach (SablonRasporedaNastupaTakm1Item item in Items)
            {
                // TODO: Mozda bi trebalo implementirati Equals u klasi Ekipa.
                // Ova provera ne radi dobro ako ekipa jos nije snimljena u bazu.
                if (item.Ekipa.Id == ekipa.Id)
                    return true;
            }
            return false;
        }

        private byte getMaxRedBroj(Sprava sprava)
        {
            byte result = 0;
            foreach (SablonRasporedaNastupaTakm1Item item in Items)
            {
                if (item.Sprava == sprava && item.RedBroj > result)
                    result = item.RedBroj;
            }
            return result;
        }

        public virtual byte getMaxRedBroj()
        {
            byte result = 0;
            foreach (SablonRasporedaNastupaTakm1Item item in Items)
            {
                if (item.RedBroj > result)
                    result = item.RedBroj;
            }
            return result;
        }

        public virtual SablonRasporedaNastupaTakm1Item getItem(Sprava sprava, int redBroj)
        {
            foreach (SablonRasporedaNastupaTakm1Item item in Items)
            {
                if (item.Sprava == sprava && item.RedBroj == redBroj)
                    return item;
            }
            return null;
        }

        public override void validate(Notification notification)
        {

        }
    }
}
