using Kontecg.Configuration;

namespace Kontecg.AutoMapper
{
    public class MultiLingualMapContext
    {
        public MultiLingualMapContext(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        public ISettingManager SettingManager { get; set; }
    }
}
