using System;
using System.Drawing;
using System.Windows.Forms;

namespace PhotosToVideo
{
    public partial class Form1 : Form
    {
        private VideoCreator _videoCreator = new VideoCreator();

        public Form1()
        {
            InitializeComponent();

            _videoCreator.ChangeImage += SetImage;
        }

        private void SetImage(Image image)
        {
            pictureBox1.Image = image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;

            _videoCreator.Stop();
            //_videoCreator.LoadImages(folderBrowserDialog1.SelectedPath);
            //_videoCreator.Start(@"C:\Users\MishaN\Desktop\newhouse");
            _videoCreator.Start(folderBrowserDialog1.SelectedPath);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _videoCreator.Reverse = checkBox1.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _videoCreator.Speed = Convert.ToInt32(numericUpDown1.Value);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _videoCreator.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            //_videoCreator.SaveVideo(@"C:\Users\MishaN\Desktop\video.mp4", Convert.ToInt32(numericUpDown2.Value));
            _videoCreator.SaveVideo(saveFileDialog1.FileName, Convert.ToInt32(numericUpDown2.Value));
        }
    }
}
