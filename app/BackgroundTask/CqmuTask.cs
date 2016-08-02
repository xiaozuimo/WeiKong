using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileUpdateService;
using Windows.ApplicationModel.Background;

namespace BackgroundTask
{
    public sealed class CqmuTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            await TileUpdate.Update();
            deferral.Complete();
        }
    }
}
