using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatPreskokFinaleKupa : RezultatSpravaFinaleKupa
    {
        private PoredakPreskokFinaleKupa poredak;

        private Nullable<float> d_2_PrvoKolo;
        public virtual Nullable<float> D_2_PrvoKolo
        {
            get { return d_2_PrvoKolo; }
            set { d_2_PrvoKolo = value; }
        }

        private Nullable<float> e_2_PrvoKolo;
        public virtual Nullable<float> E_2_PrvoKolo
        {
            get { return e_2_PrvoKolo; }
            set { e_2_PrvoKolo = value; }
        }

        private Nullable<float> pen_2_PrvoKolo;
        public virtual Nullable<float> Pen_2_PrvoKolo
        {
            get { return pen_2_PrvoKolo; }
            set { pen_2_PrvoKolo = value; }
        }

        private Nullable<float> bonus_2_PrvoKolo;
        public virtual Nullable<float> Bonus_2_PrvoKolo
        {
            get { return bonus_2_PrvoKolo; }
            set { bonus_2_PrvoKolo = value; }
        }

        private Nullable<float> _total_2_PrvoKolo;
        public virtual Nullable<float> Total_2_PrvoKolo
        {
            get { return _total_2_PrvoKolo; }
            set { _total_2_PrvoKolo = value; }
        }

        private Nullable<float> totalObeOcene_PrvoKolo;
        public virtual Nullable<float> TotalObeOcene_PrvoKolo
        {
            get { return totalObeOcene_PrvoKolo; }
            protected set { totalObeOcene_PrvoKolo = value; }
        }

        private Nullable<float> d_2_DrugoKolo;
        public virtual Nullable<float> D_2_DrugoKolo
        {
            get { return d_2_DrugoKolo; }
            set { d_2_DrugoKolo = value; }
        }

        private Nullable<float> e_2_DrugoKolo;
        public virtual Nullable<float> E_2_DrugoKolo
        {
            get { return e_2_DrugoKolo; }
            set { e_2_DrugoKolo = value; }
        }

        private Nullable<float> pen_2_DrugoKolo;
        public virtual Nullable<float> Pen_2_DrugoKolo
        {
            get { return pen_2_DrugoKolo; }
            set { pen_2_DrugoKolo = value; }
        }

        private Nullable<float> bonus_2_DrugoKolo;
        public virtual Nullable<float> Bonus_2_DrugoKolo
        {
            get { return bonus_2_DrugoKolo; }
            set { bonus_2_DrugoKolo = value; }
        }

        private Nullable<float> _total_2_DrugoKolo;
        public virtual Nullable<float> Total_2_DrugoKolo
        {
            get { return _total_2_DrugoKolo; }
            set { _total_2_DrugoKolo = value; }
        }

        private Nullable<float> totalObeOcene_DrugoKolo;
        public virtual Nullable<float> TotalObeOcene_DrugoKolo
        {
            get { return totalObeOcene_DrugoKolo; }
            protected set { totalObeOcene_DrugoKolo = value; }
        }

        public RezultatPreskokFinaleKupa()
        {

        }

        public RezultatPreskokFinaleKupa(PoredakPreskokFinaleKupa p)
        {
            poredak = p;
        }

        public virtual void initPrvoKolo(RezultatPreskok r)
        {
            base.initPrvoKolo(r);
            D_2_PrvoKolo = r.D_2;
            E_2_PrvoKolo = r.E_2;
            Bonus_2_PrvoKolo = r.Bonus_2;
            Pen_2_PrvoKolo = r.Penalty_2;
            Total_2_PrvoKolo = r.Total_2;

            TotalObeOcene_PrvoKolo = r.TotalObeOcene;
        }

        public virtual void initDrugoKolo(RezultatPreskok r)
        {
            base.initDrugoKolo(r);
            D_2_DrugoKolo = r.D_2;
            E_2_DrugoKolo = r.E_2;
            Bonus_2_DrugoKolo = r.Bonus_2;
            Pen_2_DrugoKolo = r.Penalty_2;
            Total_2_DrugoKolo = r.Total_2;

            TotalObeOcene_DrugoKolo = r.TotalObeOcene;
        }

        public override Nullable<float> getTotalPrvoKolo()
        {
            return poredak.KoristiTotalObeOcenePrvoKolo ? this.TotalObeOcene_PrvoKolo : this.TotalPrvoKolo;
        }

        public override Nullable<float> getTotalDrugoKolo()
        {
            return poredak.KoristiTotalObeOceneDrugoKolo ? this.TotalObeOcene_DrugoKolo : this.TotalDrugoKolo;
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(D_2_PrvoKolo != null ? D_2_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_2_PrvoKolo != null ? E_2_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Bonus_2_PrvoKolo != null ? Bonus_2_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Pen_2_PrvoKolo != null ? Pen_2_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Total_2_PrvoKolo != null ? Total_2_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalObeOcene_PrvoKolo != null ? TotalObeOcene_PrvoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(D_2_DrugoKolo != null ? D_2_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(E_2_DrugoKolo != null ? E_2_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Bonus_2_DrugoKolo != null ? Bonus_2_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Pen_2_DrugoKolo != null ? Pen_2_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(Total_2_DrugoKolo != null ? Total_2_DrugoKolo.Value.ToString() : NULL);
            strBuilder.AppendLine(TotalObeOcene_DrugoKolo != null ? TotalObeOcene_DrugoKolo.Value.ToString() : NULL);
        }

        public override void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader, map);

            string line = reader.ReadLine();
            D_2_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_2_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Bonus_2_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Pen_2_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Total_2_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalObeOcene_PrvoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            D_2_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E_2_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Bonus_2_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Pen_2_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Total_2_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            TotalObeOcene_DrugoKolo = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
