using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
    public class RightSidePanel : Panel
    {
        private Timeline timeline;

        private Label title;
        private ListBox markpointBox;
        private Button addButton;
        private Button removeButton;

        /**
         * Special text box that is normally hidden, but shows to let the user edit the description of a markpoint.
         */
        private TextBox editBox;

        public RightSidePanel(Timeline timeline)
        {
            this.timeline = timeline;

            title = new Label();
            title.Text = "Markpoints";
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);

            markpointBox = new ListBox();
            markpointBox.KeyPress += (obj, e) =>
            {
                if (e.KeyChar == 13)
                    CreateEditBox();
            };
            markpointBox.KeyDown += (obj, e) =>
            {
                if (e.KeyCode == Keys.F2)
                    CreateEditBox();
            };
            markpointBox.MouseDoubleClick += (obj, e) =>
            {
                int index = markpointBox.IndexFromPoint(e.Location);
                if (index != System.Windows.Forms.ListBox.NoMatches)
                {
                    // Jump to the markpoint on the timeline.
                    MarkPoint markpoint = (MarkPoint)markpointBox.Items[index];

                    timeline.CurrentPos = markpoint.Frameid;
                }
            };
            this.Controls.Add(markpointBox);

            addButton = new Button();
            addButton.Text = "Add";
            addButton.Click += AddMarkpoint;
            this.Controls.Add(addButton);

            removeButton = new Button();
            removeButton.Text = "Remove";
            removeButton.Click += (obj, e) =>
            {
                if (timeline.Recording == null)
                    return;

                // Get the selected items.
                MarkPoint markPoint = (MarkPoint)markpointBox.SelectedItem;
                timeline.Recording.RemoveMarkPoint(markPoint);
                markpointBox.Items.Remove(markPoint);
            };
            this.Controls.Add(removeButton);

            // Create the edit box.
            editBox = new TextBox();
            editBox.Left = 0;
            editBox.Top = 0;
            editBox.Size = new Size(0, 0);
            editBox.Hide();
            markpointBox.Controls.Add(editBox);
            editBox.Text = "";
            editBox.BackColor = Color.Beige;
            editBox.ForeColor = Color.Blue;
            editBox.BorderStyle = BorderStyle.FixedSingle;
            editBox.KeyPress += (obj, e) =>
            {
                if (e.KeyChar == 13)
                {
                    int index = markpointBox.SelectedIndex;
                    MarkPoint markPoint = (MarkPoint)markpointBox.Items[index];
                    markPoint.Description = editBox.Text;
                    editBox.Hide();

                    markpointBox.DisplayMember = DateTime.UtcNow.ToString();
                }

            };
            editBox.LostFocus += (obj, e) =>
            {
                int index = markpointBox.SelectedIndex;
                MarkPoint markPoint = (MarkPoint)markpointBox.Items[index];
                markPoint.Description = editBox.Text;
                editBox.Hide();

                markpointBox.DisplayMember = DateTime.UtcNow.ToString();
            };

            this.Resize += resize;
        }

        public void AddMarkpoint(object sender, EventArgs e)
        {
            if (timeline.Recording == null)
                return;

            // Insert the markpoint with an automatic description.
            int id = timeline.Recording.MarkpointId;
            MarkPoint newPoint = new MarkPoint(id, "Markpoint #" + (id + 1), timeline.CurrentPos);
            timeline.Recording.AddMarkPoint(newPoint);
            int index = timeline.Recording.MarkPoints.IndexOf(newPoint);
            markpointBox.Items.Insert(index, newPoint);
        }

        private void CreateEditBox()
        {
            int itemSelected = markpointBox.SelectedIndex;
            Rectangle r = markpointBox.GetItemRectangle(itemSelected);
            string itemText = ((MarkPoint)markpointBox.Items[itemSelected]).Description;

            editBox.Left = r.X;// + delta;
            editBox.Top = r.Y;// + delta;
            editBox.Width = r.Width - 10;
            editBox.Height = r.Height;// - delta;
            editBox.Show();

            markpointBox.Controls.Add(editBox);
            editBox.Text = itemText;
            editBox.Focus();
            editBox.SelectAll();
        }

        private void resize(Object sender, EventArgs e)
        {
            Size newSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            title.Left = 0;
            title.Top = 0;
            title.Width = newSize.Width;
            title.Height = 20;

            markpointBox.Left = 0;
            markpointBox.Top = 20 + Window.ControlMargin;
            markpointBox.Width = newSize.Width;
            markpointBox.Height = newSize.Height - 40 - 2 * Window.ControlMargin;

            addButton.Top = markpointBox.Bottom + Window.ControlMargin;
            addButton.Left = Window.ControlMargin;
            addButton.Width = (newSize.Width - 3 * Window.ControlMargin) / 2;
            addButton.Height = 20;

            removeButton.Top = markpointBox.Bottom + Window.ControlMargin;
            removeButton.Left = addButton.Right + Window.ControlMargin;
            removeButton.Width = (newSize.Width - 3 * Window.ControlMargin) / 2;
            removeButton.Height = 20;
        }

        public void Reset()
        {
            // Clear out the markpoints.
            markpointBox.Items.Clear();
        }
    }
}
