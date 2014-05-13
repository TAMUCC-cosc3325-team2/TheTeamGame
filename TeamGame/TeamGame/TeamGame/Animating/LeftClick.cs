using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame
{
    public class LeftClick : Animation
    {
        public LeftClick(Game game, Player player)
            : base(game, player, game.Content.Load<Texture2D>("art/buttonPulseSheet2"), 30, 30)
        {
            sEffect = game.Content.Load<SoundEffect>("audio/buttonBeep").CreateInstance();
            sEffect.Volume = .5f;
            base.sEffect = sEffect;
            base.hasSoundEffect = true;
        }

        public void Play(GameTime gameTime, Vector2 location)
        {
            base.pos = location;
            base.AnimationStat = Status.Playing;
            base.Draw(gameTime);
        }


    }
}
