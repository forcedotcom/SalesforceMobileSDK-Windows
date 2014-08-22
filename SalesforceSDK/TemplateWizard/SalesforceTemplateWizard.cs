using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TemplateWizard
{
    class SalesforceTemplateWizard : IWizard
    {
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            try
            {
                TemplateForm window = new TemplateForm();
                window.ShowDialog();
                PopulateReplacementDictionary(window);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void PopulateReplacementDictionary(TemplateForm window)
        {
            Dictionary<string, string> replacementsDictionary = new Dictionary<string,string>();

            if (!String.IsNullOrEmpty(window.ClientID.Text))
            {
                replacementsDictionary.Add("$ClientID$", window.ClientID.Text);
            }

            if (!String.IsNullOrEmpty(window.CallbackURL.Text))
            {
                replacementsDictionary.Add("$CallbackURL$", window.CallbackURL.Text);
            }

            if (!String.IsNullOrEmpty(window.EncryptionPassword.Text))
            {
                replacementsDictionary.Add("$EncryptionPassword$", window.EncryptionPassword.Text);
            }

            if (!String.IsNullOrEmpty(window.EncryptionSalt.Text))
            {
                replacementsDictionary.Add("$EncryptionSalt$", window.EncryptionSalt.Text);
            }

            if (window.Scopes.Text != null)
            {
                String[] scopes = window.Scopes.Text.Split(',');
                StringBuilder sb = new StringBuilder();
                int max = scopes.Length;
                int count = 1;
                foreach (String next in scopes)
                {
                    sb.Append("\"").Append(next.Trim()).Append("\"");
                    if (count < max)
                    {
                        sb.Append(", ");
                    }
                    count++;
                }
                replacementsDictionary.Add("$scopes$", sb.ToString());
            }
            ChildWizard.InheritedParams = replacementsDictionary;
        }
    }
}
