﻿using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Misc
{
    public class TakmicenjeDump
    {
        private int takmicenjeId;

        private string takmicenjeDump;
        private string kluboviDump;
        private string drzaveDump;
        private string gimnasticariDump;
        private string rezTakmicenjaDump;

        private StringBuilder strBuilder;

        public TakmicenjeDump()
        {
            takmicenjeId = -1;
            strBuilder = new StringBuilder();
        }

        public void dumpTakmicenje(int takmicenjeId)
        {
            this.takmicenjeId = takmicenjeId;

            dumpTakmicenje();
            dumpKlubovi();
            dumpDrzave();
            dumpGimnasticari();
            dumpRezTakmicenja();
        }

        private void dumpTakmicenje()
        {
            strBuilder.Clear();
            Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            takmicenje.dump(strBuilder);
            takmicenjeDump = strBuilder.ToString();
        }

        private void dumpKlubovi()
        {
            strBuilder.Clear();
            IList<KlubUcesnik> klubovi = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId);
            foreach (KlubUcesnik k in klubovi)
                k.dump(strBuilder);
            kluboviDump = strBuilder.ToString();
        }

        private void dumpDrzave()
        {
            strBuilder.Clear();
            IList<DrzavaUcesnik> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            foreach (DrzavaUcesnik d in drzave)
                d.dump(strBuilder);
            drzaveDump = strBuilder.ToString();
        }

        private void dumpGimnasticari()
        {
            strBuilder.Clear();
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenjeId);
            foreach (GimnasticarUcesnik g in gimnasticari)
                g.dump(strBuilder);
            gimnasticariDump = strBuilder.ToString();
        }

        private void dumpRezTakmicenja()
        {
            strBuilder.Clear();
            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenje(takmicenjeId);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rt.dump(strBuilder);
            rezTakmicenjaDump = strBuilder.ToString();
        }

        public string getTakmicenjeDump()
        {
            return takmicenjeId != -1 ? takmicenjeDump : String.Empty;
        }
    }
}