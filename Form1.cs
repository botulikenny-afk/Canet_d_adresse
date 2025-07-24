using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Canet_d_adresse
{
    public partial class Form1 : Form
    {
        private List<Contact> contacts = new List<Contact>();
        private int selectedIndex = -1;
        private string savePath = Path.Combine(Application.StartupPath, "contacts.csv");

        public Form1()
        {
            InitializeComponent();
            InitDataGrid();
            LoadFromFile();
        }

        private void InitDataGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nom", DataPropertyName = "Nom" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Prénom", DataPropertyName = "Prenom" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Téléphone", DataPropertyName = "Tel" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email" });

            dataGridView1.DataSource = contacts;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            contacts.Add(new Contact
            {
                Nom = textNom.Text.Trim(),
                Prenom = textPrenom.Text.Trim(),
                Tel = textTel.Text.Trim(),
                Email = textEmail.Text.Trim()
            });

            RefreshGrid();
            SaveToFile();
            ClearFields();
            MessageBox.Show("Contact ajouté !");
        }

        private void btnMod_Click(object sender, EventArgs e)
        {
            if (selectedIndex < 0 || selectedIndex >= contacts.Count) return;
            if (!ValidateFields()) return;

            contacts[selectedIndex] = new Contact
            {
                Nom = textNom.Text.Trim(),
                Prenom = textPrenom.Text.Trim(),
                Tel = textTel.Text.Trim(),
                Email = textEmail.Text.Trim()
            };

            RefreshGrid();
            SaveToFile();
            ClearFields();
            MessageBox.Show("Contact modifié !");
        }

        private void btnSup_Click(object sender, EventArgs e)
        {
            if (selectedIndex < 0 || selectedIndex >= contacts.Count) return;

            var confirm = MessageBox.Show("Confirmer la suppression ?", "Suppression", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                contacts.RemoveAt(selectedIndex);
                RefreshGrid();
                SaveToFile();
                ClearFields();
                MessageBox.Show("Contact supprimé !");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (contacts.Count == 0)
            {
                MessageBox.Show("Aucun contact à sauvegarder.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveToFile();
                MessageBox.Show(" {contacts.Count} contact(s) sauvegardé(s) dans :\n{savePath}", "Sauvegarde réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message, "Problème", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                selectedIndex = dataGridView1.SelectedRows[0].Index;
                var contact = contacts[selectedIndex];
                textNom.Text = contact.Nom;
                textPrenom.Text = contact.Prenom;
                textTel.Text = contact.Tel;
                textEmail.Text = contact.Email;
            }
            else
            {
                selectedIndex = -1;
                ClearFields();
            }
        }

        private void RefreshGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = contacts;
        }

        private void ClearFields()
        {
            textNom.Clear();
            textPrenom.Clear();
            textTel.Clear();
            textEmail.Clear();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(textNom.Text) ||
                string.IsNullOrWhiteSpace(textPrenom.Text) ||
                string.IsNullOrWhiteSpace(textTel.Text) ||
                string.IsNullOrWhiteSpace(textEmail.Text))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Champs vides", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void SaveToFile()
        {
            File.WriteAllLines(savePath, contacts.Select(c => "{c.Nom};{c.Prenom};{c.Tel};{c.Email}"));
        }

        private void LoadFromFile()
        {
            if (!File.Exists(savePath)) return;

            var lines = File.ReadAllLines(savePath);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 4)
                {
                    contacts.Add(new Contact
                    {
                        Nom = parts[0],
                        Prenom = parts[1],
                        Tel = parts[2],
                        Email = parts[3]
                    });
                }
            }
            RefreshGrid();
        }
    }

    public class Contact
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
    }
}
