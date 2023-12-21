using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Прививки
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted

    }
    public partial class Form2 : Form
    {
        Database database = new Database();
        int selectedRow;
        public Form2()
        {
            InitializeComponent();
     
        }

        private void CreateColums()
        {
            dataGridView1.Columns.Add("id_vaccine","id");
            dataGridView1.Columns.Add("Name", "Название");
            dataGridView1.Columns.Add("Description", "Описание");
        }

        private void ReadSingleRow(DataGridView dgw,IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from Vaccinations";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());

            database.openConnection();
            
            SqlDataReader reader=command.ExecuteReader();

            while(reader.Read())
            {
                ReadSingleRow(dgw,reader);
            }
            reader.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var name = textBox1.Text;
            var description = textBox2.Text;

            var addQuery = $"insert into Vaccinations(Name, Description) values ('{name}','{description}')";

            var command = new SqlCommand(addQuery, database.getConnection());
            command.ExecuteNonQuery();

            database.closeConnection();
 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
         
        }
        
        private void deleteRow()
        {
            int index=dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString()==string.Empty)
            {
                dataGridView1.Rows[index].Cells[2].Value = RowState.Deleted;
                return;
            }

        }
        private void Update()
        {
            database.openConnection();

            for(int index=0; index<dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[0].Value;
                if(rowState == RowState.Deleted)
                    continue;
                
                if(rowState ==RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Vaccinations where id_vaccine={id} ";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }
            }
            database.closeConnection();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }
    }
}
