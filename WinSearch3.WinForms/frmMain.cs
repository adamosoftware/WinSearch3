using Humanizer;
using JsonSettings.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinForms.Library;
using WinForms.Library.Models;
using WinSearch3.Library;
using WinSearch3.Models;

namespace WinSearch3
{
    public partial class frmMain : Form
    {
        private AppOptions _options;

        public frmMain()
        {
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var fs = new FileSearch()
                {
                    Locations = tbLocations.Text,
                    SearchFilename = tbFilename.Text,
                    Extensions = tbExtensions.Text,
                    Contents = tbContents.Text
                };
                
                var progress = new Progress<string>(ShowProgress);
                var search = fs.ExecuteAsync(progress);

                pbMain.Visible = true;

                var results = await search;
                LoadResults(results);

                tslStatus.Text = $"{fs.FoldersSearched:n0} folders searched, {fs.FilesSearched:n0} files searched in {fs.Elapsed.Humanize()}";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                pbMain.Visible = false;
            }
        }

        private void LoadResults(ILookup<string, string> results)
        {
            try
            {
                listView1.Items.Clear();
                listView1.BeginUpdate();

                foreach (var location in results)
                {
                    var group = new ListViewGroup(location.Key);
                    listView1.Groups.Add(group);

                    foreach (var file in results[location.Key])
                    {
                        var item = new ListViewItem(file);
                        item.Group = group;
                        item.ImageKey = FileSystem.AddIcon(imageList1, file, FileSystem.IconSize.Small);
                        listView1.Items.Add(item);
                    }
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void ShowProgress(string path)
        {
            tslStatus.Text = path;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                _options = SettingsBase.Load<AppOptions>();

                _options.FormPosition?.Apply(this);

                var binder = new ControlBinder<AppOptions>();
                binder.Add(tbLocations, model => model.Locations);
                binder.Add(tbFilename, model => model.SearchFilename);
                binder.Add(tbExtensions, model => model.Extensions);
                binder.Add(tbContents, model => model.Contents);
                binder.Document = _options;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_options != null) _options.FormPosition = FormPosition.FromForm(this);
            _options?.Save();
        }
    }
}
