﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeApisDemo
{
    public partial class frmInitializers : Form
    {
        public frmInitializers(Form frmTarget, string LoadInfoText)
        {
            InitializeComponent();
            form = frmTarget;
            lblInfo.Text = LoadInfoText;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
        );
        Form form;
        private void frmInitializers_Load(object sender, EventArgs e)
        {
            loading.Start();
        }

        private void loading_Tick(object sender, EventArgs e)
        {
            this.Hide();
            loading.Stop();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            
        }
    }
}
