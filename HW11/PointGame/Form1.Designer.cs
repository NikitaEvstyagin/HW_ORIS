﻿namespace PointGame
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
            btn_signIn = new Button();
            label1 = new Label();
            enterName = new TextBox();
            listOfUsers = new ListView();
            testLabel = new Label();
            color = new Label();
            SuspendLayout();
            // 
            // btn_signIn
            // 
            btn_signIn.Location = new Point(307, 227);
            btn_signIn.Name = "btn_signIn";
            btn_signIn.Size = new Size(94, 29);
            btn_signIn.TabIndex = 0;
            btn_signIn.Text = "Войти";
            btn_signIn.UseVisualStyleBackColor = true;
            btn_signIn.Click += btn_signIn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(278, 141);
            label1.Name = "label1";
            label1.Size = new Size(141, 20);
            label1.TabIndex = 1;
            label1.Text = "Введите ваше имя!";
            // 
            // enterName
            // 
            enterName.Location = new Point(294, 182);
            enterName.Name = "enterName";
            enterName.Size = new Size(125, 27);
            enterName.TabIndex = 2;
            // 
            // listOfUsers
            // 
            listOfUsers.Location = new Point(619, 64);
            listOfUsers.Name = "listOfUsers";
            listOfUsers.Size = new Size(157, 384);
            listOfUsers.TabIndex = 3;
            listOfUsers.UseCompatibleStateImageBehavior = false;
            listOfUsers.View = View.List;
            // 
            // testLabel
            // 
            testLabel.AutoSize = true;
            testLabel.Location = new Point(642, 31);
            testLabel.Name = "testLabel";
            testLabel.Size = new Size(0, 20);
            testLabel.TabIndex = 4;
            // 
            // color
            // 
            color.AutoSize = true;
            color.Location = new Point(710, 31);
            color.Name = "color";
            color.Size = new Size(43, 20);
            color.TabIndex = 5;
            color.Text = "color";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(color);
            Controls.Add(testLabel);
            Controls.Add(listOfUsers);
            Controls.Add(enterName);
            Controls.Add(label1);
            Controls.Add(btn_signIn);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_signIn;
        private Label label1;
        private TextBox enterName;
        private ListView listOfUsers;
        private Label testLabel;
        private Label color;
    }
}