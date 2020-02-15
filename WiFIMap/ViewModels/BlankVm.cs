using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFIMap.ViewModels
{
    public class BlankVm : BaseVm, IProjectContainerVm
    {
        public bool IsModified => false;

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
