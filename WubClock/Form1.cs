/*
 * WubClock : Wake up with dubstep
 * 
 * Alarm clock that loads up listen.pls (initially dubstep.fm) 
 * in the same directory as the program when it goes off. 
 * Hitting spacebar will snooze.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WubClock {
	public partial class Form1 : Form {
		private int hour;
		private int minute;
		private Process process;
		private bool active = false;

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
			comboBox1.SelectedIndex = Properties.Settings.Default.AMPM;
			numericUpDown1.Value = Properties.Settings.Default.Hour;
			numericUpDown2.Value = Properties.Settings.Default.Minute;

			numericUpDown1_ValueChanged(null, null);
			numericUpDown2_ValueChanged(null, null);
			Icon = Properties.Resources.wubclock_icon;
		}

		private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			// clean up the possibly launched process
			if (process != null) {
				process.CloseMainWindow();
				process.WaitForExit();
			}
		}

		// Minutes
		private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
			minute = (int)numericUpDown2.Value;
		}

		// Hours
		private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
			if (comboBox1.SelectedIndex == 0) {
				// AM selected
				hour = (int)numericUpDown1.Value;

				if (numericUpDown1.Value == 12) {
					hour = 0;
				}
			} else {
				// PM selected
				hour = (int)numericUpDown1.Value + 12;

				if (numericUpDown1.Value == 12) {
					hour = 12;
				}
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			numericUpDown1_ValueChanged(sender, e);
		}

		private void timer1_Tick(object sender, EventArgs e) {
			if (active) {
				// active, make sure this window is the active one
				this.Activate();
				return;
			}

			DateTime curTime = DateTime.Now;

			if (curTime.Hour == hour && curTime.Minute == minute) {
				active = true;
				button1.Enabled = true;
				process = System.Diagnostics.Process.Start("listen.pls");


				this.Activate();	// make self the active window
				button1.Focus();	// focus on the snooze button so spacebar snoozes

			}
		}

		// Snooze button
		private void button1_Click(object sender, EventArgs e) {
			// snooze for 9 minutes, why 9? Because science!
			minute += 9;

			// let's go close the process
			if (process != null) {
				process.CloseMainWindow();
				process.WaitForExit();
			}

		}

		// Disable button 
		private void button2_Click(object sender, EventArgs e) {
			timer1.Stop();
			button3.Enabled = true;
			numericUpDown1.Enabled = true;
			numericUpDown2.Enabled = true;
			comboBox1.Enabled = true;
			button2.Enabled = false;
		}

		// Set button
		private void button3_Click(object sender, EventArgs e) {
			timer1.Start();
			button3.Enabled = false;
			numericUpDown1.Enabled = false;
			numericUpDown2.Enabled = false;
			comboBox1.Enabled = false;
			button2.Enabled = true;

			// save set alarm time for next time
			Properties.Settings.Default.Hour = numericUpDown1.Value;
			Properties.Settings.Default.Minute = numericUpDown2.Value;
			Properties.Settings.Default.AMPM = comboBox1.SelectedIndex;
			Properties.Settings.Default.Save();
		}
	}
}
