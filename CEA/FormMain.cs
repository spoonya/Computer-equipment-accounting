using System;
using System.Windows.Forms;
using System.Data.SQLite;
using Bunifu.Framework.UI;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace CEA
{
    public partial class FormMain : Form
    {
        private string conStr = @"Data Source=|DataDirectory|\ComputerEquipmentAccounting.db;Version=3";
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        private SQLiteDataReader reader;
        private List<int> CodeEquipForAllocation = new List<int>();
        private List<int> CodeStaffForAllocation = new List<int>();
        private List<int> CodeProviderForEquip = new List<int>();
        private List<int> CodeEquipForCancell = new List<int>();
        private string date = DateTime.Now.ToString("dd MMMM yyyy");
        private Image img = Image.FromFile("logo.png");
        private int curPage = 0;

        public FormMain()
        {
            InitializeComponent();
            EquipmentFill();
            EmployeeFill();
            AllocationFill();
            CancellationFill();
            ProvidersFill();
            ProvidersListFill();
        }

        private void OpenTrans()
        {
            if (pagesOptions.Visible == true && pagesOptions.SelectedIndex == curPage)
            {
                pagesOptions.Visible = false;
            }
            else
            {
                transOptions.ShowSync(pagesOptions);
            }
        }

        private void Reset()
        {
            Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel sp = new Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel();
            foreach (Control x in pagesOptions.TabPages[pagesOptions.SelectedIndex].Controls)
            {
                if (x is Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel)
                {
                    sp = (Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel)x;
                }
            }

            foreach (Control x in sp.Controls)
            {
                if (x is Bunifu.UI.WinForms.BunifuTextbox.BunifuTextBox)
                    ((Bunifu.UI.WinForms.BunifuTextbox.BunifuTextBox)x).Text = string.Empty;
                else if (x is Bunifu.UI.WinForms.BunifuDropdown)
                    ((Bunifu.UI.WinForms.BunifuDropdown)x).SelectedIndex = -1;
            }

            ddProviderEquip.Text = "Поставщик";
            ddUpdProviderEquip.Text = "Поставщик";
            ddNameEquipAlloc.Text = "Техника";
            ddUpdNameEquipAlloc.Text = "Техника";
            ddNameStaffAlloc.Text = "Сотрудник";
            ddUpdNameStaffAlloc.Text = "Сотрудник";
            ddNameEquipCancell.Text = "Техника";
            ddUpdNameEquipCancell.Text = "Техника";
        }

        private void EquipmentFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Equipment", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[10]);

                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                            data[data.Count - 1][3] = reader[3].ToString();
                            data[data.Count - 1][4] = reader[4].ToString();
                            data[data.Count - 1][5] = reader[5].ToString();
                            data[data.Count - 1][6] = reader[6].ToString();
                            data[data.Count - 1][8] = reader[7].ToString();
                            data[data.Count - 1][9] = Convert.ToString(
                                Convert.ToInt32(reader[2].ToString()) * Convert.ToDouble(reader[7].ToString()));
                        }
                    //Получение имени поставщика по коду
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (!(data[i][6]).Equals(""))
                            data[i][7] = SelectNameProviderFromCode(Convert.ToInt32(data[i][6]));
                        else data[i][7] = "Не установлен";
                    }
                    foreach (string[] s in data)
                        dgvEquip.Rows.Add(s);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void EmployeeFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Staff", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[4]);

                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                            data[data.Count - 1][3] = reader[3].ToString();
                        }

                    foreach (string[] s in data)
                        dgvEmployee.Rows.Add(s);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void AllocationFill()
        {
            using (con = new SQLiteConnection(conStr))
            {
                List<string[]> data = new List<string[]>();

                using (cmd = new SQLiteCommand("SELECT * FROM Allocation", con))
                {
                    try
                    {
                        con.Open();
                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[6]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][5] = reader[3].ToString();
                            }
                        //Получение имени работника и техники по коду                    
                        for (int i = 0; i < data.Count; i++)
                        {
                            data[i][3] = SelectNameEmployeeFromCode(Convert.ToInt32(data[i][2]));
                            data[i][4] = SelectNameEquipFromCode(Convert.ToInt32(data[i][1]));
                        }

                        foreach (string[] s in data)
                            dgvAllocation.Rows.Add(s);
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }
        }

        private void CancellationFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Cancellation", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[5]);

                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][3] = reader[2].ToString();
                            data[data.Count - 1][4] = reader[3].ToString();
                        }

                    //Получение названия техники по коду                    
                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i][2] = SelectNameEquipFromCode(Convert.ToInt32(data[i][1]));
                    }
                    foreach (string[] s in data)
                        dgvCancellation.Rows.Add(s);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void ProvidersFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Providers", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[5]);

                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                            data[data.Count - 1][3] = reader[3].ToString();
                            data[data.Count - 1][4] = reader[4].ToString();
                        }

                    foreach (string[] s in data)
                        dgvProviders.Rows.Add(s);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void StaffListFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameEmployee, CodeEmployee FROM Staff ORDER BY NameEmployee", con))
            {
                if (ddNameStaffAlloc.Items.Count > 0) ddNameStaffAlloc.Items.Clear();
                if (ddUpdNameStaffAlloc.Items.Count > 0) ddUpdNameStaffAlloc.Items.Clear();
                CodeStaffForAllocation.Clear();
                reader = null;

                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeStaffForAllocation.Add(new int { });
                            CodeStaffForAllocation[CodeStaffForAllocation.Count - 1] = Convert.ToInt32(reader[1].ToString());

                            ddNameStaffAlloc.Items.Add(reader[0].ToString());
                            ddUpdNameStaffAlloc.Items.Add(reader[0].ToString());
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void EquipListFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameEquip, CodeEquip FROM Equipment ORDER BY NameEquip", con))
            {
                int n = pages.SelectedIndex;

                try
                {
                    con.Open();

                    switch (n)
                    {
                        case 2:
                            if (ddNameEquipAlloc.Items.Count > 0) ddNameEquipAlloc.Items.Clear();
                            if (ddUpdNameEquipAlloc.Items.Count > 0) ddUpdNameEquipAlloc.Items.Clear();
                            CodeEquipForAllocation.Clear();
                            reader = null;

                            using (reader = cmd.ExecuteReader())
                                while (reader.Read())
                                {
                                    CodeEquipForAllocation.Add(new int { });
                                    CodeEquipForAllocation[CodeEquipForAllocation.Count - 1] = Convert.ToInt32(reader[1].ToString());
                                    ddNameEquipAlloc.Items.Add(reader[0].ToString());
                                    ddUpdNameEquipAlloc.Items.Add(reader[0].ToString());
                                }
                            break;
                        case 3:
                            if (ddNameEquipCancell.Items.Count > 0) ddNameEquipCancell.Items.Clear();
                            if (ddUpdNameEquipCancell.Items.Count > 0) ddUpdNameEquipCancell.Items.Clear();
                            CodeEquipForCancell.Clear();
                            reader = null;

                            using (reader = cmd.ExecuteReader())
                                while (reader.Read())
                                {
                                    CodeEquipForCancell.Add(new int { });
                                    CodeEquipForCancell[CodeEquipForCancell.Count - 1] = Convert.ToInt32(reader[1].ToString());
                                    ddNameEquipCancell.Items.Add(reader[0].ToString());
                                    ddUpdNameEquipCancell.Items.Add(reader[0].ToString());
                                }
                            break;
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void ProvidersListFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameProvider, CodeProvider FROM Providers ORDER BY NameProvider", con))
            {
                if (ddProviderEquip.Items.Count > 0) ddProviderEquip.Items.Clear();
                if (ddUpdProviderEquip.Items.Count > 0) ddUpdProviderEquip.Items.Clear();
                CodeProviderForEquip.Clear();
                reader = null;
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeProviderForEquip.Add(new int { });
                            CodeProviderForEquip[CodeProviderForEquip.Count - 1] = Convert.ToInt32(reader[1].ToString());
                            ddProviderEquip.Items.Add(reader[0].ToString());
                            ddUpdProviderEquip.Items.Add(reader[0].ToString());
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void SelectCountEquip(string NameEquip, Bunifu.UI.WinForms.BunifuTextbox.BunifuTextBox tb)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT CountFree FROM Equipment WHERE NameEquip = @name", con))
            {
                reader = null;
                cmd.Parameters.AddWithValue("@name", NameEquip);
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            tb.Text = reader[0].ToString();
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void ddNameEquipAlloc_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectCountEquip(ddNameEquipAlloc.Text, tbCountFreeEquipAlloc);
        }

        private void ddUpdNameEquipAlloc_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCountEquip(ddUpdNameEquipAlloc.Text, tbUpdCountFreeEquipAlloc);
        }

        private void InsertEquipment()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("INSERT into Equipment(NameEquip, Count, CountFree, DescriptionEquip," + 
                "DatePurchaseEquip, CodeProvider, Price)" +
                "VALUES (@name, @count, @countFree, @descrip, @date, @provider, @price); SELECT last_insert_rowid();", con))
            {
                if (ddProviderEquip.SelectedIndex != -1 && !tbNameEquip.Text.Equals("") && !tbCountEquip.Text.Equals("")
                    && !tbPriceEquip.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@name", tbNameEquip.Text);
                    if (tbCountEquip.Text.Equals("")) tbCountEquip.Text = "1";
                    cmd.Parameters.AddWithValue("@count", tbCountEquip.Text);
                    cmd.Parameters.AddWithValue("@countFree", tbCountEquip.Text);
                    cmd.Parameters.AddWithValue("@descrip", tbDescriptionEquip.Text);
                    DateTime date = Convert.ToDateTime(datePickEquip.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@provider", CodeProviderForEquip[ddProviderEquip.SelectedIndex]);
                    cmd.Parameters.AddWithValue("@price", tbPriceEquip.Text);

                    try
                    {
                        con.Open();

                        cmd.ExecuteNonQuery();
                        dgvEquip.Rows.Clear();
                        EquipmentFill();
                        Reset();
                        MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    catch (SQLiteException)
                    {
                        MessageBox.Show("Введите уникальное имя техники!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InsertStaff()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("INSERT into Staff(NameEmployee, TelephoneEmployee, PositionEmployee)" +
                "VALUES (@name, @phone, @position)", con))
            {
                if (!tbNameStaff.Text.Equals("") && !tbPhoneStaff.Text.Equals("") && !tbPositionStaff.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@name", tbNameStaff.Text);
                    cmd.Parameters.AddWithValue("@phone", tbPhoneStaff.Text);
                    cmd.Parameters.AddWithValue("@position", tbPositionStaff.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    dgvEmployee.Rows.Clear();
                    EmployeeFill();
                    Reset();
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void InsertAllocation()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("INSERT into Allocation(CodeEquip, CodeEmployee, Count)" +
                "VALUES (@codeEquip, @codeEmployee, @count)", con))
            {
                if (ddNameEquipAlloc.SelectedIndex != -1 && ddNameStaffAlloc.SelectedIndex != -1 && !tbCountEquipAlloc.Text.Equals(""))
                {
                    if (Convert.ToInt32(tbCountEquipAlloc.Text) <= Convert.ToInt32(tbCountFreeEquipAlloc.Text)
                        && Convert.ToInt32(tbCountEquipAlloc.Text) > 0)
                    {
                        cmd.Parameters.AddWithValue("@codeEquip", CodeEquipForAllocation[ddNameEquipAlloc.SelectedIndex]);
                        cmd.Parameters.AddWithValue("@codeEmployee", CodeStaffForAllocation[ddNameStaffAlloc.SelectedIndex]);
                        cmd.Parameters.AddWithValue("@count", tbCountEquipAlloc.Text);

                        try
                        {
                            con.Open();

                            cmd.ExecuteNonQuery();
                            dgvAllocation.Rows.Clear();
                            AllocationFill();

                            dgvEquip.Rows.Clear();
                            Reset();
                            EquipmentFill();
                            MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

                        }
                        catch (SQLiteException)
                        {
                            throw;
                        }
                    }
                    else
                        MessageBox.Show("Недопустимое количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InsertCancellation()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT CountFree FROM Equipment WHERE CodeEquip = @codeEquip", con))
            {
                if (ddNameEquipCancell.SelectedIndex != -1 && !tbCountCancell.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@codeEquip", CodeEquipForCancell[ddNameEquipCancell.SelectedIndex]);
                    cmd.Parameters.AddWithValue("@reason", tbReasonCancell.Text);
                    cmd.Parameters.AddWithValue("@count", tbCountCancell.Text);

                    con.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    int count = Convert.ToInt32(reader[0].ToString());
                    reader.Close();

                    if (count >= Convert.ToInt32(tbCountCancell.Text) && Convert.ToInt32(tbCountCancell.Text) > 0)
                    {
                        cmd.CommandText = "INSERT into Cancellation(CodeEquip, Count, Reason) VALUES (@codeEquip, @count, @reason)";

                        try
                        {
                            cmd.ExecuteNonQuery();
                            dgvCancellation.Rows.Clear();
                            CancellationFill();

                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            Reset();
                            MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        }
                        catch (SQLiteException)
                        {
                            throw;
                        }
                    }
                    else MessageBox.Show("Недопустимое количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InsertProvider()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("INSERT into Providers(NameProvider, TelephoneProvider, City, Adress)" +
                "VALUES (@name, @phone, @city, @adress)", con))
            {
                cmd.Parameters.AddWithValue("@name", tbNameProvider.Text);
                cmd.Parameters.AddWithValue("@phone", tbPhoneProvider.Text);
                cmd.Parameters.AddWithValue("@city", tbCityProvider.Text);
                cmd.Parameters.AddWithValue("@adress", tbAdressProvider.Text);

                try
                {
                    con.Open();

                    if (!tbNameProvider.Text.Equals("") && !tbPhoneProvider.Text.Equals("") && !tbCityProvider.Text.Equals("")
                        && !tbAdressProvider.Text.Equals(""))
                    {
                        cmd.ExecuteNonQuery();
                        dgvProviders.Rows.Clear();
                        ProvidersFill();
                        Reset();
                        MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    else
                        MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private string SelectNameEmployeeFromCode(int code)
        {
            string name = null;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameEmployee FROM Staff WHERE CodeEmployee = @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private string SelectNameEquipFromCode(int code)
        {
            string name = null;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameEquip FROM Equipment WHERE CodeEquip = @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private string SelectNameProviderFromCode(int code)
        {
            string name = null;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT NameProvider FROM Providers WHERE CodeProvider = @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void transColorButton_Click(object sender, EventArgs e)
        {
            ((BunifuFlatButton)sender).Visible = false;
            transColorBtn.ShowSync(((BunifuFlatButton)sender));
        }

        private void button_Click(object sender, EventArgs e)
        {
            pnlSelector.Visible = false;
            pnlSelector.Top = ((Control)sender).Top;
            pnlSelector.Height = ((Control)sender).Height;
            transButton.ShowSync(pnlSelector);

            pagesOptions.Visible = false;
        }

        private void btnEquip_Click(object sender, EventArgs e)
        {
            ProvidersListFill();
            tbSearch.Clear();
            button_Click(sender, e);
            pages.SetPage("Техника");
            menuEquip.Enabled = true;
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            tbSearch.Clear();
            pages.SetPage("Работники");
            menuEquip.Enabled = true;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            StaffListFill();

            button_Click(sender, e);
            tbSearch.Clear();
            pages.SetPage("Распределение");
            menuEquip.Enabled = true;
            EquipListFill();
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            tbSearch.Clear();
            pages.SetPage("Списание");
            menuEquip.Enabled = true;
            EquipListFill();
        }

        private void btnProviders_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            tbSearch.Clear();
            pages.SetPage("Поставщики");
            menuEquip.Enabled = true;
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            tbSearch.Clear();
            pages.SetPage("Отчёты");
            menuEquip.Enabled = false;
        }

        private void insertRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: pagesOptions.SetPage("Добавить технику"); break;
                case 1: pagesOptions.SetPage("Добавить работника"); break;
                case 2: pagesOptions.SetPage("Добавить распределение"); break;
                case 3: pagesOptions.SetPage("Добавить списание"); break;
                case 4: pagesOptions.SetPage("Добавить поставщика"); break;
            }

            OpenTrans();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pagesOptions.Visible = false;
        }

        private void btnAddEquip_Click(object sender, EventArgs e)
        {
            InsertEquipment();
            transColorButton_Click(sender, e);
        }

        private void deleteRecord_Click(object sender, EventArgs e)
        {
            bool success = false;
            if (MessageBox.Show("Удалить запись(и)?", "Удаление", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                int n = pages.SelectedIndex;
                switch (n)
                {
                    case 0:
                        if (dgvEquip.RowCount > 0)
                        {
                            DeleteEquip(DeleteRows(dgvEquip));
                            dgvAllocation.Rows.Clear();
                            AllocationFill();
                            dgvCancellation.Rows.Clear();
                            CancellationFill();
                            success = true;
                        }
                        break;
                    case 1:
                        if (dgvEmployee.RowCount > 0)
                        {
                            DeleteStaff(DeleteRows(dgvEmployee));
                            dgvAllocation.Rows.Clear();
                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            AllocationFill();
                            success = true;
                        }
                        break;
                    case 2:
                        if (dgvAllocation.RowCount > 0)
                        {
                            DeleteAllocation(DeleteRows(dgvAllocation));
                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            success = true;
                        }
                        break;
                    case 3:
                        if (dgvCancellation.RowCount > 0)
                        {
                            DeleteCancellation(DeleteRows(dgvCancellation));
                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            success = true;
                        }
                        break;
                    case 4:
                        if (dgvProviders.RowCount > 0)
                        {
                            DeleteProvider(DeleteRows(dgvProviders));
                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            success = true;
                        }
                        break;
                }
                if (success)
                    MessageBox.Show("Удаление успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                else
                    MessageBox.Show("Строка не выбрана", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                success = false;
            }
        }

        private int[] DeleteRows(DataGridView dgv)
        {
            int[] deletedRows = new int[dgv.SelectedRows.Count];
            int i = 0;

            foreach (DataGridViewRow item in dgv.SelectedRows)
            {
                deletedRows[i++] = Convert.ToInt32(dgv[0, item.Index].Value.ToString());
                dgv.Rows.RemoveAt(item.Index);
            }
            return deletedRows;
        }

        private void DeleteEquip(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("PRAGMA foreign_keys = ON; DELETE FROM Equipment WHERE CodeEquip = @code ", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void DeleteStaff(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("PRAGMA foreign_keys = ON; DELETE FROM Staff WHERE CodeEmployee = @code;", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void DeleteAllocation(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("PRAGMA foreign_keys = ON; DELETE FROM Allocation WHERE CodeAllocation = @code;", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void DeleteCancellation(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("DELETE FROM Cancellation WHERE CodeCancell = @code;", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void DeleteProvider(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("PRAGMA foreign_keys = ON; DELETE FROM Providers WHERE CodeProvider = @code;", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void updateRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0:
                    pagesOptions.SetPage("Редактировать технику"); break;
                case 1:
                    pagesOptions.SetPage("Редактировать работника"); break;
                case 2:
                    pagesOptions.SetPage("Редактировать распределение"); break;
                case 3:
                    pagesOptions.SetPage("Редактировать списание"); break;
                case 4:
                    pagesOptions.SetPage("Редактировать поставщика"); break;
            }
            OpenTrans();
        }

        private void UpdateEquip(int code, int oldCount)
        {
            int curRow = 0;
            if (dgvEquip.SelectedRows.Count > 0)
                curRow = dgvEquip.SelectedRows[0].Index;

            int count = Convert.ToInt32(tbUpdCountEquip.Text);
            int countFree = oldCount - Convert.ToInt32(dgvEquip[3, curRow].Value.ToString());
            int countFreeNew = count - countFree;

            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("UPDATE Equipment SET NameEquip = @name, Count = @count, CountFree = @countFree, " +
                "DescriptionEquip = @descrip, DatePurchaseEquip = @date, CodeProvider = @provider, Price = @price " +  
                "WHERE CodeEquip = @code", con))
            {
                if (!tbUpdNameEquip.Text.Equals("") && !tbUpdCountEquip.Text.Equals("") && !tbUpdPriceEquip.Text.Equals(""))
                {
                    if (countFreeNew > 0)
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@name", tbUpdNameEquip.Text);
                        if (tbUpdCountEquip.Text.Equals("")) tbUpdCountEquip.Text = "1";
                        cmd.Parameters.AddWithValue("@count", tbUpdCountEquip.Text);
                        cmd.Parameters.AddWithValue("@countFree", countFreeNew.ToString());
                        cmd.Parameters.AddWithValue("@descrip", tbUpdDescripEquip.Text);
                        DateTime date = Convert.ToDateTime(datePickUpdEquip.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@price", tbUpdPriceEquip.Text);
                        if (ddUpdProviderEquip.SelectedIndex != -1)
                            cmd.Parameters.AddWithValue("@provider", CodeProviderForEquip[ddUpdProviderEquip.SelectedIndex]);
                        else if (!dgvEquip[6, curRow].Value.ToString().Equals(""))
                            cmd.Parameters.AddWithValue("@provider", dgvEquip[6, curRow].Value.ToString());
                        else
                            cmd.Parameters.AddWithValue("@provider", null);

                        try
                        {
                            con.Open();

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            dgvEquip.Rows.Clear();

                            EquipmentFill();

                            dgvEquip.ClearSelection();
                            dgvEquip.Rows[curRow].Selected = true;
                            dgvEquip.CurrentCell = dgvEquip[3, curRow];

                            MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                                MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        }
                        catch (SQLiteException)
                        {
                            MessageBox.Show("Введите уникальное имя техники!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        MessageBox.Show("Недопустимое количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateAllocation(int code, int oldCount)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("UPDATE Allocation SET CodeEquip = @codeEquip, CodeEmployee = @codeEmp, Count = @count " +
                "WHERE CodeAllocation = @code", con))
            {
                int curRow = 0;

                if (dgvAllocation.SelectedRows.Count > 0)
                    curRow = dgvAllocation.SelectedRows[0].Index;

                if (!tbUpdCountEquipAlloc.Text.Equals(""))
                {
                    if (Convert.ToInt32(tbUpdCountEquipAlloc.Text) <= Convert.ToInt32(tbUpdCountFreeEquipAlloc.Text) + oldCount
                        && Convert.ToInt32(tbUpdCountEquipAlloc.Text) > 0)
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        if (ddUpdNameEquipAlloc.SelectedIndex != -1)
                            cmd.Parameters.AddWithValue("@codeEquip", CodeEquipForAllocation[ddUpdNameEquipAlloc.SelectedIndex]);
                        else
                            cmd.Parameters.AddWithValue("@codeEquip", dgvAllocation[1, curRow].Value.ToString());
                        if (ddUpdNameStaffAlloc.SelectedIndex != -1)
                            cmd.Parameters.AddWithValue("@codeEmp", CodeStaffForAllocation[ddUpdNameStaffAlloc.SelectedIndex]);
                        else
                            cmd.Parameters.AddWithValue("@codeEmp", dgvAllocation[2, curRow].Value.ToString());
                        cmd.Parameters.AddWithValue("@count", tbUpdCountEquipAlloc.Text);

                        try
                        {
                            con.Open();

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            dgvAllocation.Rows.Clear();
                            AllocationFill();

                            dgvAllocation.ClearSelection();
                            dgvAllocation.Rows[curRow].Selected = true;
                            dgvAllocation.CurrentCell = dgvAllocation[3, curRow];

                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        }
                        catch (SQLiteException)
                        {
                            throw;
                        }
                    }
                    else MessageBox.Show("Недопустимое количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateCancellation(int code, int oldCount)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT CountFree FROM Equipment WHERE CodeEquip = @codeEquip", con))
            {
                int prev = Convert.ToInt32(tbUpdCountCancell.Text);

                int curRow = 0;
                if (dgvCancellation.SelectedRows.Count > 0)
                    curRow = dgvCancellation.SelectedRows[0].Index;

                if (!tbUpdCountCancell.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    if (ddUpdNameEquipCancell.SelectedIndex != -1)
                        cmd.Parameters.AddWithValue("@codeEquip", CodeEquipForCancell[ddUpdNameEquipCancell.SelectedIndex]);
                    else
                        cmd.Parameters.AddWithValue("@codeEquip", dgvCancellation[1, curRow].Value.ToString());

                    cmd.Parameters.AddWithValue("@count", tbUpdCountCancell.Text);
                    cmd.Parameters.AddWithValue("@reason", tbUpdReasonCancell.Text);

                    con.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    int count = Convert.ToInt32(reader[0].ToString());
                    reader.Close();

                    if (count + oldCount >= Convert.ToInt32(tbUpdCountCancell.Text) && Convert.ToInt32(tbUpdCountCancell.Text) > 0)
                    {
                        cmd.CommandText = "UPDATE Cancellation SET CodeEquip = @codeEquip, Count = @count, Reason = @reason " +
                       "WHERE CodeCancell = @code";

                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            dgvCancellation.Rows.Clear();
                            CancellationFill();

                            dgvCancellation.ClearSelection();
                            dgvCancellation.Rows[curRow].Selected = true;
                            dgvCancellation.CurrentCell = dgvCancellation[3, curRow];

                            dgvEquip.Rows.Clear();
                            EquipmentFill();
                            MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        }
                        catch (SQLiteException)
                        {
                            throw;
                        }
                    }
                    else
                        MessageBox.Show("Недопустимое количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateProvider(int code)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("UPDATE Providers SET NameProvider = @name, TelephoneProvider = @phone, " +
                "City = @city, Adress = @adress WHERE CodeProvider = @code", con))
            {
                int curRow = 0;
                if (dgvProviders.SelectedRows.Count > 0)
                    curRow = dgvProviders.SelectedRows[0].Index;

                try
                {
                    if (!tbUpdNameProvider.Text.Equals("") && !tbUpdPhoneProvider.Text.Equals("") && !tbUpdCityProvider.Text.Equals("")
                        && !tbUpdAdressProvider.Text.Equals(""))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@name", tbUpdNameProvider.Text);
                        cmd.Parameters.AddWithValue("@phone", tbUpdPhoneProvider.Text);
                        cmd.Parameters.AddWithValue("@city", tbUpdCityProvider.Text);
                        cmd.Parameters.AddWithValue("@adress", tbUpdAdressProvider.Text);

                        con.Open();

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        dgvProviders.Rows.Clear();
                        ProvidersFill();

                        dgvProviders.ClearSelection();
                        dgvProviders.Rows[curRow].Selected = true;
                        dgvProviders.CurrentCell = dgvProviders[3, curRow];
                        MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    else
                        MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void UpdateStaff(int code)
        {
            int curRow = 0;
            if (dgvEmployee.SelectedRows.Count > 0)
                curRow = dgvEmployee.SelectedRows[0].Index;

            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("UPDATE Staff SET NameEmployee = @name, TelephoneEmployee = @phone, PositionEmployee = @position " +
                "WHERE CodeEmployee = @code", con))
            {
                if (!tbUpdNameStaff.Text.Equals("") && !tbUpdPhoneStaff.Text.Equals("") && !tbUpdPositionStaff.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", tbUpdNameStaff.Text);
                    cmd.Parameters.AddWithValue("@phone", tbUpdPhoneStaff.Text);
                    cmd.Parameters.AddWithValue("@position", tbUpdPositionStaff.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    dgvEmployee.Rows.Clear();
                    EmployeeFill();
                    dgvEmployee.ClearSelection();
                    dgvEmployee.Rows[curRow].Selected = true;
                    dgvEmployee.CurrentCell = dgvEmployee[3, curRow];

                    MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgvEquip_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvEquip.SelectedRows.Count > 0)
                curRow = dgvEquip.SelectedRows[0].Index;

            tbUpdNameEquip.Text = dgvEquip[1, curRow].Value.ToString();
            tbUpdCountEquip.Text = dgvEquip[2, curRow].Value.ToString();
            tbUpdDescripEquip.Text = dgvEquip[4, curRow].Value.ToString();
            datePickUpdEquip.Text = dgvEquip[5, curRow].Value.ToString();
            ddUpdProviderEquip.Text = dgvEquip[7, curRow].Value.ToString();
            tbUpdPriceEquip.Text = dgvEquip[8, curRow].Value.ToString();
        }

        private void dgvEmployee_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvEmployee.SelectedRows.Count > 0)
                curRow = dgvEmployee.SelectedRows[0].Index;

            tbUpdNameStaff.Text = dgvEmployee[1, curRow].Value.ToString();
            tbUpdPhoneStaff.Text = dgvEmployee[2, curRow].Value.ToString();
            tbUpdPositionStaff.Text = dgvEmployee[3, curRow].Value.ToString();
        }

        private void dgvAllocation_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvProviders_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvProviders.SelectedRows.Count > 0)
                curRow = dgvProviders.SelectedRows[0].Index;

            tbUpdNameProvider.Text = dgvProviders[1, curRow].Value.ToString();
            tbUpdPhoneProvider.Text = dgvProviders[2, curRow].Value.ToString();
            tbUpdCityProvider.Text = dgvProviders[3, curRow].Value.ToString();
            tbUpdAdressProvider.Text = dgvProviders[4, curRow].Value.ToString();
        }

        private void dgvCancellation_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvCancellation.SelectedRows.Count > 0)
                curRow = dgvCancellation.SelectedRows[0].Index;

            ddUpdNameEquipCancell.Text = dgvCancellation[2, curRow].Value.ToString();
            tbUpdCountCancell.Text = dgvCancellation[3, curRow].Value.ToString();
            tbUpdReasonCancell.Text = dgvCancellation[4, curRow].Value.ToString();
        }

        private void btnUpdateEquip_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            int curRow = 0;

            if (dgvEquip.RowCount > 0 && dgvEquip.SelectedRows.Count > 0)
            {
                curRow = dgvEquip.SelectedRows[0].Index;
                UpdateEquip(Convert.ToInt32(dgvEquip[0, curRow].Value.ToString()),
                    Convert.ToInt32(dgvEquip[2, curRow].Value.ToString()));

                dgvAllocation.Rows.Clear();
                AllocationFill();
                dgvCancellation.Rows.Clear();
                CancellationFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnUpdStaff_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvEmployee.RowCount > 0 && dgvEmployee.SelectedRows.Count > 0)
            {
                curRow = dgvEmployee.SelectedRows[0].Index;

                UpdateStaff(Convert.ToInt32(dgvEmployee[0, curRow].Value.ToString()));

                dgvAllocation.Rows.Clear();
                AllocationFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnUpdProvider_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            int curRow = 0;

            if (dgvProviders.RowCount > 0 && dgvProviders.SelectedRows.Count > 0)
            {
                curRow = dgvProviders.SelectedRows[0].Index;

                UpdateProvider(Convert.ToInt32(dgvProviders[0, curRow].Value.ToString()));

                dgvEquip.Rows.Clear();
                EquipmentFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnUpdAlloc_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            int curRow = 0;

            if (dgvAllocation.RowCount > 0 && dgvAllocation.SelectedRows.Count > 0)
            {
                curRow = dgvAllocation.SelectedRows[0].Index;
                int oldCount = Convert.ToInt32(dgvAllocation[5, curRow].Value.ToString());
                UpdateAllocation(Convert.ToInt32(dgvAllocation[0, curRow].Value.ToString()), oldCount);
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void btnUpdCancell_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            int curRow = 0;

            if (dgvCancellation.RowCount > 0 && dgvCancellation.SelectedRows.Count > 0)
            {
                curRow = dgvCancellation.SelectedRows[0].Index;
                int oldCount = Convert.ToInt32(dgvCancellation[3, curRow].Value.ToString());
                UpdateCancellation(Convert.ToInt32(dgvCancellation[0, curRow].Value.ToString()), oldCount);
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void searchRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;

            pagesOptions.SetPage("Поиск");

            OpenTrans();
        }

        private void SearchEquip()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Equipment WHERE " +
                "((CodeEquip in (SELECT CodeEquip FROM Equipment WHERE NameEquip LIKE @name) " +
                "OR(CodeProvider in (SELECT CodeProvider FROM Providers WHERE NameProvider LIKE @name))))", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

                try
                {
                    dgvEquip.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        EquipmentFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[8]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][3] = reader[3].ToString();
                                data[data.Count - 1][4] = reader[4].ToString();
                                data[data.Count - 1][5] = reader[5].ToString();
                                data[data.Count - 1][6] = reader[6].ToString();
                            }
                        //Получение имени поставщика по коду
                        for (int i = 0; i < data.Count; i++)
                        {
                            data[i][7] = SelectNameProviderFromCode(Convert.ToInt32(data[i][6]));
                        }
                        foreach (string[] s in data)
                            dgvEquip.Rows.Add(s);
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void SearchStaff()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Staff WHERE NameEmployee LIKE @name", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

                try
                {
                    dgvEmployee.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        EmployeeFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[4]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][3] = reader[3].ToString();
                            }
                        foreach (string[] s in data)
                            dgvEmployee.Rows.Add(s);
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void SearchProvider()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Providers WHERE NameProvider LIKE @name", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

                try
                {
                    dgvProviders.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        ProvidersFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[5]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][3] = reader[3].ToString();
                                data[data.Count - 1][4] = reader[4].ToString();
                            }
                        foreach (string[] s in data)
                            dgvProviders.Rows.Add(s);
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void SearchAllocation()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Allocation WHERE " +
                "((CodeEquip in (SELECT CodeEquip FROM Equipment WHERE NameEquip LIKE @name) " +
                "OR (CodeEmployee in (SELECT CodeEmployee FROM Staff WHERE NameEmployee LIKE @name))))", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

                try
                {
                    dgvAllocation.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        AllocationFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[6]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][5] = reader[3].ToString();
                            }
                        //Получение имени работника и техники по коду                    
                        for (int i = 0; i < data.Count; i++)
                        {
                            data[i][3] = SelectNameEmployeeFromCode(Convert.ToInt32(data[i][2]));
                            data[i][4] = SelectNameEquipFromCode(Convert.ToInt32(data[i][1]));
                        }

                        foreach (string[] s in data)
                            dgvAllocation.Rows.Add(s);
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void SearchCancellation()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Cancellation WHERE " +
                "((CodeEquip in (SELECT CodeEquip FROM Equipment WHERE NameEquip LIKE @name) " +
                "OR (Reason LIKE @name)))", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

                try
                {
                    dgvCancellation.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        CancellationFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[5]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][3] = reader[2].ToString();
                                data[data.Count - 1][4] = reader[3].ToString();
                            }

                        //Получение названия техники по коду                    
                        for (int i = 0; i < data.Count; i++)
                        {
                            data[i][2] = SelectNameEquipFromCode(Convert.ToInt32(data[i][1]));
                        }

                        foreach (string[] s in data)
                            dgvCancellation.Rows.Add(s);
                    }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void tbSearch_TextChange(object sender, EventArgs e)
        {
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: SearchEquip(); break;
                case 1: SearchStaff(); break;
                case 2: SearchAllocation(); break;
                case 3: SearchCancellation(); break;
                case 4: SearchProvider(); break;
            }
        }

        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            InsertStaff();
        }

        private void btnAddProvider_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            InsertProvider();
        }

        private void btnAllocAdd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            InsertAllocation();
        }

        private void btnAddCancell_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            InsertCancellation();
        }

        private void dgvAllocation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvAllocation.SelectedRows.Count > 0)
                curRow = dgvAllocation.SelectedRows[0].Index;

            ddUpdNameStaffAlloc.Text = dgvAllocation[3, curRow].Value.ToString();
            ddUpdNameEquipAlloc.Text = dgvAllocation[4, curRow].Value.ToString();
            tbUpdCountEquipAlloc.Text = dgvAllocation[5, curRow].Value.ToString();

            SelectCountEquip(ddUpdNameEquipAlloc.Text, tbUpdCountFreeEquipAlloc);
        }

        private void btnAllocReport1_Click(object sender, EventArgs e)
        {
            easyHTMLReport.Clear();
            easyHTMLReport.AddImage(img, "width = 15%, style = 'float: right'");
            easyHTMLReport.AddString("<h1>Организация</h1>");
            easyHTMLReport.AddString("<h2>Отчёт о распределённой технике</h2>");
            easyHTMLReport.AddString("<h3>Дата отчёта: " + date + "</h3>");

            int count = 0;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT sum(Count) FROM Allocation", con))
            {
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }
                easyHTMLReport.AddLineBreak();
                easyHTMLReport.AddDatagridView(dgvAllocation);
                easyHTMLReport.AddLineBreak();
                easyHTMLReport.AddString("Количество распределённой техники: " + count + "<br>");
                easyHTMLReport.ShowPrintPreviewDialog();
            }
        }

        private void btnCancellReport1_Click(object sender, EventArgs e)
        {
            easyHTMLReport.Clear();
            easyHTMLReport.AddImage(img, "width = 15%, style = 'float: right'");
            easyHTMLReport.AddString("<h1>Организация</h1>");
            easyHTMLReport.AddString("<h2>Отчёт о списанной технике</h2>");
            easyHTMLReport.AddString("<h3>Дата отчёта: " + date + "</h3>");

            int count = 0;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT sum(Count) FROM Cancellation", con))
            {
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }

                easyHTMLReport.AddLineBreak();
                easyHTMLReport.AddDatagridView(dgvCancellation);
                easyHTMLReport.AddLineBreak();
                easyHTMLReport.AddString("Количество списанной техники: " + count + "<br>");
                easyHTMLReport.ShowPrintPreviewDialog();
            }
        }

        private void btnProvidersReport1_Click(object sender, EventArgs e)
        {
            easyHTMLReport.Clear();
            easyHTMLReport.AddImage(img, "width = 15%, style = 'float: right'");
            easyHTMLReport.AddString("<h1>Организация</h1>");
            easyHTMLReport.AddString("<h2>Отчёт о поставщиках</h2>");
            easyHTMLReport.AddString("<h3>Дата отчёта: " + date + "</h3>");
            int count = 0;
            string name = null;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT P.NameProvider, count(E.CodeProvider) FROM Providers as P, Equipment as E " + 
                "WHERE P.CodeProvider = E.CodeProvider GROUP BY E.CodeProvider", con))
            {
                try
                {
                    easyHTMLReport.AddLineBreak();
                    easyHTMLReport.AddDatagridView(dgvProviders);
                    easyHTMLReport.AddLineBreak();

                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[1].ToString());
                            name = reader[0].ToString();
                            easyHTMLReport.AddString("Количество поставок от \"" + name + "\": " + count + "<br>");
                        }
                          
                    easyHTMLReport.ShowPrintPreviewDialog();
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void btnEquipReport1_Click(object sender, EventArgs e)
        {
            easyHTMLReport.Clear();
            easyHTMLReport.AddImage(img, "width = 15%, style = 'float: right'");
            easyHTMLReport.AddString("<h1>Организация</h1>");
            easyHTMLReport.AddString("<h2>Отчёт о технике</h2>");
            easyHTMLReport.AddString("<h3>Дата отчёта: " + date + "</h3>");
            int count = 0, sum = 0, totalPrice = 0;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT count(CodeEquip) FROM Equipment", con))
            {
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }

                    cmd.CommandText = "SELECT sum(Count) FROM Equipment";
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            sum = Convert.ToInt32(reader[0].ToString());
                        }

                    cmd.CommandText = "SELECT Count, Price FROM Equipment";
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            totalPrice += Convert.ToInt32(reader[0].ToString()) * Convert.ToInt32(reader[1].ToString());
                        }
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }

            easyHTMLReport.AddLineBreak();
            easyHTMLReport.AddDatagridView(dgvEquip);
            easyHTMLReport.AddLineBreak();
            easyHTMLReport.AddString("Всего наименований: " + count + "<br>");
            easyHTMLReport.AddString("Всего техники: " + sum + "<br>");
            easyHTMLReport.AddString("Общая стоимость: " + totalPrice + " руб.<br>");
            easyHTMLReport.ShowPrintPreviewDialog();
        }

        private void btnStaffReport1_Click_1(object sender, EventArgs e)
        {
            easyHTMLReport.Clear();
            easyHTMLReport.AddImage(img, "width = 15%, style = 'float: right'");
            easyHTMLReport.AddString("<h1>Организация</h1>");
            easyHTMLReport.AddString("<h2>Отчёт о сотрудниках</h2>");
            easyHTMLReport.AddString("<h3>Дата отчёта: " + date + "</h3>");
            easyHTMLReport.AddDatagridView(dgvEmployee);
            easyHTMLReport.AddLineBreak();
            int count = 0;
            string name = null;
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT S.NameEmployee, sum(A.Count) FROM Staff as S, Allocation as A " +
                "WHERE S.CodeEmployee = A.CodeEmployee GROUP BY A.CodeEmployee", con))
            {
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[1].ToString());
                            name = reader[0].ToString();
                            easyHTMLReport.AddString("Количество техники у \"" + name + "\": " + count + "<br>");
                        }

                    easyHTMLReport.ShowPrintPreviewDialog();
                }
                catch (SQLiteException)
                {
                    throw;
                }          
                easyHTMLReport.ShowPrintPreviewDialog();
            }
        }

        private void btnMinMenu_Click(object sender, EventArgs e)
        {   
            pnlMenu.Visible = false;
            logo.Visible = false;
            pnlMenu.Width = 53;
            transMenu.ShowSync(pnlMenu);
            btnMaxMenu.Visible = true;
            btnMinMenu.Visible = false;
        }

        private void btnMaxMenu_Click(object sender, EventArgs e)
        {
            btnMaxMenu.Visible = false;
            pnlMenu.Visible = false;
            logo.Visible = true;
            pnlMenu.Width = 192;
            transMenu.ShowSync(pnlMenu);
            btnMinMenu.Visible = true;
        }
    }
}
