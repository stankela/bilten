using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Bilten.UI
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            InitializeComponent();

            // Get this type's assembly
            Assembly asm = this.GetType().Assembly;

            // Get the stream for the "Images\UEG.jpg" emdedded resource
            // NOTE1: Make sure not to close this stream,
            //        or the Bitmap object will lose access to it
            // NOTE2: Also be very careful to match the case
            //        on the resource name itself
            // NOTE3: Build action for UEG.jpg must be Embedded resource 
            Stream stream =
              asm.GetManifestResourceStream("Bilten.Images.UEG.jpg");

            // Load the bitmap from the stream
            this.BackgroundImage = new Bitmap(stream);

            this.ClientSize = this.BackgroundImage.Size;
            this.Text = Application.ProductName;
        }
    }
}