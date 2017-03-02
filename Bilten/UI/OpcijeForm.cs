using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Util;
using Bilten.Dao;
using NHibernate;
using NHibernate.Context;

namespace Bilten.UI
{
    public partial class OpcijeForm : Form
    {
        private Opcije opcije;
        private bool closedByOK;
        private bool closedByCancel;
        private Nullable<int> takmicenjeId;

        public OpcijeForm(Nullable<int> takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            opcije = Opcije.Instance;
            initUI();
            updateUIFromEntity(opcije);
        }

        private void initUI()
        {
            this.Text = "Opcije";

            txtBrojDecD.Text = String.Empty;
            txtBrojDecE1.Text = String.Empty;
            txtBrojDecE.Text = String.Empty;
            txtBrojDecPen.Text = String.Empty;
            txtBrojDecTotal.Text = String.Empty;

        }

        private void updateUIFromEntity(Opcije opcije)
        {
            txtBrojDecD.Text = opcije.BrojDecimalaD.ToString();
            txtBrojDecE1.Text = opcije.BrojDecimalaE1.ToString();
            txtBrojDecE.Text = opcije.BrojDecimalaE.ToString();
            txtBrojDecPen.Text = opcije.BrojDecimalaPen.ToString();
            txtBrojDecTotal.Text = opcije.BrojDecimalaTotal.ToString();
        }

        private void OpcijeForm_Shown(object sender, EventArgs e)
        {
            lblBrojDecD.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    Notification notification = new Notification();
                    requiredFieldsAndFormatValidation(notification);
                    if (!notification.IsValid())
                        throw new BusinessException(notification);

                    update();

                    session.Transaction.Commit();
                    closedByOK = true;
                }
            }
            catch (BusinessException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                if (ex.Notification != null)
                {
                    NotificationMessage msg = ex.Notification.FirstMessage;
                    MessageDialogs.showMessage(msg.Message, this.Text);
                    setFocus(msg.FieldName);
                }
                else if (!string.IsNullOrEmpty(ex.InvalidProperty))
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                    setFocus(ex.InvalidProperty);
                }
                else
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                }
                this.DialogResult = DialogResult.None;
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                //discardChanges();
                MessageDialogs.showError(ex.Message, this.Text);
                this.DialogResult = DialogResult.Cancel;
                closedByCancel = true;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                //discardChanges();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
                closedByCancel = true;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void requiredFieldsAndFormatValidation(Notification notification)
        {
            int dummyInt;
            if (txtBrojDecD.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaD", "Broj decimala D ocene je obavezan.");
            }
            else if (!int.TryParse(txtBrojDecD.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "BrojDecimalaD", "Nepravilan format za broj decimala D ocene.");
            }

            if (txtBrojDecE1.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE1", "Broj decimala E1-E6 ocena je obavezan.");
            }
            else if (!int.TryParse(txtBrojDecE1.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "BrojDecimalaE1", "Nepravilan format za broj decimala E1-E6 ocena.");
            }

            if (txtBrojDecE.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE", "Broj decimala E ocene je obavezan.");
            }
            else if (!int.TryParse(txtBrojDecE.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "BrojDecimalaE", "Nepravilan format za broj decimala E ocene.");
            }

            if (txtBrojDecPen.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaPen", "Broj decimala penalizacije je obavezan.");
            }
            else if (!int.TryParse(txtBrojDecPen.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "BrojDecimalaPen", "Nepravilan format za broj decimala penalizacije.");
            }

            if (txtBrojDecTotal.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "BrojDecimalaTotal", "Broj decimala konacne ocene je obavezan.");
            }
            else if (!int.TryParse(txtBrojDecTotal.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "BrojDecimalaTotal", "Nepravilan format za broj decimala konacne ocene.");
            }

        }

        private void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "BrojDecimalaD":
                    txtBrojDecD.Focus();
                    break;

                case "BrojDecimalaE1":
                    txtBrojDecE1.Focus();
                    break;

                case "BrojDecimalaE":
                    txtBrojDecE.Focus();
                    break;

                case "BrojDecimalaPen":
                    txtBrojDecPen.Focus();
                    break;

                case "BrojDecimalaTotal":
                    txtBrojDecTotal.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        private void update()
        {
            Notification notification = new Notification();
            validate(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);

            updateEntityFromUI(opcije);
            DAOFactoryFactory.DAOFactory.GetOpcijeDAO().Update(opcije);
        }

        private void validate(Notification notification)
        {
            int brojDecD = int.Parse(txtBrojDecD.Text);
            if (brojDecD <= 0 || brojDecD > 3)
            {
                notification.RegisterMessage(
                    "BrojDecimalaD", "Broj decimala D ocene mora da bude izmedju 1 i 3.");
            }

            int brojDecE1 = int.Parse(txtBrojDecE1.Text);
            if (brojDecE1 <= 0 || brojDecE1 > 3)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE1", "Broj decimala E1-E6 ocena mora da bude izmedju 1 i 3.");
            }

            int brojDecE = int.Parse(txtBrojDecE.Text);
            if (brojDecE <= 0 || brojDecE > 3)
            {
                notification.RegisterMessage(
                    "BrojDecimalaE", "Broj decimala E ocene mora da bude izmedju 1 i 3.");
            }

            int brojDecPen = int.Parse(txtBrojDecPen.Text);
            if (brojDecPen <= 0 || brojDecPen > 3)
            {
                notification.RegisterMessage(
                    "BrojDecimalaPen", "Broj decimala penalizacije mora da bude izmedju 1 i 3.");
            }

            int brojDecTotal = int.Parse(txtBrojDecTotal.Text);
            if (brojDecTotal <= 0 || brojDecTotal > 3)
            {
                notification.RegisterMessage(
                    "BrojDecimalaTotal", "Broj decimala konacne ocene mora da bude izmedju 1 i 3.");
            }
        }

        private void updateEntityFromUI(Opcije opcije)
        {
            opcije.BrojDecimalaD = byte.Parse(txtBrojDecD.Text);
            opcije.BrojDecimalaE1 = byte.Parse(txtBrojDecE1.Text);
            opcije.BrojDecimalaE = byte.Parse(txtBrojDecE.Text);
            opcije.BrojDecimalaPen = byte.Parse(txtBrojDecPen.Text);
            opcije.BrojDecimalaTotal = byte.Parse(txtBrojDecTotal.Text);

        }

        protected virtual void discardChanges()
        {
            // Empty
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            discardChanges();
            closedByCancel = true;
        }

        private void OpcijeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closedByOK && !closedByCancel)
            {
                if (isDirty())
                {
                    bool canClose = MessageBox.Show(
                        "Izmene koje ste uneli nece biti sacuvane?", "Klub",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) ==
                            DialogResult.OK;
                    e.Cancel = !canClose;
                }
            }
        }

        private bool isDirty()
        {
            // TODO
            return true;
        }

        private void OpcijeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closedByOK && !closedByCancel)
            {
                discardChanges();
            }
        }

    }
}