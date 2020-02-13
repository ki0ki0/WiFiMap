using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFIMap.Interfaces;
using WiFIMap.Model;

namespace WiFIMap.ViewModels
{
    public class ResultVm : BaseVm, IProjectContainer
    {
        public ResultVm(IProject project)
        {
            CurrentProject = project;CurrentProject.ProjectChanged += CurrentProjectOnProjectChanged;
        }

        private void CurrentProjectOnProjectChanged(object sender, EventArgs e)
        {
            
        }

        public IProject CurrentProject { get; }
    }
}
