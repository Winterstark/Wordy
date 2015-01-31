using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Wordy
{
    public partial class formAbout : Form
    {
        public formMain main;


        public formAbout()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Winterstark/Wordy");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Winterstark");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.flickr.com/services/api/");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://rhymebrain.com/");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://xtux345.deviantart.com/art/Elements-of-Harmony-Dictionary-Icon-280443607?q=boost%3Apopular%20dictionary%20icon&qo=9");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.psdgraphics.com/psd-icons/psd-check-and-cross-icons/");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:winterstark@gmail.com");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://flickrnet.codeplex.com/");
        }

        private void formAbout_Load(object sender, EventArgs e)
        {
            //icon
            string iconPath = Application.StartupPath + "\\Wordy.ico";

            if (File.Exists(iconPath))
                this.Icon = new Icon(iconPath);

            //logo
            string logoPath = Application.StartupPath + "\\Wordy.png";

            if (File.Exists(logoPath))
                picLogo.ImageLocation = logoPath;
        }

        private void formAbout_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Show();
        }

        private void picWordnik_Click(object sender, EventArgs e)
        {
            Process.Start("http://developer.wordnik.com/");
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://msdn.microsoft.com/en-us/library/dd576287.aspx");
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://adamwhitcroft.com/batch/");
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://modernuiicons.com/");
        }

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.customicondesign.com/free-icons/flag-icon-set/flat-round-world-flag-icon-set/");
        }

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.freesound.org/people/Bertrof/packs/8276/");
        }
    }
}
