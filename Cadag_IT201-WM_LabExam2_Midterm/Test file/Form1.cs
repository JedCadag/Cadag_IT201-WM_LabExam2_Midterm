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
        // Fields used by the computation methods
        private int qty;
        private double price;
        private double discount_amt;
        private double discounted_amt;

        // --- GLOBAL VARIABLES FOR SUMMARY TRACKING ---
        private int totalQty = 0;
        private double totalDiscountGiven = 0;
        private double totalDiscountedAmount = 0;

        // Track discounts by type for detailed summary
        private double seniorDiscountAmount = 0;
        private double employeeDiscountAmount = 0;
        private double cardDiscountAmount = 0;
        private double noDiscountAmount = 0;
        private int currentDiscountType = 0; // 0=none, 1=senior, 2=card, 3=employee

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
                textBox1.Text = "Click an item to order...";
            }

            // Set item names and prices for picture boxes (labels label17..label36)
            var itemNames = new string[] {
                "Adobo", "Sinigang", "Kare-Kare", "Lechon", "Sisig", "Crispy Pata", "Pancit Guisado", "Bulalo", "Arroz Caldo", "Bicol Express",
                "Dinuguan", "Lumpia Shanghai", "Tinola", "Kaldereta", "Pinakbet", "Inihaw na Liempo", "Tortang Talong", "Tapa", "Tokwa’t Baboy", "Laing"
            };
            var prices = new string[] {
                "95.00", "110.00", "145.00", "150.00", "125.00", "150.00", "85.00", "140.00", "75.00", "105.00",
                "95.00", "80.00", "100.00", "135.00", "85.00", "120.00", "75.00", "115.00", "80.00", "90.00"
            };

            for (int i = 1; i <= 20; i++)
            {
                int index = i - 1; // capture
                var pb = this.Controls.Find($"pictureBox{ i }", true).FirstOrDefault() as PictureBox;
                // label names for item labels start at label17
                var lbl = this.Controls.Find($"label{ 17 + index }", true).FirstOrDefault() as Label;
                string itemName = itemNames.Length > index ? itemNames[index] : string.Format("Item {0}", i);
                string price = prices.Length > index ? prices[index] : "0";
                if (lbl != null)
                {
                    lbl.Text = itemName;
                }
                if (pb != null)
                {
                    pb.Click += (s, ev) =>
                    {
                        price_item_TextValue(itemName, price);
                        quantityTxtbox();
                    };
                }
            }

            // Wire radio buttons to discount handlers
            if (this.radioButton1 != null)
                this.radioButton1.CheckedChanged += (s, ev) => { if (this.radioButton1.Checked) ApplySeniorDiscount(); };
            if (this.radioButton2 != null)
                this.radioButton2.CheckedChanged += (s, ev) => { if (this.radioButton2.Checked) ApplyCardDiscount(); };
            if (this.radioButton3 != null)
                this.radioButton3.CheckedChanged += (s, ev) => { if (this.radioButton3.Checked) ApplyEmployeeDiscount(); };
            if (this.radioButton4 != null)
                this.radioButton4.CheckedChanged += (s, ev) => { if (this.radioButton4.Checked) ApplyNoDiscount(); };

            // Wire cash rendered textbox for backspace and auto-calculate change
            if (this.textBox13 != null)
            {
                this.textBox13.KeyDown += CashRenderedTextBox_KeyDown;
                this.textBox13.TextChanged += CashRenderedTextBox_TextChanged;
            }

            // Wire number pad buttons (0-9 and dot)
            if (this.button2 != null) this.button2.Click += AnyNumberButton_Click; // 7
            if (this.button3 != null) this.button3.Click += AnyNumberButton_Click; // 4
            if (this.button4 != null) this.button4.Click += AnyNumberButton_Click; // 1
            if (this.button5 != null) this.button5.Click += AnyNumberButton_Click; // 0
            if (this.button6 != null) this.button6.Click += DotButton_Click; // .
            if (this.button7 != null) this.button7.Click += AnyNumberButton_Click; // 2
            if (this.button8 != null) this.button8.Click += AnyNumberButton_Click; // 5
            if (this.button9 != null) this.button9.Click += AnyNumberButton_Click; // 8
            if (this.button11 != null) this.button11.Click += AnyNumberButton_Click; // 3
            if (this.button12 != null) this.button12.Click += AnyNumberButton_Click; // 6
            if (this.button13 != null) this.button13.Click += AnyNumberButton_Click; // 9

            // Wire control buttons
            if (this.button1 != null) this.button1.Click -= button1_Click; // Remove old handler
            if (this.button1 != null) this.button1.Click += (s, ev) => EnterButton_Click(s, ev); // ENTER
            if (this.button19 != null) this.button19.Click += (s, ev) => NewButton_Click(s, ev); // NEW
            if (this.button20 != null) this.button20.Click += (s, ev) => CancelButton_Click(s, ev); // CANCEL
            if (this.button21 != null) this.button21.Click += (s, ev) => ExitButton_Click(s, ev); // EXIT
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Example: Update label text on textbox input
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

        // --- Methods added for quantity/price/discount handling ---
        private void quantityTxtbox()
        {
            // Clears and focuses the quantity textbox (mapped to textBox2)
            if (this.textBox2 != null)
            {
                this.textBox2.Clear();
                this.textBox2.Focus();
            }
        }

        private void quantity_price_Convert()
        {
            // Parse quantity and price from the mapped textboxes (textBox2 and textBox3)
            qty = 0;
            price = 0.0;
            if (this.textBox2 != null)
            {
                int parsedQty;
                if (!int.TryParse(this.textBox2.Text, out parsedQty))
                    throw new FormatException("Quantity is not a valid integer.");
                qty = parsedQty;
            }
            if (this.textBox3 != null)
            {
                double parsedPrice;
                if (!double.TryParse(this.textBox3.Text, out parsedPrice))
                    throw new FormatException("Price is not a valid number.");
                price = parsedPrice;
            }
        }

        private void computation_Formula_and_DisplayData()
        {
            // Compute discounted amount and display discount and discounted total
            // discount_amt should be set by caller before calling this method
            discounted_amt = (qty * price) - discount_amt;
            if (this.textBox4 != null)
            {
                this.textBox4.Text = discount_amt.ToString("N2");
            }
            if (this.textBox5 != null)
            {
                this.textBox5.Text = discounted_amt.ToString("N2");
            }
        }

        public void price_item_TextValue(string itemname, string priceValue)
        {
            // Map item name and price into the form controls (textBox1 and textBox3)
            if (this.textBox1 != null)
            {
                this.textBox1.Text = itemname;
            }
            if (this.textBox3 != null)
            {
                this.textBox3.Text = priceValue;
            }
        }

        // Example implementations for the radio button discount actions.
        // These methods follow the behavior described in the snippet the user provided.
        private void ApplySeniorDiscount()
        {
            try
            {
                currentDiscountType = 1; // Senior
                quantity_price_Convert();
                discount_amt = (qty * price) * 0.30;
                computation_Formula_and_DisplayData();
                // Uncheck other radio buttons if present
                if (this.radioButton2 != null) this.radioButton2.Checked = false;
                if (this.radioButton3 != null) this.radioButton3.Checked = false;
                if (this.radioButton4 != null) this.radioButton4.Checked = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Input is invalid");
                quantityTxtbox();
            }
        }

        private void ApplyCardDiscount()
        {
            try
            {
                currentDiscountType = 2; // Card
                quantity_price_Convert();
                discount_amt = (qty * price) * 0.15;
                computation_Formula_and_DisplayData();
                if (this.radioButton1 != null) this.radioButton1.Checked = false;
                if (this.radioButton4 != null) this.radioButton4.Checked = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid data input in quantity");
                quantityTxtbox();
            }
        }

        private void ApplyNoDiscount()
        {
            try
            {
                currentDiscountType = 0; // None
                quantity_price_Convert();
                discount_amt = 0;
                computation_Formula_and_DisplayData();
                if (this.radioButton1 != null) this.radioButton1.Checked = false;
                if (this.radioButton2 != null) this.radioButton2.Checked = false;
                if (this.radioButton3 != null) this.radioButton3.Checked = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid data input in quantity");
                quantityTxtbox();
            }
        }

        private void ApplyEmployeeDiscount()
        {
            try
            {
                currentDiscountType = 3; // Employee
                quantity_price_Convert();
                discount_amt = (qty * price) * 0.20; // Employee discount (subtle difference from senior/card)
                computation_Formula_and_DisplayData();
                if (this.radioButton1 != null) this.radioButton1.Checked = false;
                if (this.radioButton2 != null) this.radioButton2.Checked = false;
                if (this.radioButton4 != null) this.radioButton4.Checked = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid data input in quantity");
                quantityTxtbox();
            }
        }

        // --- SUMMARY AND CONTROL LOGIC ---

        private void UpdateSummaryValues()
        {
            totalQty += qty;
            totalDiscountGiven += discount_amt;
            totalDiscountedAmount += discounted_amt;

            // Track discount by current type
            if (currentDiscountType == 1)
                seniorDiscountAmount += discount_amt;
            else if (currentDiscountType == 2)
                cardDiscountAmount += discount_amt;
            else if (currentDiscountType == 3)
                employeeDiscountAmount += discount_amt;
            else
                noDiscountAmount += discount_amt;

            if (this.textBox6 != null)
                this.textBox6.Text = totalQty.ToString();
            if (this.textBox7 != null)
                this.textBox7.Text = $"Total: {totalDiscountGiven.ToString("N2")}\nSenior: {seniorDiscountAmount.ToString("N2")}\nEmployee: {employeeDiscountAmount.ToString("N2")}\nCard: {cardDiscountAmount.ToString("N2")}";
            if (this.textBox8 != null)
                this.textBox8.Text = totalDiscountedAmount.ToString("N2");
        }

        private void AnyNumberButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (this.textBox13 != null)
            {
                this.textBox13.Text += btn.Text;
            }
        }

        private void CashRenderedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && this.textBox13 != null && this.textBox13.Text.Length > 0)
            {
                this.textBox13.Text = this.textBox13.Text.Substring(0, this.textBox13.Text.Length - 1);
                e.Handled = true;
                AutoCalculateChange();
            }
        }

        private void CashRenderedTextBox_TextChanged(object sender, EventArgs e)
        {
            AutoCalculateChange();
        }

        private void AutoCalculateChange()
        {
            if (this.textBox13 == null || this.textBox14 == null)
                return;

            double cash;
            if (double.TryParse(this.textBox13.Text, out cash))
            {
                double change = cash - totalDiscountedAmount;
                if (change < 0)
                {
                    this.textBox14.Text = "0.00";
                }
                else
                {
                    this.textBox14.Text = change.ToString("N2");
                }
            }
            else
            {
                this.textBox14.Text = "0.00";
            }
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.textBox13 == null || this.textBox14 == null)
                    return;
                double cash = Convert.ToDouble(this.textBox13.Text);
                double change = cash - totalDiscountedAmount;
                if (change < 0)
                {
                    MessageBox.Show("Insufficient cash rendered!");
                    this.textBox14.Text = "0.00";
                }
                else
                {
                    this.textBox14.Text = change.ToString("N2");
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid cash amount.");
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            // Reset summary
            totalQty = 0;
            totalDiscountGiven = 0;
            totalDiscountedAmount = 0;
            seniorDiscountAmount = 0;
            employeeDiscountAmount = 0;
            cardDiscountAmount = 0;
            noDiscountAmount = 0;
            currentDiscountType = 0;
            
            if (this.textBox6 != null) this.textBox6.Clear();
            if (this.textBox7 != null) this.textBox7.Clear();
            if (this.textBox8 != null) this.textBox8.Clear();
            if (this.textBox13 != null) this.textBox13.Clear();
            if (this.textBox14 != null) this.textBox14.Clear();
            if (this.textBox1 != null) this.textBox1.Clear();
            if (this.textBox2 != null) this.textBox2.Clear();
            if (this.textBox3 != null) this.textBox3.Clear();
            if (this.textBox4 != null) this.textBox4.Clear();
            if (this.textBox5 != null) this.textBox5.Clear();
            if (this.textBox2 != null) this.textBox2.Focus();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (this.textBox13 != null) this.textBox13.Clear();
            if (this.textBox14 != null) this.textBox14.Clear();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DotButton_Click(object sender, EventArgs e)
        {
            if (this.textBox13 != null && !this.textBox13.Text.Contains("."))
            {
                this.textBox13.Text += ".";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button22_Click(object sender, EventArgs e)
        {


        }
    }
}
