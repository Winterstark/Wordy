using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace VisualMaker
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        private void formMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Image[] visuals = new Bitmap[files.Length];
            for (int i = 0; i < files.Length; i++)
                visuals[i] = Image.FromFile(files[i]);

            int nW, nH, ind;
            nW = 1 + (visuals.Length >= 2 ? 1 : 0) + (visuals.Length >= 5 ? 1 : 0);
            nH = 1 + (visuals.Length >= 3 ? 1 : 0);
            Bitmap visMosaic = new Bitmap(nW * 240, nH * 240);
            ind = 0;

            for (int y = 0; y < nH; y++)
                for (int x = 0; x < nW; x++)
                {
                    if (ind == visuals.Length)
                        break;

                    visMosaic.SetResolution(visuals[ind].HorizontalResolution, visuals[ind].VerticalResolution);
                    Graphics gfx = Graphics.FromImage(visMosaic);

                    int resizedW, resizedH;
                    if (visuals[ind].Width > visuals[ind].Height)
                    {
                        resizedW = 240;
                        resizedH = (int)((float)visuals[ind].Height / visuals[ind].Width * 240);
                    }
                    else
                    {
                        resizedW = (int)((float)visuals[ind].Width / visuals[ind].Height * 240);
                        resizedH = 240;
                    }

                    gfx.DrawImage(visuals[ind], x * 240 + (240 - resizedW) / 2, y * 240 + (240 - resizedH) / 2, resizedW, resizedH);

                    ind++;
                }

            foreach (Image vis in visuals)
                vis.Dispose();
            foreach (string file in files)
                File.Delete(file);

            string outputPath = files[0];
            for (int i = 0; i <= visuals.Length; i++)
                outputPath = outputPath.Replace(" (" + i.ToString() + ")", "");
            if (!outputPath.Contains(".jpg"))
                outputPath = outputPath.Substring(0, outputPath.LastIndexOf('.')) + ".jpg";

            visMosaic.Save(outputPath);
        }
    }
}
