using Bilten.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class OcenaBase : DomainObject
    {
        private Nullable<float> d;
        public virtual Nullable<float> D
        {
            get { return d; }
            set { d = value; }
        }

        private Nullable<float> e1;
        public virtual Nullable<float> E1
        {
            get { return e1; }
            set { e1 = value; }
        }

        private Nullable<float> e2;
        public virtual Nullable<float> E2
        {
            get { return e2; }
            set { e2 = value; }
        }

        private Nullable<float> e3;
        public virtual Nullable<float> E3
        {
            get { return e3; }
            set { e3 = value; }
        }

        private Nullable<float> e4;
        public virtual Nullable<float> E4
        {
            get { return e4; }
            set { e4 = value; }
        }

        private Nullable<float> e5;
        public virtual Nullable<float> E5
        {
            get { return e5; }
            set { e5 = value; }
        }

        private Nullable<float> e6;
        public virtual Nullable<float> E6
        {
            get { return e6; }
            set { e6 = value; }
        }

        private Nullable<float> e;
        public virtual Nullable<float> E
        {
            get { return e; }
            set { e = value; }
        }

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            set { penalty = value; }
        }

        private Nullable<float> total;
        public virtual Nullable<float> Total
        {
            get { return total; }
            set { total = value; }
        }

        private byte brojEOcena;
        public virtual byte BrojEOcena
        {
            get { return brojEOcena; }
            set { brojEOcena = value; }
        }

        // NOTE: Kada RucnoUnetaOcena ima vrednost true, to znaci da se Total ne
        // izracunava vec da je rucno unesen. Ostali delovi ocene (D, E1, E2, ..., E)
        // mogu ali i ne moraju da se unesu; oni nemaju uticaja na Total.
        private bool rucnoUnetaOcena = false;
        public virtual bool RucnoUnetaOcena
        {
            get { return rucnoUnetaOcena; }
            set { rucnoUnetaOcena = value; }
        }

        private string prefix = String.Empty;
        public virtual string ValidationPrefix
        {
            set { prefix = value; }
        }

        public override void validate(Notification notification)
        {
            validateZaIzracunavanje(notification);

            if (E == null)
            {
                if (!RucnoUnetaOcena)
                {
                    notification.RegisterMessage(
                        prefix + "E", "E ocena je obavezna.");
                }
            }
            else if (E < 0)
            {
                notification.RegisterMessage(
                    prefix + "E", "E ocena ne sme da bude negativna.");
            }

            if (Total == null)
            {
                notification.RegisterMessage(
                    prefix + "Total", "Konacna ocena je obavezna.");
            }
            else if (Total.Value < 0)
            {
                notification.RegisterMessage(
                    prefix + "Total", "Konacna ocena ne sme da bude negativna.");
            }
        }

        public virtual void validateZaIzracunavanje(Notification notification)
        {
            if (D == null)
            {
                if (!RucnoUnetaOcena)
                {
                    notification.RegisterMessage(
                        prefix + "D", "D ocena je obavezna.");
                }
            }
            else if (D < 0)
            {
                notification.RegisterMessage(
                    prefix + "D", "D ocena ne sme da bude negativna.");
            }

            Nullable<float>[] eOcene = new Nullable<float>[6] { E1, E2, E3, E4, E5, E6 };
            for (byte i = 1; i <= BrojEOcena; i++)
            {
                validateEOcena(notification, eOcene[i - 1], i);
            }

            if (Penalty != null)
            {
                if (Penalty < 0)
                {
                    notification.RegisterMessage(
                        prefix + "Penalty", "Penalizacija ne sme da bude negativna.");
                }
                else if (E != null && Penalty > E)
                {
                    //notification.RegisterMessage(
                      //  prefix + "Penalty", "Penalizacija ne sme da bude veca od E ocene.");
                }
            }

            // TODO: Fali validacija za L, T, O i Vreme
        }

        private void validateEOcena(Notification notification, Nullable<float> eOcena,
            byte broj)
        {
            string propName = String.Format("E{0}", broj);
            if (eOcena == null)
            {
                if (!RucnoUnetaOcena)
                {
                    notification.RegisterMessage(
                        prefix + propName, propName + " ocena je obavezna.");
                }
            }
            else if (eOcena < 0)
            {
                notification.RegisterMessage(
                        prefix + propName, propName + " ocena ne sme da bude negativna.");
            }
        }

        private float izracunajEOcenu(int brojDecimala)
        {
            if (BrojEOcena == 0)
                return E.Value;

            Nullable<float>[] eOcene = new Nullable<float>[6] { E1, E2, E3, E4, E5, E6 };
     
            int min = getMinEOcenaBroj() - 1;
            int max = getMaxEOcenaBroj() - 1;
            decimal result = 0;
            for (int i = 0; i < BrojEOcena; i++)
            {
                if (i != min && i != max)
                    result += (decimal)eOcene[i].Value;
            }

            if (min != max)
            {
                result = result / (BrojEOcena - 2);
            }
            else
            {
                // sve e ocene su jednake, i result sadrzi zbir svih ocena sem jedne
                result = result / (BrojEOcena - 1);
            }
            return (float)RounderToZero.round(result, brojDecimala);
        }

        public virtual int getMinEOcenaBroj()
        {
            Nullable<float>[] eOcene = new Nullable<float>[6] { E1, E2, E3, E4, E5, E6 };
            int min = 0;
            for (int i = 1; i < BrojEOcena; i++)
            {
                if (eOcene[i].Value < eOcene[min].Value)
                    min = i;
            }
            return min + 1;
        }

        public virtual int getMaxEOcenaBroj()
        {
            Nullable<float>[] eOcene = new Nullable<float>[6] { E1, E2, E3, E4, E5, E6 };
            int max = 0;
            for (int i = 1; i < BrojEOcena; i++)
            {
                if (eOcene[i].Value > eOcene[max].Value)
                    max = i;
            }
            return max + 1;
        }

        public virtual void izracunajOcenu(int brojDecimalaE, int brojDecimalaPen,
            int brojDecimalaTotal)
        {
            E = izracunajEOcenu(brojDecimalaE);
            Total = (float)RounderToZero.round((decimal)D.Value + (decimal)E.Value -
                (decimal)((Penalty != null) ? Penalty.Value : 0), brojDecimalaTotal);
            if (Total < 0)
                Total = 0;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(D != null ? D.Value.ToString() : NULL);
            strBuilder.AppendLine(E1 != null ? E1.Value.ToString() : NULL);
            strBuilder.AppendLine(E2 != null ? E2.Value.ToString() : NULL);
            strBuilder.AppendLine(E3 != null ? E3.Value.ToString() : NULL);
            strBuilder.AppendLine(E4 != null ? E4.Value.ToString() : NULL);
            strBuilder.AppendLine(E5 != null ? E5.Value.ToString() : NULL);
            strBuilder.AppendLine(E6 != null ? E6.Value.ToString() : NULL);            
            strBuilder.AppendLine(E != null ? E.Value.ToString() : NULL);
            strBuilder.AppendLine(Penalty != null ? Penalty.Value.ToString() : NULL);
            strBuilder.AppendLine(Total != null ? Total.Value.ToString() : NULL);
            strBuilder.AppendLine(BrojEOcena.ToString());
            strBuilder.AppendLine(RucnoUnetaOcena.ToString());
        }

        public virtual void loadFromDump(StringReader reader)
        {
            string line = reader.ReadLine();
            D = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E1 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E2 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E3 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E4 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E5 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E6 = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Penalty = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Total = line != NULL ? float.Parse(line) : (float?)null;

            BrojEOcena = byte.Parse(reader.ReadLine());
            RucnoUnetaOcena = bool.Parse(reader.ReadLine());
        }
    }
}
