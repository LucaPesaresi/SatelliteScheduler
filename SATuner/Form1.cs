using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SATuner
{
    public partial class Form1 : Form
    {
        private string ars, dtos, consts;

        public Form1()
        {
            InitializeComponent();
        }

        private void rbRR_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRR.Checked)
            {
                k_start.Enabled = true;
                noise_start.Enabled = true;

                tf_start.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            GetInput(tbPath.Text);

            int seed = Convert.ToInt32(seed_start.Text);

            if (cbAuto.Checked)
            {
                int k_e = Convert.ToInt32(k_end.Text);
                int noise_e = Convert.ToInt32(noise_end.Text);
            }

            int k_s = Convert.ToInt32(k_start.Text);
            int noise_s = Convert.ToInt32(noise_start.Text);
            int nit = 100;

            Instance instance = new Instance(ars, dtos, consts, seed);
            Plan plan_noisyrankmem = Euristics.CreateInitialPlan(instance, noise_s, nit);
           
            Tuner T = new Tuner(instance, plan_noisyrankmem, seed);

            if (rbRR.Checked)
            {
                if (cbAuto.Checked)
                {
                    int k_e = Convert.ToInt32(k_end.Text);
                    int k_i = Convert.ToInt32(k_step.Text);
                    int noise_e = Convert.ToInt32(noise_end.Text);
                    int noise_i = Convert.ToInt32(noise_step.Text);
                    T.BuildTuningRR(k_s, k_e, k_i, noise_s, noise_e, noise_i);
                }
                else
                {
                    T.RRSingle(k_s, noise_s);
                }
            }
            if (rbSA.Checked)
            {
                T.TuningSA();
            }

            dgvOutput.DataSource = T.GetList();
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAuto.Checked)
            {
                k_end.Enabled = true;
                noise_end.Enabled = true;
                tf_end.Enabled = true;

                k_step.Enabled = true;
                noise_step.Enabled = true;
                tf_step.Enabled = true;
            }
            else
            {
                k_end.Enabled = false;
                noise_end.Enabled = false;
                tf_end.Enabled = false;

                k_step.Enabled = false;
                noise_step.Enabled = false;
                tf_step.Enabled = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvOutput.DataSource = null;
        }

        private void rbSA_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSA.Checked)
            {
                tf_start.Enabled = true;
                k_start.Enabled = false;
                noise_start.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (fbd)
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if (Directory.GetFiles(fbd.SelectedPath).Length > 3)
                    {

                        GetInput(fbd.SelectedPath);
                    }
                }
            }
        }

        private void GetInput(string path)
        {
            ars = File.ReadAllText(path + @"\ARs.json");
            dtos = File.ReadAllText(path + @"\DTOs.json");
            consts = File.ReadAllText(path + @"\constants.json");
            tbPath.Text = fbd.SelectedPath;

            k_start.Enabled = true;
            noise_start.Enabled = true;
            tf_start.Enabled = true;

            btnTest.Enabled = true;
        }
    }
}
