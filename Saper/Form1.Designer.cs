namespace Saper
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
            _controlPanel = new Panel();
            _langChangerButton = new Button();
            _minesLabel = new Label();
            _difficultyComboBox = new ComboBox();
            _newGameButton = new Button();
            _controlPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _controlPanel
            // 
            _controlPanel.Controls.Add(_langChangerButton);
            _controlPanel.Controls.Add(_minesLabel);
            _controlPanel.Controls.Add(_difficultyComboBox);
            _controlPanel.Controls.Add(_newGameButton);
            _controlPanel.Location = new Point(0, 0);
            _controlPanel.Name = "_controlPanel";
            _controlPanel.Size = new Size(533, 199);
            _controlPanel.TabIndex = 0;
            // 
            // _langChangerButton
            // 
            _langChangerButton.Location = new Point(3, 11);
            _langChangerButton.Name = "_langChangerButton";
            _langChangerButton.Size = new Size(30, 23);
            _langChangerButton.TabIndex = 3;
            _langChangerButton.Text = "Ru";
            _langChangerButton.UseVisualStyleBackColor = true;
            _langChangerButton.Click += LangChanger_Click;
            // 
            // _minesLabel
            // 
            _minesLabel.Location = new Point(12, 38);
            _minesLabel.Name = "_minesLabel";
            _minesLabel.Size = new Size(86, 23);
            _minesLabel.TabIndex = 2;
            // 
            // _difficultyComboBox
            // 
            _difficultyComboBox.Location = new Point(39, 12);
            _difficultyComboBox.Name = "_difficultyComboBox";
            _difficultyComboBox.Size = new Size(167, 23);
            _difficultyComboBox.TabIndex = 0;
            _difficultyComboBox.SelectedIndexChanged += DifficultyComboBox_SelectedIndexChanged;
            // 
            // _newGameButton
            // 
            _newGameButton.Location = new Point(212, 11);
            _newGameButton.Name = "_newGameButton";
            _newGameButton.Size = new Size(92, 23);
            _newGameButton.TabIndex = 1;
            _newGameButton.Text = "New game";
            _newGameButton.Click += NewGameButton_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(311, 261);
            Controls.Add(_controlPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Minesweeper";
            Load += Form1_Load;
            _controlPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Panel _controlPanel;
        private Button _langChangerButton;
    }
}
