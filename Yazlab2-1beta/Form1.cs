using PuzzleGame;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yazlab2_1beta
{
    public partial class Form1 : Form
    {
        string path;
        Bitmap[,] bmps = new Bitmap[4, 4];
        int buttonId = -1;
        bool isFirstTime = true;
        int count = 0;
        int score = 100;
        int maxScore = 0;

        public Form1()
        {
            InitializeComponent();

            btn_Mix.Enabled = false;
            ButtonState(false);
            FileRead();
            maxLabel.Text = maxScore.ToString();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;


        }

        public void FileRead()
        {
            if (File.Exists("enyuksekskor.txt"))
            {
                var lines = File.ReadAllLines("enyuksekskor.txt");
                List<int> list = new List<int>();

                foreach (var line in lines)
                {
                    list.Add(Int32.Parse(line));
                }

                if(list.Count != 0)
                    maxScore = list.Max();
            }

        }

        public void FileWrite()
        {
            using (StreamWriter sw = File.AppendText("enyuksekskor.txt"))
            {
                sw.WriteLine(score);
            }
            

        }

        public void ButtonState(bool status)
        {
            for (int i = 0; i < 16; i++)
            {
                String a = "btn_P" + i;
                Controls.Find(a, false)[0].Enabled = status;
            }
        }

        public int[] Shuffle()
        {
            Random rastgele = new Random();
            var rand = new int[16];
            for (int i = 0; i < 16; i++)
            {
                rand[i] = i;
            }

            for (int i = 15; i >= 0; i--)
            {
                int sayi = rastgele.Next(9);
                int tmp = rand[i];
                rand[i] = rand[sayi];
                rand[sayi] = tmp;
            }

            return rand;
        }

        public void Mix(Bitmap[,] bmps)
        {
            var random = Shuffle();
            int a = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {

                    Button foobar = (Button)Controls.Find("btn_P" + a, false)[0];
                    Hashing hashing = new Hashing();
                    ImageConverter converter = new ImageConverter();
                    foobar.Tag = new Hashing();
                    

                    byte[] orgData = converter.ConvertTo(bmps[i, k], typeof(byte[])) as byte[];
                    hashing.Original = ImageHashing(orgData);
                    ((Hashing)foobar.Tag).Original = hashing.Original;

                    //foreach (var element in ((Hashing)foobar.Tag).Original)
                    //{
                    //    Console.Write(element);
                    //}
                    //Console.WriteLine("----------------------");





                    Button foobar2 = (Button)Controls.Find("btn_P" + random[a], false)[0];
                    //foobar2.Tag = Hashing.getInstance();
                    foobar2.BackgroundImage = bmps[i, k];




                    //byte[] rawImageData = converter.ConvertTo(foobar2.BackgroundImage, typeof(byte[])) as byte[];

                    //hashing.Mixed = ImageHashing(rawImageData);
                    //((Hashing)foobar2.Tag).Mixed = hashing.Mixed;

                    foobar2.BackgroundImageLayout = ImageLayout.Stretch;
                    a++;
                    //foreach (var element in ((Hashing)foobar.Tag).Mixed)
                    //{
                    //    Console.Write(element);
                    //}
                    //Console.WriteLine();
                }
            }
            isFirstTime = false;


            ButtonControl();

            isFinitto();

        }

        public void ButtonControl()
        {
            bool isOK = false;

            for (int i = 0; i < 16; i++)
            {
                Button foobar = (Button)Controls.Find("btn_P" + i, false)[0];

                ImageConverter converter = new ImageConverter();

                byte[] orgData = converter.ConvertTo(foobar.BackgroundImage, typeof(byte[])) as byte[];

                var newHash = ImageHashing(orgData);

                if (newHash.SequenceEqual(((Hashing)foobar.Tag).Original))
                {
                    isOK = true;
                    foobar.FlatAppearance.BorderColor = Color.Green;
                    count++;
                }
            }

            if (isOK)
            {
                ButtonState(true);
                btn_Mix.Enabled = false;
            }
        }

        public byte[] ImageHashing(byte[] rawImageData)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(rawImageData);

            //foreach (var element in hash)
            //{
            //    Console.Write(element);
            //}
            //Console.WriteLine();

            return hash;
        }

        public string FilePath()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    btn_Mix.Enabled = true;
                    return dlg.FileName;
                }

                else
                {
                    ///todo yaparsın sen aslan
                    return null;
                }
            }
        }

        // sec
        private void button1_Click(object sender, EventArgs e)
        {
            score = 100;
            for (int i = 0; i < 16; i++)
            {
                Button btn = (Button)Controls.Find("btn_P" + i, false)[0];
                btn.FlatAppearance.BorderColor = Color.Black;
            }

            isFirstTime = true;
            path = FilePath();

            // Create a new Bitmap object from the picture file on disk,
            // and assign that to the PictureBox.Image property
            var bitmap = new Bitmap(path);


            Image img = bitmap; // a.png has 312X312 width and height

            int width = img.Width / 4;
            int height = img.Height / 4;
            

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bmps[i, j] = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(bmps[i, j]);
                    g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(j * width, i * height, width, height), GraphicsUnit.Pixel);
                    g.Dispose();
                    //Console.WriteLine("i =  " + i + "  j =  " + j + "  " + bmps[i, j].Width + "  -  " + bmps[i, j].Height);
                }
            }
        }

        private void btn_Mix_Click(object sender, EventArgs e)
        {        
            Mix(bmps);
        }

        private void btnClick(object sender, EventArgs e)
        {

            Button button = (Button)sender;
            if(buttonId == -1){
                buttonId = Int32.Parse(button.Name.Substring(5, button.Name.Length - 5));
                //Controls.Find("btn_P"+)
            }
            else if(buttonId != Int32.Parse(button.Name.Substring(5, button.Name.Length - 5))){

                int secondId = Int32.Parse(button.Name.Substring(5, button.Name.Length - 5));

                Button first = (Button) Controls.Find("btn_P" + buttonId, false)[0];
                Button second = (Button)Controls.Find("btn_P" + secondId, false)[0];


                Image tmp = first.BackgroundImage;

                first.BackgroundImage = second.BackgroundImage;
                second.BackgroundImage = tmp;

                ImageConverter converter = new ImageConverter();
                byte[] firstData = converter.ConvertTo(first.BackgroundImage, typeof(byte[])) as byte[];
                byte[] secondData = converter.ConvertTo(second.BackgroundImage, typeof(byte[])) as byte[];

                var firstHash = ImageHashing(firstData);
                var secondHash = ImageHashing(secondData);


                if (firstHash.SequenceEqual(((Hashing)first.Tag).Original))
                {
                    first.FlatAppearance.BorderColor = Color.Green;
                    score -= 2;
                }
                else
                {
                    first.FlatAppearance.BorderColor = Color.Red;
                    score -= 2;
                }

                if (secondHash.SequenceEqual(((Hashing)second.Tag).Original))
                {
                    second.FlatAppearance.BorderColor = Color.Green;
                    score -= 2;
                }
                else
                {
                    second.FlatAppearance.BorderColor = Color.Red;
                    score -= 2;
                }

                buttonId = -1;

                isFinitto();

            }

        }

        public void isFinitto()
        {
            int i = 0;
            for (i=0; i < 16; i++)
            {
                Button btn = (Button)Controls.Find("btn_P" + i, false)[0];
                
                if(btn.FlatAppearance.BorderColor != Color.Green)
                {
                    break;
                }
            }

            if (i == 16)
            {
                MessageBox.Show("Winner winner chicken dinner! Score: " + score);
                FileWrite();
                if (score > maxScore)
                    maxLabel.Text = score.ToString();

            }


            
        }
    }

}
