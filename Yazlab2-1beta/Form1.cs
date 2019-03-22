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
        public enum State { Enable, Disable };
        FileOp op = new FileOp("enyuksekskor.txt");
        Bitmap[,] bmps = new Bitmap[4, 4];
        int buttonId = -1;
        int count = 0;
        int score = 100;
        int maxScore = 0;
        int minMove = 0;

        public Form1()
        {
            InitializeComponent();

            btn_Mix.Enabled = false;
            ButtonState(State.Disable);

            // read scores
            op.Read(ref maxScore);
            maxLabel.Text = maxScore.ToString();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        public int shortestPath(int[] arr)
        {
            int movePoint = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[arr[i]] == i)
                {
                    int tmp = arr[i];
                    arr[i] = arr[arr[i]];
                    arr[arr[i]] = tmp;

                    movePoint++;
                }
            }

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != i)
                {
                    for (int j = 0; j < arr.Length; j++)
                    {
                        if (arr[j] == i)
                        {
                            int tmp = arr[j];
                            arr[j] = arr[i];
                            arr[i] = tmp;

                            movePoint++;
                        }
                    }

                }
            }
            return movePoint;
        }

        public void ButtonState(State status)
        {
            for (int i = 0; i < 16; i++)
            {
                string key = "btn_P" + i;
                Controls[key].Enabled = (status == State.Enable) ? true : false;
            }
        }

        public void ButtonState(int index, State status)
        {
            string key = "btn_P" + index;
            Controls[key].Enabled = (status == State.Enable) ? true : false;
        }

        public int[] Shuffle()
        {
            Random rand = new Random();
            var randNums = new int[16];
            for (int i = 0; i < 16; i++)
            {
                randNums[i] = i;
            }

            for (int i = 15; i >= 0; i--)
            {
                int num = rand.Next(9);
                int tmp = randNums[i];
                randNums[i] = randNums[num];
                randNums[num] = tmp;
            }

            return randNums;
        }

        public void Mix(Bitmap[,] bmps)
        {
            var random = Shuffle();
            minMove = shortestPath(random.Clone() as int[]);
            int a = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Button foobar = Controls["btn_P" + a] as Button;
                    ImageConverter converter = new ImageConverter();
                    // tagging button with hash bytes
                    foobar.Tag = ImageHashing(bmps[i, k]);
                    Button foobar2 = (Button)Controls.Find("btn_P" + random[a], false)[0];
                    foobar2.BackgroundImage = bmps[i, k];
                    foobar2.BackgroundImageLayout = ImageLayout.Stretch;
                    a++;

                }
            }


            ButtonControl();

            if (isFinitto())
                MessageBox.Show("Score: " + score);


        }

        public void ButtonControl()
        {
            bool isOK = false;

            for (int i = 0; i < 16; i++)
            {
                Button foobar = (Button)Controls.Find("btn_P" + i, false)[0];

                ImageConverter converter = new ImageConverter();

                var newHash = ImageHashing(foobar.BackgroundImage);

                if (newHash.SequenceEqual(foobar.Tag as byte[]))
                {
                    isOK = true;
                    foobar.FlatAppearance.BorderColor = Color.Green;
                    count++;
                }
            }

            if (isOK)
            {
                ButtonState(State.Enable);
                btn_Mix.Enabled = false;
            }
        }


        //hash the image but better.
        //and it's accepts Image instead a Bitmap beacuse Bitmap extends Image class so it can accept BMP and Button Image without any cast operation
        public byte[] ImageHashing(Image Image)
        {
            // convert image to rawData as bytes
            var rawImgData = new ImageConverter().ConvertTo(Image, typeof(byte[])) as byte[];
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            //then compute hash and return
            return md5.ComputeHash(rawImgData);
        }

        //return selected file's path
        public string FilePath()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;";

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
                Button btn = Controls["btn_P" + i] as Button;
                btn.FlatAppearance.BorderColor = Color.Black;
            }
            

            // dosya secilmezse null doner!!
            string path = FilePath();

            // path null ise islem yapma cunku bitmap null path ile initialize olmaz

            if (path is null)
                return;

            var bitmap = new Bitmap(path);


            Image img = bitmap;

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
            Button button = sender as Button;
            if (!button.Enabled)
            {
                return;
            }

            if (buttonId == -1)
            {
                buttonId = int.Parse(button.Name.Substring(5, button.Name.Length - 5));
                //Controls.Find("btn_P"+)
            }
            else if (buttonId != int.Parse(button.Name.Substring(5, button.Name.Length - 5)))
            {

                int secondId = int.Parse(button.Name.Substring(5, button.Name.Length - 5));
                Button first = Controls["btn_P" + buttonId] as Button;
                Button second = Controls["btn_P" + secondId] as Button;


                Image tmp = first.BackgroundImage;

                first.BackgroundImage = second.BackgroundImage;
                second.BackgroundImage = tmp;

                // buralarıda degistirdim cunku mantiklii bu resmi gonderdim kendi halletti
                var firstHash = ImageHashing(first.BackgroundImage);
                var secondHash = ImageHashing(second.BackgroundImage);

                /*
                 * diyorum ki reyiz bi hamle yaptık biri dogru yerdeyse puan kırmayak. 
                 */
                if (firstHash.SequenceEqual(first.Tag as byte[]))
                {
                    first.FlatAppearance.BorderColor = Color.Green;
                    ButtonState(buttonId, State.Disable);
                    score -= 2;
                }

                else
                {
                    first.FlatAppearance.BorderColor = Color.Red;
                    ButtonState(buttonId, State.Enable);
                    score -= 2;
                }

                if (secondHash.SequenceEqual(second.Tag as byte[]))
                {
                    second.FlatAppearance.BorderColor = Color.Green;
                    ButtonState(secondId, State.Disable);
                    score -= 2;
                }

                else
                {
                    second.FlatAppearance.BorderColor = Color.Red;
                    ButtonState(secondId, State.Enable);
                    score -= 2;
                }

                buttonId = -1;

                if (isFinitto())
                    MessageBox.Show("Score: " + score);

            }

        }

        public bool isFinitto()
        {
            int i = 0;
            /* 
             * 16 tane yeşil dışı rengi olan buton yoksa oyun bitmemiştir.
             * çünkü doğru resmin olduğu butunun sınır rengi yeşil yapıldı.
             */
            for (i = 0; i < 16; i++)
            {
                Button btn = (Button)Controls.Find("btn_P" + i, false)[0];

                if (btn.FlatAppearance.BorderColor != Color.Green)
                {
                    break;
                }
            }

            if (i == 16)
            {
                if (score < 0)
                    score = 0;
                if (score > 100)
                    score = 100;
                op.Write(score);
                return true;
            }


            return false;
        }
    }

}
