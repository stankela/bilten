using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class VrhovniSudijaForm : Form
    {
        private int takmicenjeId;
        private List<SudijaUcesnik> sudije;
        private readonly string PRAZNO = "<<Prazno>>";

        private SudijaUcesnik SelectedVrhovniSudija
        {
            get { return cmbVrhovniSudija.SelectedItem as SudijaUcesnik; }
            set { cmbVrhovniSudija.SelectedItem = value; }
        }

        public VrhovniSudijaForm(int takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    sudije = new List<SudijaUcesnik>(
                        DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindByTakmicenjeFetchKlubDrzava(takmicenjeId));

                    PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                        TypeDescriptor.GetProperties(typeof(SudijaUcesnik))["Prezime"],
                        TypeDescriptor.GetProperties(typeof(SudijaUcesnik))["Ime"]
                    };
                    ListSortDirection[] sortDir = new ListSortDirection[] {
                        ListSortDirection.Ascending,
                        ListSortDirection.Ascending
                    };
                    sudije.Sort(new SortComparer<SudijaUcesnik>(propDesc, sortDir));

                    SudijaUcesnik emptySudija = new SudijaUcesnik();
                    emptySudija.Prezime = PRAZNO;
                    emptySudija.Ime = "";
                    sudije.Insert(0, emptySudija);

                    cmbVrhovniSudija.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbVrhovniSudija.DataSource = sudije;
                    cmbVrhovniSudija.DisplayMember = "PrezimeIme";
                    cmbVrhovniSudija.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cmbVrhovniSudija.AutoCompleteSource = AutoCompleteSource.ListItems;

                    SelectedVrhovniSudija = takmicenje.VrhovniSudija;
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    takmicenje.VrhovniSudija = SelectedVrhovniSudija;
                    if (takmicenje.VrhovniSudija != null && takmicenje.VrhovniSudija.Prezime == PRAZNO)
                        takmicenje.VrhovniSudija = null;

                    takmicenje.LastModified = DateTime.Now;

                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);
                    session.Transaction.Commit();
                }
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void VrhovniSudijaForm_Shown(object sender, EventArgs e)
        {
            btnCancel.Focus();
        }
    }
}
