using SafriSoftv1._3.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class Setting: BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public SettingType SettingType { get; set; }
    }
}