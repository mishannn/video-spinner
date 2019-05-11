using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace PhotosToVideo
{
    class VideoCreator
    {
        public delegate void ChangeImageHandler(Image image);
        public event ChangeImageHandler ChangeImage;

        private Thread _spinThread = null;
        private bool _started = false;

        private string[] _imagePaths = null;
        private Image[] _images = null;

        public int Speed { get; set; } = 5;
        public bool Reverse { get; set; } = false;

        //public VideoCreator() { }

        private int GetNextImageIndex(int currentImageIndex)
        {
            if (Reverse)
            {
                if (currentImageIndex > 0)
                {
                    return currentImageIndex - 1;
                }
                else
                {
                    return _imagePaths.Length - 1;
                }
            }
            else
            {
                if (currentImageIndex < (_imagePaths.Length - 1))
                {
                    return currentImageIndex + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void Spin()
        {
            int imageIndex = 0;
            while (_started)
            {
                ChangeImage?.Invoke(_images[imageIndex]);
                Thread.Sleep(1000 / Speed);

                imageIndex = GetNextImageIndex(imageIndex);
            }
        }

        public void Start(string path)
        {
            if (_started)
                return;

            _imagePaths = Directory.GetFiles(path)
                .Where(fileName => new Regex(@"^.+\\\d+\.jpg$").Match(fileName).Success)
                .ToArray();
            Array.Sort(_imagePaths, new ImagePathComparer());

            List<Image> images = new List<Image>();
            foreach (string imagePath in _imagePaths)
            {
                images.Add(Image.FromFile(imagePath));
            }
            _images = images.ToArray();

            _spinThread = new Thread(Spin);
            _spinThread.Start();
            _started = true;
        }

        public void Stop()
        {
            if (!_started)
                return;

            _started = false;
            _spinThread.Join();

            foreach (Image image in _images)
            {
                image.Dispose();
            }
        }

        public void SaveVideo(string path, int repeat)
        {
            if (_images == null || _images.Length == 0)
                return;

            using (VideoFileWriter writer = new VideoFileWriter())
            {
                int width = _images[0].Width;
                if (width % 2 == 1)
                    width += 1;

                int height = _images[0].Height;
                if (height % 2 == 1)
                    height += 1;

                int bitRate = 15 * 1024 * 1024;
                writer.Open(path, width, height, 30, VideoCodec.MPEG4, bitRate);

                for (int i = 0; i < repeat; i++)
                {
                    if (Reverse)
                    {
                        for (int j = _images.Length - 1; j > 0; j--)
                        {
                            for (int k = 0; k < 30 / Speed; k++)
                            {
                                writer.WriteVideoFrame((Bitmap)_images[j]);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < _images.Length; j++)
                        {
                            for (int k = 0; k < 30 / Speed; k++)
                            {
                                writer.WriteVideoFrame((Bitmap)_images[j]);
                            }
                        }
                    }
                }

                writer.Close();
            }
        }
    }
}

