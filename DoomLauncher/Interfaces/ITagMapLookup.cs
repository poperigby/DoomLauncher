﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface ITagMapLookup
    {
        void Refresh();
        ITagData[] GetTags(IGameFile gameFile);
    }
}
