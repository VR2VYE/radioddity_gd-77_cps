using ReadWriteCsv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR
{
	public class ChannelsForm : DockContent, IDisp, ISingleRow
	{
		public const string SZ_HEADER_TEXT_NAME = "HeaderText";

		private const int SCL_FREQ = 100000;

		private static readonly string[] SZ_HEADER_TEXT;

		//private IContainer components;

		private Panel pnlChannel;

		private DataGridView dgvChannels;

		private Button btnClear;

		private Button btnDelete;

		private Button btnAdd;

		private CustomCombo cmbAddChMode;

		private SGTextBox txtRxFreq;

		private SGTextBox txtName;

		private ComboBox cmbPower;

		private ComboBox cmbChMode;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;

		private SGTextBox txtTxFreq;

		private Button btnImport;

		private Button btnExport;

		private Button btnDeleteSelect;

		public TreeNode Node
		{
			get;
			set;
		}

		public void SaveData()
		{
			this.dgvChannels.Focus();
		}

		public void DispData()
		{
			try
			{
				this.dgvChannels.Rows.Clear();
				for (int i = 0; i < ChannelForm.data.Count; i++)
				{
					if (ChannelForm.data.DataIsValid(i))
					{
						ChannelForm.ChannelOne channelOne = ChannelForm.data[i];

						int index = this.dgvChannels.Rows.Add((i + 1).ToString(), channelOne.Name, channelOne.ChModeS, channelOne.RxFreq, channelOne.TxFreq, channelOne.TxColor.ToString(), channelOne.RepeaterSlotS, channelOne.ContactString, channelOne.RxGroupListString, channelOne.ScanListString, channelOne.RxTone, channelOne.TxTone);//, channelOne.PowerString);
						this.dgvChannels.Rows[index].Tag = i;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void RefreshName()
		{
		}

		public ChannelsForm()
		{
			
			//base._002Ector();
			this.InitializeComponent();
			base.Scale(Settings.smethod_6());
		}

		public static void RefreshCommonLang()
		{
			string name = typeof(ChannelsForm).Name;
			Settings.smethod_78("HeaderText", ChannelsForm.SZ_HEADER_TEXT, name);
		}

		private void ChannelsForm_Load(object sender, EventArgs e)
		{
			Settings.smethod_68(this);
			this.method_1();
			this.DispData();
			this.cmbAddChMode.SelectedIndex = 0;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			int selectedIndex = this.cmbAddChMode.SelectedIndex;
			int minIndex = ChannelForm.data.GetMinIndex();
			MainForm mainForm = base.MdiParent as MainForm;
			string minName = ChannelForm.data.GetMinName(this.Node);
			string text = this.cmbAddChMode.Text;
			ChannelForm.data.SetIndex(minIndex, 1);
			ChannelForm.data.SetChName(minIndex, minName);
			ChannelForm.data.SetDefaultFreq(minIndex);
			ChannelForm.data.Default(minIndex);
			ChannelForm.data.SetChMode(minIndex, text);
			ChannelForm.ChannelOne channelOne = ChannelForm.data[minIndex];
			this.dgvChannels.Rows.Insert(minIndex, (minIndex + 1).ToString(), channelOne.Name, channelOne.RxFreq, channelOne.TxFreq, channelOne.ChModeS, channelOne.PowerString, channelOne.RxTone, channelOne.TxTone, channelOne.TxColor.ToString(), channelOne.RxGroupListString, channelOne.ContactString, channelOne.RepeaterSlotS);
			this.dgvChannels.Rows[minIndex].Tag = minIndex;
			this.updateAddAndDeleteButtons();
			int[] array = new int[3]
			{
				2,
				6,
				54
			};
			mainForm.InsertTreeViewNode(this.Node, minIndex, typeof(ChannelForm), array[selectedIndex], ChannelForm.data);
			mainForm.RefreshRelatedForm(base.GetType());
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int index = this.dgvChannels.CurrentRow.Index;
			int index2 = (int)this.dgvChannels.CurrentRow.Tag;
			if (index == 0)
			{
				MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
			}
			else
			{
				this.dgvChannels.Rows.Remove(this.dgvChannels.CurrentRow);
				ChannelForm.data.ClearIndex(index2);
				this.updateAddAndDeleteButtons();
				MainForm mainForm = base.MdiParent as MainForm;
				mainForm.DeleteTreeViewNode(this.Node, index);
				mainForm.RefreshRelatedForm(base.GetType());
			}
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			int index = 1;
			int num = 0;
			MainForm mainForm = base.MdiParent as MainForm;
			while (this.dgvChannels.RowCount > 1)
			{
				num = (int)this.dgvChannels.Rows[1].Tag;
				this.dgvChannels.Rows.RemoveAt(1);
				this.Node.Nodes.RemoveAt(index);
				ChannelForm.data.ClearIndex(num);
			}
			this.updateAddAndDeleteButtons();
			mainForm.RefreshRelatedForm(base.GetType());
		}

		private void btnDeleteSelected_Click(object sender, EventArgs e)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int count = this.dgvChannels.SelectedRows.Count;
			MainForm mainForm = base.MdiParent as MainForm;
			while (this.dgvChannels.SelectedRows.Count > 0)
			{
				num = this.dgvChannels.SelectedRows[0].Index;
				num2 = (int)this.dgvChannels.SelectedRows[0].Tag;
				if (num != 0)
				{
					this.dgvChannels.Rows.Remove(this.dgvChannels.SelectedRows[0]);
					ChannelForm.data.ClearIndex(num2);
					mainForm.DeleteTreeViewNode(this.Node, num);
					num3++;
					if (num3 == count)
					{
						break;
					}
					continue;
				}
				MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
				break;
			}
			this.updateAddAndDeleteButtons();
			mainForm.RefreshRelatedForm(base.GetType());
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			int i = 0;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = "*.csv";
			saveFileDialog.AddExtension = true;
			saveFileDialog.Filter = "csv files|*.csv";
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.CheckPathExists = true;
			saveFileDialog.FileName = "Channels_" + DateTime.Now.ToString("MMdd_HHmmss");
			if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != null)
			{
				using (CsvFileWriter csvFileWriter = new CsvFileWriter(new FileStream(saveFileDialog.FileName, FileMode.Create), Encoding.Default))
				{
					CsvRow csvRow = new CsvRow();
					csvRow.AddRange(ChannelsForm.SZ_HEADER_TEXT);
					csvFileWriter.WriteRow(csvRow);
					for (i = 0; i < ChannelForm.data.Count; i++)
					{
						if (ChannelForm.data.DataIsValid(i))
						{
							csvRow.RemoveAll(ChannelsForm.smethod_0);
							ChannelForm.ChannelOne channelOne = ChannelForm.data[i];
							csvRow.Add("CH_DATA");
							csvRow.Add(channelOne.Name);
							csvRow.Add(channelOne.ChModeS);
							csvRow.Add(channelOne.RxFreq);
							csvRow.Add(channelOne.TxFreq);
							csvRow.Add(channelOne.TxColor.ToString());

							csvRow.Add(channelOne.RepeaterSlotS);// Timeslot
							csvRow.Add(channelOne.ContactString);
							csvRow.Add(channelOne.RxGroupListString);
							csvRow.Add(channelOne.ScanListString);

							csvRow.Add(channelOne.RxTone);
							csvRow.Add(channelOne.TxTone);
							csvRow.Add(channelOne.PowerString);
							csvRow.Add(channelOne.BandwidthString);
							csvRow.Add(channelOne.OnlyRxString);
							csvRow.Add(channelOne.SquelchString);

							csvRow.Add(channelOne.AdmitCriteria.ToString());

							csvRow.Add(channelOne.Tot.ToString());
							csvRow.Add(channelOne.TotRekey.ToString());

							csvRow.Add(channelOne.TxSignaling.ToString());
							csvRow.Add(channelOne.RxSignaling.ToString());

							csvRow.Add(channelOne.Key.ToString());
							csvRow.Add(channelOne.EmgSystem.ToString());

							csvRow.Add(channelOne.Flag1.ToString());
							csvRow.Add(channelOne.Flag2.ToString());
							csvRow.Add(channelOne.Flag3.ToString());
							csvRow.Add(channelOne.Flag4.ToString());

							csvRow.Add(channelOne.RssiThreshold.ToString());
							csvRow.Add(channelOne.VoiceEmphasis.ToString());
							csvRow.Add(channelOne.TxSignaling.ToString());
							csvRow.Add(channelOne.UnmuteRule.ToString());
							csvRow.Add(channelOne.RxSignaling.ToString());
							csvRow.Add(channelOne.ArtsInterval.ToString());

							csvFileWriter.WriteRow(csvRow);
						}
					}
				}
			}
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			int num2 = 0;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "csv files|*.csv";
			if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
			{
				using (CsvFileReader csvFileReader = new CsvFileReader(openFileDialog.FileName, Encoding.Default))
				{
					CsvRow csvRow = new CsvRow();
					csvFileReader.ReadRow(csvRow);
					if (csvRow.SequenceEqual(ChannelsForm.SZ_HEADER_TEXT))
					{
						/* Don't clear channels first. We now just update or append.
						for (int num = 0; num < ChannelForm.data.Count; num++)
						{
							ChannelForm.data.SetIndex(num, 0);
						}
						 */

						while (csvFileReader.ReadRow(csvRow))
						{
							int num = 1;

							string name = ((List<string>)csvRow)[num++];
							int foundIndex = ChannelForm.data.FindIndexForName(name);

							if (foundIndex == -1)
							{
								num2 = ChannelForm.data.GetMinIndex();
								if (num2 == -1)
								{
									MessageBox.Show("Error. Maximum numbers of channels reached. Import aborted");
									break;// stop processing
								}
							}
							else
							{
								num2 = foundIndex;
							}

							ChannelForm.ChannelOne value = ChannelForm.data[num2];
										
							value.Name = name;
							value.ChModeS = ((List<string>)csvRow)[num++];
							value.RxFreq = ((List<string>)csvRow)[num++];
							value.TxFreq = ((List<string>)csvRow)[num++];
							value.TxColor = Convert.ToInt32(((List<string>)csvRow)[num++]);

							value.RepeaterSlotS = ((List<string>)csvRow)[num++];// Timeslot
							value.ContactString = ((List<string>)csvRow)[num++];
							value.RxGroupListString = ((List<string>)csvRow)[num++];
							value.ScanListString = ((List<string>)csvRow)[num++];

							value.RxTone = ((List<string>)csvRow)[num++];
							value.TxTone = ((List<string>)csvRow)[num++];
							value.PowerString = ((List<string>)csvRow)[num++];
							value.BandwidthString = ((List<string>)csvRow)[num++];
							value.OnlyRxString = ((List<string>)csvRow)[num++];
							value.SquelchString = ((List<string>)csvRow)[num++];

							value.AdmitCriteria = Convert.ToInt32(((List<string>)csvRow)[num++]);

							value.Tot = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.TotRekey = Convert.ToInt32(((List<string>)csvRow)[num++]);

							value.TxSignaling = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.RxSignaling = Convert.ToInt32(((List<string>)csvRow)[num++]);

							value.Key = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.EmgSystem = Convert.ToInt32(((List<string>)csvRow)[num++]);

							value.Flag1 = Convert.ToByte(((List<string>)csvRow)[num++]);
							value.Flag2 = Convert.ToByte(((List<string>)csvRow)[num++]);
							value.Flag3 = Convert.ToByte(((List<string>)csvRow)[num++]);
							value.Flag4 = Convert.ToByte(((List<string>)csvRow)[num++]);

							value.RssiThreshold = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.VoiceEmphasis = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.TxSignaling = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.UnmuteRule = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.RxSignaling = Convert.ToInt32(((List<string>)csvRow)[num++]);
							value.ArtsInterval = Convert.ToInt32(((List<string>)csvRow)[num++]);


							ChannelForm.data.SetIndex(num2, 1);
							ChannelForm.data.Default(num2);
							ChannelForm.data[num2] = value;
						}
						this.DispData();
						MainForm mainForm = base.MdiParent as MainForm;
						mainForm.InitChannels(this.Node);
						mainForm.RefreshRelatedForm(base.GetType());
					}
					else
					{
						MessageBox.Show("DataFormatError");
					}
				}
			}
		}

		private void updateAddAndDeleteButtons()
		{
			this.btnDelete.Enabled = !this.dgvChannels.SelectedRows.Contains(this.dgvChannels.Rows[0]);
			this.btnAdd.Enabled = (this.dgvChannels.RowCount < ChannelForm.data.Count);
		}

		private void method_1()
		{
			int num = 0;
			int[] array = new int[12]
			{
				80,
				100,
				80,
				80,
				80,
				80,
				80,
				80,
				80,
				100,
				100,
				100
			};
			this.dgvChannels.ReadOnly = true;
			this.dgvChannels.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dgvChannels.AllowUserToAddRows = false;
			this.dgvChannels.AllowUserToDeleteRows = false;
			this.dgvChannels.AllowUserToResizeRows = false;
			this.dgvChannels.AllowUserToOrderColumns = false;
			this.dgvChannels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewTextBoxColumn dataGridViewTextBoxColumn = null;
			string[] sZ_HEADER_TEXT = ChannelsForm.SZ_HEADER_TEXT;
			for(int i =0;i<array.Length;i++)
			{
				//string headerText = sZ_HEADER_TEXT[i];
			//foreach (string headerText in sZ_HEADER_TEXT)
			//{
				dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
				dataGridViewTextBoxColumn.HeaderText = sZ_HEADER_TEXT[i];
				dataGridViewTextBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				dataGridViewTextBoxColumn.ReadOnly = true;
				dataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridViewTextBoxColumn.Width = array[num++];
				this.dgvChannels.Columns.Add(dataGridViewTextBoxColumn);
			}
			Settings.smethod_37(this.cmbAddChMode, ChannelForm.SZ_CH_MODE);
			Settings.smethod_37(this.cmbChMode, ChannelForm.SZ_CH_MODE);
			Settings.smethod_37(this.cmbPower, ChannelForm.SZ_POWER);
			this.txtName.MaxLength = 16;
			this.txtRxFreq.MaxLength = 9;
			this.txtTxFreq.MaxLength = 9;
		}

		private void iPdgpEleug(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex >= 1)
			{
				Control[] array = new Control[6]
				{
					null,
					this.txtName,
					this.txtRxFreq,
					this.txtTxFreq,
					this.cmbChMode,
					this.cmbPower
				};
				Control control = array[e.ColumnIndex];
				if (this.dgvChannels.CurrentRow.Tag != null)
				{
					int num = (int)this.dgvChannels.CurrentRow.Tag;
					Rectangle cellDisplayRectangle = this.dgvChannels.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
					Point location = cellDisplayRectangle.Location;
					location.Offset(this.dgvChannels.Location);
					location.Offset(this.pnlChannel.Location);
					control.Location = location;
					control.Size = cellDisplayRectangle.Size;
					control.Text = ((DataGridView)sender).CurrentCell.Value.ToString();
					control.Visible = true;
					control.Focus();
					control.BringToFront();
				}
			}
		}

		private void NligzloMrR(object sender, DataGridViewRowPostPaintEventArgs e)
		{
			try
			{
				DataGridView dataGridView = sender as DataGridView;
				if (e.RowIndex >= dataGridView.FirstDisplayedScrollingRowIndex)
				{
					using (SolidBrush brush = new SolidBrush(dataGridView.RowHeadersDefaultCellStyle.ForeColor))
					{
						string s = (e.RowIndex + 1).ToString();
						e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, (float)(e.RowBounds.Location.X + 15), (float)(e.RowBounds.Location.Y + 5));
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void dgvChannels_SelectionChanged(object sender, EventArgs e)
		{
			this.updateAddAndDeleteButtons();
		}

		private void dgvChannels_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			MainForm mainForm = base.MdiParent as MainForm;
			if (mainForm != null)
			{
				DataGridView dataGridView = sender as DataGridView;
				int index = (int)dataGridView.Rows[e.RowIndex].Tag;
				mainForm.DispChildForm(typeof(ChannelForm), index);
			}
		}

		private void cmbChMode_Leave(object sender, EventArgs e)
		{
			int index = this.dgvChannels.CurrentRow.Index;
			int index2 = (int)this.dgvChannels.CurrentRow.Tag;
			int selectedIndex = this.cmbChMode.SelectedIndex;
			string text = this.cmbChMode.Text;
			this.cmbChMode.Visible = false;
			if (!(this.cmbChMode.Text == ChannelForm.data[index2].ChModeS))
			{
				int[] array = new int[3]
				{
					2,
					6,
					54
				};
				this.dgvChannels.CurrentCell.Value = text;
				ChannelForm.data.SetChMode(index2, text);
				this.Node.Nodes[index].ImageIndex = array[selectedIndex];
				this.Node.Nodes[index].SelectedImageIndex = array[selectedIndex];
				((MainForm)base.MdiParent).RefreshRelatedForm(base.GetType());
			}
		}

		private void txtName_Leave(object sender, EventArgs e)
		{
			int index = (int)this.dgvChannels.CurrentRow.Tag;
			int index2 = this.dgvChannels.CurrentRow.Index;
			this.txtName.Visible = false;
			string text = this.txtName.Text;
			if (!(text == ChannelForm.data[index].Name) && !ChannelForm.data.NameExist(text))
			{
				this.dgvChannels.CurrentCell.Value = text;
				ChannelForm.data.SetName(index, text);
				this.Node.Nodes[index2].Text = text;
				((MainForm)base.MdiParent).RefreshRelatedForm(base.GetType());
			}
		}

		private void CaeqgYciuW(object sender, EventArgs e)
		{
			int index = (int)this.dgvChannels.CurrentRow.Tag;
			this.txtRxFreq.Visible = false;
			int int_ = 0;
			double num = 0.0;
			string text = this.txtRxFreq.Text;
			try
			{
				uint num2 = 0u;
				num = double.Parse(text);
				if (Settings.smethod_19(num, ref num2) < 0)
				{
					return;
				}
				int_ = Settings.smethod_27(num, 100000.0);
				Settings.smethod_29(ref int_, 250, 625);
				num = Settings.smethod_28(int_, 100000);
				text = string.Format("{0:f5}", num);
			}
			catch
			{
				return;
			}
			this.dgvChannels.CurrentCell.Value = text;
			ChannelForm.data.SetRxFreq(index, text);
			if (ChannelForm.data.FreqIsSameRange(index) < 0)
			{
				ChannelForm.data.SetTxFreq(index, text);
				this.dgvChannels.CurrentRow.Cells[3].Value = text;
			}
			((MainForm)base.MdiParent).RefreshRelatedForm(base.GetType());
		}

		private void txtTxFreq_Leave(object sender, EventArgs e)
		{
			int index = (int)this.dgvChannels.CurrentRow.Tag;
			this.txtTxFreq.Visible = false;
			int int_ = 0;
			double num = 0.0;
			string text = this.txtTxFreq.Text;
			try
			{
				uint num2 = 0u;
				num = double.Parse(text);
				if (Settings.smethod_19(num, ref num2) < 0)
				{
					return;
				}
				int_ = Settings.smethod_27(num, 100000.0);
				Settings.smethod_29(ref int_, 250, 625);
				num = Settings.smethod_28(int_, 100000);
				text = string.Format("{0:f5}", num);
			}
			catch
			{
				return;
			}
			this.dgvChannels.CurrentCell.Value = text;
			ChannelForm.data.SetTxFreq(index, text);
			if (ChannelForm.data.FreqIsSameRange(index) < 0)
			{
				ChannelForm.data.SetRxFreq(index, text);
				this.dgvChannels.CurrentRow.Cells[2].Value = text;
			}
			((MainForm)base.MdiParent).RefreshRelatedForm(base.GetType());
		}

		private void cmbPower_Leave(object sender, EventArgs e)
		{
			int index = (int)this.dgvChannels.CurrentRow.Tag;
			this.cmbPower.Visible = false;
			this.dgvChannels.CurrentCell.Value = this.cmbPower.Text;
			ChannelForm.data.SetPower(index, this.cmbPower.Text);
			((MainForm)base.MdiParent).RefreshRelatedForm(base.GetType());
		}

		public void RefreshSingleRow(int index)
		{
			ChannelForm.ChannelOne channelOne = ChannelForm.data[index];
			int index2 = 0;
			foreach (DataGridViewRow item in (IEnumerable)this.dgvChannels.Rows)
			{
				if (Convert.ToInt32(item.Tag) != index)
				{
					continue;
				}
				index2 = item.Index;
				break;
			}
			this.dgvChannels.Rows[index2].Cells[1].Value = channelOne.Name;
			this.dgvChannels.Rows[index2].Cells[2].Value = channelOne.RxFreq;
			this.dgvChannels.Rows[index2].Cells[3].Value = channelOne.TxFreq;
			this.dgvChannels.Rows[index2].Cells[4].Value = channelOne.ChModeS;
			this.dgvChannels.Rows[index2].Cells[5].Value = channelOne.PowerString;
			this.dgvChannels.Rows[index2].Cells[6].Value = channelOne.RxTone;
			this.dgvChannels.Rows[index2].Cells[7].Value = channelOne.TxTone;
			this.dgvChannels.Rows[index2].Cells[8].Value = channelOne.TxColor.ToString();
			this.dgvChannels.Rows[index2].Cells[9].Value = channelOne.RxGroupListString;
			this.dgvChannels.Rows[index2].Cells[10].Value = channelOne.ContactString;
			this.dgvChannels.Rows[index2].Cells[11].Value = channelOne.RepeaterSlotS;
		}

		protected override void Dispose(bool disposing)
		{
			/*if (disposing && this.components != null)
			{
				this.components.Dispose();
			}*/
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.pnlChannel = new System.Windows.Forms.Panel();
			this.btnImport = new System.Windows.Forms.Button();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnDeleteSelect = new System.Windows.Forms.Button();
			this.cmbPower = new System.Windows.Forms.ComboBox();
			this.cmbChMode = new System.Windows.Forms.ComboBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.dgvChannels = new System.Windows.Forms.DataGridView();
			this.txtTxFreq = new DMR.SGTextBox();
			this.txtRxFreq = new DMR.SGTextBox();
			this.txtName = new DMR.SGTextBox();
			this.cmbAddChMode = new CustomCombo();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pnlChannel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlChannel
			// 
			this.pnlChannel.AutoScroll = true;
			this.pnlChannel.AutoSize = true;
			this.pnlChannel.Controls.Add(this.btnImport);
			this.pnlChannel.Controls.Add(this.btnExport);
			this.pnlChannel.Controls.Add(this.btnDeleteSelect);
			this.pnlChannel.Controls.Add(this.txtTxFreq);
			this.pnlChannel.Controls.Add(this.txtRxFreq);
			this.pnlChannel.Controls.Add(this.txtName);
			this.pnlChannel.Controls.Add(this.cmbPower);
			this.pnlChannel.Controls.Add(this.cmbChMode);
			this.pnlChannel.Controls.Add(this.cmbAddChMode);
			this.pnlChannel.Controls.Add(this.btnClear);
			this.pnlChannel.Controls.Add(this.btnDelete);
			this.pnlChannel.Controls.Add(this.btnAdd);
			this.pnlChannel.Controls.Add(this.dgvChannels);
			this.pnlChannel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlChannel.Location = new System.Drawing.Point(0, 0);
			this.pnlChannel.Name = "pnlChannel";
			this.pnlChannel.Size = new System.Drawing.Size(1136, 531);
			this.pnlChannel.TabIndex = 0;
			// 
			// btnImport
			// 
			this.btnImport.Location = new System.Drawing.Point(608, 10);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(75, 23);
			this.btnImport.TabIndex = 13;
			this.btnImport.Text = "Import";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// btnExport
			// 
			this.btnExport.Location = new System.Drawing.Point(527, 11);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(75, 23);
			this.btnExport.TabIndex = 14;
			this.btnExport.Text = "Export";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnDeleteSelect
			// 
			this.btnDeleteSelect.Location = new System.Drawing.Point(220, 11);
			this.btnDeleteSelect.Name = "btnDeleteSelect";
			this.btnDeleteSelect.Size = new System.Drawing.Size(128, 23);
			this.btnDeleteSelect.TabIndex = 12;
			this.btnDeleteSelect.Text = "Delete Selected";
			this.btnDeleteSelect.UseVisualStyleBackColor = true;
			this.btnDeleteSelect.Click += new System.EventHandler(this.btnDeleteSelected_Click);
			// 
			// cmbPower
			// 
			this.cmbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbPower.FormattingEnabled = true;
			this.cmbPower.Location = new System.Drawing.Point(979, 10);
			this.cmbPower.Name = "cmbPower";
			this.cmbPower.Size = new System.Drawing.Size(61, 24);
			this.cmbPower.TabIndex = 8;
			this.cmbPower.Visible = false;
			this.cmbPower.Leave += new System.EventHandler(this.cmbPower_Leave);
			// 
			// cmbChMode
			// 
			this.cmbChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbChMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbChMode.FormattingEnabled = true;
			this.cmbChMode.Location = new System.Drawing.Point(912, 10);
			this.cmbChMode.Name = "cmbChMode";
			this.cmbChMode.Size = new System.Drawing.Size(61, 24);
			this.cmbChMode.TabIndex = 7;
			this.cmbChMode.Visible = false;
			this.cmbChMode.Leave += new System.EventHandler(this.cmbChMode_Leave);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(364, 11);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(105, 23);
			this.btnClear.TabIndex = 3;
			this.btnClear.Text = "Clear all";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(1047, 11);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(75, 23);
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Visible = false;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(139, 11);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// dgvChannels
			// 
			this.dgvChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.dgvChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvChannels.Location = new System.Drawing.Point(12, 42);
			this.dgvChannels.Name = "dgvChannels";
			this.dgvChannels.ReadOnly = true;
			this.dgvChannels.RowHeadersWidth = 50;
			this.dgvChannels.RowTemplate.Height = 23;
			this.dgvChannels.Size = new System.Drawing.Size(1110, 457);
			this.dgvChannels.TabIndex = 9;
			this.dgvChannels.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChannels_RowHeaderMouseDoubleClick);
			this.dgvChannels.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.NligzloMrR);
			this.dgvChannels.SelectionChanged += new System.EventHandler(this.dgvChannels_SelectionChanged);
			// 
			// txtTxFreq
			// 
			this.txtTxFreq.InputString = null;
			this.txtTxFreq.Location = new System.Drawing.Point(845, 11);
			this.txtTxFreq.MaxByteLength = 0;
			this.txtTxFreq.Name = "txtTxFreq";
			this.txtTxFreq.Size = new System.Drawing.Size(61, 23);
			this.txtTxFreq.TabIndex = 6;
			this.txtTxFreq.Visible = false;
			this.txtTxFreq.Leave += new System.EventHandler(this.txtTxFreq_Leave);
			// 
			// txtRxFreq
			// 
			this.txtRxFreq.InputString = null;
			this.txtRxFreq.Location = new System.Drawing.Point(778, 10);
			this.txtRxFreq.MaxByteLength = 0;
			this.txtRxFreq.Name = "txtRxFreq";
			this.txtRxFreq.Size = new System.Drawing.Size(61, 23);
			this.txtRxFreq.TabIndex = 6;
			this.txtRxFreq.Visible = false;
			this.txtRxFreq.Leave += new System.EventHandler(this.CaeqgYciuW);
			// 
			// txtName
			// 
			this.txtName.InputString = null;
			this.txtName.Location = new System.Drawing.Point(711, 10);
			this.txtName.MaxByteLength = 0;
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(61, 23);
			this.txtName.TabIndex = 5;
			this.txtName.Visible = false;
			this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
			// 
			// cmbAddChMode
			// 
			this.cmbAddChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbAddChMode.FormattingEnabled = true;
			this.cmbAddChMode.Location = new System.Drawing.Point(12, 10);
			this.cmbAddChMode.Name = "cmbAddChMode";
			this.cmbAddChMode.Size = new System.Drawing.Size(109, 24);
			this.cmbAddChMode.TabIndex = 0;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.HeaderText = "Column1";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.HeaderText = "Column2";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn3
			// 
			this.dataGridViewTextBoxColumn3.HeaderText = "Column3";
			this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			this.dataGridViewTextBoxColumn3.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn4
			// 
			this.dataGridViewTextBoxColumn4.HeaderText = "Column4";
			this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
			this.dataGridViewTextBoxColumn4.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn5
			// 
			this.dataGridViewTextBoxColumn5.HeaderText = "Column5";
			this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
			this.dataGridViewTextBoxColumn5.ReadOnly = true;
			// 
			// ChannelsForm
			// 
			this.ClientSize = new System.Drawing.Size(1136, 531);
			this.Controls.Add(this.pnlChannel);
			this.Font = new System.Drawing.Font("Arial", 10F);
			this.Name = "ChannelsForm";
			this.Text = "Channels";
			this.Load += new System.EventHandler(this.ChannelsForm_Load);
			this.pnlChannel.ResumeLayout(false);
			this.pnlChannel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		[CompilerGenerated]
		private static bool smethod_0(string string_0)
		{
			return true;
		}

		static ChannelsForm()
		{
			
			ChannelsForm.SZ_HEADER_TEXT = new string[32];
			/*
			{
				"Number",
				"Name",
				"Ch Mode",
				"Rx Freq",
				"Tx Freq",
				"Power",
				"Rx Tone",
				"Tx Tone",
				"Color Code",
				"Rx Group List",
				"Contact",
				"Time Slot"
			};
			 */
		}
	}
}
