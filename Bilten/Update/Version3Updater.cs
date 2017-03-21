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
                            rt.Takmicenje2.clearUcesnici();
                            rt.Takmicenje2.Poredak.Rezultati.Clear();
                            DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rt.Takmicenje2);
                        }
                        if (rt.Propozicije.PostojiTak3 && !rt.Propozicije.OdvojenoTak3)
                        {
                            rt.Takmicenje3.clearUcesnici();
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                            {
                                p.Rezultati.Clear();
                            }
                            rt.Takmicenje3.PoredakPreskok.Rezultati.Clear();
                            DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rt.Takmicenje3);
                        }
                    }
                }

                if (takmicenja.Count > 0)
                {
                    session.Transaction.Commit();
                }
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