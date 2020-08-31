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

        private readonly Dictionary<string, List<PetitionItem>> categoryItems = new Dictionary<string, List<PetitionItem>>();
        private readonly List<PetitionItem> allCategoryItems = new List<PetitionItem>();
        private string currentCategory = "전체";

        public Form1()
        {
            InitializeComponent();
            categoryItems.Add("전체", allCategoryItems);
        }

        private void addCategoryListView(string category)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => addCategoryListView(category)));
                return;
            }

            categoryList.Items.Add(category);
        }

        private void addItemListView(PetitionItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => addItemListView(item)));
                return;
            }


            if (currentCategory == "전체" || item.category == currentCategory)
            {
                listBox1.Items.Add(item);
            }
        }

        private void addCategoryItem(PetitionItem item)
        {
            List<PetitionItem> c;
            lock (this)
            {
                if (!categoryItems.ContainsKey(item.category))
                {
                    c = new List<PetitionItem>();
                    categoryItems.Add(item.category, c);
                    addCategoryListView(item.category);
                }
                else
                {
                    c = categoryItems[item.category];
                }
                c.Add(item);
                allCategoryItems.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addCategoryListView("전체");

            Request.listAll(item =>
            {
                addCategoryItem(item);
                addItemListView(item);
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
                Invoke(new Action(() => categoryList_Click(sender, e)));
                return;
            }

            lock (this)
            {
                string selectCategory = (string)categoryList.SelectedItem ?? "전체";
    
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                listBox1.Items.AddAll(categoryItems[selectCategory]);
                listBox1.EndUpdate();

                currentCategory = selectCategory;
            }
        }
    }
}
