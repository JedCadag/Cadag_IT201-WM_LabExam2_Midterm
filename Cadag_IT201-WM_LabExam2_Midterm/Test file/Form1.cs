using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_file
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Example: Set default text for a label and textbox
            Label label2 = this.Controls.Find("label2", true).FirstOrDefault() as Label;
            if (label2 != null)
            {
                label2.Text = "Welcome to the application!";
            }

            TextBox textBox1 = this.Controls.Find("textBox1", true).FirstOrDefault() as TextBox;
            if (textBox1 != null)
            {
                textBox1.Text = "Enter your name here...";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Example: Update label text based on textbox input
            TextBox textBox1 = this.Controls.Find("textBox1", true).FirstOrDefault() as TextBox;
            Label label2 = this.Controls.Find("label2", true).FirstOrDefault() as Label;

            if (textBox1 != null && label2 != null)
            {
                label2.Text = $"Hello, {textBox1.Text}!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Display a message when button1 is clicked
            MessageBox.Show("Button 1 clicked!");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Confirm before closing the form
            var result = MessageBox.Show("Are you sure you want to close the application?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private int CalculateSum(int num1, int num2)
        {
            // Utility method to calculate the sum of two numbers
            return num1 + num2;
        }

        private void UpdateLabel(string labelName, string text)
        {
            // Update the text of a label dynamically
            Label label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
            if (label != null)
            {
                label.Text = text;
            }
        }
    }
}
