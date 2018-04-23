using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PrimarySchoolManagentSystem
{
    public partial class ResultInput : Form
    {

        static string connectionString = @"server=localhost;user id=root;database=primaryschoolmanagement";

        MySqlConnection con = new MySqlConnection(connectionString);
        MySqlCommand cmd;
    
        MySqlDataAdapter adapter;
        DataTable dt = new DataTable();
        public ResultInput()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Class";
            dataGridView1.Columns[2].Name = "Subject";
            dataGridView1.Columns[3].Name = "Roll";
            dataGridView1.Columns[4].Name = "Term";
            dataGridView1.Columns[5].Name = "Mark";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Home h = new Home();
            h.Show();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            try
            {
                con.Open();
                string cbquery = "SELECT Subject FROM classsubject WHERE Class='"+comboBox1.SelectedItem+"'";
                cmd = new MySqlCommand(cbquery,con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    comboBox2.Items.Add(reader["Subject"].ToString());
                }
                con.Close();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = comboBox1.Text;
            textBox3.Text = comboBox2.Text;
            textBox1.Text = "";
            retrieve();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        //add buttton
        private void button1_Click(object sender, EventArgs e)
        {
            
            addMark();
            retrieve();
            if(checkBox1.Checked)
            {
                int a = int.Parse(textBox1.Text);
                a++;
                textBox1.Text = a.ToString();
            }
            clearTextAfterAdd();
        }

        //add item to db
        public void addMark()
        {
            try
            {
                con.Open();
                
                string csID;
                int i=0;
                string csquery = "select ID from classsubject where Class='" + comboBox1.Text + "' and Subject='"+comboBox2.Text+"'";
                cmd = new MySqlCommand(csquery, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    csID = reader["ID"].ToString();
                    i = int.Parse(csID);
                }
                con.Close();
                
                
                con.Open();

                string insertMark = "INSERT INTO marks(ID,ClassSubjectID,Roll_Number,Term,Marks) VALUES(null,'" + i + "','" + textBox1.Text + "','" + comboBox3.Text + "','" + textBox4.Text+ "')";
                cmd = new MySqlCommand(insertMark,con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("mark added");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        //retrive item to datagridview
        private void retrieve()
        {
            string qRetMark = "Select marks.ID,Class,Subject,Roll_Number,Term,Marks FROM marks,classsubject where classsubject.Class='"+comboBox1.Text+"' AND classsubject.Subject='"+comboBox2.Text+"' AND marks.Term='"+comboBox3.Text+"' AND marks.ClassSubjectID in(Select ID FROM classsubject where Class='"+comboBox1.Text+"' and Subject='"+comboBox2.Text+"')";
            dataGridView1.Rows.Clear();
            cmd = new MySqlCommand(qRetMark, con);
            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    
                    populate(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());
                }

                con.Close();
                //clear dt
                dt.Rows.Clear();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Source);
            }
        }

        //populate to datagridview
        private void populate(String id, String class1, string subject, String roll, string term, String marks)
        {
            dataGridView1.Rows.Add(id, class1,subject,roll,term,marks);
        }
   
        //updating mark
        private void updateMark(int id, int roll,string term, int marks)
        {
            string sql = "UPDATE marks SET Roll_Number='" + roll + "',Term='" + term + "',Marks='" + marks + "' WHERE ID='" + id + "'";
            cmd = new MySqlCommand(sql, con);

            try
            {
                con.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.UpdateCommand = con.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;

                if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                {

                    MessageBox.Show("Updated");
                    //clearText();

                }

                con.Close();
                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //deleting mark
        private void deleteMark(int id)
        {
            string sql = "DELETE FROM marks WHERE ID='" + id + "'";
            cmd = new MySqlCommand(sql, con);

            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.DeleteCommand = con.CreateCommand();

                adapter.DeleteCommand.CommandText = sql;

                if (MessageBox.Show("Sure?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        
                        MessageBox.Show("Deleted successfully");

                    }
                }

                con.Close();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
                

            }
        }

        //textbox,combobox fill from datagridview
        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                comboBox3.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                comboBox1.Text = textBox2.Text;
                comboBox2.Text = textBox3.Text;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please Select The Whole Row");
            }
            

        }


        //clear text
        private void clearTextAfterupdate()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            
        }

        private void clearTextAfterAdd()
        {
            if(checkBox1.Checked)
            {
                textBox4.Text = "";
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }

        }



        //update button
        private void button2_Click(object sender, EventArgs e)
        {
            String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);

            updateMark(id, int.Parse(textBox1.Text), comboBox3.Text, int.Parse(textBox4.Text));
            retrieve();
            clearTextAfterupdate();
        }

        //delete button
        private void button3_Click(object sender, EventArgs e)
        {
            String selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);
            deleteMark(id);
            retrieve();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            this.AcceptButton = button1;
        }


    }
}
