﻿using System;
using System.Windows.Forms;
using GitCommands;

namespace GitUI
{
    public partial class FormCherryPickCommitSmall : GitExtensionsForm
    {
        private bool IsMerge;
        public FormCherryPickCommitSmall(GitRevision revision)
        {
            this.Revision = revision;
            InitializeComponent();

            Translate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IsMerge = GitCommandHelpers.IsMerge(Revision.Guid);
            if (IsMerge)
            {
                GitRevision[] Parents = GitCommandHelpers.GetParents(Revision.Guid);
                for (int i = 0; i < Parents.Length; i++)
                {
                    ParentsList.Items.Add(i + 1 + "");
                    ParentsList.Items[ParentsList.Items.Count - 1].SubItems.Add(Parents[i].Message);
                    ParentsList.Items[ParentsList.Items.Count - 1].SubItems.Add(Parents[i].Author);
                    ParentsList.Items[ParentsList.Items.Count - 1].SubItems.Add(Parents[i].CommitDate.ToShortDateString());
                }
                ParentsList.TopItem.Selected = true;
            }
            else
            {
                this.ParentsList.Visible = false;
                this.ParentsLabel.Visible = false;
                this.Height = this.Height - (ParentsList.Height + ParentsLabel.Height);
                this.Pick.Location = new System.Drawing.Point(this.Pick.Location.X,
                    this.Pick.Location.Y - (ParentsList.Height + ParentsLabel.Height));
                this.AutoCommit.Location = new System.Drawing.Point(this.AutoCommit.Location.X,
                    this.AutoCommit.Location.Y - (ParentsList.Height + ParentsLabel.Height));
            }

        }

        public GitRevision Revision { get; set; }

        private void FormCherryPickCommitSmall_Load(object sender, EventArgs e)
        {
            Commit.Text = string.Format("Commit: {0}", Revision.Guid);
            Author.Text = string.Format("Author: {0}", Revision.Author);
            Date.Text = string.Format("Commit date: {0}", Revision.CommitDate);
            Message.Text = string.Format("Message: {0}", Revision.Message);
        }

        private void Revert_Click(object sender, EventArgs e)
        {
            string arguments = "";
            bool CanExecute = true;
            if (IsMerge)
            {
                if (ParentsList.SelectedItems.Count == 0)
                {
                    MessageBox.Show("None parent is selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CanExecute = false;
                }                  
                else
                {
                    arguments = "-m " + (ParentsList.SelectedItems[0].Index + 1);
                }
            }
            if (CanExecute)
            {
                new FormProcess(GitCommandHelpers.CherryPickCmd(Revision.Guid, AutoCommit.Checked, arguments)).ShowDialog();
                MergeConflictHandler.HandleMergeConflicts();
                Close();
            }            
        }
    }
}
