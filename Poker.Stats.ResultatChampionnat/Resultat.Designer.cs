namespace Poker.Stats.ResultatChampionnat
{
    partial class Resultat
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
            this.btnCalculClassement = new System.Windows.Forms.Button();
            this.cboMethodeCalcul = new System.Windows.Forms.ComboBox();
            this.lstClassement = new System.Windows.Forms.ListView();
            this.colClassement = new System.Windows.Forms.ColumnHeader();
            this.colNomJoueur = new System.Windows.Forms.ColumnHeader();
            this.colNbPoints = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // btnCalculClassement
            // 
            this.btnCalculClassement.Location = new System.Drawing.Point(285, 15);
            this.btnCalculClassement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCalculClassement.Name = "btnCalculClassement";
            this.btnCalculClassement.Size = new System.Drawing.Size(253, 28);
            this.btnCalculClassement.TabIndex = 1;
            this.btnCalculClassement.Text = "Afficher le classement";
            this.btnCalculClassement.UseVisualStyleBackColor = true;
            this.btnCalculClassement.Click += new System.EventHandler(this.btnCalculClassement_Click);
            // 
            // cboMethodeCalcul
            // 
            this.cboMethodeCalcul.DisplayMember = "Libelle";
            this.cboMethodeCalcul.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMethodeCalcul.FormattingEnabled = true;
            this.cboMethodeCalcul.Location = new System.Drawing.Point(17, 15);
            this.cboMethodeCalcul.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboMethodeCalcul.Name = "cboMethodeCalcul";
            this.cboMethodeCalcul.Size = new System.Drawing.Size(259, 24);
            this.cboMethodeCalcul.TabIndex = 2;
            // 
            // lstClassement
            // 
            this.lstClassement.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colClassement,
            this.colNomJoueur,
            this.colNbPoints});
            this.lstClassement.Location = new System.Drawing.Point(16, 50);
            this.lstClassement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstClassement.MultiSelect = false;
            this.lstClassement.Name = "lstClassement";
            this.lstClassement.Size = new System.Drawing.Size(521, 322);
            this.lstClassement.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstClassement.TabIndex = 3;
            this.lstClassement.UseCompatibleStateImageBehavior = false;
            this.lstClassement.View = System.Windows.Forms.View.Details;
            // 
            // colClassement
            // 
            this.colClassement.Text = "Place";
            this.colClassement.Width = 78;
            // 
            // colNomJoueur
            // 
            this.colNomJoueur.Text = "Joueur";
            this.colNomJoueur.Width = 142;
            // 
            // colNbPoints
            // 
            this.colNbPoints.Text = "";
            this.colNbPoints.Width = 105;
            // 
            // Resultat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 388);
            this.Controls.Add(this.lstClassement);
            this.Controls.Add(this.cboMethodeCalcul);
            this.Controls.Add(this.btnCalculClassement);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Resultat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Résultat du championnat";
            this.Load += new System.EventHandler(this.Resultat_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCalculClassement;
        private System.Windows.Forms.ComboBox cboMethodeCalcul;
        private System.Windows.Forms.ListView lstClassement;
        private System.Windows.Forms.ColumnHeader colClassement;
        private System.Windows.Forms.ColumnHeader colNomJoueur;
        private System.Windows.Forms.ColumnHeader colNbPoints;
    }
}

