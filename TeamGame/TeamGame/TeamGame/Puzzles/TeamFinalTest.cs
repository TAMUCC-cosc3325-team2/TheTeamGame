using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;

namespace TeamGame.Puzzles
{
    public class TeamFinalTest : IPuzzle
    {
        Rectangle testArea, circleLarge, circleSmall, teamAverage;

        Color team1, team2, team3, team4;

        Vector2 largeDirection, smallDirection;

        int largeVelocity, smallVelocity;

        Texture2D large, small, average;

        float randNum;

        /// <summary>
        /// Final team puzzle. Welcome to the end.
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        public TeamFinalTest(Game game, Player player)
            : base(game, player)
        {
            testArea = new Rectangle(0, 0, 1024, 724);

            largeVelocity = 1;
            smallVelocity = 1;

            randNum = Game1.random.Next(0, 359);
            largeDirection = new Vector2(3 * (int)Math.Cos(MathHelper.ToRadians(randNum)), 3 * (int)Math.Sin(MathHelper.ToRadians(randNum)));
            randNum = Game1.random.Next(0, 359);
            smallDirection = new Vector2(3 * (float)Math.Cos(MathHelper.ToRadians(randNum)), 3 * (float)Math.Sin(MathHelper.ToRadians(randNum)));
            circleLarge = new Rectangle(testArea.Center.X - 268/2, testArea.Center.Y - 268/2, 268, 268);
            circleSmall = new Rectangle(testArea.Center.X - 120/2, testArea.Center.Y - 120/2, 120, 120);
            
        }

        public override void Initialize()
        {
            large = Game.Content.Load<Texture2D>("art/bigCircle");
            small = Game.Content.Load<Texture2D>("art/bigCircle");
            average = Game.Content.Load<Texture2D>("art/averagePosition");
            
        }

        public override void Draw(GameTime gameTime)
        {
            //No healthbar in final test
            //base.Draw(gameTime); // draw healthbar

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(large, circleLarge, Color.White);
            spriteBatch.Draw(small, circleSmall, Color.White);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return;

            if (circleLarge.Top <= testArea.Top)
                largeDirection.Y = -largeDirection.Y;
            else if (circleLarge.Bottom >= testArea.Bottom)
                largeDirection.Y = -largeDirection.Y;
            if (circleLarge.Right >= testArea.Right)
                largeDirection.X = -largeDirection.X;
            else if (circleLarge.Left <= testArea.Left)
                largeDirection.X = -largeDirection.X;

            Vector2 temp = circleLarge.Location.ToVector2() + largeDirection;
            circleLarge.X = (int)temp.X;
            circleLarge.Y = (int)temp.Y;
            //circleSmall.X += (int)smallDirection.X;
            //circleSmall.Y += (int)smallDirection.Y;
        }

        public new void PuzzleOver(bool p)
        {

        }

        public override void Encode(NetOutgoingMessage msg)
        {

        }

        public override void Decode(NetIncomingMessage msg)
        {

        }
    }
}
