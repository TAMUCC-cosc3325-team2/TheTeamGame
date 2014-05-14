using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace TeamGame.Puzzles
{
    class StartingGame : IPuzzle
    {
        SoundEffectInstance intro;
        public StartingGame(Game game, Player player)
            : base(game, player){}

        public override void Initialize()
        {
            if (player.IsLocal())
            {
                intro = Game.Content.Load<SoundEffect>("audio/Intro" + Game1.localPlayer.ColorName()).CreateInstance();
                intro.Play();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (player.IsLocal() && intro.State != SoundState.Playing)
                PuzzleOver(true);
        }

        public override void Encode(NetOutgoingMessage msg)
        {

        }

        public override void Decode(NetIncomingMessage msg)
        {

        }

        public override void PuzzleOver(bool Correct)
        {
            Game.Components.Remove(this);
            Game1.pStates[this.player].status = 12;
            Game1.gameDifficulty = 0.004;
            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player);
            ((Puzzles.Transition)Game1.pStates[this.player].puzzle).starting = true;
            ((Puzzles.Transition)Game1.pStates[this.player].puzzle).countUpdates = 60;
            ((Net)Game.Services.GetService(typeof(Net))).clock = new TimeSpan();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(Game.Content.Load<Texture2D>("art/playerBorder"), player.GetRegion().Location.ToVector2() + new Vector2(-4, -4), player.ColorOf());
            spriteBatch.End();
        }
    }
}
