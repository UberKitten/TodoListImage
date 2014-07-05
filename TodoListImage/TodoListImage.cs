using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace TodoListImage
{
    public partial class TodoListImage : ServiceBase
    {
        public TodoListImage()
        {
            InitializeComponent();
        }

        private FileSystemWatcher fsw;
        private FileInfo file;
        private DirectoryInfo imageDir;

        protected override void OnStart(string[] args)
        {
            MyOnStart();
        }

        internal void MyOnStart()
        {
            file = new FileInfo(Properties.Settings.Default.TodoPath);
            imageDir = new DirectoryInfo(Properties.Settings.Default.ImageDir);
            fsw = new FileSystemWatcher(file.DirectoryName, file.Name);

            fsw.Changed += fsw_Changed;
            fsw.Error += fsw_Error;

            fsw.EnableRaisingEvents = true;
        }

        void fsw_Error(object sender, ErrorEventArgs e)
        {
            this.Stop();
        }

        Regex date = new Regex("^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}[ ]{0,1}", RegexOptions.Compiled);

        private string[] ParseFile()
        {
            string[] allLines = File.ReadAllLines(file.FullName, UTF8Encoding.UTF8);
            List<string> lines = new List<string>();

            foreach (string line in allLines)
            {
                // Lines that start with x are completed
                if (!line.StartsWith("x"))
                {
                    // Remove the date from the beginning, when it was added
                    lines.Add("· " + date.Replace(line, ""));
                }
            }

            return lines.ToArray();
        }

        const int marginY = 5;

        // http://stackoverflow.com/a/3042963
        DateTime lastRead = DateTime.MinValue;

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(file.FullName);

            if (lastWriteTime != lastRead)
            {
                Debug.WriteLine("Change detected: " + e.ChangeType.ToString());
                String[] lines = ParseFile();
                Debug.WriteLine(lines.Count() + " lines");

                // Create first image
                Image img = new Bitmap(1, 1);
                Graphics drawing = Graphics.FromImage(img);

                // Used for drawing text
                Font font = new Font("Verdana", 14);
                Brush textBrush = Brushes.White;

                // Measure size of text and minimum image size
                SizeF min = new SizeF(0, 0);
                SizeF[] lineSizes = new SizeF[lines.Count()];
                for (int i = 0; i < lines.Count(); i++)
                {
                    lineSizes[i] = drawing.MeasureString(lines[i], font);
                    min.Width = Math.Max(min.Width, lineSizes[i].Width);

                    if (i == lines.Count())
                        min.Height += lineSizes[i].Height;
                    else
                        min.Height += lineSizes[i].Height + marginY;

                }

                Debug.Print(String.Format("Size: {0}x{1}", min.Width, min.Height));

                // Clear these up so we can use them again with no leaks
                drawing.Dispose();
                img.Dispose();

                // Create new images with our desired size
                img = new Bitmap((int)Math.Ceiling(min.Width), (int)Math.Ceiling(min.Height));
                drawing = Graphics.FromImage(img);

                // Paint it black
                drawing.Clear(Color.Black);

                // Where we're drawing our current text
                PointF current = new PointF(0, 0);

                // Draw it!
                for (int i = 0; i < lines.Count(); i++)
                {
                    drawing.DrawString(lines[i], font, textBrush, current);

                    current.Y += lineSizes[i].Height + marginY;
                }

                // Save as PNG
                Debug.WriteLine("Saving");
                img.Save(Path.Combine(imageDir.FullName, "Background 1.png"), System.Drawing.Imaging.ImageFormat.Png);
                img.Save(Path.Combine(imageDir.FullName, "Background 2.png"), System.Drawing.Imaging.ImageFormat.Png);

                // Clean up
                drawing.Dispose();
                img.Dispose();

                // Any Changed events AFTER this point will work correctly
                lastRead = lastWriteTime;
            }
        }

        protected override void OnStop()
        {
            fsw.Dispose();
        }
    }
}
