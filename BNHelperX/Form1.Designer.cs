namespace BNHelperX
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelOutRoom = new System.Windows.Forms.Panel();
            this.labelMapContain = new System.Windows.Forms.Label();
            this.labelTitleIs = new System.Windows.Forms.Label();
            this.labelTitleContain = new System.Windows.Forms.Label();
            this.textBoxMapContain = new System.Windows.Forms.TextBox();
            this.textBoxTitleIs = new System.Windows.Forms.TextBox();
            this.textBoxTitleContain = new System.Windows.Forms.TextBox();
            this.checkBoxAutoTry = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoJoin = new System.Windows.Forms.CheckBox();
            this.panelInRoom = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxMessage = new System.Windows.Forms.ListBox();
            this.textBoxChat = new System.Windows.Forms.TextBox();
            this.listViewAdvList = new BNHelperX.DoubleBufferListView();
            this.listViewPlayerList = new BNHelperX.DoubleBufferListView();
            this.panelOutRoom.SuspendLayout();
            this.panelInRoom.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelOutRoom
            // 
            this.panelOutRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelOutRoom.Controls.Add(this.labelMapContain);
            this.panelOutRoom.Controls.Add(this.labelTitleIs);
            this.panelOutRoom.Controls.Add(this.labelTitleContain);
            this.panelOutRoom.Controls.Add(this.textBoxMapContain);
            this.panelOutRoom.Controls.Add(this.textBoxTitleIs);
            this.panelOutRoom.Controls.Add(this.textBoxTitleContain);
            this.panelOutRoom.Controls.Add(this.checkBoxAutoTry);
            this.panelOutRoom.Controls.Add(this.checkBoxAutoJoin);
            this.panelOutRoom.Controls.Add(this.listViewAdvList);
            this.panelOutRoom.Location = new System.Drawing.Point(12, 12);
            this.panelOutRoom.Name = "panelOutRoom";
            this.panelOutRoom.Size = new System.Drawing.Size(500, 328);
            this.panelOutRoom.TabIndex = 0;
            // 
            // labelMapContain
            // 
            this.labelMapContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMapContain.AutoSize = true;
            this.labelMapContain.Location = new System.Drawing.Point(258, 310);
            this.labelMapContain.Name = "labelMapContain";
            this.labelMapContain.Size = new System.Drawing.Size(89, 12);
            this.labelMapContain.TabIndex = 7;
            this.labelMapContain.Text = "And 地图名包含";
            // 
            // labelTitleIs
            // 
            this.labelTitleIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTitleIs.AutoSize = true;
            this.labelTitleIs.Location = new System.Drawing.Point(81, 283);
            this.labelTitleIs.Name = "labelTitleIs";
            this.labelTitleIs.Size = new System.Drawing.Size(41, 12);
            this.labelTitleIs.TabIndex = 2;
            this.labelTitleIs.Text = "游戏名";
            // 
            // labelTitleContain
            // 
            this.labelTitleContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTitleContain.AutoSize = true;
            this.labelTitleContain.Location = new System.Drawing.Point(81, 310);
            this.labelTitleContain.Name = "labelTitleContain";
            this.labelTitleContain.Size = new System.Drawing.Size(65, 12);
            this.labelTitleContain.TabIndex = 5;
            this.labelTitleContain.Text = "游戏名包含";
            // 
            // textBoxMapContain
            // 
            this.textBoxMapContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxMapContain.Location = new System.Drawing.Point(353, 306);
            this.textBoxMapContain.Name = "textBoxMapContain";
            this.textBoxMapContain.Size = new System.Drawing.Size(100, 21);
            this.textBoxMapContain.TabIndex = 8;
            // 
            // textBoxTitleIs
            // 
            this.textBoxTitleIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxTitleIs.Location = new System.Drawing.Point(128, 279);
            this.textBoxTitleIs.Name = "textBoxTitleIs";
            this.textBoxTitleIs.Size = new System.Drawing.Size(140, 21);
            this.textBoxTitleIs.TabIndex = 3;
            // 
            // textBoxTitleContain
            // 
            this.textBoxTitleContain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxTitleContain.Location = new System.Drawing.Point(152, 306);
            this.textBoxTitleContain.Name = "textBoxTitleContain";
            this.textBoxTitleContain.Size = new System.Drawing.Size(100, 21);
            this.textBoxTitleContain.TabIndex = 6;
            // 
            // checkBoxAutoTry
            // 
            this.checkBoxAutoTry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAutoTry.AutoSize = true;
            this.checkBoxAutoTry.Location = new System.Drawing.Point(3, 282);
            this.checkBoxAutoTry.Name = "checkBoxAutoTry";
            this.checkBoxAutoTry.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoTry.TabIndex = 1;
            this.checkBoxAutoTry.Text = "暴力抢坑";
            this.checkBoxAutoTry.UseVisualStyleBackColor = true;
            this.checkBoxAutoTry.CheckedChanged += new System.EventHandler(this.checkBoxAutoTry_CheckedChanged);
            // 
            // checkBoxAutoJoin
            // 
            this.checkBoxAutoJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAutoJoin.AutoSize = true;
            this.checkBoxAutoJoin.Location = new System.Drawing.Point(3, 309);
            this.checkBoxAutoJoin.Name = "checkBoxAutoJoin";
            this.checkBoxAutoJoin.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoJoin.TabIndex = 4;
            this.checkBoxAutoJoin.Text = "自动进主";
            this.checkBoxAutoJoin.UseVisualStyleBackColor = true;
            this.checkBoxAutoJoin.CheckedChanged += new System.EventHandler(this.checkBoxAutoJoin_CheckedChanged);
            // 
            // panelInRoom
            // 
            this.panelInRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInRoom.Controls.Add(this.splitContainer1);
            this.panelInRoom.Location = new System.Drawing.Point(12, 12);
            this.panelInRoom.Name = "panelInRoom";
            this.panelInRoom.Size = new System.Drawing.Size(500, 328);
            this.panelInRoom.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewPlayerList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBoxMessage);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxChat);
            this.splitContainer1.Size = new System.Drawing.Size(500, 328);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // listBoxMessage
            // 
            this.listBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMessage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxMessage.FormattingEnabled = true;
            this.listBoxMessage.ItemHeight = 12;
            this.listBoxMessage.Location = new System.Drawing.Point(3, 1);
            this.listBoxMessage.Name = "listBoxMessage";
            this.listBoxMessage.Size = new System.Drawing.Size(494, 100);
            this.listBoxMessage.TabIndex = 0;
            this.listBoxMessage.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxMessage_DrawItem);
            // 
            // textBoxChat
            // 
            this.textBoxChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxChat.Location = new System.Drawing.Point(3, 107);
            this.textBoxChat.Name = "textBoxChat";
            this.textBoxChat.Size = new System.Drawing.Size(494, 21);
            this.textBoxChat.TabIndex = 2;
            this.textBoxChat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // listViewAdvList
            // 
            this.listViewAdvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAdvList.FullRowSelect = true;
            this.listViewAdvList.GridLines = true;
            this.listViewAdvList.Location = new System.Drawing.Point(3, 3);
            this.listViewAdvList.Name = "listViewAdvList";
            this.listViewAdvList.Size = new System.Drawing.Size(494, 270);
            this.listViewAdvList.TabIndex = 0;
            this.listViewAdvList.UseCompatibleStateImageBehavior = false;
            this.listViewAdvList.View = System.Windows.Forms.View.Details;
            this.listViewAdvList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewAdvList_MouseDoubleClick);
            // 
            // listViewPlayerList
            // 
            this.listViewPlayerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPlayerList.FullRowSelect = true;
            this.listViewPlayerList.GridLines = true;
            this.listViewPlayerList.Location = new System.Drawing.Point(3, 3);
            this.listViewPlayerList.MultiSelect = false;
            this.listViewPlayerList.Name = "listViewPlayerList";
            this.listViewPlayerList.Size = new System.Drawing.Size(494, 188);
            this.listViewPlayerList.TabIndex = 1;
            this.listViewPlayerList.UseCompatibleStateImageBehavior = false;
            this.listViewPlayerList.View = System.Windows.Forms.View.Details;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 352);
            this.Controls.Add(this.panelOutRoom);
            this.Controls.Add(this.panelInRoom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "War3 BN Helper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelOutRoom.ResumeLayout(false);
            this.panelOutRoom.PerformLayout();
            this.panelInRoom.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelOutRoom;
        private System.Windows.Forms.Panel panelInRoom;
        private System.Windows.Forms.ListBox listBoxMessage;
        private System.Windows.Forms.TextBox textBoxChat;
        private System.Windows.Forms.CheckBox checkBoxAutoJoin;
        private System.Windows.Forms.Label labelMapContain;
        private System.Windows.Forms.Label labelTitleContain;
        private System.Windows.Forms.TextBox textBoxMapContain;
        private System.Windows.Forms.TextBox textBoxTitleContain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DoubleBufferListView listViewPlayerList;
        private DoubleBufferListView listViewAdvList;
        private System.Windows.Forms.Label labelTitleIs;
        private System.Windows.Forms.TextBox textBoxTitleIs;
        private System.Windows.Forms.CheckBox checkBoxAutoTry;
    }
}

