using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Media;

namespace DIY
{
    public class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_BG
        {
            get => this["c_DIY_BG"].ToString();
            set => this["c_DIY_BG"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_FG
        {
            get => this["c_DIY_FG"].ToString();
            set => this["c_DIY_FG"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_BUTTON_HOVER
        {
            get => this["c_DIY_BUTTON_HOVER"].ToString();
            set => this["c_DIY_BUTTON_HOVER"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_MENU_BG
        {
            get => this["c_DIY_MENU_BG"].ToString();
            set => this["c_DIY_MENU_BG"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_BRUSH_BG
        {
            get => this["c_DIY_BRUSH_BG"].ToString();
            set => this["c_DIY_BRUSH_BG"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public String c_DIY_BRUSH_FG
        {
            get => this["c_DIY_BRUSH_FG"].ToString();
            set => this["c_DIY_BRUSH_FG"] = value;
        }
    }
}
