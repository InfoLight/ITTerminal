﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace ITTerminal
{
    partial class ExchangeMenu : TerminalForm
    {
        private User user;
        private User admin;
        private CardReader cardReader;
        private Equipment oldEquipment;
        private Equipment newEquipment;
        private BarcodeReader barcodeReader;
        private static string status = ConfigurationManager.ConnectionStrings["status"].ConnectionString;


        public ExchangeMenu()
        {
            InitializeComponent();
            ////////////////////////
            AdminCardPanel.BackColor = Color.FromArgb(150, 255, 255, 255);
            OldEquipmentPanel.BackColor = Color.FromArgb(150, 255, 255, 255);
            NewEquipmentPanel.BackColor = Color.FromArgb(150, 255, 255, 255);
            ////////////////////////
            CloseButton.BackColor = Color.FromArgb(130, 255, 255, 255);
            CloseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 255, 255, 255);
            SubmitButton.BackColor = Color.FromArgb(130, 255, 255, 255);
            SubmitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 255, 255, 255);
            ///////////////////////
        }

        private void ExchangeMenu_Load(object sender, EventArgs e)
        {
            GeneralPanel_Resize(null, null);
            cardReader = new CardReader();
            cardReader.Read(CardId);
            barcodeReader = new BarcodeReader(this);
            barcodeReader.Read(EquipmentId);
        }

        public override void EquipmentId(string id)
        {
            if (oldEquipment == null)
            {
                barcodeReader.Read(EquipmentId);
                oldEquipment = Connector1C.doesSomeoneHasEquipment(id);
                if (oldEquipment == null)
                {
                    showMessage("Equipment is not found / Оборудование не найдено");

                    return;
                }
                NewEquipmentPanel.Enabled = true;
                user = Connector1C.equipmentPlace(id);
                waitingOldEquipmentLabel.Visible = false;
                waitingOldEquipmentLabelRus.Visible = false;
                OldEquipmentPanel.BackgroundImage = Properties.Resources.tick;
                label1.Visible = true;
                label1.Text = "User (Пользователь): " + user.Name + "\nOld equipment (Старое оборудование): " + oldEquipment.Name;
            }
            else
            {
                newEquipment = Connector1C.getEquipmentByID(id);
                if (newEquipment == null)
                {
                    barcodeReader.Read(EquipmentId);
                    showMessage("Equipment is not found / Оборудование не найдено");
                    return;
                }
                waitingNewEquipmentLabel.Visible = false;
                waitingNewEquipmentLabelRus.Visible = false;
                NewEquipmentPanel.BackgroundImage = Properties.Resources.tick;
                label2.Visible = true;
                label2.Text = "New equipment (Новое оборудование): " + newEquipment.Name;
            }
        }

        public override void CardId(string id)
        {
            Action a = () =>
            {
                if (admin == null)
                {
                    admin = CardManager.getUser(id);
                    if (admin == null || admin.Position != status)
                    {
                        admin = null;
                        cardReader.Read(CardId);
                        showMessage("Admin is not found / Администратор не найден");
                        return;
                    }
                    waititngCardLabel.Visible = false;
                    waititngCardLabelRus.Visible = false;
                    AdminCardPanel.BackgroundImage = Properties.Resources.tick;
                    AdminCardPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    label3.Visible = true;
                    label3.Text = "Admin (Администратор): " + admin.Name;
                }
            };
            BeginInvoke(a);
        }

        private void GeneralPanel_Resize(object sender, EventArgs e)
        {
            //Panels
            GeneralPanel.Location = new Point(0, 70);
            GeneralPanel.Size = new Size(this.Size.Width, this.Size.Height - 70);
            Header.Location = new Point(0, 10);
            Header.Size = new Size(this.Size.Width, 60);
            //////////////////////////
            int width = GeneralPanel.Width * 40 / 100;
            int height = GeneralPanel.Height * 40 / 100;
            int widthI = GeneralPanel.Width * 5 / 100;
            int heightI = GeneralPanel.Height * 5 / 100;

            MessageLabel.Location = new Point(widthI * 6, heightI * 6);
            MessageLabel.Size = new Size(width, height);

            headerLabel.Location = new Point(widthI, headerLabel.Location.Y);

            OldEquipmentPanel.Location = new Point(widthI, heightI);
            OldEquipmentPanel.Size = new Size(width, height);

            NewEquipmentPanel.Location = new Point(3 * widthI + width, heightI);
            NewEquipmentPanel.Size = new Size(width, height);

            AdminCardPanel.Location = new Point(widthI, 2 * heightI + height);
            AdminCardPanel.Size = new Size(width, height);

            DateBox.Location = new Point(3 * widthI + width, 2 * heightI + height);
            DateBox.Size = new Size(DeadlineDate.Width + 25, DeadlineDate.Height + 25);

            int buttonWidth = GeneralPanel.Width * 15 / 100;
            int buttonHeight = GeneralPanel.Height * 10 / 100;

            SubmitButton.Location = new Point(GeneralPanel.Width - buttonWidth - widthI, GeneralPanel.Height - buttonHeight - heightI);
            SubmitButton.Size = new Size(buttonWidth, buttonHeight);

            CloseButton.Location = new Point(SubmitButton.Location.X - buttonWidth - widthI / 2, SubmitButton.Location.Y);
            CloseButton.Size = new Size(buttonWidth, buttonHeight);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            cardReader.CloseConnection();
            this.Close();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (oldEquipment == null)
            {
                showMessage("Old equipment is not found / Старое оборудование не найдено");
            }
            else if (newEquipment == null)
            {
                showMessage("New equipment is not found / Новое оборудование не найдено");
            }
            else if (DeadlineDate.SelectionStart <= DateTime.Today)
            {
                showMessage("Chosen date is unavailable / Выбранная дата недоступна");
            }
            else if (admin == null)
            {
                showMessage("Admin is not found / Администратор не найден");
            }
            else
            {
                PrintSheet.PrintExchangeSheet(user, oldEquipment, newEquipment, DeadlineDate.SelectionStart.ToShortDateString());
                Connector1C.transferEquipment(user, admin, oldEquipment);
                Connector1C.getEquipment(user, newEquipment);
                cardReader.CloseConnection();
                this.Close();
            }
        }

        private void showMessage(string msg)
        {
            MessageLabel.Text = msg;
            MessageLabel.Visible = true;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            MessageLabel.Visible = false;
            timer.Enabled = false;
        }
    }
}
