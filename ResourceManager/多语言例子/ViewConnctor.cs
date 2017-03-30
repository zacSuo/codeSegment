using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Core;

namespace CamInter
{
    public partial class ViewConnctor : Form
    {
        private Action<Form, enumProductType, bool> handlerAfterSave = null;
        public ViewConnctor(Action<Form, enumProductType, bool> afterSave)
        {
            this.handlerAfterSave = afterSave;

            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strName = this.tbName.Text.Trim();
            if (StringValidator.HasContent(strName, lbName.Text))
            {
               this.handlerAfterSave(this, enumProductType.Interface,new DBUtility().InsertItem(new Connectors()
                {
                    Name = strName,
                    Description = this.tbDesc.Text
                }
                    ));
            }
        }
    }
}