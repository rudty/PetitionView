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
        private readonly Dictionary<string, List<PetitionItem>> categories = new Dictionary<string, List<PetitionItem>>();
        private readonly List<PetitionItem> allPetitionItems = new List<PetitionItem>();

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
                    categoryList.Items.Add(category.Key);
                }
                categoryList.EndUpdate();
            }));
        }

        private void addCategory(PetitionItem item)
        {
            bool changed = false;
            lock (this)
            {
                List<PetitionItem> c;
                if (!categories.ContainsKey(item.category))
                {
                    c = new List<PetitionItem>();
                    categories.Add(item.category, c);
                    changed = true;
                } 
                else
                {
                    c = categories[item.category];
                }
                c.Add(item);
                allPetitionItems.Add(item);
            }
            if (changed)
            {
                updateCategoies();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            categories.Add("전체", allPetitionItems);

            Request.listAll(e =>
            {
                addCategory(e);
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

        private void categoryList_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                categoryList.Invoke(new Action(() => categoryList_Click(sender, e)));
            }
            else
            {
                lock (this)
                {
                    listBox1.BeginUpdate();
                    listBox1.Items.Clear();
                    string selectCategory = categoryList.SelectedItem as string;
                    foreach (var item in categories[selectCategory])
                    {
                        listBox1.Items.Add(item);
                    }
                    listBox1.EndUpdate();
                }
            }
        }
    }
}
