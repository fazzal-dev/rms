﻿using Savora.Model;
using Savora.Utilities;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Savora.View
{
    public partial class frmTableView : SampleView
    {
        bool Authorized = false;
        public frmTableView()
        {
            InitializeComponent();
        }

        private async void frmTableView_Load(object sender, EventArgs e)
        {
            await getData();
        }

        public async Task getData()
        {
            string query = "SELECT * FROM tables WHERE Name LIKE '%" + txtSearchBox.Text + "%'";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);

            await DatabaseHelper.LoadData(query, DataGridView, lb);
        }

        public async override void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            await getData();
        }

        public async override void btnAdd_Click(object sender, EventArgs e)
        {

            string loggedInUserRole = await DatabaseHelper.GetUserRole(frmLogin.LoggedInUsername);
            Authorized = AuthorizationManager.IsAuthorized(loggedInUserRole, "AddTable");
            if (Authorized)
            {
                //for Blur Background
                BlurUtility.BlurBackground(new frmTableAdd());
                await getData();
            }
            else
            {
                MessageBox.Show("You are not authorized to perform this action.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

           

        }

        private async void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Authorized)
            {
                // Check if the clicked cell belongs to the "dgvEdit" column
                if (DataGridView.CurrentCell.OwningColumn.Name == "dgvEdit")
                {
                    frmTableAdd form = new frmTableAdd();
                    // Retrieve the ID from the selected row and set it in the form
                    form.id = Convert.ToInt32(DataGridView.CurrentRow.Cells["dgvid"].Value);
                    // Set the text of the txtAddTable TextBox in the form from the selected row
                    form.txtAddTable.Text = Convert.ToString(DataGridView.CurrentRow.Cells["dgvName"].Value);
                    BlurUtility.BlurBackground(form);
                    await getData();
                }

                if (DataGridView.CurrentCell.OwningColumn.Name == "dgvDelete")
                {

                    bool res = MessageBox.Show("Are you sure you want to delete?", "Savora", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
                    if (res)
                    {
                        int id = Convert.ToInt32(DataGridView.CurrentRow.Cells["dgvid"].Value);
                        string query = $"DELETE FROM tables WHERE ID = {id}";
                        Hashtable ht = new Hashtable();
                        await DatabaseHelper.CURD(query, ht);
                        await getData();
                    }

                }
            }
            else
            {
                MessageBox.Show("You are not authorized to perform this action.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
