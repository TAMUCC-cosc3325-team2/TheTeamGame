using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamGame.Puzzles
{
    /// <summary>
    /// This "team" puzzle is actually deceptively just four
    /// individual puzzles that all four teammates play at once
    /// </summary>
    class TeamCirclesInOrder : TPuzzle
    {
        byte cols = 14;
        byte rows = 8;
        byte buttonSize = 36;

        List<byte> buttons = new List<byte>(5);
        bool clickedPrev = true;

        public TeamCirclesInOrder(Game game, Player player)
            : base(game, player)
        {

        }

        public override void Initialize()
        {
            List<int> randList = new List<int>(Enumerable.Range(0, rows * cols / 4));
            for (int i = 0; i < 5; i++ )
            {
                buttons.Add((byte) randList[Game1.random.Next(0, randList.Count)]);
            }

            base.Initialize();
        }

        private Vector2 buttonPos(byte bPos)
        {
            Vector2 offset = player.TeamOf().GetRegion().Location.ToVector2();
            if (bPos > 28)
                return new Vector2(-1000, -1000);
            bPos = (byte) ((4 * bPos) + player.ID() - 1);
            return new Vector2(buttonSize * (bPos % cols) + offset.X, buttonSize * ((byte) (bPos / cols)) + offset.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Rectangle region = player.TeamOf().GetRegion();

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteBatch.Begin();
            foreach (byte b in buttons)
                spriteBatch.Draw(Game.Content.Load<Texture2D>("art/circleAnimation"), buttonPos(b), new Rectangle(0, 0, 36, 36), player.ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Encode(Lidgren.Network.NetOutgoingMessage msg)
        {
            
        }
        public override void Decode(Lidgren.Network.NetIncomingMessage msg)
        {
            
        }

        

    }
}
