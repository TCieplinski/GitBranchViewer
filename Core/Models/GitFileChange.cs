using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBranchViewer.Core.Models
{
    public record GitFileChange(string Status, string FileName);
}
