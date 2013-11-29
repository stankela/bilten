using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class NastupNaSpravi : DomainObject
    {
        private bool nastupaDvaPuta;
        public virtual bool NastupaDvaPuta
        {
            get { return nastupaDvaPuta; }
            set { nastupaDvaPuta = value; }
        }

        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private int ekipa;
        public virtual int Ekipa
        {
            get { return ekipa; }
            set { ekipa = value; }
        }

        private string kategorija = String.Empty;

        public NastupNaSpravi()
        { 
        
        }

        public NastupNaSpravi(bool nastupaDvaPuta, GimnasticarUcesnik gimnasticar, int ekipa)
        {
            this.nastupaDvaPuta = nastupaDvaPuta;
            this.gimnasticar = gimnasticar;
            this.ekipa = ekipa;

            // Svojstvo Kategorija kesiram zato sto sam imao problema na start listama gde nije mogao da ga ocita iz
            // gimnasticara.
            if (gimnasticar != null)
            {
                try
                {
                    kategorija = gimnasticar.TakmicarskaKategorija.ToString();
                }
                catch (Exception)
                {
                    kategorija = String.Empty;
                }
            }
        }

        public virtual string TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj.ToString();
                else
                    return String.Empty;
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

        public virtual string Kategorija
        {
            get
            {
                if (kategorija != String.Empty)
                    return kategorija;

                if (Gimnasticar != null)
                {
                    try
                    {
                        kategorija = Gimnasticar.TakmicarskaKategorija.ToString();
                    }
                    catch (Exception)
                    {
                        kategorija = String.Empty;
                    }
                    return kategorija;
                }
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

    }
}
