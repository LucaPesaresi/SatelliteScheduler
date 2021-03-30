namespace SATuner
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.k_start = new System.Windows.Forms.TextBox();
            this.noise_start = new System.Windows.Forms.TextBox();
            this.tf_start = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.rbRR = new System.Windows.Forms.RadioButton();
            this.rbSA = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.k_end = new System.Windows.Forms.TextBox();
            this.noise_end = new System.Windows.Forms.TextBox();
            this.tf_end = new System.Windows.Forms.TextBox();
            this.seed_start = new System.Windows.Forms.TextBox();
            this.dgvOutput = new System.Windows.Forms.DataGridView();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.tf_step = new System.Windows.Forms.TextBox();
            this.noise_step = new System.Windows.Forms.TextBox();
            this.k_step = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbAuto = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(97, 30);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(196, 20);
            this.tbPath.TabIndex = 0;
            this.tbPath.Text = "C:\\Users\\luca2\\Desktop\\Tesi\\LAB\\day1_0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(50, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(50, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Seed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(93, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "K-ruin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(164, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Noise";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(256, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "Temp Factor";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(309, 28);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(58, 20);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Sfoglia";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // k_start
            // 
            this.k_start.Enabled = false;
            this.k_start.Location = new System.Drawing.Point(97, 184);
            this.k_start.Name = "k_start";
            this.k_start.Size = new System.Drawing.Size(45, 20);
            this.k_start.TabIndex = 7;
            this.k_start.Text = "10";
            // 
            // noise_start
            // 
            this.noise_start.Enabled = false;
            this.noise_start.Location = new System.Drawing.Point(168, 184);
            this.noise_start.Name = "noise_start";
            this.noise_start.Size = new System.Drawing.Size(45, 20);
            this.noise_start.TabIndex = 8;
            this.noise_start.Text = "10";
            // 
            // tf_start
            // 
            this.tf_start.Enabled = false;
            this.tf_start.Location = new System.Drawing.Point(260, 184);
            this.tf_start.Name = "tf_start";
            this.tf_start.Size = new System.Drawing.Size(95, 20);
            this.tf_start.TabIndex = 9;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(49, 476);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 40);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Cancella";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(267, 476);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 40);
            this.btnTest.TabIndex = 15;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(75, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 13);
            this.label7.TabIndex = 16;
            // 
            // rbRR
            // 
            this.rbRR.AutoSize = true;
            this.rbRR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbRR.Location = new System.Drawing.Point(53, 113);
            this.rbRR.Name = "rbRR";
            this.rbRR.Size = new System.Drawing.Size(138, 20);
            this.rbRR.TabIndex = 17;
            this.rbRR.TabStop = true;
            this.rbRR.Text = "Ruin and Recreate";
            this.rbRR.UseVisualStyleBackColor = true;
            this.rbRR.CheckedChanged += new System.EventHandler(this.rbRR_CheckedChanged);
            // 
            // rbSA
            // 
            this.rbSA.AutoSize = true;
            this.rbSA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSA.Location = new System.Drawing.Point(218, 113);
            this.rbSA.Name = "rbSA";
            this.rbSA.Size = new System.Drawing.Size(149, 20);
            this.rbSA.TabIndex = 18;
            this.rbSA.TabStop = true;
            this.rbSA.Text = "Simulated Annealing";
            this.rbSA.UseVisualStyleBackColor = true;
            this.rbSA.CheckedChanged += new System.EventHandler(this.rbSA_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(46, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Inizio";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(46, 227);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 16);
            this.label9.TabIndex = 20;
            this.label9.Text = "Fine";
            // 
            // k_end
            // 
            this.k_end.Enabled = false;
            this.k_end.Location = new System.Drawing.Point(97, 226);
            this.k_end.Name = "k_end";
            this.k_end.Size = new System.Drawing.Size(45, 20);
            this.k_end.TabIndex = 21;
            // 
            // noise_end
            // 
            this.noise_end.Enabled = false;
            this.noise_end.Location = new System.Drawing.Point(168, 226);
            this.noise_end.Name = "noise_end";
            this.noise_end.Size = new System.Drawing.Size(45, 20);
            this.noise_end.TabIndex = 22;
            // 
            // tf_end
            // 
            this.tf_end.Enabled = false;
            this.tf_end.Location = new System.Drawing.Point(260, 226);
            this.tf_end.Name = "tf_end";
            this.tf_end.Size = new System.Drawing.Size(95, 20);
            this.tf_end.TabIndex = 26;
            // 
            // seed_start
            // 
            this.seed_start.Location = new System.Drawing.Point(97, 64);
            this.seed_start.Name = "seed_start";
            this.seed_start.Size = new System.Drawing.Size(84, 20);
            this.seed_start.TabIndex = 28;
            this.seed_start.Text = "0";
            // 
            // dgvOutput
            // 
            this.dgvOutput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvOutput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutput.Location = new System.Drawing.Point(49, 316);
            this.dgvOutput.Name = "dgvOutput";
            this.dgvOutput.Size = new System.Drawing.Size(318, 138);
            this.dgvOutput.TabIndex = 30;
            // 
            // tf_step
            // 
            this.tf_step.Enabled = false;
            this.tf_step.Location = new System.Drawing.Point(260, 265);
            this.tf_step.Name = "tf_step";
            this.tf_step.Size = new System.Drawing.Size(95, 20);
            this.tf_step.TabIndex = 34;
            // 
            // noise_step
            // 
            this.noise_step.Enabled = false;
            this.noise_step.Location = new System.Drawing.Point(168, 265);
            this.noise_step.Name = "noise_step";
            this.noise_step.Size = new System.Drawing.Size(45, 20);
            this.noise_step.TabIndex = 33;
            // 
            // k_step
            // 
            this.k_step.Enabled = false;
            this.k_step.Location = new System.Drawing.Point(97, 265);
            this.k_step.Name = "k_step";
            this.k_step.Size = new System.Drawing.Size(45, 20);
            this.k_step.TabIndex = 32;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(46, 266);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 16);
            this.label10.TabIndex = 31;
            this.label10.Text = "Step";
            // 
            // cbAuto
            // 
            this.cbAuto.AutoSize = true;
            this.cbAuto.Location = new System.Drawing.Point(264, 66);
            this.cbAuto.Name = "cbAuto";
            this.cbAuto.Size = new System.Drawing.Size(47, 17);
            this.cbAuto.TabIndex = 35;
            this.cbAuto.Text = "auto";
            this.cbAuto.UseVisualStyleBackColor = true;
            this.cbAuto.CheckedChanged += new System.EventHandler(this.cbAuto_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(215, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 16);
            this.label6.TabIndex = 36;
            this.label6.Text = "Mode:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 561);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbAuto);
            this.Controls.Add(this.tf_step);
            this.Controls.Add(this.noise_step);
            this.Controls.Add(this.k_step);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.dgvOutput);
            this.Controls.Add(this.seed_start);
            this.Controls.Add(this.tf_end);
            this.Controls.Add(this.noise_end);
            this.Controls.Add(this.k_end);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rbSA);
            this.Controls.Add(this.rbRR);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tf_start);
            this.Controls.Add(this.noise_start);
            this.Controls.Add(this.k_start);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPath);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox k_start;
        private System.Windows.Forms.TextBox noise_start;
        private System.Windows.Forms.TextBox tf_start;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbRR;
        private System.Windows.Forms.RadioButton rbSA;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox k_end;
        private System.Windows.Forms.TextBox noise_end;
        private System.Windows.Forms.TextBox tf_end;
        private System.Windows.Forms.TextBox seed_start;
        private System.Windows.Forms.DataGridView dgvOutput;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.TextBox tf_step;
        private System.Windows.Forms.TextBox noise_step;
        private System.Windows.Forms.TextBox k_step;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbAuto;
        private System.Windows.Forms.Label label6;
    }
}

