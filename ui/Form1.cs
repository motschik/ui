using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ui
{
    public partial class Form1 : Form
    {
        public const int SIZE = 360;

        public Form1()
        {
            InitializeComponent();
            TopMost = true;
            Size = new System.Drawing.Size(SIZE, SIZE);
            pictureBox1.Size = new System.Drawing.Size(SIZE, SIZE);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private bool enableDance = true;

        private void Dance()
        {
            while (enableDance)
            {
                VideoCapture vcap = new VideoCapture("ui.mp4");

                while (vcap.IsOpened() && enableDance)
                {
                    Mat mat = new Mat();


                    if (vcap.Read(mat))
                    {
                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Image.Dispose();
                        }

                        if (mat.IsContinuous())
                        {
                            Mat matRsz = mat.Clone(new Rect(280, 0, 720, 720));
                            Mat hsvmat = matRsz.CvtColor(ColorConversionCodes.BGR2HSV);

                            Mat rangeMat = hsvmat.InRange(new Scalar(50, 100, 100), new Scalar(72, 255, 255));

                            Cv2.BitwiseNot(rangeMat, rangeMat);

                            Mat dst = new Mat();
                            matRsz.CopyTo(dst, rangeMat);
                            Cv2.Resize(dst, dst, new OpenCvSharp.Size(SIZE, SIZE));

                            Bitmap bmp = BitmapConverter.ToBitmap(dst);
                            pictureBox1.Image = bmp;

                            hsvmat.Dispose();
                            rangeMat.Dispose();
                            dst.Dispose();
                        }
                        else
                        {
                            break;
                        }
                        Application.DoEvents();
                    }
                    else
                    {
                        break;
                    }
                    Thread.Sleep((int)(300 / vcap.Fps));
                    mat.Dispose();
                }

                vcap.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                this.Invoke(new Action(() => Dance()));
            });

        }

        private System.Drawing.Point p1;
        private System.Drawing.Point p2;
        private bool moving = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            p1 = e.Location;
            moving = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                p2 = System.Drawing.Point.Subtract(e.Location, (System.Drawing.Size)p1);
                Location = System.Drawing.Point.Add(p2,(System.Drawing.Size)Location);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            enableDance = false;
        }
    }
}
