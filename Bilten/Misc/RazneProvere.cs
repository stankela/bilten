using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
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
    }
}
