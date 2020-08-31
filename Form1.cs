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
        private readonly List<PetitionItem> allCategoryItems = new List<PetitionItem>();
        private string currentCategory = "전체";

        public Form1()
        {
            InitializeComponent();
        }

        private void addCategoryListView(string category)
        {
            categoryList.Invoke(new Action(() => categoryList.Items.Add(category)));
        }

        private void addCategory(PetitionItem item)
        {
            List<PetitionItem> c;
            lock (this)
            {
                if (!categories.ContainsKey(item.category))
                {
                    c = new List<PetitionItem>();
                    categories.Add(item.category, c);
                    addCategoryListView(item.category);
                }
                else
                {
                    c = categories[item.category];
                }
                c.Add(item);
                allCategoryItems.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addCategoryListView("전체");
            categories.Add("전체", allCategoryItems);

            Request.listAll(e =>
            {
                addCategory(e);
                if (currentCategory == "전체" || e.category == currentCategory)
                {
                    listBox1.Invoke(new Action(() => listBox1.Items.Add(e)));
                }
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
                    string selectCategory = (string)categoryList.SelectedItem;
                    if (selectCategory == null)
                    {
                        return;
                    }
                    listBox1.BeginUpdate();
                    listBox1.Items.Clear();
                    foreach (var item in categories[selectCategory])
                    {
                        listBox1.Items.Add(item);
                    }
                    listBox1.EndUpdate();

                    currentCategory = selectCategory;
                }
            }
        }
    }
}
