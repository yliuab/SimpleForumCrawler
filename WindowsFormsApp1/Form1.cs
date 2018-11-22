using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsApp1.Logic;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        CentralRepository _centralRepository;
        UtilityRepository _utilityRepository;
        int timePassed = 0;

        public Form1()
        {
            InitializeComponent();
            _centralRepository = new CentralRepository();
            _utilityRepository = new UtilityRepository();
            comboBox1.SelectedIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "00:00:00";
            label2.Text = "0%";
            timePassed = 0;
            progressBar1.Value = 0;
            timer1.Start();
            int selected = comboBox1.SelectedIndex;
            string url = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                _centralRepository.DataFilePath = textBox2.Text;
            }
            new Thread((ThreadStart)delegate ()
            {
                _centralRepository.StartCrawlSelectedForum(url);
            }).Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _centralRepository.AnalyzeSelectedForum();
            MessageBox.Show("Completed");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int progress = _centralRepository.Progress;
            timePassed++;
            label1.Text = _utilityRepository.GetFormatTimeString(timePassed);
            label2.Text = progress + "%";
            progressBar1.Value = progress;
            if (progress >= 100)
            {
                timer1.Stop();
                MessageBox.Show("Crawl data completed, saved in: " + _centralRepository.DataFilePath);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:// 9 Game
                    _centralRepository.SelectedForum = Forum.Forum9Game;
                    break;
                case 1:// 163
                    _centralRepository.SelectedForum = Forum.Forum163;
                    break;
                case 2:// Baidu
                    _centralRepository.SelectedForum = Forum.BaiduTieba;
                    break;
                case 3:// NGA
                    _centralRepository.SelectedForum = Forum.NGA;
                    break;
                default:
                    break;
            }
            textBox2.Text = _centralRepository.DataFilePath;
        }
    }
}
