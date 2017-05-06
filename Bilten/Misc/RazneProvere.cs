using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Services;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten
{
    public class RazneProvere
    {
        private IList<int> getTakmicenjaId()
        {
            IList<int> result = new List<int>();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    foreach (Takmicenje t in DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindAll())
                        result.Add(t.Id);
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
            return result;
        }

        // Proveri da li za svako finale kupa nijedno od prva dva kola nema odvojeno takmicenje 3.
        public void proveriPrvaDvaKola()
        {
            IList<int> takmicenjaId = getTakmicenjaId();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    for (int i = 0; i < takmicenjaId.Count; ++i)
                    {
                        Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjaId[i]);
                        if (!t.FinaleKupa)
                            continue;

                        string takmicenjeHeader = i.ToString() + ". " + t.ToString();
                        if (t.FinaleKupa)
                            takmicenjeHeader += " - FINALE KUPA";
                        takmicenjeHeader += " (" + t.Id + ")";

                        RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                        foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByTakmicenje(t.PrvoKolo.Id))
                        {
                            if (rt.Propozicije.odvojenoTak3() && t.PrvoKolo.ZavrsenoTak1)
                                MessageBox.Show("Postoji odvojeno takmicenje 3 u prvom kolu\n\n" + takmicenjeHeader);
                        }
                        foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByTakmicenje(t.DrugoKolo.Id))
                        {
                            if (rt.Propozicije.odvojenoTak3() && t.DrugoKolo.ZavrsenoTak1)
                                MessageBox.Show("Postoji odvojeno takmicenje 3 u drugom kolu\n\n" + takmicenjeHeader);
                        }
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

        // Proveri da li za svaki rezultat postoji i ocena.
        public void proveriRezultateIOcene()
        {
            // TODO4
        }

        // Proveri da li uvek vazi da je RezultatskoTakmicenje.Takmicenje2 == null kada je odvojenoTak2() == false.
        // Isto i za takmicenja 3 i 4.
        public void proveriTakmicenja234()
        {
            // TODO4
        }

        // Proveri za sva finala kupa i zbir vise kola, da li postoje gimnasticari koji su nastupali u razlicitim
        // kategorijama u prethodnim kolima.
        public void proveriViseKola()
        {
            IList<int> takmicenjaId = getTakmicenjaId();
            string takmicenjeHeader = String.Empty;
            for (int j = 0; j < takmicenjaId.Count; ++j)
            {
                ISession session = null;
                try
                {
                    using (session = NHibernateHelper.Instance.OpenSession())
                    using (session.BeginTransaction())
                    {
                        CurrentSessionContext.Bind(session);
                        TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                        Takmicenje t = takmicenjeDAO.FindById(takmicenjaId[j]);
                        if (!t.FinaleKupa && !t.ZbirViseKola)
                            continue;

                        takmicenjeHeader = j.ToString() + ". " + t.ToString();
                        if (t.FinaleKupa)
                            takmicenjeHeader += " - FINALE KUPA";
                        else
                            takmicenjeHeader += " - ZBIR VISE KOLA";
                        takmicenjeHeader += " (" + t.Id + ")";

                        List<Takmicenje> prethodnaKola = new List<Takmicenje>();
                        prethodnaKola.Add(t.PrvoKolo);
                        prethodnaKola.Add(t.DrugoKolo);
                        if (t.TreceKolo != null)
                            prethodnaKola.Add(t.TreceKolo);
                        if (t.CetvrtoKolo != null)
                            prethodnaKola.Add(t.CetvrtoKolo);

                        RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

                        List<IList<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<IList<RezultatskoTakmicenje>>();
                        foreach (Takmicenje prethKolo in prethodnaKola)
                            rezTakmicenjaPrethodnaKola.Add(rezTakDAO.FindByTakmicenje(prethKolo.Id));

                        IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(t.Id);

                        // Za svakog gimnasticara, zapamti u kojim kategorijama je ucestvovao u prethodnim kolima
                        IDictionary<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>> mapaUcestvovanja
                            = new Dictionary<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>>();

                        foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                        {
                            for (int i = 0; i < rezTakmicenjaPrethodnaKola.Count; ++i)
                            {
                                IList<RezultatskoTakmicenje> rezTakmicenjaPrethKolo = rezTakmicenjaPrethodnaKola[i];
                                RezultatskoTakmicenje rtFrom = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethKolo, 0, rt.Kategorija);
                                if (rtFrom == null)
                                {
                                    // Ovo se pojavljuje kod takmicenja ciji je id 226.
                                    continue;
                                }

                                Pair<int, TakmicarskaKategorija> koloKatPair = new Pair<int, TakmicarskaKategorija>(i, rt.Kategorija);

                                foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                                {
                                    if (!mapaUcestvovanja.ContainsKey(g))
                                    {
                                        IList<Pair<int, TakmicarskaKategorija>> pairList = new List<Pair<int, TakmicarskaKategorija>>();
                                        pairList.Add(koloKatPair);
                                        mapaUcestvovanja.Add(g, pairList);
                                    }
                                    else
                                        mapaUcestvovanja[g].Add(koloKatPair);
                                }
                            }
                        }

                        foreach (KeyValuePair<GimnasticarUcesnik, IList<Pair<int, TakmicarskaKategorija>>> entry in mapaUcestvovanja)
                        {
                            GimnasticarUcesnik g = entry.Key;
                            TakmicarskaKategorija prevKat = null;
                            bool ok = true;
                            foreach (Pair<int, TakmicarskaKategorija> koloKatPair in entry.Value)
                            {
                                TakmicarskaKategorija kat = koloKatPair.Second;
                                if (prevKat == null)
                                    prevKat = kat;
                                else if (!kat.Equals(prevKat))
                                    ok = false;
                            }
                            if (!ok)
                                MessageBox.Show(takmicenjeHeader + "\n\n" + g.ImeSrednjeImePrezimeDatumRodjenja);
                        }
                    }
                }
                catch (Exception)
                {
                    if (session != null && session.Transaction != null && session.Transaction.IsActive)
                        session.Transaction.Rollback();
                    MessageBox.Show(takmicenjeHeader);
                }
                finally
                {
                    CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                }
            }
        }

        public void takmicenjaBezOcene()
        {
            StreamWriter logStreamWriter = File.CreateText("takmicenja_bez_ocene.txt");
            IList<int> takmicenjaId = getTakmicenjaId();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    for (int i = 0; i < takmicenjaId.Count; ++i)
                    {
                        Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjaId[i]);

                        string takmicenjeHeader = i.ToString() + ". " + t.ToString();
                        if (t.FinaleKupa)
                            takmicenjeHeader += " - FINALE KUPA";
                        else if (t.ZbirViseKola)
                            takmicenjeHeader += " - ZBIR VISE KOLA";
                        takmicenjeHeader += " (" + t.Id + ")";

                        if (!DAOFactoryFactory.DAOFactory.GetOcenaDAO().existsOcene(t.Id))
                        {
                            if (t.ZbirViseKola)
                                continue;
                            if (t.FinaleKupa)
                            {
                                bool odvojeno = false;
                                IList<RezultatskoTakmicenje> rezTakmicenja
                                    = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().FindByTakmicenje(t.Id);
                                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                                {
                                    if (rt.odvojenoTak3())
                                    {
                                        odvojeno = true;
                                        break;
                                    }
                                }
                                if (!odvojeno)
                                    continue;
                            }
                            logStreamWriter.WriteLine(takmicenjeHeader);
                        }
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
                logStreamWriter.Close();
            }
        }
    }
}
