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
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class ZrebForm : Form
    {
        private Takmicenje takmicenje;

        public ZrebForm(int takmicenjeId)
        {
            InitializeComponent();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    initUI();
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

        private void initUI()
        {
            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            pictureBox1.Image = SlikeSprava.getImage(sprave[0]);
            pictureBox2.Image = SlikeSprava.getImage(sprave[1]);
            pictureBox3.Image = SlikeSprava.getImage(sprave[2]);
            pictureBox4.Image = SlikeSprava.getImage(sprave[3]);
            if (takmicenje.Gimnastika == Gimnastika.MSG)
            {
                pictureBox5.Image = SlikeSprava.getImage(sprave[4]);
                pictureBox6.Image = SlikeSprava.getImage(sprave[5]);
            }
            else
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                textBox6.Visible = false;
                textBox7.Visible = false;
            }
            if (String.IsNullOrEmpty(takmicenje.ZrebZaFinalePoSpravama))
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                return;
            }

            rbtJedanZreb.CheckedChanged -= new EventHandler(this.rbtJedanZreb_CheckedChanged);
            rbtPosebanZreb.CheckedChanged -= new EventHandler(this.rbtPosebanZreb_CheckedChanged);
            if (takmicenje.ZrebZaFinalePoSpravama[0] != '#')
            {
                textBox1.Text = takmicenje.ZrebZaFinalePoSpravama;
                rbtJedanZreb.Checked = true;
            }
            else
            {
                List<List<int>> zreb = Zreb.parseZreb(takmicenje.ZrebZaFinalePoSpravama, takmicenje.Gimnastika);
                textBox2.Text = Zreb.createRawZreb(zreb[0]);
                textBox3.Text = Zreb.createRawZreb(zreb[1]);
                textBox4.Text = Zreb.createRawZreb(zreb[2]);
                textBox5.Text = Zreb.createRawZreb(zreb[3]);
                if (takmicenje.Gimnastika == Gimnastika.MSG)
                {
                    textBox6.Text = Zreb.createRawZreb(zreb[4]);
                    textBox7.Text = Zreb.createRawZreb(zreb[5]);
                }
                rbtPosebanZreb.Checked = true;
            }
            rbtJedanZreb.CheckedChanged += new EventHandler(this.rbtJedanZreb_CheckedChanged);
            rbtPosebanZreb.CheckedChanged += new EventHandler(this.rbtPosebanZreb_CheckedChanged);
            rbtCheckedChanged();
        }

        private void focusSprava(string sprava, Gimnastika gim)
        {
            if (gim == Gimnastika.MSG)
            {
                if (sprava == "Parter")
                    textBox2.Focus();
                else if (sprava == "Konj")
                    textBox3.Focus();
                else if (sprava == "Karike")
                    textBox4.Focus();
                else if (sprava == "Preskok")
                    textBox5.Focus();
                else if (sprava == "Razboj")
                    textBox6.Focus();
                else if (sprava == "Vratilo")
                    textBox7.Focus();
            }
            else
            {
                if (sprava == "Preskok")
                    textBox2.Focus();
                else if (sprava == "Dvovisinski razboj")
                    textBox3.Focus();
                else if (sprava == "Greda")
                    textBox4.Focus();
                else if (sprava == "Parter")
                    textBox5.Focus();
            }
        }

        private bool validate()
        {
            if (!rbtJedanZreb.Checked && !rbtPosebanZreb.Checked)
            {
                MessageDialogs.showMessage("Unesite zreb za finale.", this.Text);
                return false;
            }

            if (rbtJedanZreb.Checked)
            {
                List<int> zreb = Zreb.parseRawZreb(textBox1.Text.Trim());
                if (zreb == null || zreb.Count == 0)
                {
                    MessageDialogs.showMessage("Nepravilno unesen zreb za finale.", this.Text);
                    textBox1.Focus();
                    return false;
                }
            }
            if (rbtPosebanZreb.Checked)
            {
                string[] sprave = Sprave.getSpraveNazivi(takmicenje.Gimnastika);
                string[] textBoxes;
                int brojSprava;
                if (takmicenje.Gimnastika == Gimnastika.MSG)
                {
                    textBoxes = new string[] { textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(),
                                               textBox5.Text.Trim(), textBox6.Text.Trim(), textBox7.Text.Trim() };
                    brojSprava = 6;
                }
                else
                {
                    textBoxes = new string[] { textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(),
                                               textBox5.Text.Trim() };
                    brojSprava = 4;
                }
                for (int i = 0; i < brojSprava; ++i)
                {
                    List<int> zreb = Zreb.parseRawZreb(textBoxes[i]);
                    if (zreb == null || zreb.Count == 0)
                    {
                        string sprava = sprave[i];
                        if (sprava == "Greda")
                            sprava = "Gredu";
                        MessageDialogs.showMessage("Nepravilno unesen zreb za " + sprava + ".", this.Text);
                        focusSprava(sprave[i], takmicenje.Gimnastika);
                        return false;
                    }
                }
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!validate())
            {
                DialogResult = DialogResult.None;
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (rbtJedanZreb.Checked)
                    {
                        takmicenje.ZrebZaFinalePoSpravama = textBox1.Text.Trim();
                    }
                    else
                    {
                        takmicenje.ZrebZaFinalePoSpravama = Zreb.kompresuj(takmicenje.Gimnastika,
                                       textBox2.Text.Trim(), textBox3.Text.Trim(), textBox4.Text.Trim(),
                                       textBox5.Text.Trim(), textBox6.Text.Trim(), textBox7.Text.Trim());
                    }

                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void ZrebForm_Shown(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void rbtCheckedChanged()
        {
            if (rbtJedanZreb.Checked)
            {
                textBox1.Enabled = true;
                setSpraveEnabled(false);
            }
            if (rbtPosebanZreb.Checked)
            {
                textBox1.Enabled = false;
                setSpraveEnabled(true);
            }
        }

        private void setSpraveEnabled(bool value)
        {
            textBox2.Enabled = value;
            textBox3.Enabled = value;
            textBox4.Enabled = value;
            textBox5.Enabled = value;
            textBox6.Enabled = value;
            textBox7.Enabled = value;
        }

        private void rbtJedanZreb_CheckedChanged(object sender, EventArgs e)
        {
            rbtCheckedChanged();
        }

        private void rbtPosebanZreb_CheckedChanged(object sender, EventArgs e)
        {
            rbtCheckedChanged();
        }
    }
}
