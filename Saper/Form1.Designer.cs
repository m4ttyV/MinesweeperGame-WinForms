namespace Saper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

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
            _controlPanel = new Panel();
            _hintLabel = new Label();
            _titleLabel = new Label();
            _langChangerButton = new Button();
            _minesLabel = new Label();
            _difficultyComboBox = new ComboBox();
            _newGameButton = new Button();
            _boardPanel = new Panel();
            _controlPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _controlPanel
            // 
            _controlPanel.Controls.Add(_hintLabel);
            _controlPanel.Controls.Add(_titleLabel);
            _controlPanel.Controls.Add(_langChangerButton);
            _controlPanel.Controls.Add(_minesLabel);
            _controlPanel.Controls.Add(_difficultyComboBox);
            _controlPanel.Controls.Add(_newGameButton);
            _controlPanel.Location = new Point(12, 12);
            _controlPanel.Name = "_controlPanel";
            _controlPanel.Size = new Size(520, 148);
            _controlPanel.TabIndex = 0;
            // 
            // _hintLabel
            // 
            _hintLabel.AutoEllipsis = true;
            _hintLabel.Location = new Point(24, 126);
            _hintLabel.Name = "_hintLabel";
            _hintLabel.Size = new Size(456, 20);
            _hintLabel.TabIndex = 6;
            // 
            // _titleLabel
            // 
            _titleLabel.Location = new Point(24, 18);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(250, 34);
            _titleLabel.TabIndex = 5;
            _titleLabel.Text = "Minesweeper";
            // 
            // _langChangerButton
            // 
            _langChangerButton.Location = new Point(388, 60);
            _langChangerButton.Name = "_langChangerButton";
            _langChangerButton.Size = new Size(76, 36);
            _langChangerButton.TabIndex = 3;
            _langChangerButton.Text = "RU";
            _langChangerButton.UseVisualStyleBackColor = true;
            _langChangerButton.Click += LangChanger_Click;
            // 
            // _minesLabel
            // 
            _minesLabel.Location = new Point(344, 24);
            _minesLabel.Name = "_minesLabel";
            _minesLabel.Size = new Size(140, 34);
            _minesLabel.TabIndex = 2;
            _minesLabel.Text = "Mines left: 10";
            // 
            // _difficultyComboBox
            // 
            _difficultyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _difficultyComboBox.FormattingEnabled = true;
            _difficultyComboBox.Location = new Point(24, 60);
            _difficultyComboBox.Name = "_difficultyComboBox";
            _difficultyComboBox.Size = new Size(220, 23);
            _difficultyComboBox.TabIndex = 0;
            _difficultyComboBox.SelectedIndexChanged += DifficultyComboBox_SelectedIndexChanged;
            // 
            // _newGameButton
            // 
            _newGameButton.Location = new Point(256, 60);
            _newGameButton.Name = "_newGameButton";
            _newGameButton.Size = new Size(120, 36);
            _newGameButton.TabIndex = 1;
            _newGameButton.Text = "New game";
            _newGameButton.UseVisualStyleBackColor = true;
            _newGameButton.Click += NewGameButton_Click;
            // 
            // _boardPanel
            // 
            _boardPanel.Location = new Point(12, 174);
            _boardPanel.Name = "_boardPanel";
            _boardPanel.Size = new Size(520, 300);
            _boardPanel.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(544, 486);
            Controls.Add(_boardPanel);
            Controls.Add(_controlPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Minesweeper";
            Load += Form1_Load;
            _controlPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel _controlPanel;
        private Label _hintLabel;
        private Label _titleLabel;
        private Button _langChangerButton;
        private Label _minesLabel;
        private ComboBox _difficultyComboBox;
        private Button _newGameButton;
        private Panel _boardPanel;
    }
}
