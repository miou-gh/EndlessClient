﻿using EOLib.Domain.Character;
using EOLib.IO.Map;
using Optional;
using System.Collections.Generic;

namespace EOLib.Domain.Map
{
    public interface IMapCellState
    {
        bool InBounds { get; }

        MapCoordinate Coordinate { get;  }

        IReadOnlyList<IItem> Items { get; }

        TileSpec TileSpec { get; }

        Option<NPC.NPC> NPC { get; }

        Option<Character.Character> Character { get; }
        
        Option<ChestKey> ChestKey { get; }

        Option<IWarp> Warp { get; }

        Option<ISign> Sign { get; }
    }
}
