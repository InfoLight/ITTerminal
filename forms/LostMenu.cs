﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITTerminal
{
    partial class LostMenu : TerminalForm
    {
        private User user;
        private Equipment[] equipments;

        public LostMenu()
        {
            InitializeComponent();
        }

        private void LostMenu_Load(object sender, EventArgs e)
        {
            GeneralPanel_Resize(null, null);
            //Connection with scanners
        }

        public override void CardId(string id)
        {
            user = CardManager.getUser(id);
            if (user == null) return;
            //equipments = (1c api request by 'user')
            label1.Visible = true;
            label1.Text = user.Name + ", choose equipment that you lost:";
            EquipmentList.Visible = true;
            if (equipments != null)
            {
                for (int i = 0; i < equipments.Count(); i++)
                {
                    EquipmentList.Items.Add(equipments[i].Name);
                }
            }
        }

        private void GeneralPanel_Resize(object sender, EventArgs e)
        {
            int width = GeneralPanel.Width * 60 / 100;
            int height = GeneralPanel.Height * 70 / 100;
            int widthI = GeneralPanel.Width * 5 / 100;
            int heightI = GeneralPanel.Height * 5 / 100;
            CardPanel.Location = new Point(4 * widthI, heightI);
            CardPanel.Size = new Size(width, height);
            int buttonWidth = GeneralPanel.Width * 15 / 100;
            int buttonHeight = GeneralPanel.Height * 10 / 100;
            SubmitButton.Location = new Point(GeneralPanel.Width - buttonWidth - widthI, GeneralPanel.Height - buttonHeight - heightI);
            SubmitButton.Size = new Size(buttonWidth, buttonHeight);
            CloseButton.Location = new Point(SubmitButton.Location.X - buttonWidth - widthI / 2, SubmitButton.Location.Y);
            CloseButton.Size = new Size(buttonWidth, buttonHeight);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (user != null && EquipmentList.SelectedIndex != -1)
            {
                PrintSheet.PrintLostSheet(user, equipments[EquipmentList.SelectedIndex]);
                //TODO
                this.Close();
            }
        }
    }
}
