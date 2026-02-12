namespace MMLauncher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panelTop = new Panel();
            vanillaButton = new Button();
            label1 = new Label();
            refreshButton = new Button();
            discordButton = new Button();
            infoButton = new Button();
            versionLabel = new Label();
            addmodsButton = new Button();
            launchButton = new Button();
            setupButton = new Button();
            pictureBox1 = new PictureBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            deleteModToolStripMenuItem = new ToolStripMenuItem();
            openModFolderToolStripMenuItem = new ToolStripMenuItem();
            outputRichTextBox = new RichTextBox();
            modImageBox = new PictureBox();
            descriptionRichTextBox = new RichTextBox();
            modListBox = new CheckedListBox();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)modImageBox).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(32, 32, 32);
            panelTop.Controls.Add(vanillaButton);
            panelTop.Controls.Add(label1);
            panelTop.Controls.Add(refreshButton);
            panelTop.Controls.Add(discordButton);
            panelTop.Controls.Add(infoButton);
            panelTop.Controls.Add(versionLabel);
            panelTop.Controls.Add(launchButton);
            panelTop.Controls.Add(addmodsButton);
            panelTop.Controls.Add(setupButton);
            panelTop.Controls.Add(pictureBox1);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1224, 54);
            panelTop.TabIndex = 0;
            // 
            // vanillaButton
            // 
            vanillaButton.BackColor = Color.FromArgb(48, 48, 48);
            vanillaButton.FlatStyle = FlatStyle.Popup;
            vanillaButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            vanillaButton.ForeColor = SystemColors.ControlLight;
            vanillaButton.Location = new Point(135, 4);
            vanillaButton.Name = "vanillaButton";
            vanillaButton.Size = new Size(130, 45);
            vanillaButton.TabIndex = 7;
            vanillaButton.Text = "Launch Vanilla";
            vanillaButton.UseVisualStyleBackColor = false;
            vanillaButton.Click += vanillaButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 4F);
            label1.ForeColor = Color.FromArgb(28, 28, 28);
            label1.Location = new Point(403, 23);
            label1.Name = "label1";
            label1.Size = new Size(60, 16);
            label1.TabIndex = 7;
            label1.Text = "Bring Back \r\nBefore Style Shigaraki";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.FromArgb(48, 48, 48);
            refreshButton.FlatStyle = FlatStyle.Popup;
            refreshButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            refreshButton.ForeColor = SystemColors.ControlLight;
            refreshButton.Location = new Point(605, 4);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(130, 45);
            refreshButton.TabIndex = 6;
            refreshButton.Text = "Refresh List";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += refreshButton_Click;
            // 
            // discordButton
            // 
            discordButton.BackColor = Color.FromArgb(48, 48, 48);
            discordButton.BackgroundImageLayout = ImageLayout.None;
            discordButton.FlatStyle = FlatStyle.Popup;
            discordButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            discordButton.ForeColor = SystemColors.ControlLight;
            discordButton.Location = new Point(741, 4);
            discordButton.Name = "discordButton";
            discordButton.Size = new Size(159, 45);
            discordButton.TabIndex = 5;
            discordButton.Text = "Modding Discord";
            discordButton.UseVisualStyleBackColor = false;
            discordButton.Click += discordButton_Click;
            // 
            // infoButton
            // 
            infoButton.BackColor = Color.FromArgb(48, 48, 48);
            infoButton.BackgroundImageLayout = ImageLayout.None;
            infoButton.FlatStyle = FlatStyle.Popup;
            infoButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            infoButton.ForeColor = SystemColors.ControlLight;
            infoButton.Location = new Point(906, 4);
            infoButton.Name = "infoButton";
            infoButton.Size = new Size(70, 45);
            infoButton.TabIndex = 4;
            infoButton.Text = "Info";
            infoButton.UseVisualStyleBackColor = false;
            infoButton.Click += infoButton_Click;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Font = new Font("Calibri", 19F, FontStyle.Bold, GraphicsUnit.Point, 0);
            versionLabel.ForeColor = SystemColors.ControlLight;
            versionLabel.Location = new Point(975, 9);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(245, 32);
            versionLabel.TabIndex = 1;
            versionLabel.Text = "Launcher Version: 6.9";
            versionLabel.Click += versionLabel_Click;
            // 
            // addmodsButton
            // 
            addmodsButton.BackColor = Color.FromArgb(48, 48, 48);
            addmodsButton.FlatStyle = FlatStyle.Popup;
            addmodsButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            addmodsButton.ForeColor = SystemColors.ControlLight;
            addmodsButton.Location = new Point(469, 4);
            addmodsButton.Name = "addmodsButton";
            addmodsButton.Size = new Size(130, 45);
            addmodsButton.TabIndex = 3;
            addmodsButton.Text = "Add Mods";
            addmodsButton.UseVisualStyleBackColor = false;
            addmodsButton.Click += addmodsButton_Click;
            // 
            // launchButton
            // 
            launchButton.BackColor = Color.FromArgb(48, 48, 48);
            launchButton.FlatStyle = FlatStyle.Popup;
            launchButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            launchButton.ForeColor = SystemColors.ControlLight;
            launchButton.Location = new Point(271, 4);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(130, 45);
            launchButton.TabIndex = 2;
            launchButton.Text = "Launch Game";
            launchButton.UseVisualStyleBackColor = false;
            launchButton.Click += launchButton_Click;
            // 
            // setupButton
            // 
            setupButton.BackColor = Color.FromArgb(48, 48, 48);
            setupButton.BackgroundImageLayout = ImageLayout.None;
            setupButton.FlatStyle = FlatStyle.Popup;
            setupButton.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            setupButton.ForeColor = SystemColors.ControlLight;
            setupButton.Location = new Point(59, 4);
            setupButton.Name = "setupButton";
            setupButton.Size = new Size(70, 45);
            setupButton.TabIndex = 1;
            setupButton.Text = "Setup";
            setupButton.UseVisualStyleBackColor = false;
            setupButton.Click += setupButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icon;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 45);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { deleteModToolStripMenuItem, openModFolderToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(164, 48);
            // 
            // deleteModToolStripMenuItem
            // 
            deleteModToolStripMenuItem.Name = "deleteModToolStripMenuItem";
            deleteModToolStripMenuItem.Size = new Size(163, 22);
            deleteModToolStripMenuItem.Text = "delete mod";
            deleteModToolStripMenuItem.Click += deleteModToolStripMenuItem_Click;
            // 
            // openModFolderToolStripMenuItem
            // 
            openModFolderToolStripMenuItem.Name = "openModFolderToolStripMenuItem";
            openModFolderToolStripMenuItem.Size = new Size(163, 22);
            openModFolderToolStripMenuItem.Text = "open mod folder";
            openModFolderToolStripMenuItem.Click += openModFolderToolStripMenuItem_Click;
            // 
            // outputRichTextBox
            // 
            outputRichTextBox.BackColor = Color.FromArgb(32, 32, 32);
            outputRichTextBox.Dock = DockStyle.Bottom;
            outputRichTextBox.Font = new Font("Yu Gothic", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            outputRichTextBox.ForeColor = Color.Lime;
            outputRichTextBox.Location = new Point(0, 588);
            outputRichTextBox.Name = "outputRichTextBox";
            outputRichTextBox.ReadOnly = true;
            outputRichTextBox.Size = new Size(1224, 141);
            outputRichTextBox.TabIndex = 4;
            outputRichTextBox.Text = "";
            // 
            // modImageBox
            // 
            modImageBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            modImageBox.BackColor = Color.FromArgb(32, 32, 32);
            modImageBox.BackgroundImageLayout = ImageLayout.Stretch;
            modImageBox.Location = new Point(711, 60);
            modImageBox.Name = "modImageBox";
            modImageBox.Size = new Size(510, 289);
            modImageBox.TabIndex = 3;
            modImageBox.TabStop = false;
            // 
            // descriptionRichTextBox
            // 
            descriptionRichTextBox.Anchor = AnchorStyles.Right;
            descriptionRichTextBox.BackColor = Color.FromArgb(32, 32, 32);
            descriptionRichTextBox.Font = new Font("Yu Gothic", 9F, FontStyle.Bold);
            descriptionRichTextBox.ForeColor = SystemColors.ControlLight;
            descriptionRichTextBox.Location = new Point(711, 355);
            descriptionRichTextBox.Name = "descriptionRichTextBox";
            descriptionRichTextBox.ReadOnly = true;
            descriptionRichTextBox.Size = new Size(507, 227);
            descriptionRichTextBox.TabIndex = 2;
            descriptionRichTextBox.Text = resources.GetString("descriptionRichTextBox.Text");
            // 
            // modListBox
            // 
            modListBox.Anchor = AnchorStyles.None;
            modListBox.BackColor = Color.FromArgb(32, 32, 32);
            modListBox.BorderStyle = BorderStyle.FixedSingle;
            modListBox.CheckOnClick = true;
            modListBox.ContextMenuStrip = contextMenuStrip1;
            modListBox.Font = new Font("Calibri", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            modListBox.ForeColor = SystemColors.ControlLight;
            modListBox.FormattingEnabled = true;
            modListBox.Location = new Point(3, 60);
            modListBox.Name = "modListBox";
            modListBox.ScrollAlwaysVisible = true;
            modListBox.Size = new Size(702, 522);
            modListBox.TabIndex = 1;
            modListBox.ItemCheck += modListBox_ItemCheck;
            modListBox.DragDrop += modListBox_DragDrop;
            modListBox.DragOver += modListBox_DragOver;
            modListBox.MouseDown += modListBox_MouseDown;
            modListBox.MouseMove += modListBox_MouseMove;
            modListBox.MouseUp += modListBox_MouseUp;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 48, 48);
            ClientSize = new Size(1224, 729);
            Controls.Add(modListBox);
            Controls.Add(descriptionRichTextBox);
            Controls.Add(modImageBox);
            Controls.Add(outputRichTextBox);
            Controls.Add(panelTop);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "AJ Launcher";
            Load += Form1_Load_1;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)modImageBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTop;
        private PictureBox pictureBox1;
        private Label versionLabel;
        private Button addmodsButton;
        private Button launchButton;
        private Button setupButton;
        private Button infoButton;
        private Button discordButton;
        private RichTextBox outputRichTextBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem deleteModToolStripMenuItem;
        private ToolStripMenuItem openModFolderToolStripMenuItem;
        private PictureBox modImageBox;
        private RichTextBox descriptionRichTextBox;
        private CheckedListBox modListBox;
        private Label label1;
        private Button refreshButton;
        private Button vanillaButton;
    }
}
