using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatSpravaFinaleKupa : Rezultat
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private Nullable<float> d_PrvoKolo;
        public virtual Nullable<float> D_PrvoKolo
        {
            get { return d_PrvoKolo; }
            set { d_PrvoKolo = value; }
        }

        private Nullable<float> e_PrvoKolo;
        public virtual Nullable<float> E_PrvoKolo
        {
            get { return e_PrvoKolo; }
            set { e_PrvoKolo = value; }
        }

        private Nullable<float> _totalPrvoKolo;
        public virtual Nullable<float> TotalPrvoKolo
        {
            get { return _totalPrvoKolo; }
            set { _totalPrvoKolo = value; }
        }

        private Nullable<float> d_DrugoKolo;
        public virtual Nullable<float> D_DrugoKolo
        {
            get { return d_DrugoKolo; }
            set { d_DrugoKolo = value; }
        }

        private Nullable<float> e_DrugoKolo;
        public virtual Nullable<float> E_DrugoKolo
        {
            get { return e_DrugoKolo; }
            set { e_DrugoKolo = value; }
        }

        private Nullable<float> _totalDrugoKolo;
        public virtual Nullable<float> TotalDrugoKolo
        {
            get { return _totalDrugoKolo; }
            set { _totalDrugoKolo = value; }
        }

        // moram ovako jer je set accessor za Total protected u klasi Rezultat
        public void setTotal(Nullable<float> value)
        {
            Total = value;
        }

        public virtual Nullable<int> TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj;
                else
                    return null;
            }
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

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(D_PrvoKolo != null ? D_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_PrvoKolo != null ? E_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalPrvoKolo != null ? TotalPrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(D_DrugoKolo != null ? D_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_DrugoKolo != null ? E_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalDrugoKolo != null ? TotalDrugoKolo.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IDictionary<int, GimnasticarUcesnik> gimnasticariMap)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? gimnasticariMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            D_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalPrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            D_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalDrugoKolo = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
