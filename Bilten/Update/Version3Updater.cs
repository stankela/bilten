using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.UI;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.IO;

public class Version3Updater
{
    public void update()
    {
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);
                IList<Takmicenje> takmicenja = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindAllOdvojenoFinale();

                int brojRezultata = 0;
                int brojUcesnika = 0;

                foreach (Takmicenje t in takmicenja)
                {
                    if (!t.ZavrsenoTak1)
                        throw new Exception("GRESKA");
                    IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenjeFetch_Tak1_PoredakUkupno_KlubDrzava(t.Id);
                    foreach (RezultatskoTakmicenje tak in rezTakmicenja)
                    {
                        NHibernateUtil.Initialize(tak.Takmicenje1.PoredakUkupno.Rezultati);

                        // potrebno u Poredak.create
                        NHibernateUtil.Initialize(tak.Propozicije);
                    }
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.Propozicije.PostojiTak2 && !rt.Propozicije.OdvojenoTak2)
                        {
                            /*brojUcesnika += rt.Takmicenje2.Ucesnici.Count;
                            rt.Takmicenje2.clearUcesnici();

                            brojRezultata += rt.Takmicenje2.Poredak.Rezultati.Count;
                            rt.Takmicenje2.Poredak.Rezultati.Clear();

                            DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rt.Takmicenje2);*/
                        }
                        if (rt.Propozicije.PostojiTak3 && !rt.Propozicije.OdvojenoTak3)
                        {
                            brojUcesnika += rt.Takmicenje3.Ucesnici.Count;
                            rt.Takmicenje3.clearUcesnici();

                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                            {
                                brojRezultata += p.Rezultati.Count;
                                p.Rezultati.Clear();
                            }
                            brojRezultata += rt.Takmicenje3.PoredakPreskok.Rezultati.Count;
                            rt.Takmicenje3.PoredakPreskok.Rezultati.Clear();

                            //MessageDialogs.showMessage(rt.Naziv + "   " + t.Datum + "\n\n" + brojUcesnika.ToString()
                              //  + "\n\n" + brojRezultata.ToString(), "Bilten");

                            DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rt.Takmicenje3);
                        }

                    }

                }

                MessageDialogs.showMessage("broj Ucesnika: " + brojUcesnika + "\n\n" + "broj Rezultata: " +
                    brojRezultata, "Bilten");

                if (takmicenja.Count > 0)
                {
                    session.Transaction.Commit();
                }
            
                //string databaseFile = "BiltenPodaci.sdf";
                //SqlCeUtilities.ExecuteScript(databaseFile, "", Path.GetFullPath(@"DatabaseUpdate_version3.sql"));
            }
        }
        catch (Exception ex)
        {
            if (session != null && session.Transaction != null && session.Transaction.IsActive)
                session.Transaction.Rollback();
            throw new InfrastructureException(ex.Message, ex);
        }
        finally
        {
            CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
        }
    }
}