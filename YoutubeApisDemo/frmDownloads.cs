﻿using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using VideoLibrary;
using System.Windows.Forms;
using System.Threading;
using YoutubeExtractor;

namespace YoutubeApisDemo
{
    public partial class frmDownloads : MaterialForm
    {
        #region Fields
        private bool IsAudioFormat = false;
        private string VideoName = "";
        private List<string> lstQuality;
        private string VideoSize = "";
        private string VideoFormat = "";
        private string Extension = "";
        private string AudioFormat = "";
        private string AudioBitrate = "";
        private byte[] bytesToSave;
        #endregion

        #region Contructor
        public frmDownloads(string youtubeUrl)
        {
            InitializeComponent();
            GetVideoInfo(youtubeUrl);
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            lblSize.Text = VideoSize;
            lblName.Text = VideoName;
            foreach (var item in lstQuality)
            {
                cbQuality.Items.Add(item);
            }
            cbQuality.SelectedIndex = 0;
            saveFileDlg.Title = "Save video to...";

            if (IsAudioFormat == false)
            {
                saveFileDlg.Filter = GetVideoExtension(VideoFormat);
            }
            else
            {
                saveFileDlg.Filter = GetAudioExtension(VideoFormat);
            }

            saveFileDlg.InitialDirectory = GetDefaultPath();
            saveFileDlg.FileName = GetDefaultPath() + "\\" + RemoveSpace(VideoName);
            lblPath.Text = saveFileDlg.FileName;
        }
        #endregion

        #region Methods and Events

        private static string RemoveSpace(string strInput)
        {
            var res = "";
            if (strInput.Contains(" "))
            {
                res = strInput.Replace(" ", "");
            }

            return res;
        }

        private static string GetDefaultPath()
        {
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userPath, "Downloads");
        }

        private static string GetVideoExtension(string VideoFormat)
        {
            var res = "";
            switch (VideoFormat)
            {
                case "WebM":
                    res = "HTML5 Video Format (*.webm)|*.webm";
                    break;
                case "Mp4":
                    res = "MPEG-4 Video Format (*.mp4)|*.mp4";
                    break;
                case "Flash":
                    res = "Flash Video (*.flv)|*.flv";
                    break;
                case "Mobile":
                    res = "Mobile Device Format (*.3gp)|*.3gp";
                    break;
                case "Unknown":
                    res = "All file (*.*)|*.*";
                    break;
                default:
                    res = "All file (*.*)|*.*";
                    break;
            }

            return res;
        }

        private static string GetAudioExtension(string AudioFormat)
        {
            var res = "";
            switch (AudioFormat)
            {
                case "Aac":
                    res = "Advanced Audio Coding Format (*.aac)|*.aac";
                    break;
                case "Mp3":
                    res = "MPEG-3 Audio Format (*.mp3)|*.mp3";
                    break;
                case "Vorbis":
                    res = "Vorbis OGG Format (*.ogg)|*.ogg;*.ora";
                    break;
                case "Unknown":
                    res = "All file (*.*)|*.*";
                    break;
                default:
                    res = "All file (*.*)|*.*";
                    break;
            }
            return res;
        }

        private KeyValuePair<string, double> ConvertToByteSize(double bytesData)
        {
            var rc = 1024;
            double valueData = 0;
            string keySize = "";
            if (bytesData >= (rc * rc * rc))
            {
                keySize = "GB";
                valueData = bytesData / (rc * rc * rc);
            }
            else
            {
                if (bytesData >= rc * rc)
                {
                    keySize = "MB";
                    valueData = bytesData / (rc * rc);
                }
                else
                {
                    if (bytesData >= rc)
                    {
                        keySize = "KB";
                        valueData = bytesData / rc;
                    }
                    else
                    {
                        keySize = "B";
                        valueData = bytesData;
                    }
                }
            }
            return new KeyValuePair<string, double>(keySize, valueData);
        }

        private void GetVideoInfo(string VideoURL)
        {
            using (var services = Client.For(YouTube.Default))
            {
                var video = services.GetVideo(VideoURL);
                VideoName = video.Title;               
                AudioFormat = video.AudioFormat.ToString();
                AudioBitrate = video.AudioBitrate.ToString();
                var tempResolution = video.Resolution;
                Extension = video.FileExtension;
                VideoFormat = video.Format.ToString();
                lstQuality = new List<string>();
                lstQuality.Add(VideoFormat.ToUpper() + " - " + tempResolution + "p");
                lstQuality.Add("Audio " + AudioFormat.ToUpper() + " - " + AudioBitrate + "Kbps");
                
                
            }
        }

        private void GetVideSize(double val)
        {           
            var space = ConvertToByteSize(val);
            var temp = Convert.ToInt32(space.Value);
            double temp2 = space.Value - temp;
            var temp3 = Math.Round(space.Value, 2);
            VideoSize = string.Format("{0} {1}", temp3, space.Key);
        }
        

        private void cbQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbQuality.SelectedIndex == 0)
            {
                IsAudioFormat = false;
                saveFileDlg.Filter = GetVideoExtension(VideoFormat);
                lblPath.Text = saveFileDlg.FileName;
            }
            if (cbQuality.SelectedIndex == 1)
            {
                IsAudioFormat = true;
                saveFileDlg.Filter = GetAudioExtension(VideoFormat);
                lblPath.Text = saveFileDlg.FileName;
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            if (IsAudioFormat == false)
            {
                saveFileDlg.Filter = GetVideoExtension(VideoFormat);
            }
            else
            {
                saveFileDlg.Filter = GetAudioExtension(AudioFormat);
            }

            var dlg = saveFileDlg.ShowDialog();

            if (dlg == DialogResult.OK)
            {
                lblPath.Text = saveFileDlg.FileName;
            }
        }

        private void bwDownload_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            for(int i = 0; i <= 100; i++)
            {
                Thread.Sleep(100);
                bwDownload.ReportProgress(i);
            }
            
        }

        private void bwDownload_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
            lblStatus.Text = "Downloading " + pbStatus.Value.ToString() + "%...";
        }

        private void bwDownload_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "Completed!";
            //pbStatus.Style = ProgressBarStyle.Blocks;
            btnDownload.Enabled = true;
            btnDownload.Text = "Re-download";
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btDownload_Click(object sender, EventArgs e)
        {
            btnDownload.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
            if (bwDownload.IsBusy == true)
            {
                return;
            }
            else
            {
                btnPause.Visible = true;
                pbStatus.Style = ProgressBarStyle.Blocks;
                bwDownload.RunWorkerAsync();
                
            }
            
        }

        private void frmDownloads_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Ready";
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            bwDownload.CancelAsync();
        }
        #endregion
    }
}
