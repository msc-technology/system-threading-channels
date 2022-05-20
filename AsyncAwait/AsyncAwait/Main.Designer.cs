namespace AsyncAwait
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainPanel = new System.Windows.Forms.Panel();
            this.MainTextArea = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.AsyncCall = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Elapsed = new System.Windows.Forms.TextBox();
            this.ParallelNotAsync = new System.Windows.Forms.Button();
            this.MainPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.MainTextArea);
            this.MainPanel.Location = new System.Drawing.Point(12, 12);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(397, 961);
            this.MainPanel.TabIndex = 0;
            // 
            // MainTextArea
            // 
            this.MainTextArea.Location = new System.Drawing.Point(3, 3);
            this.MainTextArea.Multiline = true;
            this.MainTextArea.Name = "MainTextArea";
            this.MainTextArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.MainTextArea.Size = new System.Drawing.Size(391, 418);
            this.MainTextArea.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(415, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Not Really Async";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.FakeAsync_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(544, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "What time is it now?";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.TimeDialog_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(544, 42);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(123, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Clear Panel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ClearPanel_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(415, 70);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(123, 54);
            this.button4.TabIndex = 4;
            this.button4.Text = "Parallel, seems async but it is not";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ParallelFakeAsync_Click);
            // 
            // AsyncCall
            // 
            this.AsyncCall.Location = new System.Drawing.Point(415, 130);
            this.AsyncCall.Name = "AsyncCall";
            this.AsyncCall.Size = new System.Drawing.Size(123, 23);
            this.AsyncCall.TabIndex = 5;
            this.AsyncCall.Text = "Let\'s go async";
            this.AsyncCall.UseVisualStyleBackColor = true;
            this.AsyncCall.Click += new System.EventHandler(this.AsyncCall_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Elapsed);
            this.panel1.Location = new System.Drawing.Point(894, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 447);
            this.panel1.TabIndex = 6;
            // 
            // Elapsed
            // 
            this.Elapsed.Location = new System.Drawing.Point(3, 3);
            this.Elapsed.Multiline = true;
            this.Elapsed.Name = "Elapsed";
            this.Elapsed.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Elapsed.Size = new System.Drawing.Size(352, 441);
            this.Elapsed.TabIndex = 1;
            // 
            // ParallelNotAsync
            // 
            this.ParallelNotAsync.Location = new System.Drawing.Point(415, 42);
            this.ParallelNotAsync.Name = "ParallelNotAsync";
            this.ParallelNotAsync.Size = new System.Drawing.Size(123, 23);
            this.ParallelNotAsync.TabIndex = 7;
            this.ParallelNotAsync.Text = "Parallel but not async";
            this.ParallelNotAsync.UseVisualStyleBackColor = true;
            this.ParallelNotAsync.Click += new System.EventHandler(this.ParallelNotAsync_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 985);
            this.Controls.Add(this.ParallelNotAsync);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.AsyncCall);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MainPanel);
            this.Name = "Main";
            this.Text = "Welcome!";
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox MainTextArea;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button AsyncCall;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox Elapsed;
        private System.Windows.Forms.Button ParallelNotAsync;
    }
}

