﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

using System;
using EndlessClient.Rendering.CharacterProperties;
using EndlessClient.Rendering.Map;
using EOLib.Domain.Character;
using EOLib.Domain.Map;
using EOLib.Graphics;
using EOLib.IO.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EndlessClient.Rendering.MapEntityRenderers
{
    public class MapObjectLayerRenderer : BaseMapEntityRenderer
    {
        private readonly INativeGraphicsManager _nativeGraphicsManager;
        private readonly ICurrentMapProvider _currentMapProvider;

        public override MapRenderLayer RenderLayer
        {
            get { return MapRenderLayer.Objects; }
        }

        protected override int RenderDistance
        {
            get { return 22; }
        }

        public MapObjectLayerRenderer(INativeGraphicsManager nativeGraphicsManager,
                                      ICurrentMapProvider currentMapProvider,
                                      ICharacterProvider characterProvider,
                                      ICharacterRenderOffsetCalculator characterRenderOffsetCalculator)
            : base(characterProvider, characterRenderOffsetCalculator)
        {
            _nativeGraphicsManager = nativeGraphicsManager;
            _currentMapProvider = currentMapProvider;
        }

        public override void RenderElementAt(SpriteBatch spriteBatch, int row, int col, int alpha)
        {
            int gfxNum;
            if ((gfxNum = MapFile.GFX[MapLayer.Objects][row, col]) <= 0)
                return;

            //todo: handling for spike traps when players walk over them: see OldMapRenderer._drawMapObjectsAtLoc

            var gfx = _nativeGraphicsManager.TextureFromResource(GFXTypes.MapObjects, gfxNum, true);
            var pos = GetDrawCoordinatesFromGridUnits(col, row);
            pos = new Vector2(pos.X - (int)Math.Round(gfx.Width / 2.0) + 29, pos.Y - (gfx.Height - 28));
            spriteBatch.Draw(gfx, pos, Color.FromNonPremultiplied(255, 255, 255, alpha));
        }

        private IReadOnlyMapFile MapFile { get { return _currentMapProvider.CurrentMap; } }
    }
}
