﻿using System;
using System.Reflection;
using System.Windows.Forms;
using Dhhr.KppParser.Service;

namespace Dhhr.KppParser.Gui
{
    public partial class MainForm : Form
    {
        private readonly Settings _settings;
        private readonly string _version;

        public MainForm(Settings settings)
        {
            _settings = settings;
            InitializeComponent();

            _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            VersionLabel.Text = $@"Versjon: {_version}";
        }

        private void EpisodeButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "csv",
                Filter = @"CSV filer|*.csv",
                Multiselect = false,
                Title = @"Velg fil med episoder"
            };

            openFileDialog.ShowDialog();
            EpisodePathBox.Text = openFileDialog.FileName;
        }

        private void TjenesteButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "csv",
                Filter = @"CSV filer|*.csv",
                Multiselect = false,
                Title = @"Velg fil med tjenester"
            };

            openFileDialog.ShowDialog();
            TjenestePathBox.Text = openFileDialog.FileName;
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            var args = new Args
            {
                EpisodePath = EpisodePathBox.Text,
                TjenestePath = TjenestePathBox.Text,
                FraDato = FromDatePicker.Value,
                TilDato = ToDatePicker.Value,
                OrganizationName = OrgNameBox.Text,
                OrganizationHerId = OrgHerIdBox.Text,
                OrganizationName2 = OrgNameBox2.Text,
                OrganizationHerId2 = OrgHerIdBox2.Text,
                HDirHerId = RadioTrial.Checked
                    ? _settings.HdirQa
                    : _settings.HdirProd,
                Leverandor = _settings.Leverandor,
                NavnEpj = _settings.NavnEpj,
                VersjonEpj = _settings.VersjonEpj,
                ProgramVersion = _version,
            };

            if (!KppService.IsValid(args, out var errorMessage))
            {
                MessageBox.Show($"Kan ikke lage melding. Årsak:\n\n{errorMessage}", @"Ugyldig input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fileDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                Filter = @"XML filer|*.xml",
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                Title = @"Velg hvor filen skal lagres",
                ValidateNames = true
            };

            if (fileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            args.OutputPath = fileDialog.FileName;

            var progressForm = new ProgressForm(args);
            progressForm.ShowDialog();
        }
    }
}
