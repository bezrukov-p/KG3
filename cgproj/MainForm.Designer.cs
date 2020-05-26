namespace cgproj
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ConsolePanel = new System.Windows.Forms.Panel();
            this.ConsoleText = new System.Windows.Forms.TextBox();
            this.ConsoleView = new System.Windows.Forms.Label();
            this.ConsolePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConsolePanel
            // 
            this.ConsolePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConsolePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ConsolePanel.Controls.Add(this.ConsoleView);
            this.ConsolePanel.Controls.Add(this.ConsoleText);
            this.ConsolePanel.Location = new System.Drawing.Point(134, 0);
            this.ConsolePanel.Name = "ConsolePanel";
            this.ConsolePanel.Size = new System.Drawing.Size(367, 450);
            this.ConsolePanel.TabIndex = 1;
            // 
            // ConsoleText
            // 
            this.ConsoleText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConsoleText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ConsoleText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConsoleText.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ConsoleText.Location = new System.Drawing.Point(12, 425);
            this.ConsoleText.Name = "ConsoleText";
            this.ConsoleText.Size = new System.Drawing.Size(343, 13);
            this.ConsoleText.TabIndex = 0;
            // 
            // ConsoleView
            // 
            this.ConsoleView.ForeColor = System.Drawing.SystemColors.Control;
            this.ConsoleView.Location = new System.Drawing.Point(9, 9);
            this.ConsoleView.Name = "ConsoleView";
            this.ConsoleView.Size = new System.Drawing.Size(346, 405);
            this.ConsoleView.TabIndex = 1;
            this.ConsoleView.Text = "Консоль:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 450);
            this.Controls.Add(this.ConsolePanel);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "3dmax";
            this.ConsolePanel.ResumeLayout(false);
            this.ConsolePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel ConsolePanel;
        private System.Windows.Forms.TextBox ConsoleText;
        private System.Windows.Forms.Label ConsoleView;
    }
}

