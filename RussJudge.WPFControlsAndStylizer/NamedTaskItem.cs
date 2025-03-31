using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RussJudge.WPFControlsAndStylizer
{
    public class NamedTaskItem(string description, Task task, CancellationTokenSource cancelTokenSource)
    {
        public string Description { get; set; } = description;
        public Task Task { get; set; } = task;
        public CancellationTokenSource CancelTokenSource { get; set; } = cancelTokenSource;

    }
}
