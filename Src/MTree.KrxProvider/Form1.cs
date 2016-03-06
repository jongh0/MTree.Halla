using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTree.KrxProvider
{
    public partial class Form1 : Form
    {
        private KrxProvider provider;

        public Form1()
        {
            InitializeComponent();

            provider = new KrxProvider();
        }
    }
}
