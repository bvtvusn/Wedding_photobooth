using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EZ_http_browser;

namespace WeddingPhotobooth
{
    public partial class Form1 : Form
    {
        bool fullScreenstate;

        //EzShareClient ez_Client;
        //DataAccessClass downloader;
        //HttpFolder root;
        PhotoboothLogic photobooth;
        // State machine:

        //      //Waiting mode
        //Countdown mode
        //Download Thumbnail (not saved)
        //Download Lage image (Can be aborted?)
        //Keep or delete(With timer)


        //Back to waiting mode

        // Future work:
        // Make arduino activate the camera. (Alternatively, activate it manually)
        // Make nice GUI
        // Test imagesharing with FTP
        // Test "mouse" remote control

            // Bilde av fjernkontrollen
            // Bilde av en bilderamme
        public Form1()
        {
            InitializeComponent();

            photobooth = new PhotoboothLogic();
            photobooth.NormalUserMessageEvent += Photobooth_NormalUserMessageEvent; ;
            photobooth.StateUpdateEvent += Photobooth_StateUpdateEvent; ;
            photobooth.ThumbnailReceivedEvent += Photobooth_ImageReceived;
            photobooth.FullImageReceived += Photobooth_ImageReceived;
            photobooth.ErrorEvent += Photobooth_ErrorEvent; ;
            photobooth.DebugEvent += Photobooth_DebugEvent; ;

            photobooth.downloader.downloadProgresChangedEvent += Downloader_downloadProgresChangedEvent; ;


        }

        private void Photobooth_StateUpdateEvent(PhotoboothLogic.PhotoboothStates obj)
        {
            switch (obj)
            {
                case PhotoboothLogic.PhotoboothStates.Idle:
                    pictureBox1.Image = null;
                    lblHeading.Text = "Dette er en photobooth. Klikk på \"Ta bilde\" på fjernkontrollen.";
                    pictureBox1.Image = global::WeddingPhotobooth.Properties.Resources.PictureFrame1;
                    break;
                case PhotoboothLogic.PhotoboothStates.Countdown:
                    lblError.Text = ""; // Removes error message every loop;
                    lblHeading.Text = "Nedtelling";
                    break;
                case PhotoboothLogic.PhotoboothStates.WaitForImage:
                    lblHeading.Text = "Venter på bilde";
                    break;
                case PhotoboothLogic.PhotoboothStates.DownloadingThumbnail:
                    lblHeading.Text = "Laster inn bilde";
                    break;
                case PhotoboothLogic.PhotoboothStates.DownloadingFullsize:
                    lblHeading.Text = "Laster inn full oppløsning";
                    break;
                case PhotoboothLogic.PhotoboothStates.KeepDecision:
                    lblHeading.Text = "Trykk \"Slett\" for å slette bildet. Bildet lagres ellers automatisk.";
                    progressBar1.Value = 0;
                    break;
                case PhotoboothLogic.PhotoboothStates.DecisionConfirmation:
                    lblHeading.Text = "Bildet er ...";
                    break;
                default:
                    break;
            }
            
        }

        private void Downloader_downloadProgresChangedEvent(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Photobooth_ErrorEvent(string obj)
        {
            //txtError.Text = obj;
            lblError.Text = "Error: \r\n" + obj;
        }

        private void Photobooth_DebugEvent(string obj)
        {
            txtDebug.AppendText(obj + "\r\n");
        }

        private void Photobooth_ImageReceived(Bitmap obj)
        {
            pictureBox1.Image = obj;
        }

        private void Photobooth_NormalUserMessageEvent(string obj)
        {
            lblHeading.Text = obj;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Q)
            {
                ToggleFullscreen();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (fullScreenstate)
            {
    //0x210 is WM_PARENTNOTIFY
                if (m.Msg == 0x210 && m.WParam.ToInt32() == 513) //513 is WM_LBUTTONCLICK
                {
                    photobooth.StartCapOrKeep();
                    //MessageBox.Show("left"); //You have a mouseclick(left)on the underlying user control
                }
                if (m.Msg == 0x210 && m.WParam.ToInt32() == 516) //513 is WM_RBUTTONCLICK
                {
                    //photobooth.KeepImageDecision(false);
                    photobooth.DeleteButtonPressed();
                    //MessageBox.Show("Right"); //You have a mouseclick(left)on the underlying user control
                }
            }
            
            base.WndProc(ref m);
        }



        private void btnFullscreen_Click(object sender, EventArgs e)
        {
            ToggleFullscreen();


        }

        private void ToggleFullscreen()
        {
            fullScreenstate = Control.DoFullscreen(this);
            grpSettings.Visible = !fullScreenstate;
        }

        private void grpSettings_Enter(object sender, EventArgs e)
        {

        }

        

        private void btnCapture_Click(object sender, EventArgs e)
        {
            photobooth.StartCapOrKeep();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //photobooth.KeepImageDecision(false);
            photobooth.DeleteButtonPressed();
        }
        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    MessageBox.Show(e.KeyCode.ToString() + " " + e.KeyData.ToString() + " " + e.KeyValue.ToString());
        //}
    }

    static class Control
    {
        static bool isMax = false, isFull = false;
        static Point old_loc, default_loc;
        static Size old_size, default_size;

        public static void SetInitial(Form form)
        {
            old_loc = form.Location;
            old_size = form.Size;
            default_loc = form.Location;
            default_size = form.Size;
        }

        //public static DoMaximize(Form form, Button btnMax)
        //{
        //    if (isMax == false) // If not maximized, maximize!!
        //    {
        //        old_loc = new Point(form.Location.X, form.Location.Y);
        //        old_size = new Size(form.Size.Width, form.Size.Height);
        //        Maximize(form);
        //        isMax = true;
        //        isFull = false;
        //        btnMax.Text = "2";
        //    }
        //    else
        //    {

        //    }
        //}

        public static bool DoFullscreen(Form form)
        {
            if (isFull == false) // If not Fullscreen, Do fullscreen!!
            {
                old_loc = new Point(form.Location.X, form.Location.Y);
                old_size = new Size(form.Size.Width, form.Size.Height);
                Fullscreen(form);
                isMax = false;
                isFull = true;
                return true;
            }
            else
            {
                form.Location = old_loc;
                form.Size = old_size;
                Fullscreen(form);
                isMax = false;
                isFull = false;
                return false;
            }
        }
        static void Fullscreen(Form form)
        {
            if (form.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Normal;
                form.TopMost = false;
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
                
            else if (form.WindowState == FormWindowState.Normal)
            {
                form.TopMost = true;
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
            }
        }

        static void Maximize(Form form)
        {
            int x = SystemInformation.WorkingArea.Width;
            int y = SystemInformation.WorkingArea.Height;
            form.WindowState = FormWindowState.Normal;
            form.Location = new Point(0, 0);
            form.Size = new Size(x, y);
        }

        static void Exit()
        {
            Application.Exit();
        }
        
    }
}
