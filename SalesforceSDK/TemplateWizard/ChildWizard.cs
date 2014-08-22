using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateWizard
{
    public sealed class ChildWizard : IWizard
    {
        internal static Dictionary<string, string> InheritedParams;

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            foreach (var item in InheritedParams)
            {
                if (!replacementsDictionary.ContainsKey(item.Key))
                {
                    replacementsDictionary.Add(item.Key, item.Value);
                }
                else
                {
                    replacementsDictionary[item.Key] = item.Value;
                } 
            }
        }

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

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
