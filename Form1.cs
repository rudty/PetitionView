using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetitionsList
{

    public partial class Form1 : Form
    {
        HashSet<string> categories = new HashSet<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void updateCategoies()
        {
            categoryList.Invoke(new Action(() =>
            {
                categoryList.BeginUpdate();
                categoryList.Items.Clear();
                foreach (var category in categories)
                {
                    categoryList.Items.Add(category);
                }
                categoryList.EndUpdate();
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Request.listAll(e =>
            {
                bool changed;
                lock (this)
                {
                    changed = categories.Add(e.category);
                }
                if (changed)
                {
                    updateCategoies();
                }
                listBox1.Invoke(new Action(() => listBox1.Items.Add(e)));
            });
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            var copy = Keys.Control | Keys.C;
            if (e.KeyData == copy)
            {
                var item = listBox1.SelectedItem as PetitionItem;
                Clipboard.SetText(item.title);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem as PetitionItem;
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c start /max https://www1.president.go.kr/petitions/" + item.id,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
