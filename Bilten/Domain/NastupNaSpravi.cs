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

        public NastupNaSpravi()
        { 
        
        }

        public NastupNaSpravi(bool nastupaDvaPuta, GimnasticarUcesnik gimnasticar)
        {
            this.nastupaDvaPuta = nastupaDvaPuta;
            this.gimnasticar = gimnasticar;
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
                if (Gimnasticar != null)
                    return Gimnasticar.TakmicarskaKategorija.ToString();
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
