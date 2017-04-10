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

    class RezultatSpravaFinaleKupaUpdate
    {
        public int GimnasticarId;
        public Sprava Sprava;
        public KvalifikacioniStatus KvalStatus;
    }

    public void updateVersion13()
    {
        SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM rezultati_sprava_finale_kupa_update");
        SqlCeDataReader rdr = SqlCeUtilities.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);

        IDictionary<int, List<RezultatSpravaFinaleKupaUpdate>> rezultatiUpdate
            = new Dictionary<int, List<RezultatSpravaFinaleKupaUpdate>>();
        while (rdr.Read())
        {
            RezultatSpravaFinaleKupaUpdate rezultat = new RezultatSpravaFinaleKupaUpdate();
            rezultat.GimnasticarId = (int)rdr["gimnasticar_id"];
            rezultat.Sprava = (Sprava)(byte)rdr["sprava"];
            rezultat.KvalStatus = (KvalifikacioniStatus)(byte)rdr["kval_status"];
            
            int rezultatskoTakmicenjeId = (int)rdr["rez_takmicenje_id"];
            if (rezultatiUpdate.ContainsKey(rezultatskoTakmicenjeId))
                rezultatiUpdate[rezultatskoTakmicenjeId].Add(rezultat);
            else
            {
                List<RezultatSpravaFinaleKupaUpdate> rezultati = new List<RezultatSpravaFinaleKupaUpdate>();
                rezultati.Add(rezultat);
                rezultatiUpdate.Add(rezultatskoTakmicenjeId, rezultati);
            }
        }
        rdr.Close(); // obavezno, da bi se zatvorila konekcija otvorena u executeReader
               
        ISession session = null;
        try
        {
            using (session = NHibernateHelper.Instance.OpenSession())
            using (session.BeginTransaction())
            {
                CurrentSessionContext.Bind(session);

                ISet<string> rezTakmicenjaSet = new HashSet<string>();

                IList<Takmicenje> takmicenja = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindViseKola();

                foreach (Takmicenje takmicenje in takmicenja)
                {
                    List<Takmicenje> prethodnaKola = new List<Takmicenje>();
                    prethodnaKola.Add(takmicenje.PrvoKolo);
                    prethodnaKola.Add(takmicenje.DrugoKolo);
                    if (takmicenje.TreceKolo != null)
                        prethodnaKola.Add(takmicenje.TreceKolo);
                    if (takmicenje.CetvrtoKolo != null)
                        prethodnaKola.Add(takmicenje.CetvrtoKolo);

                    RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    IList<RezultatskoTakmicenje> svaRezTakmicenja = rezultatskoTakmicenjeDAO.FindByTakmicenje(takmicenje.Id);
                    
                    List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<IList<RezultatskoTakmicenje>>();
                    foreach (Takmicenje prethKolo in prethodnaKola)
                    {
                        rezTakmicenjaPrethodnaKola.Add(
                            rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Gimnasticari(prethKolo.Id));
                    }

                    IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
                    IList<RezultatskoTakmicenje> preskociRezTakmicenja = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.TakmicenjeDescription.RedBroj != 0)
                            continue;

                        // preskoci rez. takmicenja gde ne postoji poklapanje u svim prethodnim kolima
                        bool preskoci = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija) == null
                            || Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija) == null
                            || (takmicenje.TreceKolo != null && Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija) == null)
                            || (takmicenje.CetvrtoKolo != null && Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija) == null);

                        if (!preskoci || takmicenje.Id == 226)
                            rezTakmicenja.Add(rt);
                        else
                            preskociRezTakmicenja.Add(rt);
                    }

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        PoredakUkupno poredak1 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija).Takmicenje1.PoredakUkupno;
                        PoredakUkupno poredak2;
                        if (rt.Id != 654)
                            poredak2 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija).Takmicenje1.PoredakUkupno;
                        else
                            poredak2 = new PoredakUkupno();

                        PoredakUkupno poredak3 = null;
                        PoredakUkupno poredak4 = null;
                        if (takmicenje.TreceKolo != null)
                            poredak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija).Takmicenje1.PoredakUkupno;
                        if (takmicenje.CetvrtoKolo != null)
                            poredak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija).Takmicenje1.PoredakUkupno;

                        if (takmicenje.FinaleKupa)
                        {
                            if (rt.Takmicenje1.PoredakUkupnoFinaleKupa == null)
                                rt.Takmicenje1.PoredakUkupnoFinaleKupa = new PoredakUkupnoFinaleKupa();
                            rt.Takmicenje1.PoredakUkupnoFinaleKupa.create(rt, poredak1, poredak2);
                        }
                        else if (takmicenje.ZbirViseKola)
                        {
                            if (rt.Takmicenje1.PoredakUkupnoZbirViseKola == null)
                                rt.Takmicenje1.PoredakUkupnoZbirViseKola = new PoredakUkupnoZbirViseKola();
                            rt.Takmicenje1.PoredakUkupnoZbirViseKola.create(rt, poredak1, poredak2, poredak3, poredak4);
                        }
                    }

                    if (takmicenje.FinaleKupa)
                    {
                        foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                        {
                            RezultatskoTakmicenje rezTak1 =
                                Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija);
                            RezultatskoTakmicenje rezTak2 =
                                Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija);

                            rt.Takmicenje1.initPoredakSpravaFinaleKupa(takmicenje.Gimnastika);

                            foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                            {
                                if (s != Sprava.Preskok)
                                {
                                    PoredakSprava poredakSprava1 = rezTak1.Takmicenje1.getPoredakSprava(s);
                                    PoredakSprava poredakSprava2;
                                    if (rt.Id != 654)
                                        poredakSprava2 = rezTak2.Takmicenje1.getPoredakSprava(s);
                                    else
                                        poredakSprava2 = new PoredakSprava();
                                    rt.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rt, poredakSprava1, poredakSprava2);
                                }
                                else
                                {
                                    PoredakPreskok poredakPreskok1 = rezTak1.Takmicenje1.PoredakPreskok;
                                    bool obaPreskoka1 = rezTak1.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;

                                    PoredakPreskok poredakPreskok2;
                                    bool obaPreskoka2;

                                    if (rt.Id != 654)
                                    {
                                        poredakPreskok2 = rezTak2.Takmicenje1.PoredakPreskok;
                                        obaPreskoka2 = rezTak2.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                                    }
                                    else
                                    {
                                        poredakPreskok2 = new PoredakPreskok();
                                        obaPreskoka2 = false;
                                    }
                                    rt.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rt,
                                        poredakPreskok1, poredakPreskok2, obaPreskoka1, obaPreskoka2);
                                }
                            }

                            // update kval status
                            if (rezultatiUpdate.ContainsKey(rt.Id))
                            {
                                rezTakmicenjaSet.Add(rt.ToString());
                                foreach (RezultatSpravaFinaleKupaUpdate rezultatUpdate in rezultatiUpdate[rt.Id])
                                {
                                    foreach (RezultatSpravaFinaleKupa rezultat in
                                        rt.Takmicenje1.getPoredakSpravaFinaleKupa(rezultatUpdate.Sprava).Rezultati)
                                    {
                                        if (rezultat.Gimnasticar.Id == rezultatUpdate.GimnasticarId)
                                            rezultat.KvalStatus = rezultatUpdate.KvalStatus;
                                    }
                                }
                            }
                        }
                    }

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        PoredakEkipno poredak1
                            = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], 0, rt.Kategorija).Takmicenje1.PoredakEkipno;
                        PoredakEkipno poredak2;
                        if (rt.Id != 654)
                            poredak2 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], 0, rt.Kategorija).Takmicenje1.PoredakEkipno;
                        else
                            poredak2 = new PoredakEkipno();
                        
                        PoredakEkipno poredak3 = null;
                        PoredakEkipno poredak4 = null;
                        if (takmicenje.TreceKolo != null)
                            poredak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], 0, rt.Kategorija).Takmicenje1.PoredakEkipno;
                        if (takmicenje.CetvrtoKolo != null)
                            poredak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], 0, rt.Kategorija).Takmicenje1.PoredakEkipno;

                        if (takmicenje.FinaleKupa)
                        {
                            if (rt.Takmicenje1.PoredakEkipnoFinaleKupa == null)
                                rt.Takmicenje1.PoredakEkipnoFinaleKupa = new PoredakEkipnoFinaleKupa();
                            rt.Takmicenje1.PoredakEkipnoFinaleKupa.create(rt, poredak1, poredak2);
                        }
                        else if (takmicenje.ZbirViseKola)
                        {
                            if (rt.Takmicenje1.PoredakEkipnoZbirViseKola == null)
                                rt.Takmicenje1.PoredakEkipnoZbirViseKola = new PoredakEkipnoZbirViseKola();
                            rt.Takmicenje1.PoredakEkipnoZbirViseKola.create(rt, poredak1, poredak2, poredak3, poredak4);
                        }
                    }

                    foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                        foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethKolo)
                            rezultatskoTakmicenjeDAO.Evict(rt);

                    Takmicenje1DAO takmicenje1DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO();
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                        takmicenje1DAO.Update(rt.Takmicenje1);

                    foreach (RezultatskoTakmicenje rt in preskociRezTakmicenja)
                    {
                        // brisi rezultatska takmicenja i ekipe
                        EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
                        foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                            ekipaDAO.Delete(e);
                        rezultatskoTakmicenjeDAO.Delete(rt);
                    }
                }

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
}
