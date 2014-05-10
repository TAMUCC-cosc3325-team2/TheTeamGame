using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;

namespace TeamGame.Puzzles
{
    class Transition : IPuzzle
    {
        int countUpdates = 90;
        bool puzzleSuccess;
        Texture2D plusTexture, chevronTexture;
        Vector2 displacement, chevronPosition;

        public Transition(Game game, Player player, bool puzzleSuccess)
            : base(game, player)
        {
            this.puzzleSuccess = puzzleSuccess;
            this.Visible = false;

            if (puzzleSuccess)
                ((Net)Game.Services.GetService(typeof(Net))).NotifyStatusIncrease();
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
            plusTexture = Game.Content.Load<Texture2D>("art/CorrectPlus");
            chevronTexture = Game.Content.Load<Texture2D>("art/CorrectChevronRight");
            chevronPosition = drawRegion.Center.ToVector2();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!puzzleSuccess)
                return;
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
            spriteBatch.Draw(plusTexture, new Vector2(), player.ColorOf());
            spriteBatch.End();


            Rectangle temp = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = this.drawRegion;
            spriteBatch.Begin();
            spriteBatch.Draw(chevronTexture, chevronPosition + displacement, null, player.ColorOf(), player.ClockwiseAngle(), new Vector2(42,35), 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = temp;
            
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return;

            displacement *= 1.05f;

            countUpdates -= 1;

            if (countUpdates == 0)
                PuzzleOver(true);

        }

        public new void PuzzleOver(bool p)
        {
            Game.Components.Remove(this);
            byte randomPuzzle = (byte)3;//(Game1.random.Next(1, 3) * 2);
            Game1.pStates[this.player].puzzle = randomPuzzle.CreateFromID(this.Game, this.player);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write(puzzleSuccess);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            puzzleSuccess = msg.ReadBoolean();
        }
    }
}
