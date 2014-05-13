using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame.Puzzles
{
    class Transition : IPuzzle
    {
        public bool starting = false;

        public int countUpdates = 180;
        bool puzzleSuccess;
        Texture2D plusTexture, chevronTexture, fadeTexture;
        Vector2 displacement, chevronPosition;
        int fadeAlpha = 0, fadeStep = 5;
        SoundEffectInstance statusIncrease;

        public Transition(Game game, Player player, bool puzzleSuccess)
            : base(game, player)
        {
            this.puzzleSuccess = puzzleSuccess;
            
            statusIncrease = game.Content.Load<SoundEffect>("audio/statusIncrease").CreateInstance();
            statusIncrease.Volume = .5f;

            if (puzzleSuccess)
            {
                ((Net)Game.Services.GetService(typeof(Net))).NotifyStatusIncrease();
                statusIncrease.Play();
            }
                
        }
        public Transition(Game game, Player player)
            : base(game, player)
        {
            this.puzzleSuccess = false;
        }

        public override void Initialize()
        {
            // displacement = player.ClockwisePlayer().GetRegion().Center.ToVector2() - drawRegion.Center.ToVector2();
            displacement = Vector2.Transform(new Vector2(35,0), Matrix.CreateRotationZ(player.ClockwiseAngle()));
            plusTexture = Game.Content.Load<Texture2D>("art/correctPlus");
            chevronTexture = Game.Content.Load<Texture2D>("art/correctChevronRight");
            chevronPosition = drawRegion.Center.ToVector2();
            fadeTexture = Game.Content.Load<Texture2D>("art/transitionFade4");
        }

        public override void Draw(GameTime gameTime)
        {
            if (starting)
                return;
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice); // draw pulsing success/failure
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Rectangle playerRegion = player.GetRegion();
            Rectangle relative = player.GetRelativeRegion();
            spriteBatch.Draw(fadeTexture, player.GetRegion(), player.GetRelativeRegion(), puzzleSuccess?(new Color(64, 255, 64, fadeAlpha)):(new Color(255, 69, 0, fadeAlpha)));
            spriteBatch.End();

            base.Draw(gameTime); // draw status bars over fade

            if (!puzzleSuccess)
                return;
            
            // draw success plus
            if (this.countUpdates > 60)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
                spriteBatch.Draw(plusTexture, new Vector2(), player.ColorOf());
                spriteBatch.End();
            }

            
            Game.GraphicsDevice.ScissorRectangle = this.drawRegion;
            
            RasterizerState rs = new RasterizerState(); // draw success chevron
            rs.ScissorTestEnable = true;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, rs);
            spriteBatch.Draw(chevronTexture, chevronPosition + displacement, null, player.ColorOf(), player.ClockwiseAngle(), new Vector2(42,35), 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
            
        }

        public override void Update(GameTime gameTime)
        {
            if (countUpdates == 0 && Game1.pStates[player].status > 0)
                    PuzzleOver(true);
            displacement *= 1.05f;
            fadeAlpha += fadeStep;
            if (fadeAlpha >= 300)
                fadeStep *= -1;

            if (player != Game1.localPlayer)
                return;

            countUpdates -= 1;
            if (countUpdates == 130 && !this.starting)
            {
                Game.Content.Load<SoundEffect>("audio/" + player.ClockwisePlayer().ColorName()).Play();
                Game.Content.Load<SoundEffect>("audio/Status" + (puzzleSuccess ? "In" : "De") + "creased").Play();
            }
        }

        public new void PuzzleOver(bool p)
        {
            Game.Components.Remove(this);
            byte randomPuzzle = (byte)(Game1.random.Next(2,6));
            Game1.pStates[this.player].puzzle = randomPuzzle.CreateFromID(this.Game, this.player);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write(puzzleSuccess);
            msg.Write(starting);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            puzzleSuccess = msg.ReadBoolean();
            starting = msg.ReadBoolean();
        }
    }
}
