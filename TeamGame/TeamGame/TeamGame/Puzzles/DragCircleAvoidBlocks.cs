using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;

namespace TeamGame.Puzzles
{
    class DragCircleAvoidBlocks : IPuzzle
    {
        Vector2 backgroundPos, ball1Pos, ball2Pos, block1Pos, block2Pos;
        Texture2D background, ball, block1, block2;

        public DragCircleAvoidBlocks(Game game, Player player)
            : base(game, player, player.GetRegion())
        {
            backgroundPos = player.GetRegion().Location.ToVector2();
            ball1Pos = backgroundPos + new Vector2(30,30);
            ball2Pos = new Vector2(player.GetRegion().Width - 30, player.GetRegion().Height - 30);
            block1Pos = new Vector2(player.GetRegion().X + player.GetRegion().Width / 3, player.GetRegion().Y + player.GetRegion().Height / 3);
            block2Pos = new Vector2(player.GetRegion().X + player.GetRegion().Width * 2 / 3, player.GetRegion().Y + player.GetRegion().Height * 2 / 3);
            this.Visible = true;

        }

        public override void Initialize()
        {

            background = Game.Content.Load<Texture2D>("art/dragCircleTest");
            ball = Game.Content.Load<Texture2D>("art/dragableBall");
            block1 = Game.Content.Load<Texture2D>("art/avoidBlockSmall");
            block2 = Game.Content.Load<Texture2D>("art/avoidBlockLarge");

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundPos, player.ColorOf());
            spriteBatch.Draw(block1, block1Pos, player.ColorOf());
            spriteBatch.Draw(block2, block2Pos, player.ColorOf());
            spriteBatch.Draw(ball, ball1Pos, player.ColorOf());
            spriteBatch.Draw(ball, ball2Pos, player.ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner
            if (Mouse.GetState().LeftButton.IsClicked())
                PuzzleOver(true);

        }

        public new void PuzzleOver(bool p)
        {
            if (p)
            {
                Game.Components.Remove(this);
                ((Net)Game.Services.GetService(typeof(Net))).pStates[this.player].puzzle = new Puzzles.NumeralSearch(Game, player);
            }
            
        }

        public override void Encode(NetOutgoingMessage msg)
        {

        }

        public override void Decode(NetIncomingMessage msg)
        {

        }
    }
}
