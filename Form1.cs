using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace alc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        List<List<string>> gor = new List<List<string>>(); //gor[0][0] - название видео карты; gor[0][1] - ее номер gpu; все остальное - значение.


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                backgroundWorker1.RunWorkerAsync(openFileDialog1.FileName) ;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //SHDocVw.InternetExplorer IE;
            //IE = new SHDocVw.InternetExplorer();
            //IE.Navigate("http://nnmgp.ru/");
            //IE.Visible = true;
            System.Diagnostics.Process.Start("http://nnmgp.ru/");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            this.Invoke(new MethodInvoker(() => button1.Enabled = false));
            this.Invoke(new MethodInvoker(() => textBox1.Text = "Не нерничай, считаем, смотри на прогресс бар ↓↓↓"));
            string put = (string)e.Argument;
            {
                int t = 0;
                ArrayList al = new ArrayList();
                al.AddRange(System.IO.File.ReadAllLines(put));
                this.Invoke(new MethodInvoker(() => progressBar1.Maximum = al.Count));
                for (int q = 0; q < al.Count; q++)
                {
                    this.Invoke(new MethodInvoker(() => progressBar1.Value = q));

                    if (al[q].ToString().Contains("Cards available"))
                    {

                        for (int r = 1; ; r++)
                        {
                            if (al[q + r].ToString().Contains("Total cards:"))
                            {
                                // MessageBox.Show("fuf");
                                q = q + r;
                                break;
                            }
                            else
                            {
                                if (al[q + r].ToString().Contains("GPU #" + t + ":"))
                                {
                                    //al[q+1].ToString().
                                    int a = al[q + r].ToString().IndexOf("GPU");
                                    int b = al[q + r].ToString().IndexOf(",");
                                    //textBox1.Text = al[q + 1].ToString().Substring(a+8, b-a-8);
                                    gor.Add(new List<string>());
                                    gor[t].Add(al[q + r].ToString().Substring(a + 8, b - a - 8));   //Название видео карты
                                    gor[t].Add(al[q + r].ToString().Substring(a, 6));   //номер gpu
                                    gor[t].Add("");   //Средняя скорость GPU
                                    gor[t].Add("0");   //колличество найденых шаров
                                                       // textBox1.Text = textBox1.Text + "\r\n" + gor[t][0];
                                                       //textBox1.Text = textBox1.Text + "\r\n" + gor[t][1];
                                    t++;
                                    //MessageBox.Show(t.ToString());
                                }
                            }
                        }
                    }
                    //собрали видео карты, сортируем  температуры...
                    if (al[q].ToString().Contains("GPU0 t="))
                    {
                        if (t == 1)
                        {
                            int a = al[q].ToString().IndexOf("GPU0 t=");
                            gor[0].Add(al[q].ToString().Substring(a + 7, 2));
                            //textBox1.Text = textBox1.Text + "\r\n" + al[q].ToString().Substring(a + 7, 2);
                        }
                        else
                        {
                            string[] st = al[q].ToString().Split(',');
                            for (int r = 0; r < st.Length; r++)
                            {
                                //MessageBox.Show(st[r]);
                                int a = st[r].ToString().IndexOf("GPU" + r + " t=");
                                gor[r].Add(st[r].ToString().Substring(a + 7, 2));
                                //MessageBox.Show(r+" "+st[r].ToString().Substring(a + 7, 2));
                                //textBox1.Text = textBox1.Text + "\r\n" + r + " " + st[r].ToString().Substring(a + 7, 2);


                            }
                        }
                    }
                    if (al[q].ToString().Contains("Total Speed:"))
                    {
                        if (t == 1)
                        {
                            int a = al[q + 1].ToString().IndexOf("GPU0");
                            gor[0][2] = gor[0][2] + "|" + al[q + 1].ToString().Substring(a + 5, 6);
                        }
                        else
                        {
                            string[] st = al[q + 1].ToString().Split(',');
                            for (int r = 0; r < st.Length; r++)
                            {
                                int a = st[r].ToString().IndexOf("GPU" + r);
                                gor[r][2] = gor[r][2] + "|" + st[r].Substring(a + 5, 6);
                            }
                        }
                    }
                    if (al[q].ToString().Contains("SHARE FOUND"))
                    {
                        if (t == 1)
                        {
                            gor[0][3] = (Convert.ToInt16(gor[0][3]) + 1).ToString();
                        }
                        else
                        {
                            int a = al[q].ToString().IndexOf("GPU");
                            int m = Convert.ToInt16(al[q].ToString().Substring(a + 4, 2).Trim(')'));
                            gor[m][3] = (Convert.ToInt16(gor[m][3]) + 1).ToString();
                            //MessageBox.Show(gor[m][3].ToString());
                        }
                    }
                }
                //MessageBox.Show("wer");
                for (int y = 0; y < gor.Count; y++)
                {
                    if (y == 0)
                    {
                        this.Invoke(new MethodInvoker(() => textBox1.Text = gor[y][0].ToString() + " (" + gor[y][1].ToString() + ")")); }
                    else
                    { this.Invoke(new MethodInvoker(() => textBox1.Text = textBox1.Text + "\r\n" + gor[y][0].ToString() + " (" + gor[y][1].ToString() + ")")); }
                    //считаем среднюю скорость
                    string[] speed = gor[y][2].ToString().Split('|');
                    double Sspeed = 0;
                    for (int p = 1; p < speed.Length; p++)
                    {
                        //MessageBox.Show(speed[p]);
                        try
                        {
                            Sspeed = Sspeed + Convert.ToDouble(speed[p].Replace('.', ','));
                        }
                        catch
                        {
                            //MessageBox.Show(speed[p]); 
                        }
                    }
                    Sspeed = Math.Round(Sspeed / Convert.ToDouble(speed.Length), 3);
                    //
                    this.Invoke(new MethodInvoker(() => textBox1.Text = textBox1.Text + "\r\n" + "Средняя скорость = " + Sspeed.ToString()));
                    //textBox1.Text = textBox1.Text + "\r\n" + "Среднее кол-во шаров = " + Math.Round(Convert.ToDouble(gor[y][3])/ Convert.ToDouble(gor[y].Count - 2) / 2880, 3);
                    this.Invoke(new MethodInvoker(() => textBox1.Text = textBox1.Text + "\r\n" + "Среднее кол-во шаров = " + Math.Round((Convert.ToDouble(gor[y][3]) / (Convert.ToDouble(gor[y].Count - 2) / 2880)), 3).ToString()));
                    //MessageBox.Show("1");
                    //textBox1.Text = textBox1.Text + "\r\n" + gor[i][1].ToString();
                    gor[y].RemoveRange(0, 4);
                    gor[y].Sort();
                    double a = 1;
                    //textBox1.Text = textBox1.Text + "\r\n" + gor[i][0].ToString();
                    for (int w = 1; w < gor[y].Count - 1; w++)
                    {

                        //MessageBox.Show(h.ToString());

                        //MessageBox.Show(gor[i][w - 1].ToString() + " == " + gor[i][w].ToString());
                        if (gor[y][w].ToString() == gor[y][w - 1].ToString())
                        {
                            a++;
                        }
                        else
                        {
                            //textBox1.Text = textBox1.Text + "\t" + a.ToString() + "\r\n" + gor[i][w].ToString();
                            //textBox1.Text = textBox1.Text + "\r\n" + gor[i][w].ToString();
                            double h = Convert.ToDouble(gor[y].Count - 2) / 2880;
                            //MessageBox.Show(h.ToString());
                            this.Invoke(new MethodInvoker(() => textBox1.Text = textBox1.Text + "\r\n" + gor[y][w - 1].ToString() + "\t" + Math.Round((a * 30 / 3600) / h, 3).ToString()));
                            a = 1;
                        }


                    }
                }
                this.Invoke(new MethodInvoker(() => textBox1.SelectionStart = textBox1.TextLength));
                this.Invoke(new MethodInvoker(() => textBox1.ScrollToCaret()));
                this.Invoke(new MethodInvoker(() => textBox1.Enabled = true));
            }
            
        }
    }
}
