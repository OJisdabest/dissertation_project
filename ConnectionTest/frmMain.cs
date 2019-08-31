using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ConnectionTest
{
    public partial class frmMain : Form
    {
        //Declaration of connection string to be used throughout to connect to local sql database.
        public string connstring = "Data Source=OLIVERPC\\SQLEXPRESS;User ID=oliverh;Password=password";
        //Declaration of variables to be used throughout program.
        bool home = true;
        bool inbox = false;
        bool add = false;
        bool prof = false;
        bool reg = false;
        bool login = false;
        string loggedinas = "";
        string prodselected = "";
        string userID = "";
        //Declaration of some objects created inside the code which needed to be global rather than local to the method.
        Button btnComment = new Button();
        TextBox txtbComment = new TextBox();
        ListBox lstComments = new ListBox();
        public frmMain()
        {
            InitializeComponent();
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            lstComments.Size = new Size(400, 200);
            //textbox comment properties
            txtbComment.Location = new Point(80, 12 + (6 * 28));
            txtbComment.Multiline = true;
            txtbComment.Size = new Size(325, 70);
            // btnComment properties
            btnComment.BackColor = Color.LightGray;
            btnComment.Location = new Point(5, 12 + (7 * 28));
            btnComment.Size = new Size(70, 40);
            btnComment.Text = "Post Comment";
            btnComment.FlatStyle = FlatStyle.Flat;
            btnComment.Visible = false;
            //Declares child form created object button click (links to method)
            this.btnComment.Click += new System.EventHandler(Post_Comment);
            //load data from database into datagrid on form load
            //sets datagrid location on form load
            dataGridView1.Location = new Point(0, 30);
            dataGridView1.Size = new Size(570, 800);
            Fill_DataGrid();
            Size_Datagrid();
            //sets panel size on form load (currently wider to hold all objects)
            Size = new Size(585, 934);
            //calls visibility method to see which objects should be visible based on page title
            Visibility();

            //panel header properties
            Panel panel1 = new Panel
            {
                Location = new Point(0, 0),
                Name = "Panel1",
                Size = new Size(585, 30),
                BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(panel1);

            //menu button locations defined on form load
            Btn_Home.Location = new Point(0, 829);
            Btn_Inbox.Location = new Point(142, 829);
            Btn_AddItem.Location = new Point(284, 829);
            Btn_Profile.Location = new Point(426, 829);
            //menu button colors defined on form load
            Btn_Home.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            Btn_Inbox.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            Btn_AddItem.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            Btn_Profile.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            Btn_Upload.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            Btn_Submit.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));

            //profile photo properties
            PicB_ProfilePhoto.Size = new Size(484, 379);
            PicB_ProfilePhoto.Location = new Point(42, 49);
        }
        //method for allowing user to post a comment
        private void Post_Comment(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("insert into application.dbo.[Comments] (ProductID, Username, Comment) VALUES (@A, @B, @C)", conn))
                    {
                        if (txtbComment.Text.Length > 0 & loggedinas != "")
                        {
                            cmd.Parameters.AddWithValue("@A", prodselected);
                            cmd.Parameters.AddWithValue("@B", loggedinas);
                            cmd.Parameters.AddWithValue("@C", txtbComment.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Comment posted!");
                            lstComments.Items.Clear();
                            Load_Comments();
                        }
                        else
                        {
                            MessageBox.Show("Must login to comment or no data inputted!");
                        }
                    }
                    //reset and refresh textboxes and comments
                    txtbComment.ResetText();
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //fill datagrid with database data
        private void Fill_DataGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Select ProductID as ID, ProductName as Name, Available, Description, Location, ExpiryDate, ProductImage as Image from application.dbo.Product", conn))
                    {
                        cmd.ExecuteNonQuery();
                        DataTable tblDataSource = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(tblDataSource);
                            dataGridView1.DataSource = tblDataSource;
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //define datagrid column widths, heights and visibility
        private void Size_Datagrid()
        {
            DataGridViewColumn column1 = dataGridView1.Columns[0];
            DataGridViewColumn column2 = dataGridView1.Columns[1];
            DataGridViewColumn column3 = dataGridView1.Columns[2];
            DataGridViewColumn column4 = dataGridView1.Columns[3];
            DataGridViewColumn column5 = dataGridView1.Columns[4];
            DataGridViewColumn column6 = dataGridView1.Columns[5];
            DataGridViewImageColumn column7 = (DataGridViewImageColumn)dataGridView1.Columns[6];
            column1.Width = 20;
            column2.Width = 105;
            column3.Visible = false;
            column4.Visible = false;
            column5.Width = 95;
            column6.Width = 85;
            column7.Width = 200;
            column7.ImageLayout = DataGridViewImageCellLayout.Stretch;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 200;
            }
        }
        //allow user to upload photo to attach to item
        private void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (loggedinas != "")
                {
                    //allows user to select an image from any location which they wish to upload for the product image
                    string imageLocation = "";
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
                    };
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        imageLocation = dialog.FileName;
                        Picb_Upload.ImageLocation = imageLocation;
                    }
                    Image myImage = Image.FromFile(imageLocation);
                    Lbl_UploadImage.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //menu button home sets page title and visibility of objects to true/ false
        private void Btn_Home_Click(object sender, EventArgs e)
        {
            Lbl_PageTitle.Text = "Home";
            home = true;
            inbox = false;
            add = false;
            prof = false;
            login = false;
            reg = false;
            Visibility();
        }
        //menu button inbox sets page title and visibility of objects to true/ false
        private void Btn_Inbox_Click(object sender, EventArgs e)
        {
            Lbl_PageTitle.Text = "Inbox";
            inbox = true;
            home = false;
            add = false;
            prof = false;
            login = false;
            reg = false;
            Visibility();
        }
        //menu button add sets page title and visibility of objects to true/ false
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            Lbl_PageTitle.Text = "Add Item";
            add = true;
            inbox = false;
            home = false;
            prof = false;
            login = false;
            reg = false;
            Visibility();
        }
        //when a user is logged in and they click profile this will show their details, which they can modify
        private void Load_Profile()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("select UserID, Username, Email, Password, FirstName, LastName, ProfilePhoto from application.dbo.[User] WHERE Username = @ID", conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", loggedinas);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                Txtb_UserID.Text = (dr[0].ToString());
                                Txtb_Username.Text = (dr[1].ToString());
                                Txtb_Email.Text = (dr[2].ToString());
                                Txtb_Password.Text = (dr[3].ToString());
                                Txtb_FirstName.Text = (dr[4].ToString());
                                Txtb_LastName.Text = (dr[5].ToString());
                                if (dr[6] != null)
                                {
                                    MemoryStream ms = new MemoryStream((byte[])dr[6]);
                                    PicB_ProfilePhoto.Image = Image.FromStream(ms);
                                }
                                Txtb_UserID.ReadOnly = true;
                                Txtb_Username.ReadOnly = true;
                                Txtb_Email.ReadOnly = true;
                                userID = (dr[0].ToString());
                            }
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //loads user comments for specific products
        private void Load_Comments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from application.dbo.[Comments] WHERE ProductID = @A", conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@A", prodselected);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lstComments.Items.Add("Username: " + dr[3] + " --- " + dr[4] + "---" + dr[5].ToString());
                                lstComments.Items.Add("--------------------------------------------------");
                            }
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //menu button profile sets page title and visibility of objects to true/ false
        private void Btn_Profile_Click(object sender, EventArgs e)
        {
            Lbl_PageTitle.Text = "Profile";
            prof = true;
            inbox = false;
            add = false;
            home = false;
            login = false;
            reg = false;
            Visibility();
            Load_Profile();
        }
        //sets visibility of objects based on variables
        private void Visibility()
        {
            if (home == true)
            {
                //set home objects visibility true
                dataGridView1.Visible = true;
                add = false;
                inbox = false;
                prof = false;
                login = false;
                reg = false;
            }
            else
            {
                dataGridView1.Visible = false;
            }

            if (inbox == true)
            {
                //set inbox objects visibility true
                add = false;
                home = false;
                prof = false;
                login = false;
                reg = false;
            }
            else
            {

            }

            if (add == true)
            {
                //set add item objects visibility true
                Lbl_ProductName.Visible = true;
                Lbl_Available.Visible = true;
                Lbl_Description.Visible = true;
                Lbl_Location.Visible = true;
                Lbl_ExpiryDate.Visible = true;
                Txtb_ProductName.Visible = true;
                Chkb_Available.Visible = true;
                Txtb_Description.Visible = true;
                Txtb_Location.Visible = true;
                DatePicker.Visible = true;
                Picb_Upload.Visible = true;
                Btn_Upload.Visible = true;
                Btn_Submit.Visible = true;
                Lbl_UploadImage.Visible = true;
                home = false;
                inbox = false;
                prof = false;
                login = false;
                reg = false;
            }
            else
            {
                Lbl_ProductName.Visible = false;
                Lbl_Available.Visible = false;
                Lbl_Description.Visible = false;
                Lbl_Location.Visible = false;
                Lbl_ExpiryDate.Visible = false;
                Txtb_ProductName.Visible = false;
                Chkb_Available.Visible = false;
                Txtb_Description.Visible = false;
                Txtb_Location.Visible = false;
                DatePicker.Visible = false;
                Picb_Upload.Visible = false;
                Btn_Upload.Visible = false;
                Btn_Submit.Visible = false;
                Lbl_UploadImage.Visible = false;
            }

            if (prof == true)
            {
                //set profile objects visibility true
                add = false;
                inbox = false;
                home = false;
                login = false;
                reg = false;
                Lbl_UserID.Visible = true;
                Txtb_UserID.Visible = true;
                PicB_ProfilePhoto.Visible = true;
                Lbl_Username.Visible = true;
                Lbl_Email.Visible = true;
                Lbl_FirstName.Visible = true;
                Txtb_Username.Visible = true;
                Txtb_Email.Visible = true;
                Txtb_FirstName.Visible = true;
                Btn_ProfilePhotoUpload.Visible = true;
                Btn_SubmitPhoto.Visible = true;
                Lbl_Password.Visible = true;
                Txtb_Password.Visible = true;
                lbl_LastName.Visible = true;
                Txtb_LastName.Visible = true;
            }
            else
            {
                Lbl_UserID.Visible = false;
                Txtb_UserID.Visible = false;
                PicB_ProfilePhoto.Visible = false;
                Lbl_Username.Visible = false;
                Lbl_Email.Visible = false;
                Lbl_FirstName.Visible = false;
                Txtb_Username.Visible = false;
                Txtb_Email.Visible = false;
                Txtb_FirstName.Visible = false;
                Btn_ProfilePhotoUpload.Visible = false;
                Btn_SubmitPhoto.Visible = false;
                Lbl_Password.Visible = false;
                Txtb_Password.Visible = false;
                lbl_LastName.Visible = false;
                Txtb_LastName.Visible = false;
            }

            if (reg == true)
            {
                Txtb_RegisterUsername.Visible = true;
                Txtb_RegisterEmail.Visible = true;
                Txtb_RegisterPassword.Visible = true;
                Txtb_RegisterFirstName.Visible = true;
                Txtb_RegisterLastName.Visible = true;
                Lbl_RegisterUsername.Visible = true;
                Lbl_RegisterEmail.Visible = true;
                Lbl_RegisterPassword.Visible = true;
                Lbl_RegisterFirstName.Visible = true;
                Lbl_RegisterLastName.Visible = true;
                Btn_Cancel.Visible = true;
                Btn_Register.Visible = true;
                prof = false;
                add = false;
                inbox = false;
                home = false;
                login = false;
            }
            else
            {
                Txtb_RegisterUsername.Visible = false;
                Txtb_RegisterEmail.Visible = false;
                Txtb_RegisterPassword.Visible = false;
                Txtb_RegisterFirstName.Visible = false;
                Txtb_RegisterLastName.Visible = false;
                Lbl_RegisterUsername.Visible = false;
                Lbl_RegisterEmail.Visible = false;
                Lbl_RegisterPassword.Visible = false;
                Lbl_RegisterFirstName.Visible = false;
                Lbl_RegisterLastName.Visible = false;
                Btn_Cancel.Visible = false;
                Btn_Register.Visible = false;
            }

            if (login == true)
            {
                Btn_Login.Visible = true;
                Btn_LoginCancel.Visible = true;
                Lbl_LoginPassword.Visible = true;
                Lbl_LoginUsername.Visible = true;
                Txtb_LoginUsername.Visible = true;
                Txtb_LoginPassword.Visible = true;
                prof = false;
                add = false;
                inbox = false;
                home = false;
                reg = false;
            }
            else
            {
                Btn_Login.Visible = false;
                Btn_LoginCancel.Visible = false;
                Lbl_LoginPassword.Visible = false;
                Lbl_LoginUsername.Visible = false;
                Txtb_LoginUsername.Visible = false;
                Txtb_LoginPassword.Visible = false;
            }
        }
        //resets add item form
        private void Reset_Fields()
        {
            Txtb_ProductName.ResetText();
            Chkb_Available.Checked = false;
            Txtb_Description.ResetText();
            Txtb_Location.ResetText();
            DatePicker.ResetText();
            Picb_Upload.Image = null;
        }
        //resets register form
        private void Reset_RegisterForm()
        {
            Txtb_RegisterUsername.ResetText();
            Txtb_RegisterEmail.ResetText();
            Txtb_RegisterPassword.ResetText();
            Txtb_RegisterFirstName.ResetText();
            Txtb_RegisterLastName.ResetText();
        }
        //selects the image and converts it to binary data to insert into database
        //if selected fields are null then button wont upload data
        private void Btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                Image myImage = Picb_Upload.Image;
                if (Picb_Upload.Image != null & Txtb_ProductName.Text != null & Chkb_Available.Checked != false & Txtb_Description.Text != null & Txtb_Location.Text != null & DatePicker.Value != null)
                {

                    byte[] data;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        data = ms.ToArray();
                    }
                    using (SqlConnection conn = new SqlConnection(connstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("insert into application.dbo.Product (ProductName, Available, Description, Location, ExpiryDate, ProductImage) VALUES (@A,@B,@C,@D,@E,@F)", conn))
                        {
                            cmd.Parameters.AddWithValue("@A", Txtb_ProductName.Text);
                            string checkState = "";
                            if (Chkb_Available.Checked == true)
                            {
                                checkState = "Yes";
                            }
                            else
                            {
                                checkState = "No";
                            }
                            cmd.Parameters.AddWithValue("@B", checkState);
                            cmd.Parameters.AddWithValue("@C", Txtb_Description.Text);
                            cmd.Parameters.AddWithValue("@D", Txtb_Location.Text);
                            cmd.Parameters.AddWithValue("@E", DatePicker.Text);
                            cmd.Parameters.AddWithValue("@F", data);
                            cmd.ExecuteNonQuery();
                        }
                        Reset_Fields();
                        Fill_DataGrid();
                        Size_Datagrid();
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please fill in all the relevant fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //on data input, search box will display relevant items in datagrid
        private void Txtb_Search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Select ProductID as ID, ProductName as Name, Available, Description, Location, ExpiryDate as Expiry_Date, ProductImage as Image from application.dbo.Product WHERE ProductName LIKE @A + '%' OR Location LIKE @B + '%' OR ExpiryDate LIKE '%' + @C + '%'", conn))
                    {
                        cmd.Parameters.AddWithValue("@A", Txtb_Search.Text);
                        cmd.Parameters.AddWithValue("@B", Txtb_Search.Text);
                        cmd.Parameters.AddWithValue("@C", Txtb_Search.Text);
                        cmd.ExecuteNonQuery();
                        DataTable tblDataSource = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(tblDataSource);
                            dataGridView1.DataSource = tblDataSource;
                        }
                        Size_Datagrid();
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //allows user to upload a profile photo
        private void Btn_ProfilePhotoUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (loggedinas != "")
                {
                    //allows user to select an image from any location which they wish to upload for the product image
                    string imageLocation = "";
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
                    };
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        imageLocation = dialog.FileName;
                        PicB_ProfilePhoto.ImageLocation = imageLocation;
                    }
                    Image imgProfile = Image.FromFile(imageLocation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //converts image user has uploaded to binary and inserts into database
        private void Btn_SubmitPhoto_Click(object sender, EventArgs e)
        {
            try
            {
                //selects the image and converts it to binary data to insert into database
                Image imgProfile = PicB_ProfilePhoto.Image;
                if (PicB_ProfilePhoto.Image != null & Txtb_Username != null & Txtb_Email.Text != null & Txtb_Password.Text != null & Txtb_FirstName.Text != null & Txtb_LastName.Text != null)
                {
                    byte[] data;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        imgProfile.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        data = ms.ToArray();
                    }
                    using (SqlConnection conn = new SqlConnection(connstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("update application.dbo.[User] SET Password = @A, FirstName = @B, LastName = @C, ProfilePhoto = @D WHERE UserID = @E", conn))
                        {
                            cmd.Parameters.AddWithValue("@A", Txtb_Password.Text);
                            cmd.Parameters.AddWithValue("@B", Txtb_FirstName.Text);
                            cmd.Parameters.AddWithValue("@C", Txtb_LastName.Text);
                            cmd.Parameters.AddWithValue("@D", data);
                            cmd.Parameters.AddWithValue("@E", Txtb_UserID.Text);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //when any cell is clicked on the datagrid a form will be displayed containing all the information for that particular product row that has been clicked on
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {

                    using (SqlCommand cmd = new SqlCommand("Select ProductID, ProductName, Available, Description, Location, ExpiryDate FROM application.dbo.Product", conn))
                    {
                        conn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                //creates the form
                                Form frmItemSelected = new Form();
                                //creates label array
                                Label[] lbl_Array = new Label[6];
                                for (int l = 0; l < lbl_Array.Count(); l++)
                                {
                                    lbl_Array[l] = new Label();
                                }
                                //each item in array is given a value based on datagrid headers
                                int j = 0;
                                foreach (Label lbl in lbl_Array)
                                {
                                    string lblname = "Lbl_" + (dr[j].ToString());
                                    lbl.Name = lblname;
                                    lbl.Location = new Point(5, 12 + (j * 28));
                                    lbl.Size = new Size(75, 15);
                                    lbl.Text = dataGridView1.Columns[j].HeaderCell.Value + ":".ToString();
                                    lbl.Visible = true;
                                    frmItemSelected.Controls.Add(lbl);
                                    j++;
                                }
                                //Comment section label
                                Label lblComment = new Label();
                                lblComment.Location = new Point(5, 12 + (6 * 28));
                                lblComment.Size = new Size(75, 15);
                                lblComment.Text = "Comment: ";
                                //All comments label location
                                //lblAllComments.Location = new Point(5, 12 + (9 * 28));
                                lstComments.Location = new Point(5, 12 + (9 * 28));
                                //declares visibility of global objects in this child form
                                btnComment.Visible = true;
                                lstComments.Visible = true;
                                txtbComment.Visible = true;
                                txtbComment.Text = "";
                                //creates textbox array
                                //defines product ID to load relevant data
                                prodselected = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                                TextBox[] txtb_Array = new TextBox[6];
                                for (int a = 0; a < txtb_Array.Count(); a++)
                                {
                                    txtb_Array[a] = new TextBox();
                                }
                                //each item in array is given a value received from query
                                int i = 0;
                                foreach (TextBox txtb in txtb_Array)
                                {
                                    string txtbname = "TxtB_" + (dr[i].ToString());
                                    txtb.Name = txtbname;
                                    txtb.Text = (dataGridView1.CurrentRow.Cells[i].Value.ToString());
                                    txtb.Location = new Point(80, 10 + (i * 28));
                                    txtb.ReadOnly = true;
                                    txtb.Size = new Size(325, 10);
                                    txtb.Visible = true;
                                    frmItemSelected.Controls.Add(txtb);
                                    i++;
                                }
                                //sets global variable to ID value
                                //creates picturebox, converts image from datagrid to bytes and displays it
                                PictureBox picB = new PictureBox();
                                MemoryStream ms = new MemoryStream((byte[])dataGridView1.CurrentRow.Cells[6].Value);
                                picB.Image = Image.FromStream(ms);
                                picB.SizeMode = PictureBoxSizeMode.StretchImage;
                                picB.Location = new Point(420, 5);
                                picB.Size = new Size(400, 400);
                                //adding all objects into form controls allows form to display these objects
                                frmItemSelected.Controls.Add(picB);
                                frmItemSelected.StartPosition = FormStartPosition.CenterParent;
                                frmItemSelected.Controls.Add(lblComment);
                                frmItemSelected.Controls.Add(btnComment);
                                frmItemSelected.Controls.Add(txtbComment);
                                frmItemSelected.Controls.Add(lstComments);
                                //frmItemSelected.Controls.Add(lblAllComments);
                                //form properties
                                frmItemSelected.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
                                frmItemSelected.Text = "UShare";
                                frmItemSelected.AutoSize = true;
                                frmItemSelected.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                                lstComments.Items.Clear();
                                Load_Comments();
                                frmItemSelected.ShowDialog();
                            }
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //sets visibility of objects when register link is clicked
        private void Link_Register_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Lbl_PageTitle.Text = "Register";
            Reset_RegisterForm();
            reg = true;
            add = false;
            inbox = false;
            home = false;
            prof = false;
            login = false;
            Visibility();
        }
        //sets visibility of objects when login link is clicked
        private void Link_Login_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Lbl_PageTitle.Text = "Login";
            Txtb_LoginUsername.ResetText();
            Txtb_LoginPassword.ResetText();
            login = true;
            reg = false;
            add = false;
            inbox = false;
            home = false;
            prof = false;
            //resets profile image stream
            if (Link_Login.Text == "Logout")
            {
                Link_Login.Text = "Login";
                Lbl_LoggedIn.Text = "";
                Link_Register.Visible = true;
                loggedinas = "";
                PicB_ProfilePhoto.Image = null;
            }
            Visibility();
        }
        //inserts user data into database if user successfully completes all fields to register
        private void Btn_Register_Click(object sender, EventArgs e)
        {
            try
            {
                if (Txtb_RegisterUsername.Text != null & Txtb_RegisterEmail.Text != null & Txtb_RegisterPassword.Text != null & Txtb_RegisterFirstName.Text != null & Txtb_RegisterLastName.Text != null & Txtb_RegisterEmail.Text.EndsWith("@my.shu.ac.uk") & Txtb_RegisterUsername.Text.Length == 8)
                {
                    using (SqlConnection conn = new SqlConnection(connstring))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("insert into application.dbo.[User] (Username, Email, Password, FirstName, LastName) VALUES (@A,@B,@C,@D,@E)", conn))
                        {
                            cmd.Parameters.AddWithValue("@A", Txtb_RegisterUsername.Text);
                            cmd.Parameters.AddWithValue("@B", Txtb_RegisterEmail.Text);
                            cmd.Parameters.AddWithValue("@C", Txtb_RegisterPassword.Text);
                            cmd.Parameters.AddWithValue("@D", Txtb_RegisterFirstName.Text);
                            cmd.Parameters.AddWithValue("@E", Txtb_RegisterLastName.Text);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        conn.Dispose();
                        Reset_RegisterForm();
                        reg = false;
                        login = true;
                        Visibility();
                        MessageBox.Show("Successfully registered. You may now login.");
                    }
                }
                else
                {
                    MessageBox.Show("Email must end with @my.shu.ac.uk! Username must be 8 characters!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please fill in all the relevant fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //takes user back to home if they click cancel on register page
        private void Btn_Cancel_Click_1(object sender, EventArgs e)
        {
            home = true;
            login = false;
            reg = false;
            add = false;
            inbox = false;
            prof = false;
            Visibility();
        }
        //logs user in if correct details are entered and displays a greeting at top of app containing username
        private void Btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("select Count(*) from application.dbo.[User] WHERE Username=@A and Password=@B", conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@A", Txtb_LoginUsername.Text);
                        cmd.Parameters.AddWithValue("@B", Txtb_LoginPassword.Text);
                        int result = (int)cmd.ExecuteScalar();
                        if (result > 0)
                        {
                            if (Link_Login.Text == "Login")
                            {
                                MessageBox.Show("Successful login!");
                                home = true;
                                Load_Profile();
                                Lbl_LoggedIn.Text = "Hi, " + Txtb_LoginUsername.Text;
                                loggedinas = Txtb_LoginUsername.Text;
                                Lbl_LoggedIn.Visible = true;
                                Visibility();
                                Link_Register.Visible = false;
                                Link_Login.Text = "Logout";
                                Txtb_LoginUsername.ResetText();
                                Txtb_LoginPassword.ResetText();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect login details! Please try again!");
                            Txtb_LoginUsername.ResetText();
                            Txtb_LoginPassword.ResetText();
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message);
            }
        }
        //takes user back to home if cancel button is clicked
        private void Btn_LoginCancel_Click(object sender, EventArgs e)
        {
            home = true;
            Txtb_LoginUsername.ResetText();
            Txtb_LoginPassword.ResetText();
            Visibility();
        }
    }
}