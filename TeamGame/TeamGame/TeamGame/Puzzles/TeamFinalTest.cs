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
    public class TeamFinalTest : IPuzzle
    {
        Rectangle testArea;
        Vector2 largeVelocity, smallVelocity, largePosition, smallPosition, averagePosition;
        int largeRadius = 134, smallRadius = 60;
        int score, countUpdates;
        Texture2D largeTexture, smallTexture, averageTexture;
        SpriteFont scoreFont;

        /// <summary>
        /// Final team puzzle. Welcome to the end.
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        public TeamFinalTest(Game game, Player player)
            : base(game, player)
        {
            testArea = new Rectangle(0, 0, 1024, 724);

            smallVelocity = new Vector2((float)Game1.random.NextDouble() - 0.5f, (float)Game1.random.NextDouble() - 0.5f);
            smallVelocity.Normalize();
            largeVelocity = new Vector2((float)Game1.random.NextDouble() - 0.5f, (float)Game1.random.NextDouble() - 0.5f);
            largeVelocity.Normalize();

            largePosition = new Vector2(testArea.Center.X, testArea.Center.Y);
            smallPosition = new Vector2(0, 0);
            averagePosition = new Vector2(0, 0);

        }

        public override void Initialize()
        {
            largeTexture = Game.Content.Load<Texture2D>("art/bigCircle");
            smallTexture = Game.Content.Load<Texture2D>("art/smallCircle");
            averageTexture = Game.Content.Load<Texture2D>("art/averagePosition");
            scoreFont = Game.Content.Load<SpriteFont>("ArmyHollow");

            PlayerExtensions.individualColors = false;
            System.Windows.Forms.Cursor myCursor = Extensions.LoadCustomCursor("Content/cursors/cursor" + Game1.localPlayer.ColorName() + ".cur");
            System.Windows.Forms.Form winForm = ((System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Game.Window.Handle));
            winForm.Cursor = myCursor;
            

        }

        public override void Draw(GameTime gameTime)
        {
            //No healthbar in final test
            //base.Draw(gameTime); // draw healthbar

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(largeTexture, largePosition, null, Color.White, 0, new Vector2(largeRadius, largeRadius), new Vector2(1), SpriteEffects.None, 0.5f);
            spriteBatch.Draw(smallTexture, smallPosition + largePosition, null, Color.White, 0, new Vector2(smallRadius, smallRadius), new Vector2(smallRadius/60), SpriteEffects.None, 0.5f);
            spriteBatch.Draw(averageTexture, averagePosition, player.TeamOf().ColorOf());
            spriteBatch.DrawString(scoreFont, score.ToString(), Vector2.Zero, player.TeamOf().ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            #region UpdateAverage
            if (player.ID() != Game1.localPlayer.ID()) // only update averageTexture once per team
                return;
            averagePosition = new Vector2(-9, -9);
            foreach (Player p in player.TeamList())
                averagePosition += Game1.pStates[p].cursorPosition * .25f;
            #endregion

            if (player != Game1.localPlayer)
                return;

            if ((new Vector2(averagePosition.X - averageTexture.Width / 2, averagePosition.Y - averageTexture.Height / 2) - largePosition).LengthSquared() <= smallRadius * smallRadius)
            {
                if (countUpdates % 1 == 0)
                {
                    score++;
                }
            }

            largePosition += largeVelocity;
            #region BounceLargeCircle
            if (largePosition.Y - largeRadius <= testArea.Top)
            {
                largePosition.Y += (testArea.Top - largePosition.Y + largeRadius);
                largeVelocity.Y *= -1;
            }
            else if (largePosition.Y + largeRadius >= testArea.Bottom)
            {
                largePosition.Y -= (largePosition.Y + largeRadius - testArea.Bottom);
                largeVelocity.Y *= -1;
            }
            if (largePosition.X + largeRadius >= testArea.Right)
            {
                largePosition.X -= (largePosition.X + largeRadius - testArea.Right);
                largeVelocity.X *= -1;
            }
            else if (largePosition.X - largeRadius <= testArea.Left)
            {
                largePosition.X += (testArea.Left- largePosition.X + largeRadius);
                largeVelocity.X *= -1;
            }
            #endregion
            
            Vector2 lastPosition = smallPosition.Plus(0,0);
            //smallPosition += smallVelocity;
            /*#region BounceSmallCircle
            if (smallPosition.LengthSquared() > largeRadius * largeRadius)
            {
                Vector2 exit = 0.5f * (lastPosition + smallPosition);
                exit *= largeRadius / exit.Length();
                smallPosition = exit;
                float projection = 2 * Vector2.Dot(exit, smallVelocity) / (largeRadius * largeRadius);
                smallVelocity -= 2 * projection * exit;
            }
            #endregion */
            if (countUpdates == int.MaxValue)
                countUpdates = 0;
            countUpdates++;
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
