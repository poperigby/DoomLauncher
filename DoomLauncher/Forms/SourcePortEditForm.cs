﻿using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SourcePortEditForm : Form
    {
        private readonly IDataSourceAdapter m_adapter;
        private readonly ITabView[] m_additionalFileViews;
        private readonly SourcePortLaunchType m_type;

        public SourcePortEditForm(IDataSourceAdapter adapter, ITabView[] additionalTabViews, SourcePortLaunchType type)
        {
            InitializeComponent();
            pbInfo.Image = DoomLauncher.Properties.Resources.bon2b;
            lblInfo.Text = string.Format("These files will automatically be added when this executable{0} is selected.", Environment.NewLine);

            ctrlFiles.Initialize("GameFileID", "FileName");
            ctrlFiles.NewItemNeeded += ctrlFiles_NewItemNeeded;

            m_adapter = adapter;
            m_additionalFileViews = additionalTabViews;
            m_type = type;

            Text = m_type.ToString("g");

            sourcePortEdit1.ShowOptions(type == SourcePortLaunchType.Utility);

            if (type == SourcePortLaunchType.Utility && tblMain.RowStyles[1].Height > 0)
            {
                grpAdditionalFiles.Visible = false;
                tblMain.RowStyles[1].Height = 0;
                Height -= grpAdditionalFiles.Height;
            }
        }

        private void ctrlFiles_NewItemNeeded(object sender, AdditionalFilesEventArgs e)
        {
            using (FileSelectForm fileSelect = new FileSelectForm())
            {
                fileSelect.Initialize(m_adapter, m_additionalFileViews);
                fileSelect.MultiSelect = true;
                fileSelect.StartPosition = FormStartPosition.CenterParent;

                if (fileSelect.ShowDialog(this) == DialogResult.OK)
                {
                    IGameFile[] selectedFiles = fileSelect.SelectedFiles;

                    if (selectedFiles.Length > 0)
                    {
                        e.NewItems = selectedFiles.Cast<object>().ToList();
                    }
                }
            }
        }

        public void SetDataSource(ISourcePort sourcePort)
        {
            sourcePortEdit1.SetDataSource(sourcePort);
            ctrlFiles.SetDataSource(Util.GetAdditionalFiles(m_adapter, sourcePort));
        }

        public void UpdateDataSource(ISourcePort sourcePort)
        {
            sourcePortEdit1.UpdateDataSource(sourcePort);
            sourcePort.SettingsFiles = ctrlFiles.GetAdditionalFilesString();
        }

        public void SetSupportedExtensions(string text)
        {
            sourcePortEdit1.SetSupportedExtensions(text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string err = null;

            if (string.IsNullOrEmpty(sourcePortEdit1.SourcePortName))
                err = "Please enter a name for the source port.";
            if (string.IsNullOrEmpty(sourcePortEdit1.SourcePortExec))
                err = "Please select an executable for the source port.";

            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
