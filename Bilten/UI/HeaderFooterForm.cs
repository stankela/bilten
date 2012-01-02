using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class HeaderFooterForm : Form
    {
        private DeoTakmicenjaKod deoTakKod;
        public DeoTakmicenjaKod DeoTakKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        public HeaderFooterForm(DeoTakmicenjaKod deoTakKod, bool prikaziDEOceneVisible, bool brojSpravaPoStraniVisible)
        {
            InitializeComponent();
            this.Text = "Opcije za stampanje";
            this.deoTakKod = deoTakKod;

            ckbPrikaziDEOcene.Visible = prikaziDEOceneVisible;
            ckbPrikaziDEOcene.Enabled = prikaziDEOceneVisible;

            panel1.Visible = brojSpravaPoStraniVisible;
            panel1.Enabled = brojSpravaPoStraniVisible;
            
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();

            initFont();

            Cursor.Hide();
            Cursor.Current = Cursors.Arrow;
 
            
            initSize();
        }

        private void initFont()
        {
            cmbFont1.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFont1.DataSource = createFontNames();

            cmbFont2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFont2.DataSource = createFontNames();

            cmbFont3.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFont3.DataSource = createFontNames();

            cmbFont4.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFont4.DataSource = createFontNames();

            cmbFontFooter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFontFooter.DataSource = createFontNames();
        }

        private List<string> createFontNames()
        {
            List<string> result = new List<string>();
            foreach (FontFamily ff in FontFamily.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular)
                    && ff.IsStyleAvailable(FontStyle.Bold)
                    && ff.IsStyleAvailable(FontStyle.Italic))
                {
                    result.Add(ff.Name);
                }
            }
            return result;
        }

        private void initSize()
        {
            cmbSize1.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSize1.DataSource = createFontSizes();

            cmbSize2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSize2.DataSource = createFontSizes();

            cmbSize3.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSize3.DataSource = createFontSizes();

            cmbSize4.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSize4.DataSource = createFontSizes();

            cmbSizeFooter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSizeFooter.DataSource = createFontSizes();
        }

        private List<int> createFontSizes()
        {
            List<int> result = new List<int>();
            for (int i = 8; i <= 24; i++)
            {
                result.Add(i);
            }
            return result;
        }

        private string _header1Text;
        public string Header1Text
        {
            get { return _header1Text; }
            set { txtHeader1.Text = value; }
        }

        private string _header2Text;
        public string Header2Text
        {
            get { return _header2Text; }
            set { txtHeader2.Text = value; }
        }

        private string _header3Text;
        public string Header3Text
        {
            get { return _header3Text; }
            set { txtHeader3.Text = value; }
        }

        private string _header4Text;
        public string Header4Text
        {
            get { return _header4Text; }
            set { txtHeader4.Text = value; }
        }

        private string _footerText;
        public string FooterText
        {
            get { return _footerText; }
            set { txtFooter.Text = value; }
        }

        private string _header1Font;
        public string Header1Font
        {
            get { return _header1Font; }
            set { selectFont(cmbFont1, value); }
        }

        private string _header2Font;
        public string Header2Font
        {
            get { return _header2Font; }
            set { selectFont(cmbFont2, value); }
        }

        private string _header3Font;
        public string Header3Font
        {
            get { return _header3Font; }
            set { selectFont(cmbFont3, value); }
        }

        private string _header4Font;
        public string Header4Font
        {
            get { return _header4Font; }
            set { selectFont(cmbFont4, value); }
        }

        private string _footerFont;
        public string FooterFont
        {
            get { return _footerFont; }
            set { selectFont(cmbFontFooter, value); }
        }

        private int _header1FontSize;
        public int Header1FontSize
        {
            get { return _header1FontSize; }
            set { selectSize(cmbSize1, value); }
        }

        private int _header2FontSize;
        public int Header2FontSize
        {
            get { return _header2FontSize; }
            set { selectSize(cmbSize2, value); }
        }

        private int _header3FontSize;
        public int Header3FontSize
        {
            get { return _header3FontSize; }
            set { selectSize(cmbSize3, value); }
        }

        private int _header4FontSize;
        public int Header4FontSize
        {
            get { return _header4FontSize; }
            set { selectSize(cmbSize4, value); }
        }

        private int _footerFontSize;
        public int FooterFontSize
        {
            get { return _footerFontSize; }
            set { selectSize(cmbSizeFooter, value); }
        }

        private bool _header1FontBold;
        public bool Header1FontBold
        {
            get { return _header1FontBold; }
            set { ckbBold1.Checked = value; }
        }

        private bool _header2FontBold;
        public bool Header2FontBold
        {
            get { return _header2FontBold; }
            set { ckbBold2.Checked = value; }
        }

        private bool _header3FontBold;
        public bool Header3FontBold
        {
            get { return _header3FontBold; }
            set { ckbBold3.Checked = value; }
        }

        private bool _header4FontBold;
        public bool Header4FontBold
        {
            get { return _header4FontBold; }
            set { ckbBold4.Checked = value; }
        }

        private bool _footerFontBold;
        public bool FooterFontBold
        {
            get { return _footerFontBold; }
            set { ckbBoldFooter.Checked = value; }
        }

        private bool _header1FontItalic;
        public bool Header1FontItalic
        {
            get { return _header1FontItalic; }
            set { ckbItalic1.Checked = value; }
        }

        private bool _header2FontItalic;
        public bool Header2FontItalic
        {
            get { return _header2FontItalic; }
            set { ckbItalic2.Checked = value; }
        }

        private bool _header3FontItalic;
        public bool Header3FontItalic
        {
            get { return _header3FontItalic; }
            set { ckbItalic3.Checked = value; }
        }

        private bool _header4FontItalic;
        public bool Header4FontItalic
        {
            get { return _header4FontItalic; }
            set { ckbItalic4.Checked = value; }
        }

        private bool _footerFontItalic;
        public bool FooterFontItalic
        {
            get { return _footerFontItalic; }
            set { ckbItalicFooter.Checked = value; }
        }

        private bool _prikaziDEOcene;
        public bool PrikaziDEOcene
        {
            get { return _prikaziDEOcene; }
            set 
            { 
                _prikaziDEOcene = value;
                ckbPrikaziDEOcene.Checked = value; 
            }
        }

        private bool _stampajSveSprave;
        public bool StampajSveSprave
        {
            get { return _stampajSveSprave; }
            set
            {
                _stampajSveSprave = value;
                rbtSveSprave.Checked = value;
                rbtSamoIzabranaSprava.Checked = !value;
                if (panel1.Enabled)
                {
                    lblBrojSprava.Enabled = rbtSveSprave.Checked;
                    txtBrojSprava.Enabled = rbtSveSprave.Checked;
                }
            }
        }

        private int _brojSpravaPoStrani;
        public virtual int BrojSpravaPoStrani
        {
            get { return _brojSpravaPoStrani; }
            set{
                _brojSpravaPoStrani = value;
                txtBrojSprava.Text = value.ToString();
            }
        }

        private void selectFont(ComboBox cmb, string value)
        {
            List<string> fontNames = cmb.DataSource as List<string>;
            int index = fontNames.IndexOf(value);
            if (index != -1)
                cmb.SelectedIndex = index;
            else
                cmb.SelectedIndex = fontNames.IndexOf("Times New Roman");
        }

        private void selectSize(ComboBox cmb, int value)
        {
            List<int> fontSizes = cmb.DataSource as List<int>;
            int index = fontSizes.IndexOf(value);
            if (index != -1)
                cmb.SelectedIndex = index;
            else
                cmb.SelectedIndex = fontSizes.IndexOf(12);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtBrojSprava.Enabled)
            {
                int dummyInt;
                string errorMsg = String.Empty;
                if (txtBrojSprava.Text.Trim() == String.Empty)
                {
                    errorMsg = "Unesite broj sprava po strani.";
                }
                else if (!int.TryParse(txtBrojSprava.Text, out dummyInt) || dummyInt < 1 || dummyInt > 3)
                {
                    errorMsg = "Broj sprava po strani moze da bude 1, 2 ili 3.";
                }
                if (errorMsg != String.Empty)
                {
                    MessageDialogs.showMessage(errorMsg, this.Text);
                    txtBrojSprava.Focus();
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            _header1Text = txtHeader1.Text.Trim();
            _header2Text = txtHeader2.Text.Trim();
            _header3Text = txtHeader3.Text.Trim();
            _header4Text = txtHeader4.Text.Trim();
            _footerText = txtFooter.Text.Trim();

            _header1Font = (string)cmbFont1.SelectedItem;
            _header2Font = (string)cmbFont2.SelectedItem;
            _header3Font = (string)cmbFont3.SelectedItem;
            _header4Font = (string)cmbFont4.SelectedItem;
            _footerFont = (string)cmbFontFooter.SelectedItem;

            _header1FontSize = Int32.Parse(cmbSize1.SelectedItem.ToString());
            _header2FontSize = Int32.Parse(cmbSize2.SelectedItem.ToString());
            _header3FontSize = Int32.Parse(cmbSize3.SelectedItem.ToString());
            _header4FontSize = Int32.Parse(cmbSize4.SelectedItem.ToString());
            _footerFontSize = Int32.Parse(cmbSizeFooter.SelectedItem.ToString());
            
            _header1FontBold = ckbBold1.Checked;
            _header2FontBold = ckbBold2.Checked;
            _header3FontBold = ckbBold3.Checked;
            _header4FontBold = ckbBold4.Checked;
            _footerFontBold = ckbBoldFooter.Checked;

            _header1FontItalic = ckbItalic1.Checked;
            _header2FontItalic = ckbItalic2.Checked;
            _header3FontItalic = ckbItalic3.Checked;
            _header4FontItalic = ckbItalic4.Checked;
            _footerFontItalic = ckbItalicFooter.Checked;

            _prikaziDEOcene = ckbPrikaziDEOcene.Checked;
            _stampajSveSprave = rbtSveSprave.Checked;
            if (_stampajSveSprave)
                _brojSpravaPoStrani = Int32.Parse(txtBrojSprava.Text);
        }

        private string getSelectedFont(ComboBox cmb)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void HeaderFooterForm_Shown(object sender, EventArgs e)
        {
            lblHeader1.Focus();
        }

        private void rbtSveSprave_CheckedChanged(object sender, EventArgs e)
        {
            lblBrojSprava.Enabled = rbtSveSprave.Checked;
            txtBrojSprava.Enabled = rbtSveSprave.Checked;
        }

    }
}