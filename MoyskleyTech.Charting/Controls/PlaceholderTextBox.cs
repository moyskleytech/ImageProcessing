using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class PlaceholderTextBox : Control
    {
        Label l = new Label();
        TextBox t = new TextBox();
        public string PlaceHolder
        {
            get; set;
        }
        private bool onPlaceHolder;
        private bool OnPlaceHolder
        {
            get
            { return onPlaceHolder; }
            set
            {
                if (value)
                {
                    l.ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    l.ForeColor = this.ForeColor;
                }
                onPlaceHolder = value;
            }
        }
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            if (OnPlaceHolder = (l.Text.Trim().Length == 0))
                l.Text = PlaceHolder;
        }
        public PlaceholderTextBox()
        {
            l.Dock = t.Dock = DockStyle.Fill;
            l.FlatStyle = FlatStyle.Flat;
            t.BorderStyle = l.BorderStyle = BorderStyle.FixedSingle;


            this.Controls.Add(l);
            this.Controls.Add(t);
            t.Validated += T_Validated;
            l.Click += L_Click;
            t.Resize += T_Resize;
            this.Height = t.Height;
            l.Text = PlaceHolder;
        }

        private void T_Resize(object sender, EventArgs e)
        {
            //this.Height = t.Height;
        }

        private void L_Click(object sender, EventArgs e)
        {
            l.Hide();
            t.Focus();
        }

        private void T_Validated(object sender, EventArgs e)
        {
            this.Text = t.Text;
            l.Show();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            l.Text = t.Text = this.Text;
            if (OnPlaceHolder =( l.Text.Trim().Length == 0))
                l.Text = PlaceHolder;
        }
    }
}
