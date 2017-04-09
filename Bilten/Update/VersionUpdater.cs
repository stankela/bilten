using Bilten;
using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;

public class VersionUpdater
{
    public void updateVersion3()
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
    
    public void updateVersion7()
    {
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);

                GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                GimnasticarUcesnik g = gimUcesnikDAO.FindById(14422);
                OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                IList<Ocena> ocene = ocenaDAO.FindOceneForGimnasticar(g,
                    DeoTakmicenjaKod.Takmicenje1);

                ocenaDAO.Delete(ocene[0]);
                gimUcesnikDAO.Delete(g);
                session.Transaction.Commit();
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

    public class RezultatSpravaFinaleKupaUpdate
    {
        private int rezTakmicenjeId;
        public virtual int RezultatskoTakmicenjeId
        {
            get { return rezTakmicenjeId; }
            set { rezTakmicenjeId = value; }
        }

        private int gimnasticarId;
        public virtual int GimnasticarId
        {
            get { return gimnasticarId; }
            set { gimnasticarId = value; }
        }

        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        private KvalifikacioniStatus kvalStatus;
        public virtual KvalifikacioniStatus KvalStatus
        {
            get { return kvalStatus; }
            set { kvalStatus = value; }
        }

    }
    
    public void updateVersion13()
    {
        // TODO: drop table rezultati_sprava_finale_kupa_update


        PoredakSpravaFinaleKupa p = null;

        string findByRezTakSQL =
           "SELECT * FROM rezultati_sprava_finale_kupa_update " +
           "WHERE rez_takmicenje_id = @rez_takmicenje_id";

        SqlCeCommand cmd = new SqlCeCommand(findByRezTakSQL);
        //cmd.Parameters.Add("@rez_takmicenje_id", SqlDbType.Int).Value = rezTak.Id;
        SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

        List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate = new List<RezultatSpravaFinaleKupaUpdate>();
        while (rdr.Read())
        {
            RezultatSpravaFinaleKupaUpdate rezultat = new RezultatSpravaFinaleKupaUpdate();
            rezultat.RezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
            rezultat.GimnasticarId = (int)rdr["gimnasticar_id"];
            rezultat.Sprava = (Sprava)(byte)rdr["sprava"];
            rezultat.KvalStatus = (KvalifikacioniStatus)(byte)rdr["kval_status"];
            rezultatiUpdate.Add(rezultat);
        }

        rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader

   
        foreach (RezultatSpravaFinaleKupaUpdate rezultatUpdate in rezultatiUpdate)
        {
            if (rezultatUpdate.Sprava != p.Sprava)
                continue;
            foreach (RezultatSpravaFinaleKupa rezultat in p.Rezultati)
            {
                if (rezultatUpdate.GimnasticarId == rezultat.Gimnasticar.Id)
                {
                    rezultat.KvalStatus = rezultatUpdate.KvalStatus;
                    break;
                }
            }
        }
    }
}
