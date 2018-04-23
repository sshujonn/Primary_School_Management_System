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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PrimarySchoolManagentSystem
{
    public partial class ResultShow : Form
    {
        static string connectionString = @"server=localhost;user id=root;database=primaryschoolmanagement";

        int[] array = new int[30];
        int total = 0,total1=0,total2=0;
        MySqlConnection con = new MySqlConnection(connectionString);
        MySqlCommand cmd;

        MySqlDataAdapter adapter;
        DataTable dt = new DataTable();
        public ResultShow()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Position";
            dataGridView1.Columns[1].Name = "Student Roll";
            dataGridView1.Columns[2].Name = "Mark";
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            comboBox2.Items.Add("All Subjects");
            try
            {
                con.Open();
                string cbquery = "SELECT Subject FROM classsubject WHERE Class='" + comboBox3.SelectedItem + "'";
                cmd = new MySqlCommand(cbquery, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["Subject"].ToString());
                }
                con.Close();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //retrive item to datagridview
        private void retrieve()
        {
            string qRetMark;
            if(comboBox2.Text=="All Subjects")
            {
                qRetMark = "SELECT Roll_Number,SUM(Marks) FROM marks WHERE Term='" + comboBox1.Text + "' AND ClassSubjectID IN (SELECT ID FROM classsubject WHERE Class='" + comboBox3.Text + "') GROUP by Roll_Number ORDER BY SUM(Marks) DESC";

            }
            else
            {
                qRetMark = "SELECT Roll_Number,SUM(Marks) FROM marks WHERE Term='" + comboBox1.Text + "' AND ClassSubjectID IN (SELECT ID FROM classsubject WHERE Class='" + comboBox3.Text + "' AND Subject='" + comboBox2.Text + "') GROUP by Roll_Number ORDER BY SUM(Marks) DESC";

            }
            
            dataGridView1.Rows.Clear();
            cmd = new MySqlCommand(qRetMark, con);
            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(dt);
                int a=0;
                foreach (DataRow row in dt.Rows)
                {
                    a++;
                    populate(a.ToString(),  row[0].ToString(),row[1].ToString());
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
        private void populate(String position,String roll_number,  String tot_marks)
        {
            
            dataGridView1.Rows.Add(position,roll_number,tot_marks);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            retrieve();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Document doc = new Document(iTextSharp.text.PageSize.LETTER,10,10,42,35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("A:/3rd year ii term/SE project/New folder/PrimarySchoolManagentSystem/Result Cards/Result Card " + label18.Text+" Roll "+ label19.Text + ".pdf", FileMode.Create));
            doc.Open();
            
            iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance("resultCard1.png");
            PNG.ScaleToFit(400f,800f);
           // PNG.SetAbsolutePosition(doc.PageSize.Width-36f-72f,doc.PageSize.Height-36-416f);
            
            doc.Add(PNG);
            Paragraph paragraph; 
            paragraph = new Paragraph("Full Name:     " + label18.Text + "\n" + "Roll:             " + label19.Text + "\nClass:             "+comboBox5.Text+"\n\n");
            paragraph.Font.Size=20f;
            paragraph.Font.IsBold();
            doc.Add(paragraph);

            PdfPTable tbl = new PdfPTable(4);
           // PdfPCell cell=new PdfPCell(new Phrase("a",new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL,14f,iTextSharp.text.Font.BOLD,iTextSharp.text.BaseColor.BLUE)));

            paragraph = new Paragraph("   SUBJECT");
            paragraph.Font.Size = 15f;
            paragraph.Font.IsBold();
            tbl.AddCell(paragraph);
            paragraph = new Paragraph("   TERM-1");
            paragraph.Font.Size = 15f;
            paragraph.Font.IsBold();
            tbl.AddCell(paragraph);
            paragraph = new Paragraph("   TERM-2");
            paragraph.Font.Size = 15f;
            paragraph.Font.IsBold();
            tbl.AddCell(paragraph);
            paragraph = new Paragraph("   FINAL");
            paragraph.Font.Size = 15f;
            paragraph.Font.IsBold();
            tbl.AddCell(paragraph);

            tbl.AddCell(label12.Text);
            tbl.AddCell(textBox1.Text);
            tbl.AddCell(textBox12.Text);
            tbl.AddCell(textBox18.Text);

            tbl.AddCell(label13.Text);
            tbl.AddCell(textBox2.Text);
            tbl.AddCell(textBox11.Text);
            tbl.AddCell(textBox17.Text);

            tbl.AddCell(label14.Text);
            tbl.AddCell(textBox3.Text);
            tbl.AddCell(textBox10.Text);
            tbl.AddCell(textBox16.Text);

            tbl.AddCell(label15.Text);
            tbl.AddCell(textBox4.Text);
            tbl.AddCell(textBox9.Text);
            tbl.AddCell(textBox15.Text);

            tbl.AddCell(label16.Text);
            tbl.AddCell(textBox5.Text);
            tbl.AddCell(textBox8.Text);
            tbl.AddCell(textBox14.Text);

            tbl.AddCell(label17.Text);
            tbl.AddCell(textBox6.Text);
            tbl.AddCell(textBox7.Text);
            tbl.AddCell(textBox13.Text);


            tbl.AddCell("TOTAL MARK: ");
            tbl.AddCell("         "+total.ToString());
            //string Total1,Total2;
            /*if(total1==0)
            {
                tbl.AddCell("");
            }
            else if(total2==0)
            {
                tbl.AddCell("");
            }
            else
            {*/
                tbl.AddCell("         " + total1.ToString());
                tbl.AddCell("         " + total2.ToString());
          //  }
            

            doc.Add(tbl);
            doc.Close();
            MessageBox.Show("PDF Created!!");
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string cbquery = "SELECT Name FROM student WHERE Roll_Number='" + comboBox4.SelectedItem + "'";
                cmd = new MySqlCommand(cbquery, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    
                    label18.Text=reader["Name"].ToString();
                    label19.Text = comboBox4.SelectedItem.ToString();
                    label18.Show();
                    label19.Show();
                }
                con.Close();

                con.Open();
                if(comboBox6.SelectedItem!="All Terms")
                {
                    string mrkquery = "SELECT Marks FROM marks WHERE Roll_Number='" + comboBox4.SelectedItem + "'and Term='"+comboBox6.SelectedItem+"' and ClassSubjectID in ( select ID from classsubject where Class='"+comboBox5.SelectedItem+"') order by ClassSubjectID DESC";
                    cmd = new MySqlCommand(mrkquery, con);
                    MySqlDataReader reader1 = cmd.ExecuteReader();
                    int i = 0;
                    while(reader1.Read())
                    {
                        array[i]=int.Parse(reader1["Marks"].ToString());
                        i++;
                    }

                    total = array[0] + array[1] + array[2] + array[3] + array[4] + array[5];

                    if(comboBox6.SelectedItem=="1st")
                    {
                        textBox1.Text = "           " + array[0].ToString();
                        textBox2.Text = "           " + array[1].ToString();
                        textBox3.Text = "           " + array[2].ToString();
                        textBox4.Text = "           " + array[3].ToString();
                        textBox5.Text = "           " + array[4].ToString();
                        textBox6.Text = "           " + array[5].ToString();
                        total = array[0] + array[1] + array[2] + array[3] + array[4] + array[5];
                        total1 = 0;
                        total2 = 0;


                        textBox12.Text = "";
                        textBox11.Text = "";
                        textBox10.Text = "";
                        textBox9.Text = "";
                        textBox8.Text = "";
                        textBox7.Text = "";


                        textBox18.Text = "";
                        textBox17.Text = "";
                        textBox16.Text = "";
                        textBox15.Text = "";
                        textBox14.Text = "";
                        textBox13.Text = "";
                    }
                    else if (comboBox6.SelectedItem == "2nd")
                    {
                        textBox12.Text = "           " + array[0].ToString();
                        textBox11.Text = "           " + array[1].ToString();
                        textBox10.Text = "           " + array[2].ToString();
                        textBox9.Text = "           " + array[3].ToString();
                        textBox8.Text = "           " + array[4].ToString();
                        textBox7.Text = "           " + array[5].ToString();
                        total = array[0] + array[1] + array[2] + array[3] + array[4] + array[5];
                        total1 = 0;
                        total2 = 0;

                        textBox18.Text = "";
                        textBox17.Text = "";
                        textBox16.Text = "";
                        textBox15.Text = "";
                        textBox14.Text = "";
                        textBox13.Text = "";

                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                    }

                    else if (comboBox6.SelectedItem == "Final")
                    {
                        textBox18.Text = "           " + array[0].ToString();
                        textBox17.Text = "           " + array[1].ToString();
                        textBox16.Text = "           " + array[2].ToString();
                        textBox15.Text = "           " + array[3].ToString();
                        textBox14.Text = "           " + array[4].ToString();
                        textBox13.Text = "           " + array[5].ToString();
                        total = array[0] + array[1] + array[2] + array[3] + array[4] + array[5];
                        total1 = 0;
                        total2 = 0;

                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";

                        textBox12.Text = "";
                        textBox11.Text = "";
                        textBox10.Text = "";
                        textBox9.Text = "";
                        textBox8.Text = "";
                        textBox7.Text = "";
                    }
                    /*else
                    {

                    }*/

                }

                else
                {
                    string mrkquery = "SELECT Marks FROM marks WHERE Roll_Number='" + comboBox4.SelectedItem + "' and ClassSubjectID in ( select ID from classsubject where Class='" + comboBox5.SelectedItem + "') order by ClassSubjectID DESC";
                    cmd = new MySqlCommand(mrkquery, con);
                    MySqlDataReader reader1 = cmd.ExecuteReader();
                    int i = 0;
                    while (reader1.Read())
                    {
                        array[i] = int.Parse(reader1["Marks"].ToString());
                        i++;
                    }

                    textBox1.Text = "           " + array[0].ToString();
                    textBox2.Text = "           " + array[1].ToString();
                    textBox3.Text = "           " + array[2].ToString();
                    textBox4.Text = "           " + array[3].ToString();
                    textBox5.Text = "           " + array[4].ToString();
                    textBox6.Text = "           " + array[5].ToString();
                    total = array[0] + array[1] + array[2] + array[3] + array[4] + array[5];

                    textBox12.Text = "           " + array[6].ToString();
                    textBox11.Text = "           " + array[7].ToString();
                    textBox10.Text = "           " + array[8].ToString();
                    textBox9.Text = "           " + array[9].ToString();
                    textBox8.Text = "           " + array[10].ToString();
                    textBox7.Text = "           " + array[11].ToString();
                    total1 = array[6] + array[7] + array[8] + array[9] + array[10] + array[11];

                    textBox18.Text = "           " + array[12].ToString();
                    textBox17.Text = "           " + array[13].ToString();
                    textBox16.Text = "           " + array[14].ToString();
                    textBox15.Text = "           " + array[15].ToString();
                    textBox14.Text = "           " + array[16].ToString();
                    textBox13.Text = "           " + array[17].ToString();
                    total2 = array[12] + array[13] + array[14] + array[15] + array[16] + array[17];


                }
                
                con.Close();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                
                
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            try
            {
                con.Open();
                string cbquery = "SELECT Roll_Number FROM student WHERE CurrentClass='" + comboBox5.SelectedItem + "' order by Roll_Number ASC";
                cmd = new MySqlCommand(cbquery, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox4.Items.Add(reader["Roll_Number"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Home h = new Home();
            h.Show();
            this.Hide();
        }
    }
}
