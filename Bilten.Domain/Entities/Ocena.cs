using Bilten.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class Ocena : OcenaBase
    {
        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        private DeoTakmicenjaKod deoTakmicenjaKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakmicenjaKod; }
            set { deoTakmicenjaKod = value; }
        }

        private Nullable<float> totalObeOcene;
        public virtual Nullable<float> TotalObeOcene
        {
            get { return totalObeOcene; }
            set { totalObeOcene = value; }
        }

        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private DrugaOcena ocena2;
        public virtual DrugaOcena Ocena2
        {
            get { return ocena2; }
            set { ocena2 = value; }
        }

        public Ocena()
        { 
        
        }

        public override void izracunajOcenu(int brojDecimalaE, int brojDecimalaBon, int brojDecimalaPen,
            int brojDecimalaTotal, bool odbaciMinMaxEOcenu)
        {
            base.izracunajOcenu(brojDecimalaE, brojDecimalaBon, brojDecimalaPen, brojDecimalaTotal, odbaciMinMaxEOcenu);
            if (Ocena2 != null)
            {
                Ocena2.izracunajOcenu(brojDecimalaE, brojDecimalaBon, brojDecimalaPen, brojDecimalaTotal, odbaciMinMaxEOcenu);
                TotalObeOcene = getTotalObeOcene(brojDecimalaTotal);
            }
        }

        public virtual float getTotalObeOcene(int brojDecimalaTotal)
        {
            if (Ocena2 != null)
                return (float)RounderToZero.round(((decimal)Total + (decimal)Ocena2.Total) / 2,
                                   brojDecimalaTotal);
            else
                return Total.Value;
        }

        public override void validate(Notification notification)
        {
            base.validate(notification);
            if (Ocena2 != null)
            {
                Ocena2.validate(notification);
                if (TotalObeOcene == null)
                {
                    notification.RegisterMessage(
                        "TotalObeOcene", "Konacna ocena je obavezna.");
                }
                else if (TotalObeOcene.Value < 0)
                {
                    notification.RegisterMessage(
                        "TotalObeOcene", "Konacna ocena ne sme da bude negativna.");
                }
            }
        }

        public override void validateZaIzracunavanje(Notification notification)
        {
            base.validateZaIzracunavanje(notification);
            if (Ocena2 != null)
                Ocena2.validateZaIzracunavanje(notification);
        }

        public virtual string PrezimeIme
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.PrezimeIme;
                else
                    return String.Empty;
            }
        }

        public virtual string KlubDrzava
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.KlubDrzava;
                else
                    return String.Empty;
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is Ocena)) return false;

            Ocena that = (Ocena)other;
            bool result = this.Gimnasticar.Equals(that.Gimnasticar)
                && this.Sprava == that.Sprava
                && this.DeoTakmicenjaKod == that.DeoTakmicenjaKod;
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnasticar.GetHashCode();
                result = 29 * result + Sprava.GetHashCode();
                result = 29 * result + DeoTakmicenjaKod.GetHashCode();
                return result;
            }
        }

        // Svojstva za prikaz ocena iz preskoka gde postoje dva preskoka. Koristi se u OceneForm
        public virtual string D_2
        {
            get
            {
                string result = String.Empty;
                if (D != null)
                    result = D.Value.ToString("F" + Opcije.Instance.BrojDecimalaD);
                if (Ocena2 != null && Ocena2.D != null)
                    result += Environment.NewLine + Ocena2.D.Value.ToString("F" + Opcije.Instance.BrojDecimalaD);
                return result;
            }
        }

        public virtual string E1_2
        {
            get
            {
                string result = String.Empty;
                if (E1 != null)
                    result = E1.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E1 != null)
                    result += Environment.NewLine + Ocena2.E1.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E2_2
        {
            get
            {
                string result = String.Empty;
                if (E2 != null)
                    result = E2.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E2 != null)
                    result += Environment.NewLine + Ocena2.E2.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E3_2
        {
            get
            {
                string result = String.Empty;
                if (E3 != null)
                    result = E3.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E3 != null)
                    result += Environment.NewLine + Ocena2.E3.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E4_2
        {
            get
            {
                string result = String.Empty;
                if (E4 != null)
                    result = E4.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E4 != null)
                    result += Environment.NewLine + Ocena2.E4.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E5_2
        {
            get
            {
                string result = String.Empty;
                if (E5 != null)
                    result = E5.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E5 != null)
                    result += Environment.NewLine + Ocena2.E5.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E6_2
        {
            get
            {
                string result = String.Empty;
                if (E6 != null)
                    result = E6.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                if (Ocena2 != null && Ocena2.E6 != null)
                    result += Environment.NewLine + Ocena2.E6.Value.ToString("F" + Opcije.Instance.BrojDecimalaE1);
                return result;
            }
        }

        public virtual string E_2
        {
            get
            {
                string result = String.Empty;
                if (E != null)
                    result = E.Value.ToString("F" + Opcije.Instance.BrojDecimalaE);
                if (Ocena2 != null && Ocena2.E != null)
                    result += Environment.NewLine + Ocena2.E.Value.ToString("F" + Opcije.Instance.BrojDecimalaE);
                return result;
            }
        }

        public virtual string Bonus_2
        {
            get
            {
                string result = String.Empty;
                if (Bonus != null)
                    result = Bonus.Value.ToString("F" + Opcije.Instance.BrojDecimalaBon);
                if (Ocena2 != null)
                {
                    if (Ocena2.Bonus != null)
                        result += Environment.NewLine + Ocena2.Bonus.Value.ToString("F" + Opcije.Instance.BrojDecimalaBon);
                    else
                        // da bi bonus za prvu ocenu bio u istoj liniji sa ostalim
                        // podacima za prvu ocenu (inace bi ga prikazao po sredini)
                        result += Environment.NewLine;
                }
                return result;
            }
        }

        public virtual string Penalty_2
        {
            get
            {
                string result = String.Empty;
                if (Penalty != null)
                    result = Penalty.Value.ToString("F" + Opcije.Instance.BrojDecimalaPen);
                if (Ocena2 != null)
                {
                    if (Ocena2.Penalty != null)
                        result += Environment.NewLine + Ocena2.Penalty.Value.ToString("F" + Opcije.Instance.BrojDecimalaPen);
                    else
                        // da bi penalty za prvu ocenu bio u istoj liniji sa ostalim
                        // podacima za prvu ocenu (inace bi ga prikazao po sredini)
                        result += Environment.NewLine;
                }
                return result;
            }
        }

        public virtual string Total_2
        {
            get
            {
                string result = String.Empty;
                if (Total != null)
                    result = Total.Value.ToString("F" + Opcije.Instance.BrojDecimalaTotal);
                if (Ocena2 != null && Ocena2.Total != null)
                    result += Environment.NewLine + Ocena2.Total.Value.ToString("F" + Opcije.Instance.BrojDecimalaTotal);
                return result;
            }
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Sprava.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());
            strBuilder.AppendLine(TotalObeOcene != null ? TotalObeOcene.Value.ToString() : NULL);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);

            if (Ocena2 == null)
                strBuilder.AppendLine(NULL);
            else
                Ocena2.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            string line = reader.ReadLine();
            TotalObeOcene = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            DrugaOcena o = null;
            if (line != NULL)
            {
                o = new DrugaOcena();
                o.loadFromDump(reader);                
            }
            Ocena2 = o;
        }
    }
}
