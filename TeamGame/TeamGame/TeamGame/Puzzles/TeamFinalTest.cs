using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame.Puzzles
{
    public class TeamFinalTest : IPuzzle
    {
        static Dictionary<Team, int> teamScores = new Dictionary<Team,int>{{Team.t1, 0}, {Team.t2, 0}, {Team.t3, 0}, {Team.t4, 0}};
        Rectangle testArea, circleLarge, circleSmall;
        Vector2 averagePosition;
        int largeRadius, smallRadius;
        int countUpdates = 0;
        Texture2D largeTexture, smallTexture, averageTexture;
        SpriteFont scoreFont;
        Vector2 largeDirection, smallDirection;

        MouseState mouse, prevMouse;

        /// <summary>
        /// Final team puzzle. Welcome to the end.
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        public TeamFinalTest(Game game, Player player)
            : base(game, player)
        {
            testArea = new Rectangle(0, 0, 1024, 724);

            //largePosition = new Vector2(testArea.Center.X, testArea.Center.Y);
            //smallPosition = new Vector2(0, 0);
            averagePosition = new Vector2(0, 0);

            largeDirection = new Vector2(2 * (float)Math.Cos(MathHelper.ToRadians(30)), 2 * (float)Math.Sin(MathHelper.ToRadians(30)));
            smallDirection = new Vector2(2 * (float)Math.Cos(MathHelper.ToRadians(278)), 2 * (float)Math.Sin(MathHelper.ToRadians(278)));
            circleLarge = new Rectangle(testArea.Center.X - 268 / 2, testArea.Center.Y - 268 / 2, 268, 268);
            circleSmall = new Rectangle(testArea.Center.X - 120 / 2, testArea.Center.Y - 120 / 2, 120, 120);

            foreach (Player p in Enum.GetValues(typeof(Player)))
                if (p != Player.None)
                    teamScores[p.TeamOf()] += Game1.pStates[p].score;

            smallRadius = circleSmall.Width / 2 + 10;
            largeRadius = circleLarge.Width / 2;
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
            if (!player.IsLocal())
                return;

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(largeTexture, circleLarge, Color.White);
            spriteBatch.Draw(largeTexture, circleSmall, Color.White);
            spriteBatch.Draw(averageTexture, averagePosition - new Vector2(9, 9), player.ColorOf());
            spriteBatch.DrawString(scoreFont, teamScores[Team.t1].ToString(), new Vector2(12, 12), Player.t1p1.ColorOf());
            spriteBatch.DrawString(scoreFont, teamScores[Team.t2].ToString(), new Vector2(1012 - scoreFont.MeasureString(teamScores[Team.t2].ToString()).X, 12), Player.t2p1.ColorOf());
            spriteBatch.DrawString(scoreFont, teamScores[Team.t3].ToString(), new Vector2(12, 712 - scoreFont.MeasureString(teamScores[Team.t2].ToString()).Y), Player.t3p1.ColorOf());
            spriteBatch.DrawString(scoreFont, teamScores[Team.t4].ToString(), new Vector2(1012 - scoreFont.MeasureString(teamScores[Team.t2].ToString()).X, 712 - scoreFont.MeasureString(teamScores[Team.t2].ToString()).Y), Player.t4p1.ColorOf());
            spriteBatch.End();
           
        }
            
        public override void Update(GameTime gameTime)
        {
            #region UpdateAverage
            if (player.ID() != Game1.localPlayer.ID()) // only update averageTexture once per team
                return;
            List<Player> teamInGame = new List<Player>();
            foreach (Player p in player.TeamList())
                if (Game1.pStates[p].puzzle is Puzzles.TeamFinalTest)
                    teamInGame.Add(p);
            averagePosition = new Vector2(0,0);
            foreach (Player p in teamInGame)
                averagePosition += Game1.pStates[p].cursorPosition / teamInGame.Count;
            #endregion

            if (player != Game1.localPlayer)
                return;

            mouse = Mouse.GetState();

            #region increase score
            if ((new Vector2(averagePosition.X - averageTexture.Width / 2, averagePosition.Y - averageTexture.Height / 2) - new Vector2(circleSmall.Center.X, circleSmall.Center.Y)).LengthSquared() <= (smallRadius * smallRadius))
            {
                /*if (countUpdates % 1 == 0)
                {
                    switch (player.TeamOf())
                    {
                        case Team.t1:
                            score1++;
                            break;
                        case Team.t2:
                            score2++;
                            break;
                        case Team.t3:
                            score3++;
                            break;
                        case Team.t4:
                            score4++;
                            break;
                    }
                }*/
                bool scoring = true;
                foreach (Player p in teamInGame)
                    if ((Game1.pStates[p].cursorPosition - circleLarge.Center.ToVector2()).LengthSquared() < largeRadius * largeRadius)
                        scoring = false;
                if (scoring)
                    teamScores[this.player.TeamOf()]++;
            }
            #endregion

            #region bounce large circle
            //if the large circle collides with the top or bottom of the test area,
            //flip the large circle's y-velocity
            if (circleLarge.Top <= testArea.Top)
                largeDirection.Y = -largeDirection.Y;
            else if (circleLarge.Bottom >= testArea.Bottom)
                largeDirection.Y = -largeDirection.Y;

            //if the large circle collides with the left or right of the test area,
            //flip the large circle's x-velocity
            if (circleLarge.Right >= testArea.Right)
                largeDirection.X = -largeDirection.X;
            else if (circleLarge.Left <= testArea.Left)
                largeDirection.X = -largeDirection.X;
            #endregion

            #region bounce small circle
            //vectors to represent the center of both circles
            Vector2 lCircle = new Vector2(circleLarge.Center.X, circleLarge.Center.Y);
            Vector2 sCircle = new Vector2(circleSmall.Center.X, circleSmall.Center.Y);

            //if (difference_between_centers_of_circles^2 >= radius^2) then collision has happened
            if ((lCircle - sCircle).LengthSquared() >= smallRadius * smallRadius)
            {
                //flip the small circle's velocity
                smallDirection = -smallDirection;
            }
            #endregion

            #region update circles' positions
            //vector to determine new location of the large circle
            Vector2 vtemp = new Vector2(circleLarge.Location.X, circleLarge.Location.Y) + largeDirection;
            //vector to move the small circle with respect to the large circle
            Vector2 vtemp2 = new Vector2(circleSmall.Location.X, circleSmall.Location.Y) + largeDirection;

            //cast vector to int required by rectangle location
            circleLarge.X = (int)vtemp.X;
            circleLarge.Y = (int)vtemp.Y;
            circleSmall.X = (int)vtemp2.X;
            circleSmall.Y = (int)vtemp2.Y;
            //move small circle based by it's velocity
            circleSmall.X += (int)smallDirection.X;
            circleSmall.Y += (int)smallDirection.Y;
            #endregion

            prevMouse = mouse;
            countUpdates++;
        }

        public new void PuzzleOver(bool p)
        {

        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write((Int32) teamScores[this.player.TeamOf()]);
            msg.Write((Int32) countUpdates);
            msg.Write((short) this.circleLarge.Location.X);
            msg.Write((short) this.circleLarge.Location.Y);
            msg.Write((short)this.circleSmall.Location.X);
            msg.Write((short)this.circleSmall.Location.Y);
            msg.Write(largeDirection.X);
            msg.Write(largeDirection.Y);
            msg.Write(smallDirection.X);
            msg.Write(smallDirection.Y);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            int temp = msg.ReadInt32();
            if (temp > teamScores[player.TeamOf()])
                teamScores[this.player.TeamOf()] = temp;
            temp = msg.ReadInt32();
            TeamFinalTest LPuzzle = ((TeamFinalTest)Game1.pStates[Game1.localPlayer].puzzle);
            if (LPuzzle.countUpdates < temp)
            {
                LPuzzle.countUpdates = temp;
                LPuzzle.circleLarge.Location = new Point(msg.ReadInt16(), msg.ReadInt16());
                LPuzzle.circleSmall.Location = new Point(msg.ReadInt16(), msg.ReadInt16());
                LPuzzle.largeDirection.X = msg.ReadFloat();
                LPuzzle.largeDirection.Y = msg.ReadFloat();
                LPuzzle.smallDirection.X = msg.ReadFloat();
                LPuzzle.smallDirection.Y = msg.ReadFloat();
            }
        }
    }
}
