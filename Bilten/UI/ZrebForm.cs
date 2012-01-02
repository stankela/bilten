using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class ZrebForm : Form
    {
        private Takmicenje takmicenje;
        private IDataContext dataContext;

        public ZrebForm(int takmicenjeId)
        {
            InitializeComponent();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = loadTakmicenje(takmicenjeId);

                initUI();

                //dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Id", CriteriaOperator.Equal, takmicenjeId));
            IList<Takmicenje> result = dataContext.GetByCriteria<Takmicenje>(q);
            return result[0];
        }

        private void initUI()
        {
            textBox1.Text = takmicenje.ZrebZaFinalePoSpravama;
        }

        // TODO: Dodaj provere i validaciju.

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje.ZrebZaFinalePoSpravama = textBox1.Text.Trim();
                dataContext.Save(takmicenje);
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private void ZrebForm_Shown(object sender, EventArgs e)
        {
            label1.Focus();
        }

    }
}
