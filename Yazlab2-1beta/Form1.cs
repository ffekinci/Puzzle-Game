using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        Dictionary<int, byte[]> shuffleHash;
        int buttonId = -1;

        public Form1()
        {
            InitializeComponent();
            
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
            shuffleHash = new Dictionary<int, byte[]>();
            var random = Shuffle();
            int a = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Button foobar = (Button)Controls.Find("btn_P" + random[a], false)[0];


                    foobar.BackgroundImage = bmps[i, k];

                    ImageConverter converter = new ImageConverter();
                    byte[] rawImageData = converter.ConvertTo(foobar.BackgroundImage, typeof(byte[])) as byte[];
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] hash = md5.ComputeHash(rawImageData);

                    shuffleHash.Add(random[a], hash);
                    
                    Dictionary<int, byte[]>.ValueCollection DegerListesi = shuffleHash.Values;


                    //foreach (var item in DegerListesi)
                    //{
                    //    foreach (var elem in item)
                    //    {
                    //        Console.Write(elem);
                    //    }
                    //    Console.WriteLine("");
                    //}

                    //for (int o = 0; o < hash.Length; o++)
                    //{
                    //    Console.Write(hash[o]);
                    //}
                    //Console.WriteLine(" - " + random[a] + "  ");

                    foobar.BackgroundImageLayout = ImageLayout.Stretch;
                    a++;
                }
            }
        }

        public string FilePath()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.FileName;
                }

                else
                {
                    ///todo yaparsın sen aslan
                    return null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            path = FilePath();
            // Create a new Bitmap object from the picture file on disk,
            // and assign that to the PictureBox.Image property
            

        }

        private void btn_Mix_Click(object sender, EventArgs e)
        {

            var bitmap = new Bitmap(path);


            Image img = bitmap; // a.png has 312X312 width and height

            int width = img.Width / 4;
            int height = img.Height / 4;
            Bitmap[,] bmps = new Bitmap[4, 4];

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

            int a = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Button foobar = (Button)Controls.Find("btn_P" + a, false)[0];
                    foobar.BackgroundImage = bmps[i, k];
                    foobar.BackgroundImageLayout = ImageLayout.Stretch;
                    a++;
                }
            }

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

                byte[] firstHash;
                byte[] secondHash;

                shuffleHash.TryGetValue(buttonId, out firstHash);
                shuffleHash.TryGetValue(secondId, out secondHash);

                shuffleHash[buttonId] = secondHash;
                shuffleHash[secondId] = firstHash;

                buttonId = -1;

            }

        }
    }

}
